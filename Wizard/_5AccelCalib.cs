using System;
using System.Text;
using System.Windows.Forms;
using MissionPlanner.Controls;

namespace MissionPlanner.Wizard
{
    public partial class _5AccelCalib : MyUserControl, IWizard
    {
        byte count = 0;
        static bool busy = false;

        public _5AccelCalib()
        {
            InitializeComponent();
        }

        public int WizardValidate()
        {
            return 1;
        }

        public bool WizardBusy()
        {
            return busy;
        }

        private void BUT_start_Click(object sender, EventArgs e)
        {
            ((MyButton) sender).Enabled = false;
            BUT_continue.Enabled = true;

            busy = true;

            try
            {
                // start the process off
                MainV2.comPort.doCommand(MAVLink.MAV_CMD.PREFLIGHT_CALIBRATION, 0, 0, 0, 0, 1, 0, 0);

                MainV2.comPort.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.STATUSTEXT, receivedPacket);
                MainV2.comPort.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.COMMAND_LONG, receivedPacket);
            }
            catch
            {
                busy = false;
                CustomMessageBox.Show(Strings.ErrorNoResponce, Strings.ERROR);
                return;
            }

            BUT_continue.Focus();
        }

        private bool receivedPacket(MAVLink.MAVLinkMessage arg)
        {
            if (arg.msgid == (uint)MAVLink.MAVLINK_MSG_ID.STATUSTEXT)
            {
                var message = ASCIIEncoding.ASCII.GetString(arg.ToStructure<MAVLink.mavlink_statustext_t>().text);

                UpdateUserMessage(message);

                if (message.ToLower().Contains("calibration successful") ||
                 message.ToLower().Contains("calibration failed"))
                {
                    try
                    {
                        Invoke((MethodInvoker)delegate
                        {
                            imageLabel1.Text = Strings.Done;
                            BUT_continue.Enabled = false;
                            BUT_start.Enabled = true;
                        });

                        busy = false;
                        MainV2.comPort.UnSubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.STATUSTEXT, receivedPacket);
                        MainV2.comPort.UnSubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.COMMAND_LONG, receivedPacket);
                    }
                    catch
                    {
                    }
                }
            }

            if (arg.msgid == (uint)MAVLink.MAVLINK_MSG_ID.COMMAND_LONG)
            {
                var message = arg.ToStructure<MAVLink.mavlink_command_long_t>();
                if (message.command == (ushort)MAVLink.MAV_CMD.ACCELCAL_VEHICLE_POS)
                {
                    MAVLink.ACCELCAL_VEHICLE_POS pos = (MAVLink.ACCELCAL_VEHICLE_POS)message.param1;

                    UpdateUserMessage("Please place vehicle " + pos.ToString());
                }
            }

            return true;
        }

        public void UpdateUserMessage(string message)
        {
            this.Invoke((MethodInvoker) delegate()
            {
                if (message.ToLower().Contains("initi"))
                {
                    //imageLabel1.Image = MissionPlanner.Properties.Resources.calibration01;
                    //imageLabel1.Text = message;
                }
                if (message.ToLower().Contains("level"))
                {
                    imageLabel1.Image = MissionPlanner.Properties.Resources.calibration01;
                    imageLabel1.Text = message;
                }
                else if (message.ToLower().Contains("left"))
                {
                    imageLabel1.Image = MissionPlanner.Properties.Resources.calibration07;
                    imageLabel1.Text = message;
                }
                else if (message.ToLower().Contains("right"))
                {
                    imageLabel1.Image = MissionPlanner.Properties.Resources.calibration05;
                    imageLabel1.Text = message;
                }
                else if (message.ToLower().Contains("down"))
                {
                    imageLabel1.Image = MissionPlanner.Properties.Resources.calibration04;
                    imageLabel1.Text = message;
                }
                else if (message.ToLower().Contains("up"))
                {
                    imageLabel1.Image = MissionPlanner.Properties.Resources.calibration06;
                    imageLabel1.Text = message;
                }
                else if (message.ToLower().Contains("back"))
                {
                    imageLabel1.Image = MissionPlanner.Properties.Resources.calibration03;
                    imageLabel1.Text = message;
                }
                else if (message.ToLower().Contains("calibration"))
                {
                    imageLabel1.Image = MissionPlanner.Properties.Resources.calibration01;
                    imageLabel1.Text = message;
                }

                imageLabel1.Refresh();
            });
        }

        private void BUT_continue_Click(object sender, EventArgs e)
        {
            count++;

            try
            {
                MainV2.comPort.sendPacket(new MAVLink.mavlink_command_ack_t() {command = 1, result = count}, MainV2.comPort.sysidcurrent, MainV2.comPort.compidcurrent);
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(Strings.CommandFailed + ex, Strings.ERROR);
                Wizard.instance.Close();
            }
        }
    }
}