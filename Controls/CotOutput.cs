using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using GeoidHeightsDotNet;
using MissionPlanner.Utilities.CoT;

namespace MissionPlanner.Controls
{
    /// <summary>One MAVLink component as the Cursor on Target output sees it, in metres, m/s and degrees.</summary>
    public class CotVehicle
    {
        /// <summary>The connection the component was heard on.</summary>
        public object Connection;
        public uint SysId;
        public byte CompId;
        public MAVLink.MAV_TYPE Type;
        public MAVLink.MAV_AUTOPILOT Autopilot;
        public double Lat;
        public double Lng;
        /// <summary>Height above mean sea level.</summary>
        public double AltMslM;
        public double GroundSpeedMs;
        public double CourseDeg;
        /// <summary>The GPS's own accuracy estimates; 0 when it does not report them.</summary>
        public double HorizontalAccuracyM;
        public double VerticalAccuracyM;
        public DateTime LastPacketUtc;
        /// <summary>The connection is playing back a telemetry log.</summary>
        public bool Playback;
        /// <summary>The firmware's version text, such as "ArduCopter V4.7.1 (4776a3fb)".</summary>
        public string Firmware;
    }

    /// <summary>What the user set for a vehicle in the grid.</summary>
    public class CotIdentity
    {
        public string Uid;
        /// <summary>The CoT type; null for the round's type.</summary>
        public string Type;
        public bool Takv;
        public string Callsign;
        public string Endpoint;
        public string Vmf;
    }

    public enum CotSkip
    {
        None,
        /// <summary>A gimbal, camera, companion computer or another ground station.</summary>
        NotAVehicle,
        NoPosition,
        Silent,
        Playback
    }

    public static class CotVehicles
    {
        /// <summary>
        /// Seconds without a packet after which a vehicle is no longer reported: the time after which
        /// Mission Planner shows a link quality of 0.
        /// </summary>
        public const double SilentSeconds = 10;

        public static CotVehicle Read(MAVState mav, MAVLinkInterface connection)
        {
            var cs = mav.cs;
            return new CotVehicle
            {
                Connection = connection,
                SysId = mav.sysid,
                CompId = mav.compid,
                Type = mav.aptype,
                Autopilot = mav.apname,
                Lat = cs.lat,
                Lng = cs.lng,
                // CurrentState gives altitude and speed in the units chosen on CONFIG > Planner
                AltMslM = cs.altasl / CurrentState.multiplieralt,
                GroundSpeedMs = cs.groundspeed / CurrentState.multiplierspeed,
                CourseDeg = cs.groundcourse,
                HorizontalAccuracyM = cs.gpsh_acc,
                VerticalAccuracyM = cs.gpsv_acc,
                LastPacketUtc = mav.lastvalidpacket,
                Playback = connection != null && connection.logreadmode,
                Firmware = mav.VersionString
            };
        }

        public static CotSkip Check(CotVehicle vehicle, DateTime utcNow)
        {
            // MAVLink components that are not flight controllers send MAV_AUTOPILOT_INVALID
            if (vehicle.Autopilot == MAVLink.MAV_AUTOPILOT.INVALID)
                return CotSkip.NotAVehicle;
            if (vehicle.Playback)
                return CotSkip.Playback;
            // CurrentState keeps 0, 0 until the first valid position
            if (vehicle.Lat == 0 && vehicle.Lng == 0)
                return CotSkip.NoPosition;
            if ((utcNow - vehicle.LastPacketUtc).TotalSeconds > SilentSeconds)
                return CotSkip.Silent;
            return CotSkip.None;
        }

        /// <summary>
        /// CoT heights are above the WGS 84 ellipsoid. TAK turns them back into heights above sea level
        /// with EGM96, so adding the EGM96 geoid height makes TAK show Mission Planner's altitude.
        /// </summary>
        public static double HeightAboveEllipsoid(CotVehicle vehicle)
        {
            return vehicle.AltMslM + GeoidHeights.undulation(vehicle.Lat, vehicle.Lng);
        }

        /// <summary>
        /// The CoT type for the kind of vehicle the heartbeat reports, as MIL-STD-2525 friendly tracks:
        /// fixed-wing and VTOL drones, rotary-wing drones, lighter than air, ground vehicles, boats,
        /// submarines and antenna trackers (ground sensors). ATAK draws each with its own symbol.
        /// </summary>
        public static string AutoType(MAVLink.MAV_TYPE type)
        {
            switch (type)
            {
                case MAVLink.MAV_TYPE.QUADROTOR:
                case MAVLink.MAV_TYPE.HEXAROTOR:
                case MAVLink.MAV_TYPE.OCTOROTOR:
                case MAVLink.MAV_TYPE.TRICOPTER:
                case MAVLink.MAV_TYPE.COAXIAL:
                case MAVLink.MAV_TYPE.HELICOPTER:
                case MAVLink.MAV_TYPE.DECAROTOR:
                case MAVLink.MAV_TYPE.DODECAROTOR:
                    return "a-f-A-M-H-Q";
                case MAVLink.MAV_TYPE.AIRSHIP:
                case MAVLink.MAV_TYPE.FREE_BALLOON:
                    return "a-f-A-M-L";
                case MAVLink.MAV_TYPE.GROUND_ROVER:
                    return "a-f-G-E-V";
                case MAVLink.MAV_TYPE.SURFACE_BOAT:
                    return "a-f-S";
                case MAVLink.MAV_TYPE.SUBMARINE:
                    return "a-f-U";
                case MAVLink.MAV_TYPE.ANTENNA_TRACKER:
                    return "a-f-G-E-S";
                default:
                    // fixed wing, the VTOL types and anything else: the output's type before Auto
                    return "a-f-A-M-F-Q";
            }
        }

        /// <summary>Two minutes, or three rounds when the interval is longer, so a track never goes stale between rounds.</summary>
        public static double StaleSeconds(double intervalSeconds)
        {
            return Math.Max(120, 3 * intervalSeconds);
        }
    }

    /// <summary>
    /// Builds the events of one round and remembers which tracks it reported. A track that is no
    /// longer reported (its link went quiet, the connection closed or started a log playback, its UID
    /// changed) gets one last event that is already stale, so TAK greys it out at once instead of
    /// showing it as current until its stale time.
    /// </summary>
    public class CotRound
    {
        /// <summary>CoT's value for an unknown circular or linear error.</summary>
        public const string UnknownError = "9999999.0";

        /// <summary>The takv platform and version: the software that sends the events.</summary>
        public const string Platform = "Mission Planner";

        public static readonly string Version = typeof(CotRound).Assembly.GetName().Version.ToString();

        /// <summary>
        /// System ids heard on more than one connection in the last round. Each connection after the
        /// first gets -2, -3 after the UID, so TAK keeps the vehicles apart instead of one jumping track.
        /// </summary>
        public List<uint> SharedSysIds { get; private set; } = new List<uint>();

        const string TimeFormat = "yyyy-MM-ddTHH:mm:ss.ffK";

        readonly Dictionary<(object, uint, byte), @event> reported = new Dictionary<(object, uint, byte), @event>();

        public List<@event> Build(IEnumerable<CotVehicle> vehicles, Func<CotVehicle, CotIdentity> identity, string type,
            bool details, DateTime utcNow, double staleSeconds)
        {
            var events = new List<@event>();
            var current = new HashSet<(object, uint, byte)>();

            var reportable = vehicles.Where(v => CotVehicles.Check(v, utcNow) == CotSkip.None).ToList();
            var connections = reportable.GroupBy(v => v.SysId)
                .ToDictionary(g => g.Key, g => g.Select(v => v.Connection).Distinct().ToList());
            SharedSysIds = connections.Where(c => c.Value.Count > 1).Select(c => c.Key).ToList();

            foreach (var vehicle in reportable)
            {
                var key = (vehicle.Connection, vehicle.SysId, vehicle.CompId);
                var id = identity(vehicle) ?? new CotIdentity { Uid = "NOsysid" + vehicle.SysId };
                var nth = connections[vehicle.SysId].IndexOf(vehicle.Connection);
                if (nth > 0)
                    id.Uid += "-" + (nth + 1);

                var cotevent = Event(vehicle, id, type, details, utcNow, staleSeconds);

                if (reported.TryGetValue(key, out var before) && before.uid != cotevent.uid)
                    events.Add(Expired(before, utcNow));

                reported[key] = cotevent;
                current.Add(key);
                events.Add(cotevent);
            }

            foreach (var key in reported.Keys.Where(key => !current.Contains(key)).ToList())
            {
                events.Add(Expired(reported[key], utcNow));
                reported.Remove(key);
            }

            return events;
        }

        /// <summary>One already-stale event for every track still reported, for when the output stops.</summary>
        public List<@event> ExpireAll(DateTime utcNow)
        {
            var events = reported.Values.Select(cotevent => Expired(cotevent, utcNow)).ToList();
            reported.Clear();
            return events;
        }

        static @event Event(CotVehicle vehicle, CotIdentity id, string type, bool details, DateTime utcNow,
            double staleSeconds)
        {
            var cotevent = new @event
            {
                uid = id?.Uid ?? "",
                type = string.IsNullOrEmpty(id?.Type) ? type ?? "" : id.Type,
                time = Time(utcNow),
                start = Time(utcNow.AddSeconds(-5)),
                stale = Time(utcNow.AddSeconds(staleSeconds)),
                // m-g: machine generated, from GPS
                how = "m-g",
                detail = new detail
                {
                    track = new track
                    {
                        course = Number(vehicle.CourseDeg, 2),
                        speed = Number(vehicle.GroundSpeedMs, 2)
                    }
                },
                point = new point
                {
                    lat = Number(vehicle.Lat, 7),
                    lon = Number(vehicle.Lng, 7),
                    hae = Number(CotVehicles.HeightAboveEllipsoid(vehicle), 2),
                    ce = Error(vehicle.HorizontalAccuracyM),
                    le = Error(vehicle.VerticalAccuracyM)
                }
            };

            if (details && id != null)
            {
                if (id.Takv)
                {
                    cotevent.detail.takv = new takv
                    {
                        platform = Platform,
                        version = Version,
                        device = string.IsNullOrEmpty(vehicle.Firmware) ? vehicle.Type.ToString() : vehicle.Firmware
                    };
                }

                if (!string.IsNullOrEmpty(id.Callsign) || !string.IsNullOrEmpty(id.Endpoint))
                {
                    cotevent.detail.contact = new contact
                    {
                        callsign = string.IsNullOrEmpty(id.Callsign) ? null : id.Callsign,
                        endpoint = string.IsNullOrEmpty(id.Endpoint) ? null : id.Endpoint
                    };
                }

                if (!string.IsNullOrEmpty(id.Vmf))
                    cotevent.detail.uid = new uid { vmf = id.Vmf };
            }

            return cotevent;
        }

        static @event Expired(@event cotevent, DateTime utcNow)
        {
            var now = Time(utcNow);
            return new @event
            {
                version = cotevent.version,
                uid = cotevent.uid,
                type = cotevent.type,
                time = now,
                start = now,
                stale = now,
                how = cotevent.how,
                detail = cotevent.detail,
                point = cotevent.point
            };
        }

        // the invariant culture keeps ':' as the time separator and the Gregorian calendar, whatever
        // the PC's regional settings
        public static string Time(DateTime utc)
        {
            return utc.ToString(TimeFormat, CultureInfo.InvariantCulture);
        }

        static string Number(double value, int decimals)
        {
            return value.ToString("F" + decimals, CultureInfo.InvariantCulture);
        }

        static string Error(double metres)
        {
            return metres > 0 ? Number(metres, 1) : UnknownError;
        }
    }
}
