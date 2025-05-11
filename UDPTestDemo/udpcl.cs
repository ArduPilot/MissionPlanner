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

namespace MissionPlanner
{
    public partial class udpcl : Form
    {
        public udpcl()
        {
            InitializeComponent();
            countdown.Elapsed += sendheartbeat;
            //ten.Elapsed += sensorsend;
        }

        System.Timers.Timer ten = new System.Timers.Timer { Interval = 10000, AutoReset = true };

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

        /// <summary>
        /// 区域搜索消息
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct search
        {
            public byte head1;
            public byte head2;
            public byte functionField;
            public byte data_length;
            public Int32 lng;
            public Int32 lat;
            public Int16 alt;
            public Int16 width;
            public Int16 length;
        }

        /// <summary>
        /// 指点飞行
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct guided
        {
            public byte head1;
            public byte head2;
            public byte functionField;
            public byte data_length;
            public Int32 lng;
            public Int32 lat;
            public Int16 alt;        
        }

        /// <summary>
        /// 设置区域避障
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct bizhang
        {
            public byte head1;
            public byte head2;
            public byte functionField;
            public byte data_length;
            public byte obstacle_num;
            public Int32 lng;
            public Int32 lat;
            public Int16 width;
            public Int16 length;
        }



        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct mavlink_data
        {
            public byte head1;
            public byte head2;
            public byte functionid;
            public byte length;
            public UInt16 flightid;
            public Int16 flytime;
            public byte status;
            public byte vehicle_status;
            public byte fly_status;
            public byte sendors_status;
            public Int32 lng;
            public Int32 lat;
            public Int16 alt;
            public Int16 flyspeed;
            public Int16 roll;
            public Int16 pitch;
            public Int16 yaw;
            public Int16 voltage;
            public byte task;
            public byte healthstate;
            public byte hangstate;
            public byte IRState;
            public byte LaserState;
            public byte EOState;
            public byte GuideState;
            public byte CommState;
            public byte GPSState;
            public byte BDState;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct waypoint
        {
            public byte head1;
            public byte head2;
            public byte functionid;
            public byte length;
            public UInt16 flightid;
            public Int16 route;
            public byte wp_num;
            public byte wp_id;
            public Int32 lng;
            public Int32 lat;
            public Int16 alt;
            public UInt16 angle;
            public Int16 speed;
        }


        private void sendheartbeat(object sender, ElapsedEventArgs e)
        {
            try
            {
                foreach(var port in MainV2.Comports)
                {
                    foreach(var mav in port.MAVlist)
                    {
                        mavlink_data data = new mavlink_data();
                        data.head1 = 0xAA;
                        data.head2 = 0xAF;
                        data.functionid = 0x61;
                        data.length = 0x26;
                        data.flightid = (UInt16)mav.sysid;
                        data.flytime = (short)mav.cs.timeInAir;
                        data.status = 0x00;
                        data.vehicle_status = 0x00;
                        data.fly_status = 0x00;
                        data.sendors_status = 0x00;
                        data.lng = (int)(mav.cs.lng * 1e7);
                        data.lat = (int)(mav.cs.lat * 1e7);
                        if (mav.cs.alt < 0)
                        {
                            data.alt = (short)0;
                        }
                        else
                        {
                            data.alt = (short)(mav.cs.alt * 100);
                        }
                        data.flyspeed = (short)(mav.cs.groundspeed * 100);
                        data.roll = (short)(mav.cs.roll * 100);
                        data.pitch = (short)(mav.cs.pitch * 100);
                        data.yaw = (short)(mav.cs.yaw * 100);
                        data.voltage = (short)mav.cs.battery_voltage;
                        data.task = 0x03;
                        data.healthstate = 0x02;
                        data.hangstate = 0x02;
                        data.IRState = 0x02;
                        data.LaserState= 0x02;
                        data.EOState = 0x02;
                        data.GuideState = 0x02;
                        data.CommState = 0x02;
                        data.GPSState = 0x02;
                        data.BDState = 0x02;
                        byte[] send = StructToBytes(data);
                        udpClient.Send(send, send.Length, endPoint);
                    }
                }
               
            }
            catch
            {

            }
        }

        System.Timers.Timer countdown = new System.Timers.Timer { Interval = 1000, AutoReset = true };

        IPEndPoint endPoint;

        Thread receive;

        private UdpClient udpClient;

        /// <summary>
        /// 开始转发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void myButton1_Click(object sender, EventArgs e)
        {
            if (myButton1.Text == "连接")
            {
                endPoint = new IPEndPoint(IPAddress.Parse(textBox1.Text), int.Parse(textBox2.Text));
                udpClient = new UdpClient(60000);

                receive = new Thread(receivemessage);
                receive.Start();
                countdown.Start();
                //ten.Start();
                myButton1.Text = "断开连接";
            }
            else
            {
                countdown.Stop();
                //ten.Stop();
                udpClient.Close();
                myButton1.Text = "连接";
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

        private void receivemessage()
        {
            try
            {
                while (true)
                {
                    IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    byte[] data = new byte[4096];
                    data = udpClient.Receive(ref remoteEndPoint);//此方法把数据来源ip、port放到第二个参数中

                    if (data[0] == 0xAA && data[1] == 0xAA)
                    {
                        byte id = data[2];
                        if (id == 0x13)
                        {
                            search packet = ByteArrayToStructure<search>(data);
                            MessageBox.Show("中心经度:" + (packet.lng/1e6).ToString() + "\n" + "中心纬度:" + (packet.lat/1e6).ToString() + "\n" /*+ "高度:" + (packet.alt / 100).ToString() + "\n" */+ "宽度:" + (packet.width/100).ToString() + "\n" + "长度:" + (packet.length/100).ToString(), "区域搜索消息");
                        }
                        if (id == 0x17)
                        {
                            MessageBox.Show("起飞指令!");
                        }
                        if (id == 0x14)
                        {
                            guided packet = ByteArrayToStructure<guided>(data);
                            MessageBox.Show("经度:" + (packet.lng/1e6).ToString() + "\n" + "纬度:" + (packet.lat/1e6).ToString() /*+ "\n" + "高度:" + (packet.alt/100).ToString()*/, "指点飞行");
                        }
                        if (id == 0x11)
                        {
                            guided packet = ByteArrayToStructure<guided>(data);
                            MessageBox.Show("接收任务侦察指令"/*"经度:" + (packet.lng/1e6).ToString() + "\n" + "纬度:" + (packet.lat/1e6).ToString() + "\n" + "高度:" + (packet.alt / 100).ToString()*/, "侦察指令");
                        }
                        if (id == 0x15)
                        {
                            guided packet = ByteArrayToStructure<guided>(data);
                            MessageBox.Show("经度:" + (packet.lng/1e6).ToString() + "\n" + "纬度:" + (packet.lat/1e6).ToString() /*+ "\n" + "高度:" + (packet.alt / 100).ToString()*/, "设置移动避障");
                        }
                        if (id == 0x16)
                        {
                            bizhang packet = ByteArrayToStructure<bizhang>(data);
                            MessageBox.Show("障碍物点数:" + packet.obstacle_num.ToString() + "\n" + "中心经度:" + (packet.lng/1e6).ToString() + "\n" + "中心纬度:" + (packet.lat/1e6).ToString() +/* "\n" + "高度:" + (packet.alt/100).ToString() +*/ "\n" + "宽度:" + (packet.width/100).ToString() + "\n"+"长度:" + (packet.length/100).ToString(), "设置区域避障");

                            //GCSViews.FlightPlanner.instance.addmarkertar("", packet.lng * 1e-6, packet.lat * 1e-6, 0, packet.width / 100);
                        }
                    }
                }
            }
            catch
            {

            }
        }
     

        private void sensorsend()
        {
            try
            {
                if (udpClient != null)
                {
                    for (int i = 0; i < GCSViews.FlightPlanner.instance.pointlist.Count; i++)
                    {                      
                        waypoint data = new waypoint();
                        data.head1 = 0xAA;
                        data.head2 = 0xAF;
                        data.functionid = 0x60;
                        data.length = 0x14;
                        data.flightid = (ushort)MainV2.comPort.MAV.sysid;
                        data.route = Int16.Parse(textBox3.Text);
                        data.wp_num = (byte)GCSViews.FlightPlanner.instance.pointlist.Count;
                        data.wp_id = (byte)(i + 1);
                        data.lng = ((int)(GCSViews.FlightPlanner.instance.pointlist[i].Lng * 1e7));
                        data.lat = (int)(GCSViews.FlightPlanner.instance.pointlist[i].Lat * 1e7);
                        if(i == 0)
                        {
                            data.alt = (short)0;
                        }
                        else
                        {
                            data.alt = (short)((GCSViews.FlightPlanner.instance.pointlist[i].Alt * 100) - float.Parse(GCSViews.FlightPlanner.instance.TXT_homealt.Text));
                        }
                        
                        data.angle = 0;

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
                       
                        byte[] send = StructToBytes(data);
                        udpClient.Send(send, send.Length, endPoint);
                    }
                }
                //    foreach (var port in MainV2.Comports)
                //    {
                //        foreach (var mav in port.MAVlist)
                //        {
                //            if (mav.sysid == 1)
                //            {
                //                count1++;
                //                waypoint data = new waypoint();
                //                data.head1 = 0xAA;
                //                data.head2 = 0xAF;
                //                data.functionid = 0x60;
                //                data.length = 0x12;
                //                data.flightid = mav.sysid;
                //                data.wp_num = 0x01;
                //                data.wp_id = count1;
                //                data.lng = (int)(mav.cs.lng * 1e7);
                //                data.lat = (int)(mav.cs.lat * 1e7);
                //                if (mav.cs.alt < 0)
                //                {
                //                    data.alt = (short)0;
                //                }
                //                else
                //                {
                //                    data.alt = (short)(mav.cs.alt * 100);
                //                }
                //                data.angle = 0;
                //                data.speed = (short)(mav.cs.groundspeed * 100);                            
                //                byte[] send = StructToBytes(data);
                //                udpClient.Send(send, send.Length, endPoint);
                //            }
                //            if (mav.sysid == 7)
                //            {
                //                count2++;
                //                waypoint data = new waypoint();
                //                data.head1 = 0xAA;
                //                data.head2 = 0xAF;
                //                data.functionid = 0x60;
                //                data.length = 0x12;
                //                data.flightid = mav.sysid;
                //                data.wp_num = 0x01;
                //                data.wp_id = count2;
                //                data.lng = (int)(mav.cs.lng * 1e7);
                //                data.lat = (int)(mav.cs.lat * 1e7);
                //                if (mav.cs.alt < 0)
                //                {
                //                    data.alt = (short)0;
                //                }
                //                else
                //                {
                //                    data.alt = (short)(mav.cs.alt * 100);
                //                }
                //                data.angle = 0;
                //                data.speed = (short)(mav.cs.groundspeed * 100);
                //                byte[] send = StructToBytes(data);
                //                udpClient.Send(send, send.Length, endPoint);
                //            }
                //        }
                //    }
            }
            catch
            {

            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct target
        {
            public byte head1;
            public byte head2;
            public byte functionid;
            public byte length;
            public Int32 targetkind;
            public byte targettask;
            public byte amount;
            public Int32 lng;
            public Int32 lat;
            public Int32 alt;
            public Int16 speed;
            public Int32 course;
        }

        private void myButton2_Click(object sender, EventArgs e)
        {
            if (udpClient != null)
            {
                try
                {
                    foreach (var port in MainV2.Comports)
                    {
                        foreach (var mav in port.MAVlist)
                        {
                            if (mav.sysid == 1)
                            {
                                target data = new target();
                                data.head1 = 0xAA;
                                data.head2 = 0xAF;
                                data.functionid = 0x62;
                                data.length = 0x18;
                                data.targetkind = BitConverter.ToInt32(new byte[] { 0x16, 0x00, 0x00, 0x00 }, 0);
                                data.targettask = 0x03;
                                data.amount = 0x01;
                                data.lng = (int)(mav.cs.lng * 1e7);
                                data.lat = (int)(mav.cs.lat * 1e7);
                                if (mav.cs.alt < 0)
                                {
                                    data.alt = (short)0;
                                }
                                else
                                {
                                    data.alt = (short)(mav.cs.alt * 100);
                                }
                                data.speed = (short)(mav.cs.groundspeed * 100);
                                data.course = (int)(mav.cs.yaw*100);
                                byte[] send = StructToBytes(data);
                                udpClient.Send(send, send.Length, endPoint);
                            }
                        }
                    }

                }
                catch
                {

                }
            }
            else
            {
                MessageBox.Show("请先连接!");
            }
        }

        /// <summary>
        /// 2号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void myButton3_Click(object sender, EventArgs e)
        {
            if (udpClient != null)
            {
                try
                {
                    foreach (var port in MainV2.Comports)
                    {
                        foreach (var mav in port.MAVlist)
                        {
                            if (mav.sysid == 7)
                            {
                                target data = new target();
                                data.head1 = 0xAA;
                                data.head2 = 0xAF;
                                data.functionid = 0x62;
                                data.length = 0x18;
                                data.targetkind = BitConverter.ToInt32(new byte[] { 0x16, 0x00, 0x00, 0x00 }, 0);
                                data.targettask = 0x03;
                                data.amount = 0x01;
                                data.lng = (int)(mav.cs.lng * 1e7);
                                data.lat = (int)(mav.cs.lat * 1e7);
                                if (mav.cs.alt < 0)
                                {
                                    data.alt = (short)0;
                                }
                                else
                                {
                                    data.alt = (short)(mav.cs.alt * 100);
                                }
                                data.speed = (short)(mav.cs.groundspeed * 100);
                                data.course = (int)(mav.cs.yaw*100);
                                byte[] send = StructToBytes(data);
                                udpClient.Send(send, send.Length, endPoint);
                            }
                        }
                    }

                }
                catch
                {

                }
            }
            else
            {
                MessageBox.Show("请先连接!");
            }
        }

        private void udpcl_Load(object sender, EventArgs e)
        {

        }

        private void myButton4_Click(object sender, EventArgs e)
        {
            sensorsend();
        }
    }
}
