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
                    //setup bindings
                    chroll = (int)(float)MainV2.comPort.MAV.param["RCMAP_ROLL"];
                    chpitch = (int)(float)MainV2.comPort.MAV.param["RCMAP_PITCH"];
                    chthro = (int)(float)MainV2.comPort.MAV.param["RCMAP_THROTTLE"];
                    chyaw = (int)(float)MainV2.comPort.MAV.param["RCMAP_YAW"];
                }
                catch (Exception)
                {
                    CustomMessageBox.Show(Strings.ErrorReceivingParams, Strings.ERROR);
                    this.Enabled = false;
                    return;
                }
            }

            BARroll.DataBindings.Clear();
            BARpitch.DataBindings.Clear();
            BARthrottle.DataBindings.Clear();
            BARyaw.DataBindings.Clear();
            BAR5.DataBindings.Clear();
            BAR6.DataBindings.Clear();
            BAR7.DataBindings.Clear();
            BAR8.DataBindings.Clear();
            BAR9.DataBindings.Clear();
            BAR10.DataBindings.Clear();
            BAR11.DataBindings.Clear();
            BAR12.DataBindings.Clear();
            BAR13.DataBindings.Clear();
            BAR14.DataBindings.Clear();
            BAR15.DataBindings.Clear();
            BAR16.DataBindings.Clear();

            BARroll.DataBindings.Add(new Binding("Value", currentStateBindingSource, "ch" + chroll + "in", true));
            BARpitch.DataBindings.Add(new Binding("Value", currentStateBindingSource, "ch" + chpitch + "in", true));
            BARthrottle.DataBindings.Add(new Binding("Value", currentStateBindingSource, "ch" + chthro + "in", true));
            BARyaw.DataBindings.Add(new Binding("Value", currentStateBindingSource, "ch" + chyaw + "in", true));

            BAR5.DataBindings.Add(new Binding("Value", currentStateBindingSource, "ch5in", true));
            BAR6.DataBindings.Add(new Binding("Value", currentStateBindingSource, "ch6in", true));
            BAR7.DataBindings.Add(new Binding("Value", currentStateBindingSource, "ch7in", true));
            BAR8.DataBindings.Add(new Binding("Value", currentStateBindingSource, "ch8in", true));

            BAR9.DataBindings.Add(new Binding("Value", currentStateBindingSource, "ch9in", true));
            BAR10.DataBindings.Add(new Binding("Value", currentStateBindingSource, "ch10in", true));
            BAR11.DataBindings.Add(new Binding("Value", currentStateBindingSource, "ch11in", true));
            BAR12.DataBindings.Add(new Binding("Value", currentStateBindingSource, "ch12in", true));
            BAR13.DataBindings.Add(new Binding("Value", currentStateBindingSource, "ch13in", true));
            BAR14.DataBindings.Add(new Binding("Value", currentStateBindingSource, "ch14in", true));
            BAR15.DataBindings.Add(new Binding("Value", currentStateBindingSource, "ch15in", true));
            BAR16.DataBindings.Add(new Binding("Value", currentStateBindingSource, "ch16in", true));

            //Add channel to pitch/roll/throttle/yaw bars labels
            BARroll.Label = BARroll.Label + " (rc" + chroll.ToString() + ")";
            BARpitch.Label = BARpitch.Label + " (rc" + chpitch.ToString() + ")";
            BARthrottle.Label = BARthrottle.Label + " (rc" + chthro.ToString() + ")";
            BARyaw.Label = BARyaw.Label + " (rc" + chyaw.ToString() + ")";

            try
            {
                // force this screen to work
                MainV2.comPort.requestDatastream(MAVLink.MAV_DATA_STREAM.RC_CHANNELS, 2);
            }
            catch
            {
            }

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

            // this controls the direction of the output, not the input.
            CHK_revroll.setup(new double[] { -1, 1 }, new double[] { 1, 0 }, new string[] { "RC" + chroll + "_REV", "RC" + chroll + "_REVERSED" },
                MainV2.comPort.MAV.param);
            CHK_revpitch.setup(new double[] { -1, 1 }, new double[] { 1, 0 }, new string[] { "RC" + chpitch + "_REV", "RC" + chpitch + "_REVERSED" },
                MainV2.comPort.MAV.param);
            CHK_revthr.setup(new double[] { -1, 1 }, new double[] { 1, 0 }, new string[] { "RC" + chthro + "_REV", "RC" + chthro + "_REVERSED" },
                MainV2.comPort.MAV.param);
            CHK_revyaw.setup(new double[] { -1, 1 }, new double[] { 1, 0 }, new string[] { "RC" + chyaw + "_REV", "RC" + chyaw + "_REVERSED" },
                MainV2.comPort.MAV.param);

            if (MainV2.comPort.MAV.param["RC"+ chroll + "_REVERSED"]?.Value == 1)
            {
                reverseChannel(true, BARroll);
            }
            if (MainV2.comPort.MAV.param["RC" + chpitch + "_REVERSED"]?.Value == 1)
            {
                reverseChannel(true, BARpitch);
            }
            if (MainV2.comPort.MAV.param["RC" + chthro + "_REVERSED"]?.Value == 1)
            {
                reverseChannel(true, BARthrottle);
            }
            if (MainV2.comPort.MAV.param["RC" + chyaw + "_REVERSED"]?.Value == 1)
            {
                reverseChannel(true, BARyaw);
            }


            // run after to ensure they are disabled on copter
            if (MainV2.comPort.MAV.cs.firmware == Firmwares.ArduCopter2)
            {
                CHK_revroll.Visible = false;
                CHK_revpitch.Visible = false;
                CHK_revthr.Visible = false;
                CHK_revyaw.Visible = false;
            }

            startup = false;
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
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
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

            CustomMessageBox.Show(
                "Ensure your transmitter is on and receiver is powered and connected\nEnsure your motor does not have power/no props!!!");

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

            CustomMessageBox.Show(
                "Click OK and move all RC sticks and switches to their\nextreme positions so the red bars hit the limits.");

            run = true;


            while (run)
            {
                Application.DoEvents();

                Thread.Sleep(5);

                MainV2.comPort.MAV.cs.UpdateCurrentSettings(currentStateBindingSource.UpdateDataSource(MainV2.comPort.MAV.cs), true, MainV2.comPort);

                // check for non 0 values
                if (MainV2.comPort.MAV.cs.ch1in > 800 && MainV2.comPort.MAV.cs.ch1in < 2200)
                {
                    rcmin[0] = Math.Min(rcmin[0], MainV2.comPort.MAV.cs.ch1in);
                    rcmax[0] = Math.Max(rcmax[0], MainV2.comPort.MAV.cs.ch1in);

                    rcmin[1] = Math.Min(rcmin[1], MainV2.comPort.MAV.cs.ch2in);
                    rcmax[1] = Math.Max(rcmax[1], MainV2.comPort.MAV.cs.ch2in);

                    rcmin[2] = Math.Min(rcmin[2], MainV2.comPort.MAV.cs.ch3in);
                    rcmax[2] = Math.Max(rcmax[2], MainV2.comPort.MAV.cs.ch3in);

                    rcmin[3] = Math.Min(rcmin[3], MainV2.comPort.MAV.cs.ch4in);
                    rcmax[3] = Math.Max(rcmax[3], MainV2.comPort.MAV.cs.ch4in);

                    rcmin[4] = Math.Min(rcmin[4], MainV2.comPort.MAV.cs.ch5in);
                    rcmax[4] = Math.Max(rcmax[4], MainV2.comPort.MAV.cs.ch5in);

                    rcmin[5] = Math.Min(rcmin[5], MainV2.comPort.MAV.cs.ch6in);
                    rcmax[5] = Math.Max(rcmax[5], MainV2.comPort.MAV.cs.ch6in);

                    rcmin[6] = Math.Min(rcmin[6], MainV2.comPort.MAV.cs.ch7in);
                    rcmax[6] = Math.Max(rcmax[6], MainV2.comPort.MAV.cs.ch7in);

                    rcmin[7] = Math.Min(rcmin[7], MainV2.comPort.MAV.cs.ch8in);
                    rcmax[7] = Math.Max(rcmax[7], MainV2.comPort.MAV.cs.ch8in);

                    rcmin[8] = Math.Min(rcmin[8], MainV2.comPort.MAV.cs.ch9in);
                    rcmax[8] = Math.Max(rcmax[8], MainV2.comPort.MAV.cs.ch9in);

                    rcmin[9] = Math.Min(rcmin[9], MainV2.comPort.MAV.cs.ch10in);
                    rcmax[9] = Math.Max(rcmax[9], MainV2.comPort.MAV.cs.ch10in);

                    rcmin[10] = Math.Min(rcmin[10], MainV2.comPort.MAV.cs.ch11in);
                    rcmax[10] = Math.Max(rcmax[10], MainV2.comPort.MAV.cs.ch11in);

                    rcmin[11] = Math.Min(rcmin[11], MainV2.comPort.MAV.cs.ch12in);
                    rcmax[11] = Math.Max(rcmax[11], MainV2.comPort.MAV.cs.ch12in);

                    rcmin[12] = Math.Min(rcmin[12], MainV2.comPort.MAV.cs.ch13in);
                    rcmax[12] = Math.Max(rcmax[12], MainV2.comPort.MAV.cs.ch13in);

                    rcmin[13] = Math.Min(rcmin[13], MainV2.comPort.MAV.cs.ch14in);
                    rcmax[13] = Math.Max(rcmax[13], MainV2.comPort.MAV.cs.ch14in);

                    rcmin[14] = Math.Min(rcmin[14], MainV2.comPort.MAV.cs.ch15in);
                    rcmax[14] = Math.Max(rcmax[14], MainV2.comPort.MAV.cs.ch15in);

                    rcmin[15] = Math.Min(rcmin[15], MainV2.comPort.MAV.cs.ch16in);
                    rcmax[15] = Math.Max(rcmax[15], MainV2.comPort.MAV.cs.ch16in);

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
            }

            if (rcmin[0] > 800 && rcmin[0] < 2200)
            {
            }
            else
            {
                CustomMessageBox.Show("Bad channel 1 input, canceling");
                return;
            }

            CustomMessageBox.Show("Ensure all your sticks are centered and throttle is down, and click ok to continue\nIf you have a sprung throttle then center it and click ok");

            MainV2.comPort.MAV.cs.UpdateCurrentSettings(currentStateBindingSource.UpdateDataSource(MainV2.comPort.MAV.cs), true, MainV2.comPort);

            rctrim[0] = Constrain(MainV2.comPort.MAV.cs.ch1in, 0);
            rctrim[1] = Constrain(MainV2.comPort.MAV.cs.ch2in, 1);
            rctrim[2] = Constrain(MainV2.comPort.MAV.cs.ch3in, 2);
            rctrim[3] = Constrain(MainV2.comPort.MAV.cs.ch4in, 3);
            rctrim[4] = Constrain(MainV2.comPort.MAV.cs.ch5in, 4);
            rctrim[5] = Constrain(MainV2.comPort.MAV.cs.ch6in, 5);
            rctrim[6] = Constrain(MainV2.comPort.MAV.cs.ch7in, 6);
            rctrim[7] = Constrain(MainV2.comPort.MAV.cs.ch8in, 7);

            rctrim[8] = Constrain(MainV2.comPort.MAV.cs.ch9in, 8);
            rctrim[9] = Constrain(MainV2.comPort.MAV.cs.ch10in, 9);
            rctrim[10] = Constrain(MainV2.comPort.MAV.cs.ch11in, 10);
            rctrim[11] = Constrain(MainV2.comPort.MAV.cs.ch12in, 11);
            rctrim[12] = Constrain(MainV2.comPort.MAV.cs.ch13in, 12);
            rctrim[13] = Constrain(MainV2.comPort.MAV.cs.ch14in, 13);
            rctrim[14] = Constrain(MainV2.comPort.MAV.cs.ch15in, 14);
            rctrim[15] = Constrain(MainV2.comPort.MAV.cs.ch16in, 15);

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

        private void CHK_revch1_CheckedChanged(object sender, EventArgs e)
        {
            reverseChannel(((CheckBox)sender).Checked, BARroll);
        }

        private void CHK_revch2_CheckedChanged(object sender, EventArgs e)
        {
            reverseChannel(((CheckBox)sender).Checked, BARpitch);
        }

        private void CHK_revch3_CheckedChanged(object sender, EventArgs e)
        {
            reverseChannel(((CheckBox)sender).Checked, BARthrottle);
        }

        private void CHK_revch4_CheckedChanged(object sender, EventArgs e)
        {
            reverseChannel(((CheckBox)sender).Checked, BARyaw);
        }

        private void reverseChannel(bool normalreverse, Control progressbar)
        {
            if (normalreverse)
            {
                ((HorizontalProgressBar2)progressbar).reverse = true;
                ((HorizontalProgressBar2)progressbar).BackgroundColor = Color.FromArgb(148, 193, 31);
                ((HorizontalProgressBar2)progressbar).ValueColor = Color.FromArgb(0x43, 0x44, 0x45);
            }
            else
            {
                ((HorizontalProgressBar2)progressbar).reverse = false;
                ((HorizontalProgressBar2)progressbar).BackgroundColor = Color.FromArgb(0x43, 0x44, 0x45);
                ((HorizontalProgressBar2)progressbar).ValueColor = Color.FromArgb(148, 193, 31);
            }

            if (startup)
                return;
            if (MainV2.comPort.MAV.param["SWITCH_ENABLE"] != null &&
                (float)MainV2.comPort.MAV.param["SWITCH_ENABLE"] == 1)
            {
                try
                {
                    MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, "SWITCH_ENABLE", 0);
                    CustomMessageBox.Show("Disabled Dip Switchs");
                }
                catch
                {
                    CustomMessageBox.Show("Error Disableing Dip Switch");
                }
            }
        }

        private void BUT_Bindradiodsm2_Click(object sender, EventArgs e)
        {
            try
            {
                MainV2.comPort.doCommand((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, MAVLink.MAV_CMD.START_RX_PAIR, 0, 0, 0, 0, 0, 0, 0);
                CustomMessageBox.Show(Strings.Put_the_transmitter_in_bind_mode__Receiver_is_waiting);
            }
            catch
            {
                CustomMessageBox.Show(Strings.Error_binding);
            }
        }

        private void BUT_BindradiodsmX_Click(object sender, EventArgs e)
        {
            try
            {
                MainV2.comPort.doCommand((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, MAVLink.MAV_CMD.START_RX_PAIR, 0, 1, 0, 0, 0, 0, 0);
                CustomMessageBox.Show(Strings.Put_the_transmitter_in_bind_mode__Receiver_is_waiting);
            }
            catch
            {
                CustomMessageBox.Show(Strings.Error_binding);
            }
        }

        private void BUT_Bindradiodsm8_Click(object sender, EventArgs e)
        {
            try
            {
                MainV2.comPort.doCommand((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, MAVLink.MAV_CMD.START_RX_PAIR, 0, 2, 0, 0, 0, 0, 0);
                CustomMessageBox.Show(Strings.Put_the_transmitter_in_bind_mode__Receiver_is_waiting);
            }
            catch
            {
                CustomMessageBox.Show(Strings.Error_binding);
            }
        }
    }
}