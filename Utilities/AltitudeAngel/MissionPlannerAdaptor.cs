using System;
using System.Collections.Generic;
using System.Linq;
using AltitudeAngelWings.Extra;
using AltitudeAngelWings.Models;
using DotSpatial.Positioning;
using DotSpatial.Topology;

namespace MissionPlanner.Utilities.AltitudeAngel
{
    internal class MissionPlannerAdaptor : IMissionPlanner
    {
        private readonly Func<IList<Locationwp>> _getFlightPlan;
        public IMap FlightPlanningMap { get; }
        public IMap FlightDataMap { get; }

        public MissionPlannerAdaptor(IMap flightDataMap, IMap flightPlanningMap, Func<IList<Locationwp>> getFlightPlan)
        {
            FlightDataMap = flightDataMap;
            FlightPlanningMap = flightPlanningMap;
            _getFlightPlan = getFlightPlan;
        }

        public void SaveSetting(string key, string data)
        {
            Settings.Instance[key] = data;
        }

        public string LoadSetting(string key)
        {
            return Settings.Instance.ContainsKey(key) ? Settings.Instance[key] : null;
        }

        public void ClearSetting(string key)
        {
            Settings.Instance.Remove(key);
        }

        public FlightPlan GetFlightPlan()
        {
            var waypoints = _getFlightPlan().Where(IsValidWaypoint).ToList();
            if (waypoints.Count == 0)
            {
                return null;
            }
            var envelope = GeometryFactory.Default.CreateMultiPoint(waypoints
                .Select(l => new Coordinate(l.lng, l.lat))).Envelope;
            var center = envelope.Center();
            var minPos = new Position(new Latitude(envelope.Minimum.Y), new Longitude(envelope.Minimum.X));
            var maxPos = new Position(new Latitude(envelope.Maximum.Y), new Longitude(envelope.Maximum.X));
            var boundingRadius = (int)Math.Ceiling(minPos.DistanceTo(maxPos).ToMeters().Value);
            return new FlightPlan(waypoints.Select(f => new FlightPlanWaypoint
            {
                Latitude = f.lat,
                Longitude = f.lng,
                Altitude = (int) f.alt,
            }))
            {
                CenterLongitude = center.X,
                CenterLatitude = center.Y,
                BoundingRadius = boundingRadius
            };
        }

        private static bool IsValidWaypoint(Locationwp location)
        {
            // Command that can contain latitude and longitude
            var cmd = (MAVLink.MAV_CMD)location.id;
            if (cmd != MAVLink.MAV_CMD.WAYPOINT &&
                cmd != MAVLink.MAV_CMD.LOITER_UNLIM &&
                cmd != MAVLink.MAV_CMD.LOITER_TURNS &&
                cmd != MAVLink.MAV_CMD.LOITER_TIME &&
                cmd != MAVLink.MAV_CMD.LAND &&
                cmd != MAVLink.MAV_CMD.TAKEOFF &&
                cmd != MAVLink.MAV_CMD.FOLLOW &&
                cmd != MAVLink.MAV_CMD.LOITER_TO_ALT &&
                cmd != MAVLink.MAV_CMD.PATHPLANNING &&
                cmd != MAVLink.MAV_CMD.SPLINE_WAYPOINT &&
                cmd != MAVLink.MAV_CMD.VTOL_TAKEOFF &&
                cmd != MAVLink.MAV_CMD.VTOL_LAND &&
                cmd != MAVLink.MAV_CMD.PAYLOAD_PLACE &&
                cmd != MAVLink.MAV_CMD.DO_SET_HOME &&
                cmd != MAVLink.MAV_CMD.DO_LAND_START &&
                cmd != MAVLink.MAV_CMD.DO_REPOSITION &&
                cmd != MAVLink.MAV_CMD.DO_SET_ROI_LOCATION &&
                cmd != MAVLink.MAV_CMD.DO_MOUNT_CONTROL &&
                cmd != MAVLink.MAV_CMD.OVERRIDE_GOTO &&
                cmd != MAVLink.MAV_CMD.SET_GUIDED_SUBMODE_CIRCLE &&
                cmd != MAVLink.MAV_CMD.FENCE_RETURN_POINT &&
                cmd != MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_INCLUSION &&
                cmd != MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_EXCLUSION &&
                cmd != MAVLink.MAV_CMD.FENCE_CIRCLE_INCLUSION &&
                cmd != MAVLink.MAV_CMD.FENCE_CIRCLE_EXCLUSION &&
                cmd != MAVLink.MAV_CMD.RALLY_POINT &&
                cmd != MAVLink.MAV_CMD.PAYLOAD_PREPARE_DEPLOY &&
                cmd != MAVLink.MAV_CMD.WAYPOINT_USER_1 &&
                cmd != MAVLink.MAV_CMD.WAYPOINT_USER_2 &&
                cmd != MAVLink.MAV_CMD.WAYPOINT_USER_3 &&
                cmd != MAVLink.MAV_CMD.WAYPOINT_USER_4 &&
                cmd != MAVLink.MAV_CMD.WAYPOINT_USER_5 &&
                cmd != MAVLink.MAV_CMD.SPATIAL_USER_1 &&
                cmd != MAVLink.MAV_CMD.SPATIAL_USER_2 &&
                cmd != MAVLink.MAV_CMD.SPATIAL_USER_3 &&
                cmd != MAVLink.MAV_CMD.SPATIAL_USER_4 &&
                cmd != MAVLink.MAV_CMD.SPATIAL_USER_5)
            {
                return false;
            }

            // Exclude "empty" latitude and longitude
            return location.lat != 0 && location.lng != 0;
        }
    }
}