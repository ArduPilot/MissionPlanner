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
using MissionPlanner.Comms;
using System.Net.Sockets;
using System.Net;

namespace CameraControl
{
    public class PluginCC : MissionPlanner.Plugin.Plugin
    {
        private KeyValuePair<MAVLink.MAVLINK_MSG_ID, Func<MAVLink.MAVLinkMessage, bool>>? sub;
        private KeyValuePair<MAVLink.MAVLINK_MSG_ID, Func<MAVLink.MAVLinkMessage, bool>>? sub1;
        private KeyValuePair<MAVLink.MAVLINK_MSG_ID, Func<MAVLink.MAVLinkMessage, bool>>? sub2; 
        private KeyValuePair<MAVLink.MAVLINK_MSG_ID, Func<MAVLink.MAVLinkMessage, bool>>? sub3;
        private KeyValuePair<MAVLink.MAVLINK_MSG_ID, Func<MAVLink.MAVLinkMessage, bool>>? sub4;

        public override string Name
        {
            get { return "HL Control"; }
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
            return false;
        }

        public override bool Loaded()
        {
            var rootbut = new ToolStripMenuItem("Herelink info");
            //rootbut.Click += but_Click;
            ToolStripItemCollection col = Host.FDMenuHud.Items;
            col.Add(rootbut);

            var but = new ToolStripMenuItem("HL Get Info");
            but.Click += but3_Click;
            rootbut.DropDownItems.Add(but);

             but = new ToolStripMenuItem("HL connect");
            but.Click += but4_Click;
            rootbut.DropDownItems.Add(but);


            return true;
        }

        private void but4_Click(object sender, EventArgs e)
        {
            var udc = new UdpSerialConnect();
            udc.Port = "14552";
            udc.client = new UdpClient("192.168.144.11", 14552);
            udc.IsOpen = true;
            udc.hostEndPoint = new IPEndPoint(IPAddress.Parse("192.168.144.11"), 14552);
            
            MainV2.Comports.Add(new MAVLinkInterface() { BaseStream = udc });

        }

        public override bool Loop()
        {
            return true;
        }

        public override bool Exit()
        {
            return true;
        }
        
        private void but3_Click(object sender, EventArgs e)
        {
            var mav = Host.comPort.MAVlist.First();

            mav.parent.doCommand(43, 250, MAVLink.MAV_CMD.START_RX_PAIR, 0, 0, 0, 0, 0, 0, 0, true);

            //mav.parent.doCommand(42, 250, MAVLink.MAV_CMD.START_RX_PAIR, 0, 0, 0, 0, 0, 0, 0, false);

            mav.parent.GetParam(43, 250, "HL_POW_MODE");
            mav.parent.GetParam(43, 250, "HL_POW_DB");
            mav.parent.GetParam(43, 250, "HL_FREQ_MODE");
            mav.parent.GetParam(43, 250, "HL_FREQ_MHZ");
            mav.parent.GetParam(43, 250, "HL_RADIO_UL_BW");
            mav.parent.GetParam(43, 250, "HL_RADIO_DL_BW");
            mav.parent.GetParam(43, 250, "HL_FW_RELEASE");
        }

    }
}