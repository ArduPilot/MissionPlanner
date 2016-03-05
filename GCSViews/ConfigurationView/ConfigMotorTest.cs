using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using MissionPlanner.Controls;
using MissionPlanner.HIL;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigMotorTest : UserControl, IActivate
    {
        public ConfigMotorTest()
        {
            InitializeComponent();
        }

        /*
#if (FRAME_CONFIG == QUAD_FRAME)
        MAV_TYPE_QUADROTOR,
#elif (FRAME_CONFIG == TRI_FRAME)
        MAV_TYPE_TRICOPTER,
#elif (FRAME_CONFIG == HEXA_FRAME || FRAME_CONFIG == Y6_FRAME)
        MAV_TYPE_HEXAROTOR,
#elif (FRAME_CONFIG == OCTA_FRAME || FRAME_CONFIG == OCTA_QUAD_FRAME)
        MAV_TYPE_OCTOROTOR,
#elif (FRAME_CONFIG == HELI_FRAME)
        MAV_TYPE_HELICOPTER,
#elif (FRAME_CONFIG == SINGLE_FRAME)  //because mavlink did not define a singlecopter, we use a rocket
        MAV_TYPE_ROCKET,
#elif (FRAME_CONFIG == COAX_FRAME)  //because mavlink did not define a singlecopter, we use a rocket
        MAV_TYPE_ROCKET,
#else
  #error Unrecognised frame type
#endif*/

        public void Activate()
        {
            var x = 20;
            var y = 40;

            var motormax = this.get_motormax();

            MyButton but;
            for (var a = 1; a <= motormax; a++)
            {
                but = new MyButton();
                but.Text = "Test motor " + (char) ((a - 1) + 'A');
                but.Location = new Point(x, y);
                but.Click += but_Click;
                but.Tag = a;

                Controls.Add(but);

                y += 25;
            }

            but = new MyButton();
            but.Text = "Test all motors";
            but.Location = new Point(x, y);
            but.Size = new Size(75, 37);
            but.Click += but_TestAll;
            Controls.Add(but);

            y += 39;

            but = new MyButton();
            but.Text = "Stop all motors";
            but.Location = new Point(x, y);
            but.Size = new Size(75, 37);
            but.Click += but_StopAll;
            Controls.Add(but);
        }

        private int get_motormax()
        {
            var motormax = 8;

            if (!MainV2.comPort.MAV.param.ContainsKey("FRAME"))
            {
                Enabled = false;
                return motormax;
            }

            var motors = new Motor[0];

            if (MainV2.comPort.MAV.aptype == MAVLink.MAV_TYPE.TRICOPTER)
            {
                motormax = 4;

                motors = Motor.build_motors(MAVLink.MAV_TYPE.TRICOPTER, (int) (double) MainV2.comPort.MAV.param["FRAME"]);
            }
            else if (MainV2.comPort.MAV.aptype == MAVLink.MAV_TYPE.QUADROTOR)
            {
                motormax = 4;

                motors = Motor.build_motors(MAVLink.MAV_TYPE.QUADROTOR, (int) (double) MainV2.comPort.MAV.param["FRAME"]);
            }
            else if (MainV2.comPort.MAV.aptype == MAVLink.MAV_TYPE.HEXAROTOR)
            {
                motormax = 6;

                motors = Motor.build_motors(MAVLink.MAV_TYPE.HEXAROTOR, (int) (double) MainV2.comPort.MAV.param["FRAME"]);
            }
            else if (MainV2.comPort.MAV.aptype == MAVLink.MAV_TYPE.OCTOROTOR)
            {
                motormax = 8;

                motors = Motor.build_motors(MAVLink.MAV_TYPE.OCTOROTOR, (int) (double) MainV2.comPort.MAV.param["FRAME"]);
            }
            else if (MainV2.comPort.MAV.aptype == MAVLink.MAV_TYPE.HELICOPTER)
            {
                motormax = 0;
            }

            return motormax;
        }

        private void but_TestAll(object sender, EventArgs e)
        {
            int speed = (int) NUM_thr_percent.Value;
            int time = (int) NUM_duration.Value;

            int motormax = this.get_motormax();
            for (int i = 1; i <= motormax; i++)
            {
                testMotor(i, speed, time);
            }
        }

        private void but_StopAll(object sender, EventArgs e)
        {
            int motormax = this.get_motormax();
            for (int i = 1; i <= motormax; i++)
            {
                testMotor(i, 0, 0);
            }
        }

        private void but_Click(object sender, EventArgs e)
        {
            int speed = (int) NUM_thr_percent.Value;
            int time = (int) NUM_duration.Value;
            try
            {
                var motor = (int) ((MyButton) sender).Tag;
                this.testMotor(motor, speed, time);
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Failed to test motor\n" + ex);
            }
        }

        private void testMotor(int motor, int speed, int time)
        {
            try
            {
                if (
                    !MainV2.comPort.doMotorTest(motor, MAVLink.MOTOR_TEST_THROTTLE_TYPE.MOTOR_TEST_THROTTLE_PERCENT,
                        speed, time))
                {
                    CustomMessageBox.Show("Command was denied by the autopilot");
                }
            }
            catch
            {
                CustomMessageBox.Show(Strings.ErrorCommunicating + "\nMotor: " + motor, Strings.ERROR);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start("http://copter.ardupilot.com/wiki/connect-escs-and-motors/");
            }
            catch
            {
                CustomMessageBox.Show("Bad default system association", Strings.ERROR);
            }
        }
    }
}