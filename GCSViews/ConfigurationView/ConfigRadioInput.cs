using MissionPlanner.ArduPilot;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigRadioInput : MyUserControl, IActivate, IDeactivate
    {
        private readonly float[] rcmax = new float[16];
        private readonly float[] rcmin = new float[16];
        private readonly float[] rctrim = new float[16];
        private readonly Timer _timer = new Timer();
        private int chpitch = -1;
        private int chroll = -1;
        private int chthro = -1;
        private int chyaw = -1;
        private bool run;
        private bool startup;

        public ConfigRadioInput()
        {
            InitializeComponent();

            // setup rc calib extents
            for (var a = 0; a < rcmin.Length; a++)
            {
                rcmin[a] = 3000;
                rcmax[a] = 0;
                rctrim[a] = 1500;
            }


            // setup rc update
            _timer.Tick += timer_Tick;
        }

        public void Activate()
        {
            _timer.Enabled = true;
            _timer.Interval = 100;
            _timer.Start();

            LoadChannelMapping();
            SetupBarDataBindings();
            SetupBarLabels();
            SetupStickLabels();
            RequestRCDataStream();
            SetupElevonControls();
            SetupReverseCheckboxes();

            // Hide reversal checkboxes for main channels on copter (reversing would make it uncontrollable)
            if (MainV2.comPort.MAV.cs.firmware == Firmwares.ArduCopter2)
            {
                CHK_revroll.Visible = false;
                CHK_revpitch.Visible = false;
                CHK_revthr.Visible = false;
                CHK_revyaw.Visible = false;
            }

            startup = false;
        }

        private void LoadChannelMapping()
        {
            if (!MainV2.comPort.MAV.param.ContainsKey("RCMAP_ROLL"))
            {
                chroll = 1;
                chpitch = 2;
                chthro = 3;
                chyaw = 4;
            }
            else
            {
                try
                {
                    chroll = (int)(float)MainV2.comPort.MAV.param["RCMAP_ROLL"];
                    chpitch = (int)(float)MainV2.comPort.MAV.param["RCMAP_PITCH"];
                    chthro = (int)(float)MainV2.comPort.MAV.param["RCMAP_THROTTLE"];
                    chyaw = (int)(float)MainV2.comPort.MAV.param["RCMAP_YAW"];
                }
                catch (Exception)
                {
                    CustomMessageBox.Show(Strings.ErrorReceivingParams, Strings.ERROR);
                    this.Enabled = false;
                }
            }
        }

        private void SetupBarDataBindings()
        {
            var bars = new[] { BARroll, BARpitch, BARthrottle, BARyaw, BAR5, BAR6, BAR7, BAR8, BAR9, BAR10, BAR11, BAR12, BAR13, BAR14, BAR15, BAR16 };

            foreach (var bar in bars)
            {
                bar.DataBindings.Clear();
            }

            BARroll.DataBindings.Add(new Binding("Value", currentStateBindingSource, "ch" + chroll + "in", true));
            BARpitch.DataBindings.Add(new Binding("Value", currentStateBindingSource, "ch" + chpitch + "in", true));
            BARthrottle.DataBindings.Add(new Binding("Value", currentStateBindingSource, "ch" + chthro + "in", true));
            BARyaw.DataBindings.Add(new Binding("Value", currentStateBindingSource, "ch" + chyaw + "in", true));

            for (int i = 5; i <= 16; i++)
            {
                var bar = (HorizontalProgressBar2)this.GetType().GetField("BAR" + i, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(this);
                bar.DataBindings.Add(new Binding("Value", currentStateBindingSource, "ch" + i + "in", true));
            }
        }

        private void SetupBarLabels()
        {
            BARroll.Label += " (rc" + chroll + ")";
            BARpitch.Label += " (rc" + chpitch + ")";
            BARthrottle.Label += " (rc" + chthro + ")";
            BARyaw.Label += " (rc" + chyaw + ")";
        }

        private void SetupStickLabels()
        {
            stickLeft.HorizontalLabel = "Yaw (rc" + chyaw + ")";
            stickLeft.VerticalLabel = "Throttle (rc" + chthro + ")";
            stickRight.HorizontalLabel = "Roll (rc" + chroll + ")";
            stickRight.VerticalLabel = "Pitch (rc" + chpitch + ")";
        }

        private void RequestRCDataStream()
        {
            try
            {
                MainV2.comPort.requestDatastream(MAVLink.MAV_DATA_STREAM.RC_CHANNELS, 2);
            }
            catch
            {
            }
        }

        private void SetupElevonControls()
        {
            startup = true;

            if (MainV2.comPort.MAV.cs.firmware == Firmwares.ArduPlane ||
                MainV2.comPort.MAV.cs.firmware == Firmwares.Ateryx)
            {
                CHK_mixmode.setup(1, 0, "ELEVON_MIXING", MainV2.comPort.MAV.param);
                CHK_elevonrev.setup(1, 0, "ELEVON_REVERSE", MainV2.comPort.MAV.param);
                CHK_elevonch1rev.setup(1, 0, "ELEVON_CH1_REV", MainV2.comPort.MAV.param);
                CHK_elevonch2rev.setup(1, 0, "ELEVON_CH2_REV", MainV2.comPort.MAV.param);
            }
            else
            {
                groupBoxElevons.Visible = false;
            }
        }

        private void SetupReverseCheckboxes()
        {
            // Setup reverse checkboxes for main control channels
            SetupReverseCheckbox(CHK_revroll, chroll);
            SetupReverseCheckbox(CHK_revpitch, chpitch);
            SetupReverseCheckbox(CHK_revthr, chthro);
            SetupReverseCheckbox(CHK_revyaw, chyaw);

            // Setup reverse checkboxes for channels 5-16
            var reverseCheckboxes = new[] { CHK_rev5, CHK_rev6, CHK_rev7, CHK_rev8, CHK_rev9, CHK_rev10, CHK_rev11, CHK_rev12, CHK_rev13, CHK_rev14, CHK_rev15, CHK_rev16 };
            for (int i = 0; i < reverseCheckboxes.Length; i++)
            {
                SetupReverseCheckbox(reverseCheckboxes[i], i + 5);
            }
        }

        private void SetupReverseCheckbox(MavlinkCheckBox checkbox, int channel)
        {
            checkbox.setup(
                new double[] { -1, 1 },
                new double[] { 1, 0 },
                new string[] { "RC" + channel + "_REV", "RC" + channel + "_REVERSED" },
                MainV2.comPort.MAV.param);
        }

        public void Deactivate()
        {
            _timer.Stop();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            // update all linked controls - 10hz
            try
            {
                MainV2.comPort.MAV.cs.UpdateCurrentSettings(currentStateBindingSource.UpdateDataSource(MainV2.comPort.MAV.cs));

                // Update stick controls with current values
                if (chyaw > 0 && chthro > 0 && chroll > 0 && chpitch > 0)
                {
                    stickLeft.HorizontalValue = GetChannelValue(chyaw);
                    stickLeft.VerticalValue = GetChannelValue(chthro);

                    stickRight.HorizontalValue = GetChannelValue(chroll);
                    stickRight.VerticalValue = GetChannelValue(chpitch);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private int GetChannelValue(int channel)
        {
            switch (channel)
            {
                case 1: return (int)MainV2.comPort.MAV.cs.ch1in;
                case 2: return (int)MainV2.comPort.MAV.cs.ch2in;
                case 3: return (int)MainV2.comPort.MAV.cs.ch3in;
                case 4: return (int)MainV2.comPort.MAV.cs.ch4in;
                case 5: return (int)MainV2.comPort.MAV.cs.ch5in;
                case 6: return (int)MainV2.comPort.MAV.cs.ch6in;
                case 7: return (int)MainV2.comPort.MAV.cs.ch7in;
                case 8: return (int)MainV2.comPort.MAV.cs.ch8in;
                case 9: return (int)MainV2.comPort.MAV.cs.ch9in;
                case 10: return (int)MainV2.comPort.MAV.cs.ch10in;
                case 11: return (int)MainV2.comPort.MAV.cs.ch11in;
                case 12: return (int)MainV2.comPort.MAV.cs.ch12in;
                case 13: return (int)MainV2.comPort.MAV.cs.ch13in;
                case 14: return (int)MainV2.comPort.MAV.cs.ch14in;
                case 15: return (int)MainV2.comPort.MAV.cs.ch15in;
                case 16: return (int)MainV2.comPort.MAV.cs.ch16in;
                default: return 1500;
            }
        }

        private void UpdateChannelMinMax()
        {
            for (int i = 1; i <= 16; i++)
            {
                float channelValue = GetChannelValue(i);
                rcmin[i - 1] = Math.Min(rcmin[i - 1], channelValue);
                rcmax[i - 1] = Math.Max(rcmax[i - 1], channelValue);
            }
        }

        private void UpdateCalibrationBars()
        {
            BARroll.minline = (int)rcmin[chroll - 1];
            BARroll.maxline = (int)rcmax[chroll - 1];
            BARpitch.minline = (int)rcmin[chpitch - 1];
            BARpitch.maxline = (int)rcmax[chpitch - 1];
            BARthrottle.minline = (int)rcmin[chthro - 1];
            BARthrottle.maxline = (int)rcmax[chthro - 1];
            BARyaw.minline = (int)rcmin[chyaw - 1];
            BARyaw.maxline = (int)rcmax[chyaw - 1];

            setBARStatus(BAR5, rcmin[4], rcmax[4]);
            setBARStatus(BAR6, rcmin[5], rcmax[5]);
            setBARStatus(BAR7, rcmin[6], rcmax[6]);
            setBARStatus(BAR8, rcmin[7], rcmax[7]);
            setBARStatus(BAR9, rcmin[8], rcmax[8]);
            setBARStatus(BAR10, rcmin[9], rcmax[9]);
            setBARStatus(BAR11, rcmin[10], rcmax[10]);
            setBARStatus(BAR12, rcmin[11], rcmax[11]);
            setBARStatus(BAR13, rcmin[12], rcmax[12]);
            setBARStatus(BAR14, rcmin[13], rcmax[13]);
            setBARStatus(BAR15, rcmin[14], rcmax[14]);
            setBARStatus(BAR16, rcmin[15], rcmax[15]);
        }

        private void UpdateStickCalibrationLines()
        {
            stickLeft.HorizontalMinLine = (int)rcmin[chyaw - 1];
            stickLeft.HorizontalMaxLine = (int)rcmax[chyaw - 1];
            stickLeft.VerticalMinLine = (int)rcmin[chthro - 1];
            stickLeft.VerticalMaxLine = (int)rcmax[chthro - 1];

            stickRight.HorizontalMinLine = (int)rcmin[chroll - 1];
            stickRight.HorizontalMaxLine = (int)rcmax[chroll - 1];
            stickRight.VerticalMinLine = (int)rcmin[chpitch - 1];
            stickRight.VerticalMaxLine = (int)rcmax[chpitch - 1];
        }

        private void UpdateStickPositions()
        {
            stickLeft.HorizontalValue = GetChannelValue(chyaw);
            stickLeft.VerticalValue = GetChannelValue(chthro);
            stickRight.HorizontalValue = GetChannelValue(chroll);
            stickRight.VerticalValue = GetChannelValue(chpitch);
        }

        private void CaptureTrimValues()
        {
            for (int i = 1; i <= 16; i++)
            {
                rctrim[i - 1] = Constrain(GetChannelValue(i), i - 1);
            }
        }

        private void BUT_Calibrateradio_Click(object sender, EventArgs e)
        {
            if (run)
            {
                BUT_Calibrateradio.Text = Strings.Completed;
                run = false;
                return;
            }

            // Combined dialog - merge the two calibration setup messages
            CustomMessageBox.Show(
                "Radio Calibration Setup:\n\n" +
                "1. Ensure your transmitter is on and receiver is powered and connected\n" +
                "2. Ensure your motor does not have power/no props!!!\n\n" +
                "After clicking OK, move all RC sticks and switches to their\nextreme positions so the red bars hit the limits.\n\n" +
                "Click 'Click when Done' button when finished moving sticks.");

            var oldrc = MainV2.comPort.MAV.cs.raterc;
            var oldatt = MainV2.comPort.MAV.cs.rateattitude;
            var oldpos = MainV2.comPort.MAV.cs.rateposition;
            var oldstatus = MainV2.comPort.MAV.cs.ratestatus;

            MainV2.comPort.MAV.cs.raterc = 10;
            MainV2.comPort.MAV.cs.rateattitude = 0;
            MainV2.comPort.MAV.cs.rateposition = 0;
            MainV2.comPort.MAV.cs.ratestatus = 0;

            try
            {
                MainV2.comPort.requestDatastream(MAVLink.MAV_DATA_STREAM.RC_CHANNELS, 10);
            }
            catch
            {
            }

            BUT_Calibrateradio.Text = Strings.Click_when_Done;

            run = true;


            while (run)
            {
                Application.DoEvents();

                Thread.Sleep(5);

                MainV2.comPort.MAV.cs.UpdateCurrentSettings(currentStateBindingSource.UpdateDataSource(MainV2.comPort.MAV.cs), true, MainV2.comPort);

                // check for non 0 values
                if (MainV2.comPort.MAV.cs.ch1in > 800 && MainV2.comPort.MAV.cs.ch1in < 2200)
                {
                    UpdateChannelMinMax();
                    UpdateCalibrationBars();
                    UpdateStickCalibrationLines();
                    UpdateStickPositions();
                }
            }

            if (rcmin[0] > 800 && rcmin[0] < 2200)
            {
            }
            else
            {
                CustomMessageBox.Show("Bad channel 1 input, canceling");
                return;
            }

            CustomMessageBox.Show("Ensure all your sticks are centered and throttle is down, and click ok to continue");

            MainV2.comPort.MAV.cs.UpdateCurrentSettings(currentStateBindingSource.UpdateDataSource(MainV2.comPort.MAV.cs), true, MainV2.comPort);

            CaptureTrimValues();

            var data = "---------------\n";

            for (var a = 0; a < rctrim.Length; a++)
            {
                // we want these to save no matter what
                BUT_Calibrateradio.Text = Strings.Saving;
                try
                {
                    // min < max and min/max != 0
                    // trim < max && trim >= min && trim != 0 && min != max
                    if (rcmin[a] < rcmax[a] && rcmin[a] != 0 && rcmax[a] != 0 &&
                        rctrim[a] <= rcmax[a] && rctrim[a] >= rcmin[a] && rctrim[a] != 0 &&
                        rcmin[a] != rcmax[a])
                    {
                        MainV2.comPort.setParam((byte) MainV2.comPort.sysidcurrent, (byte) MainV2.comPort.compidcurrent,
                            "RC" + (a + 1).ToString("0") + "_MIN", rcmin[a], true);
                        MainV2.comPort.setParam((byte) MainV2.comPort.sysidcurrent, (byte) MainV2.comPort.compidcurrent,
                            "RC" + (a + 1).ToString("0") + "_MAX", rcmax[a], true);
                        MainV2.comPort.setParam((byte) MainV2.comPort.sysidcurrent, (byte) MainV2.comPort.compidcurrent,
                            "RC" + (a + 1).ToString("0") + "_TRIM", rctrim[a], true);
                    }
                    else
                    {
                        continue;
                    }
                }
                catch
                {
                    if (MainV2.comPort.MAV.param.ContainsKey("RC" + (a + 1).ToString("0") + "_MIN"))
                        CustomMessageBox.Show("Failed to set Channel " + (a + 1));
                }

                data = data + "CH" + (a + 1) + " " + rcmin[a] + " | " + rcmax[a] + "\n";
            }

            MainV2.comPort.MAV.cs.raterc = oldrc;
            MainV2.comPort.MAV.cs.rateattitude = oldatt;
            MainV2.comPort.MAV.cs.rateposition = oldpos;
            MainV2.comPort.MAV.cs.ratestatus = oldstatus;

            try
            {
                MainV2.comPort.requestDatastream(MAVLink.MAV_DATA_STREAM.RC_CHANNELS, oldrc);
            }
            catch
            {
            }

            CustomMessageBox.Show(
                "Here are the detected radio options\nNOTE Channels not connected are displayed as 1500 +-2\nNormal values are around 1100 | 1900\nChannel:Min | Max \n" +
                data, "Radio");

            BUT_Calibrateradio.Text = Strings.Completed;
        }

        private float Constrain(float chin, int v)
        {
            return Math.Min(Math.Max(chin, rcmin[v]), rcmax[v]);
        }

        private void setBARStatus(HorizontalProgressBar2 bar, float min, float max)
        {
            bar.minline = (int)min;
            bar.maxline = (int)max;
        }

        private void BUT_Bindradiodsm2_Click(object sender, EventArgs e)
        {
            BindRadio(0);
        }

        private void BUT_BindradiodsmX_Click(object sender, EventArgs e)
        {
            BindRadio(1);
        }

        private void BUT_Bindradiodsm8_Click(object sender, EventArgs e)
        {
            BindRadio(2);
        }

        private void BindRadio(int mode)
        {
            try
            {
                MainV2.comPort.doCommand((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, MAVLink.MAV_CMD.START_RX_PAIR, 0, mode, 0, 0, 0, 0, 0);
                CustomMessageBox.Show(Strings.Put_the_transmitter_in_bind_mode__Receiver_is_waiting);
            }
            catch
            {
                CustomMessageBox.Show(Strings.Error_binding);
            }
        }
    }
}