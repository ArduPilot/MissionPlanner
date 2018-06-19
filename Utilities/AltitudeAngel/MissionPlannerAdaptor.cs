using System;
using System.Collections.Generic;
using System.Linq;
using AltitudeAngel.IsolatedPlugin.Common;
using AltitudeAngel.IsolatedPlugin.Common.Maps;
using AltitudeAngelWings.Models;
using DotSpatial.Topology;
using MissionPlanner.GCSViews;
using FlightData = MissionPlanner.GCSViews.FlightData;

namespace MissionPlanner.Utilities.AltitudeAngel
{
    public class MissionPlannerAdaptor : IMissionPlanner
    {
        private readonly Func<IList<Locationwp>> _getFlightPlan;
        public IMap FlightPlanningMap { get; set; }
        public IMap FlightDataMap { get; set; }

        public MissionPlannerAdaptor(Func<IList<Locationwp>> getFlightPlan)
        {
            _getFlightPlan = getFlightPlan;
            FlightDataMap = new MapAdapter(FlightData.instance.gMapControl1);
            FlightPlanningMap = new MapAdapter(FlightPlanner.instance.MainMap);
        }

        public void SaveSetting(string key, string data)
        {
            Settings.Instance[key] = data;
        }

        public string LoadSetting(string key)
        {
            if (Settings.Instance.ContainsKey(key))
                return Settings.Instance[key];

            return null;
        }

        public void ClearSetting(string key)
        {
            Settings.Instance.Remove(key);
        }

        public FlightPlan GetFlightPlan()
        {
            var waypoints = _getFlightPlan();
            var envelope = GeometryFactory.Default.CreateMultiPoint(waypoints
                .Where(IsValidWaypoint)
                .Select(l => new Coordinate(l.lng, l.lat))).Envelope;
            var center = envelope.Center();
            var boundingRadius = (int)Math.Ceiling(Math.Max(envelope.Width, envelope.Height));

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
            return location.id == (ushort)MAVLink.MAV_CMD.WAYPOINT;
        }
    }
}