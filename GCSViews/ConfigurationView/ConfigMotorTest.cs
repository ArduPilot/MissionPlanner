using MissionPlanner.Controls;
using MissionPlanner.Utilities;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using Newtonsoft.Json;
using System.IO;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigMotorTest : MyUserControl, IActivate
    {
        public ConfigMotorTest()
        {
            InitializeComponent();
        }

        private int motormax = 0;

        private struct _motors
        {
            public int Number { get; set; }
            public int TestOrder { get; set; }
            public string Rotation { get; set; }
            public float Roll { get; set; }
            public float Pitch { get; set; }
        }
        struct _layouts
         {
            public int Class { get; set; }
            public int Type { get; set; }
            public _motors[] motors { get; set; }
        }
        private struct JSON_motors
        {
            public string Version { get; set; }
            public _layouts[] layouts { get; set; }
        }
        private _layouts motor_layout;

        public void Activate()
        {
            var x = 6;
            var y = 75;

            motormax = this.get_motormax();

            MyButton but;
            for (var a = 1; a <= motormax; a++)
            {
                but = new MyButton();
                but.Text = "Test motor " + (char)((a - 1) + 'A');
                but.Location = new Point(x, y);
                but.Click += but_Click;
                but.Tag = a;

                groupBox1.Controls.Add(but);

                if (motor_layout.motors != null)
                {
                    foreach (var motor in motor_layout.motors)
                    {
                        if (motor.TestOrder == a)
                        {
                            var lab = new Label();
                            lab.Text += "Motor Number: " + motor.Number;
                            if (motor.Rotation != "?")
                            {
                                lab.Text += ", " + motor.Rotation;
                            }
                            lab.Location = new Point(x + 85, y + 5);
                            lab.Width = 150;
                            groupBox1.Controls.Add(lab);
                        }
                    }
                }

                y += 25;
            }

            but = new MyButton();
            but.Text = "Test all motors";
            but.Location = new Point(x, y);
            but.Size = new Size(75, 37);
            but.Click += but_TestAll;
            groupBox1.Controls.Add(but);

            y += 39;

            but = new MyButton();
            but.Text = "Stop all motors";
            but.Location = new Point(x, y);
            but.Size = new Size(75, 37);
            but.Click += but_StopAll;
            groupBox1.Controls.Add(but);

            y += 39;

            but = new MyButton();
            but.Text = "Test all in Sequence";
            but.Location = new Point(x, y);
            but.Size = new Size(75, 37);
            but.Click += but_TestAllSeq;
            groupBox1.Controls.Add(but);

            Utilities.ThemeManager.ApplyThemeTo(this);
        }

        private int get_motormax()
        {
            var motormax = 8;

            if (MainV2.comPort.MAV.aptype == MAVLink.MAV_TYPE.GROUND_ROVER || MainV2.comPort.MAV.aptype == MAVLink.MAV_TYPE.SURFACE_BOAT)
            {
                return 4;
            }

            var enable = MainV2.comPort.MAV.param.ContainsKey("FRAME") || MainV2.comPort.MAV.param.ContainsKey("Q_FRAME_TYPE") || MainV2.comPort.MAV.param.ContainsKey("FRAME_TYPE");

            if (!enable)
            {
                Enabled = false;
                return motormax;
            }

            if (set_frame_class_and_type("FRAME_CLASS", "FRAME_TYPE") ||
                set_frame_class_and_type("Q_FRAME_CLASS", "Q_FRAME_TYPE"))
            {
                if (motor_layout.motors != null)
                {
                    return motor_layout.motors.Length;
                }
            }

            MAVLink.MAV_TYPE type = MAVLink.MAV_TYPE.QUADROTOR;

            if (MainV2.comPort.MAV.param.ContainsKey("Q_FRAME_CLASS"))
            {
                var value = (int)MainV2.comPort.MAV.param["Q_FRAME_CLASS"].Value;
                switch (value)
                {
                    case 0:
                    case 1:
                        type = MAVLink.MAV_TYPE.QUADROTOR;
                        break;
                    case 2:
                    case 5:
                        type = MAVLink.MAV_TYPE.HEXAROTOR;
                        break;
                    case 3:
                    case 4:
                        type = MAVLink.MAV_TYPE.OCTOROTOR;
                        break;
                    case 6:
                        type = MAVLink.MAV_TYPE.HELICOPTER;
                        break;
                    case 7:
                        type = MAVLink.MAV_TYPE.TRICOPTER;
                        break;
                }

            }
            else if (MainV2.comPort.MAV.param.ContainsKey("FRAME"))
            {
                type = MainV2.comPort.MAV.aptype;
            }
            else if (MainV2.comPort.MAV.param.ContainsKey("FRAME_TYPE"))
            {
                type = MainV2.comPort.MAV.aptype;
            }

            if (type == MAVLink.MAV_TYPE.TRICOPTER)
            {
                motormax = 4;
            }
            else if (type == MAVLink.MAV_TYPE.QUADROTOR)
            {
                motormax = 4;
            }
            else if (type == MAVLink.MAV_TYPE.HEXAROTOR)
            {
                motormax = 6;
            }
            else if (type == MAVLink.MAV_TYPE.OCTOROTOR)
            {
                motormax = 8;
            }
            else if (type == MAVLink.MAV_TYPE.HELICOPTER)
            {
                motormax = 0;
            }
            else if (type == MAVLink.MAV_TYPE.DODECAROTOR)
            {
                motormax = 12;
            }

            return motormax;
        }

        private bool set_frame_class_and_type(string class_param_name, string type_param_name)
        {
            if (!MainV2.comPort.MAV.param.ContainsKey(class_param_name) || !MainV2.comPort.MAV.param.ContainsKey(type_param_name))
            {
                return false;
            }
            var frame_class = (int)MainV2.comPort.MAV.param[class_param_name].Value;
            var class_list = ParameterMetaDataRepository.GetParameterOptionsInt(class_param_name, MainV2.comPort.MAV.cs.firmware.ToString());
            foreach (var item in class_list)
            {
                if (item.Key == Convert.ToInt32(frame_class))
                {
                    FrameClass.Text = "Class: " + item.Value;
                    break;
                }
            }

            var frame_type = (int)MainV2.comPort.MAV.param[type_param_name].Value;
            var type_list = ParameterMetaDataRepository.GetParameterOptionsInt(type_param_name, MainV2.comPort.MAV.cs.firmware.ToString());
            foreach (var item in type_list)
            {
                if (item.Key == Convert.ToInt32(frame_type))
                {
                    FrameType.Text = "Type: " + item.Value;
                    break;
                }
            }

            lookup_frame_layout(frame_class, frame_type);

            return true;
        }


        private void lookup_frame_layout(int frame_class, int frame_type)
        {
            try
            {
                string file = Path.GetDirectoryName(Path.GetFullPath(Assembly.GetExecutingAssembly().Location)) + Path.DirectorySeparatorChar + "APMotorLayout.json";
                using (StreamReader r = new StreamReader(file))
                {
                    string json = r.ReadToEnd();
                    var all_layouts = JsonConvert.DeserializeObject<JSON_motors>(json);
                    if (all_layouts.Version == "AP_Motors library test ver 1.2")
                    {
                        foreach (var layout in all_layouts.layouts)
                        {
                            if ((layout.Class == frame_class) && (layout.Type == frame_type))
                            {
                                motor_layout = layout;
                                break;
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void but_TestAll(object sender, EventArgs e)
        {
            int speed = (int)NUM_thr_percent.Value;
            int time = (int)NUM_duration.Value;

            for (int i = 1; i <= motormax; i++)
            {
                testMotor(i, speed, time);
            }
        }

        private void but_TestAllSeq(object sender, EventArgs e)
        {
            int speed = (int)NUM_thr_percent.Value;
            int time = (int)NUM_duration.Value;

            testMotor(1, speed, time, motormax);
        }

        private void but_StopAll(object sender, EventArgs e)
        {
            for (int i = 1; i <= motormax; i++)
            {
                testMotor(i, 0, 0);
            }
        }

        private void but_Click(object sender, EventArgs e)
        {
            int speed = (int)NUM_thr_percent.Value;
            int time = (int)NUM_duration.Value;
            try
            {
                var motor = (int)((MyButton)sender).Tag;
                this.testMotor(motor, speed, time);
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Failed to test motor\n" + ex);
            }
        }

        private void testMotor(int motor, int speed, int time, int motorcount = 0)
        {
            try
            {
                if (!MainV2.comPort.doCommand((byte)MainV2.comPort.sysidcurrent,
                        (byte)MainV2.comPort.compidcurrent,
                        MAVLink.MAV_CMD.DO_MOTOR_TEST,
                        (float)motor,
                        (float)(byte)MAVLink.MOTOR_TEST_THROTTLE_TYPE.MOTOR_TEST_THROTTLE_PERCENT,
                        (float)speed,
                        (float)time,
                        (float)motorcount,
                        0,
                        0))
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
                Process.Start("https://ardupilot.org/copter/docs/connect-escs-and-motors.html#motor-order-diagrams");
            }
            catch
            {
                CustomMessageBox.Show("Bad default system association", Strings.ERROR);
            }
        }

        private async void but_mot_spin_arm_Click(object sender, EventArgs e)
        {
            this.Enabled = false;

            if (!MainV2.comPort.MAV.param.ContainsKey("MOT_SPIN_ARM"))
            {
                CustomMessageBox.Show("param MOT_SPIN_ARM missing", Strings.ERROR);
                return;
            }

            if (NUM_thr_percent.Value < 20)
            {
                var value = (int)NUM_thr_percent.Value + 2;
                if (InputBox.Show(Strings.ChangeThrottle, "Enter arm throttle % (deadzone + 2%)", ref value) == DialogResult.OK)
                {
                    await MainV2.comPort.setParamAsync((byte)MainV2.comPort.sysidcurrent,
                        (byte)MainV2.comPort.compidcurrent, "MOT_SPIN_ARM",
                        (float)value / 100.0f).ConfigureAwait(true);
                }
            }
            else
            {
                CustomMessageBox.Show("Throttle percent above 20, too high", Strings.ERROR);
            }

            this.Enabled = true;
        }

        private async void but_mot_spin_min_Click(object sender, EventArgs e)
        {
            this.Enabled = false;

            if (!MainV2.comPort.MAV.param.ContainsKey("MOT_SPIN_MIN"))
            {
                CustomMessageBox.Show("param MOT_SPIN_MIN missing", Strings.ERROR);
                return;
            }

            if (NUM_thr_percent.Value < 20)
            {
                var value = (int)MainV2.comPort.MAV.param["MOT_SPIN_MIN"].Value + 3;
                if (InputBox.Show(Strings.ChangeThrottle, "Enter min spin throttle % (arm min + 3%)", ref value) ==
                    DialogResult.OK)
                {
                    await MainV2.comPort.setParamAsync((byte)MainV2.comPort.sysidcurrent,
                        (byte)MainV2.comPort.compidcurrent, "MOT_SPIN_MIN",
                        (float)value/100.0f).ConfigureAwait(true);
                }
            }
            else
            {
                CustomMessageBox.Show("Throttle percent above 20, too high", Strings.ERROR);
            }

            this.Enabled = true;
        }
    }
}