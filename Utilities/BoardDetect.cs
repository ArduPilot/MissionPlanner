using System;
using System.Reflection;
using System.Management;
using System.Windows.Forms;
using System.Threading;
using log4net;
using MissionPlanner.Comms;
using px4uploader;

namespace MissionPlanner.Utilities
{
    public class BoardDetect
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public enum boards
        {
            none = 0,
            b1280, // apm1
            b2560, // apm1
            b2560v2, // apm 2+
            px4, // px3
            px4v2, // pixhawk
            px4v3, // cube/pixhawk with 2mb flash
            px4v4, // pixracer
            px4v4pro, // Pixhawk 3 Pro
            fmuv5, // PixHackV5 and Pixhawk4
            vrbrainv40,
            vrbrainv45,
            vrbrainv50,
            vrbrainv51,
            vrbrainv52,
            vrbrainv54,
            vrcorev10,
            vrubrainv51,
            vrubrainv52,
            bebop2,
            disco,
            solo,
            revomini,
            mindpxv2,
            minipix,
            chbootloader
        }

        public static string chbootloader = "";
   
        /// <summary>
        /// Detect board version
        /// </summary>
        /// <param name="port"></param>
        /// <returns> boards enum value</returns>
        public static boards DetectBoard(string port)
        {
            SerialPort serialPort = new SerialPort();
            serialPort.PortName = port;

            if (!MainV2.MONO)
            {
                try
                {
                    // check the device reported productname
                    var ports = Win32DeviceMgmt.GetAllCOMPorts();

                    foreach (var item in ports)
                    {
                        log.InfoFormat("{0}: {1} - {2}", item.name, item.description, item.board);

                        //if (port.ToLower() == item.name.ToLower())
                        {
                            //USB\VID_0483&PID_DF11   -- stm32 bootloader
                            //USB\VID_1209&PID_5740   -- ardupilot chibios

                            // new style bootloader
                            if (item.hardwareid.StartsWith(@"USB\VID_0483&PID_5740") ||
                                item.hardwareid.StartsWith(@"USB\VID_2DAE&PID_1001") ||
                                item.hardwareid.StartsWith(@"USB\VID_2DAE&PID_1011") ||
                                item.hardwareid.StartsWith(@"USB\VID_1209&PID_5740")) //USB\VID_0483&PID_5740&REV_0200)
                            {
                                if (item.board == "fmuv2" || item.board.ToLower() == "fmuv2-bl")
                                {
                                    log.Info("is a fmuv2");
                                    return boards.px4v2;
                                }
                                if (item.board == "fmuv3" || item.board.ToLower() == "fmuv3-bl")
                                {
                                    log.Info("is a fmuv3");
                                    return boards.px4v3;
                                }
                                if (item.board == "fmuv4" || item.board.ToLower() == "fmuv4-bl")
                                {
                                    log.Info("is a fmuv4");
                                    return boards.px4v4;
                                }
                                if (item.board == "fmuv5" || item.board.ToLower() == "fmuv5-bl")
                                {
                                    log.Info("is a fmuv5");
                                }
                                if (item.board == "revo-mini" || item.board.ToLower() == "revo-mini-bl")
                                {
                                    log.Info("is a revo-mini");
                                    //return boards.revomini;
                                }
                                if (item.board == "mini-pix" || item.board.ToLower() == "mini-pix-bl")
                                {
                                    log.Info("is a mini-pix");
                                    //return boards.minipix;
                                }
                                if (item.board == "mindpx-v2" || item.board.ToLower() == "mindpx-v2-bl")
                                {
                                    log.Info("is a mindpx-v2");
                                    //return boards.mindpxv2;
                                }

                                chbootloader = item.board.Replace("-bl", "").Replace("-Bl", "").Replace("-BL", "");
                                return boards.chbootloader;
                            }

                            // old style bootloader

                            if (item.board == "PX4 FMU v5.x")
                            {//USB\VID_26AC&PID_0032\0
                                log.Info("is a PX4 FMU v5.x (fmuv5)");
                                return boards.fmuv5;
                            }

                            if (item.board == "PX4 FMU v4.x")
                            {
                                log.Info("is a px4v4 pixracer");
                                return boards.px4v4;
                            }

                            if (item.board == "PX4 FMU v2.x")
                            {
                                CustomMessageBox.Show(Strings.PleaseUnplugTheBoardAnd);

                                DateTime DEADLINE = DateTime.Now.AddSeconds(30);

                                while (DateTime.Now < DEADLINE)
                                {
                                    string[] allports = SerialPort.GetPortNames();

                                    foreach (string port1 in allports)
                                    {
                                        log.Info(DateTime.Now.Millisecond + " Trying Port " + port1);
                                        try
                                        {
                                            using (var up = new Uploader(port1, 115200))
                                            {
                                                up.identify();
                                                Console.WriteLine(
                                                    "Found board type {0} boardrev {1} bl rev {2} fwmax {3} on {4}",
                                                    up.board_type,
                                                    up.board_rev, up.bl_rev, up.fw_maxsize, port1);

                                                if (up.fw_maxsize == 2080768 && up.board_type == 9 && up.bl_rev >= 5)
                                                {
                                                    log.Info("is a px4v3");
                                                    return boards.px4v3;
                                                }
                                                else
                                                {
                                                    log.Info("is a px4v2");
                                                    return boards.px4v2;
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            log.Error(ex);
                                        }
                                    }
                                }

                                log.Info("Failed to detect px4 board type");
                                return boards.none;
                            }
                        }
                    }
                } catch { }

                ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_SerialPort"); // Win32_USBControllerDevice
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
                foreach (ManagementObject obj2 in searcher.Get())
                {
                    log.InfoFormat("-----------------------------------");
                    log.InfoFormat("Win32_USBDevice instance");
                    log.InfoFormat("-----------------------------------");
   
                    foreach (var item in obj2.Properties)
                    {
                        log.InfoFormat("{0}: {1}", item.Name, item.Value);
                    }


                    // check vid and pid
                    if (obj2.Properties["PNPDeviceID"].Value.ToString().Contains(@"USB\VID_2341&PID_0010"))
                    {
                        // check port name as well
                        if (obj2.Properties["Name"].Value.ToString().ToUpper().Contains(serialPort.PortName.ToUpper()))
                        {
                            log.Info("is a 2560-2");
                            return boards.b2560v2;
                        }
                    }

                    if (obj2.Properties["PNPDeviceID"].Value.ToString().Contains(@"USB\VID_26AC&PID_0010"))
                    {
                        // check port name as well
                        //if (obj2.Properties["Name"].Value.ToString().ToUpper().Contains(serialPort.PortName.ToUpper()))
                        {
                            log.Info("is a px4");
                            return boards.px4;
                        }
                    }

                    if (obj2.Properties["PNPDeviceID"].Value.ToString().Contains(@"USB\VID_26AC&PID_0032"))
                    {
                        // check port name as well
                        //if (obj2.Properties["Name"].Value.ToString().ToUpper().Contains(serialPort.PortName.ToUpper()))
                        {
                            log.Info("is a fmuv5");
                            return boards.fmuv5;
                        }
                    }

                    // chibios or normal px4
                    if (obj2.Properties["PNPDeviceID"].Value.ToString().Contains(@"USB\VID_0483&PID_5740") || 
                        obj2.Properties["PNPDeviceID"].Value.ToString().Contains(@"USB\VID_26AC&PID_0011") ||
                        obj2.Properties["PNPDeviceID"].Value.ToString().Contains(@"USB\VID_1209&PID_5740"))
                    {
                        CustomMessageBox.Show(Strings.PleaseUnplugTheBoardAnd);

                        DateTime DEADLINE = DateTime.Now.AddSeconds(30);

                        while (DateTime.Now < DEADLINE)
                        {
                            string[] allports = SerialPort.GetPortNames();

                            foreach (string port1 in allports)
                            {
                                log.Info(DateTime.Now.Millisecond + " Trying Port " + port1);
                                try
                                {
                                    using (var up = new Uploader(port1, 115200))
                                    {
                                        up.identify();
                                        Console.WriteLine(
                                            "Found board type {0} boardrev {1} bl rev {2} fwmax {3} on {4}",
                                            up.board_type,
                                            up.board_rev, up.bl_rev, up.fw_maxsize, port1);

                                        if (up.fw_maxsize == 2080768 && up.board_type == 9 && up.bl_rev >= 5)
                                        {
                                            log.Info("is a px4v3");
                                            return boards.px4v3;
                                        }
                                        else if (up.fw_maxsize == 2080768 && up.board_type == 50 && up.bl_rev >= 5)
                                        {
                                            log.Info("is a fmuv5");
                                            return boards.fmuv5;
                                        }
                                        else
                                        {
                                            log.Info("is a px4v2");
                                            return boards.px4v2;
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    log.Error(ex);
                                }
                            }
                        }

                        log.Info("Failed to detect px4 board type");
                        return boards.none;
                    }

                    if (obj2.Properties["PNPDeviceID"].Value.ToString().Contains(@"USB\VID_26AC&PID_0021"))
                    {
                        log.Info("is a px4v3 X2.1");
                        return boards.px4v3;
                    }

                    if (obj2.Properties["PNPDeviceID"].Value.ToString().Contains(@"USB\VID_26AC&PID_0012"))
                    {
                        log.Info("is a px4v4 pixracer");
                        return boards.px4v4;
                    }

                    if (obj2.Properties["PNPDeviceID"].Value.ToString().Contains(@"USB\VID_26AC&PID_0013"))
                    {
                        log.Info("is a px4v4pro pixhawk 3 pro");
                        return boards.px4v4pro;
                    }

                    if (obj2.Properties["PNPDeviceID"].Value.ToString().Contains(@"USB\VID_26AC&PID_0001"))
                    {
                        log.Info("is a px4v2 bootloader");
                        return boards.px4v2;
                    }

                    if (obj2.Properties["PNPDeviceID"].Value.ToString().Contains(@"USB\VID_26AC&PID_0016"))
                    {
                        log.Info("is a px4v2 bootloader");
                        CustomMessageBox.Show(
                            "You appear to have a bootloader with a bad PID value, please update your bootloader.");
                        return boards.px4v2;
                    }

                    //|| obj2.Properties["PNPDeviceID"].Value.ToString().Contains(@"USB\VID_26AC&PID_0012") || obj2.Properties["PNPDeviceID"].Value.ToString().Contains(@"USB\VID_26AC&PID_0013") || obj2.Properties["PNPDeviceID"].Value.ToString().Contains(@"USB\VID_26AC&PID_0014") || obj2.Properties["PNPDeviceID"].Value.ToString().Contains(@"USB\VID_26AC&PID_0015") || obj2.Properties["PNPDeviceID"].Value.ToString().Contains(@"USB\VID_26AC&PID_0016")

                    if (obj2.Properties["PNPDeviceID"].Value.ToString().Contains(@"USB\VID_27AC&PID_1140"))
                    {
                        log.Info("is a vrbrain 4.0 bootloader");
                        return boards.vrbrainv40;
                    }

                    if (obj2.Properties["PNPDeviceID"].Value.ToString().Contains(@"USB\VID_27AC&PID_1145"))
                    {
                        log.Info("is a vrbrain 4.5 bootloader");
                        return boards.vrbrainv45;
                    }

                    if (obj2.Properties["PNPDeviceID"].Value.ToString().Contains(@"USB\VID_27AC&PID_1150"))
                    {
                        log.Info("is a vrbrain 5.0 bootloader");
                        return boards.vrbrainv50;
                    }

                    if (obj2.Properties["PNPDeviceID"].Value.ToString().Contains(@"USB\VID_27AC&PID_1151"))
                    {
                        log.Info("is a vrbrain 5.1 bootloader");
                        return boards.vrbrainv51;
                    }

                    if (obj2.Properties["PNPDeviceID"].Value.ToString().Contains(@"USB\VID_27AC&PID_1152"))
                    {
                        log.Info("is a vrbrain 5.2 bootloader");
                        return boards.vrbrainv52;
                    }

                    if (obj2.Properties["PNPDeviceID"].Value.ToString().Contains(@"USB\VID_27AC&PID_1154"))
                    {
                        log.Info("is a vrbrain 5.4 bootloader");
                        return boards.vrbrainv54;
                    }

                    if (obj2.Properties["PNPDeviceID"].Value.ToString().Contains(@"USB\VID_27AC&PID_1910"))
                    {
                        log.Info("is a vrbrain core 1.0 bootloader");
                        return boards.vrcorev10;
                    }

                    if (obj2.Properties["PNPDeviceID"].Value.ToString().Contains(@"USB\VID_27AC&PID_1351"))
                    {
                        log.Info("is a vrubrain 5.1 bootloader");
                        return boards.vrubrainv51;
                    }

                    if (obj2.Properties["PNPDeviceID"].Value.ToString().Contains(@"USB\VID_27AC&PID_1352"))
                    {
                        log.Info("is a vrubrain 5.2 bootloader");
                        return boards.vrubrainv52;
                    }
                }
            }
            else
            {
                // if its mono
                if ((int)DialogResult.Yes == CustomMessageBox.Show("Is this a APM 2+?", "APM 2+", MessageBoxButtons.YesNo))
                {
                    return boards.b2560v2;
                }
                else
                {
                    if ((int)DialogResult.Yes ==
                        CustomMessageBox.Show("Is this a PX4/PIXHAWK/PIXRACER?", "PX4/PIXHAWK", MessageBoxButtons.YesNo))
                    {
                        if ((int)DialogResult.Yes ==
                            CustomMessageBox.Show("Is this a PIXRACER?", "PIXRACER", MessageBoxButtons.YesNo))
                        {
                            return boards.px4v4;
                        }
                        if ((int)DialogResult.Yes ==
                            CustomMessageBox.Show("Is this a CUBE?", "CUBE", MessageBoxButtons.YesNo))
                        {
                            return boards.px4v3;
                        }
                        if ((int)DialogResult.Yes ==
                            CustomMessageBox.Show("Is this a PIXHAWK?", "PIXHAWK", MessageBoxButtons.YesNo))
                        {
                            return boards.px4v2;
                        }
                        return boards.px4;
                    }
                    else
                    {
                        return boards.b2560;
                    }
                }
            }

            if ((int)DialogResult.Yes == CustomMessageBox.Show("Is this a Linux board?", "Linux", MessageBoxButtons.YesNo))
            {
                if ((int)DialogResult.Yes == CustomMessageBox.Show("Is this Bebop2?", "Bebop2", MessageBoxButtons.YesNo))
                {
                    return boards.bebop2;
                }

                if ((int)DialogResult.Yes == CustomMessageBox.Show("Is this Disco?", "Disco", MessageBoxButtons.YesNo))
                {
                    return boards.disco;
                }
            }

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
                serialPort.Write(new byte[] {(byte) '0', (byte) ' '}, 0, 2);
                a++;
                Thread.Sleep(50);

                //Console.WriteLine("btr {0}", serialPort.BytesToRead);
                if (serialPort.BytesToRead >= 2)
                {
                    byte b1 = (byte) serialPort.ReadByte();
                    byte b2 = (byte) serialPort.ReadByte();
                    if (b1 == 0x14 && b2 == 0x10)
                    {
                        serialPort.Close();
                        log.Info("is a 1280");
                        return boards.b1280;
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
                byte[] temp = new byte[] {0x6, 0, 0, 0, 0};
                temp = BoardDetect.genstkv2packet(serialPort, temp);
                a++;
                Thread.Sleep(50);

                try
                {
                    if (temp[0] == 6 && temp[1] == 0 && temp.Length == 2)
                    {
                        serialPort.Close();
                        //HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Enum\USB\VID_2341&PID_0010\640333439373519060F0\Device Parameters
                        if (!MainV2.MONO &&
                            !Thread.CurrentThread.CurrentCulture.IsChildOf(CultureInfoEx.GetCultureInfo("zh-Hans")))
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
                                        return boards.b2560v2;
                                    }
                                }
                            }

                            log.Info("is a 2560");
                            return boards.b2560;
                        }
                    }
                }
                catch
                {
                }
            }

            serialPort.Close();
            log.Warn("Not a 2560");

            if ((int)DialogResult.Yes == CustomMessageBox.Show("Is this a APM 2+?", "APM 2+", MessageBoxButtons.YesNo))
            {
                return boards.b2560v2;
            }
            else
            {
                if ((int)DialogResult.Yes ==
                    CustomMessageBox.Show("Is this a PX4/PIXHAWK?", "PX4/PIXHAWK", MessageBoxButtons.YesNo))
                {
                    if ((int)DialogResult.Yes ==
                        CustomMessageBox.Show("Is this a PIXHAWK?", "PIXHAWK", MessageBoxButtons.YesNo))
                    {
                        return boards.px4v2;
                    }
                    return boards.px4;
                }
                else
                {
                    return boards.b2560;
                }
            }
        }

        /// <summary>
        /// STK v2 generate packet
        /// </summary>
        /// <param name="serialPort"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        static byte[] genstkv2packet(SerialPort serialPort, byte[] message)
        {
            byte[] data = new byte[300];
            byte ck = 0;

            data[0] = 0x1b;
            ck ^= data[0];
            data[1] = 0x1;
            ck ^= data[1];
            data[2] = (byte) ((message.Length >> 8) & 0xff);
            ck ^= data[2];
            data[3] = (byte) (message.Length & 0xff);
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

            byte[] ret = BoardDetect.readpacket(serialPort);

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
        static byte[] readpacket(SerialPort serialPort)
        {
            byte[] temp = new byte[4000];
            byte[] mes = new byte[2] {0x0, 0xC0}; // fail
            int a = 7;
            int count = 0;

            serialPort.ReadTimeout = 1000;

            while (count < a)
            {
                //Console.WriteLine("count {0} a {1} mes leng {2}",count,a,mes.Length);
                try
                {
                    temp[count] = (byte) serialPort.ReadByte();
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
                temp[count] = (byte) serialPort.ReadByte();
            }
            catch
            {
            }

            count++;

            Array.Resize<byte>(ref temp, count);

            //Console.WriteLine(this.BytesToRead);

            return mes;
        }
    }
}