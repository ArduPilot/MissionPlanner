using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MissionPlanner.Controls;

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
            int x = 20;
            int y = 40;

            int motormax = 8;

            HIL.Motor[] motors = new HIL.Motor[0];

            if (MainV2.comPort.MAV.aptype == MAVLink.MAV_TYPE.TRICOPTER)
            {
                motormax = 3;

                motors = HIL.Motor.build_motors(MAVLink.MAV_TYPE.TRICOPTER, (int)(float)MainV2.comPort.MAV.param["FRAME"]);
            }
            else if (MainV2.comPort.MAV.aptype == MAVLink.MAV_TYPE.QUADROTOR)
            {
                motormax = 4;

                motors = HIL.Motor.build_motors(MAVLink.MAV_TYPE.QUADROTOR, (int)(float)MainV2.comPort.MAV.param["FRAME"]);
            }
            else if (MainV2.comPort.MAV.aptype == MAVLink.MAV_TYPE.HEXAROTOR)
            {
                motormax = 6;

                motors = HIL.Motor.build_motors(MAVLink.MAV_TYPE.HEXAROTOR, (int)(float)MainV2.comPort.MAV.param["FRAME"]);
            }
            else if (MainV2.comPort.MAV.aptype == MAVLink.MAV_TYPE.OCTOROTOR)
            {
                motormax = 8;

                motors = HIL.Motor.build_motors(MAVLink.MAV_TYPE.OCTOROTOR, (int)(float)MainV2.comPort.MAV.param["FRAME"]);
            }
            else if (MainV2.comPort.MAV.aptype == MAVLink.MAV_TYPE.HELICOPTER)
            {
                motormax = 0;
            }

            for (int a = 1; a <= motormax; a++)
            {

                MyButton but = new MyButton();
                but.Text = "Test motor " + (char)((a-1) + 'A');
                but.Location = new Point(x,y);
                but.Click += but_Click;
                but.Tag = a;

                this.Controls.Add(but);

                y += 25;
            }
        }

        void but_Click(object sender, EventArgs e)
        {
            try
            {

                int motor = (int)((MyButton)sender).Tag;

                if (MainV2.comPort.doMotorTest(motor, MAVLink.MOTOR_TEST_THROTTLE_TYPE.MOTOR_TEST_THROTTLE_PERCENT, (int)NUM_thr_percent.Value, 2))
                {

                }
                else
                {
                    CustomMessageBox.Show("Command was denied by the autopilot");
                }
            }
            catch (Exception ex) 
            {
                CustomMessageBox.Show("Failed to test motor\n" + ex.ToString());
            }
        }



        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://copter.ardupilot.com/wiki/connecting-your-rc-input-and-motors/");
        }
    }
}
