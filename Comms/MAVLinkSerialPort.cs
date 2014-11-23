﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using log4net;
using System.Threading;

namespace MissionPlanner.Comms
{
    /// <summary>
    /// this is a proxy port for SERIAL_CONTROL messages
    /// </summary>
    public class MAVLinkSerialPort : ICommsSerial
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        MAVLinkInterface mavint;

        static KeyValuePair<MAVLink.MAVLINK_MSG_ID, Func<byte[], bool>> subscription;

        uint baud = 0;

        ushort timeout = 10;

        public MAVLink.SERIAL_CONTROL_DEV port = MAVLink.SERIAL_CONTROL_DEV.TELEM1;

        CircularBuffer.CircularBuffer<byte> buffer = new CircularBuffer.CircularBuffer<byte>(4096);

        Thread bgdata;

        public int BaudRate
        {
            set
            {
                log.Info("MAVLinkSerialPort baudrate " + value);
                mavint.SendSerialControl(port, timeout, null, (uint)value);
                System.Threading.Thread.Sleep(500);
                baud = (uint)value;
            }
            get { return (int)baud; }
        }

        public MAVLinkSerialPort(MAVLinkInterface mavint, MAVLink.SERIAL_CONTROL_DEV port)
        {
            this.mavint = mavint;
            this.port = port;

            if (!mavint.BaseStream.IsOpen)
            {
                mavint.BaseStream.Open();
            }

            if (mavint.getHeartBeat().Length == 0)
            {
                throw new Exception("No valid heartbeats read from port");
            }

            if (subscription.Value != null)
                mavint.UnSubscribeToPacketType(subscription);
            
            subscription = mavint.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.SERIAL_CONTROL, ReceviedPacket,true);

            bgdata = new Thread(mainloop);
            bgdata.Name = "MAVLinkSerialPort";
            bgdata.IsBackground = true;
            bgdata.Start();
        }

        private void mainloop(object obj)
        {
            try
            {
                while (true)
                {
                    GetData();
                    System.Threading.Thread.Sleep(5);
                }
            }
            catch { }
        }

        ~MAVLinkSerialPort()
        {
            log.Info("Destroy");

            if (bgdata != null && bgdata.IsAlive)
                bgdata.Abort();
            //mavint.UnSubscribeToPacketType(subscription);
        }

        DateTime packetcounttimer = DateTime.MinValue;
        int packetcount = 0;
        int packetwithdata = 0;

        bool ReceviedPacket(byte[] packet)
        {
            if (packetcounttimer.Second != DateTime.Now.Second)
            {
                log.Info("packet count " + packetcount + " with data " + packetwithdata + " " + buffer.Size);
                packetcount = 0;
                packetwithdata = 0;
                packetcounttimer = DateTime.Now;
            }

            packetcount++;

            MAVLink.mavlink_serial_control_t item = packet.ByteArrayToStructure<MAVLink.mavlink_serial_control_t>();

            if (item.count == 0)
                return true;

            packetwithdata++;

            Console.WriteLine(DateTime.Now.Millisecond + "data count " + item.count); // ASCIIEncoding.ASCII.GetString(item.data, 0, item.count)

            lock (buffer)
            {
                buffer.AllowOverflow = true;
                buffer.Put(item.data, 0, item.count);
            }

            // we just received data, so do a request again
            GetData(true);

            return true;
        }
        DateTime lastgetdata = DateTime.MinValue;

        void GetData(bool now = false)
        {
            if (lastgetdata.AddMilliseconds(timeout) < DateTime.Now || now)
            {
                mavint.SendSerialControl(port, timeout, null);
                lastgetdata = DateTime.Now;
            }
        }

        public int Read(byte[] readto, int offset, int length)
        {
            int count = 0;

            while (buffer.Size == 0)
            {
                GetData();
                System.Threading.Thread.Sleep(1);
                if (count > ReadTimeout)
                        throw new Exception("MAVLinkSerialPort Timeout on read");
                count+= 1;
            }
            lock (buffer)
            {
                return buffer.Get(readto, offset, length);
            }
        }

        public void Write(byte[] write, int offset, int length)
        {
            byte[] data = new byte[length];

            Array.Copy(write, offset, data, 0, length);

            mavint.SendSerialControl(port, 0, data);
        }

        public void Close()
        {
            log.Info("Close");
            mavint.SendSerialControl(port, 0, null, 0, true);

      if (bgdata.IsAlive)
                bgdata.Abort();
        }

        public void DiscardInBuffer()
        {
            mavint.SendSerialControl(port, timeout, null);

            buffer.Clear();
        }

        public void Open()
        {
            log.Info("Open");
            mavint.SendSerialControl(port, timeout, null, 0, false);
            System.Threading.Thread.Sleep(100);
        }

        public int ReadByte()
        {
            byte[] buffer = new byte[1];
            if (Read(buffer, 0, 1) > 0)
                return buffer[0];
            return -1;
        }

        public int ReadChar()
        {
            return ReadByte();
        }

        public string ReadExisting()
        {
            byte[] data = new byte[buffer.Size];
            if (data.Length > 0)
                Read(data, 0, data.Length);

            string line = Encoding.ASCII.GetString(data, 0, data.Length);

            return line;
        }

        public string ReadLine()
        {
            StringBuilder sb = new StringBuilder();

            char cha;

            do  
            {
                cha = (char)ReadByte();
                sb.Append(cha);
            }
            while (cha != '\n');

            return sb.ToString();
        }

        public void Write(string text)
        {
            Write(ASCIIEncoding.ASCII.GetBytes(text), 0, text.Length);
        }

        public void WriteLine(string text)
        {
            Write(text + "\r\n");
        }

        public void toggleDTR()
        {

        }

        public System.IO.Stream BaseStream
        {
            get { throw new NotImplementedException(); }
        }

        public int BytesToRead
        {
            get {
                GetData();
                lock (buffer)
                {
                    return buffer.Size;
                }
            }
        }

        public int BytesToWrite
        {
            get { return mavint.BaseStream.BytesToWrite; }
        }

        public int DataBits{ get; set; }

        public bool DtrEnable{ get; set; }

        public bool IsOpen
        {
            get { return true; }
        }

        public System.IO.Ports.Parity Parity { get; set; }

        public string PortName { get; set; }

        public int ReadBufferSize
        {
            get
            {
                return buffer.Capacity;
            }
            set
            {
                buffer.Capacity = value;
            }
        }

        public int ReadTimeout { get; set; }

        public bool RtsEnable { get; set; }

        public System.IO.Ports.StopBits StopBits { get; set; }

        public int WriteBufferSize { get; set; }

        public int WriteTimeout { get; set; }
    }
}
