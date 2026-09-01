using System;
using System.Collections.Generic;
using MissionPlanner.Utilities;

namespace MissionPlanner.AIWaypointPlanner
{
    public sealed class MissionCompiler
    {
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
                throw new InvalidOperationException("Unsupported mission type.");

            mission.Items.Add(new CandidateMissionItem
            {
                Command = MAVLink.MAV_CMD.RETURN_TO_LAUNCH,
                Description = "任务完成后返航"
            });

            return mission;
        }

        private static CandidateMissionItem CreateTakeoff(double altitude)
        {
            return new CandidateMissionItem
            {
                Command = MAVLink.MAV_CMD.TAKEOFF,
                Altitude = altitude,
                Description = "固定翼任务起飞项（仅加入本地任务表）"
            };
        }

        private static CandidateMissionItem CreateSpeed(double speed)
        {
            return new CandidateMissionItem
            {
                Command = MAVLink.MAV_CMD.DO_CHANGE_SPEED,
                Param1 = 1.0,
                Param2 = speed,
                Description = "设置任务巡航速度"
            };
        }

        private static void AddSurveyWaypoints(CandidateMission mission, TaskSpec spec, MissionContext context)
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

                mission.Items.Add(CreateWaypoint(point, "区域巡视网格航点"));
                previous = point;
            }
        }

        private static void AddRelativeWaypoints(CandidateMission mission, TaskSpec spec, PointLatLngAlt home)
        {
            var current = new PointLatLngAlt(home);
            for (int i = 0; i < spec.legs.Count; i++)
            {
                RelativeLeg leg = spec.legs[i];
                current = current.newpos(leg.bearing_deg, leg.distance_m);
                current.Alt = leg.altitude_m;
                string description = string.IsNullOrWhiteSpace(leg.purpose)
                    ? "相对航段 " + (i + 1)
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
