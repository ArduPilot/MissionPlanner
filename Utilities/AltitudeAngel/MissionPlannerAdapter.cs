using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AltitudeAngelWings;
using AltitudeAngelWings.Extra;
using AltitudeAngelWings.Models;
using AltitudeAngelWings.Service;
using DotSpatial.Positioning;
using DotSpatial.Topology;
using NodaTime;

namespace MissionPlanner.Utilities.AltitudeAngel
{
    public class MissionPlannerAdapter : IMissionPlanner
    {
        private readonly IUiThreadInvoke _uiThreadInvoke;
        private readonly Func<IList<Locationwp>> _getFlightPlan;
        private readonly ISettings _settings;
        public IMap FlightPlanningMap { get; }
        public IMap FlightDataMap { get; }

        public MissionPlannerAdapter(IUiThreadInvoke uiThreadInvoke, IMap flightDataMap, IMap flightPlanningMap, Func<IList<Locationwp>> getFlightPlan, ISettings settings)
        {
            FlightDataMap = flightDataMap;
            FlightPlanningMap = flightPlanningMap;
            _uiThreadInvoke = uiThreadInvoke;
            _getFlightPlan = getFlightPlan;
            _settings = settings;
        }

        public Task<FlightPlan> GetFlightPlan()
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
            return Task.FromResult(new FlightPlan(waypoints.Select(f => new FlightPlanWaypoint
            {
                Latitude = f.lat,
                Longitude = f.lng,
                Altitude = (int)f.alt,
            }))
            {
                CenterLongitude = center.X,
                CenterLatitude = center.Y,
                BoundingRadius = boundingRadius,
                FlightCapability = MavTypeToFlightCapability(MainV2.comPort.MAV.aptype),
                Summary =  _settings.FlightReportName,
                Description = _settings.FlightReportDescription,
                Duration = Duration.FromTimeSpan(_settings.FlightReportTimeSpan),
                UseLocalConflictScope = _settings.UseFlightPlanLocalScope,
                AllowSmsContact = false,
                SmsPhoneNumber = "",
                DroneSerialNumber = "",
                FlightOperationMode = FlightOperationMode.BVLOS
            });
        }

        public Task CommandDroneToReturnToBase()
            => SetMode("RTL");

        public Task CommandDroneToLand(
            float latitude,
            float longitude)
            => SetMode("Land");

        public Task CommandDroneToLoiter(
            float latitude,
            float longitude,
            float altitude)
            => SetMode("Loiter");

        public Task CommandDroneAllClear()
            => SetMode("Auto");

        private static Task SetMode(string mode)
        {
            MainV2.comPort.setMode(MainV2.comPort.MAV.sysid, MainV2.comPort.MAV.compid, mode);
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task NotifyConflict(string message)
            => ShowMessageBox(message, "Notification");

        /// <inheritdoc />
        public Task NotifyConflictResolved(string message)
            => ShowMessageBox(message, "Notification");

        /// <inheritdoc />
        public Task Disarm()
            => MainV2.comPort.doARMAsync(MainV2.comPort.MAV.sysid, MainV2.comPort.MAV.compid, false);

        public Task ShowMessageBox(string message, string caption = "Message")
            => _uiThreadInvoke.Invoke(() => CustomMessageBox.Show(message, caption));

        public Task<bool> ShowYesNoMessageBox(string message, string caption = "Message")
            => _uiThreadInvoke.Invoke(() => CustomMessageBox.Show(message, caption, MessageBoxButtons.YesNo) == (int)DialogResult.Yes);

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

        private static FlightCapability MavTypeToFlightCapability(MAVLink.MAV_TYPE mavType)
        {
            switch (mavType)
            {
                case MAVLink.MAV_TYPE.FIXED_WING:
                    return FlightCapability.FixedWing;

                // There are a lot more types but these will do for now
                case MAVLink.MAV_TYPE.QUADROTOR:
                case MAVLink.MAV_TYPE.COAXIAL:
                case MAVLink.MAV_TYPE.HEXAROTOR:
                case MAVLink.MAV_TYPE.OCTOROTOR:
                case MAVLink.MAV_TYPE.TRICOPTER:
                case MAVLink.MAV_TYPE.DODECAROTOR:
                    return FlightCapability.Rotary;

                default:
                    return FlightCapability.Unspecified;
            }
        }
    }
}