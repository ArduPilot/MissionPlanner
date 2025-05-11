using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using static Community.CsharpSqlite.Sqlite3;
using static IronPython.Modules._ast;
using static MissionPlanner.Udp;
using DateTime = System.DateTime;

namespace MissionPlanner
{
    public partial class Udp : Form
    {
        public Udp()
        {
            InitializeComponent();
            countdown.Elapsed += sendheartbeat;
            senddata.Elapsed += senddt;
            ten.Elapsed += sensorsend;
            instance = this;
        }

        public static Udp instance = null;

        IPEndPoint endPoint;

        IPEndPoint endPointudp;

        Thread receive;

        System.Timers.Timer countdown = new System.Timers.Timer { Interval = 10000, AutoReset = true };

        System.Timers.Timer senddata = new System.Timers.Timer { Interval = 1000, AutoReset = true };

        System.Timers.Timer ten = new System.Timers.Timer { Interval = 10000, AutoReset = true };

        private void myButton2_Click(object sender, EventArgs e)
        {
            if(myButton2.Text == "连接")
            {
                endPoint = new IPEndPoint(IPAddress.Parse(textBox1.Text), int.Parse(textBox3.Text));
                udpClient = new UdpClient(6000);
               
                receive =  new Thread(receivemessage);
                receive.Start();
                countdown.Start();
                myButton2.Text = "断开连接";
            }
            else
            {
                countdown.Stop();
                udpClient.Close();
                myButton2.Text = "连接";
            }
        }


        private void udp_closing(object sender,FormClosingEventArgs e)
        {
            if(udpClient != null)
            {
                udpClient.Close();
            }
            //if (udpzk != null)
            //{
            //    udpzk.Close();
            //}
            countdown.Stop();
            senddata.Stop();
            ten.Stop();
        }
        UInt32 shiliid;
        byte shiyanbianhao;
        string shiyanminchen = string.Empty;
        string xiangdingname = string.Empty;
        bool missreceive = false;

        byte guankongcommand;

        bool guankongstatus = false;

        byte zhilin;
        UInt32 ID;
        UInt32 yuanid;
        Int32 lng;
        Int32 lat;
        Int32 alt;
        byte xingzhung;
        Int32 radius;
        Int32 length;
        Int32 width;
        UInt16 len;

        /// <summary>
        /// 实验解除
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Lift
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] infoType; // 信息类型编号 "06 00"

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] senderCode; // 发送方编码 "01 00 01 00"

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] receiverCode; // 接收方编码 "00 00 00 13"

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] sequenceNumber; // 流水号

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] createTime; // 创建时间

            public byte totalPackages; // UDP总包数 "01"

            public byte currentPackage; // UDP当前包序号 "01"

            public ushort dataLength; // 数据长度 (uint16): CA (202个字节)
           
            public byte nodetype; // 实验节点类型 (int8): 16

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
            public byte[] nodeName; // 实验节点名称 (char[100])      
        }


        private void receivemessage()
        {
            while (true)
            {
                try
                {
                    IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    byte[] data = new byte[4096];
                    data = udpClient.Receive(ref remoteEndPoint);//此方法把数据来源ip、port放到第二个参数中
                    
                    if (data[0] == 0x06 && data[1] == 0x00)
                    {
                        UdpPacket packet = ByteArrayToStructure<UdpPacket>(data);
                        if (packet.registrationResult == 0x01)
                        {
                            MessageBox.Show("注册成功！");                         
                        }
                        if (packet.registrationResult == 0x02)
                        {
                            string error = Encoding.UTF8.GetString(packet.rejectionReason);
                            MessageBox.Show(error, "注册失败");
                        }
                    }
                    
                    if (data[0] == 0x01 && data[1] == 0x00)
                    {
                        expre packet = ByteArrayToStructure<expre>(data);
                        shiliid = BitConverter.ToUInt32(packet.shiid,0);
                        shiyanbianhao = packet.shisysid;
                        shiyanminchen = Encoding.UTF8.GetString(packet.nodeName).TrimEnd('\0'); ;
                        xiangdingname = Encoding.UTF8.GetString(packet.xiangding).TrimEnd('\0');                             ;
                        MessageBox.Show("试验实例ID:" + shiliid.ToString() + "\n" + "试验想定编号:" + shiyanbianhao + "\n" + "试验名称:" + shiyanminchen + "\n" + "试验想定名称:" + xiangdingname);
                        missreceive = true;
                    }
                    if (data[0] == 0x03 && data[1] == 0x00)
                    {
                        guankong packet = ByteArrayToStructure<guankong>(data);
                        guankongcommand = packet.fankui;
                        if(packet.fankui == 0x04)
                        {
                            MessageBox.Show("管控结束");
                        }
                        if (packet.fankui == 0x01)
                        {
                            MessageBox.Show("管控开始");
                        }
                        guankongstatus = true;
                    }
                    if (data[0] == 0x07 && data[1] == 0x00)
                    {
                        MessageBox.Show("试验接入解除");
                    }
                    if (data[0] == 0x0B && data[1] == 0x00)
                    {
                        mission packet = ByteArrayToStructure<mission>(data);
                        missionid = BitConverter.ToUInt32(packet.shiid, 0);
                        shiyanbianhao = packet.nodetype;
                        missionsysid = packet.shisysid;
                        missionname = Encoding.UTF8.GetString(packet.nodeName);
                        MessageBox.Show("试验实例ID:" + missionid.ToString() + "\n" + "试验节点类型:" + shiyanbianhao + "\n" + "试验任务编号:" + missionsysid + "\n" + "试验任务名称:" + missionname);
                        qudong = true;
                    }

                    if (data[0] == 0x01 && data[1] == 0x40)
                    {
                        changjing packet = ByteArrayToStructure<changjing>(data);
                        zhilin = packet.type;
                        ID = packet.ID;
                        len = packet.len;
                        yuanid = packet.yuanID;
                        lng = packet.lng;
                        lat = packet.lat;
                        alt = packet.alt;
                        xingzhung = packet.xingzhuang;
                        radius = packet.radius;
                        length = packet.length;
                        width = packet.width;
                        MessageBox.Show("导调指令类型:新增禁飞区" + "\n" + "ID:" + ID.ToString() + "\n" + "复制源ID:" + yuanid.ToString() + "\n" + "经度:" + (lng*1e-6).ToString()
                           + "\n" + "纬度:" + (lat*1e-6).ToString() +"\n" + "高度:" + (alt/100).ToString() + "\n" + "形状:圆形"  + "\n" + "半径:" + (radius/100).ToString() + "\n" + "长度:" + (length/100).ToString()+ "\n" +
                           "宽度:" + (width/100).ToString());
                        //GCSViews.FlightPlanner.instance.addmarkertar("",packet.lng * 1e-6,packet.lat * 1e-6,0,packet.radius/100);
                    }
                }
                catch
                {

                }
            }
        }

        private T ByteArrayToStructure<T>(byte[] data) where T : struct
        {
            IntPtr ptr = Marshal.AllocHGlobal(data.Length);
            try
            {
                Marshal.Copy(data, 0, ptr, data.Length);
                return (T)Marshal.PtrToStructure(ptr, typeof(T));
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct UdpPacket
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] infoType; // 信息类型编号 "06 00"

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] senderCode; // 发送方编码 "01 00 01 00"

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] receiverCode; // 接收方编码 "00 00 00 13"

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] sequenceNumber; // 流水号

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] createTime; // 创建时间

            public byte totalPackages; // UDP总包数 "01"

            public byte currentPackage; // UDP当前包序号 "01"

            public ushort dataLength; // 数据长度 (uint16): CA (202个字节)

            public byte experimentNodeType; // 实验节点类型 (int8): 16

            public byte registrationResult; // 节点注册结果 (int8): 1成功，2拒绝

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
            public byte[] nodeName; // 实验节点名称 (char[100])

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
            public byte[] rejectionReason; // 拒绝注册原因 (char[100])
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct expre
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] infoType; // 信息类型编号 "06 00"

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] senderCode; // 发送方编码 "01 00 01 00"

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] receiverCode; // 接收方编码 "00 00 00 13"

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] sequenceNumber; // 流水号

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] createTime; // 创建时间

            public byte totalPackages; // UDP总包数 "01"

            public byte currentPackage; // UDP当前包序号 "01"

            public ushort dataLength; // 数据长度 (uint16): CA (202个字节)

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] shiid; // 流水号

            public byte shisysid; // 实验节点类型 (int8): 16

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
            public byte[] nodeName; // 实验节点名称 (char[100])

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
            public byte[] xiangding;
        }


        private UdpClient udpClient;

        //private UdpClient udpzk;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct heartbeat
        {
            public UInt16 bianhao;
            public UInt32 send;
            public UInt32 receive;
            public UInt32 count;
            public Int64 time;
            public byte udpcount;
            public byte currentudp;
            public UInt16 len;
            public byte type;
            public byte state;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct mavlink_data
        {
            public UInt16 head1;
            public UInt32 send;
            public UInt32 receive;
            public UInt32 liushui;
            public UInt64 time;
            public byte count;
            public byte xuhao;
            public UInt16 len;
            public UInt64 datetime;
            public UInt32 pingtai;
            public Int32 lng;
            public Int32 lat;
            public Int32 alt;
            public Int16 speed;
            public Int32 yaw;
            public Int16 roll;
            public Int16 pitch;
            public byte planecount;
            public byte ptype;
            public Int16 xinghao;
            public UInt32 zhikong;
            public UInt32 leader;
            public byte mission;
            public byte remain;
            public byte zhonglei;
            public byte tcount;
            public byte zhengji;
            public byte guadian;
            public byte hongwai;
            public byte jiguang;
            public byte kejian;
            public byte guandao;
            public byte tongxin;
            public byte gps;
            public byte beidou;
        }

        public static long GetMillisecondsSinceEpoch()
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            long millis = DateTime.UtcNow.Ticks / 10000; // Ticks are 100-nanosecond intervals
            return millis - epoch.Ticks / 10000;
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

        UInt32 heartcount = 0;
        /// <summary>
        /// 发送心跳包
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sendheartbeat(object sender, ElapsedEventArgs e)
        {
            try
            {
                heartcount++;
                heartbeat data = new heartbeat();
                data.bianhao = BitConverter.ToUInt16(new byte[2] { 0x08, 0x00 }, 0);
                data.send = BitConverter.ToUInt32(new byte[4] { 0x00, 0x00, 0x00, 0x13 }, 0);
                data.receive = BitConverter.ToUInt32(new byte[4] { 0x01, 0x00, 0x01, 0x00 }, 0);
                data.count = heartcount;
                data.time = (Int64)GetMillisecondsSinceEpoch();
                data.udpcount = 0x01;
                data.currentudp = 0x01;
                data.len = BitConverter.ToUInt16(new byte[2] { 0x02, 0x00 }, 0);
                data.type = 0x16;
                data.state = 0x00;
                byte[] send = StructToBytes(data);
                //Socket udpClient = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                //udpClient.SendTo(send, send.Length, SocketFlags.None, endPoint);
                udpClient.Send(send,send.Length,endPoint);
            }
            catch
            {

            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct target
        {
            public UInt16 head1;
            public UInt32 send;
            public UInt32 receive;
            public UInt32 liushui;
            public UInt64 time;
            public byte count;
            public byte xuhao;
            public UInt16 len;
            public UInt64 datetime;
            public UInt32 pingtai;
            public Int32 lng;
            public Int32 lat;
            public Int32 alt;
            public Int16 speed;
            public Int32 yaw;
            public Int16 roll;
            public Int16 pitch;
            public byte leixing;
        }

        /// <summary>
        /// 发送主机 1 16
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        UInt32 dtcount;
        private void senddt(object sender, ElapsedEventArgs e)
        {
            foreach (var port in MainV2.Comports)
            {
                foreach (var mav in port.MAVlist)
                {
                    if (mav.sysid == 1)
                    {
                        dtcount++;
                        target data = new target();
                        data.head1 = BitConverter.ToUInt16(new byte[2] { 0x00, 0x10 }, 0);
                        data.send = BitConverter.ToUInt32(new byte[4] { 0x00, 0x00, 0x00, 0x13 }, 0);
                        data.receive = BitConverter.ToUInt32(new byte[4] { 0x01, 0x00, 0x01, 0x00 }, 0);
                        data.liushui = dtcount;
                        data.time = (ulong)GetMillisecondsSinceEpoch();
                        data.count = 0x01;
                        data.xuhao = 0x01;
                        data.len = BitConverter.ToUInt16(new byte[2] { 0x23, 0x00 }, 0);
                        data.datetime = (ulong)GetMillisecondsSinceEpoch();
                        data.pingtai = BitConverter.ToUInt32(new byte[4] { 0x01, 0x00, 0x00, 0x00 }, 0);
                        data.lng = ((int)(mav.cs.lng * 1e6));
                        data.lat = (Int32)(mav.cs.lat * 1e6);
                        if (mav.cs.alt < 0)
                        {
                            data.alt = 0;
                        }
                        else
                        {
                            data.alt = (Int32)(mav.cs.alt * 100);
                        }
                        data.speed = (Int16)(mav.cs.groundspeed * 100);                      
                        data.yaw = (Int32)(mav.cs.yaw * 10);
                        data.roll = (Int16)(mav.cs.roll * 10);
                        data.pitch = (Int16)(mav.cs.pitch * 10);
                        data.leixing = 0x05;

                        byte[] send = StructToBytes(data);
                        //Socket udpClient = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                        udpClient.Send(send, send.Length,endPoint);
                    }
                    if(mav.sysid == 16)
                    {
                        dtcount++;
                        target data = new target();
                        data.head1 = BitConverter.ToUInt16(new byte[2] { 0x00, 0x10 }, 0);
                        data.send = BitConverter.ToUInt32(new byte[4] { 0x00, 0x00, 0x00, 0x13 }, 0);
                        data.receive = BitConverter.ToUInt32(new byte[4] { 0x01, 0x00, 0x01, 0x00 }, 0);
                        data.liushui = dtcount;
                        data.time = (ulong)GetMillisecondsSinceEpoch();
                        data.count = 0x01;
                        data.xuhao = 0x01;
                        data.len = BitConverter.ToUInt16(new byte[2] { 0x23, 0x00 }, 0);
                        data.datetime = (ulong)GetMillisecondsSinceEpoch();
                        data.pingtai = BitConverter.ToUInt32(new byte[4] { 0x02, 0x00, 0x00, 0x00 }, 0);
                        data.lng = ((int)(mav.cs.lng * 1e6));
                        data.lat = (Int32)(mav.cs.lat * 1e6);
                        if (mav.cs.alt < 0)
                        {
                            data.alt = 0;
                        }
                        else
                        {
                            data.alt = (Int32)(mav.cs.alt * 100);
                        }
                        data.speed = (Int16)(mav.cs.groundspeed * 100);
                        data.yaw = (Int32)(mav.cs.yaw * 10);
                        data.roll = (Int16)(mav.cs.roll * 10);
                        data.pitch = (Int16)(mav.cs.pitch * 10);
                        data.leixing = 0x05;

                        byte[] send = StructToBytes(data);
                        //Socket udpClient = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                        udpClient.Send(send, send.Length, endPoint);
                    }
                }
            }
            sendtarmon();
            //sendwaypoint();
        }


        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct targetmon
        {
            public UInt16 head1;
            public UInt32 send;
            public UInt32 receive;
            public UInt32 liushui;
            public UInt64 time;
            public byte count;
            public byte xuhao;
            public UInt16 len;
            public UInt64 datetime;         
            public UInt32 yuan;
            public UInt32 targetsign;
            public Int32 tartype;
            public byte tarmission;
            public byte tarcount;
            public Int32 lng;
            public Int32 lat;
            public Int32 alt;
            public Int16 speed;
            public Int32 yaw;
            
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct targetmontwo
        {
            public UInt16 head1;
            public UInt32 send;
            public UInt32 receive;
            public UInt32 liushui;
            public UInt64 time;
            public byte count;
            public byte xuhao;
            public UInt16 len;
            public UInt64 datetime;
            public UInt32 yuan;
            public UInt32 targetsign;
            public Int32 tartype;
            public byte tarmission;
            public byte tarcount;
            public Int32 lng;
            public Int32 lat;
            public Int32 alt;
            public Int16 speed;
            public Int32 yaw;

        }

        UInt32 moncount;
        void sendtarmon()
        {
            foreach (var port in MainV2.Comports)
            {
                foreach (var mav in port.MAVlist)
                {
                    if (mav.sysid == 1)
                    {
                        moncount++;
                        targetmon data = new targetmon();
                        data.head1 = BitConverter.ToUInt16(new byte[2] { 0x01, 0x20 }, 0);
                        data.send = BitConverter.ToUInt32(new byte[4] { 0x00, 0x00, 0x00, 0x13 }, 0);
                        data.receive = BitConverter.ToUInt32(new byte[4] { 0x01, 0x00, 0x01, 0x00 }, 0);
                        data.liushui = moncount;
                        data.time = (ulong)GetMillisecondsSinceEpoch();
                        data.count = 0x01;
                        data.xuhao = 0x01;
                        data.len = BitConverter.ToUInt16(new byte[2] { 0x28, 0x00 }, 0);
                        data.datetime = (ulong)GetMillisecondsSinceEpoch();
                        data.yuan = BitConverter.ToUInt32(new byte[4] { 0x08, 0x00, 0x00, 0x13 }, 0);
                        data.targetsign = BitConverter.ToUInt32(new byte[4] { 0x01, 0x00, 0x00, 0x00 }, 0);
                        data.tartype = BitConverter.ToInt32(new byte[4] { 0x05, 0x00, 0x00, 0x00 }, 0);
                        data.tarmission = 0x03;
                        data.tarcount = 0x01;
                        data.lng = ((int)(mav.cs.lng * 1e6));
                        data.lat = (Int32)(mav.cs.lat * 1e6);
                        if(mav.cs.alt < 0)
                        {
                            data.alt = 0;
                        }
                        else
                        {
                            data.alt = (Int32)(mav.cs.alt * 100);
                        }
                
                        data.speed = (Int16)(mav.cs.groundspeed * 100);
                        data.yaw = (Int32)(mav.cs.yaw * 10);
                        
                        byte[] send = StructToBytes(data);
                        //Socket udpClient = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                        udpClient.Send(send, send.Length, endPoint);
                    }
                    if(mav.sysid == 16)
                    {
                        targetmontwo data = new targetmontwo();
                        data.head1 = BitConverter.ToUInt16(new byte[2] { 0x01, 0x20 }, 0);
                        data.send = BitConverter.ToUInt32(new byte[4] { 0x00, 0x00, 0x00, 0x13 }, 0);
                        data.receive = BitConverter.ToUInt32(new byte[4] { 0x01, 0x00, 0x01, 0x00 }, 0);
                        data.liushui = moncount;
                        data.time = (ulong)GetMillisecondsSinceEpoch();
                        data.count = 0x01;
                        data.xuhao = 0x01;
                        data.len = BitConverter.ToUInt16(new byte[2] { 0x28, 0x00 }, 0);
                        data.datetime = (ulong)GetMillisecondsSinceEpoch();
                        data.yuan = BitConverter.ToUInt32(new byte[4] { 0x17, 0x00, 0x00, 0x13 }, 0);
                        data.targetsign = BitConverter.ToUInt32(new byte[4] { 0x02, 0x00, 0x00, 0x00 }, 0);
                        data.tartype = BitConverter.ToInt32(new byte[4] { 0x05, 0x00, 0x00, 0x00 }, 0);
                        data.tarmission = 0x03;
                        data.tarcount = 0x01;
                        data.lng = ((int)(mav.cs.lng * 1e6));
                        data.lat = (Int32)(mav.cs.lat * 1e6);
                        if (mav.cs.alt < 0)
                        {
                            data.alt = 0;
                        }
                        else
                        {
                            data.alt = (Int32)(mav.cs.alt * 100);
                        }
                        data.speed = (Int16)(mav.cs.groundspeed * 100);
                        data.yaw = (Int32)(mav.cs.yaw * 10);

                        byte[] send = StructToBytes(data);
                        //Socket udpClient = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                        udpClient.Send(send, send.Length, endPoint);
                    }
                }
            }
            start();
            sendtask();
            sendtask2();
        }

        UInt32 startcounts;
        private void start()
        {           
            foreach (var port in MainV2.Comports)
            {
                foreach (var mav in port.MAVlist)
                {
                    if (mav.sysid != 1 && mav.sysid != 16)
                    {
                        startcounts++;
                        mavlink_data data = new mavlink_data();
                        data.head1 = BitConverter.ToUInt16(new byte[2] { 0x01, 0x10 }, 0);
                        data.send = BitConverter.ToUInt32(new byte[4] { 0x00, 0x00, 0x00, 0x13 }, 0);
                        data.receive = BitConverter.ToUInt32(new byte[4] { 0x01, 0x00, 0x01, 0x00}, 0);
                        data.liushui = startcounts;
                        data.time = (ulong)GetMillisecondsSinceEpoch();
                        data.count = 0x01;
                        data.xuhao = 0x01;
                        data.len = BitConverter.ToUInt16(new byte[2] { 0x3B, 0x00 }, 0);
                        data.datetime = (ulong)GetMillisecondsSinceEpoch();
                        data.pingtai = BitConverter.ToUInt32(new byte[4] { mav.sysid, 0x00, 0x00, 0x13 }, 0);
                        data.lng = ((Int32)(mav.cs.lng * 1e6));
                        data.lat = (Int32)(mav.cs.lat * 1e6);
                        if (mav.cs.alt < 0)
                        {
                            data.alt = 0;
                        }
                        else
                        {
                            data.alt = (Int32)(mav.cs.alt * 100);
                        }
                        data.speed = (Int16)(mav.cs.groundspeed * 100);                       
                        data.yaw = (Int32)(mav.cs.yaw * 100);                            
                        data.roll = (Int16)(mav.cs.roll * 10);
                        data.pitch = (Int16)(mav.cs.pitch * 10);
                        data.planecount = 0x1C;
                        data.ptype = 0x02;
                        data.xinghao = BitConverter.ToInt16(new byte[2] { 0x0C, 0x00 }, 0);
                        data.zhikong = BitConverter.ToUInt32(new byte[4] { 0x00, 0x00, 0x00, 0x13 }, 0);
                        data.leader = BitConverter.ToUInt32(new byte[4] { 0x08, 0x00, 0x00, 0x13 }, 0);
                        data.mission = 0x03;
                        data.remain = (byte)MainV2.comPort.MAV.cs.battery_remaining;
                        data.zhonglei = 0x3F;
                        data.tcount = 0x00;
                        data.zhengji = 0x00;
                        data.guadian = 0x00;
                        data.hongwai = 0x00;
                        data.jiguang = 0x00;
                        data.kejian = 0x00;
                        data.guandao = 0x00;
                        data.tongxin = 0x00;
                        data.gps = 0x00;
                        data.beidou = 0x00;

                        byte[] send = StructToBytes(data);
                        Socket udpClient = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                        udpClient.SendTo(send, send.Length, SocketFlags.None, endPoint);
                    }
                }
            }
        }


        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Task
        {
            public UInt16 head1;
            public UInt32 send;
            public UInt32 receive;
            public UInt32 liushui;
            public UInt64 time;
            public byte count;
            public byte xuhao;
            public UInt16 len;
            public UInt64 datetime;
            public UInt32 pingtai;
            public byte command;
            public UInt32 tarbianshi;
            public Int32 tartype;
            public Int32 tarmodel;
            public byte tarcount;
            public Int32 enteryaw;
            public Int32 leaveyaw;
            public byte manual;
        }

        UInt32 task = 0;
        private void sendtask()
        {
            foreach (var port in MainV2.Comports)
            {
                foreach (var mav in port.MAVlist)
                {
                    if (mav.sysid >=2 && mav.sysid <= 15)
                    {
                        task++;
                        Task data = new Task();
                        data.head1 = BitConverter.ToUInt16(new byte[2] { 0x04, 0x20 }, 0);
                        data.send = BitConverter.ToUInt32(new byte[4] { 0x00, 0x00, 0x00, 0x13 }, 0);
                        data.receive = BitConverter.ToUInt32(new byte[4] { 0x01, 0x00, 0x01, 0x00 }, 0);
                        data.liushui = task;
                        data.time = (ulong)GetMillisecondsSinceEpoch();
                        data.count = 0x01;
                        data.xuhao = 0x01;
                        data.len = BitConverter.ToUInt16(new byte[2] { 0x23, 0x00 }, 0);
                        data.datetime = (ulong)GetMillisecondsSinceEpoch();
                        data.pingtai = BitConverter.ToUInt32(new byte[4] { mav.sysid, 0x00, 0x00, 0x13 }, 0);
                        data.command = 0x03;
                        data.tarbianshi = BitConverter.ToUInt32(new byte[4] { 0x01 ,0x00, 0x00, 0x00 }, 0);
                        data.tartype = BitConverter.ToInt32(new byte[4] { 0x05, 0x00, 0x00, 0x00 }, 0);
                        data.tarmodel = BitConverter.ToInt32(new byte[4] { 0x00, 0x00, 0x00, 0x00 }, 0);
                        data.tarcount = 0x01;
                        data.enteryaw = 0;
                        data.leaveyaw = 0;
                        data.manual = 0x00;
                        byte[] send = StructToBytes(data);
                        Socket udpClient = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                        udpClient.SendTo(send, send.Length, SocketFlags.None, endPoint);
                    }
                }
            }
        }

        UInt32 task2count = 0;
        private void sendtask2()
        {
            foreach (var port in MainV2.Comports)
            {
                foreach (var mav in port.MAVlist)
                {
                    if (mav.sysid >= 17 && mav.sysid <= 30)
                    {
                        task2count++;
                        Task data = new Task();
                        data.head1 = BitConverter.ToUInt16(new byte[2] { 0x04, 0x20 }, 0);
                        data.send = BitConverter.ToUInt32(new byte[4] { 0x00, 0x00, 0x00, 0x13 }, 0);
                        data.receive = BitConverter.ToUInt32(new byte[4] { 0x01, 0x00, 0x01, 0x00 }, 0);
                        data.liushui = task2count;
                        data.time = (ulong)GetMillisecondsSinceEpoch();
                        data.count = 0x01;
                        data.xuhao = 0x01;
                        data.len = BitConverter.ToUInt16(new byte[2] { 0x23, 0x00 }, 0);
                        data.datetime = (ulong)GetMillisecondsSinceEpoch();
                        data.pingtai = BitConverter.ToUInt32(new byte[4] { mav.sysid, 0x00, 0x00, 0x13 }, 0);
                        data.command = 0x03;
                        data.tarbianshi = BitConverter.ToUInt32(new byte[4] { 0x02, 0x00, 0x00, 0x00 }, 0);
                        data.tartype = BitConverter.ToInt32(new byte[4] { 0x05, 0x00, 0x00, 0x00 }, 0);                    
                        data.tarmodel = BitConverter.ToInt32(new byte[4] { 0x00, 0x00, 0x00, 0x00 }, 0);
                        data.tarcount = 0x01;
                        data.enteryaw = 0;
                        data.leaveyaw = 0;
                        data.manual = 0x00;
                        byte[] send = StructToBytes(data);
                        Socket udpClient = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                        udpClient.SendTo(send, send.Length, SocketFlags.None, endPoint);
                    }
                }
            }

        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct sensorone
        {
            public UInt16 head1;
            public UInt32 send;
            public UInt32 receive;
            public UInt32 liushui;
            public UInt64 time;
            public byte count;
            public byte xuhao;
            public UInt16 len;
            public UInt64 datetime;
            public UInt32 pingtai;
            public byte sensornum;
            public byte ir;
            public byte jiguang;
            public byte sar;
            public UInt32 sarfen;
            public UInt32 mountfocal;
            public Int32 mountpitch;
            public Int32 mountyaw;
        }

        UInt32 sensorcount = 0;
        void sendsensor()
        {
            sensorcount++;
            sensorone data = new sensorone();
            data.head1 = BitConverter.ToUInt16(new byte[2] { 0x06, 0x20 }, 0);
            data.send = BitConverter.ToUInt32(new byte[4] { 0x00, 0x00, 0x00, 0x13 }, 0);
            data.receive = BitConverter.ToUInt32(new byte[4] { 0x01, 0x00, 0x01, 0x00 }, 0);
            data.liushui = sensorcount;
            data.time = (ulong)GetMillisecondsSinceEpoch();
            data.count = 0x01;
            data.xuhao = 0x01;
            data.len = BitConverter.ToUInt16(new byte[2] { 0x23, 0x00 }, 0);
            data.datetime = (ulong)GetMillisecondsSinceEpoch();
            data.pingtai = BitConverter.ToUInt32(new byte[4] {0x08, 0x00, 0x00, 0x13 }, 0);
            data.ir = 0x00;
            data.jiguang = 0x00;
            data.sar = 0x00;
            data.sarfen = 0x00;
            data.mountfocal = 0;
            data.mountpitch = 0;
            data.mountyaw = 0;
            byte[] send = StructToBytes(data);
            Socket udpClient = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            udpClient.SendTo(send, send.Length, SocketFlags.None, endPoint);            
        }

        UInt32 sensor2count = 0;
        void sendsensor2()
        {
            sensor2count++;
            sensorone data = new sensorone();
            data.head1 = BitConverter.ToUInt16(new byte[2] { 0x06, 0x20 }, 0);
            data.send = BitConverter.ToUInt32(new byte[4] { 0x00, 0x00, 0x00, 0x13 }, 0);
            data.receive = BitConverter.ToUInt32(new byte[4] { 0x01, 0x00, 0x01, 0x00 }, 0);
            data.liushui = sensor2count;
            data.time = (ulong)GetMillisecondsSinceEpoch();
            data.count = 0x01;
            data.xuhao = 0x01;
            data.len = BitConverter.ToUInt16(new byte[2] { 0x23, 0x00 }, 0);
            data.datetime = (ulong)GetMillisecondsSinceEpoch();
            data.pingtai = BitConverter.ToUInt32(new byte[4] { 0x11, 0x00, 0x00, 0x13 }, 0);
            data.ir = 0x00;
            data.jiguang = 0x00;
            data.sar = 0x00;
            data.sarfen = 0x00;
            data.mountfocal = 0;
            data.mountpitch = 0;
            data.mountyaw = 0;
            byte[] send = StructToBytes(data);
            Socket udpClient = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            udpClient.SendTo(send, send.Length, SocketFlags.None, endPoint);
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Register
        {
            public UInt16 bianhao;
            public UInt32 send;
            public UInt32 receive;
            public UInt32 count;
            public UInt64 time;
            public byte udpcount;
            public byte currentudp;
            public UInt16 len;
            public byte type;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public byte[] IP;
            public Int16 port;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
            public byte[] name;
        }
      
        uint regcount = 0;

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void myButton1_Click(object sender, EventArgs e)
        {
            regcount++;
            Register reg = new Register();
            reg.bianhao = BitConverter.ToUInt16(new byte[2] { 0x05, 0x00 }, 0);
            reg.send = BitConverter.ToUInt32(new byte[4] { 0x00, 0x00, 0x00, 0x13 }, 0);
            reg.receive = BitConverter.ToUInt32(new byte[4] { 0x01, 0x00, 0x01, 0x00 }, 0);
            reg.count = regcount;
            reg.time = (ulong)GetMillisecondsSinceEpoch();
            reg.udpcount = 0x01;
            reg.currentudp = 0x01;
            reg.len = BitConverter.ToUInt16(new byte[2] { 0x7B, 0x00 }, 0);
            reg.type = 0x16;
            //var host = Dns.GetHostEntry(Dns.GetHostName());
            reg.IP = new byte[20] { 0x31,0x38,0x30,0x2e,0x31,0x2E,0x38,0x30,0x2e,0x31,0x30,0x31,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00 };
            //string ip = "180.1.80.101";
            //byte[] ipBytes = Encoding.ASCII.GetBytes(ip);
            //reg.IP = Encoding.ASCII.GetBytes(ip);
            //int length = ipBytes.Length;
            
            //Array.Copy(ipBytes, reg.IP, Math.Min(length, reg.IP.Length));

            //for (int i = ipBytes.Length; i < reg.IP.Length; i++)
            //{
            //    reg.IP[i] = 0x20;
            //}

            reg.name = new byte[100];
            //foreach (var ipAddress in host.AddressList)
            //{
            //    if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            //    {
            //        Encoding.ASCII.GetBytes(ipAddress.ToString(), 0, Math.Min(ipAddress.ToString().Length, 20), reg.IP, 0);
            //        break;
            //    }
            //}

            reg.port = BitConverter.ToInt16(new byte[2] { 0x70, 0x17 }, 0);

            for (int i = 0; i < reg.name.Length; i++)
            {
                reg.name[i] = 0x00;
            }

            byte[] send = StructToBytes(reg);
            //Socket udpClient = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            //udpClient.SendTo(send, send.Length, SocketFlags.None, endPoint);
            if(udpClient != null)
            {
                udpClient.Send(send, send.Length, endPoint);
            }
            else
            {
                MessageBox.Show("请先连接");
            }
        }

        private void sensorsend(object sender, ElapsedEventArgs e)
        {
            sendsensor();
            sendsensor2();
        }
        /// <summary>
        /// 连接指控
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void myButton3_Click(object sender, EventArgs e)
        {
            if (myButton3.Text == "连接")
            {
                endPointudp = new IPEndPoint(IPAddress.Parse(textBox2.Text), int.Parse(textBox4.Text));
                //udpzk = new UdpClient(6000);

                //receivezk = new Thread(receivemessagezk);
                //receivezk.Start();
                myButton3.Text = "断开连接";
            }
            else
            {
                //udpzk.Close();
                myButton3.Text = "连接";
            }
        }
        /// <summary>
        /// 实验反馈
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        Thread receivezk;


        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Taskback
        {
            public UInt16 head1;
            public UInt32 send;
            public UInt32 receive;
            public UInt32 liushui;
            public UInt64 time;
            public byte count;
            public byte xuhao;
            public UInt16 len;
            public byte type;
            public UInt32 id;
            public byte qutype;
            public byte quback;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct missionback
        {
            public UInt16 head1;
            public UInt32 send;
            public UInt32 receive;
            public UInt32 liushui;
            public UInt64 time;
            public byte count;
            public byte xuhao;
            public UInt16 len;         
            public UInt32 id;
            public byte qutype;
        }

        /// <summary>
        /// 反馈
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        UInt32 qudongcount = 0;
        UInt32 misscount;
        private void myButton4_Click(object sender, EventArgs e)
        {
            if (udpClient != null)
            {
                if (qudong)
                {
                    qudongcount++;
                    Taskback data = new Taskback();
                    data.head1 = BitConverter.ToUInt16(new byte[2] { 0x0D, 0x00 }, 0);
                    data.send = BitConverter.ToUInt32(new byte[4] { 0x00, 0x00, 0x00, 0x13 }, 0);
                    data.receive = BitConverter.ToUInt32(new byte[4] { 0x00, 0x00, 0x00, 0x12 }, 0);
                    data.liushui = qudongcount;
                    data.time = (ulong)GetMillisecondsSinceEpoch();
                    data.count = 0x01;
                    data.xuhao = 0x01;
                    data.len = BitConverter.ToUInt16(new byte[2] { 0x07, 0x00 }, 0);
                    data.type = 0x16;
                    data.id = missionid;
                    data.qutype = 0x01;
                    data.quback = 0x01;
                    byte[] send = StructToBytes(data);                   
                    udpClient.Send(send, send.Length, endPointudp);                 
                }
                if (missreceive)
                {
                    misscount++;
                    missionback data = new missionback();
                    data.head1 = BitConverter.ToUInt16(new byte[2] { 0x02, 0x00 }, 0);
                    data.send = BitConverter.ToUInt32(new byte[4] { 0x00, 0x00, 0x00, 0x13 }, 0);
                    data.receive = BitConverter.ToUInt32(new byte[4] { 0x01, 0x00, 0x01, 0x00 }, 0);
                    data.liushui = misscount;
                    data.time = (ulong)GetMillisecondsSinceEpoch();
                    data.count = 0x01;
                    data.xuhao = 0x01;
                    data.len = BitConverter.ToUInt16(new byte[2] { 0x05, 0x00 }, 0);
                    data.id = shiliid;
                    data.qutype = 0x01;
                    byte[] send = StructToBytes(data);                 
                    udpClient.Send(send, send.Length, endPoint);                 
                }
                senddata.Start();
                ten.Start();
            }
            else
            {
                MessageBox.Show("请先连接");
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct mission
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] infoType; // 信息类型编号 "06 00"

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] senderCode; // 发送方编码 "01 00 01 00"

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] receiverCode; // 接收方编码 "00 00 00 13"

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] sequenceNumber; // 流水号

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] createTime; // 创建时间

            public byte totalPackages; // UDP总包数 "01"

            public byte currentPackage; // UDP当前包序号 "01"

            public ushort dataLength; // 数据长度 (uint16): CA (202个字节)

            public byte nodetype;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] shiid; // 流水号

            public byte shisysid; // 实验节点类型 (int8): 16

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
            public byte[] nodeName; // 实验节点名称 (char[100])           
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct guankong
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] infoType; // 信息类型编号 "06 00"

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] senderCode; // 发送方编码 "01 00 01 00"

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] receiverCode; // 接收方编码 "00 00 00 13"

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] sequenceNumber; // 流水号

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] createTime; // 创建时间

            public byte totalPackages; // UDP总包数 "01"

            public byte currentPackage; // UDP当前包序号 "01"

            public ushort dataLength; // 数据长度 (uint16): CA (202个字节)             

            public byte fankui; // 实验节点类型 (int8): 16     
        }

        UInt32 missionid;
        byte missionbianhao;
        byte missionsysid;
        string missionname = "";
        bool qudong = false;
        //private void receivemessagezk()
        //{
        //    while (true)
        //    {
        //        try
        //        {
        //            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
        //            byte[] data = new byte[4096];
        //            data = udpzk.Receive(ref remoteEndPoint);//此方法把数据来源ip、port放到第二个参数中

        //            if (data[0] == 0x0B && data[1] == 0x00)
        //            {
        //                mission packet = ByteArrayToStructure<mission>(data);
        //                missionid = BitConverter.ToUInt32(packet.shiid, 0);
        //                shiyanbianhao = packet.nodetype;
        //                missionsysid = packet.shisysid;
        //                missionname = Encoding.ASCII.GetString(packet.nodeName);
        //                MessageBox.Show("实验实例ID:" + missionid.ToString() + "\n" + "实验节点类型:" + shiyanbianhao + "\n" + "实验任务编号:" + missionsysid + "\n" + "实验任务名称:" + missionname);
        //                qudong = true;
        //            }
        //        }
        //        catch
        //        {

        //        }
        //    }
        //}


        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct waypoint
        {
            public UInt16 head1;
            public UInt32 send;
            public UInt32 receive;
            public UInt32 liushui;
            public UInt64 time;
            public byte count;
            public byte xuhao;
            public UInt16 len;
            public UInt64 datetime;
            public UInt32 pingtai;
            public Int16 id;
            public byte wpcount;
            public byte wpxuhao;
            public Int32 lng;
            public Int32 lat;
            public Int32 alt;
            public Int16 speed;
            public Int32 yaw;
        }

        UInt32 waypointcount = 0;
        //参考的地方
        void sendwaypoint()
        {
            if (udpClient != null)
            {
                for (int i = 0; i < GCSViews.FlightPlanner.instance.pointlist.Count; i++)
                {
                    waypointcount++;
                    waypoint data = new waypoint();
                    data.head1 = BitConverter.ToUInt16(new byte[2] { 0x05, 0x20 }, 0);
                    data.send = BitConverter.ToUInt32(new byte[4] { 0x00, 0x00, 0x00, 0x13 }, 0);
                    data.receive = BitConverter.ToUInt32(new byte[4] { 0x01, 0x00, 0x01, 0x00 }, 0);
                    data.liushui = waypointcount;
                    data.time = (ulong)GetMillisecondsSinceEpoch();
                    data.count = 0x01;
                    data.xuhao = 0x01;
                    data.len = BitConverter.ToUInt16(new byte[2] { 0x22, 0x00 }, 0);
                    data.datetime = (ulong)GetMillisecondsSinceEpoch();
                    data.pingtai = BitConverter.ToUInt32(new byte[4] { 0x01, 0x00, 0x00, 0x13 }, 0);
                    data.id = Int16.Parse(textBox5.Text);/*BitConverter.ToInt16(new byte[2] { 0x01, 0x00 }, 0);*/
                    data.wpcount = (byte)GCSViews.FlightPlanner.instance.pointlist.Count;
                    data.wpxuhao = (byte)(i + 1);
                    data.lng = ((Int32)(GCSViews.FlightPlanner.instance.pointlist[i].Lng * 1e6));
                    data.lat = (Int32)(GCSViews.FlightPlanner.instance.pointlist[i].Lat * 1e6);

                    if(i == 0)
                    {
                        data.alt = (int)0;
                    }
                    else
                    {
                        data.alt = (int)(GCSViews.FlightPlanner.instance.pointlist[i].Alt* 100 - float.Parse(GCSViews.FlightPlanner.instance.TXT_homealt.Text));
                    }
                    
                    Dictionary<string, double> param =
                       new Dictionary<string, double>((Dictionary<string, double>)MainV2.comPort.MAV.param);
                    if (param.ContainsKey("WPNAV_SPEED"))
                    {
                        data.speed = (short)(double)param["WPNAV_SPEED"];
                    }
                    else
                    {
                        data.speed = 0;
                    }
                    data.yaw = 0;
                    byte[] send = StructToBytes(data);
                    udpClient.Send(send, send.Length, endPoint);
                }
            }

            //foreach (var port in MainV2.Comports)
            //{
            //    foreach (var mav in port.MAVlist)
            //    {
            //        if (mav.sysid != 1 && mav.sysid !=16)
            //        {
            //            waypointcount++;
            //            waypoint data = new waypoint();
            //            data.head1 = BitConverter.ToUInt16(new byte[2] { 0x05, 0x20 }, 0);
            //            data.send = BitConverter.ToUInt32(new byte[4] { 0x00, 0x00, 0x00, 0x13 }, 0);
            //            data.receive = BitConverter.ToUInt32(new byte[4] { 0x01, 0x00, 0x01, 0x00 }, 0);
            //            data.liushui = waypointcount;
            //            data.time = (ulong)GetMillisecondsSinceEpoch();
            //            data.count = 0x01;
            //            data.xuhao = 0x01;
            //            data.len = BitConverter.ToUInt16(new byte[2] { 0x22, 0x00 }, 0);
            //            data.datetime = (ulong)GetMillisecondsSinceEpoch();
            //            data.pingtai = BitConverter.ToUInt32(new byte[4] { mav.sysid, 0x00, 0x00, 0x13 }, 0);
            //            data.id = BitConverter.ToInt16(new byte[2] { 0x01, 0x00 }, 0);
            //            data.wpcount = 0x01;
            //            data.wpxuhao = 0x01;
            //            data.lng = ((int)(mav.cs.lng * 1e6));
            //            data.lat = (Int32)(mav.cs.lat * 1e6);
            //            if (mav.cs.alt < 0)
            //            {
            //                data.alt = 0;
            //            }
            //            else
            //            {
            //                data.alt = (Int32)(mav.cs.alt * 100);
            //            }
            //            data.speed = (Int16)(mav.cs.groundspeed * 100);
            //            data.yaw = (Int32)(mav.cs.yaw * 10);

            //            byte[] send = StructToBytes(data);
            //            udpClient.Send(send, send.Length, endPoint);
            //        }
            //    }
            //}
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ystart
        {
            public UInt16 head1;
            public UInt32 send;
            public UInt32 receive;
            public UInt32 liushui;
            public UInt64 time;
            public byte count;
            public byte xuhao;
            public UInt16 len;
            public byte type;
            public byte ack;
        }
        /// <summary>
        /// 实验开始
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        UInt32 startcount;
        private void myButton5_Click(object sender, EventArgs e)
        {
            if (udpClient != null)
            {
                if (guankongstatus && guankongcommand == 0x01)
                {
                    startcount++;
                    ystart data = new ystart();
                    data.head1 = BitConverter.ToUInt16(new byte[2] { 0x04, 0x00 }, 0);
                    data.send = BitConverter.ToUInt32(new byte[4] { 0x00, 0x00, 0x00, 0x13 }, 0);
                    data.receive = BitConverter.ToUInt32(new byte[4] { 0x01, 0x00, 0x01, 0x00 }, 0);
                    data.liushui = startcount;
                    data.time = (ulong)GetMillisecondsSinceEpoch();
                    data.count = 0x01;
                    data.xuhao = 0x01;
                    data.len = BitConverter.ToUInt16(new byte[2] { 0x02, 0x00 }, 0);
                    data.type = guankongcommand;
                    data.ack = 0x01;
                    byte[] send = StructToBytes(data);                  
                    udpClient.Send(send, send.Length, endPoint);                
                    //senddata.Start();
                    //ten.Start();
                }
            }
            else
            {
                MessageBox.Show("请先连接");
            }
        }

        /// <summary>
        /// 试验结束反馈
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct yend
        {
            public UInt16 head1;
            public UInt32 send;
            public UInt32 receive;
            public UInt32 liushui;
            public UInt64 time;
            public byte count;
            public byte xuhao;
            public UInt16 len;
            public byte type;
            public byte ack;
        }
        UInt32 endcount;
        private void myButton6_Click(object sender, EventArgs e)
        {
            if (udpClient != null)
            {
                if (guankongstatus && guankongcommand == 0x04)
                {
                    endcount++;
                    yend data = new yend();
                    data.head1 = BitConverter.ToUInt16(new byte[2] { 0x04, 0x00 }, 0);
                    data.send = BitConverter.ToUInt32(new byte[4] { 0x00, 0x00, 0x00, 0x13 }, 0);
                    data.receive = BitConverter.ToUInt32(new byte[4] { 0x01, 0x00, 0x01, 0x00 }, 0);
                    data.liushui = endcount;
                    data.time = (ulong)GetMillisecondsSinceEpoch();
                    data.count = 0x01;
                    data.xuhao = 0x01;
                    data.len = BitConverter.ToUInt16(new byte[2] { 0x02, 0x00 }, 0);
                    data.type = guankongcommand;
                    data.ack = 0x01;
                    byte[] send = StructToBytes(data);                   
                    udpClient.Send(send, send.Length, endPoint);                                     
                }
            }
            else
            {
                MessageBox.Show("请先连接");
            }
        }

        UInt32 rtlcount;
        private void myButton7_Click(object sender, EventArgs e)
        {
            rtlcount++;
            yend data = new yend();
            data.head1 = BitConverter.ToUInt16(new byte[2] { 0x04, 0x00 }, 0);
            data.send = BitConverter.ToUInt32(new byte[4] { 0x00, 0x00, 0x00, 0x13 }, 0);
            data.receive = BitConverter.ToUInt32(new byte[4] { 0x01, 0x00, 0x01, 0x00 }, 0);
            data.liushui = rtlcount;
            data.time = (ulong)GetMillisecondsSinceEpoch();
            data.count = 0x01;
            data.xuhao = 0x01;
            data.len = BitConverter.ToUInt16(new byte[2] { 0x02, 0x00 }, 0);
            data.type = 0x05;
            data.ack = 0x01;
            byte[] send = StructToBytes(data);
            if (udpClient != null)
            {
                udpClient.Send(send, send.Length, endPoint);
            }
            else
            {
                MessageBox.Show("请先连接");
            }
        }

        /// <summary>
        /// 试验结束反馈
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct changjing
        {
            public UInt16 head1;
            public UInt32 send;
            public UInt32 receive;
            public UInt32 liushui;
            public UInt64 time;
            public byte count;
            public byte xuhao;
            public UInt16 len;
            public byte type;
            public UInt32 ID;
            public UInt32 yuanID;
            public Int32 lng;
            public Int32 lat;
            public Int32 alt;
            public byte xingzhuang;
            public Int32 radius;
            public Int32 length;
            public Int32 width;
            public byte ack;
        }

        UInt32 daocount;
        /// <summary>
        ///反馈导调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void myButton8_Click(object sender, EventArgs e)
        {
            daocount++;
            changjing data = new changjing();
            data.head1 = BitConverter.ToUInt16(new byte[2] { 0x01, 0x40 }, 0);
            data.send = BitConverter.ToUInt32(new byte[4] { 0x00, 0x00, 0x00, 0x13 }, 0);
            data.receive = BitConverter.ToUInt32(new byte[4] { 0x01, 0x00, 0x01, 0x00 }, 0);
            data.liushui = daocount;
            data.time = (ulong)GetMillisecondsSinceEpoch();
            data.count = 0x01;
            data.xuhao = 0x01;
            data.len = len;
            data.type = zhilin;
            data.ID = ID;
            data.yuanID = yuanid;
            data.lng = lng;
            data.lat = lat;
            data.alt = alt;
            data.xingzhuang = xingzhung;
            data.radius = radius;
            data.length = length;
            data.width = width;
            data.ack = 0x01;
            byte[] send = StructToBytes(data);
            if (udpClient != null)
            {
                udpClient.Send(send, send.Length, endPoint);
            }
            else
            {
                MessageBox.Show("请先连接");
            }
        }     

        private void myButton9_Click(object sender, EventArgs e)
        {
            sendwaypoint();
        }
    }
}
