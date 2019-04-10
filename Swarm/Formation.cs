using System;
using System.Collections.Generic;
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;
using ProjNet.CoordinateSystems.Transformations;
using ProjNet.CoordinateSystems;
using MissionPlanner.Utilities;
using MissionPlanner.ArduPilot;
using Vector3 = MissionPlanner.Utilities.Vector3;

namespace MissionPlanner.Swarm
{
    /// <summary>
    /// Follow the leader
    /// </summary>
    class Formation : Swarm
    {
        Dictionary<MAVState, Vector3> offsets = new Dictionary<MAVState, Vector3>();

        private Dictionary<MAVState, Tuple<PID, PID, PID, PID>> pids =
            new Dictionary<MAVState, Tuple<PID, PID, PID, PID>>();

        private PointLatLngAlt masterpos = new PointLatLngAlt();

        public void setOffsets(MAVState mav, double x, double y, double z)
        {
            offsets[mav] = new Vector3(x, y, z);
            log.Info(mav.ToString() + " " + offsets[mav].ToString());
        }

        public Vector3 getOffsets(MAVState mav)
        {
            if (offsets.ContainsKey(mav))
            {
                return offsets[mav];
            }

            return new Vector3(offsets.Count, 0, 0);
        }

        public override void Update()
        {
            if (MainV2.comPort.MAV.cs.lat == 0 || MainV2.comPort.MAV.cs.lng == 0)
                return;

            if (Leader == null)
                Leader = MainV2.comPort.MAV;

            masterpos = new PointLatLngAlt(Leader.cs.lat, Leader.cs.lng, Leader.cs.alt, "");
        }

        double wrap_180(double input)
        {
            if (input > 180)
                return input - 360;
            if (input < -180)
                return input + 360;
            return input;
        }

        //convert Wgs84ConversionInfo to utm
        CoordinateTransformationFactory ctfac = new CoordinateTransformationFactory();

        IGeographicCoordinateSystem wgs84 = GeographicCoordinateSystem.WGS84;

        public override void SendCommand()
        {
            if (masterpos.Lat == 0 || masterpos.Lng == 0)
                return;

            //Console.WriteLine(DateTime.Now);
            //Console.WriteLine("Leader {0} {1} {2}", masterpos.Lat, masterpos.Lng, masterpos.Alt);

            int a = 0;
            foreach (var port in MainV2.Comports.ToArray())
            {
                foreach (var mav in port.MAVlist)
                {
                    if (mav == Leader)
                        continue;

                    PointLatLngAlt target = new PointLatLngAlt(masterpos);

                    try
                    {
                        int utmzone = (int) ((masterpos.Lng - -186.0)/6.0);

                        IProjectedCoordinateSystem utm = ProjectedCoordinateSystem.WGS84_UTM(utmzone,
                            masterpos.Lat < 0 ? false : true);

                        ICoordinateTransformation trans = ctfac.CreateFromCoordinateSystems(wgs84, utm);

                        double[] pll1 = {target.Lng, target.Lat};

                        double[] p1 = trans.MathTransform.Transform(pll1);

                        double heading = -Leader.cs.yaw;

                        double length = offsets[mav].length();

                        var x = ((Vector3) offsets[mav]).x;
                        var y = ((Vector3) offsets[mav]).y;

                        // add offsets to utm
                        p1[0] += x*Math.Cos(heading*MathHelper.deg2rad) - y*Math.Sin(heading*MathHelper.deg2rad);
                        p1[1] += x*Math.Sin(heading*MathHelper.deg2rad) + y*Math.Cos(heading*MathHelper.deg2rad);

                        // convert back to wgs84
                        IMathTransform inversedTransform = trans.MathTransform.Inverse();
                        double[] point = inversedTransform.Transform(p1);

                        target.Lat = point[1];
                        target.Lng = point[0];
                        target.Alt += ((Vector3) offsets[mav]).z;

                        if (mav.cs.firmware == Firmwares.ArduPlane)
                        {
                            // get distance from target position
                            var dist = target.GetDistance(mav.cs.Location);

                            // get bearing to target
                            var targyaw = mav.cs.Location.GetBearing(target);

                            var targettrailer = target.newpos(Leader.cs.yaw, Math.Abs(dist) * -0.25);
                            var targetleader = target.newpos(Leader.cs.yaw, 10 + dist);

                            var yawerror = wrap_180(targyaw - mav.cs.yaw);
                            var mavleadererror = wrap_180(Leader.cs.yaw - mav.cs.yaw);

                            if (dist < 100)
                            {
                                targyaw = mav.cs.Location.GetBearing(targetleader);
                                yawerror = wrap_180(targyaw - mav.cs.yaw);

                                var targBearing = mav.cs.Location.GetBearing(target);

                                // check the bearing for the leader and target are within 45 degrees.
                                if (Math.Abs(wrap_180(targBearing - targyaw)) > 45)
                                    dist *= -1;
                            }
                            else
                            {
                                targyaw = mav.cs.Location.GetBearing(targettrailer);
                                yawerror = wrap_180(targyaw - mav.cs.yaw);
                            }

                            // display update
                            mav.GuidedMode.x = (float)target.Lat;
                            mav.GuidedMode.y = (float)target.Lng;
                            mav.GuidedMode.z = (float)target.Alt;

                            MAVLink.mavlink_set_attitude_target_t att_target = new MAVLink.mavlink_set_attitude_target_t();
                            att_target.target_system = mav.sysid;
                            att_target.target_component = mav.compid;
                            att_target.type_mask = 0xff;

                            Tuple<PID, PID, PID, PID> pid;

                            if (pids.ContainsKey(mav))
                            {
                                pid = pids[mav];
                            }
                            else
                            {
                                pid = new Tuple<PID, PID, PID, PID>(
                                    new PID(1f, .03f, 0.02f, 10, 20, 0.1f, 0),
                                    new PID(1f, .03f, 0.02f, 10, 20, 0.1f, 0),
                                    new PID(1, 0, 0.00f, 15, 20, 0.1f, 0),
                                    new PID(0.01f, 0.001f, 0, 0.5f, 20, 0.1f, 0));
                                pids.Add(mav, pid);
                            }

                            var rollp = pid.Item1;
                            var pitchp = pid.Item2;
                            var yawp = pid.Item3;
                            var thrustp = pid.Item4;

                            var newroll = 0d;
                            var newpitch = 0d;

                            if (true)
                            {
                                var altdelta = target.Alt - mav.cs.alt;
                                newpitch = altdelta;
                                att_target.type_mask -= 0b00000010;

                                pitchp.set_input_filter_all((float)altdelta);

                                newpitch = pitchp.get_pid();
                            }

                            if (true)
                            {
                                var leaderturnrad = Leader.cs.radius;
                                var mavturnradius = leaderturnrad - x;

                                {
                                    var distToTarget = mav.cs.Location.GetDistance(target);
                                    var bearingToTarget = mav.cs.Location.GetBearing(target);

                                    // bearing stability
                                    if (distToTarget < 30)
                                        bearingToTarget = mav.cs.Location.GetBearing(targetleader);
                                    // fly in from behind
                                    if (distToTarget > 100)
                                        bearingToTarget = mav.cs.Location.GetBearing(targettrailer);

                                    var bearingDelta = wrap_180(bearingToTarget - mav.cs.yaw);
                                    var tangent90 = bearingDelta > 0 ? 90 : -90;

                                    newroll = 0;

                                    // if the delta is > 90 then we are facing the wrong direction
                                    if (Math.Abs(bearingDelta) < 85)
                                    {
                                        var insideAngle = Math.Abs(tangent90 - bearingDelta);
                                        var angleCenter = 180 - insideAngle * 2;

                                        // sine rule
                                        var sine1 = Math.Max(distToTarget, 40) /
                                                    Math.Sin(angleCenter * MathHelper.deg2rad);
                                        var radius = sine1 * Math.Sin(insideAngle * MathHelper.deg2rad);

                                        // average calced + leader offset turnradius - acts as a FF
                                        radius = (Math.Abs(radius) + Math.Abs(mavturnradius)) / 2;

                                        var angleBank = ((mav.cs.groundspeed * mav.cs.groundspeed) / radius) / 9.8;

                                        angleBank *= MathHelper.rad2deg;

                                        if (bearingDelta > 0)
                                            newroll = Math.Abs(angleBank);
                                        else
                                            newroll = -Math.Abs(angleBank);
                                    }

                                    newroll += MathHelper.constrain(bearingDelta, -20, 20);
                                }

                                // tr = gs2 / (9.8 * x)
                                // (9.8 * x) * tr = gs2
                                // 9.8 * x = gs2 / tr
                                // (gs2/tr)/9.8 = x

                                var angle = ((mav.cs.groundspeed * mav.cs.groundspeed) / mavturnradius) / 9.8;

                                //newroll = angle * MathHelper.rad2deg;

                                // 1 degree of roll for ever 1 degree of yaw error
                                //newroll += MathHelper.constrain(yawerror, -20, 20);

                                //rollp.set_input_filter_all((float)yawdelta);
                            }

                            // do speed
                            if (true)
                            {
                                //att_target.thrust = (float) MathHelper.mapConstrained(dist, 0, 40, 0, 1);
                                att_target.type_mask -= 0b01000000;

                                // in m out 0-1
                                thrustp.set_input_filter_all((float) dist);

                                // prevent buildup prior to being close
                                if(dist>40)
                                    thrustp.reset_I();

                                // 0.1 demand + pid results
                                att_target.thrust = (float)MathHelper.constrain(thrustp.get_pid(), 0.1, 1);
                            }

                            Quaternion q = new Quaternion();
                            q.from_vector312(newroll * MathHelper.deg2rad, newpitch * MathHelper.deg2rad, yawerror * MathHelper.deg2rad);

                            att_target.q = new float[4];
                            att_target.q[0] = (float) q.q1;
                            att_target.q[1] = (float) q.q2;
                            att_target.q[2] = (float) q.q3;
                            att_target.q[3] = (float) q.q4;
                   
                             //0b0= rpy
                            att_target.type_mask -= 0b10000101;
                            //att_target.type_mask -= 0b10000100;

                            Console.WriteLine("sysid {0} - {1} dist {2} r {3} p {4} y {5}", mav.sysid,
                                att_target.thrust, dist, newroll, newpitch, (targyaw - mav.cs.yaw));

                          /*  Console.WriteLine("rpyt {0} {1} {2} {3} I {4} {5} {6} {7}",
                                rollp.get_pid(), pitchp.get_pid(), yawp.get_pid(), thrustp.get_pid(),
                                rollp.get_i(), pitchp.get_i(), yawp.get_i(), thrustp.get_i());
                                */
                            port.sendPacket(att_target, mav.sysid, mav.compid);
                        }
                        else
                        {
                            Vector3 vel = new Vector3(Leader.cs.vx, Leader.cs.vy, Leader.cs.vz);

                            // do pos/vel
                            port.setPositionTargetGlobalInt(mav.sysid, mav.compid, true,
                                true, false, false,
                                MAVLink.MAV_FRAME.GLOBAL_RELATIVE_ALT_INT, target.Lat, target.Lng, target.Alt, vel.x,
                                vel.y, vel.z, 0, 0);

                            // do yaw
                            if (!gimbal)
                            {
                                // within 3 degrees dont send
                                if (Math.Abs(mav.cs.yaw - Leader.cs.yaw) > 3)
                                    port.doCommand(mav.sysid, mav.compid, MAVLink.MAV_CMD.CONDITION_YAW, Leader.cs.yaw,
                                        100.0f, 0, 0, 0, 0, 0, false);
                            }
                            else
                            {
                                // gimbal direction
                                if (Math.Abs(mav.cs.yaw - Leader.cs.yaw) > 3)
                                    port.setMountControl(mav.sysid, mav.compid, 45, 0, Leader.cs.yaw, false);
                            }
                        }

                        //Console.WriteLine("{0} {1} {2} {3}", port.ToString(), target.Lat, target.Lng, target.Alt);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Failed to send command " + mav.ToString() + "\n" + ex.ToString());
                    }

                    a++;
                }
            }
        }

        public bool gimbal { get; set; }
    }

    public class PID
    {
        /*
                    previous_error = 0
                    integral = 0
                    loop:
                    error = setpoint - measured_value
                        integral = integral + error * dt
                    derivative = (error - previous_error) / dt
                        output = Kp * error + Ki * integral + Kd * derivative
                    previous_error = error
                        wait(dt)
                        goto loop*/
        private float _dt;
        private float M_2PI = (float)(Math.PI * 2);
        private float _input;
        private float _derivative;
        private float _kp;
        private float _ki;
        private float _integrator;
        private float _imax;
        private float _kd;
        private float _ff;
        private float _filt_hz = AC_PID_FILT_HZ_DEFAULT;

        const float AC_PID_FILT_HZ_DEFAULT = 20.0f; // default input filter frequency
        const float AC_PID_FILT_HZ_MIN = 0.01f; // minimum input filter frequency

        // Constructor
        public PID(float initial_p, float initial_i, float initial_d, float initial_imax, float initial_filt_hz, float dt, float initial_ff)
        {
            _dt = dt;
            _integrator = 0.0f;
            _input = 0.0f;
            _derivative = 0.0f;

            _kp = initial_p;
            _ki = initial_i;
            _kd = initial_d;
            _imax = Math.Abs(initial_imax);
            filt_hz(initial_filt_hz);
            _ff = initial_ff;

            // reset input filter to first value received
            _flags._reset_filter = true;
        }

        // set_dt - set time step in seconds
        public void set_dt(float dt)
        {
            // set dt and calculate the input filter alpha
            _dt = dt;
        }

        // filt_hz - set input filter hz
        public void filt_hz(float hz)
        {
            _filt_hz = hz;

            // sanity check _filt_hz
            _filt_hz = Math.Max(_filt_hz, AC_PID_FILT_HZ_MIN);
        }

        public void set_input_filter_all(float input)
        {
            // don't process inf or NaN
            if (!isfinite(input))
            {
                return;
            }

            // reset input filter to value received
            if (_flags._reset_filter)
            {
                _flags._reset_filter = false;
                _input = input;
                _derivative = 0.0f;
            }

            // update filter and calculate derivative
            float input_filt_change = get_filt_alpha() * (input - _input);
            _input = _input + input_filt_change;
            if (_dt > 0.0f)
            {
                _derivative = input_filt_change / _dt;
            }
        }

        private bool isfinite(float input)
        {
            return !float.IsInfinity(input);
        }

        public float get_p()
        {
            _pid_info.P = (_input * _kp);
            return _pid_info.P;
        }

        public float get_i()
        {
            if (!is_zero(_ki) && !is_zero(_dt))
            {
                _integrator += ((float)_input * _ki) * _dt;
                if (_integrator < -_imax)
                {
                    _integrator = -_imax;
                }
                else if (_integrator > _imax)
                {
                    _integrator = _imax;
                }

                _pid_info.I = _integrator;
                return _integrator;
            }

            return 0;
        }

        public float get_d()
        {
            // derivative component
            _pid_info.D = (_kd * _derivative);
            return _pid_info.D;
        }

        public float get_ff(float requested_rate)
        {
            _pid_info.FF = (float)requested_rate * _ff;
            return _pid_info.FF;
        }

        public float get_pi()
        {
            return get_p() + get_i();
        }

        public float get_pid()
        {
            return get_p() + get_i() + get_d();
        }

        public void reset_I()
        {
            _integrator = 0;
        }

        public float get_filt_alpha()
        {
            if (is_zero(_filt_hz))
            {
                return 1.0f;
            }

            // calculate alpha
            float rc = 1 / (M_2PI * _filt_hz);
            return _dt / (_dt + rc);
        }

        private bool is_zero(float filt_hz)
        {
            return filt_hz == 0;
        }

        internal class flags
        {
            internal bool _reset_filter;
        }

        flags _flags = new flags();

        pid_info _pid_info = new pid_info();

        internal class pid_info
        {
            internal float P;
            internal float I;
            internal float D;
            internal float FF;
        }
    }


}