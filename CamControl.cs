using System;
using System.Collections.Generic;
using System.Linq;
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
}
