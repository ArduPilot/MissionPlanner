using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace TrackerHomeGPS
{
    partial class GarminUSBGPS : GPSDevice
    {
        new protected static Guid DeviceGuid = new Guid(0x2c9c45c2, (unchecked((short)0x8e7d)), 0x4c08, 0xa1, 0x2d, 0x81, 0x6b, 0xba, 0xe7, 0x22, 0xc0);

        #region GarminPacketTypes
        protected const byte Pid_USB_Protocol_Layer = 0;
        protected const byte Pid_Application_Layer = 20;
        #endregion

        #region GarminUSBProtocolLayerPacketIDs
        protected const Int16 Pid_Data_Available = 0x00000002;
        protected const Int16 Pid_Start_Session = 0x00000005;
        protected const Int16 Pid_Session_Started = 0x00000006;
        #endregion

        #region IOCTLs
        protected const UInt32 IOCTL_API_VERSION = (((FILE_DEVICE_UNKNOWN) << 16) | ((FILE_ANY_ACCESS) << 14) | ((0x00000800) << 2) | (METHOD_BUFFERED));
        protected const UInt32 IOCTL_ASYNC_IN = (((FILE_DEVICE_UNKNOWN) << 16) | ((FILE_ANY_ACCESS) << 14) | ((0x00000850) << 2) | (METHOD_BUFFERED));
        protected const UInt32 IOCTL_USB_PACKET_SIZE = (((FILE_DEVICE_UNKNOWN) << 16) | ((FILE_ANY_ACCESS) << 14) | ((0x00000851) << 2) | (METHOD_BUFFERED));
        #endregion

        #region GarminConstants
        protected const Int32 API_VERSION = 1;
        protected const Int32 MAX_BUFFER_SIZE = 4096;
        protected const Int32 ASYNC_DATA_SIZE = 64;
        #endregion

        #region GarminBasicPacketID
        protected const Int32 Pid_Protocol_Array = 253;
        protected const Int32 Pid_Product_Rqst = 254;
        protected const Int32 Pid_Product_Data = 255;
        protected const Int32 Pid_Ext_Product_Data = 248;
        #endregion

        protected enum GarminLinkProtocol1ID : ushort
        {
            Pid_Command_Data = 10,
            Pid_Xfer_Cmplt = 12,
            Pid_Date_Time_Data = 14,
            Pid_Position_Data = 17,
            Pid_Prx_Wpt_Data = 19,
            Pid_Records = 27,
            Pid_Rte_Hdr = 29,
            Pid_Rte_Wpt_Data = 30,
            Pid_Almanac_Data = 31,
            Pid_Trk_Data = 34,
            Pid_Wpt_Data = 35,
            Pid_Pvt_Data = 51,
            Pid_Rte_Link_Data = 98,
            Pid_Trk_Hdr = 99,
            Pid_FlightBook_Record = 134,
            Pid_Lap = 149,
            Pid_Wpt_Cat = 152,
            Pid_Run = 990,
            Pid_Workout = 991,
            Pid_Workout_Occurence = 992,
            Pid_Fitness_User_Profile = 993,
            Pid_Workout_Limits = 994,
            Pid_Course = 1061,
            Pid_Course_Lap = 1062,
            Pid_Course_Point = 1063,
            Pid_Course_Trk_Hdr = 1064,
            Pid_Course_Trk_Data = 1065,
            Pid_Course_Limits = 1066
        };

        protected enum GarminAppProtocol010CommandType : ushort
        {
            Cmnd_Abort_Transfer = 0,
            Cmnd_Transfer_Alm = 1,
            Cmnd_Transfer_Pos = 2,
            Cmnd_Transfer_Prx = 3,
            Cmnd_Transfer_Rte = 4,
            Cmnd_Transfer_Time = 5,
            Cmnd_Transfer_Trk = 6,
            Cmnd_Transfer_Wpt = 7,
            Cmnd_Turn_Off_Pwr = 8,
            Cmnd_Start_Pvt_Data = 49,
            Cmnd_Stop_Pvt_Data = 50,
            Cmnd_FlightBook_Transfer = 92,
            Cmnd_Transfer_Laps = 117,
            Cmnd_Transfer_Wpt_Cats = 121,
            Cmnd_Transfer_Runs = 450,
            Cmnd_Transfer_Workouts = 451,
            Cmnd_Transfer_Workout_Occurences = 452,
            Cmnd_Transfer_Fitness_User_Profile = 453,
            Cmnd_Transfer_Workout_Limits = 454,
            Cmnd_Transfer_Courses = 561,
            Cmnd_Transfer_Course_Laps = 562,
            Cmnd_Transfer_Course_Points = 563,
            Cmnd_Transfer_Course_Tracks = 564,
            Cmnd_Transfer_Course_Limits = 565,
        };

        [StructLayout(LayoutKind.Sequential, Size = 12)]
        public struct Packet
        {
            public Packet(bool USBProtocolLayer, short id, int dataSize, byte[] data)
            {
                this.mPacketId = id;
                this.mPacketType = USBProtocolLayer ? Pid_USB_Protocol_Layer : Pid_Application_Layer;
                this.mDataSize = dataSize;
                this.mData = data;
                this.mReserved1 = 0;
                this.mReserved2 = 0;
                this.mReserved3 = 0;
            }

            public byte mPacketType;
            public byte mReserved1;
            public short mReserved2;
            public short mPacketId;
            public short mReserved3;
            public int mDataSize;
            public byte[] mData;
        }

        public struct GarminProductData
        {
            public static explicit operator GarminProductData(Packet p)
            {
                GarminProductData temp = new GarminProductData();

                if (p.mDataSize <= 4) return temp;

                temp.productId = (ushort)((p.mData[1] << 8) | (p.mData[0]));
                temp.softVers = (ushort)((p.mData[3] << 8) | (p.mData[2]));
                temp.productDescription = Encoding.ASCII.GetString(p.mData, 4, p.mDataSize - 4);
                return temp;
            }

            public ushort productId;
            public ushort softVers;
            public string productDescription;
        }

        public struct ProtocolData
        {
            public byte tag;
            public ushort data;
        }

        public struct ProtocolDataPacket
        {
            public static explicit operator ProtocolDataPacket(Packet p)
            {
                ProtocolDataPacket temp = new ProtocolDataPacket();

                if (p.mDataSize % 3 != 0) return temp;

                temp.numProtocols = p.mDataSize / 3;
                temp.data = new ProtocolData[temp.numProtocols];
                for (int i = 0; i < temp.numProtocols; i++)
                {
                    temp.data[i].tag = p.mData[i * 3];
                    temp.data[i].data = (ushort)((p.mData[i * 3 + 2] << 8) | (p.mData[i * 3 + 1]));
                }

                return temp;
            }

            public int numProtocols;
            public ProtocolData[] data;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct GarminPosition
        {
            public Int32 lat;
            public Int32 lon;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct GarminRadianPosition
        {
            public static explicit operator GarminRadianPosition(Packet p)
            {
                GarminRadianPosition temp = new GarminRadianPosition();

                temp.lat = BitConverter.ToDouble(p.mData, 0);
                temp.lon = BitConverter.ToDouble(p.mData, 8);

                return temp;
            }
            public double lat;
            public double lon;
        }


    }
}
