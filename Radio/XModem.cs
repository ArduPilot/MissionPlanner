﻿using MissionPlanner.Comms;
using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace MissionPlanner.Radio
{
    public class XModem
    {
        public delegate void LogEventHandler(string message, int level = 0);

        public delegate void ProgressEventHandler(double completed);

        public static event LogEventHandler LogEvent;
        public static event ProgressEventHandler ProgressEvent;

        const byte SOH = 0x01;
        const byte EOT = 0x04;
        const byte ACK = 0x06;
        const byte NAK = 0x15;
        const byte CAN = 0x18;
        const byte C = 0x43;

        public static UInt16 CRC_calc(byte[] data, int size)
        {
            UInt16 crc = 0x0;
            UInt16[] crc_list = new UInt16[128];
            int index;

            for (index = 0; index < size; index++)
            {
                crc = (UInt16)((crc >> 8) | (crc << 8));
                crc ^= data[index];
                crc ^= (UInt16)((crc & 0xff) >> 4);
                crc ^= (UInt16)(crc << 12);
                crc ^= (UInt16)((crc & 0xff) << 5);
                crc_list[index] = crc;
            }
            return crc;
        }

        public static void SendBlock(FileStream fs, ICommsSerial Serial, int bNumber)
        {
            byte[] packet = new byte[133];
            byte[] bits = new byte[128];
            UInt16 CRC = 0;

            for (int i = 0; i < bits.Length; i++) { bits[i] = 0x26; }

            packet[0] = SOH;
            packet[1] = (byte)(bNumber % 256);
            packet[2] = (byte)(255 - (bNumber % 256));

            var bytesRead = fs.Read(bits, 0, bits.Length);

            if (bytesRead == bits.Length)
            {
                CRC = CRC_calc(bits, 128);
                System.Buffer.BlockCopy(bits, 0, packet, 3, 128);
                packet[131] = (byte)(CRC >> 8);
                packet[132] = (byte)(CRC);
                Serial.Write(packet, 0, packet.Length);
            }
            else if (bytesRead > 0)
            {
                CRC = CRC_calc(bits, 128);
                System.Buffer.BlockCopy(bits, 0, packet, 3, 128);
                packet[131] = (byte)(CRC >> 8);
                packet[132] = (byte)(CRC);
                Serial.Write(packet, 0, packet.Length);
                Serial.Write(new byte[] { EOT }, 0, 1);
                ProgressEvent?.Invoke(100);
            }
            else if (bytesRead == 0)
            {
                Serial.Write(new byte[] { EOT }, 0, 1);
                ProgressEvent?.Invoke(100);
            }
        }

        public static void Upload(string firmwarebin, ICommsSerial comPort)
        {
            comPort.ReadTimeout = 2000;

            using (var fs = new FileStream(firmwarebin, FileMode.Open))
            {
                var len = (int)fs.Length;
                len = (len % 128) == 0 ? len / 128 : (len / 128) + 1;
                var startlen = len;

                int a = 1;
                int NoAckCount = 0;
                while (len > 0)
                {
                    LogEvent?.Invoke("Uploading block " + a + "/" + startlen);

                    SendBlock(fs, comPort, a);
                    // responce ACK
                    var ack = comPort.ReadByte();
                    while (ack == 'C')
                        ack = comPort.ReadByte();

                    if (ack == ACK)
                    {
                        NoAckCount = 0;
                        len--;
                        a++;

                        ProgressEvent?.Invoke(1 - ((double)len / (double)startlen));
                    }
                    else if (ack == NAK)
                    {
                        MsgBox.CustomMessageBox.Show("Corrupted packet. Please power cycle and try again.\r\n", "Warning",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        len = 0;
                    }
                    else
                    {
                        NoAckCount++;
                        if (NoAckCount >= 10)
                        {
                        }
                    }
                }
            }

            // boot
            Thread.Sleep(100);
            comPort.Write("\r\n");
            Thread.Sleep(100);
            comPort.Write("BOOTNEW\r\n");
            Thread.Sleep(100);
        }
    }
}
