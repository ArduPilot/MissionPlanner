using System;
using System.Collections.Generic;
using MissionPlanner.Utilities;

namespace MissionPlanner.AIWaypointPlanner
{
    public sealed class MissionCompiler
    {
        private string languageCode;

        public MissionCompiler()
            : this(UiStrings.DefaultLanguageCode)
        {
        }

        public MissionCompiler(string languageCode)
        {
            LanguageCode = languageCode;
        }

        public string LanguageCode
        {
            get { return languageCode; }
            set { languageCode = UiStrings.NormalizeLanguageCode(value); }
        }

        private string L(string key)
        {
            return UiStrings.Get(languageCode, key);
        }

        public CandidateMission Compile(TaskSpec spec, MissionContext context)
        {
            if (spec == null)
                throw new ArgumentNullException("spec");
            if (context == null)
                throw new ArgumentNullException("context");

            var mission = new CandidateMission { Summary = spec.summary ?? string.Empty };

            if (spec.include_takeoff)
                mission.Items.Add(CreateTakeoff(spec.takeoff_altitude_m));

            mission.Items.Add(CreateSpeed(spec.cruise_speed_mps));

            if (spec.mission_type == "survey_polygon")
                AddSurveyWaypoints(mission, spec, context);
            else if (spec.mission_type == "relative_route")
                AddRelativeWaypoints(mission, spec, context.Home);
            else
                throw new InvalidOperationException(UiStrings.Get(languageCode, "Validation.UnsupportedTemplate"));

            mission.Items.Add(new CandidateMissionItem
            {
                Command = MAVLink.MAV_CMD.RETURN_TO_LAUNCH,
                Description = L("Mission.DescriptionRtl")
            });

            return mission;
        }

        private CandidateMissionItem CreateTakeoff(double altitude)
        {
            return new CandidateMissionItem
            {
                Command = MAVLink.MAV_CMD.TAKEOFF,
                Altitude = altitude,
                Description = L("Mission.DescriptionTakeoff")
            };
        }

        private CandidateMissionItem CreateSpeed(double speed)
        {
            return new CandidateMissionItem
            {
                Command = MAVLink.MAV_CMD.DO_CHANGE_SPEED,
                Param1 = 1.0,
                Param2 = speed,
                Description = L("Mission.DescriptionSpeed")
            };
        }

        private void AddSurveyWaypoints(CandidateMission mission, TaskSpec spec, MissionContext context)
        {
            var polygon = new List<PointLatLngAlt>();
            foreach (PointLatLngAlt point in context.Polygon)
                polygon.Add(new PointLatLngAlt(point.Lat, point.Lng, spec.cruise_altitude_m));

            List<PointLatLngAlt> grid = Utilities.Grid.CreateGrid(
                polygon,
                spec.cruise_altitude_m,
                spec.lane_spacing_m,
                0.0,
                spec.grid_angle_deg,
                0.0,
                0.0,
                Utilities.Grid.StartPosition.Home,
                false,
                0.0f,
                0.0f,
                0.0f,
                context.Home);

            PointLatLngAlt previous = null;
            foreach (PointLatLngAlt point in grid)
            {
                if (point == null || string.Equals(point.Tag, "M", StringComparison.Ordinal))
                    continue;

                if (previous != null && NearlyEqual(previous.Lat, point.Lat) &&
                    NearlyEqual(previous.Lng, point.Lng) && NearlyEqual(previous.Alt, point.Alt))
                    continue;

                mission.Items.Add(CreateWaypoint(point, L("Mission.DescriptionSurveyWaypoint")));
                previous = point;
            }
        }

        private void AddRelativeWaypoints(CandidateMission mission, TaskSpec spec, PointLatLngAlt home)
        {
            var current = new PointLatLngAlt(home);
            for (int i = 0; i < spec.legs.Count; i++)
            {
                RelativeLeg leg = spec.legs[i];
                current = current.newpos(leg.bearing_deg, leg.distance_m);
                current.Alt = leg.altitude_m;
                string description = string.IsNullOrWhiteSpace(leg.purpose)
                    ? UiStrings.Format(languageCode, "Mission.DescriptionRelativeLegFormat", i + 1)
                    : leg.purpose.Trim();
                mission.Items.Add(CreateWaypoint(current, description));
            }
        }

        private static CandidateMissionItem CreateWaypoint(PointLatLngAlt point, string description)
        {
            return new CandidateMissionItem
            {
                Command = MAVLink.MAV_CMD.WAYPOINT,
                Longitude = point.Lng,
                Latitude = point.Lat,
                Altitude = point.Alt,
                Description = description
            };
        }

        private static bool NearlyEqual(double left, double right)
        {
            return Math.Abs(left - right) < 0.000000001;
        }
    }
}
