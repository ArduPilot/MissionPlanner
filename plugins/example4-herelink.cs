using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MissionPlanner.Utilities;
using MissionPlanner.Controls;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using MissionPlanner;
using System.Drawing;
using GMap.NET.WindowsForms;
using MissionPlanner.GCSViews;
using MissionPlanner.Maps;


namespace CameraControl
{
    public class Plugin : MissionPlanner.Plugin.Plugin
    {
        private KeyValuePair<MAVLink.MAVLINK_MSG_ID, Func<MAVLink.MAVLinkMessage, bool>>? sub;
        private KeyValuePair<MAVLink.MAVLINK_MSG_ID, Func<MAVLink.MAVLinkMessage, bool>>? sub1;
        private KeyValuePair<MAVLink.MAVLINK_MSG_ID, Func<MAVLink.MAVLinkMessage, bool>>? sub2;
        private KeyValuePair<MAVLink.MAVLINK_MSG_ID, Func<MAVLink.MAVLinkMessage, bool>>? sub3;
        private KeyValuePair<MAVLink.MAVLINK_MSG_ID, Func<MAVLink.MAVLinkMessage, bool>>? sub4;

        public override string Name
        {
            get { return "Camera Control"; }
        }

        public override string Version
        {
            get { return "0.10"; }
        }

        public override string Author
        {
            get { return "Michael Oborne"; }
        }

        public override bool Init()
        {
            return true;
        }

        public override bool Loaded()
        {
            var rootbut = new ToolStripMenuItem("Herelink Video");
            //rootbut.Click += but_Click;
            ToolStripItemCollection col = Host.FDMenuHud.Items;
            col.Add(rootbut);

            var but = new ToolStripMenuItem("Connect v1");
            but.Click += but3_Click;
            rootbut.DropDownItems.Add(but);


            but = new ToolStripMenuItem("Set Video stream 1 v1");
            but.Click += but1_Click;
            rootbut.DropDownItems.Add(but);

            but = new ToolStripMenuItem("Set Video stream 2 v1");
            but.Click += but2_Click;
            rootbut.DropDownItems.Add(but);

            but = new ToolStripMenuItem("Connect stream 1 v2");
            but.Click += but7_Click;
            rootbut.DropDownItems.Add(but);

            but = new ToolStripMenuItem("Connect stream 2 v2");
            but.Click += but8_Click;
            rootbut.DropDownItems.Add(but);


            but = new ToolStripMenuItem("Connect air 1 v2");
            but.Click += but5_Click;
            rootbut.DropDownItems.Add(but);

            but = new ToolStripMenuItem("Connect air 2 v2");
            but.Click += but6_Click;
            rootbut.DropDownItems.Add(but);

            but = new ToolStripMenuItem("Reset Baud");
            but.Click += but9_Click;
            rootbut.DropDownItems.Add(but);

            return true;
        }



        public override bool Loop()
        {
            return true;
        }

        public override bool Exit()
        {
            return true;
        }

        void but_Click(object sender, EventArgs e)
        {
            var mav = Host.comPort.MAVlist.FirstOrDefault(a =>
                a.compid == (byte)MAVLink.MAV_COMPONENT.MAV_COMP_ID_CAMERA);

            if (mav == null)
                return;

            if (sub == null)
                sub = mav.parent.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.CAMERA_INFORMATION,
                    message =>
                    {
                        Console.WriteLine(message.ToJSON());
                        return true;
                    });

            if (sub1 == null)
                sub1 = mav.parent.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.CAMERA_SETTINGS,
                    message =>
                    {
                        Console.WriteLine(message.ToJSON());
                        return true;
                    });

            if (sub2 == null)
                sub2 = mav.parent.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.VIDEO_STREAM_INFORMATION,
                    message =>
                    {
                        Console.WriteLine(message.ToJSON());
                        return true;
                    });

            if (sub3 == null)
                sub3 = mav.parent.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.CAMERA_IMAGE_CAPTURED,
                    message =>
                    {
                        Console.WriteLine(message.ToJSON());
                        return true;
                    });

            if (sub4 == null)
                sub4 = mav.parent.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.CAMERA_CAPTURE_STATUS,
                    message =>
                    {
                        Console.WriteLine(message.ToJSON());
                        return true;
                    });

            mav.parent.doCommand(mav.sysid, mav.compid, MAVLink.MAV_CMD.REQUEST_CAMERA_INFORMATION, 0, 0, 0, 0, 0, 0, 0);
            mav.parent.doCommand(mav.sysid, mav.compid, MAVLink.MAV_CMD.REQUEST_VIDEO_STREAM_INFORMATION, 0, 0, 0, 0, 0, 0, 0);
            mav.parent.doCommand(mav.sysid, mav.compid, MAVLink.MAV_CMD.REQUEST_CAMERA_SETTINGS, 0, 0, 0, 0, 0, 0, 0);
            mav.parent.doCommand(mav.sysid, mav.compid, MAVLink.MAV_CMD.SET_CAMERA_MODE, 0, 1, 0, 0, 0, 0, 0);  // p2 = 1 for recording hint
            mav.parent.doCommand(mav.sysid, mav.compid, MAVLink.MAV_CMD.REQUEST_STORAGE_INFORMATION, 0, 0, 0, 0, 0, 0, 0);

            mav.parent.doCommand(mav.sysid, mav.compid, MAVLink.MAV_CMD.IMAGE_START_CAPTURE, 0, 0, 0, 0, 0, 0, 0);
            mav.parent.doCommand(mav.sysid, mav.compid, MAVLink.MAV_CMD.REQUEST_CAMERA_CAPTURE_STATUS, 0, 0, 0, 0, 0, 0,
                0, false);

        }

        private void but2_Click(object sender, EventArgs e)
        {
            var mav = Host.comPort.MAVlist.FirstOrDefault(a =>
                a.compid == (byte)MAVLink.MAV_COMPONENT.MAV_COMP_ID_CAMERA);

            if (mav == null)
                return;

            const int MAV_CMD_VIDEO_STOP_STREAMING = 2503;

            mav.parent.doCommand(mav.sysid, mav.compid, (MAVLink.MAV_CMD)MAV_CMD_VIDEO_STOP_STREAMING, 0, 0, 0, 0, 0, 0, 0, true);

            mav.parent.doCommand(mav.sysid, mav.compid, MAVLink.MAV_CMD.VIDEO_START_STREAMING, 1, 0, 0, 0, 0, 0, 0, true);
        }

        private void but1_Click(object sender, EventArgs e)
        {
            var mav = Host.comPort.MAVlist.FirstOrDefault(a =>
                a.compid == (byte)MAVLink.MAV_COMPONENT.MAV_COMP_ID_CAMERA);

            if (mav == null)
                return;

            const int MAV_CMD_VIDEO_STOP_STREAMING = 2503;

            mav.parent.doCommand(mav.sysid, mav.compid, (MAVLink.MAV_CMD)MAV_CMD_VIDEO_STOP_STREAMING, 0, 0, 0, 0, 0, 0, 0, true);

            mav.parent.doCommand(mav.sysid, mav.compid, MAVLink.MAV_CMD.VIDEO_START_STREAMING, 0, 0, 0, 0, 0, 0, 0, true);
        }

        private void but3_Click(object sender, EventArgs e)
        {
            GStreamer.StopAll();

            string ipaddr = "192.168.43.1";

            if (Settings.Instance["herelinkip"] != null)
                ipaddr = Settings.Instance["herelinkip"].ToString();

            InputBox.Show("herelink ip", "Enter herelink ip address", ref ipaddr);

            Settings.Instance["herelinkip"] = ipaddr;

            string url = String.Format(
                "rtspsrc location=rtsp://{0}:8554/fpv_stream latency=1 udp-reconnect=1 timeout=0 do-retransmission=false ! application/x-rtp ! decodebin3 ! queue max-size-buffers=1 leaky=2 ! videoconvert ! video/x-raw,format=BGRx ! appsink name=outsink",
                ipaddr);

            GStreamer.gstlaunch = GStreamer.LookForGstreamer();

            if (!GStreamer.gstlaunchexists)
            {
                GStreamerUI.DownloadGStreamer();

                if (!GStreamer.gstlaunchexists)
                {
                    return;
                }
            }

            GStreamer.StartA(url);
        }

        private void but4_Click(object sender, EventArgs e)
        {
            GStreamer.StopAll();

            string ipaddr = "192.168.43.1";

            if (Settings.Instance["herelinkip"] != null)
                ipaddr = Settings.Instance["herelinkip"].ToString();

            InputBox.Show("herelink ip", "Enter herelink ip address", ref ipaddr);

            Settings.Instance["herelinkip"] = ipaddr;

            string url = String.Format(
                "rtspsrc location=rtsp://{0}:8554/fpv_stream latency=1 udp-reconnect=1 timeout=0 do-retransmission=false ! application/x-rtp ! decodebin3 ! queue max-size-buffers=1 leaky=2 ! videoconvert ! video/x-raw,format=BGRx ! appsink name=outsink",
                ipaddr);

            GStreamer.gstlaunch = GStreamer.LookForGstreamer();

            if (!GStreamer.gstlaunchexists)
            {
                GStreamerUI.DownloadGStreamer();

                if (!GStreamer.gstlaunchexists)
                {
                    return;
                }
            }

            GStreamer.StartA(url);
        }

        private void but5_Click(object sender, EventArgs e)
        {
            GStreamer.StopAll();

            string ipaddr = "192.168.43.1";

            if (Settings.Instance["herelinkip"] != null)
                ipaddr = Settings.Instance["herelinkip"].ToString();

            InputBox.Show("herelink ip", "Enter herelink ip address", ref ipaddr);

            Settings.Instance["herelinkip"] = ipaddr;

            string url = String.Format(
                "rtspsrc location=rtsp://{0}:8554/H264Video latency=1 udp-reconnect=1 timeout=0 do-retransmission=false ! application/x-rtp ! decodebin3 ! queue max-size-buffers=1 leaky=2 ! videoconvert ! video/x-raw,format=BGRx ! appsink name=outsink",
                ipaddr);

            GStreamer.gstlaunch = GStreamer.LookForGstreamer();

            if (!GStreamer.gstlaunchexists)
            {
                GStreamerUI.DownloadGStreamer();

                if (!GStreamer.gstlaunchexists)
                {
                    return;
                }
            }

            GStreamer.StartA(url);
        }
        private void but6_Click(object sender, EventArgs e)
        {
            GStreamer.StopAll();

            string ipaddr = "192.168.43.1";

            if (Settings.Instance["herelinkip"] != null)
                ipaddr = Settings.Instance["herelinkip"].ToString();

            InputBox.Show("herelink ip", "Enter herelink ip address", ref ipaddr);

            Settings.Instance["herelinkip"] = ipaddr;

            string url = String.Format(
                "rtspsrc location=rtsp://{0}:8554/H264Video1 latency=1 udp-reconnect=1 timeout=0 do-retransmission=false ! application/x-rtp ! decodebin3 ! queue max-size-buffers=1 leaky=2 ! videoconvert ! video/x-raw,format=BGRx ! appsink name=outsink",
                ipaddr);

            GStreamer.gstlaunch = GStreamer.LookForGstreamer();

            if (!GStreamer.gstlaunchexists)
            {
                GStreamerUI.DownloadGStreamer();

                if (!GStreamer.gstlaunchexists)
                {
                    return;
                }
            }

            GStreamer.StartA(url);
        }
        private void but7_Click(object sender, EventArgs e)
        {
            GStreamer.StopAll();

            string ipaddr = "192.168.43.1";

            if (Settings.Instance["herelinkip"] != null)
                ipaddr = Settings.Instance["herelinkip"].ToString();

            InputBox.Show("herelink ip", "Enter herelink ip address", ref ipaddr);

            Settings.Instance["herelinkip"] = ipaddr;

            string url = String.Format(
                "rtspsrc location=rtsp://{0}:8554/fpv_stream latency=1 udp-reconnect=1 timeout=0 do-retransmission=false ! application/x-rtp ! decodebin3 ! queue max-size-buffers=1 leaky=2 ! videoconvert ! video/x-raw,format=BGRx ! appsink name=outsink",
                ipaddr);

            GStreamer.gstlaunch = GStreamer.LookForGstreamer();

            if (!GStreamer.gstlaunchexists)
            {
                GStreamerUI.DownloadGStreamer();

                if (!GStreamer.gstlaunchexists)
                {
                    return;
                }
            }

            GStreamer.StartA(url);
        }
        private void but8_Click(object sender, EventArgs e)
        {
            GStreamer.StopAll();

            string ipaddr = "192.168.43.1";

            if (Settings.Instance["herelinkip"] != null)
                ipaddr = Settings.Instance["herelinkip"].ToString();

            InputBox.Show("herelink ip", "Enter herelink ip address", ref ipaddr);

            Settings.Instance["herelinkip"] = ipaddr;

            string url = String.Format(
                "rtspsrc location=rtsp://{0}:8554/fpv_stream1 latency=1 udp-reconnect=1 timeout=0 do-retransmission=false ! application/x-rtp ! decodebin3 ! queue max-size-buffers=1 leaky=2 ! videoconvert ! video/x-raw,format=BGRx ! appsink name=outsink",
                ipaddr);

            GStreamer.gstlaunch = GStreamer.LookForGstreamer();

            if (!GStreamer.gstlaunchexists)
            {
                GStreamerUI.DownloadGStreamer();

                if (!GStreamer.gstlaunchexists)
                {
                    return;
                }
            }

            GStreamer.StartA(url);
        }
        private void but9_Click(object sender, EventArgs e)
        {
            //MAVLINK_MSG_ID_COMMAND_LONG
            //MAV_CMD_USER_1
            //p1 > 0
            //p2 > 0
            //p3=0

            var mav = Host.comPort.MAVlist.FirstOrDefault(a => a.compid == (byte)MAVLink.MAV_COMPONENT.MAV_COMP_ID_CAMERA);

            if (mav == null)
                return;

            mav.parent.doCommand(mav.sysid, mav.compid, MAVLink.MAV_CMD.USER_1, -1, 0, 0, 0, 0, 0, 0, false);

            mav.parent.doCommand(mav.sysid, mav.compid, MAVLink.MAV_CMD.USER_1, 1, 57600, 0, 0, 0, 0, 0);
        }
    }
}