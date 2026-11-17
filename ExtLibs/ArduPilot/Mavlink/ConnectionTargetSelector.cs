using System;

namespace MissionPlanner.Mavlink
{
    // Ranking is shared by the connection handshake and the GUI. Discovery lives
    // in MAVList; automatic selection ends with the handshake.
    internal static class ConnectionTargetSelector
    {
        private static readonly TimeSpan DiscoveryWindow = TimeSpan.FromSeconds(3);

        internal enum Priority
        {
            SpecificAutopilot = 1,
            GenericAutopilot = 2,
            Other = 3,
            WithoutHeartbeat = 4
        }

        internal static Priority GetPriority(MAVLink.MAV_AUTOPILOT autopilot)
        {
            switch (autopilot)
            {
                case MAVLink.MAV_AUTOPILOT.GENERIC:
                case MAVLink.MAV_AUTOPILOT.GENERIC_WAYPOINTS_ONLY:
                case MAVLink.MAV_AUTOPILOT.GENERIC_WAYPOINTS_AND_SIMPLE_NAVIGATION_ONLY:
                case MAVLink.MAV_AUTOPILOT.GENERIC_MISSION_FULL:
                    return Priority.GenericAutopilot;
                case MAVLink.MAV_AUTOPILOT.INVALID:
                case MAVLink.MAV_AUTOPILOT.RESERVED:
                    return Priority.Other;
                default:
                    // assume that all other defined entries are and will be specific autopilot modules
                    return Enum.IsDefined(typeof(MAVLink.MAV_AUTOPILOT), autopilot)
                        ? Priority.SpecificAutopilot : Priority.Other;
            }
        }

        internal static MAVLink.MAVLinkMessage Select(MAVState best, TimeSpan? firstCandidateSeen,
            TimeSpan now, bool deadlineReached)
        {
            if (best == null)
                return null;

            if (best.ConnectionPriority == Priority.SpecificAutopilot &&
                best.compid == (byte)MAVLink.MAV_COMPONENT.MAV_COMP_ID_AUTOPILOT1)
                return best.ConnectionHeartbeat;

            // A single accepted heartbeat is sufficient. All weaker targets use
            // the same window, starting at the first candidate, regardless of rate.
            return deadlineReached || firstCandidateSeen.HasValue && now - firstCandidateSeen.Value >= DiscoveryWindow
                ? best.ConnectionHeartbeat : null;
        }
    }
}
