using MissionPlanner.Controls;
using MissionPlanner.Utilities;
using SharpDX.DirectInput;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;


namespace MissionPlanner.Joystick
{
    public partial class JoystickSetup : Form
    {
        bool startup = true;

        int noButtons = 0;
        private int maxaxis = 16;

        public JoystickSetup()
        {
            InitializeComponent();

            MissionPlanner.Utilities.Tracking.AddPage(this.GetType().ToString(), this.Text);
        }

        private void Joystick_Load(object sender, EventArgs e)
        {
            try
            {
                var joysticklist = Joystick.getDevices();

                foreach (DeviceInstance device in joysticklist)
                {
                    CMB_joysticks.Items.Add(device.ProductName.TrimUnPrintable());
                }
            }
            catch
            {
                CustomMessageBox.Show("Error geting joystick list: do you have the directx redist installed?");
                this.Close();
                return;
            }

            if (CMB_joysticks.Items.Count > 0 && CMB_joysticks.SelectedIndex == -1)
                CMB_joysticks.SelectedIndex = 0;

            try
            {
                if (Settings.Instance.ContainsKey("joystick_name") && Settings.Instance["joystick_name"].ToString() != "")
                    CMB_joysticks.Text = Settings.Instance["joystick_name"].ToString();
            }
            catch
            {
            }

            try
            {
                if (Settings.Instance.ContainsKey("joy_elevons"))
                    CHK_elevons.Checked = bool.Parse(Settings.Instance["joy_elevons"].ToString());
            }
            catch
            {
            } // IF 1 DOESNT EXIST NONE WILL

            var tempjoystick = new Joystick(() => MainV2.comPort);

            label14.Text += " " + MainV2.comPort.MAV.cs.firmware.ToString();

            var y = label8.Bottom;

            for (int a = 1; a <= maxaxis; a++)
            {
                var config = tempjoystick.getChannel(a);

                var ax = new JoystickAxis()
                {
                    ChannelNo = a,
                    Label = "Ch " + a,
                    AxisArray = (Enum.GetValues(typeof(Joystick.joystickaxis))),
                    ChannelValue = config.axis.ToString(),
                    ExpoValue = config.expo.ToString(),
                    ReverseValue = config.reverse,
                    Location = new Point(0, y),
                    Name = "axis" + a
                };

                ax.Detect = () => Joystick.getMovingAxis(CMB_joysticks.Text, 16000).ToString();
                ax.Reverse = () => MainV2.joystick.setReverse(ax.ChannelNo, ax.ReverseValue);
                ax.SetAxis = () => MainV2.joystick.setAxis(ax.ChannelNo,
                    (Joystick.joystickaxis)Enum.Parse(typeof(Joystick.joystickaxis), ax.ChannelValue));
                ax.GetValue = () =>
                {
                    return (short)MainV2.comPort.MAV.cs.GetType().GetField("rcoverridech" + ax.ChannelNo)
                        .GetValue(MainV2.comPort.MAV.cs);
                };

                Controls.Add(ax);

                y += ax.Height;


                if ((ax.Bottom + 30) > this.Height)
                    this.Height = ax.Bottom;

                if ((ax.Right) > this.Width)
                    this.Width = ax.Right;
            }

            if (MainV2.joystick != null && MainV2.joystick.enabled)
            {
                timer1.Start();
                BUT_enable.Text = "Disable";
            }

            startup = false;
        }

        int[] getButtonNumbers()
        {
            int[] temp = new int[128];
            temp[0] = -1;
            for (int a = 0; a < temp.Length - 1; a++)
            {
                temp[a + 1] = a;
            }
            return temp;
        }

        private void BUT_enable_Click(object sender, EventArgs e)
        {
            if (MainV2.joystick == null || MainV2.joystick.enabled == false)
            {
                try
                {
                    if (MainV2.joystick != null)
                        MainV2.joystick.UnAcquireJoyStick();
                }
                catch
                {
                }

                // all config is loaded from the xmls
                Joystick joy = new Joystick(() => MainV2.comPort);

                joy.elevons = CHK_elevons.Checked;

                //show error message if a joystick is not connected when Enable is clicked
                if (!joy.start(CMB_joysticks.Text))
                {
                    CustomMessageBox.Show("Please Connect a Joystick", "No Joystick");
                    joy.Dispose();
                    return;
                }

                Settings.Instance["joystick_name"] = CMB_joysticks.Text;

                MainV2.joystick = joy;
                MainV2.joystick.enabled = true;

                BUT_enable.Text = "Disable";

                //timer1.Start();
            }
            else
            {
                MainV2.joystick.enabled = false;

                MainV2.joystick.clearRCOverride();

                MainV2.joystick = null;


                //timer1.Stop();

                BUT_enable.Text = "Enable";
            }
        }

        private void BUT_save_Click(object sender, EventArgs e)
        {
            if (MainV2.joystick == null)
            {
                CustomMessageBox.Show("Please select a joystick");
                return;
            }
            MainV2.joystick.saveconfig();

            Settings.Instance["joy_elevons"] = CHK_elevons.Checked.ToString();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (MainV2.joystick == null || MainV2.joystick.enabled == false)
                {
                    //Console.WriteLine(DateTime.Now.Millisecond + " start ");
                    Joystick joy = MainV2.joystick;
                    if (joy == null)
                    {
                        joy = new Joystick(() => MainV2.comPort);
                        for (int a = 1; a <= maxaxis; a++)
                        {
                            var config = joy.getChannel(a);

                            joy.setChannel(a, config.axis, config.reverse, config.expo);
                        }

                        joy.elevons = CHK_elevons.Checked;

                        joy.AcquireJoystick(CMB_joysticks.Text);

                        joy.name = CMB_joysticks.Text;

                        noButtons = joy.getNumButtons();

                        noButtons = Math.Min(16, noButtons);

                        SuspendLayout();

                        MainV2.joystick = joy;

                        var maxctl = Controls.Find("axis" + 1, false).FirstOrDefault();

                        for (int f = 0; f < noButtons; f++)
                        {
                            string name = (f).ToString();

                            doButtontoUI(name, maxctl.Right + 100, maxctl.Top + f * maxctl.Height);

                            var config = joy.getButton(f);

                            joy.setButton(f, config);
                        }

                        ResumeLayout();

                        ThemeManager.ApplyThemeTo(this);

                        CMB_joysticks.SelectedIndex = CMB_joysticks.Items.IndexOf(joy.name);
                    }

                    MainV2.joystick.elevons = CHK_elevons.Checked;

                    MainV2.comPort.MAV.cs.rcoverridech1 = joy.getValueForChannel(1, CMB_joysticks.Text);
                    MainV2.comPort.MAV.cs.rcoverridech2 = joy.getValueForChannel(2, CMB_joysticks.Text);
                    MainV2.comPort.MAV.cs.rcoverridech3 = joy.getValueForChannel(3, CMB_joysticks.Text);
                    MainV2.comPort.MAV.cs.rcoverridech4 = joy.getValueForChannel(4, CMB_joysticks.Text);
                    MainV2.comPort.MAV.cs.rcoverridech5 = joy.getValueForChannel(5, CMB_joysticks.Text);
                    MainV2.comPort.MAV.cs.rcoverridech6 = joy.getValueForChannel(6, CMB_joysticks.Text);
                    MainV2.comPort.MAV.cs.rcoverridech7 = joy.getValueForChannel(7, CMB_joysticks.Text);
                    MainV2.comPort.MAV.cs.rcoverridech8 = joy.getValueForChannel(8, CMB_joysticks.Text);
                    MainV2.comPort.MAV.cs.rcoverridech9 = joy.getValueForChannel(9, CMB_joysticks.Text);
                    MainV2.comPort.MAV.cs.rcoverridech10 = joy.getValueForChannel(10, CMB_joysticks.Text);
                    MainV2.comPort.MAV.cs.rcoverridech11 = joy.getValueForChannel(11, CMB_joysticks.Text);
                    MainV2.comPort.MAV.cs.rcoverridech12 = joy.getValueForChannel(12, CMB_joysticks.Text);
                    MainV2.comPort.MAV.cs.rcoverridech13 = joy.getValueForChannel(13, CMB_joysticks.Text);
                    MainV2.comPort.MAV.cs.rcoverridech14 = joy.getValueForChannel(14, CMB_joysticks.Text);
                    MainV2.comPort.MAV.cs.rcoverridech15 = joy.getValueForChannel(15, CMB_joysticks.Text);
                    MainV2.comPort.MAV.cs.rcoverridech16 = joy.getValueForChannel(16, CMB_joysticks.Text);
                    MainV2.comPort.MAV.cs.rcoverridech17 = joy.getValueForChannel(17, CMB_joysticks.Text);
                    MainV2.comPort.MAV.cs.rcoverridech18 = joy.getValueForChannel(18, CMB_joysticks.Text);

                    //Console.WriteLine(DateTime.Now.Millisecond + " end ");
                }
            }
            catch (SharpDX.SharpDXException ex)
            {
                ex.ToString();
                if (MainV2.joystick != null && MainV2.joystick.enabled == true)
                {
                    BUT_enable_Click(null, null);
                }

                if (ex.Message.Contains("DIERR_NOTACQUIRED"))
                    MainV2.joystick = null;
            }
            catch
            {
            }

            try
            {
                for (int f = 0; f < noButtons; f++)
                {
                    string name = (f).ToString();

                    var items = this.Controls.Find("hbar" + name, false);

                    if (items.Length > 0)
                        ((HorizontalProgressBar)items[0]).Value =
                            MainV2.joystick.isButtonPressed(f) ? 100 : 0;
                }
            }
            catch
            {
            } // this is for buttons - silent fail
        }

        private void CMB_joysticks_Click(object sender, EventArgs e)
        {
            CMB_joysticks.Items.Clear();

            var joysticklist = Joystick.getDevices();

            foreach (DeviceInstance device in joysticklist)
            {
                CMB_joysticks.Items.Add(device.ProductName.TrimUnPrintable());
            }

            if (CMB_joysticks.Items.Count > 0 && CMB_joysticks.SelectedIndex == -1)
                CMB_joysticks.SelectedIndex = 0;
        }

        private void cmbbutton_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (startup)
                return;

            string name = ((ComboBox)sender).Name.Replace("cmbbutton", "");

            MainV2.joystick.changeButton((int.Parse(name)), int.Parse(((ComboBox)sender).Text));
        }

        private void BUT_detbutton_Click(object sender, EventArgs e)
        {
            string name = ((MyButton)sender).Name.Replace("mybut", "");

            ComboBox cmb = (ComboBox)(this.Controls.Find("cmbbutton" + name, false)[0]);
            cmb.Text = Joystick.getPressedButton(CMB_joysticks.Text).ToString();
        }

        void doButtontoUI(string name, int x, int y)
        {
            MyLabel butlabel = new MyLabel();
            ComboBox butnumberlist = new ComboBox();
            Controls.MyButton but_detect = new Controls.MyButton();
            HorizontalProgressBar hbar = new HorizontalProgressBar();
            ComboBox cmbaction = new ComboBox();
            Controls.MyButton but_settings = new Controls.MyButton();

            if (MainV2.joystick == null)
            {
                butlabel.Dispose();
                butnumberlist.Dispose();
                but_detect.Dispose();
                hbar.Dispose();
                cmbaction.Dispose();
                but_settings.Dispose();
                return;
            }

            var config = MainV2.joystick.getButton(int.Parse(name));


            butlabel.Location = new Point(x, y);
            butlabel.Size = new Size(47, 13);
            butlabel.Text = "But " + (int.Parse(name) + 1);

            butnumberlist.Location = new Point(butlabel.Right, y);
            butnumberlist.Size = new Size(70, 21);
            butnumberlist.DataSource = getButtonNumbers();
            butnumberlist.DropDownStyle = ComboBoxStyle.DropDownList;
            butnumberlist.Name = "cmbbutton" + name;
            //if (Settings.Instance["butno" + name] != null)
            //  butnumberlist.Text = (Settings.Instance["butno" + name].ToString());
            //if (config.buttonno != -1)
            butnumberlist.Text = config.buttonno.ToString();
            butnumberlist.SelectedIndexChanged += new EventHandler(cmbbutton_SelectedIndexChanged);

            but_detect.Location = new Point(butnumberlist.Right, y);
            //but_detect.Size = BUT_detch1.Size;
            but_detect.Text = "Detect";
            but_detect.AutoSize = true;

            but_detect.Name = "mybut" + name;
            but_detect.Click += new EventHandler(BUT_detbutton_Click);

            hbar.Location = new Point(but_detect.Right, y);
            hbar.Size = new Size(100, 21);
            hbar.Name = "hbar" + name;

            cmbaction.Location = new Point(hbar.Right + 5, y);
            cmbaction.Size = new Size(100, 21);

            cmbaction.DataSource = Enum.GetNames(typeof(Joystick.buttonfunction));
            //Common.getModesList(MainV2.comPort.MAV.cs);
            //cmbaction.ValueMember = "Key";
            //cmbaction.DisplayMember = "Value";
            cmbaction.Tag = name;
            cmbaction.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbaction.Name = "cmbaction" + name;
            //if (Settings.Instance["butaction" + name] != null)
            //  cmbaction.Text = Settings.Instance["butaction" + name].ToString();
            //if (config.function != Joystick.buttonfunction.ChangeMode)
            cmbaction.Text = config.function.ToString();
            cmbaction.SelectedIndexChanged += cmbaction_SelectedIndexChanged;

            but_settings.Location = new Point(cmbaction.Right + 5, y);
            //but_settings.Size = BUT_detch1.Size;
            but_settings.Text = "Settings";
            but_settings.Name = "butsettings" + name;
            but_settings.Click += but_settings_Click;
            but_settings.Tag = cmbaction;

            // do this here so putting in text works
            this.Controls.AddRange(new Control[] { butlabel, butnumberlist, but_detect, hbar, cmbaction, but_settings });

            if ((but_settings.Bottom + 30) > this.Height)
                this.Height += 25;

            if ((but_settings.Right) > this.Width)
                this.Width = but_settings.Right + 5;
        }

        void cmbaction_SelectedIndexChanged(object sender, EventArgs e)
        {
            int num = int.Parse(((Control)sender).Tag.ToString());
            var config = MainV2.joystick.getButton(num);
            config.function =
                (Joystick.buttonfunction)Enum.Parse(typeof(Joystick.buttonfunction), ((Control)sender).Text);
            MainV2.joystick.setButton(num, config);
        }

        void but_settings_Click(object sender, EventArgs e)
        {
            var cmb = ((Control)sender).Tag as ComboBox;

            switch ((Joystick.buttonfunction)Enum.Parse(typeof(Joystick.buttonfunction), cmb.SelectedItem.ToString()))
            {
                case Joystick.buttonfunction.ChangeMode:
                    new Joy_ChangeMode((string)cmb.Tag).ShowDialog();
                    break;
                case Joystick.buttonfunction.Mount_Mode:
                    new Joy_Mount_Mode((string)cmb.Tag).ShowDialog();
                    break;
                case Joystick.buttonfunction.Do_Repeat_Relay:
                    new Joy_Do_Repeat_Relay((string)cmb.Tag).ShowDialog();
                    break;
                case Joystick.buttonfunction.Do_Repeat_Servo:
                    new Joy_Do_Repeat_Servo((string)cmb.Tag).ShowDialog();
                    break;
                case Joystick.buttonfunction.Do_Set_Relay:
                    new Joy_Do_Set_Relay((string)cmb.Tag).ShowDialog();
                    break;
                case Joystick.buttonfunction.Do_Set_Servo:
                    new Joy_Do_Set_Servo((string)cmb.Tag).ShowDialog();
                    break;
                case Joystick.buttonfunction.Button_axis0:
                    new Joy_Button_axis((string)cmb.Tag).ShowDialog();
                    break;
                case Joystick.buttonfunction.Button_axis1:
                    new Joy_Button_axis((string)cmb.Tag).ShowDialog();
                    break;
                default:
                    CustomMessageBox.Show("No settings to set", "No settings");
                    break;
            }
        }

        private void CMB_joysticks_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (MainV2.joystick != null && MainV2.joystick.enabled == false)
                    MainV2.joystick.UnAcquireJoyStick();
            }
            catch
            {
            }
        }

        private void JoystickSetup_FormClosed(object sender, FormClosedEventArgs e)
        {
            timer1.Stop();

            if (MainV2.joystick != null && MainV2.joystick.enabled == false)
            {
                MainV2.joystick.UnAcquireJoyStick();
                MainV2.joystick = null;
            }
        }

        private void CHK_elevons_CheckedChanged(object sender, EventArgs e)
        {
            if (MainV2.joystick == null)
            {
                return;
            }
            MainV2.joystick.elevons = CHK_elevons.Checked;
        }

        private void chk_manualcontrol_CheckedChanged(object sender, EventArgs e)
        {
            MainV2.joystick.manual_control = chk_manualcontrol.Checked;
        }
    }
}