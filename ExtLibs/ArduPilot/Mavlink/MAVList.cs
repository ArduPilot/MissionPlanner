using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MissionPlanner.Mavlink
{
    public class MAVList : IEnumerable<MAVState>, IDisposable
    {
        private Dictionary<ulong, MAVState> masterlist = new Dictionary<ulong, MAVState>();

        private Dictionary<ulong, MAVState> hiddenlist = new Dictionary<ulong, MAVState>();

        public MAVLinkInterface parent;

        object locker = new object();
        private long nextDiscoveryOrder;
        private readonly Stopwatch discoveryClock = Stopwatch.StartNew();
        private TimeSpan? firstCandidateSeen;

        internal TimeSpan DiscoveryTime => discoveryClock.Elapsed;

        public MAVList(MAVLinkInterface mavLinkInterface)
        {
            parent = mavLinkInterface;
            // add blank item
            hiddenlist.Add(0, new MAVState(parent, 0, 0));
        }

        public void AddHiddenList(uint sysid, byte compid)
        {
            ulong id = GetID(sysid, (byte)compid);
            lock (locker)
                hiddenlist[id] = new MAVState(parent, sysid, compid);
        }

        public MAVState this[uint sysid, int compid]
        {
            get
            {
                ulong id = GetID(sysid, (byte)compid);

                lock (locker)
                {
                    MAVState state;

                    // 3dr radio special case
                    if (hiddenlist.TryGetValue(id, out state))
                        return state;

                    if (!masterlist.TryGetValue(id, out state))
                    {
                        AddHiddenList(sysid, (byte) compid);
                        return hiddenlist[id];
                    }

                    return state;
                }
            }
            set
            {
                ulong id = GetID(sysid, (byte)compid);
                lock (locker)
                {
                    value.DiscoveryOrder = masterlist.TryGetValue(id, out var existing)
                        ? existing.DiscoveryOrder : nextDiscoveryOrder++;
                    masterlist[id] = value;
                }
            }
        }

        public int Count
        {
            get { lock (locker) return masterlist.Count; }
        }

        public List<ulong> GetRawIDS()
        {
            lock (locker)
                return masterlist.Keys.ToList();
        }

        // CAN nodes and other devices discovered without a heartbeat stay
        // available in the GUI, after the ranked heartbeat candidates.
        public MAVState[] GetRankedStates()
        {
            lock (locker)
                return masterlist.Values
                    .Where(m => m.sysid != 0 && m.compid != 0 && m.aptype != MAVLink.MAV_TYPE.GCS)
                    .OrderBy(m => m.ConnectionPriority).ThenBy(m => m.compid)
                    .ThenBy(m => m.DiscoveryOrder).ToArray();
        }

        internal void ObserveHeartbeat(MAVLink.MAVLinkMessage message, TimeSpan? receivedAt = null)
        {
            byte type, autopilot;
            if (message.msgid == (uint)MAVLink.MAVLINK_MSG_ID.HEARTBEAT)
            {
                var heartbeat = message.ToStructure<MAVLink.mavlink_heartbeat_t>();
                type = heartbeat.type;
                autopilot = heartbeat.autopilot;
            }
            else if (message.msgid == (uint)MAVLink.MAVLINK_MSG_ID.HIGH_LATENCY2)
            {
                var heartbeat = message.ToStructure<MAVLink.mavlink_high_latency2_t>();
                type = heartbeat.type;
                autopilot = heartbeat.autopilot;
            }
            else return;

            lock (locker)
            {
                Create(message.sysid, message.compid);
                var state = this[message.sysid, message.compid];
                state.aptype = (MAVLink.MAV_TYPE)type;
                state.apname = (MAVLink.MAV_AUTOPILOT)autopilot;
                state.ConnectionHeartbeat = message;
                state.ConnectionPriority = ConnectionTargetSelector.GetPriority(state.apname);
                if (!firstCandidateSeen.HasValue && message.sysid != 0 && message.compid != 0 &&
                    type != (byte)MAVLink.MAV_TYPE.GCS)
                    firstCandidateSeen = receivedAt ?? discoveryClock.Elapsed;
            }
        }

        internal MAVLink.MAVLinkMessage SelectConnectionTarget(TimeSpan now, bool deadlineReached = false)
        {
            lock (locker)
            {
                var best = GetRankedStates().FirstOrDefault(m => m.ConnectionHeartbeat != null);
                return ConnectionTargetSelector.Select(best, firstCandidateSeen, now, deadlineReached);
            }
        }

        public void Clear()
        {
            lock (locker)
            {
                masterlist.Clear();
                // Hidden entries can be reused on reconnect; discard their old
                // discovery metadata as well.
                foreach (var state in hiddenlist.Values)
                {
                    state.ConnectionHeartbeat = null;
                    state.ConnectionPriority = ConnectionTargetSelector.Priority.WithoutHeartbeat;
                }
                nextDiscoveryOrder = 0;
                firstCandidateSeen = null;
                discoveryClock.Restart();
            }
        }

        public bool Contains(uint sysid, byte compid, bool includehidden = true)
        {
            lock (locker)
            {
                foreach (var item in masterlist.Values)
                {
                    if (item.sysid == sysid && item.compid == compid)
                        return true;
                }

                if (includehidden)
                {
                    foreach (var item in hiddenlist.Values)
                    {
                        if (item.sysid == sysid && item.compid == compid)
                            return true;
                    }
                }
            }

            return false;
        }

        internal void Create(uint sysid, byte compid)
        {
            ulong id = GetID(sysid, (byte)compid);

            lock (locker) 
            { 
                if (masterlist.ContainsKey(id))
                    return;

                // move from hidden to visible
                if (hiddenlist.ContainsKey(id))
                {
                    masterlist[id] = hiddenlist[id];
                    hiddenlist.Remove(id);
                }

                if (!masterlist.ContainsKey(id))
                    masterlist[id] = new MAVState(parent, sysid, compid);

                masterlist[id].DiscoveryOrder = nextDiscoveryOrder++;
            }
        }

        public IEnumerator<MAVState> GetEnumerator()
        {
            MAVState[] snapshot;
            lock (locker)
                snapshot = masterlist.Values.ToArray();
            foreach (var key in snapshot)
            {
                yield return key;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public static ulong GetID(uint sysid, byte compid)
        {
           return  ((ulong)sysid << 8) | compid;
        }

        public static (uint sysid, byte compid) FromID(ulong id)
        {
            return ((uint)(id / 256), (byte)(id & 0xff));
        }

        public void Dispose()
        {
            lock (locker) 
            { 
                foreach (var MAV in hiddenlist)
                {
                    MAV.Value.Dispose();
                }

                foreach (var MAV in masterlist)
                {
                    MAV.Value.Dispose();
                }
            }
        }
    }
}
