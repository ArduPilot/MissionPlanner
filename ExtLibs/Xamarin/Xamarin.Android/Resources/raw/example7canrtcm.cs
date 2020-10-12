using MissionPlanner;
using MissionPlanner.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MissionPlanner.Controls;
using UAVCAN;


namespace CANRTCMExtract
{
    public class Plugin : MissionPlanner.Plugin.Plugin
    {
        public override string Name
        {
            get { return "CAN RTCM Extract"; }
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
            var but = new ToolStripMenuItem("CAN RTCM Extract");
            but.Click += but_Click;
            ToolStripItemCollection col = Host.FDMenuMap.Items;
            
            // uncomment to enable
            //col.Add(but);

            return true;
        }

        private void but_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "gpsbase file|*.gpsbase";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.DefaultExt = "rtcm";
                sfd.Filter = "RTCM|*.rtcm";
                sfd.FileName = Path.GetFileNameWithoutExtension(ofd.FileName) + ".rtcm";
                sfd.InitialDirectory = Path.GetDirectoryName(ofd.FileName);

                if (sfd.ShowDialog() == DialogResult.OK)
                { 
                    UAVCAN.uavcan can = new uavcan();

                    using (var stream = sfd.OpenFile())
                    {
                        var data = File.ReadAllBytes(ofd.FileName);
                        
                        can.MessageReceived += (frame, msg, id) =>
                        {
                            if (frame.MsgTypeID == (ushort) uavcan.UAVCAN_EQUIPMENT_GNSS_RTCMSTREAM_DT_ID)
                            {
                                var rtcm = (uavcan.uavcan_equipment_gnss_RTCMStream) msg;

                                stream.Write(rtcm.data, 0, rtcm.data_len);
                            }
                        };

                        data.ForEach(b =>
                        {
                            try
                            {
                                can.Read((byte) b);
                            } catch {}
                        });
                    }
                }
            }
        }

        public override bool Loaded()
        {
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
    }
}