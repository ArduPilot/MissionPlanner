using MissionPlanner.Controls;
using MissionPlanner.Utilities;
using Org.BouncyCastle.Crypto.Tls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using MissionPlanner.GCSViews;
using System.Drawing;
using System.Runtime.Serialization;

namespace MissionPlanner.plugins
{
    public class example20_multiplepositions : Plugin.Plugin
    {
        public override string Name { get; } = "multiple positions";

        public override string Version { get; }

        public override string Author { get; }

        public override bool Exit()
        {
            return true;
        }

        public bool showahrs2pos;
        public bool showahrs3pos;
        public bool showahrspos;
        public bool showgps2pos;
        public bool showgpspos;
        public bool showsimpos;
        private GMapOverlay overlay;

        public override bool Init()
        {
            var rootbut = new ToolStripMenuItem("Setup other positions");
            rootbut.Click += but_Click;
            ToolStripItemCollection col = Host.FDMenuMap.Items;
            col.Add(rootbut);

            overlay = new GMapOverlay("positions");
            FlightData.instance.gMapControl1.Overlays.Add(overlay);

            return true;
        }

        private void but_Click(object sender2, EventArgs e)
        {
            loopratehz = 1;
            // this needs to be set per "comport" - prevent any duplicates
            MainV2.comPort.OnPacketReceived -= OnComPortOnOnPacketReceived;
            MainV2.comPort.OnPacketReceived += OnComPortOnOnPacketReceived;
        }

        private void OnComPortOnOnPacketReceived(object sender, MAVLink.MAVLinkMessage message)
        {
            var ID = message.sysid * 256 + message.compid;
            switch ((MAVLink.MAVLINK_MSG_ID)message.msgid)
            {
                case MAVLink.MAVLINK_MSG_ID.GPS_RAW_INT:
                {
                    var pos = (MAVLink.mavlink_gps_raw_int_t)message.data;
                    UpdateOrCreate(new PointLatLng(pos.lat / 1e7, pos.lon / 1e7), ID, "GPS1");
                    break;
                }
                case MAVLink.MAVLINK_MSG_ID.AHRS2:
                {
                    var pos = (MAVLink.mavlink_ahrs2_t)message.data;
                    UpdateOrCreate(new PointLatLng(pos.lat / 1e7, pos.lng / 1e7), ID, "AHRS2");
                    break;
                }
                case MAVLink.MAVLINK_MSG_ID.AHRS3:
                {
                    var pos = (MAVLink.mavlink_ahrs3_t)message.data;
                    UpdateOrCreate(new PointLatLng(pos.lat / 1e7, pos.lng / 1e7), ID, "AHRS3");
                    break;
                }
                case MAVLink.MAVLINK_MSG_ID.HIGH_LATENCY:
                {
                    var pos = (MAVLink.mavlink_high_latency_t)message.data;
                    UpdateOrCreate(new PointLatLng(pos.latitude / 1e7, pos.longitude / 1e7), ID, "HighLatency");
                    break;
                }
                case MAVLink.MAVLINK_MSG_ID.HIGH_LATENCY2:
                {
                    var pos = (MAVLink.mavlink_high_latency2_t)message.data;
                    UpdateOrCreate(new PointLatLng(pos.latitude / 1e7, pos.longitude / 1e7), ID, "HighLatency2");
                    break;
                }
                case MAVLink.MAVLINK_MSG_ID.SIMSTATE:
                {
                    var pos = (MAVLink.mavlink_simstate_t)message.data;
                    UpdateOrCreate(new PointLatLng(pos.lat / 1e7, pos.lng / 1e7), ID, "SimState");
                    break;
                }
                case MAVLink.MAVLINK_MSG_ID.SIM_STATE:
                {
                    var pos = (MAVLink.mavlink_sim_state_t)message.data;
                    UpdateOrCreate(new PointLatLng(pos.lat, pos.lon), ID, "Sim State");
                    break;
                }
                case MAVLink.MAVLINK_MSG_ID.GLOBAL_POSITION_INT:
                {
                    var pos = (MAVLink.mavlink_global_position_int_t)message.data;
                    UpdateOrCreate(new PointLatLng(pos.lat / 1e7, pos.lon / 1e7), ID, "Global Position");
                    break;
                }
                case MAVLink.MAVLINK_MSG_ID.GPS2_RAW:
                {
                    var pos = (MAVLink.mavlink_gps2_raw_t)message.data;
                    UpdateOrCreate(new PointLatLng(pos.lat / 1e7, pos.lon / 1e7), ID, "GPS2");
                    break;
                }
            }
        }

        private void UpdateOrCreate(PointLatLng pointLatLng, int ID, string sourcetext = "")
        {
            var existing = overlay.Markers.Where(a => a.Tag.ToString() == ID.ToString()+sourcetext);
            if (existing.Count() > 0)
            {
                existing.First().Position = pointLatLng;
                ((GMarkerGoogle_edit)existing.First()).LastUpdate = DateTime.Now;
            }
            else
            {
                var marker = new GMarkerGoogle_edit(pointLatLng, GMarkerGoogleType.green_dot)
                    { Tag = ID.ToString() + sourcetext, ToolTipText = sourcetext, ToolTipMode = MarkerTooltipMode.OnMouseOver, LastUpdate = DateTime.Now };
                overlay.Markers.Add(marker);
            }
        }

        public override bool Loaded()
        {            
            return true;
        }

        public override bool Loop()
        {
            try
            {
                
            }
            catch (Exception e)
            {

            }

            return true;
        }
    }

    public class GMarkerGoogle_edit : GMarkerGoogle
    {
        public DateTime LastUpdate = DateTime.MinValue;

        public GMarkerGoogle_edit(PointLatLng p, GMarkerGoogleType type) : base(p, type)
        {
        }

        public GMarkerGoogle_edit(PointLatLng p, Bitmap Bitmap) : base(p, Bitmap)
        {
        }

        protected GMarkerGoogle_edit(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}