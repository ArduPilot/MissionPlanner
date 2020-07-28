using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MissionPlanner.Utilities;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using MissionPlanner;

namespace Shortcuts
{
    public class ButtonPlugin : MissionPlanner.Plugin.Plugin
    {
        public override string Name
        {
            get { return "Button change"; }
        }

        public override string Version
        {
            get { return "0.1"; }
        }

        public override string Author
        {
            get { return "EOSBandi"; }
        }

        public override bool Init()
        {
            return true;
        }

        public override bool Loaded()
        {
            Host.comPort.OnPacketReceived += MavOnOnPacketReceivedHandler;
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

        private void MavOnOnPacketReceivedHandler(object o, MAVLink.MAVLinkMessage linkMessage)
        {
            if ((MAVLink.MAVLINK_MSG_ID) linkMessage.msgid == MAVLink.MAVLINK_MSG_ID.BUTTON_CHANGE)
            {
                //Button change message received
                //Check and display message
                Console.WriteLine("BUTTON CHANGE message received");
            }
        }
    }
}