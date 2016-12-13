﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using GMap.NET;
using log4net;
using MissionPlanner.Utilities;
using System.Collections.Concurrent;

namespace MissionPlanner
{
    public class MAVState : MAVLink, IDisposable
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public MAVLinkInterface parent;

        public MAVState(MAVLinkInterface mavLinkInterface)
        {
            this.parent = mavLinkInterface;
            this.packetspersecond = new double[0x100];
            this.packetspersecondbuild = new DateTime[0x100];
            this.lastvalidpacket = DateTime.MinValue;
            this.sysid = 0;
            this.compid = 0;
            sendlinkid = (byte)(new Random().Next(256));
            signing = false;
            this.param = new MAVLinkParamList();
            this.packets = new Dictionary<uint, MAVLinkMessage>();
            this.aptype = 0;
            this.apname = 0;
            this.recvpacketcount = 0;
            this.VersionString = "";
            this.SoftwareVersions = "";
            this.SerialString = "";
            this.FrameString = "";
            this.Proximity = new Proximity(this);

            camerapoints.Clear();

            GMapMarkerOverlapCount.Clear();

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
        public MAVLinkParamList param { get; set; }

        public Dictionary<string, MAV_PARAM_TYPE> param_types = new Dictionary<string, MAV_PARAM_TYPE>();

        /// <summary>
        /// storage of a previous packet recevied of a specific type
        /// </summary>
        Dictionary<uint, MAVLinkMessage> packets { get; set; }

        object packetslock = new object();

        public MAVLinkMessage getPacket(uint mavlinkid)
        {
            //log.InfoFormat("getPacket {0}", (MAVLINK_MSG_ID)mavlinkid);
            lock (packetslock)
            {
                if (packets.ContainsKey(mavlinkid))
                {
                    return packets[mavlinkid];
                }
            }

            return null;
        }

        public void addPacket(MAVLinkMessage msg)
        {
            lock (packetslock)
            {
                packets[msg.msgid] = msg;
            }
        }

        public void clearPacket(uint mavlinkid)
        {
            lock (packetslock)
            {
                if (packets.ContainsKey(mavlinkid))
                {
                    packets[mavlinkid] = null;
                }
            }
        }

        public void Dispose()
        {
             Proximity.Dispose();
        }

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
        public ConcurrentDictionary<int, mavlink_mission_item_t> wps = new ConcurrentDictionary<int, mavlink_mission_item_t>();

        public ConcurrentDictionary<int, mavlink_rally_point_t> rallypoints = new ConcurrentDictionary<int, mavlink_rally_point_t>();

        public ConcurrentDictionary<int, mavlink_fence_point_t> fencepoints = new ConcurrentDictionary<int, mavlink_fence_point_t>();

        public List<mavlink_camera_feedback_t> camerapoints = new List<mavlink_camera_feedback_t>();

        public GMapMarkerOverlapCount GMapMarkerOverlapCount = new GMapMarkerOverlapCount(PointLatLng.Empty);

        /// <summary>
        /// Store the guided mode wp location
        /// </summary>
        public mavlink_mission_item_t GuidedMode = new mavlink_mission_item_t();

        public Proximity Proximity;

        internal int recvpacketcount = 0;
    }
}