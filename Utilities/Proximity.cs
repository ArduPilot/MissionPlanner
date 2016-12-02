using log4net;
using MissionPlanner.HIL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using static MAVLink;

namespace MissionPlanner.Utilities
{
    public class Proximity : IDisposable
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        MAVState _parent;
        directionState _dS = new directionState();

        KeyValuePair<MAVLINK_MSG_ID, Func<MAVLinkMessage, bool>> sub;

        Form temp = new Form();

        public Proximity(MAVState mavInt)
        {
            _parent = mavInt;
            sub = mavInt.parent.SubscribeToPacketType(MAVLINK_MSG_ID.DISTANCE_SENSOR, messageReceived);
            log.InfoFormat("created for {0} - {1}", mavInt.sysid, mavInt.compid);

            temp.Paint += Temp_Paint;
        }

        private void Temp_Paint(object sender, PaintEventArgs e)
        {
            var rawdata = _dS.GetRaw();

            e.Graphics.Clear(temp.BackColor);

            var midx = this.temp.Width / 2.0f;
            var midy = this.temp.Height / 2.0f;

            e.Graphics.DrawArc(System.Drawing.Pens.Green, midx - 10, midy - 10, 20, 20, 0, 360);

            foreach (var temp in rawdata.ToList())
            {
                Vector3 location = new Vector3(0, temp.Distance / 5, 0);

                var halflength = location.length() / 2.0f;
                var doublelength = location.length() * 2.0f;
                var length = location.length();

                switch (temp.Orientation)
                {
                    case MAV_SENSOR_ORIENTATION.MAV_SENSOR_ROTATION_NONE:
                        location.rotate(Common.Rotation.ROTATION_NONE);
                        e.Graphics.DrawString(temp.Distance.ToString(), Control.DefaultFont, System.Drawing.Brushes.Green, midx - (float)location.x, midy - (float)location.y);
                        e.Graphics.DrawArc(System.Drawing.Pens.Red, (float)(midx - length), (float)(midy - length), (float)doublelength, (float)doublelength, -22.5f - 90f, 45f);
                        break;
                    case MAV_SENSOR_ORIENTATION.MAV_SENSOR_ROTATION_YAW_45:
                        location.rotate(Common.Rotation.ROTATION_YAW_45);
                        e.Graphics.DrawString(temp.Distance.ToString(), Control.DefaultFont, System.Drawing.Brushes.Green, midx - (float)location.x, midy - (float)location.y);
                        e.Graphics.DrawArc(System.Drawing.Pens.Red, (float)(midx - length), (float)(midy - length), (float)doublelength, (float)doublelength, 22.5f - 90f, 45f);
                        break;
                    case MAV_SENSOR_ORIENTATION.MAV_SENSOR_ROTATION_YAW_90:
                        location.rotate(Common.Rotation.ROTATION_YAW_90);
                        e.Graphics.DrawString(temp.Distance.ToString(), Control.DefaultFont, System.Drawing.Brushes.Green, midx - (float)location.x, midy - (float)location.y);
                        e.Graphics.DrawArc(System.Drawing.Pens.Red, (float)(midx - length), (float)(midy - length), (float)doublelength, (float)doublelength, 67.5f - 90f, 45f);
                        break;
                    case MAV_SENSOR_ORIENTATION.MAV_SENSOR_ROTATION_YAW_135:
                        location.rotate(Common.Rotation.ROTATION_YAW_135);
                        e.Graphics.DrawString(temp.Distance.ToString(), Control.DefaultFont, System.Drawing.Brushes.Green, midx - (float)location.x, midy - (float)location.y);
                        e.Graphics.DrawArc(System.Drawing.Pens.Red, (float)(midx - length), (float)(midy - length), (float)doublelength, (float)doublelength, 112.5f - 90f, 45f);
                        break;
                    case MAV_SENSOR_ORIENTATION.MAV_SENSOR_ROTATION_YAW_180:
                        location.rotate(Common.Rotation.ROTATION_YAW_180);
                        e.Graphics.DrawString(temp.Distance.ToString(), Control.DefaultFont, System.Drawing.Brushes.Green, midx - (float)location.x, midy - (float)location.y);
                        e.Graphics.DrawArc(System.Drawing.Pens.Red, (float)(midx - length), (float)(midy - length), (float)doublelength, (float)doublelength, 157.5f - 90f, 45f);
                        break;
                    case MAV_SENSOR_ORIENTATION.MAV_SENSOR_ROTATION_YAW_225:
                        location.rotate(Common.Rotation.ROTATION_YAW_225);
                        e.Graphics.DrawString(temp.Distance.ToString(), Control.DefaultFont, System.Drawing.Brushes.Green, midx - (float)location.x, midy - (float)location.y);
                        e.Graphics.DrawArc(System.Drawing.Pens.Red, (float)(midx - length), (float)(midy - length), (float)doublelength, (float)doublelength, 202.5f - 90f, 45f);
                        break;
                    case MAV_SENSOR_ORIENTATION.MAV_SENSOR_ROTATION_YAW_270:
                        location.rotate(Common.Rotation.ROTATION_YAW_270);
                        e.Graphics.DrawString(temp.Distance.ToString(), Control.DefaultFont, System.Drawing.Brushes.Green, midx - (float)location.x, midy - (float)location.y);
                        e.Graphics.DrawArc(System.Drawing.Pens.Red, (float)(midx - length), (float)(midy - length), (float)doublelength, (float)doublelength, 247.5f - 90f, 45f);
                        break;
                    case MAV_SENSOR_ORIENTATION.MAV_SENSOR_ROTATION_YAW_315:
                        location.rotate(Common.Rotation.ROTATION_YAW_315);
                        e.Graphics.DrawString(temp.Distance.ToString(), Control.DefaultFont, System.Drawing.Brushes.Green, midx - (float)location.x, midy - (float)location.y);
                        e.Graphics.DrawArc(System.Drawing.Pens.Red, (float)(midx - length), (float)(midy - length), (float)doublelength, (float)doublelength, 292.5f - 90f, 45f);
                        break;
                }
            }
        }

        ~Proximity()
        {
            _parent.parent.UnSubscribeToPacketType(sub);
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

                _dS.Add((MAV_SENSOR_ORIENTATION)dist.orientation, dist.current_distance, DateTime.Now, 3);

                //Console.WriteLine("DIST " + (MAV_SENSOR_ORIENTATION)dist.orientation + " " + dist.current_distance);

                if (temp.IsHandleCreated)
                {
                    //temp.Text = arg.sysid.ToString();
                    temp.Invalidate();
                    //temp.Refresh();
                }
                else
                {
                    if (!temp.IsDisposed)
                    {
                        MainV2.instance.Invoke((MethodInvoker)delegate { temp.Show(); });
                    }
                }
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
                public MAV_SENSOR_ORIENTATION Orientation;
                public double Distance;
                public DateTime Received;
                public double Age;

                public DateTime ExpireTime { get { return Received.AddSeconds(Age); } }

                public data(MAV_SENSOR_ORIENTATION orientation, double distance, DateTime received, double age = 1)
                {
                    Orientation = orientation;
                    Distance = distance;
                    Received = received;
                    Age = age;
                }
            }

            public void Add(MAV_SENSOR_ORIENTATION orientation, double distance, DateTime received, double age = 1)
            {
                var existing = _dists.Where((a) => { return a.Orientation == orientation; });

                foreach (var item in existing.ToList())
                {
                    _dists.Remove(item);
                }

                _dists.Add(new data(orientation, distance, received, age));

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