using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using log4net;

namespace MissionPlanner.ArduPilot
{
    public class AIS
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static List<(DateTime TS, MAVLink.mavlink_ais_vessel_t msg)> _Vessels = new List<(DateTime, MAVLink.mavlink_ais_vessel_t)>();

        public static int Age { get; set; } = 60 * 5;

        public static MAVLink.mavlink_ais_vessel_t[] Vessels
        {
            get { return _Vessels.Where(a=>a.TS.AddSeconds(Age) > DateTime.Now).Select(a=>a.msg).ToArray(); }
        }

        public static void Start(MAVLinkInterface mav)
        {
            mav.OnPacketReceived += (sender, message) =>
            {
                if (message.msgid == (uint) MAVLink.MAVLINK_MSG_ID.AIS_VESSEL)
                {
                    try
                    {
                        var msg = (MAVLink.mavlink_ais_vessel_t) message.data;

                        lock (_Vessels)
                        {
                            var existing = _Vessels.Where(a => a.msg.MMSI == msg.MMSI);
                            if (existing.Count() == 0)
                            {
                                _Vessels.Add((DateTime.Now, msg));
                            }
                            else
                            {
                                _Vessels.Remove(existing.First());
                                _Vessels.Add((DateTime.Now, msg));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }
                }
            };
        }
    }
}