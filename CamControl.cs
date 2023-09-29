using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MissionPlanner
{
    public class CamControl
    {
        static ushort[] wCRC_Table = new ushort[] { 0x0000, 0xCC01, 0xD801, 0x1400, 0xF001, 0x3C00, 0x2800, 0xE401, 0xA001, 0x6C00, 0x7800, 0xB401, 0x5000, 0x9C01, 0x8801, 0x4400 };
        static byte[] Ctrstr = Array.Empty<byte>();
        static byte[] head_up = new byte[] { 0xEB, 0x90, 0x0A, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x40, 0x88, 0x0F, 0x04, 0x00, 0x00, 0xf4, 0x20, 0x00, 0x00 };
        static byte[] head_down = new byte[] { 0xEB, 0x90, 0x0A, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x40, 0x88, 0x0F, 0x04, 0x00, 0x00, 0xf4, 0x50, 0x00, 0x00 };

        static void crc_fly16()
        {
            int length = Ctrstr.Length;
            int lens = length;
            ushort crcTmp = 0xFFFF;
            while (length > 0)
            {
                length--;
                ushort tmp = (ushort)(wCRC_Table[(Ctrstr[lens - length - 1] ^ crcTmp) & 15] ^ (crcTmp >> 4));
                crcTmp = (ushort)(wCRC_Table[((Ctrstr[lens - length - 1] >> 4) ^ tmp) & 15] ^ (tmp >> 4));
            }
            byte crc1 = (byte)(0x00FF & crcTmp);
            byte crc2 = (byte)(0xFF & (crcTmp >> 8));

            Ctrstr = Ctrstr.Concat(new byte[] { crc1, crc2 }).ToArray();
        }

        static void TCP_CMD()
        {
            crc_fly16();

            try
            {
                Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                s.Connect("192.168.42.200", 2000);

                byte[] data = Ctrstr.ToArray();
                s.Send(data);
                Console.WriteLine("send data TCP Control CMD");
                s.Close();
                Ctrstr = Array.Empty<byte>();
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.Message);
                return;
            }
        }

        // run for up
        public void upCMD()
        {
            Ctrstr = head_up.ToArray();
            TCP_CMD();
        }

        // run for down
        public void downCMD()
        {
            Ctrstr = head_down.ToArray();
            TCP_CMD();
        }

        //static void Main(string[] args)
        //{
        //    upCMD();
        //}
    }

    public class Cam20xControl
    {
        // Zoom commands
        public const string ZoomOut = "#TPUM2wZMC01";
        public const string ZoomIn = "#TPUM2wZMC02";
        public const string ZoomStop = "#TPUM2wZMC00";

        // Gimbal commands
        public const string GimbalStop = "#TPUG2wPTZ00";
        public const string GimbalUp = "#TPUG2wPTZ01";
        public const string GimbalDown = "#TPUG2wPTZ02";
        public const string GimbalLeft = "#TPUG2wPTZ03";
        public const string GimbalRight = "#TPUG2wPTZ04";
        public const string GimbalHome = "#TPUG2wPTZ05";
        public const string GimbalYawVelocity = "#TPUG2wGSY";
        public const string GimbalPitchVelocity = "#TPUG2wGSP";
        public const string GimbalYawPitchVelocity = "#tpUG4wGSM";

        // Camera Focus Control
        public const string FocusStop = "#TPUM2wFCC00";
        public const string FocusPlus = "#TPUM2wFCC01";
        public const string FocusMinus = "#TPUM2wFCC02";
        private UdpClient udpClient;
        public const string ipAddress = "192.168.144.108";
        public const int port = 9003;
        public bool connect=false;
        public string parmDecode(string parm)
        {


            if (parm == "G0")
            {
                return GimbalStop;
            }
            else if (parm == "Z0")
            {
                return ZoomStop;
            }
            else if (parm == "F0")
            {
                return FocusStop;
            }
            else if (parm == "Up")
            {
                return GimbalUp;
            }
            else if (parm == "Down")
            {
                return GimbalDown;
            }
            else if (parm == "Right")
            {
                return GimbalRight;
            }
            else if (parm == "Left")
            {
                return GimbalLeft;
            }
            else if (parm == "F+")
            {
                return FocusPlus;
            }
            else if (parm == "F-")
            {
                return FocusMinus;

            }
            else if (parm == "ZIn")
            {
                return ZoomIn;
            }
            else if (parm == "ZOut")
            {
                return ZoomOut;
            }
            return null;
        }
        public void Cam20x(string parm)
        {
            if (!connect)
            {
                udpClient = new UdpClient();

                // Set SO_REUSEADDR equivalent option (SO_REUSEPORT) for socket reuse
                udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                connect = true;
            }
            var Camcommand = parmDecode(parm);

            udpClient.Connect(IPAddress.Parse(ipAddress), port);
            byte[] data = GetActualCommand(Camcommand);
            using (UdpClient udpClient = new UdpClient())
            {
                // Send the data to the target IP address and port
                udpClient.Send(data, data.Length, ipAddress, port);
            }
            if (parm == "G0" || parm == "Z0" || parm == "F0")
            {
                connect = false;
                udpClient.Close();

            }


        }
        public Cam20xControl()
        {

            

        }
        public static string CalculateCRC8(string data)
        {
            int crc = 0;
            foreach (char c in data)
            {
                crc += (int)c;
            }
            crc %= 256;
            string hexstr = crc.ToString("X2");
            return hexstr;
        }

        public static byte[] GetActualCommand(string cmd)
        {
            string crcstr = CalculateCRC8(cmd);
            foreach (char c in crcstr)
            {
                cmd += c;
            }

            return Encoding.UTF8.GetBytes(cmd);
        }


    }

}
