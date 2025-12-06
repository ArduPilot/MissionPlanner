using MissionPlanner.Controls;
using MissionPlanner.Utilities;
using System;
using System.Collections.Generic;
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
        // Frame type SVG URLs from ArduPilot documentation
        private static readonly Dictionary<(int frameClass, int frameType), string> FrameSvgUrls = new Dictionary<(int, int), string>
        {
            // QUAD (Class 1)
            {(1, 0), "http://ardupilot.org/copter/_images/m_01_00_quad_plus.svg"},
            {(1, 1), "http://ardupilot.org/copter/_images/m_01_01_quad_x.svg"},
            {(1, 2), "http://ardupilot.org/copter/_images/m_01_02_quad_v.svg"},
            {(1, 3), "http://ardupilot.org/copter/_images/m_01_03_quad_h.svg"},
            {(1, 4), "http://ardupilot.org/copter/_images/m_01_04_quad_v_tail.svg"},
            {(1, 5), "http://ardupilot.org/copter/_images/m_01_05_quad_a_tail.svg"},
            {(1, 6), "http://ardupilot.org/copter/_images/m_01_06_quad_plus_rev.svg"},
            {(1, 12), "http://ardupilot.org/copter/_images/m_01_12_quad_x_bf.svg"},
            {(1, 13), "http://ardupilot.org/copter/_images/m_01_13_quad_x_dji.svg"},
            {(1, 14), "http://ardupilot.org/copter/_images/m_01_14_quad_x_cw.svg"},
            {(1, 16), "http://ardupilot.org/copter/_images/m_01_16_quad_plus_nyt.svg"},
            {(1, 17), "http://ardupilot.org/copter/_images/m_01_17_quad_x_nyt.svg"},
            {(1, 18), "http://ardupilot.org/copter/_images/m_01_18_quad_x_bf_rev.svg"},
            {(1, 19), "http://ardupilot.org/copter/_images/m_01_19_quad_y4a.svg"},
            // HEXA (Class 2)
            {(2, 0), "http://ardupilot.org/copter/_images/m_02_00_hexa_plus.svg"},
            {(2, 1), "http://ardupilot.org/copter/_images/m_02_01_hexa_x.svg"},
            {(2, 3), "http://ardupilot.org/copter/_images/m_02_03_hexa_h.svg"},
            {(2, 13), "http://ardupilot.org/copter/_images/m_02_13_hexa_x_dji.svg"},
            {(2, 14), "http://ardupilot.org/copter/_images/m_02_14_hexa_x_cw.svg"},
            // OCTA (Class 3)
            {(3, 0), "http://ardupilot.org/copter/_images/m_03_00_octo_plus.svg"},
            {(3, 1), "http://ardupilot.org/copter/_images/m_03_01_octo_x.svg"},
            {(3, 2), "http://ardupilot.org/copter/_images/m_03_02_octo_v.svg"},
            {(3, 3), "http://ardupilot.org/copter/_images/m_03_03_octo_h.svg"},
            {(3, 13), "http://ardupilot.org/copter/_images/m_03_13_octo_x_dji.svg"},
            {(3, 14), "http://ardupilot.org/copter/_images/m_03_14_octo_x_cw.svg"},
            {(3, 15), "http://ardupilot.org/copter/_images/m_03_15_octo_i.svg"},
            // OCTA QUAD (Class 4)
            {(4, 0), "http://ardupilot.org/copter/_images/m_04_00_octo_quad_plus.svg"},
            {(4, 1), "http://ardupilot.org/copter/_images/m_04_01_octo_quad_x.svg"},
            {(4, 2), "http://ardupilot.org/copter/_images/m_04_02_octo_quad_v.svg"},
            {(4, 3), "http://ardupilot.org/copter/_images/m_04_03_octo_quad_h.svg"},
            {(4, 12), "http://ardupilot.org/copter/_images/m_04_12_octo_quad_x_bf.svg"},
            {(4, 14), "http://ardupilot.org/copter/_images/m_04_14_octo_quad_x_cw.svg"},
            {(4, 18), "http://ardupilot.org/copter/_images/m_04_18_octo_quad_x_bf_rev.svg"},
            // Y6 (Class 5)
            {(5, 0), "http://ardupilot.org/copter/_images/m_05_00_y6_a.svg"},
            {(5, 10), "http://ardupilot.org/copter/_images/m_05_10_y6_b.svg"},
            {(5, 11), "http://ardupilot.org/copter/_images/m_05_11_y6_f.svg"},
            // TRI (Class 7)
            {(7, 0), "http://ardupilot.org/copter/_images/m_07_00_tricopter.svg"},
            {(7, 6), "http://ardupilot.org/copter/_images/m_07_06_tricopter_pitch_rev.svg"},
            // BICOPTER (Class 10)
            {(10, 0), "http://ardupilot.org/copter/_images/m_10_00_bicopter.svg"},
            // DODECAHEXA (Class 12)
            {(12, 0), "http://ardupilot.org/copter/_images/m_12_00_dodecahexa_plus.svg"},
            {(12, 1), "http://ardupilot.org/copter/_images/m_12_01_dodecahexa_x.svg"},
            // DECA (Class 14)
            {(14, 0), "http://ardupilot.org/copter/_images/m_14_00_deca_plus.svg"},
            {(14, 1), "http://ardupilot.org/copter/_images/m_14_01_deca_x_and_cw_x.svg"}
        };

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

        public async void Activate()
        {
            // Initialize WebView2
            try
            {
                await webViewFrame.EnsureCoreWebView2Async(null);
                webViewFrame.DefaultBackgroundColor = System.Drawing.Color.Transparent;
                webViewFrame.Visible = false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("WebView2 initialization error: " + ex.Message);
            }

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

                panel1.Controls.Add(but);

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
                            panel1.Controls.Add(lab);
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
            panel1.Controls.Add(but);

            y += 39;

            but = new MyButton();
            but.Text = "Stop all motors";
            but.Location = new Point(x, y);
            but.Size = new Size(75, 37);
            but.Click += but_StopAll;
            panel1.Controls.Add(but);

            y += 39;

            but = new MyButton();
            but.Text = "Test all in Sequence";
            but.Location = new Point(x, y);
            but.Size = new Size(75, 37);
            but.Click += but_TestAllSeq;
            panel1.Controls.Add(but);

            // Load MOT_SPIN_ARM and MOT_SPIN_MIN parameters
            // Temporarily remove event handlers to avoid triggering parameter writes during load
            NUM_mot_spin_arm.ValueChanged -= NUM_mot_spin_arm_ValueChanged;
            NUM_mot_spin_min.ValueChanged -= NUM_mot_spin_min_ValueChanged;

            if (MainV2.comPort.MAV.param.ContainsKey("MOT_SPIN_ARM"))
            {
                NUM_mot_spin_arm.Value = (decimal)MainV2.comPort.MAV.param["MOT_SPIN_ARM"].Value;
            }
            if (MainV2.comPort.MAV.param.ContainsKey("MOT_SPIN_MIN"))
            {
                NUM_mot_spin_min.Value = (decimal)MainV2.comPort.MAV.param["MOT_SPIN_MIN"].Value;
            }

            // Re-add event handlers
            NUM_mot_spin_arm.ValueChanged += NUM_mot_spin_arm_ValueChanged;
            NUM_mot_spin_min.ValueChanged += NUM_mot_spin_min_ValueChanged;

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
            UpdateFrameImage(frame_class, frame_type);

            return true;
        }

        private void UpdateFrameImage(int frame_class, int frame_type)
        {
            try
            {
                if (webViewFrame.CoreWebView2 == null)
                {
                    Debug.WriteLine("WebView2 CoreWebView2 not initialized");
                    return;
                }

                // Lookup SVG URL based on frame class and type
                if (FrameSvgUrls.TryGetValue((frame_class, frame_type), out string svgUrl))
                {
                    // Create HTML to display SVG with proper sizing
                    string html = $@"
                        <!DOCTYPE html>
                        <html>
                        <head>
                            <style>
                                body {{
                                    margin: 0;
                                    padding: 0;
                                    display: flex;
                                    align-items: center;
                                    justify-content: center;
                                    width: 400px;
                                    height: 400px;
                                    overflow: hidden;
                                }}
                                img {{
                                    max-width: 100%;
                                    max-height: 100%;
                                    object-fit: contain;
                                }}
                            </style>
                        </head>
                        <body>
                            <img src='{svgUrl}' alt='Frame diagram' />
                        </body>
                        </html>";

                    webViewFrame.CoreWebView2.NavigateToString(html);
                    webViewFrame.Visible = true;
                }
                else
                {
                    Debug.WriteLine($"No SVG found for frame class {frame_class}, type {frame_type}");
                    webViewFrame.Visible = false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error updating frame image: " + ex.Message);
                webViewFrame.Visible = false;
            }
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

        private async void NUM_mot_spin_arm_ValueChanged(object sender, EventArgs e)
        {
            if (!MainV2.comPort.MAV.param.ContainsKey("MOT_SPIN_ARM"))
            {
                return;
            }

            try
            {
                await MainV2.comPort.setParamAsync((byte)MainV2.comPort.sysidcurrent,
                    (byte)MainV2.comPort.compidcurrent, "MOT_SPIN_ARM",
                    (float)NUM_mot_spin_arm.Value).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Failed to set MOT_SPIN_ARM: " + ex.Message, Strings.ERROR);
            }
        }

        private async void NUM_mot_spin_min_ValueChanged(object sender, EventArgs e)
        {
            if (!MainV2.comPort.MAV.param.ContainsKey("MOT_SPIN_MIN"))
            {
                return;
            }

            try
            {
                await MainV2.comPort.setParamAsync((byte)MainV2.comPort.sysidcurrent,
                    (byte)MainV2.comPort.compidcurrent, "MOT_SPIN_MIN",
                    (float)NUM_mot_spin_min.Value).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Failed to set MOT_SPIN_MIN: " + ex.Message, Strings.ERROR);
            }
        }
    }
}