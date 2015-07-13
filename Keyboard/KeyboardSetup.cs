using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;
using MissionPlanner.Keyboard;

namespace MissionPlanner.Keyboard
{
    public partial class KeyboardSetup : Form
    {

        [DllImport("user32.dll")]
        static extern bool HideCaret(IntPtr hWnd);

        globalKeyboardHook gkh = new globalKeyboardHook();

        public KeyboardSetup()
        {
            InitializeComponent();
            MissionPlanner.Utilities.Tracking.AddPage(this.GetType().ToString(), this.Text);
            
        }

        public void Keyboard_Load(object sender, EventArgs e)
        {
            //fixed border
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            //maximize button removed
            this.MaximizeBox = false;
            setModeComboBox.DataSource = Common.getModesList(MainV2.comPort.MAV.cs);
            setModeComboBox.ValueMember = "Key";
            setModeComboBox.DisplayMember = "Value";

            if (MainV2.keyboard)
            {
                BUT_enable.Text = "Disable";
            }
        }

        private void BUT_enable_Click(object sender, EventArgs e)
        {
            if (!MainV2.keyboard)
            {
                if (MainV2.comPort.BaseStream.IsOpen)
                {
                    MainV2.comPort.MAV.cs.rcoverridech1 = checkChannel(1, "trim");
                    MainV2.comPort.MAV.cs.rcoverridech2 = checkChannel(2, "trim");
                    MainV2.comPort.MAV.cs.rcoverridech3 = checkChannel(3, "min");
                    MainV2.comPort.MAV.cs.rcoverridech4 = checkChannel(4, "trim");
                    try
                    {
                        gkh.HookedKeys.Add((Keys)System.Enum.Parse(typeof(Keys), accelerateBox.Text, true));
                        gkh.HookedKeys.Add((Keys)System.Enum.Parse(typeof(Keys), decelerateBox.Text, true));
                        gkh.HookedKeys.Add((Keys)System.Enum.Parse(typeof(Keys), rollLeftBox.Text, true));
                        gkh.HookedKeys.Add((Keys)System.Enum.Parse(typeof(Keys), rollRightBox.Text, true));
                        gkh.HookedKeys.Add((Keys)System.Enum.Parse(typeof(Keys), steerLeftBox.Text, true));
                        gkh.HookedKeys.Add((Keys)System.Enum.Parse(typeof(Keys), steerRightBox.Text, true));
                        gkh.HookedKeys.Add((Keys)System.Enum.Parse(typeof(Keys), pitchForwardBox.Text, true));
                        gkh.HookedKeys.Add((Keys)System.Enum.Parse(typeof(Keys), pitchBackwardBox.Text, true));
                        gkh.HookedKeys.Add((Keys)System.Enum.Parse(typeof(Keys), armBox.Text, true));
                        gkh.HookedKeys.Add((Keys)System.Enum.Parse(typeof(Keys), desarmBox.Text, true));
                        gkh.HookedKeys.Add((Keys)System.Enum.Parse(typeof(Keys), setModeBox.Text, true));
                        gkh.HookedKeys.Add(Keys.Control);
                    }
                    catch
                    {
                        MainV2.instance.Invoke((System.Action)
                        delegate
                        {
                            CustomMessageBox.Show("Please insert a key in all boxes before pressing enable", "Empty Boxes");
                        });
                        return;
                    }
                    gkh.KeyDown += new KeyEventHandler(gkh_KeyDown);
                    gkh.KeyUp += new KeyEventHandler(gkh_KeyUp);
                    gkh.hook();
                    MainV2.keyboard = true;
                    BUT_enable.Text = "Disable";
                    timer1.Start();

                }
                else
                    CustomMessageBox.Show("Please connect a UAV first", "Open ComPort");
            }
            else
            {
                gkh.KeyDown -= new KeyEventHandler(gkh_KeyDown);
                gkh.KeyUp -= new KeyEventHandler(gkh_KeyUp);
                gkh.HookedKeys.Clear();
                gkh.unhook();
                MainV2.keyboard = false;
                clearRCOverride();
                timer1.Stop();
                BUT_enable.Text = "Enable";
            }            
        }

        void gkh_KeyUp(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == (Keys)System.Enum.Parse(typeof(Keys), rollLeftBox.Text, true))
            {
                MainV2.comPort.MAV.cs.rcoverridech1 = checkChannel(1,"trim");
            }
            if (e.KeyCode == (Keys)System.Enum.Parse(typeof(Keys), rollRightBox.Text, true))
            {
                MainV2.comPort.MAV.cs.rcoverridech1 = checkChannel(1, "trim");
            }
            if (e.KeyCode == (Keys)System.Enum.Parse(typeof(Keys), pitchForwardBox.Text, true))
            {
                MainV2.comPort.MAV.cs.rcoverridech2 = checkChannel(2, "trim");
            }
            if (e.KeyCode == (Keys)System.Enum.Parse(typeof(Keys), pitchBackwardBox.Text, true))
            {
                MainV2.comPort.MAV.cs.rcoverridech2 = checkChannel(2, "trim");
            }
            if (e.KeyCode == (Keys)System.Enum.Parse(typeof(Keys), steerLeftBox.Text, true))
            {
                MainV2.comPort.MAV.cs.rcoverridech4 = checkChannel(4, "trim");
            }
            if (e.KeyCode == (Keys)System.Enum.Parse(typeof(Keys), steerRightBox.Text, true))
            {
                MainV2.comPort.MAV.cs.rcoverridech4 = checkChannel(4, "trim");
            }
       
        }

        void gkh_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == (Keys)System.Enum.Parse(typeof(Keys), armBox.Text, true))
            {
                MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate()
                {
                    try
                    {
                        MainV2.comPort.doARM(true);
                    }
                    catch { CustomMessageBox.Show("Failed to Arm"); }
                });
            }
            if (e.KeyCode == (Keys)System.Enum.Parse(typeof(Keys), desarmBox.Text, true))
            {
                MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate()
                {
                    try
                    {
                        MainV2.comPort.doARM(false);
                    }
                    catch { CustomMessageBox.Show("Failed to Disarm"); }
                });
            }
            if (e.KeyCode == (Keys)System.Enum.Parse(typeof(Keys), setModeBox.Text, true))
            {
                MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate()
                {
                    try
                    {
                        MainV2.comPort.setMode(setModeComboBox.Text.ToString());
                    }
                    catch { CustomMessageBox.Show("Failed to change Modes"); }
                });
            }
            if (e.KeyCode == (Keys)System.Enum.Parse(typeof(Keys), rollLeftBox.Text, true))
            {
                if (Control.ModifierKeys == Keys.Control)
                    MainV2.comPort.MAV.cs.rcoverridech1 = checkChannel(1, "min");
                else
                    MainV2.comPort.MAV.cs.rcoverridech1 = (ushort)(checkChannel(1, "trim") - Convert.ToUInt16(rollTrackBar.Value));
            }
            if (e.KeyCode == (Keys)System.Enum.Parse(typeof(Keys), rollRightBox.Text, true))
            {
                if (Control.ModifierKeys == Keys.Control)
                    MainV2.comPort.MAV.cs.rcoverridech1 = checkChannel(1, "max");
                else
                    MainV2.comPort.MAV.cs.rcoverridech1 = (ushort)(checkChannel(1, "trim") + Convert.ToUInt16(rollTrackBar.Value));
            }
            if (e.KeyCode == (Keys)System.Enum.Parse(typeof(Keys), pitchForwardBox.Text, true))
            {
                if (Control.ModifierKeys == Keys.Control)
                    MainV2.comPort.MAV.cs.rcoverridech2 = checkChannel(2, "min");
                else
                    MainV2.comPort.MAV.cs.rcoverridech2 = (ushort)(checkChannel(2, "trim") - Convert.ToUInt16(pitchTrackBar.Value));
            }
            if (e.KeyCode == (Keys)System.Enum.Parse(typeof(Keys), pitchBackwardBox.Text, true))
            {
                if (Control.ModifierKeys == Keys.Control)
                    MainV2.comPort.MAV.cs.rcoverridech2 = checkChannel(2, "max");
                else
                    MainV2.comPort.MAV.cs.rcoverridech2 = (ushort)(checkChannel(2, "trim") + Convert.ToUInt16(pitchTrackBar.Value));
            }
            if (e.KeyCode == (Keys)System.Enum.Parse(typeof(Keys), accelerateBox.Text, true))
            {
                if (MainV2.comPort.MAV.cs.rcoverridech3 < checkChannel(3, "max"))
                    MainV2.comPort.MAV.cs.rcoverridech3 += (ushort)(100);
            }
            if (e.KeyCode == (Keys)System.Enum.Parse(typeof(Keys), decelerateBox.Text, true))
            {
                if (MainV2.comPort.MAV.cs.rcoverridech3 > checkChannel(3, "min"))
                    MainV2.comPort.MAV.cs.rcoverridech3 -= (ushort)(100);
            }
            if (e.KeyCode == (Keys)System.Enum.Parse(typeof(Keys), steerLeftBox.Text, true))
            {
                if (Control.ModifierKeys == Keys.Control)
                    MainV2.comPort.MAV.cs.rcoverridech4 = checkChannel(4, "min");
                else
                    MainV2.comPort.MAV.cs.rcoverridech4 = (ushort)(checkChannel(4, "trim") - Convert.ToUInt16(yawTrackBar.Value));
            }
            if (e.KeyCode == (Keys)System.Enum.Parse(typeof(Keys), steerRightBox.Text, true))
            {
                if(Control.ModifierKeys == Keys.Control)
                    MainV2.comPort.MAV.cs.rcoverridech4 = checkChannel(4, "max");
                else
                    MainV2.comPort.MAV.cs.rcoverridech4 = (ushort)(checkChannel(4, "trim") + Convert.ToUInt16(yawTrackBar.Value));
            }

        }

        public void clearRCOverride()
        {

            MAVLink.mavlink_rc_channels_override_t rc = new MAVLink.mavlink_rc_channels_override_t();

            rc.target_component = MainV2.comPort.MAV.compid;
            rc.target_system = MainV2.comPort.MAV.sysid;

            rc.chan1_raw = 0;
            rc.chan2_raw = 0;
            rc.chan3_raw = 0;
            rc.chan4_raw = 0;
            rc.chan5_raw = 0;
            rc.chan6_raw = 0;
            rc.chan7_raw = 0;
            rc.chan8_raw = 0;

            try
            {
                MainV2.comPort.sendPacket(rc);
                MainV2.comPort.sendPacket(rc);
                MainV2.comPort.sendPacket(rc);
            }
            catch { }
        }


        private ushort checkChannel(int chan, string minmaxtrim)
        {
            ushort min, max, trim = 0;
            if (MainV2.comPort.MAV.param.Count > 0)
            {
                try
                {
                    if (MainV2.comPort.MAV.param.ContainsKey("RC" + chan + "_MIN"))
                    {
                        min = (ushort)(float)(MainV2.comPort.MAV.param["RC" + chan + "_MIN"]);
                        max = (ushort)(float)(MainV2.comPort.MAV.param["RC" + chan + "_MAX"]);
                        trim = (ushort)(float)(MainV2.comPort.MAV.param["RC" + chan + "_TRIM"]);
                    }
                    else
                    {
                        min = 1000;
                        max = 2000;
                        trim = 1500;
                    }
                }
                catch
                {
                    min = 1000;
                    max = 2000;
                    trim = 1500;
                }
            }
            else
            {
                min = 1000;
                max = 2000;
                trim = 1500;
            }
            if (minmaxtrim == "min")
                return min;
            if (minmaxtrim == "max")
                return max;
            if (minmaxtrim == "trim")
                return trim;
            else
                return 0;
        }

        /*protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Right:
                case Keys.Left:
                case Keys.Up:
                case Keys.Down:
                    return true;
                case Keys.Shift | Keys.Right:
                case Keys.Shift | Keys.Left:
                case Keys.Shift | Keys.Up:
                case Keys.Shift | Keys.Down:
                    return true;
            }
            return base.IsInputKey(keyData);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            MAVLink.mavlink_rc_channels_override_t rc = new MAVLink.mavlink_rc_channels_override_t();
            rc.target_component = MainV2.comPort.MAV.compid;
            rc.target_system = MainV2.comPort.MAV.sysid;
            rc.chan1_raw = 0;
            rc.chan2_raw = 0;
            rc.chan3_raw = 0;
            rc.chan4_raw = 0;
            rc.chan5_raw = 0;
            rc.chan6_raw = 0;
            rc.chan7_raw = 0;
            rc.chan8_raw = 0;
            switch (e.KeyCode)
            {
                case Keys.Left:
                    //MainV2.comPort.MAV.cs.rcoverridech1 = (ushort)(1400);
                    rc.chan1_raw = (ushort)(1400);
                    MainV2.comPort.sendPacket(rc);
                    break;
                case Keys.Right:
                    //MainV2.comPort.MAV.cs.rcoverridech1 = (ushort)(1600);
                    rc.chan1_raw = (ushort)(1600);
                    MainV2.comPort.sendPacket(rc);
                    break;
                case Keys.Up:
                    //MainV2.comPort.MAV.cs.rcoverridech2 = (ushort)(1400);
                    rc.chan2_raw = (ushort)(1400);
                    MainV2.comPort.sendPacket(rc);
                    break;
                case Keys.Down:
                    //MainV2.comPort.MAV.cs.rcoverridech2 = (ushort)(1600);
                    rc.chan2_raw = (ushort)(1600);
                    MainV2.comPort.sendPacket(rc);
                    break;
                case Keys.Escape:
                    //MainV2.comPort.MAV.cs.rcoverridech1 = (ushort)0;
                    //MainV2.comPort.MAV.cs.rcoverridech2 = (ushort)0;
                    //MainV2.joystick.clearRCOverride();
                    rc.chan1_raw = (ushort)(0);
                    rc.chan2_raw = (ushort)(0);
                    MainV2.comPort.sendPacket(rc);
                    break;
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            MAVLink.mavlink_rc_channels_override_t rc = new MAVLink.mavlink_rc_channels_override_t();
            rc.target_component = MainV2.comPort.MAV.compid;
            rc.target_system = MainV2.comPort.MAV.sysid;
            rc.chan1_raw = 0;
            rc.chan2_raw = 0;
            rc.chan3_raw = 0;
            rc.chan4_raw = 0;
            rc.chan5_raw = 0;
            rc.chan6_raw = 0;
            rc.chan7_raw = 0;
            rc.chan8_raw = 0;
            switch (e.KeyCode)
            {
                case Keys.Left:
                    //MainV2.comPort.MAV.cs.rcoverridech1 = (ushort)(1500);
                    rc.chan1_raw = (ushort)(1500);
                    MainV2.comPort.sendPacket(rc);
                    break;
                case Keys.Right:
                    //MainV2.comPort.MAV.cs.rcoverridech1 = (ushort)(1500);
                    rc.chan1_raw = (ushort)(1500);
                    MainV2.comPort.sendPacket(rc);
                    break;
                case Keys.Up:
                    //MainV2.comPort.MAV.cs.rcoverridech2 = (ushort)(1500);
                    rc.chan2_raw = (ushort)(1500);
                    MainV2.comPort.sendPacket(rc);
                    break;
                case Keys.Down:
                    //MainV2.comPort.MAV.cs.rcoverridech2 = (ushort)(1500);
                    rc.chan2_raw = (ushort)(1500);
                    MainV2.comPort.sendPacket(rc);
                    break;
            }
        }*/

        private void timer1_Tick(object sender, EventArgs e)
        {
            progressBarRoll.Value = MainV2.comPort.MAV.cs.rcoverridech1;
            progressBarPitch.Value = MainV2.comPort.MAV.cs.rcoverridech2;
            progressBarThrottle.Value = MainV2.comPort.MAV.cs.rcoverridech3;
            progressBarYaw.Value = MainV2.comPort.MAV.cs.rcoverridech4;            

            try
            {
                progressBarRoll.Maximum = checkChannel(1,"max");
                progressBarPitch.Maximum = checkChannel(2, "max");
                progressBarThrottle.Maximum = checkChannel(3, "max");
                progressBarYaw.Maximum = checkChannel(4, "max");

                progressBarRoll.Minimum = checkChannel(1, "min");
                progressBarPitch.Minimum = checkChannel(2, "min");
                progressBarThrottle.Minimum = checkChannel(3, "min");
                progressBarYaw.Minimum = checkChannel(4, "min");
            
            }
            catch
            {
                //Exception Error in the application. -2147024866 (DIERR_INPUTLOST)

            }

            try
            {
                rollTrackBar.Maximum = (checkChannel(1, "max")- checkChannel(1, "min"))/2;
                pitchTrackBar.Maximum = (checkChannel(2, "max") - checkChannel(2, "min"))/2;
                yawTrackBar.Maximum = (checkChannel(4, "max") - checkChannel(4, "min"))/2;
                rollTrackBarMaxValue.Text = rollTrackBar.Maximum.ToString();
                pitchTrackBarMaxValue.Text = pitchTrackBar.Maximum.ToString();
                yawTrackBarMaxValue.Text = yawTrackBar.Maximum.ToString();

                rollTrackBar.Minimum = 100;
                pitchTrackBar.Minimum = 100;
                yawTrackBar.Minimum = 100;
                rollTrackBarMinValue.Text = rollTrackBar.Minimum.ToString();
                pitchTrackBarMinValue.Text = pitchTrackBar.Minimum.ToString();
                yawTrackBarMinValue.Text = yawTrackBar.Minimum.ToString();

            }
            catch
            {
                //Exception Error in the application. -2147024866 (DIERR_INPUTLOST)

            }
        }

        string auxText;

        public void allTextBox_Click(object sender, EventArgs e)
        {
            TextBox tbox = (TextBox)sender;            
            if (!MainV2.keyboard)
            {
                tbox.Cursor = Cursors.Default;
                if (tbox.ReadOnly)
                {
                    auxText = tbox.Text;
                    tbox.ReadOnly = false;
                    tbox.Text = string.Empty;
                    tbox.BackColor = TextBox.DefaultBackColor;
                    tbox.ForeColor = TextBox.DefaultForeColor;
                }
            }
            HideCaret(tbox.Handle);
            
        }

        private void allTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox tbox = (TextBox)sender;
            if(!tbox.ReadOnly)
            {
                switch(e.KeyCode)
                {
                    case Keys.ControlKey:
                        CustomMessageBox.Show("Control Key not allowed.", "Warning");
                        tbox.Text = auxText;
                        break;
                    case Keys.LControlKey:
                        CustomMessageBox.Show("Control Key not allowed.", "Warning");
                        tbox.Text = auxText;
                        break;
                    case Keys.Escape:
                        tbox.Text = auxText;
                        break;
                    default:
                        tbox.AppendText(e.KeyCode.ToString());
                        break;
                }
                tbox.BackColor = ThemeManager.ControlBGColor;
                tbox.ForeColor = ThemeManager.TextColor;
                tbox.ReadOnly = true;
                
            }
        }

        private void allTextBox_Leave(object sender, EventArgs e)
        {
            TextBox tbox = (TextBox)sender;
            if (!tbox.ReadOnly)
            {
                tbox.Text = auxText;
                tbox.BackColor = ThemeManager.ControlBGColor;
                tbox.ForeColor = ThemeManager.TextColor;
                tbox.ReadOnly = true;
            }
        }

        private void allTextBox_MouseEnter(object sender, EventArgs e)
        {
            TextBox tbox = (TextBox)sender;
            if (MainV2.keyboard)
            {
                tbox.Cursor = Cursors.No;
            }
            if (!MainV2.keyboard)
            {
                tbox.Cursor = Cursors.Hand;
            }
        }

        private void BUT_help_Click(object sender, EventArgs e)
        {
            new Keyboard_Help().ShowDialog();
        }

    }
}
