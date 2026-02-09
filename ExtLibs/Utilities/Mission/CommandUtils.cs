using System;
using System.Collections.Generic;
using System.Linq;

namespace MissionPlanner.Utilities.Mission
{
    /// <summary>
    /// Classification helpers for MAV_CMD command IDs and Locationwp items.
    /// </summary>
    public static class CommandUtils
    {
        public static bool IsLoiter(ushort cmd)
        {
            return cmd == (ushort)MAVLink.MAV_CMD.LOITER_UNLIM ||
                   cmd == (ushort)MAVLink.MAV_CMD.LOITER_TIME ||
                   cmd == (ushort)MAVLink.MAV_CMD.LOITER_TURNS ||
                   cmd == (ushort)MAVLink.MAV_CMD.LOITER_TO_ALT;
        }

        public static bool IsTakeoff(ushort cmd)
        {
            return cmd == (ushort)MAVLink.MAV_CMD.TAKEOFF ||
                   cmd == (ushort)MAVLink.MAV_CMD.TAKEOFF_LOCAL ||
                   cmd == (ushort)MAVLink.MAV_CMD.VTOL_TAKEOFF;
        }

        public static bool IsLand(ushort cmd)
        {
            return cmd == (ushort)MAVLink.MAV_CMD.LAND ||
                   cmd == (ushort)MAVLink.MAV_CMD.VTOL_LAND ||
                   cmd == (ushort)MAVLink.MAV_CMD.LAND_LOCAL;
        }

        /// <summary>
        /// Returns the loiter radius for a loiter command.
        /// Sign indicates direction: positive = clockwise, negative = counter-clockwise.
        /// Uses param3 for most loiters, param2 for LOITER_TO_ALT.
        /// Falls back to <paramref name="defaultRadius"/> when the parameter is zero.
        /// </summary>
        /// <exception cref="ArgumentException">Command is not a loiter.</exception>
        public static double LoiterRadius(Locationwp cmd, double defaultRadius)
        {
            switch (cmd.id)
            {
            case (ushort)MAVLink.MAV_CMD.LOITER_TURNS:
            case (ushort)MAVLink.MAV_CMD.LOITER_TIME:
            case (ushort)MAVLink.MAV_CMD.LOITER_UNLIM:
                return cmd.p3 != 0 ? cmd.p3 : defaultRadius;
            case (ushort)MAVLink.MAV_CMD.LOITER_TO_ALT:
                return cmd.p2 != 0 ? cmd.p2 : defaultRadius;
            default:
                throw new ArgumentException("Command is not a loiter command");
            }
        }

        /// <summary>
        /// Whether the loiter's cross-track line to the next waypoint is
        /// tangent to the loiter circle (param4 = 1) rather than passing
        /// through the center. Only meaningful for non-terminal loiters.
        /// </summary>
        public static bool IsLoiterXTrackTangent(Locationwp cmd)
        {
            return IsLoiter(cmd.id) && !IsTerminal(cmd.id) && cmd.p4 == 1;
        }

        /// <summary>
        /// Returns the radius for the map marker circle drawn around a
        /// waypoint. Loiters use their loiter radius; everything else uses the
        /// WP acceptance radius. Always returns a non-negative value.
        /// </summary>
        public static double MarkerRadius(Locationwp cmd, double defaultLoiterRadius, double defaultWPRadius)
        {
            if (IsLoiter(cmd.id))
            {
                return Math.Abs(LoiterRadius(cmd, defaultLoiterRadius));
            }
            else
            {
                return Math.Abs(defaultWPRadius);
            }
        }

        /// <summary>
        /// Whether the command has a meaningful lat/lon position. Returns false
        /// for takeoffs (ArduPilot ignores their lat/lon) and for commands
        /// without the [hasLocation] MAVLink attribute.
        /// </summary>
        public static bool HasLocation(Locationwp cmd)
        {
            if (!HasLatLon(cmd))
            {
                return false;
            }
            // ArduPilot ignores takeoff lat/lon (vehicle takes off from current
            // position). The MAVLink spec marks these [hasLocation] for the
            // altitude field, probably to indicate that the altitude is stored,
            // but the lat/lon are not meaningful.
            if (IsTakeoff(cmd.id))
            {
                return false;
            }
            var mavCmdType = typeof(MAVLink.MAV_CMD);
            if (!Enum.IsDefined(mavCmdType, cmd.id))
                return true; // unknown, assume positional if lat/lon present
            var name = ((MAVLink.MAV_CMD)cmd.id).ToString();
            var memberInfo = mavCmdType.GetMember(name).FirstOrDefault();
            if (memberInfo == null)
                return true; // should not be possible

            return memberInfo
                .GetCustomAttributes(false)
                .OfType<MAVLink.hasLocation>()
                .Any();
        }

        static bool HasLatLon(Locationwp cmd)
        {
            return !(cmd.lat == 0 && cmd.lng == 0) &&
                   !double.IsNaN(cmd.lat) &&
                   !double.IsNaN(cmd.lng);
        }

        /// <summary>
        /// Whether the command ends a flight path with no sequential fallthrough to
        /// the next command (RTL, LOITER_UNLIM, FLIGHT_TERMINATION, DO_RALLY_LAND).
        /// </summary>
        public static bool IsTerminal(ushort cmd)
        {
            return cmd == (ushort)MAVLink.MAV_CMD.DO_RALLY_LAND ||
                   cmd == (ushort)MAVLink.MAV_CMD.RETURN_TO_LAUNCH ||
                   cmd == (ushort)MAVLink.MAV_CMD.DO_FLIGHTTERMINATION ||
                   cmd == (ushort)MAVLink.MAV_CMD.LOITER_UNLIM;
        }

        /// <summary>
        /// Whether a command is a connected point in a route (like nav commands
        /// in missions, or polygon fence points). Terminal commands and
        /// takeoffs are nodes even though they may lack a lat/lon; their
        /// position is resolved elsewhere.
        /// </summary>
        public static bool IsNode(Locationwp cmd)
        {
            // Despite not being NAV commands, these fence polygon commands are connected nodes
            if (cmd.id == (ushort)MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_EXCLUSION ||
                cmd.id == (ushort)MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_INCLUSION)
            {
                return true;
            }

            // The obsolete "ROI" command is in the "NAV" range, but is not actually a NAV command
            if (cmd.id == (ushort)MAVLink.MAV_CMD.ROI)
            {
                return false;
            }

            // These commands terminate the flight, so even though some do not have a location, they get a node
            if (IsTerminal(cmd.id))
            {
                return true;
            }

            // Takeoffs don't have a lat/lon set, but they are still nodes
            // (their location will be home or the immediately preceding land)
            if (IsTakeoff(cmd.id))
            {
                return true;
            }

            // Otherwise, anything in the NAV command range that has a location is a connected node
            if (cmd.id < (ushort)MAVLink.MAV_CMD.LAST)
            {
                return HasLocation(cmd);
            }

            return false;
        }

        /// <summary>
        /// Whether the command is a label that marks a point in the mission
        /// sequence without being a navigated waypoint itself
        /// (JUMP_TAG, DO_LAND_START, DO_RETURN_PATH_START).
        /// </summary>
        public static bool IsBookmark(ushort cmd)
        {
            return cmd == (ushort)MAVLink.MAV_CMD.JUMP_TAG ||
                   cmd == (ushort)MAVLink.MAV_CMD.DO_LAND_START ||
                   cmd == (ushort)MAVLink.MAV_CMD.DO_RETURN_PATH_START;
        }

        public static bool IsRegionOfInterest(ushort cmd)
        {
            return cmd == (ushort)MAVLink.MAV_CMD.ROI ||
                   cmd == (ushort)MAVLink.MAV_CMD.DO_SET_ROI ||
                   cmd == (ushort)MAVLink.MAV_CMD.DO_SET_ROI_LOCATION;
        }

        public static bool IsFencePoint(ushort cmd)
        {
            return cmd == (ushort)MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_EXCLUSION ||
                   cmd == (ushort)MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_INCLUSION ||
                   cmd == (ushort)MAVLink.MAV_CMD.FENCE_CIRCLE_EXCLUSION ||
                   cmd == (ushort)MAVLink.MAV_CMD.FENCE_CIRCLE_INCLUSION;
        }

        /// <summary>
        /// Resolves the 0-based mission index that a jump command targets. For
        /// DO_JUMP, converts from the 1-based param1. For DO_JUMP_TAG, looks up
        /// the tag in <paramref name="jumpTags"/> (key = tag value, value =
        /// 0-based mission index). Returns false if the tag is not found.
        /// </summary>
        /// <exception cref="ArgumentException">Command is not a jump.</exception>
        public static bool TryGetJumpTarget(Locationwp cmd, Dictionary<int, int> jumpTags, out int jumpTarget)
        {
            switch (cmd.id)
            {
            case (ushort)MAVLink.MAV_CMD.DO_JUMP:
                jumpTarget = (int)cmd.p1 - 1;
                return true;
            case (ushort)MAVLink.MAV_CMD.DO_JUMP_TAG:
                return jumpTags.TryGetValue((int)cmd.p1, out jumpTarget);
            default:
                throw new ArgumentException("Command is not a jump");
            }
        }

        public static bool IsJumpCommand(ushort cmd)
        {
            return cmd == (ushort)MAVLink.MAV_CMD.DO_JUMP ||
                   cmd == (ushort)MAVLink.MAV_CMD.DO_JUMP_TAG;
        }

        /// <summary>
        /// Returns the repeat count for a jump command (param2).
        /// Positive values = finite repeats, negative = jump forever, 0 = don't jump.
        /// </summary>
        /// <exception cref="ArgumentException">Command is not a jump.</exception>
        public static int GetJumpCount(Locationwp cmd)
        {
            if (IsJumpCommand(cmd.id))
            {
                return (int)cmd.p2;
            }
            else
            {
                throw new ArgumentException("Command is not a jump");
            }
        }

        /// <summary>
        /// Whether the copter comes to a stop at this waypoint, affecting the
        /// spline tangent calculation. Waypoints with a delay (param1 > 0), loiter
        /// turns, takeoffs, and landings all cause a stop.
        /// </summary>
        public static bool IsSplineStoppedCopter(Locationwp cmd)
        {
            if (IsTakeoff(cmd.id) || IsLand(cmd.id))
            {
                return true;
            }
            switch (cmd.id)
            {
            case (ushort)MAVLink.MAV_CMD.WAYPOINT:
            case (ushort)MAVLink.MAV_CMD.SPLINE_WAYPOINT:
            case (ushort)MAVLink.MAV_CMD.LOITER_TIME:
                return cmd.p1 > 0;
            case (ushort)MAVLink.MAV_CMD.LOITER_TURNS:
            case (ushort)MAVLink.MAV_CMD.PAYLOAD_PLACE:
                return true;
            default:
                return false;
            }
        }

        /// <summary>
        /// Resolves the physical location of a takeoff node.
        /// If preceded by a land, uses the land's position (or backtracks to find one).
        /// If preceded by a non-land or nothing, returns home.
        /// Returns null if a takeoff follows a non-land node (undefined behavior).
        /// </summary>
        /// <exception cref="ArgumentException">Node is not a takeoff.</exception>
        public static PointLatLngAlt GetTakeoffLocation(MissionNode node, PointLatLngAlt home)
        {
            if (!IsTakeoff(node.Command.id))
            {
                throw new ArgumentException("Command is not a takeoff");
            }
            var prev_node = node.IncomingEdges?.FirstOrDefault()?.FromNode;
            if (prev_node == null)
            {
                // Nothing leads to here, so just use home
                return home;
            }
            if (!IsLand(prev_node.Command.id))
            {
                // Placing a takeoff right after a non-land command is undefined behavior (usually skipped).
                // Null is better than home in this case.
                return null;
            }
            // Last node was a land. Takeoff from there
            if (HasLocation(prev_node.Command))
            {
                return new PointLatLngAlt(prev_node.Command);
            }
            // If the land has no location, backtrack until we find one
            var visited = new HashSet<int>();
            while (prev_node != null && !visited.Contains(prev_node.MissionIndex) && !HasLocation(prev_node.Command))
            {
                visited.Add(prev_node.MissionIndex);
                prev_node = prev_node.IncomingEdges?.FirstOrDefault()?.FromNode;
            }
            if (prev_node == null || !HasLocation(prev_node.Command))
            {
                return home;
            }
            return new PointLatLngAlt(prev_node.Command);
        }
    }
}
