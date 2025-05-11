using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using MissionPlanner.Controls;
using static MissionPlanner.Udp;
using System.Runtime.InteropServices;
using MissionPlanner.Swarm.Sequence;
using System.Text;
using Onvif.Core.Client;
using BitMiracle.LibTiff.Classic;

namespace MissionPlanner.GCSViews
{
    public class UDP
    {
        const string UDP_IP = "192.168.0.1";  // 目标IP
        const int UDP_PORT = 15005;           // 目标端口
        IPEndPoint endPoint;
        IPEndPoint endPointudp;
        UdpClient udpClient;
        public static Boolean is_true = false;
        private Thread workerThread;
        private CancellationTokenSource cancellationTokenSource;
        //private bool is_true = false;
        public void UDPlink(Boolean isLink)
        {
            
            //Console.WriteLine("这是一个日志信息");
            if (isLink)
            {
                is_true = true;
                endPoint = new IPEndPoint(IPAddress.Parse(UDP_IP), UDP_PORT);
                udpClient = new UdpClient(16000);
                // 启动发送数据的工作线程
                //StartWorker();
                ThreadPool.QueueUserWorkItem(sendmessage);
            }
            else {
                is_true = false ;
                // 关闭UDP客户端并停止线程
                if (udpClient != null)
                {
                    udpClient.Close();
                }

               
            }
            
        }


        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct PdxpPacket
        {
            public byte VER;              // 协议版本号 (1字节)
            public byte MID;              // 任务代号 (1字节)
            public ushort SID;            // 发送方地址 (2字节)
            public ushort DID;            // 接收方地址 (2字节)
            public uint BID;              // 数据包类型标识 (4字节)
            public uint No;               // 包序号 (4字节)
            public ushort L;              // 数据域长度 (2字节)
            public Int16 UAVId;           // 无人机编号 (2字节)
            public Int32 Longitude;         // 经度 (4字节)
            public Int32 Latitude;          // 纬度 (4字节)
            public Int16 RelativeHeight;  // 相对高度 (2字节)
            public Int16 Altitude;        // 海拔高度 (2字节)
            public Int64 GPSTime;           // GPS时间 (8字节)
            public Int32 Heading;           // 方向角 (4字节)
            public Int16 EastVelocity;    // 东向速度 (2字节)
            public Int16 NorthVelocity;   // 北向速度 (2字节)
            public Int16 VerticalVelocity;// 垂向速度 (2字节)
            public sbyte GPSSatellites;   // GPS搜星数量 (1字节)		_battery_voltage	12.587000937804632	double
            public Int16 Batteruy_V;      //电压(2字节)
            public byte Failsafe;          //是否进入故障安全模式（当前未触发）(1字节)
            public byte Gpsstatus;         //GPS 状态（6 表示正常，具体值需查 MAVLink 协议）(1字节)
        }


        private void sendmessage(object nothing)
        {
            int sequence = 0;
            while (is_true)
            {
                // 执行任务，可以替换为实际的代码逻辑
                //Console.WriteLine("线程正在执行...");
                //Thread.Sleep(1000); // 模拟工作1秒钟
               
                    foreach (var port in MainV2.Comports)
                    {
                        foreach (var mav in port.MAVlist)
                        {
                        sequence++;
                            DateTime gpsEpoch = new DateTime(1980, 1, 6, 0, 0, 0, DateTimeKind.Utc);
                            TimeSpan elapsed = mav.cs.gpstime.ToUniversalTime() - gpsEpoch;
                            Int64 ticks = (Int64)(elapsed.TotalMilliseconds * 10); // 转换为 0.1 毫秒单位
                            // 初始化PDXP数据包并填充默认值
                            PdxpPacket pdxpPacket = new PdxpPacket
                            {
                                VER = 1,                           // 协议版本
                                MID = 0x01,                        // 任务代号
                                SID = 0x0001,                      // 发送方地址
                                DID = 0x0002,                      // 接收方地址
                                BID = 0x00000001,                  // 数据包类型标识
                                No = (uint)sequence,                            // 初始包序号
                                L = 37,                            // 数据域长度（29字节）
                                UAVId = mav.sysid,                         // 无人机编号
                                Longitude = (int)(mav.cs.lng * 1e6),     // 经度：东经116.4度
                                Latitude = (int)(mav.cs.lat * 1e6),       // 纬度：北纬40.0度
                                RelativeHeight = (Int16)(mav.cs.alt * 10),                // 相对高度（0.1m单位）
                                Altitude = ConvertVelocity(mav.cs.altasl),                   // 海拔高度（0.1m单位）
                                //Altitude = ConvertVelocity(5),                   // 海拔高度（0.1m单位）
                                GPSTime = (Int64)ticks,               // GPS时间：103.44小时（示例值）
                                Heading = (Int32)(mav.cs.yaw * 10),                     // 方向角（0.1度单位）
                                //EastVelocity = ConvertVelocity(-2.00),                // 东向速度（0.1单位）
                                //NorthVelocity = ConvertVelocity(-2),                // 北向速度（0.1单位）
                                //VerticalVelocity = ConvertVelocity(-2.0),             // 垂向速度（0.1单位）
                                EastVelocity = ConvertVelocity(mav.cs.vy),                // 东向速度（0.1单位）
                                NorthVelocity = ConvertVelocity(mav.cs.vx),                // 北向速度（0.1单位）
                                VerticalVelocity = ConvertVelocity(mav.cs.vz),             // 垂向速度（0.1单位）
                                GPSSatellites = (sbyte)(mav.cs.satcount) ,              // GPS搜星数量
                                Batteruy_V = (Int16)(mav.cs.battery_voltage * 100),     //单位 *100
                                Failsafe = (byte)(((bool)mav.cs.failsafe) == true ? 1 : 0),
                                Gpsstatus = (byte)(mav.cs.gpsstatus)     //gps状态,大于等于3就可以了

                            };
                            // 打印参数日志
                        //    Console.WriteLine("📊 PDXP 数据包字段解析：");
                        //    Console.WriteLine($"VER: {pdxpPacket.VER}        // 协议版本");
                        //    Console.WriteLine($"MID: 0x{pdxpPacket.MID:X2}   // 任务代号");
                        //    Console.WriteLine($"SID: 0x{pdxpPacket.SID:X4}   // 发送方地址");
                        //    Console.WriteLine($"DID: 0x{pdxpPacket.DID:X4}   // 接收方地址");
                        //    Console.WriteLine($"BID: 0x{pdxpPacket.BID:X8}   // 数据包类型标识");
                        //    Console.WriteLine($"No: {pdxpPacket.No}          // 包序号");
                        //    Console.WriteLine($"L: {pdxpPacket.L}            // 数据域长度（字节）");
                        //    Console.WriteLine($"UAVId: {pdxpPacket.UAVId}    // 无人机编号（sysid={mav.sysid}）");
                        //    Console.WriteLine($"Longitude: {pdxpPacket.Longitude}  // 经度 ×1e6 → {mav.cs.lng:F6}°");
                        //    Console.WriteLine($"Latitude: {pdxpPacket.Latitude}   // 纬度 ×1e6 → {mav.cs.lat:F6}°");
                        //    Console.WriteLine($"RelativeHeight: {pdxpPacket.RelativeHeight}  // 相对高度 ×10 → {mav.cs.alt:F2}m");
                        //    Console.WriteLine($"Altitude: {pdxpPacket.Altitude}              // 海拔高度 ×10 → {mav.cs.altasl:F2}m");
                        //    Console.WriteLine($"GPSTime: {pdxpPacket.GPSTime}               // GPS 时间戳（0.1ms）");
                        //    Console.WriteLine($"Heading: {pdxpPacket.Heading}               // 方向角 ×10 → {mav.cs.yaw:F1}°");
                        //    Console.WriteLine($"EastVelocity: {pdxpPacket.EastVelocity}     // 东向速度 ×10 → {mav.cs.vy:F1}m/s");
                        //    Console.WriteLine($"NorthVelocity: {pdxpPacket.NorthVelocity}   // 北向速度 ×10 → {mav.cs.vx:F1}m/s");
                        //    Console.WriteLine($"VerticalVelocity: {pdxpPacket.VerticalVelocity}  // 垂向速度 ×10 → {mav.cs.vz:F1}m/s");
                        //    Console.WriteLine($"GPSSatellites: {pdxpPacket.GPSSatellites}   // GPS 搜星数量（satcount={mav.cs.satcount}）");
                            
                        //    Console.WriteLine($"Batteruy_V: {pdxpPacket.Batteruy_V}   // 电池电压 ×100 → {mav.cs.battery_voltage:F2}V");
                        //    Console.WriteLine($"Failsafe: {pdxpPacket.Failsafe}   // 是否进入故障安全模式（{mav.cs.failsafe}）");
                        //    Console.WriteLine($"Gpsstatus: {pdxpPacket.Gpsstatus}   // GPS 状态（{mav.cs.gpsstatus:F0}，≥3 表示正常）");
                        //Console.WriteLine("--------------------------------------------------");

                            // 将结构体转换为字节数组
                            byte[] packetBytes = StructToBytes(pdxpPacket);

                            // 发送UDP数据包
                            udpClient.Send(packetBytes, packetBytes.Length, endPoint);
                        }
                    }
                   

                    System.Threading.Thread.Sleep(1000);  // 每 100ms 发送一次
               
            }
            // 线程停止时的清理工作
            Console.WriteLine("线程已停止.");
        }
        
        public static Int16 ConvertVelocity(double vx)
        {
            double scaled = vx * 10; // 转换为 0.1m/s 单位
            //return (Int16)(scaled >= 0 ? Math.Floor(scaled + 0.5) : Math.Ceiling(scaled - 0.5));
            return (Int16)Math.Round(scaled, MidpointRounding.AwayFromZero);
            //return (Int16)(scaled >= 0 ? Math.Floor(scaled) : Math.Ceiling(scaled));

        }
        //{"类型“MissionPlanner.Udp+Register”不能作为非托管结构进行封送处理；无法计算有意义的大小或偏移量。"}
        public static byte[] StructToBytes(object structObj)
        {
            int size = Marshal.SizeOf(structObj);
            IntPtr buffer = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(structObj, buffer, false);
                byte[] bytes = new byte[size];
                Marshal.Copy(buffer, bytes, 0, size);
                return bytes;
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }
        private void receivemessage()
        {
            while (true)
            {
                try
                {
                    

                   
                }
                catch
                {

                }
            }
        }
    }

   
}
