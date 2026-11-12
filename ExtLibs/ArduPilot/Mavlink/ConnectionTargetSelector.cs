using System;
using System.Collections.Generic;
using System.Linq;

namespace MissionPlanner.Mavlink
{
    // Owned by the connection worker, not by packet-reader callbacks. Selection
    // ends with the handshake; later discovery must not override a user's choice.
    internal sealed class ConnectionTargetSelector
    {
        private static readonly TimeSpan FallbackDelay = TimeSpan.FromSeconds(3);
        private readonly Dictionary<ulong, Candidate> candidates = new Dictionary<ulong, Candidate>();
        private TimeSpan? firstSeen;

        internal sealed class Candidate
        {
            public MAVLink.MAVLinkMessage Message;
            public int Count;
            public bool IsAutopilot;
            public int Order;
        }

        public bool HasCandidates => candidates.Count != 0;

        public void Observe(MAVLink.MAVLinkMessage message, TimeSpan receivedAt)
        {
            byte type;
            byte autopilot;
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

            if (message.sysid == 0 || message.compid == 0 || type == (byte)MAVLink.MAV_TYPE.GCS)
                return;

            if (!firstSeen.HasValue || receivedAt < firstSeen.Value)
                firstSeen = receivedAt;
            var id = MAVList.GetID(message.sysid, message.compid);
            Candidate candidate;
            if (!candidates.TryGetValue(id, out candidate))
            {
                candidate = new Candidate { Order = candidates.Count };
                candidates.Add(id, candidate);
            }
            candidate.Message = message;
            candidate.Count++;
            // MAVLink defines the autopilot field, not the component number, as
            // the way to identify a flight controller. System IDs are arbitrary.
            candidate.IsAutopilot = autopilot != (byte)MAVLink.MAV_AUTOPILOT.INVALID;
        }

        public Candidate Select(TimeSpan now, bool allowFallback = false)
        {
            var autopilot = candidates.Values.Where(c => c.IsAutopilot && c.Count >= 2)
                .OrderByDescending(c => c.Count).ThenBy(c => c.Order).FirstOrDefault();
            if (autopilot != null)
                return autopilot;

            if (!allowFallback && (!firstSeen.HasValue || now - firstSeen.Value < FallbackDelay))
                return null;

            // Keep companion/peripheral-only connections supported, including the
            // old confirmation threshold for devices using other component IDs.
            return candidates.Values.Where(c => c.Message.compid == 1 ? c.Count >= 2 : c.Count >= 4)
                .OrderByDescending(c => c.Count).ThenBy(c => c.Order).FirstOrDefault();
        }
    }
}
