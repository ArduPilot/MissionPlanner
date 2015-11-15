using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using log4net;
using MissionPlanner.HIL;
using MissionPlanner.GCSViews;


namespace MissionPlanner.HIL
{
    public class Motor : Utils
    {
        // offsets for motors in motor_out, _motor_filtered and _motor_to_channel_map arrays
        const int AP_MOTORS_MOT_1 = 0;
        const int AP_MOTORS_MOT_2 = 1;
        const int AP_MOTORS_MOT_3 = 2;
        const int AP_MOTORS_MOT_4 = 3;
        const int AP_MOTORS_MOT_5 = 4;
        const int AP_MOTORS_MOT_6 = 5;
        const int AP_MOTORS_MOT_7 = 6;
        const int AP_MOTORS_MOT_8 = 7;

        // frame definitions
        const int AP_MOTORS_PLUS_FRAME = 0;
        const int AP_MOTORS_X_FRAME = 1;
        const int AP_MOTORS_V_FRAME = 2;
        const int AP_MOTORS_H_FRAME = 3; // same as X frame but motors spin in opposite direction
        const int AP_MOTORS_VTAIL_FRAME = 4; // Lynxmotion Hunter VTail 400/500

        const int AP_MOTORS_NEW_PLUS_FRAME = 10;
            // NEW frames are same as original 4 but with motor orders changed to be clockwise from the front

        const int AP_MOTORS_NEW_X_FRAME = 11;
        const int AP_MOTORS_NEW_V_FRAME = 12;
        const int AP_MOTORS_NEW_H_FRAME = 13; // same as X frame but motors spin in opposite direction

        const int AP_MOTORS_MATRIX_YAW_FACTOR_CW = -1;
        const int AP_MOTORS_MATRIX_YAW_FACTOR_CCW = 1;

        new const bool True = true;
        new const bool False = false;

        public Motor self;
        public double angle;
        public bool clockwise;
        public double servo;
        public int testing_order;

        static Motor[] motors;

        public Motor(double angle, bool clockwise, double servo, int testing_order)
        {
            self = this;
            self.angle = (angle + 360)%360;
            self.clockwise = clockwise;
            self.servo = servo;
            self.testing_order = testing_order;
        }

        public static Motor[] build_motors(MAVLink.MAV_TYPE frame, int frame_orientation)
        {
            motors = new Motor[0];

            if (frame == MAVLink.MAV_TYPE.HEXAROTOR) // y6
            {
                // hard coded config for supported frames
                if (frame_orientation == AP_MOTORS_PLUS_FRAME)
                {
                    // plus frame set-up
                    add_motor(AP_MOTORS_MOT_1, 0, AP_MOTORS_MATRIX_YAW_FACTOR_CW, 1);
                    add_motor(AP_MOTORS_MOT_2, 180, AP_MOTORS_MATRIX_YAW_FACTOR_CCW, 4);
                    add_motor(AP_MOTORS_MOT_3, -120, AP_MOTORS_MATRIX_YAW_FACTOR_CW, 5);
                    add_motor(AP_MOTORS_MOT_4, 60, AP_MOTORS_MATRIX_YAW_FACTOR_CCW, 2);
                    add_motor(AP_MOTORS_MOT_5, -60, AP_MOTORS_MATRIX_YAW_FACTOR_CCW, 6);
                    add_motor(AP_MOTORS_MOT_6, 120, AP_MOTORS_MATRIX_YAW_FACTOR_CW, 3);
                }
                else
                {
                    // X frame set-up
                    add_motor(AP_MOTORS_MOT_1, 90, AP_MOTORS_MATRIX_YAW_FACTOR_CW, 2);
                    add_motor(AP_MOTORS_MOT_2, -90, AP_MOTORS_MATRIX_YAW_FACTOR_CCW, 5);
                    add_motor(AP_MOTORS_MOT_3, -30, AP_MOTORS_MATRIX_YAW_FACTOR_CW, 6);
                    add_motor(AP_MOTORS_MOT_4, 150, AP_MOTORS_MATRIX_YAW_FACTOR_CCW, 3);
                    add_motor(AP_MOTORS_MOT_5, 30, AP_MOTORS_MATRIX_YAW_FACTOR_CCW, 1);
                    add_motor(AP_MOTORS_MOT_6, -150, AP_MOTORS_MATRIX_YAW_FACTOR_CW, 4);
                }
            }
            else if (frame == MAVLink.MAV_TYPE.HEXAROTOR && false) // y6
            {
                if (frame_orientation >= AP_MOTORS_NEW_PLUS_FRAME)
                {
                    // Y6 motor definition with all top motors spinning clockwise, all bottom motors counter clockwise
                    add_motor_raw(AP_MOTORS_MOT_1, -1.0, 0.500, AP_MOTORS_MATRIX_YAW_FACTOR_CW, 1);
                    add_motor_raw(AP_MOTORS_MOT_2, -1.0, 0.500, AP_MOTORS_MATRIX_YAW_FACTOR_CCW, 2);
                    add_motor_raw(AP_MOTORS_MOT_3, 0.0, -1.000, AP_MOTORS_MATRIX_YAW_FACTOR_CW, 3);
                    add_motor_raw(AP_MOTORS_MOT_4, 0.0, -1.000, AP_MOTORS_MATRIX_YAW_FACTOR_CCW, 4);
                    add_motor_raw(AP_MOTORS_MOT_5, 1.0, 0.500, AP_MOTORS_MATRIX_YAW_FACTOR_CW, 5);
                    add_motor_raw(AP_MOTORS_MOT_6, 1.0, 0.500, AP_MOTORS_MATRIX_YAW_FACTOR_CCW, 6);
                }
                else
                {
                    // original Y6 motor definition
                    add_motor_raw(AP_MOTORS_MOT_1, -1.0, 0.666, AP_MOTORS_MATRIX_YAW_FACTOR_CCW, 2);
                    add_motor_raw(AP_MOTORS_MOT_2, 1.0, 0.666, AP_MOTORS_MATRIX_YAW_FACTOR_CW, 5);
                    add_motor_raw(AP_MOTORS_MOT_3, 1.0, 0.666, AP_MOTORS_MATRIX_YAW_FACTOR_CCW, 6);
                    add_motor_raw(AP_MOTORS_MOT_4, 0.0, -1.333, AP_MOTORS_MATRIX_YAW_FACTOR_CW, 4);
                    add_motor_raw(AP_MOTORS_MOT_5, -1.0, 0.666, AP_MOTORS_MATRIX_YAW_FACTOR_CW, 1);
                    add_motor_raw(AP_MOTORS_MOT_6, 0.0, -1.333, AP_MOTORS_MATRIX_YAW_FACTOR_CCW, 3);
                }
            }
            else if (frame == MAVLink.MAV_TYPE.OCTOROTOR)
            {
                // hard coded config for supported frames
                if (frame_orientation == AP_MOTORS_PLUS_FRAME)
                {
                    // plus frame set-up
                    add_motor(AP_MOTORS_MOT_1, 0, AP_MOTORS_MATRIX_YAW_FACTOR_CW, 1);
                    add_motor(AP_MOTORS_MOT_2, 180, AP_MOTORS_MATRIX_YAW_FACTOR_CW, 5);
                    add_motor(AP_MOTORS_MOT_3, 45, AP_MOTORS_MATRIX_YAW_FACTOR_CCW, 2);
                    add_motor(AP_MOTORS_MOT_4, 135, AP_MOTORS_MATRIX_YAW_FACTOR_CCW, 4);
                    add_motor(AP_MOTORS_MOT_5, -45, AP_MOTORS_MATRIX_YAW_FACTOR_CCW, 8);
                    add_motor(AP_MOTORS_MOT_6, -135, AP_MOTORS_MATRIX_YAW_FACTOR_CCW, 6);
                    add_motor(AP_MOTORS_MOT_7, -90, AP_MOTORS_MATRIX_YAW_FACTOR_CW, 7);
                    add_motor(AP_MOTORS_MOT_8, 90, AP_MOTORS_MATRIX_YAW_FACTOR_CW, 3);
                }
                else if (frame_orientation == AP_MOTORS_V_FRAME)
                {
                    // V frame set-up
                    add_motor_raw(AP_MOTORS_MOT_1, 1.0, 0.34, AP_MOTORS_MATRIX_YAW_FACTOR_CW, 7);
                    add_motor_raw(AP_MOTORS_MOT_2, -1.0, -0.32, AP_MOTORS_MATRIX_YAW_FACTOR_CW, 3);
                    add_motor_raw(AP_MOTORS_MOT_3, 1.0, -0.32, AP_MOTORS_MATRIX_YAW_FACTOR_CCW, 6);
                    add_motor_raw(AP_MOTORS_MOT_4, -0.5, -1.0, AP_MOTORS_MATRIX_YAW_FACTOR_CCW, 4);
                    add_motor_raw(AP_MOTORS_MOT_5, 1.0, 1.0, AP_MOTORS_MATRIX_YAW_FACTOR_CCW, 8);
                    add_motor_raw(AP_MOTORS_MOT_6, -1.0, 0.34, AP_MOTORS_MATRIX_YAW_FACTOR_CCW, 2);
                    add_motor_raw(AP_MOTORS_MOT_7, -1.0, 1.0, AP_MOTORS_MATRIX_YAW_FACTOR_CW, 1);
                    add_motor_raw(AP_MOTORS_MOT_8, 0.5, -1.0, AP_MOTORS_MATRIX_YAW_FACTOR_CW, 5);
                }
                else
                {
                    // X frame set-up
                    add_motor(AP_MOTORS_MOT_1, 22.5, AP_MOTORS_MATRIX_YAW_FACTOR_CW, 1);
                    add_motor(AP_MOTORS_MOT_2, -157.5, AP_MOTORS_MATRIX_YAW_FACTOR_CW, 5);
                    add_motor(AP_MOTORS_MOT_3, 67.5, AP_MOTORS_MATRIX_YAW_FACTOR_CCW, 2);
                    add_motor(AP_MOTORS_MOT_4, 157.5, AP_MOTORS_MATRIX_YAW_FACTOR_CCW, 4);
                    add_motor(AP_MOTORS_MOT_5, -22.5, AP_MOTORS_MATRIX_YAW_FACTOR_CCW, 8);
                    add_motor(AP_MOTORS_MOT_6, -112.5, AP_MOTORS_MATRIX_YAW_FACTOR_CCW, 6);
                    add_motor(AP_MOTORS_MOT_7, -67.5, AP_MOTORS_MATRIX_YAW_FACTOR_CW, 7);
                    add_motor(AP_MOTORS_MOT_8, 112.5, AP_MOTORS_MATRIX_YAW_FACTOR_CW, 3);
                }
            }
            else if (frame == MAVLink.MAV_TYPE.OCTOROTOR && false) // octaquad
            {
                // hard coded config for supported frames
                if (frame_orientation == AP_MOTORS_PLUS_FRAME)
                {
                    // plus frame set-up
                    add_motor(AP_MOTORS_MOT_1, 0, AP_MOTORS_MATRIX_YAW_FACTOR_CCW, 1);
                    add_motor(AP_MOTORS_MOT_2, -90, AP_MOTORS_MATRIX_YAW_FACTOR_CW, 7);
                    add_motor(AP_MOTORS_MOT_3, 180, AP_MOTORS_MATRIX_YAW_FACTOR_CCW, 5);
                    add_motor(AP_MOTORS_MOT_4, 90, AP_MOTORS_MATRIX_YAW_FACTOR_CW, 3);
                    add_motor(AP_MOTORS_MOT_5, -90, AP_MOTORS_MATRIX_YAW_FACTOR_CCW, 8);
                    add_motor(AP_MOTORS_MOT_6, 0, AP_MOTORS_MATRIX_YAW_FACTOR_CW, 2);
                    add_motor(AP_MOTORS_MOT_7, 90, AP_MOTORS_MATRIX_YAW_FACTOR_CCW, 4);
                    add_motor(AP_MOTORS_MOT_8, 180, AP_MOTORS_MATRIX_YAW_FACTOR_CW, 6);
                }
                else if (frame_orientation == AP_MOTORS_V_FRAME)
                {
                    // V frame set-up
                    add_motor(AP_MOTORS_MOT_1, 45, 0.7981, 1);
                    add_motor(AP_MOTORS_MOT_2, -45, -0.7981, 7);
                    add_motor(AP_MOTORS_MOT_3, -135, 1.0000, 5);
                    add_motor(AP_MOTORS_MOT_4, 135, -1.0000, 3);
                    add_motor(AP_MOTORS_MOT_5, -45, 0.7981, 8);
                    add_motor(AP_MOTORS_MOT_6, 45, -0.7981, 2);
                    add_motor(AP_MOTORS_MOT_7, 135, 1.0000, 4);
                    add_motor(AP_MOTORS_MOT_8, -135, -1.0000, 6);
                }
                else if (frame_orientation == AP_MOTORS_H_FRAME)
                {
                    // H frame set-up - same as X but motors spin in opposite directions
                    add_motor(AP_MOTORS_MOT_1, 45, AP_MOTORS_MATRIX_YAW_FACTOR_CW, 1);
                    add_motor(AP_MOTORS_MOT_2, -45, AP_MOTORS_MATRIX_YAW_FACTOR_CCW, 7);
                    add_motor(AP_MOTORS_MOT_3, -135, AP_MOTORS_MATRIX_YAW_FACTOR_CW, 5);
                    add_motor(AP_MOTORS_MOT_4, 135, AP_MOTORS_MATRIX_YAW_FACTOR_CCW, 3);
                    add_motor(AP_MOTORS_MOT_5, -45, AP_MOTORS_MATRIX_YAW_FACTOR_CW, 8);
                    add_motor(AP_MOTORS_MOT_6, 45, AP_MOTORS_MATRIX_YAW_FACTOR_CCW, 2);
                    add_motor(AP_MOTORS_MOT_7, 135, AP_MOTORS_MATRIX_YAW_FACTOR_CW, 4);
                    add_motor(AP_MOTORS_MOT_8, -135, AP_MOTORS_MATRIX_YAW_FACTOR_CCW, 6);
                }
                else
                {
                    // X frame set-up
                    add_motor(AP_MOTORS_MOT_1, 45, AP_MOTORS_MATRIX_YAW_FACTOR_CCW, 1);
                    add_motor(AP_MOTORS_MOT_2, -45, AP_MOTORS_MATRIX_YAW_FACTOR_CW, 7);
                    add_motor(AP_MOTORS_MOT_3, -135, AP_MOTORS_MATRIX_YAW_FACTOR_CCW, 5);
                    add_motor(AP_MOTORS_MOT_4, 135, AP_MOTORS_MATRIX_YAW_FACTOR_CW, 3);
                    add_motor(AP_MOTORS_MOT_5, -45, AP_MOTORS_MATRIX_YAW_FACTOR_CCW, 8);
                    add_motor(AP_MOTORS_MOT_6, 45, AP_MOTORS_MATRIX_YAW_FACTOR_CW, 2);
                    add_motor(AP_MOTORS_MOT_7, 135, AP_MOTORS_MATRIX_YAW_FACTOR_CCW, 4);
                    add_motor(AP_MOTORS_MOT_8, -135, AP_MOTORS_MATRIX_YAW_FACTOR_CW, 6);
                }
            }
            else if (frame == MAVLink.MAV_TYPE.QUADROTOR)
            {
                // hard coded config for supported frames
                if (frame_orientation == AP_MOTORS_PLUS_FRAME)
                {
                    // plus frame set-up
                    add_motor(AP_MOTORS_MOT_1, 90, AP_MOTORS_MATRIX_YAW_FACTOR_CCW, 2);
                    add_motor(AP_MOTORS_MOT_2, -90, AP_MOTORS_MATRIX_YAW_FACTOR_CCW, 4);
                    add_motor(AP_MOTORS_MOT_3, 0, AP_MOTORS_MATRIX_YAW_FACTOR_CW, 1);
                    add_motor(AP_MOTORS_MOT_4, 180, AP_MOTORS_MATRIX_YAW_FACTOR_CW, 3);
                }
                else if (frame_orientation == AP_MOTORS_V_FRAME)
                {
                    // V frame set-up
                    add_motor(AP_MOTORS_MOT_1, 45, 0.7981, 1);
                    add_motor(AP_MOTORS_MOT_2, -135, 1.0000, 3);
                    add_motor(AP_MOTORS_MOT_3, -45, -0.7981, 4);
                    add_motor(AP_MOTORS_MOT_4, 135, -1.0000, 2);
                }
                else if (frame_orientation == AP_MOTORS_H_FRAME)
                {
                    // H frame set-up - same as X but motors spin in opposite directiSons
                    add_motor(AP_MOTORS_MOT_1, 45, AP_MOTORS_MATRIX_YAW_FACTOR_CW, 1);
                    add_motor(AP_MOTORS_MOT_2, -135, AP_MOTORS_MATRIX_YAW_FACTOR_CW, 3);
                    add_motor(AP_MOTORS_MOT_3, -45, AP_MOTORS_MATRIX_YAW_FACTOR_CCW, 4);
                    add_motor(AP_MOTORS_MOT_4, 135, AP_MOTORS_MATRIX_YAW_FACTOR_CCW, 2);
                }
                else if (frame_orientation == AP_MOTORS_VTAIL_FRAME)
                {
                    /* Lynxmotion Hunter Vtail 400/500

                       Roll control comes only from the front motors, Yaw control only from the rear motors
                       roll factor is measured by the angle perpendicular to that of the prop arm to the roll axis (x)
                       pitch factor is measured by the angle perpendicular to the prop arm to the pitch axis (y)

                       assumptions:
                                            20      20
                        \      /          3_____________1
                         \    /                  |
                          \  /                   |
                       40  \/  40            20  |  20
                          Tail                  / \
                                               2   4

                       All angles measured from their closest axis

                       Note: if we want the front motors to help with yaw,
                             motors 1's yaw factor should be changed to sin(radians(40)).  Where "40" is the vtail angle
                             motors 3's yaw factor should be changed to -sin(radians(40))
                     */

                    // front right: 70 degrees right of roll axis, 20 degrees up of pitch axis, no yaw
                    add_motor_raw(AP_MOTORS_MOT_1, cosf(radians(160)), cosf(radians(-70)), 0, 1);
                    // back left: no roll, 70 degrees down of pitch axis, full yaw
                    add_motor_raw(AP_MOTORS_MOT_2, 0, cosf(radians(160)), AP_MOTORS_MATRIX_YAW_FACTOR_CCW, 3);
                    // front left: 70 degrees left of roll axis, 20 degrees up of pitch axis, no yaw
                    add_motor_raw(AP_MOTORS_MOT_3, cosf(radians(20)), cosf(radians(70)), 0, 4);
                    // back right: no roll, 70 degrees down of pitch axis, full yaw
                    add_motor_raw(AP_MOTORS_MOT_4, 0, cosf(radians(-160)), AP_MOTORS_MATRIX_YAW_FACTOR_CW, 2);
                }
                else
                {
                    // X frame set-up
                    add_motor(AP_MOTORS_MOT_1, 45, AP_MOTORS_MATRIX_YAW_FACTOR_CCW, 1);
                    add_motor(AP_MOTORS_MOT_2, -135, AP_MOTORS_MATRIX_YAW_FACTOR_CCW, 3);
                    add_motor(AP_MOTORS_MOT_3, -45, AP_MOTORS_MATRIX_YAW_FACTOR_CW, 4);
                    add_motor(AP_MOTORS_MOT_4, 135, AP_MOTORS_MATRIX_YAW_FACTOR_CW, 2);
                }
            }
            else if (frame == MAVLink.MAV_TYPE.TRICOPTER)
            {
                if (frame_orientation >= AP_MOTORS_NEW_PLUS_FRAME)
                {
                    // Y6 motor definition with all top motors spinning clockwise, all bottom motors counter clockwise
                    add_motor_raw(AP_MOTORS_MOT_1, -1.0, 0.500, AP_MOTORS_MATRIX_YAW_FACTOR_CW, 1);
                    add_motor_raw(AP_MOTORS_MOT_2, -1.0, 0.500, AP_MOTORS_MATRIX_YAW_FACTOR_CCW, 2);
                    add_motor_raw(AP_MOTORS_MOT_3, 0.0, -1.000, AP_MOTORS_MATRIX_YAW_FACTOR_CW, 3);
                    add_motor_raw(AP_MOTORS_MOT_4, 0.0, -1.000, AP_MOTORS_MATRIX_YAW_FACTOR_CCW, 4);
                    add_motor_raw(AP_MOTORS_MOT_5, 1.0, 0.500, AP_MOTORS_MATRIX_YAW_FACTOR_CW, 5);
                    add_motor_raw(AP_MOTORS_MOT_6, 1.0, 0.500, AP_MOTORS_MATRIX_YAW_FACTOR_CCW, 6);
                }
                else
                {
                    // original Y6 motor definition
                    add_motor_raw(AP_MOTORS_MOT_1, -1.0, 0.666, AP_MOTORS_MATRIX_YAW_FACTOR_CCW, 2);
                    add_motor_raw(AP_MOTORS_MOT_2, 1.0, 0.666, AP_MOTORS_MATRIX_YAW_FACTOR_CW, 5);
                    add_motor_raw(AP_MOTORS_MOT_3, 1.0, 0.666, AP_MOTORS_MATRIX_YAW_FACTOR_CCW, 6);
                    add_motor_raw(AP_MOTORS_MOT_4, 0.0, -1.333, AP_MOTORS_MATRIX_YAW_FACTOR_CW, 4);
                    add_motor_raw(AP_MOTORS_MOT_5, -1.0, 0.666, AP_MOTORS_MATRIX_YAW_FACTOR_CW, 1);
                    add_motor_raw(AP_MOTORS_MOT_6, 0.0, -1.333, AP_MOTORS_MATRIX_YAW_FACTOR_CCW, 3);
                }
            }
            return motors;
        }

        private static void add_motor_raw(int motor_num, double roll_fac, double pitch_fac, double yaw_fac,
            int testing_order)
        {
            if (motors.Length < (motor_num + 1))
            {
                Array.Resize(ref motors, motor_num + 1);
            }

            motors[motor_num] = new Motor(Math.Atan2(-roll_fac, pitch_fac)*rad2deg, yaw_fac > 0, motor_num,
                testing_order);
        }

        private static void add_motor(int motor_num, double angle_degrees, double yaw_factor, int testing_order)
        {
            add_motor_raw(
                motor_num,
                cosf(radians(angle_degrees + 90)), // roll factor
                cosf(radians(angle_degrees)), // pitch factor
                yaw_factor, // yaw factor
                testing_order);
        }
    }

    public class MultiCopter : Aircraft
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        MultiCopter self;

        DateTime seconds = DateTime.Now;

        public double[] motor_speed = null;

        double hover_throttle;
        double terminal_velocity;
        double terminal_rotation_rate;
        Motor[] motors;

        Vector3 old_position;


        //# scaling from total motor power to Newtons. Allows the copter
        //# to hover against gravity when each motor is at hover_throttle
        double thrust_scale;

        DateTime last_time;

        public MultiCopter(string frame = "quadx")
        {
            self = this;

            motors = Motor.build_motors(MAVLink.MAV_TYPE.QUADROTOR,
                (int) MissionPlanner.GCSViews.ConfigurationView.ConfigFrameType.Frame.Plus);
            motor_speed = new double[motors.Length];
            mass = 1.5; // # Kg
            frame_height = 0.1;

            hover_throttle = 0.51;
            terminal_velocity = 15.0;
            terminal_rotation_rate = 4*(360.0*deg2rad);

            thrust_scale = (mass*gravity)/(motors.Length*hover_throttle);

            last_time = DateTime.Now;
        }

        double scale_rc(int sn, float servo, float min, float max)
        {
            return ((servo - 1000)/1000.0);
        }


        public void update(ref double[] servos, Simulation.FGNetFDM fdm)
        {
            for (int i = 0; i < servos.Length; i++)
            {
                var servo = servos[(int) self.motors[i].servo];
                if (servo <= 0.0)
                {
                    motor_speed[i] = 0;
                }
                else
                {
                    motor_speed[i] = scale_rc(i, (float) servo, 0.0f, 1.0f);
                    //servos[i] = motor_speed[i];
                }
            }
            double[] m = motor_speed;

            //# how much time has passed?
            //DateTime t = DateTime.Now;
            //TimeSpan delta_time = t - last_time; // 0.02
            //last_time = t;

            // run at 1000hz lockstep
            TimeSpan delta_time = new TimeSpan(0, 0, 0, 0, 1);

            if (delta_time.TotalMilliseconds > 100) // somethings wrong / debug
            {
                delta_time = new TimeSpan(0, 0, 0, 0, 20);
            }

            // rotational acceleration, in degrees/s/s, in body frame
            Vector3 rot_accel = new Vector3(0, 0, 0);
            double thrust = 0.0;

            foreach (var i in range((self.motors.Length)))
            {
                rot_accel.x += -radians(5000.0)*sin(radians(self.motors[i].angle))*m[i];
                rot_accel.y += radians(5000.0)*cos(radians(self.motors[i].angle))*m[i];
                if (!self.motors[i].clockwise)
                {
                    rot_accel.z -= m[i]*radians(400.0);
                }
                else
                {
                    rot_accel.z += m[i]*radians(400.0);
                }
                thrust += m[i]*self.thrust_scale; // newtons
            }

            //Console.WriteLine("rot_accel " + rot_accel.ToString());

            // rotational air resistance
            rot_accel.x -= self.gyro.x*radians(5000.0)/self.terminal_rotation_rate;
            rot_accel.y -= self.gyro.y*radians(5000.0)/self.terminal_rotation_rate;
            rot_accel.z -= self.gyro.z*radians(400.0)/self.terminal_rotation_rate;

            //  Console.WriteLine("rot_accel " + rot_accel.ToString());

            // update rotational rates in body frame
            self.gyro += rot_accel*delta_time.TotalSeconds;

            //   Console.WriteLine("gyro " + gyro.ToString());

            // update attitude
            self.dcm.rotate(self.gyro*delta_time.TotalSeconds);
            self.dcm.normalize();

            // air resistance
            Vector3 air_resistance = -self.velocity*(self.gravity/self.terminal_velocity);

            accel_body = new Vector3(0, 0, -thrust/self.mass);
            Vector3 accel_earth = self.dcm*accel_body;
            accel_earth += new Vector3(0, 0, self.gravity);
            accel_earth += air_resistance;

            // add in some wind (turn force into accel by dividing by mass).
            // accel_earth += self.wind.drag(self.velocity) / self.mass;

            // if we're on the ground, then our vertical acceleration is limited
            // to zero. This effectively adds the force of the ground on the aircraft
            if (self.on_ground() && accel_earth.z > 0)
                accel_earth.z = 0;

            // work out acceleration as seen by the accelerometers. It sees the kinematic
            // acceleration (ie. real movement), plus gravity
            self.accel_body = self.dcm.transposed()*(accel_earth + new Vector3(0, 0, -self.gravity));

            // new velocity vector
            self.velocity += accel_earth*delta_time.TotalSeconds;

            if (double.IsNaN(velocity.x) || double.IsNaN(velocity.y) || double.IsNaN(velocity.z))
                velocity = new Vector3();

            // new position vector
            old_position = self.position.copy();
            self.position += self.velocity*delta_time.TotalSeconds;

            if (home_latitude == 0)
            {
                home_latitude = fdm.latitude*rad2deg;
                home_longitude = fdm.longitude*rad2deg;
                home_altitude = fdm.altitude;
                ground_level = home_altitude;
            }

            // constrain height to the ground
            if (self.on_ground())
            {
                if (!self.on_ground(old_position))
                    Console.WriteLine("Hit ground at {0} m/s", (self.velocity.z));

                self.velocity = new Vector3(0, 0, 0);
                // zero roll/pitch, but keep yaw
                double r = 0;
                double p = 0;
                double y = 0;
                self.dcm.to_euler(ref r, ref p, ref y);
                self.dcm.from_euler(0, 0, y);

                self.position = new Vector3(self.position.x, self.position.y,
                    -(self.ground_level + self.frame_height - self.home_altitude));
            }

            // update lat/lon/altitude
            self.update_position();
        }
    }
}