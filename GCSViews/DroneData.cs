using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Runtime.InteropServices;
namespace MissionPlanner.GCSViews
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class DroneData
    {
        // 无人机编号 (int16, 量化单位 1)
        public short DroneId { get; set; }

        // 经纬度 (int32, 量化单位 1e-6 度)
        public int Latitude { get; set; }      // 单位：1e-6 度
        public int Longitude { get; set; }     // 单位：1e-6 度

        // 相对高度 (int16, 量化单位 0.1m，范围 -500~1000)
        public short RelativeAltitude { get; set; }  // 单位：0.1m

        // 海拔高度 (int16, 量化单位 0.1m，范围 -32767~32767)
        public short AbsoluteAltitude { get; set; }  // 单位：0.1m

        // GPS时间 (int32, 量化单位 0.1ms)
        public int GpsTime { get; set; }       // 单位：0.1ms

        // 方向角 (int32, 量化单位 0.1度，范围 0~360)
        public int Heading { get; set; }        // 单位：0.1度

        // 速度 (int16, 量化单位 0.1m/s)
        public short EastVelocity { get; set; }   // 东向速度
        public short NorthVelocity { get; set; }  // 北向速度
        public short UpVelocity { get; set; }     // 垂向速度

        // GPS搜星数量 (int8, 量化单位 1)
        public sbyte GpsSatellites { get; set; }   // 有符号8位整数（-128~127）

        // 从浮点值构建 DroneData（自动量化）
        public static DroneData FromValues(
            short droneId,
            double latitude,          // 单位：度
            double longitude,         // 单位：度
            double relativeAltitude,   // 单位：米
            double absoluteAltitude,   // 单位：米
            long gpsTimeMs,           // 单位：毫秒
            double heading,           // 单位：度
            double eastVelocity,      // 单位：m/s
            double northVelocity,
            double upVelocity,
            byte gpsSatellites)
        {
            return new DroneData
            {
                DroneId = droneId,
                Latitude = Convert.ToInt32(latitude * 1e6),
                Longitude = Convert.ToInt32(longitude * 1e6),
                RelativeAltitude = Convert.ToInt16(relativeAltitude / 0.1),
                AbsoluteAltitude = Convert.ToInt16(absoluteAltitude / 0.1),
                GpsTime = Convert.ToInt32(gpsTimeMs / 0.1),
                Heading = Convert.ToInt32(heading * 10),
                EastVelocity = Convert.ToInt16(eastVelocity / 0.1),
                NorthVelocity = Convert.ToInt16(northVelocity / 0.1),
                UpVelocity = Convert.ToInt16(upVelocity / 0.1),
                GpsSatellites = Convert.ToSByte(gpsSatellites)
            };
        }

        // 序列化为字节数组（结构体转byte[]）
        public byte[] Serialize()
        {
            int size = Marshal.SizeOf(this);
            byte[] arr = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(this, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }

        // 反序列化（byte[]转结构体）
        public static DroneData Deserialize(byte[] data)
        {
            IntPtr ptr = Marshal.AllocHGlobal(data.Length);
            Marshal.Copy(data, 0, ptr, data.Length);
            var droneData = (DroneData)Marshal.PtrToStructure(ptr, typeof(DroneData));
            Marshal.FreeHGlobal(ptr);
            return droneData;
        }
    }
}
