﻿using MissionPlanner.Comms;
using MissionPlanner.Utilities;
using System;
using System.Windows.Forms;

namespace MissionPlanner.Antenna
{
    public class TrackerGeneric
    {
        private TrackerUI _tracker;
        private System.Threading.Thread t12;
        private static bool threadrun = false;
        private static ITrackerOutput tracker;
        private Func<MAVLinkInterface> _comPort;

        private MAVLinkInterface comPort
        {
            get { return _comPort(); }
        }

        public TrackerGeneric(TrackerUI tracker, Func<MAVLinkInterface> comPort)
        {
            _comPort = comPort;
            _tracker = tracker;

            _tracker.CMB_interface.SelectedIndexChanged += new System.EventHandler(CMB_interface_SelectedIndexChanged);
            _tracker.TRK_pantrim.Scroll += new System.EventHandler(TRK_pantrim_Scroll);
            _tracker.TXT_panrange.TextChanged += new System.EventHandler(TXT_panrange_TextChanged);
            _tracker.TXT_tiltrange.TextChanged += new System.EventHandler(TXT_tiltrange_TextChanged);

            _tracker.TRK_tilttrim.Scroll += new System.EventHandler(TRK_tilttrim_Scroll);

            _tracker.CHK_revpan.CheckedChanged += new System.EventHandler(CHK_revpan_CheckedChanged);

            _tracker.CHK_revtilt.CheckedChanged += new System.EventHandler(CHK_revtilt_CheckedChanged);

            _tracker.BUT_connect.Click += new System.EventHandler(BUT_connect_Click);

            _tracker.TXT_centerpan.TextChanged += new System.EventHandler(TXT_centerpan_TextChanged);

            _tracker.TXT_centertilt.TextChanged += new System.EventHandler(TXT_centertilt_TextChanged);

            _tracker.TXT_panspeed.TextChanged += new System.EventHandler(TXT_panspeed_TextChanged);

            _tracker.TXT_panaccel.TextChanged += new System.EventHandler(TXT_panaccel_TextChanged);


            _tracker.TXT_tiltspeed.TextChanged += new System.EventHandler(TXT_tiltspeed_TextChanged);
            _tracker.BUT_find.Click += new System.EventHandler(BUT_find_Click);
            _tracker.TXT_tiltaccel.TextChanged += new System.EventHandler(TXT_tiltaccel_TextChanged);


        }

        public void Deactivate()
        {
            saveconfig();
        }

        private enum interfaces
        {
            Maestro,
            ArduTracker,
            DegreeTracker
        }

        private void saveconfig()
        {
            foreach (Control ctl in _tracker.Controls)
            {
                if (typeof(TextBox) == ctl.GetType() ||
                    typeof(ComboBox) == ctl.GetType())
                {
                    Settings.Instance["Tracker_" + ctl.Name] = ctl.Text;
                }
                if (typeof(TrackBar) == ctl.GetType())
                {
                    Settings.Instance["Tracker_" + ctl.Name] = ((TrackBar)ctl).Value.ToString();
                }
                if (typeof(CheckBox) == ctl.GetType())
                {
                    Settings.Instance["Tracker_" + ctl.Name] = ((CheckBox)ctl).Checked.ToString();
                }
            }
        }

        public void BUT_connect_Click(object sender, EventArgs e)
        {
            saveconfig();

            if (threadrun)
            {
                threadrun = false;
                _tracker.BUT_connect.Text = Strings.Connect;
                tracker.Close();
                foreach (Control ctl in _tracker.Controls)
                {
                    if (ctl.Name.StartsWith("TXT_"))
                        ctl.Enabled = true;

                    if (ctl.Name.StartsWith("CMB_"))
                        ctl.Enabled = true;
                }

                _tracker.BUT_find.Enabled = true;
                //CustomMessageBox.Show("Disconnected!");
                return;
            }

            if (tracker != null && tracker.ComPort != null && tracker.ComPort.IsOpen)
            {
                tracker.ComPort.Close();
            }

            if (_tracker.CMB_interface.Text == interfaces.Maestro.ToString())
                tracker = new Antenna.Maestro();
            if (_tracker.CMB_interface.Text == interfaces.ArduTracker.ToString())
                tracker = new Antenna.ArduTracker();
            if (_tracker.CMB_interface.Text == interfaces.DegreeTracker.ToString())
                tracker = new Antenna.DegreeTracker();

            try
            {
                tracker.ComPort = new SerialPort()
                {
                    PortName = _tracker.CMB_serialport.Text,
                    BaudRate = int.Parse(_tracker.CMB_baudrate.Text)
                };
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(Strings.ErrorConnecting + ex.Message, Strings.ERROR);
                return;
            }

            try
            {
                tracker.PanStartRange = int.Parse(_tracker.TXT_panrange.Text) / 2 * -1;
                tracker.PanEndRange = int.Parse(_tracker.TXT_panrange.Text) / 2;
                tracker.TrimPan = _tracker.TRK_pantrim.Value;

                tracker.TiltStartRange = int.Parse(_tracker.TXT_tiltrange.Text) / 2 * -1;
                tracker.TiltEndRange = int.Parse(_tracker.TXT_tiltrange.Text) / 2;
                tracker.TrimTilt = _tracker.TRK_tilttrim.Value;

                tracker.PanReverse = _tracker.CHK_revpan.Checked;
                tracker.TiltReverse = _tracker.CHK_revtilt.Checked;

                tracker.PanPWMRange = int.Parse(_tracker.TXT_pwmrangepan.Text);
                tracker.TiltPWMRange = int.Parse(_tracker.TXT_pwmrangetilt.Text);

                tracker.PanPWMCenter = int.Parse(_tracker.TXT_centerpan.Text);
                tracker.TiltPWMCenter = int.Parse(_tracker.TXT_centertilt.Text);

                tracker.PanSpeed = int.Parse(_tracker.TXT_panspeed.Text);
                tracker.PanAccel = int.Parse(_tracker.TXT_panaccel.Text);
                tracker.TiltSpeed = int.Parse(_tracker.TXT_tiltspeed.Text);
                tracker.TiltAccel = int.Parse(_tracker.TXT_tiltaccel.Text);
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(Strings.InvalidNumberEntered + ex.Message, Strings.ERROR);
                return;
            }

            if (tracker.Init())
            {
                if (tracker.Setup())
                {
                    if (_tracker.TXT_centerpan.Text != tracker.PanPWMCenter.ToString()) _tracker.TXT_centerpan.Text = tracker.PanPWMCenter.ToString();

                    if (_tracker.TXT_centertilt.Text != tracker.TiltPWMCenter.ToString()) _tracker.TXT_centertilt.Text = tracker.TiltPWMCenter.ToString();

                    try
                    {
                        tracker.PanAndTilt(0, 0);
                    }
                    catch (Exception ex)
                    {
                        CustomMessageBox.Show("Failed to set initial pan and tilt\n" + ex.Message, Strings.ERROR);
                        tracker.Close();
                        return;
                    }

                    foreach (Control ctl in _tracker.Controls)
                    {
                        if (ctl.Name.StartsWith("TXT_"))
                            ctl.Enabled = false;

                        if (ctl.Name.StartsWith("CMB_"))
                            ctl.Enabled = false;
                    }
                    //BUT_find.Enabled = false;

                    t12 = new System.Threading.Thread(new System.Threading.ThreadStart(mainloop))
                    {
                        IsBackground = true,
                        Name = "Antenna Tracker"
                    };
                    t12.Start();
                }
            }

            _tracker.BUT_connect.Text = Strings.Disconnect;
        }

        private void mainloop()
        {
            threadrun = true;
            while (threadrun)
            {
                try
                {
                    // 10 hz - position updates default to 3 hz on the stream rate
                    tracker.PanAndTilt(comPort.MAV.cs.AZToMAV, comPort.MAV.cs.ELToMAV);
                    System.Threading.Thread.Sleep(100);
                }
                catch
                {
                }
            }
        }

        public void TRK_pantrim_Scroll(object sender, EventArgs e)
        {
            if (tracker != null)
                tracker.TrimPan = _tracker.TRK_pantrim.Value;
            _tracker.LBL_pantrim.Text = _tracker.TRK_pantrim.Value.ToString();
        }

        public void TRK_tilttrim_Scroll(object sender, EventArgs e)
        {
            if (tracker != null)
                tracker.TrimTilt = _tracker.TRK_tilttrim.Value;
            _tracker.LBL_tilttrim.Text = _tracker.TRK_tilttrim.Value.ToString();
        }

        public void TXT_panrange_TextChanged(object sender, EventArgs e)
        {
            int range;

            int.TryParse(_tracker.TXT_panrange.Text, out range);

            range = 360;

            _tracker.TRK_pantrim.Minimum = range / 2 * -1;
            _tracker.TRK_pantrim.Maximum = range / 2;
        }

        public void TXT_tiltrange_TextChanged(object sender, EventArgs e)
        {
            int range;

            int.TryParse(_tracker.TXT_tiltrange.Text, out range);

            _tracker.TRK_tilttrim.Minimum = range / 2 * -1;
            _tracker.TRK_tilttrim.Maximum = range / 2;
        }

        public void CHK_revpan_CheckedChanged(object sender, EventArgs e)
        {
        }

        public void CHK_revtilt_CheckedChanged(object sender, EventArgs e)
        {
        }

        public void BUT_find_Click(object sender, EventArgs e)
        {
            System.Threading.ThreadPool.QueueUserWorkItem(tm1_Tick);
        }

        private void tm1_Tick(object item)
        {
            float snr = comPort.MAV.cs.localsnrdb;
            float best = snr;

            float tilt = 0;
            float pan = 0;

            if (snr == 0)
            {
                CustomMessageBox.Show(Strings.No_valid_sik_radio, Strings.ERROR);
                return;
            }

            _tracker.Invoke((MethodInvoker)delegate
            {
                tilt = _tracker.TRK_tilttrim.Value;
                pan = _tracker.TRK_pantrim.Value;
            });


            // scan half range within 30 degrees
            float ans = checkpos((pan - float.Parse(_tracker.TXT_panrange.Text) / 4), (pan + float.Parse(_tracker.TXT_panrange.Text) / 4) - 1,
                30);

            // scan new range within 30 - little overlap
            ans = checkpos((-30 + ans), (30 + ans), 5);

            // scan new range
            ans = checkpos((-5 + ans), (5 + ans), 1);

            setpan(ans);
        }

        private void setpan(float no)
        {
            _tracker.Invoke((MethodInvoker)delegate
            {
                try
                {
                    _tracker.TRK_pantrim.Value = (int)no;
                    TRK_pantrim_Scroll(null, null);
                }
                catch
                {
                    return;
                }
            });
        }

        private float checkpos(float start, float end, float scale)
        {
            float lastsnr = 0;
            float best = 0;

            setpan(start);

            System.Threading.Thread.Sleep(4000);

            for (float n = start; n < end; n += scale)
            {
                setpan(n);

                System.Threading.Thread.Sleep(2000);

                Console.WriteLine("Angle " + n + " snr " + comPort.MAV.cs.localsnrdb);

                if (comPort.MAV.cs.localsnrdb > lastsnr)
                {
                    best = n;
                    lastsnr = comPort.MAV.cs.localsnrdb;
                }
            }

            return best;
        }

        public void CMB_interface_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_tracker.CMB_interface.Text == interfaces.Maestro.ToString())
            {
                _tracker.TXT_panspeed.Enabled = true;
                _tracker.TXT_panaccel.Enabled = true;
                _tracker.TXT_tiltspeed.Enabled = true;
                _tracker.TXT_tiltaccel.Enabled = true;
            }
            else
            {
                _tracker.TXT_panspeed.Enabled = false;
                _tracker.TXT_panaccel.Enabled = false;
                _tracker.TXT_tiltspeed.Enabled = false;
                _tracker.TXT_tiltaccel.Enabled = false;
            }
        }

        public void TXT_centerpan_TextChanged(object sender, EventArgs e)
        {
        }

        public void TXT_centertilt_TextChanged(object sender, EventArgs e)
        {
        }

        public void TXT_panspeed_TextChanged(object sender, EventArgs e)
        {
            int speed;

            int.TryParse(_tracker.TXT_panspeed.Text, out speed);
            if (tracker != null)
                tracker.PanSpeed = speed;
        }

        public void TXT_tiltspeed_TextChanged(object sender, EventArgs e)
        {
            int speed;

            int.TryParse(_tracker.TXT_tiltspeed.Text, out speed);
            if (tracker != null)
                tracker.TiltSpeed = speed;
        }

        public void TXT_panaccel_TextChanged(object sender, EventArgs e)
        {
            int accel;

            int.TryParse(_tracker.TXT_panaccel.Text, out accel);
            if (tracker != null)
                tracker.PanAccel = accel;
        }

        public void TXT_tiltaccel_TextChanged(object sender, EventArgs e)
        {
            int accel;

            int.TryParse(_tracker.TXT_tiltaccel.Text, out accel);
            if (tracker != null)
                tracker.TiltAccel = accel;
        }

        public void Activate()
        {


            _tracker.CMB_serialport.DataSource = SerialPort.GetPortNames();

            if (threadrun)
            {
                _tracker.BUT_connect.Text = Strings.Disconnect;
            }

            foreach (string key in Settings.Instance.Keys)
            {
                if (key.StartsWith("Tracker_"))
                {
                    var ctls = _tracker.Controls.Find(key.Replace("Tracker_", ""), true);

                    foreach (Control ctl in ctls)
                    {
                        if (typeof(TextBox) == ctl.GetType() ||
                            typeof(ComboBox) == ctl.GetType())
                        {
                            if (Settings.Instance[key] != null)
                                ctl.Text = Settings.Instance[key];
                        }
                        else if (typeof(TrackBar) == ctl.GetType())
                        {
                            ((TrackBar)ctl).Value = Settings.Instance.GetInt32(key);
                        }
                        else if (typeof(CheckBox) == ctl.GetType())
                        {
                            ((CheckBox)ctl).Checked = Settings.Instance.GetBoolean(key);
                        }
                    }
                }
            }

            // update other fields from load params
            TXT_panrange_TextChanged(null, null);
            TXT_tiltrange_TextChanged(null, null);
            TRK_pantrim_Scroll(null, null);
            TRK_tilttrim_Scroll(null, null);
        }
    }
}