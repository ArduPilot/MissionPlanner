using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MissionPlanner.ArduPilot;
using MissionPlanner.Controls;
using MissionPlanner.Models;
using MissionPlanner.Utilities;
using Transitions;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigMount : MyUserControl, IActivate
    {
        private readonly Transition _NoErrorTransition;
        private Transition[] _ErrorTransition;
        public string ParamHead = "MNT_";
        private bool startup = true;

        public ConfigMount()
        {
            InitializeComponent();

            var delay = new Transition(new TransitionType_Linear(2000));
            var fadeIn = new Transition(new TransitionType_Linear(800));

            _ErrorTransition = new[] {delay, fadeIn};

            _NoErrorTransition = new Transition(new TransitionType_Linear(10));

            //setup button actions
            foreach (var btn in Controls.Cast<Control>().OfType<Button>())
                btn.Click += HandleButtonClick;

            LNK_wiki.MouseEnter += (s, e) => FadeLinkTo((LinkLabel) s, Color.CornflowerBlue);
            LNK_wiki.MouseLeave += (s, e) => FadeLinkTo((LinkLabel) s, Color.WhiteSmoke);

            SetErrorMessageOpacity();

            if (MainV2.comPort.MAV.cs.firmware == Firmwares.ArduPlane)
            {
                mavlinkComboBoxTilt.Items.AddRange(Enum.GetNames(typeof (Channelap)));
                mavlinkComboBoxRoll.Items.AddRange(Enum.GetNames(typeof (Channelap)));
                mavlinkComboBoxPan.Items.AddRange(Enum.GetNames(typeof (Channelap)));
                CMB_shuttertype.Items.AddRange(Enum.GetNames(typeof (ChannelCameraShutter)));
            }
            else
            {
                mavlinkComboBoxTilt.Items.AddRange(Enum.GetNames(typeof (Channelac)));
                mavlinkComboBoxRoll.Items.AddRange(Enum.GetNames(typeof (Channelac)));
                mavlinkComboBoxPan.Items.AddRange(Enum.GetNames(typeof (Channelac)));
                CMB_shuttertype.Items.AddRange(Enum.GetNames(typeof (ChannelCameraShutter)));
            }

            string remove = "SERVO";
            //cleanup list based on version
            if (MainV2.comPort.MAV.param.ContainsKey("SERVO1_MIN"))
            {
                remove = "RC";
            }

            for (int i = 0; i < mavlinkComboBoxTilt.Items.Count; i++)
            {
                var item = mavlinkComboBoxTilt.Items[i] as string;
                if (item.StartsWith(remove))
                {
                    mavlinkComboBoxTilt.Items.Remove(mavlinkComboBoxTilt.Items[i]);
                    i--;
                    continue;
                }
            }
            for (int i = 0; i < mavlinkComboBoxRoll.Items.Count; i++)
            {
                var item = mavlinkComboBoxRoll.Items[i] as string;
                if (item.StartsWith(remove))
                {
                    mavlinkComboBoxRoll.Items.Remove(mavlinkComboBoxRoll.Items[i]);
                    i--;
                    continue;
                }
            }
            for (int i = 0; i < mavlinkComboBoxPan.Items.Count; i++)
            {
                var item = mavlinkComboBoxPan.Items[i] as string;
                if (item.StartsWith(remove))
                {
                    mavlinkComboBoxPan.Items.Remove(mavlinkComboBoxPan.Items[i]);
                    i--;
                    continue;
                }
            }
            for (int i = 0; i < CMB_shuttertype.Items.Count; i++)
            {
                var item = CMB_shuttertype.Items[i] as string;
                if (item.StartsWith(remove))
                {
                    CMB_shuttertype.Items.Remove(CMB_shuttertype.Items[i]);
                    i--;
                    continue;
                }
            }

            CMB_mnt_type.setup(ParameterMetaDataRepository.GetParameterOptionsInt("MNT_TYPE",
                MainV2.comPort.MAV.cs.firmware.ToString()), "MNT_TYPE", MainV2.comPort.MAV.param);
        }

        public void Activate()
        {
            var copy = new Dictionary<string, double>((Dictionary<string, double>) MainV2.comPort.MAV.param);

            if (!copy.ContainsKey("CAM_TRIGG_TYPE"))
            {
                Enabled = false;
                return;
            }

            startup = true;

            CMB_shuttertype.SelectedItem = Enum.GetName(typeof(ChannelCameraShutter),
                (Int32)MainV2.comPort.MAV.param["CAM_TRIGG_TYPE"]);

            foreach (string item in copy.Keys)
            {
                if (item.EndsWith("_FUNCTION"))
                {
                    switch (MainV2.comPort.MAV.param[item].ToString())
                    {
                        case "6":
                            mavlinkComboBoxPan.Text = item.Replace("_FUNCTION", "");
                            break;
                        case "7":
                            mavlinkComboBoxTilt.Text = item.Replace("_FUNCTION", "");
                            break;
                        case "8":
                            mavlinkComboBoxRoll.Text = item.Replace("_FUNCTION", "");
                            break;
                        case "10":
                            CMB_shuttertype.Text = item.Replace("_FUNCTION", "");
                            break;
                        default:
                            break;
                    }
                }
            }

            startup = false;

            try
            {
                updateShutter();
                updatePitch();
                updateRoll();
                updateYaw();

                CHK_stab_tilt.setup(1, 0, ParamHead + "STAB_TILT", MainV2.comPort.MAV.param);
                CHK_stab_roll.setup(1, 0, ParamHead + "STAB_ROLL", MainV2.comPort.MAV.param);
                CHK_stab_pan.setup(1, 0, ParamHead + "STAB_PAN", MainV2.comPort.MAV.param);

                NUD_CONTROL_x.setup(-180, 180, 1, 1, ParamHead + "CONTROL_X", MainV2.comPort.MAV.param);
                NUD_CONTROL_y.setup(-180, 180, 1, 1, ParamHead + "CONTROL_Y", MainV2.comPort.MAV.param);
                NUD_CONTROL_z.setup(-180, 180, 1, 1, ParamHead + "CONTROL_Z", MainV2.comPort.MAV.param);

                NUD_NEUTRAL_x.setup(-180, 180, 1, 1, ParamHead + "NEUTRAL_X", MainV2.comPort.MAV.param);
                NUD_NEUTRAL_y.setup(-180, 180, 1, 1, ParamHead + "NEUTRAL_Y", MainV2.comPort.MAV.param);
                NUD_NEUTRAL_z.setup(-180, 180, 1, 1, ParamHead + "NEUTRAL_Z", MainV2.comPort.MAV.param);

                NUD_RETRACT_x.setup(-180, 180, 1, 1, ParamHead + "RETRACT_X", MainV2.comPort.MAV.param);
                NUD_RETRACT_y.setup(-180, 180, 1, 1, ParamHead + "RETRACT_Y", MainV2.comPort.MAV.param);
                NUD_RETRACT_z.setup(-180, 180, 1, 1, ParamHead + "RETRACT_Z", MainV2.comPort.MAV.param);
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Failed to set Param\n" + ex);
                Enabled = false;
            }
        }

        private void ensureDisabled(ComboBox cmb, int number, string exclude = "")
        {
            foreach (var itemobj in cmb.Items)
            {
                string item = itemobj.ToString();
                if (MainV2.comPort.MAV.param.ContainsKey(item + "_FUNCTION"))
                {
                    var ans = (float) MainV2.comPort.MAV.param[item + "_FUNCTION"];

                    if (item == exclude)
                        continue;

                    if (ans == number)
                    {
                        MainV2.comPort.setParam(item + "_FUNCTION", 0);
                    }
                }
            }
        }

        private void updateShutter()
        {
            // shutter
            if (CMB_shuttertype.Text == "")
                return;

            if (CMB_shuttertype.Text != "Disable")
            {
                if (CMB_shuttertype.Text == ChannelCameraShutter.Relay.ToString())
                {
                    ensureDisabled(CMB_shuttertype, 10);
                    MainV2.comPort.setParam("CAM_TRIGG_TYPE", 1);
                }
                else if (CMB_shuttertype.Text == ChannelCameraShutter.Transistor.ToString())
                {
                    ensureDisabled(CMB_shuttertype, 10);
                    MainV2.comPort.setParam("CAM_TRIGG_TYPE", 4);
                }
                else
                {
                    ensureDisabled(CMB_shuttertype, 10);
                    MainV2.comPort.setParam(CMB_shuttertype.Text + "_FUNCTION", 10);
                    // servo
                    MainV2.comPort.setParam("CAM_TRIGG_TYPE", 0);
                }
            }
            else
            {
                // servo
                MainV2.comPort.setParam("CAM_TRIGG_TYPE", 0);
                ensureDisabled(CMB_shuttertype, 10);
            }


            mavlinkNumericUpDownShutM.setup(800, 2200, 1, 1, CMB_shuttertype.Text + "_MIN", MainV2.comPort.MAV.param);
            mavlinkNumericUpDownShutMX.setup(800, 2200, 1, 1, CMB_shuttertype.Text + "_MAX", MainV2.comPort.MAV.param);

            mavlinkNumericUpDownshut_pushed.setup(800, 2200, 1, 1, "CAM_SERVO_ON", MainV2.comPort.MAV.param);
            mavlinkNumericUpDownshut_notpushed.setup(800, 2200, 1, 1, "CAM_SERVO_OFF", MainV2.comPort.MAV.param);
            mavlinkNumericUpDownshut_duration.setup(1, 200, 1, 1, "CAM_DURATION", MainV2.comPort.MAV.param);
        }

        private void updatePitch()
        {
            // pitch
            if (mavlinkComboBoxTilt.Text == "")
                return;

            if (mavlinkComboBoxTilt.Text != "Disable")
            {
                MainV2.comPort.setParam(mavlinkComboBoxTilt.Text + "_FUNCTION", 7);
                //MainV2.MainV2.comPort.setParam(ParamHead+"STAB_TILT", 1);
            }
            else
            {
                //MainV2.comPort.setParam(ParamHead+"STAB_TILT", 0);
                ensureDisabled(mavlinkComboBoxTilt, 7);
            }


            mavlinkNumericUpDownTSM.setup(800, 2200, 1, 1, mavlinkComboBoxTilt.Text + "_MIN", MainV2.comPort.MAV.param);
            mavlinkNumericUpDownTSMX.setup(800, 2200, 1, 1, mavlinkComboBoxTilt.Text + "_MAX", MainV2.comPort.MAV.param);
            mavlinkNumericUpDownTAM.setup(-90, 0, 100, 1, ParamHead + "ANGMIN_TIL", MainV2.comPort.MAV.param);
            mavlinkNumericUpDownTAMX.setup(0, 90, 100, 1, ParamHead + "ANGMAX_TIL", MainV2.comPort.MAV.param);
            mavlinkCheckBoxTR.setup(new double[] {-1, 1}, new double[] {1, 0},
                new string[] {mavlinkComboBoxTilt.Text + "_REV", mavlinkComboBoxTilt.Text + "_REVERSED"},
                MainV2.comPort.MAV.param);
            CMB_inputch_tilt.setup(typeof (Channelinput), ParamHead + "RC_IN_TILT", MainV2.comPort.MAV.param);
        }

        private void updateRoll()
        {
            // roll
            if (mavlinkComboBoxRoll.Text == "")
                return;

            if (mavlinkComboBoxRoll.Text != "Disable")
            {
                MainV2.comPort.setParam(mavlinkComboBoxRoll.Text + "_FUNCTION", 8);
                //MainV2.comPort.setParam(ParamHead+"STAB_ROLL", 1);
            }
            else
            {
                //MainV2.comPort.setParam(ParamHead+"STAB_ROLL", 0);
                ensureDisabled(mavlinkComboBoxRoll, 8);
            }

            mavlinkNumericUpDownRSM.setup(800, 2200, 1, 1, mavlinkComboBoxRoll.Text + "_MIN", MainV2.comPort.MAV.param);
            mavlinkNumericUpDownRSMX.setup(800, 2200, 1, 1, mavlinkComboBoxRoll.Text + "_MAX", MainV2.comPort.MAV.param);
            mavlinkNumericUpDownRAM.setup(-90, 0, 100, 1, ParamHead + "ANGMIN_ROL", MainV2.comPort.MAV.param);
            mavlinkNumericUpDownRAMX.setup(0, 90, 100, 1, ParamHead + "ANGMAX_ROL", MainV2.comPort.MAV.param);
            mavlinkCheckBoxRR.setup(new double[] { -1, 1 }, new double[] { 1, 0 },
                new string[] { mavlinkComboBoxRoll.Text + "_REV", mavlinkComboBoxRoll.Text + "_REVERSED" },
                MainV2.comPort.MAV.param);
            CMB_inputch_roll.setup(typeof (Channelinput), ParamHead + "RC_IN_ROLL", MainV2.comPort.MAV.param);
        }

        private void updateYaw()
        {
            // yaw
            if (mavlinkComboBoxPan.Text == "")
                return;

            if (mavlinkComboBoxPan.Text != "Disable")
            {
                MainV2.comPort.setParam(mavlinkComboBoxPan.Text + "_FUNCTION", 6);
                //MainV2.comPort.setParam(ParamHead+"STAB_PAN", 1);
            }
            else
            {
                //MainV2.comPort.setParam(ParamHead+"STAB_PAN", 0);
                ensureDisabled(mavlinkComboBoxPan, 6);
            }

            mavlinkNumericUpDownPSM.setup(800, 2200, 1, 1, mavlinkComboBoxPan.Text + "_MIN", MainV2.comPort.MAV.param);
            mavlinkNumericUpDownPSMX.setup(800, 2200, 1, 1, mavlinkComboBoxPan.Text + "_MAX", MainV2.comPort.MAV.param);
            mavlinkNumericUpDownPAM.setup(-180, 0, 100, 1, ParamHead + "ANGMIN_PAN", MainV2.comPort.MAV.param);
            mavlinkNumericUpDownPAMX.setup(0, 180, 100, 1, ParamHead + "ANGMAX_PAN", MainV2.comPort.MAV.param);
            mavlinkCheckBoxPR.setup(new double[] { -1, 1 }, new double[] { 1, 0 },
                new string[] { mavlinkComboBoxPan.Text + "_REV", mavlinkComboBoxPan.Text + "_REVERSED" },
                MainV2.comPort.MAV.param);
            CMB_inputch_pan.setup(typeof (Channelinput), ParamHead + "RC_IN_PAN", MainV2.comPort.MAV.param);
        }

        private void SetErrorMessageOpacity()
        {
            /* if (_presenter.HasError)
            {
                // Todo - is this the prob? maybe single log trasition
                var t = new Transition(new TransitionType_Acceleration(1000));
                t.add(PBOX_WarningIcon, "Opacity", 1.0F);
                t.add(LBL_Error, "Opacity", 1.0F);
                t.run();

                //Transition.runChain(_ErrorTransition);
            }
            else*/
            {
                _NoErrorTransition.run();
            }
        }

        private static void FadeLinkTo(LinkLabel l, Color c)
        {
            var changeColorTransition = new Transition(new TransitionType_Linear(300));
            changeColorTransition.add(l, "LinkColor", c);
            changeColorTransition.run();
        }

        // Common handler for all buttons
        // Will execute an ICommand if one is found on the button Tag
        private static void HandleButtonClick(object sender, EventArgs e)
        {
            if (sender is Button)
            {
                var cmd = (sender as Button).Tag as ICommand;

                if (cmd != null)
                    if (cmd.CanExecute(null))
                        cmd.Execute(null);
            }
        }

        // Something has changed on the presenter - This may be an Icommand
        // enabled state, so update the buttons as appropriate
        private void CheckCommandStates(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            foreach (var btn in Controls.Cast<Control>().OfType<Button>())
            {
                var cmd = btn.Tag as ICommand;
                if (cmd != null)
                    btn.Enabled = cmd.CanExecute(null);
            }
        }

        private void LNK_Wiki_Clicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(
                new ProcessStartInfo(
                    "http://copter.ardupilot.com/wiki/common-optional-hardware/common-cameras-and-gimbals/common-camera-gimbal/"));
        }

        private void mavlinkComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (startup)
                return;

            var cmb = sender as ComboBox;

            try
            {
                // cleanup all others - disableing any previous selection
                ensureDisabled(cmb, 6, mavlinkComboBoxPan.Text);
                ensureDisabled(cmb, 7, mavlinkComboBoxTilt.Text);
                ensureDisabled(cmb, 8, mavlinkComboBoxRoll.Text);

                // enable 3 axis stabilize
                if (MainV2.comPort.MAV.param.ContainsKey(ParamHead + "MODE"))
                    MainV2.comPort.setParam(ParamHead + "MODE", 3);

                updateShutter();
                updatePitch();
                updateRoll();
                updateYaw();
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Failed to set Param\n" + ex);
            }
        }

        // 0 = disabled 1 = enabled
        private enum Channelap
        {
            Disable = 0,
            RC5 = 1,
            RC6 = 1,
            RC7 = 1,
            RC8 = 1,
            RC9 = 1,
            RC10 = 1,
            RC11 = 1,
            RC12 = 1,
            RC13 = 1,
            RC14 = 1,
            SERVO1 = 1,
            SERVO2 = 1,
            SERVO3 = 1,
            SERVO4 = 1,
            SERVO5 = 1,
            SERVO6 = 1,
            SERVO7 = 1,
            SERVO8 = 1,
            SERVO9 = 1,
            SERVO10 = 1,
            SERVO11 = 1,
            SERVO12 = 1,
            SERVO13 = 1,
            SERVO14 = 1
        }

        // 0 = disabled 1 = enabled
        private enum Channelac
        {
            Disable = 0,
            RC5 = 1,
            RC6 = 1,
            RC7 = 1,
            RC8 = 1,
            RC9 = 1,
            RC10 = 1,
            RC11 = 1,
            RC12 = 1,
            RC13 = 1,
            RC14 = 1,
            SERVO1 = 1,
            SERVO2 = 1,
            SERVO3 = 1,
            SERVO4 = 1,
            SERVO5 = 1,
            SERVO6 = 1,
            SERVO7 = 1,
            SERVO8 = 1,
            SERVO9 = 1,
            SERVO10 = 1,
            SERVO11 = 1,
            SERVO12 = 1,
            SERVO13 = 1,
            SERVO14 = 1
        }

        private enum ChannelCameraShutter
        {
            Disable = 0,
            RC5 = 5,
            RC6 = 6,
            RC7 = 7,
            RC8 = 8,
            RC9 = 9,
            RC10 = 10,
            RC11 = 11,
            RC12 = 12,
            RC13 = 13,
            RC14 = 14,
            SERVO1 = 1,
            SERVO2 = 2,
            SERVO3 = 3,
            SERVO4 = 4,
            SERVO5 = 5,
            SERVO6 = 6,
            SERVO7 = 7,
            SERVO8 = 8,
            SERVO9 = 9,
            SERVO10 = 10,
            SERVO11 = 11,
            SERVO12 = 12,
            SERVO13 = 13,
            SERVO14 = 14,
            Relay = 1,
            Transistor = 4
        }

        private enum Channelinput
        {
            Disable = 0,
            RC5 = 5,
            RC6 = 6,
            RC7 = 7,
            RC8 = 8,
            RC9 = 9,
            RC10 = 10,
            RC11 = 11,
            RC12 = 12,
            RC13 = 13,
            RC14 = 14,
            RC15 = 15,
            RC16 = 16,
        }
    }
}
