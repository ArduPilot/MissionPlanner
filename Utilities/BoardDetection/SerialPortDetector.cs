using System;
using System.Collections.Generic;
using System.Management;
using System.Threading;
using log4net;
using MissionPlanner.ArduPilot;
using MissionPlanner.Comms;

namespace MissionPlanner.Utilities.BoardDetection
{
    public class SerialPortDetector : IDetector
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SerialPortDetector));

        public DetectResult Detect(string port, IReadOnlyList<DeviceInfo> ports)
        {
            ICommsSerial serialPort = new SerialPort();
            serialPort.PortName = port;

            if (serialPort.IsOpen)
                serialPort.Close();

            serialPort.DtrEnable = true;
            serialPort.BaudRate = 57600;
            serialPort.Open();

            Thread.Sleep(100);

            int a = 0;
            while (a < 20) // 20 * 50 = 1 sec
            {
                //Console.WriteLine("write " + DateTime.Now.Millisecond);
                serialPort.DiscardInBuffer();
                serialPort.Write(new byte[] { (byte)'0', (byte)' ' }, 0, 2);
                a++;
                Thread.Sleep(50);

                //Console.WriteLine("btr {0}", serialPort.BytesToRead);
                if (serialPort.BytesToRead >= 2)
                {
                    byte b1 = (byte)serialPort.ReadByte();
                    byte b2 = (byte)serialPort.ReadByte();
                    if (b1 == 0x14 && b2 == 0x10)
                    {
                        serialPort.Close();
                        log.Info("is a 1280");
                        return new DetectResult(Boards.b1280, null);
                    }
                }
            }

            if (serialPort.IsOpen)
                serialPort.Close();

            log.Warn("Not a 1280");

            Thread.Sleep(500);

            serialPort.DtrEnable = true;
            serialPort.BaudRate = 115200;
            serialPort.Open();

            Thread.Sleep(100);

            a = 0;
            while (a < 4)
            {
                byte[] temp = new byte[] { 0x6, 0, 0, 0, 0 };
                temp = genstkv2packet(serialPort, temp);
                a++;
                Thread.Sleep(50);

                try
                {
                    if (temp[0] == 6 && temp[1] == 0 && temp.Length == 2)
                    {
                        serialPort.Close();
                        //HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Enum\USB\VID_2341&PID_0010\640333439373519060F0\Device Parameters
                        // if (!MONO)
                        {
                            ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_SerialPort");
                            // Win32_USBControllerDevice
                            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
                            foreach (ManagementObject obj2 in searcher.Get())
                            {
                                //Console.WriteLine("Dependant : " + obj2["Dependent"]);

                                // all apm 1-1.4 use a ftdi on the imu board.

                                /*    obj2.Properties.ForEach(x =>
                                {
                                    try
                                    {
                                        log.Info(((PropertyData)x).Name.ToString() + " = " + ((PropertyData)x).Value.ToString());
                                    }
                                    catch { }
                                });
                                */
                                // check vid and pid
                                if (obj2.Properties["PNPDeviceID"].Value.ToString().Contains(@"USB\VID_2341&PID_0010"))
                                {
                                    // check port name as well
                                    if (
                                        obj2.Properties["Name"].Value.ToString()
                                            .ToUpper()
                                            .Contains(serialPort.PortName.ToUpper()))
                                    {
                                        log.Info("is a 2560-2");
                                        return new DetectResult(Boards.b2560v2, null);
                                    }
                                }
                            }

                            log.Info("is a 2560");
                            return new DetectResult(Boards.b2560, null);
                        }
                    }
                }
                catch
                {
                }
            }

            serialPort.Close();
            log.Warn("Not a 2560");

            return DetectResult.None;
        }
        
        /// <summary>
        /// STK v2 generate packet
        /// </summary>
        /// <param name="serialPort"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private static byte[] genstkv2packet(ICommsSerial serialPort, byte[] message)
        {
            byte[] data = new byte[300];
            byte ck = 0;

            data[0] = 0x1b;
            ck ^= data[0];
            data[1] = 0x1;
            ck ^= data[1];
            data[2] = (byte)((message.Length >> 8) & 0xff);
            ck ^= data[2];
            data[3] = (byte)(message.Length & 0xff);
            ck ^= data[3];
            data[4] = 0xe;
            ck ^= data[4];

            int a = 5;
            foreach (byte let in message)
            {
                data[a] = let;
                ck ^= let;
                a++;
            }
            data[a] = ck;
            a++;

            serialPort.Write(data, 0, a);
            //Console.WriteLine("about to read packet");

            byte[] ret = readpacket(serialPort);

            //if (ret[1] == 0x0)
            {
                //Console.WriteLine("received OK");
            }

            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serialPort"></param>
        /// <returns></returns>
        private static byte[] readpacket(ICommsSerial serialPort)
        {
            byte[] temp = new byte[4000];
            byte[] mes = new byte[2] { 0x0, 0xC0 }; // fail
            int a = 7;
            int count = 0;

            serialPort.ReadTimeout = 1000;

            while (count < a)
            {
                //Console.WriteLine("count {0} a {1} mes leng {2}",count,a,mes.Length);
                try
                {
                    temp[count] = (byte)serialPort.ReadByte();
                }
                catch
                {
                    break;
                }


                //Console.Write("{1}", temp[0], (char)temp[0]);

                if (temp[0] != 0x1b)
                {
                    count = 0;
                    continue;
                }

                if (count == 3)
                {
                    a = (temp[2] << 8) + temp[3];
                    mes = new byte[a];
                    a += 5;
                }

                if (count >= 5)
                {
                    mes[count - 5] = temp[count];
                }

                count++;
            }

            //Console.WriteLine("read ck");
            try
            {
                temp[count] = (byte)serialPort.ReadByte();
            }
            catch
            {
            }

            count++;

            Array.Resize(ref temp, count);

            //Console.WriteLine(this.BytesToRead);

            return mes;
        }
    }
}