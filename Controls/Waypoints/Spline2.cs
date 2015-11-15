using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MissionPlanner.HIL;
using MissionPlanner.Utilities;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;
using int32_t = System.Int32;

namespace MissionPlanner.Controls.Waypoints
{
    public class Spline2 : MissionPlanner.HIL.Utils
    {
        public Vector3 target_pos = new Vector3();
        public Vector3 target_vel = new Vector3();

        // spline segment end types enum
        public enum spline_segment_end_type
        {
            SEGMENT_END_STOP = 0,
            SEGMENT_END_STRAIGHT,
            SEGMENT_END_SPLINE
        };

        // segment types, either straight or spine
        public enum SegmentType
        {
            SEGMENT_STRAIGHT = 0,
            SEGMENT_SPLINE = 1
        };

        public struct wpnav_flags
        {
            public bool reached_destination; // true if we have reached the destination

            public bool fast_waypoint;
                // true if we should ignore the waypoint radius and consider the waypoint complete once the intermediate target has reached the waypoint

            public SegmentType segment_type; // active segment is either straight or spline
        };

        public wpnav_flags _flags = new wpnav_flags();

        // spline variables
        float _spline_time = 0; // current spline time between origin and destination

        public Vector3 _spline_origin_vel = new Vector3();
            // the target velocity vector at the origin of the spline segment

        Vector3 _spline_destination_vel = new Vector3();
            // the target velocity vector at the destination point of the spline segment

        Vector3[] _hermite_spline_solution = new Vector3[4];
            // array describing spline path between origin and destination

        float _spline_vel_scaler = 0; //
        float _slow_down_dist; // vehicle should begin to slow down once it is within this distance from the destination
        // To-Do: this should be used for straight segments as well
        float _yaw; // heading according to yaw
        private float _wp_accel_cms = 100; // 1 m/s/s
        private uint32_t _wp_last_update;
        private float _wp_speed_cms = 600; // 6 m/s
        public Vector3 _origin;
        public Vector3 _destination;
        private object _wp_speed_up_cms = 100;
        //private int _wp_speed_down_cms = 100;

        double scaleLongDown;
        double scaleLongUp;

        // pv_latlon_to_vector - convert lat/lon coordinates to a position vector
        public Vector3 pv_location_to_vector(PointLatLngAlt loc)
        {
            PointLatLngAlt home = MainV2.comPort.MAV.cs.HomeLocation;

            scaleLongDown = longitude_scale(home);
            scaleLongUp = 1.0f/scaleLongDown;

            Vector3 tmp = new Vector3((loc.Lat*1.0e7f - home.Lat*1.0e7f)*LATLON_TO_CM,
                (loc.Lng*1.0e7f - home.Lng*1.0e7f)*LATLON_TO_CM*scaleLongDown, loc.Alt*100);
            return tmp;
        }

        public PointLatLngAlt pv_vector_to_location(Vector3 loc)
        {
            return new PointLatLngAlt(pv_get_lat(loc)*1.0e-7f, pv_get_lon(loc)*1.0e-7f, loc.z/100, "");
        }

        // pv_get_lon - extract latitude from position vector
        int32_t pv_get_lat(Vector3 pos_vec)
        {
            PointLatLngAlt home = MainV2.comPort.MAV.cs.HomeLocation;

            return (int32_t) (home.Lat*1.0e7f) + (int32_t) (pos_vec.x/LATLON_TO_CM);
        }

        // pv_get_lon - extract longitude from position vector
        int32_t pv_get_lon(Vector3 pos_vec)
        {
            PointLatLngAlt home = MainV2.comPort.MAV.cs.HomeLocation;

            return (int32_t) (home.Lng*1.0e7f) + (int32_t) (pos_vec.y/LATLON_TO_CM*scaleLongUp);
        }

        const float LATLON_TO_CM = 1.113195f;

        float longitude_scale(PointLatLngAlt loc)
        {
            float scale = 1.0f;
            scale = (float) Math.Cos(loc.Lat*deg2rad);
            return scale;
        }

        private void calculate_wp_leash_length()
        {
            //throw new NotImplementedException();
        }

        /// set_origin_and_destination - set origin and destination using lat/lon coordinates
        public void set_wp_origin_and_destination(Vector3 origin, Vector3 destination)
        {
            // store origin and destination locations
            _origin = origin;
            _destination = destination;
            Vector3 pos_delta = _destination - _origin;

            _track_length = pos_delta.length(); // get track length

            _flags.reached_destination = false;
            _flags.fast_waypoint = false; // default waypoint back to slow
            _flags.segment_type = SegmentType.SEGMENT_STRAIGHT;
        }

        /// set_spline_destination waypoint using position vector (distance from home in cm)
        ///     seg_type should be calculated by calling function based on the mission
        public void set_spline_destination(Vector3 destination, bool stopped_at_start,
            spline_segment_end_type seg_end_type, Vector3 next_destination)
        {
            // should be origin
            Vector3 origin;

            // if waypoint controller is active and copter has reached the previous waypoint use it for the origin
            if (_flags.reached_destination)
            {
// && ((hal.scheduler->millis() - _wp_last_update) < 1000) ) {
                origin = _destination;
            }
            else
            {
                // my edit
                origin = _origin;
                // otherwise calculate origin from the current position and velocity
                //_pos_control.get_stopping_point_xy(origin);
                //_pos_control.get_stopping_point_z(origin);
            }

            // set origin and destination
            set_spline_origin_and_destination(origin, destination, stopped_at_start, seg_end_type, next_destination);
        }

        /// set_spline_origin_and_destination - set origin and destination waypoints using position vectors (distance from home in cm)
        ///     seg_type should be calculated by calling function based on the mission
        void set_spline_origin_and_destination(Vector3 origin, Vector3 destination, bool stopped_at_start,
            spline_segment_end_type seg_end_type, Vector3 next_destination)
        {
            // mission is "active" if wpnav has been called recently and vehicle reached the previous waypoint
            bool prev_segment_exists = (_flags.reached_destination);
                // && ((hal.scheduler->millis() - _wp_last_update) < 1000));

            // segment start types
            // stop - vehicle is not moving at origin
            // straight-fast - vehicle is moving, previous segment is straight.  vehicle will fly straight through the waypoint before beginning it's spline path to the next wp
            //     _flag.segment_type holds whether prev segment is straight vs spline but we don't know if it has a delay
            // spline-fast - vehicle is moving, previous segment is splined, vehicle will fly through waypoint but previous segment should have it flying in the correct direction (i.e. exactly parallel to position difference vector from previous segment's origin to this segment's destination)

            // calculate spline velocity at origin
            if (stopped_at_start || !prev_segment_exists)
            {
                // if vehicle is stopped at the origin, set origin velocity to 0.1 * distance vector from origin to destination
                _spline_origin_vel = (destination - origin)*0.1f;
                _spline_time = 0.0f;
                _spline_vel_scaler = 0.0f;
            }
            else
            {
                // look at previous segment to determine velocity at origin
                if (_flags.segment_type == SegmentType.SEGMENT_STRAIGHT)
                {
                    // previous segment is straight, vehicle is moving so vehicle should fly straight through the origin
                    // before beginning it's spline path to the next waypoint. Note: we are using the previous segment's origin and destination
                    _spline_origin_vel = (_destination - _origin);
                    _spline_time = 0.0f;
                        // To-Do: this should be set based on how much overrun there was from straight segment?
                    _spline_vel_scaler = 0.0f;
                        // To-Do: this should be set based on speed at end of prev straight segment?
                }
                else
                {
                    // previous segment is splined, vehicle will fly through origin
                    // we can use the previous segment's destination velocity as this segment's origin velocity
                    // Note: previous segment will leave destination velocity parallel to position difference vector
                    //       from previous segment's origin to this segment's destination)
                    _spline_origin_vel = _spline_destination_vel;
                    if (_spline_time > 1.0f && _spline_time < 1.1f)
                    {
                        // To-Do: remove hard coded 1.1f
                        _spline_time -= 1.0f;
                    }
                    else
                    {
                        _spline_time = 0.0f;
                    }
                    _spline_vel_scaler = 0.0f;
                }
            }

            // calculate spline velocity at destination
            switch (seg_end_type)
            {
                case spline_segment_end_type.SEGMENT_END_STOP:
                    // if vehicle stops at the destination set destination velocity to 0.1 * distance vector from origin to destination
                    _spline_destination_vel = (destination - origin)*0.1f;
                    _flags.fast_waypoint = false;
                    break;

                case spline_segment_end_type.SEGMENT_END_STRAIGHT:
                    // if next segment is straight, vehicle's final velocity should face along the next segment's position
                    _spline_destination_vel = (next_destination - destination);
                    _flags.fast_waypoint = true;
                    break;

                case spline_segment_end_type.SEGMENT_END_SPLINE:
                    // if next segment is splined, vehicle's final velocity should face parallel to the line from the origin to the next destination
                    _spline_destination_vel = (next_destination - origin);
                    _flags.fast_waypoint = true;
                    break;
            }

            // code below ensures we don't get too much overshoot when the next segment is short
            float vel_len = (float) ((_spline_origin_vel + _spline_destination_vel).length());
            float pos_len = (float) (destination - origin).length()*4.0f;
            if (vel_len > pos_len)
            {
                // if total start+stop velocity is more than twice position difference
                // use a scaled down start and stop velocityscale the  start and stop velocities down
                float vel_scaling = pos_len/vel_len;
                // update spline calculator
                update_spline_solution(origin, destination, _spline_origin_vel*vel_scaling,
                    _spline_destination_vel*vel_scaling);
            }
            else
            {
                // update spline calculator
                update_spline_solution(origin, destination, _spline_origin_vel, _spline_destination_vel);
            }

            // initialise yaw heading to current heading
            //_yaw = _ahrs->yaw_sensor;

            // store origin and destination locations
            _origin = origin;
            _destination = destination;

            // calculate slow down distance
            calc_slow_down_distance(_wp_speed_cms, _wp_accel_cms);

            // initialise intermediate point to the origin
            //_pos_control.set_pos_target(origin);
            _flags.reached_destination = false;
            _flags.segment_type = SegmentType.SEGMENT_SPLINE;
        }

        /// calc_slow_down_distance - calculates distance before waypoint that target point should begin to slow-down assuming it is travelling at full speed
        void calc_slow_down_distance(float speed_cms, float accel_cmss)
        {
            // protect against divide by zero
            if (accel_cmss <= 0.0f)
            {
                _slow_down_dist = 0.0f;
                return;
            }
            // To-Do: should we use a combination of horizontal and vertical speeds?
            // To-Do: update this automatically when speed or acceleration is changed
            _slow_down_dist = speed_cms*speed_cms/(4.0f*accel_cmss);
        }


        /// update_spline - update spline controller
        public void update_spline()
        {
            // exit immediately if this is not a spline segment
            if (_flags.segment_type != SegmentType.SEGMENT_SPLINE)
            {
                return;
            }

            // calculate dt
            uint32_t now = 0; //hal.scheduler->millis();
            float dt = (now - _wp_last_update)/1000.0f;

            // reset step back to 0 if 0.1 seconds has passed and we completed the last full cycle
            if (dt > 0.095f)
            {
                // double check dt is reasonable
                if (dt >= 1.0f)
                {
                    dt = 0.0f;
                }
                // capture time since last iteration
                _wp_last_update = now;

                // advance the target if necessary
                advance_spline_target_along_track(dt);
                //_pos_control.trigger_xy();
            }
            else
            {
                // run position controller
                //_pos_control.update_pos_controller(false);
            }
        }

        void update_spline_solution(Vector3 origin, Vector3 dest, Vector3 origin_vel, Vector3 dest_vel)
        {
            _hermite_spline_solution[0] = origin;
            _hermite_spline_solution[1] = origin_vel;
            _hermite_spline_solution[2] = -origin*3.0f - origin_vel*2.0f + dest*3.0f - dest_vel;
            _hermite_spline_solution[3] = origin*2.0f + origin_vel - dest*2.0f + dest_vel;
        }

        public void advance_spline_target_along_track(float dt)
        {
            if (!_flags.reached_destination)
            {
                // update target position and velocity from spline calculator
                calc_spline_pos_vel(_spline_time, ref target_pos, ref target_vel);

                // update velocity
                float spline_dist_to_wp = (float) (_destination - target_pos).length();

                // if within the stopping distance from destination, set target velocity to sqrt of distance * 2 * acceleration
                if (!_flags.fast_waypoint && spline_dist_to_wp < _slow_down_dist)
                {
                    _spline_vel_scaler = (float) Math.Sqrt(spline_dist_to_wp*2.0f*_wp_accel_cms);
                }
                else if (_spline_vel_scaler < _wp_speed_cms)
                {
                    // increase velocity using acceleration
                    // To-Do: replace 0.1f below with update frequency passed in from main program
                    _spline_vel_scaler += _wp_accel_cms*dt;
                }

                // constrain target velocity
                if (_spline_vel_scaler > _wp_speed_cms)
                {
                    _spline_vel_scaler = _wp_speed_cms;
                }

                // scale the spline_time by the velocity we've calculated vs the velocity that came out of the spline calculator
                float spline_time_scale = (float) (_spline_vel_scaler/target_vel.length());

                // update target position
                //_pos_control.set_pos_target(target_pos);

                // update the yaw
                _yaw = RadiansToCentiDegrees(atan2(target_vel.y, target_vel.x));

                // advance spline time to next step
                _spline_time += spline_time_scale*dt;

                // 20 segments per spline
                // _spline_time +=0.05f;

                // we will reach the next waypoint in the next step so set reached_destination flag
                // To-Do: is this one step too early?
                if (_spline_time >= 1.0f)
                {
                    _flags.reached_destination = true;
                }
            }
        }

        private float RadiansToCentiDegrees(double p)
        {
            return (float) (p*rad2deg);
        }


        void calc_spline_pos_vel(float spline_time, ref Vector3 position, ref Vector3 velocity)
        {
            float spline_time_sqrd = spline_time*spline_time;
            float spline_time_cubed = spline_time_sqrd*spline_time;

            position = _hermite_spline_solution[0] +
                       _hermite_spline_solution[1]*spline_time +
                       _hermite_spline_solution[2]*spline_time_sqrd +
                       _hermite_spline_solution[3]*spline_time_cubed;

            velocity = _hermite_spline_solution[1] +
                       _hermite_spline_solution[2]*2.0f*spline_time +
                       _hermite_spline_solution[3]*3.0f*spline_time_sqrd;
        }

        public double _track_length { get; set; }
    }
}