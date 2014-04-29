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

        public void Activate()
        {
            int x = 20;
            int y = 40;

            int motormax = 8;

            //HIL.Motor.build_motors("");


            if (MainV2.comPort.MAV.aptype == MAVLink.MAV_TYPE.QUADROTOR)
            {
                motormax = 4;
            }
            else if (MainV2.comPort.MAV.aptype == MAVLink.MAV_TYPE.HEXAROTOR)
            {
                motormax = 6;
            }
            else if (MainV2.comPort.MAV.aptype == MAVLink.MAV_TYPE.OCTOROTOR)
            {
                motormax = 8;
            }
            else if (MainV2.comPort.MAV.aptype == MAVLink.MAV_TYPE.TRICOPTER)
            {
                motormax = 3;
            }
            else if (MainV2.comPort.MAV.aptype == MAVLink.MAV_TYPE.HELICOPTER)
            {
                motormax = 0;
            }

            for (int a = 1; a <= motormax; a++)
            {

                MyButton but = new MyButton();
                but.Text = "Test motor " + a;
                but.Location = new Point(x,y);
                but.Click += but_Click;
                but.Tag = a;

                this.Controls.Add(but);

                y += 25;
            }
        }

        void but_Click(object sender, EventArgs e)
        {
            int motor = (int)((MyButton)sender).Tag;

            if (doaction(motor, MAVLink.MOTOR_TEST_THROTTLE_TYPE.MOTOR_TEST_THROTTLE_PERCENT, (int)NUM_thr_percent.Value, 2))
            {
                
            }
            else
            {
                CustomMessageBox.Show("Command was denied by the autopilot");
            }
        }

        bool doaction(int motor, MAVLink.MOTOR_TEST_THROTTLE_TYPE thr_type, int throttle, int timeout)
        {
            return MainV2.comPort.doCommand(MAVLink.MAV_CMD.DO_MOTOR_TEST, (float)motor, (float)(byte)thr_type, (float)throttle, (float)timeout, 0, 0, 0);
        }
    }
}
