using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MissionPlanner.Utilities
{
    public class Pelco
    {
        [Flags]
        public enum cmd1 : byte
        {
            None = 0,
            FocusNear = 1,
            IrisOpen = 2,
            IrisClose = 4,
            CameraOnOrOff = 8,
            AutoOrManualScan = 16,
            Reserved1 = 32,
            Reserved2 = 64,
            Sense = 128
        }

        [Flags]
        public enum cmd2 : byte
        {
            None = 0,
            FixedTo0 = 1,
            PanRight = 2,
            PanLeft = 4,
            TiltUp = 8,
            TiltDown = 16,
            ZoomTele = 32,
            ZoomWide = 64,
            FocusFar = 128
        }

        /*
           Pelco-D
           Byte 1 - Start transmission - always 0xFF
           Byte 2 - Address of camera
           Byte 3 - Command 1
           Byte 4 - Command 2
           Byte 5 - Data 1
           Byte 6 - Data 2
           Byte 7 - Checksum

Camera Address: 1
Pan Left at high speed: FF 01 00 04 3F 00 44
Pan Right at medium speed: FF 01 00 02 20 00 23
Tilt Up at high speed: FF 01 00 08 00 3F 48
Tilt Down at medium speed: FF 01 00 10 20 00 31
Stop all actions (Pan / Tilt / Zoom / Iris etc.): FF 01 00 00 00 00 01
         */
        public struct D
        {
            public byte Sync;
            public byte CameraAddress;
            public cmd1 Command1;
            public cmd2 Command2;
            public byte Data1;
            public byte Data2;
            public byte CheckSum;
        }
        
        public byte[] GenerateCommand(byte cameraaddr, cmd1 cmd1, cmd2 cmd2, byte data1,byte data2)
        {
            return new byte[]
            {
                0xff, cameraaddr, (byte) cmd1, (byte) cmd2, data1, data2,
                (byte) (0xff + cameraaddr + (byte)cmd1 + (byte)cmd2 + data1 + data2)
            };
        }
         

        /*
           Pelco-P
           Byte 1 - Start transmission. Always 0xA0
           Byte 2 - Address of camera - Range 0x00 to 0x1F
           Byte 3 - Command 1
           Byte 4 - Command 2
           Byte 5 - Data 1
           Byte 6 - Data 2
           Byte 7 - End Transmission. Always 0xAF
           Byte 8 - Checksum 
         */
        public struct P
        {
            public byte STX; //0xa0
            public byte CameraAddress;
            public byte Data1;
            public byte Data2;
            public byte Data3;
            public byte Data4;
            public byte ETX; // 0xaf
            public byte CheckSum;
        }

        /// <summary>
        /// full pelco-d packet inc sync and cs
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        bool CheckSum(byte[] input)
        {
            var cscount = input.Length - 2; // Sync and CS
            var currentcs = input.Last();
            var sum = input.Skip(1).Take(cscount).Sum(a => a);
            var inputcs = sum % 256;
            if (inputcs == currentcs)
                return true;
            return false;
        }
    }
}
