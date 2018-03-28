using log4net;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using MissionPlanner.ArduPilot;
using static MAVLink;

namespace MissionPlanner.Utilities
{
    public class Proximity : IDisposable
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        MAVState _parent;
        directionState _dS = new directionState();
        public directionState DirectionState => _dS;

        KeyValuePair<MAVLINK_MSG_ID, Func<MAVLinkMessage, bool>> sub;

        public bool DataAvailable { get; set; } = false;

        public Proximity(MAVState mavInt)
        {
            _parent = mavInt;
            sub = mavInt.parent.SubscribeToPacketType(MAVLINK_MSG_ID.DISTANCE_SENSOR, messageReceived);
            log.InfoFormat("created for {0} - {1}", mavInt.sysid, mavInt.compid);
        }

        ~Proximity()
        {
            _parent?.parent?.UnSubscribeToPacketType(sub);
        }

        private bool messageReceived(MAVLinkMessage arg)
        {
            //accept any compid, but filter sysid
            if (arg.sysid != _parent.sysid)
                return true;

            if (arg.msgid == (uint)MAVLINK_MSG_ID.DISTANCE_SENSOR)
            {
                var dist = arg.ToStructure<mavlink_distance_sensor_t>();

                if (dist.current_distance >= dist.max_distance)
                    return true;

                if (dist.current_distance <= dist.min_distance)
                    return true;

                _dS.Add(dist.id, (MAV_SENSOR_ORIENTATION)dist.orientation, dist.current_distance, DateTime.Now, 3);

                DataAvailable = true;
            }

            return true;
        }

        public void Dispose()
        {
            if (_parent != null)
                _parent.parent.UnSubscribeToPacketType(sub);
        }

        public class directionState
        {
            List<data> _dists = new List<data>();

            public class data
            {
                public uint SensorId;
                public MAV_SENSOR_ORIENTATION Orientation;
                public double Distance;
                public DateTime Received;
                public double Age;

                public DateTime ExpireTime { get { return Received.AddSeconds(Age); } }

                public data(uint id, MAV_SENSOR_ORIENTATION orientation, double distance, DateTime received, double age = 1)
                {
                    SensorId = id;
                    Orientation = orientation;
                    Distance = distance;
                    Received = received;
                    Age = age;
                }
            }

            public void Add(uint id, MAV_SENSOR_ORIENTATION orientation, double distance, DateTime received, double age = 1)
            {
                var existing = _dists.Where((a) => { return a.Orientation == orientation; });

                foreach (var item in existing.ToList())
                {
                    _dists.Remove(item);
                }

                _dists.Add(new data(id, orientation, distance, received, age));

                expire();
            }

            /// <summary>
            /// Closest distance
            /// </summary>
            /// <returns></returns>
            public double GetClosest()
            {
                expire();

                double min = double.MaxValue;

                for (int a = 0; a < _dists.Count; a++)
                {
                    min = Math.Min(min, _dists[a].Distance);
                }

                return min;
            }

            /// <summary>
            /// List of direction bellow the min_distance
            /// </summary>
            /// <param name="min_distance"></param>
            /// <returns>List of directions</returns>
            public List<MAV_SENSOR_ORIENTATION> GetWarnings(double min_distance = 2)
            {
                expire();

                List<MAV_SENSOR_ORIENTATION> list = new List<MAV_SENSOR_ORIENTATION>();

                for (int a = 0; a < _dists.Count; a++)
                {
                    if (_dists[a].Distance < min_distance)
                    {
                        list.Add(_dists[a].Orientation);
                    }
                }

                return list;
            }

            public List<data> GetRaw()
            {
                expire();

                return _dists;
            }

            void expire()
            {
                lock (this)
                {
                    for (int a = 0; a < _dists.Count; a++)
                    {
                        var expireat = _dists[a].ExpireTime;

                        if (expireat < DateTime.Now)
                        {
                            // remove it
                            _dists.RemoveAt(a);
                            // make sure we dont skip an element
                            a--;
                            // move on
                            continue;
                        }
                    }
                }
            }
        }
    }
}