using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MissionPlanner
{
    public class MAVState : MAVLink
    {
        public MAVState()
        {
            this.packetspersecond = new double[0x100];
            this.packetspersecondbuild = new DateTime[0x100];
            this.lastvalidpacket = DateTime.MinValue;
            this.sysid = 0;
            this.compid = 0;
            this.param = new MAVLinkParamList();
            this.packets = new byte[0x100][];
            this.packetseencount = new int[0x100];
            this.aptype = 0;
            this.apname = 0;
            this.recvpacketcount = 0;
            this.VersionString = "";
            this.SoftwareVersions = "";
            this.SerialString = "";
            this.FrameString = "";

            this.packetslost = 0f;
            this.packetsnotlost = 0f;
            this.packetlosttimer = DateTime.MinValue;
            cs.parent = this;
        }

        public float packetslost = 0;
        public float packetsnotlost = 0;
        public DateTime packetlosttimer = DateTime.MinValue;
        public float synclost = 0;


        // all
        public string VersionString { get; set; }
        // px4+ only
        public string SoftwareVersions { get; set; }
        // px4+ only
        public string SerialString { get; set; }
        // AC frame type
        public string FrameString { get; set; }

        public string Guid { get; set; }

        /// <summary>
        /// the static global state of the currently connected MAV
        /// </summary>
        public CurrentState cs = new CurrentState();

        /// <summary>
        /// mavlink remote sysid
        /// </summary>
        public byte sysid { get; set; }

        /// <summary>
        /// mavlink remove compid
        /// </summary>
        public byte compid { get; set; }

        /// <summary>
        /// storage for whole paramater list
        /// </summary>
        public MAVLinkParamList param { get; set; }

        public Dictionary<string, MAV_PARAM_TYPE> param_types = new Dictionary<string, MAV_PARAM_TYPE>();

        /// <summary>
        /// storage of a previous packet recevied of a specific type
        /// </summary>
        public byte[][] packets { get; set; }

        public int[] packetseencount { get; set; }

        /// <summary>
        /// time seen of last mavlink packet
        /// </summary>
        public DateTime lastvalidpacket { get; set; }

        /// <summary>
        /// used to calc packets per second on any single message type - used for stream rate comparaison
        /// </summary>
        public double[] packetspersecond { get; set; }

        /// <summary>
        /// time last seen a packet of a type
        /// </summary>
        public DateTime[] packetspersecondbuild = new DateTime[256];

        /// <summary>
        /// mavlink ap type
        /// </summary>
        public MAV_TYPE aptype { get; set; }

        public MAV_AUTOPILOT apname { get; set; }

        public Common.ap_product Product_ID
        {
            get
            {
                if (param.ContainsKey("INS_PRODUCT_ID")) return (Common.ap_product) (float) param["INS_PRODUCT_ID"];
                return Common.ap_product.AP_PRODUCT_ID_NONE;
            }
        }

        /// <summary>
        /// used as a snapshot of what is loaded on the ap atm. - derived from the stream
        /// </summary>
        public Dictionary<int, mavlink_mission_item_t> wps = new Dictionary<int, mavlink_mission_item_t>();

        public Dictionary<int, mavlink_rally_point_t> rallypoints = new Dictionary<int, mavlink_rally_point_t>();

        public Dictionary<int, mavlink_fence_point_t> fencepoints = new Dictionary<int, mavlink_fence_point_t>();

        /// <summary>
        /// Store the guided mode wp location
        /// </summary>
        public mavlink_mission_item_t GuidedMode = new mavlink_mission_item_t();

        internal int recvpacketcount = 0;
    }
}