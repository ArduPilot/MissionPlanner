using log4net;
using MissionPlanner.ArduPilot;
using MissionPlanner.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace MissionPlanner
{
    public class MAVState : MAVLink, IDisposable
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        [JsonIgnore]
        [IgnoreDataMember]
        public MAVLinkInterface parent;

        public MAVState(MAVLinkInterface mavLinkInterface, byte sysid, byte compid)
        {
            this.parent = mavLinkInterface;
            this.sysid = sysid;
            this.compid = compid;
            this.packetspersecond = new Dictionary<uint, double>();
            this.packetspersecondbuild = new Dictionary<uint, DateTime>();
            this.lastvalidpacket = DateTime.MinValue;
            sendlinkid = (byte)(new Random().Next(256));
            signing = false;
            this.param = new MAVLinkParamList();
            this.packets = new Dictionary<uint, Queue<MAVLinkMessage>>();
            this.aptype = 0;
            this.apname = 0;
            this.recvpacketcount = 0;
            this.VersionString = "";
            this.SoftwareVersions = "";
            this.SerialString = "";
            this.FrameString = "";
            if (sysid != 255 && !(compid == 0 && sysid == 0)) // && !parent.logreadmode)
                this.Proximity = new Proximity(this);

            camerapoints.Clear();

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

        private byte _sysid;
        /// <summary>
        /// mavlink remote sysid
        /// </summary>
        public byte sysid
        {
            get { return _sysid; }
            set { _sysid = value; }
        }

        /// <summary>
        /// mavlink remove compid
        /// </summary>
        public byte compid { get; set; }

        public byte linkid { get; set; }

        public byte sendlinkid { get; internal set; }

        public UInt64 timestamp { get; set; }

        internal byte[] signingKey;

        /// <summary>
        /// are we signing outgoing packets, and checking incomming packet signatures
        /// </summary>
        public bool signing { get; set; }

        /// <summary>
        /// ignore the incomming signature
        /// </summary>
        public bool signingignore { get; set; }

        /// <summary>
        /// mavlink 2 enable
        /// </summary>
        public bool mavlinkv2 = false;

        /// <summary>
        /// storage for whole paramater list
        /// </summary>
        [JsonIgnore]
        [IgnoreDataMember]
        public MAVLinkParamList param { get; set; }
        [JsonIgnore]
        [IgnoreDataMember]
        public Dictionary<string, MAV_PARAM_TYPE> param_types = new Dictionary<string, MAV_PARAM_TYPE>();

        /// <summary>
        /// storage of a previous packet recevied of a specific type
        /// </summary>
        Dictionary<uint, Queue<MAVLinkMessage>> packets { get; set; }

        object packetslock = new object();

        public MAVLinkMessage getPacket(uint mavlinkid)
        {
            //log.InfoFormat("getPacket {0}", (MAVLINK_MSG_ID)mavlinkid);
            lock (packetslock)
            {
                if (packets.ContainsKey(mavlinkid))
                {
                    if(packets[mavlinkid].Count > 0)
                        return packets[mavlinkid].Dequeue();
                }
            }

            return null;
        }

        public void addPacket(MAVLinkMessage msg)
        {
            lock (packetslock)
            {
                // create queue if it does not exist
                if (!packets.ContainsKey(msg.msgid))
                {
                    packets[msg.msgid] = new Queue<MAVLinkMessage>();
                }
                // cleanup queue if not polling this message
                while (packets[msg.msgid].Count > 5)
                {
                    packets[msg.msgid].Dequeue();
                }

                packets[msg.msgid].Enqueue(msg);
            }
        }

        public void clearPacket(uint mavlinkid)
        {
            lock (packetslock)
            {
                if (packets.ContainsKey(mavlinkid))
                {
                    packets[mavlinkid].Clear();;
                }
            }
        }

        public void Dispose()
        {
            if (Proximity != null)
                Proximity.Dispose();
        }

        /// <summary>
        /// time seen of last mavlink packet
        /// </summary>
        public DateTime lastvalidpacket { get; set; }

        /// <summary>
        /// used to calc packets per second on any single message type - used for stream rate comparaison
        /// </summary>
        [JsonIgnore]
        [IgnoreDataMember]
        public Dictionary<uint,double> packetspersecond { get; set; }

        /// <summary>
        /// time last seen a packet of a type
        /// </summary>
        [JsonIgnore]
        [IgnoreDataMember]
        public Dictionary<uint, DateTime> packetspersecondbuild { get; set; }

        /// <summary>
        /// mavlink ap type
        /// </summary>
        public MAV_TYPE aptype { get; set; }

        public MAV_AUTOPILOT apname { get; set; }

        public bool CANNode { get; set; } = false;

        public ap_product Product_ID
        {
            get
            {
                if (param.ContainsKey("INS_PRODUCT_ID")) return (ap_product) (float) param["INS_PRODUCT_ID"];
                return ap_product.AP_PRODUCT_ID_NONE;
            }
        }

        /// <summary>
        /// used as a snapshot of what is loaded on the ap atm. - derived from the stream
        /// </summary>
        public ConcurrentDictionary<int, mavlink_mission_item_t> wps = new ConcurrentDictionary<int, mavlink_mission_item_t>();

        public ConcurrentDictionary<int, mavlink_rally_point_t> rallypoints = new ConcurrentDictionary<int, mavlink_rally_point_t>();

        public ConcurrentDictionary<int, mavlink_fence_point_t> fencepoints = new ConcurrentDictionary<int, mavlink_fence_point_t>();

        public List<mavlink_camera_feedback_t> camerapoints = new List<mavlink_camera_feedback_t>();

        /// <summary>
        /// Store the guided mode wp location
        /// </summary>
        public mavlink_mission_item_t GuidedMode = new mavlink_mission_item_t();

        public Proximity Proximity;

        internal int recvpacketcount = 0;
        public Int64 time_offset_ns { get; set; }

        public override string ToString()
        {
            return sysid.ToString();
        }
    }
}