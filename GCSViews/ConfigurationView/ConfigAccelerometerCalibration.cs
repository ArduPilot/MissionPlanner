using log4net;
using MissionPlanner.Controls;
using System;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigAccelerometerCalibration : MyUserControl, IActivate, IDeactivate
    {
        private const float DisabledOpacity = 0.2F;
        private const float EnabledOpacity = 1.0F;
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private byte count;

        bool _incalibrate = false;
        private MAVLink.ACCELCAL_VEHICLE_POS pos;
        private int sub1;
        private int sub2;

        public ConfigAccelerometerCalibration()
        {
            InitializeComponent();
            ClearOrientationImage();
        }

        private void ClearOrientationImage()
        {
            pictureBoxOrientation.Visible = false;
            pictureBoxOrientation.Image?.Dispose();
            pictureBoxOrientation.Image = null;
            pictureBoxOrientation.Height = 0;
        }

        public void Activate()
        {
            BUT_calib_accell.Enabled = true;
            _incalibrate = false;
            ClearOrientationImage();
        }

        public void Deactivate()
        {
            MainV2.comPort.giveComport = false;
            _incalibrate = false;
            ClearOrientationImage();
        }

        private string GetOrientationImagePath(MAVLink.ACCELCAL_VEHICLE_POS position)
        {
            string imageName = null;

            switch (position)
            {
                case MAVLink.ACCELCAL_VEHICLE_POS.LEVEL:
                    imageName = "acc_flat.png";
                    break;
                case MAVLink.ACCELCAL_VEHICLE_POS.LEFT:
                    imageName = "acc_left.png";
                    break;
                case MAVLink.ACCELCAL_VEHICLE_POS.RIGHT:
                    imageName = "acc_right.png";
                    break;
                case MAVLink.ACCELCAL_VEHICLE_POS.NOSEDOWN:
                    imageName = "acc_nose_down.png";
                    break;
                case MAVLink.ACCELCAL_VEHICLE_POS.NOSEUP:
                    imageName = "acc_nose_up.png";
                    break;
                case MAVLink.ACCELCAL_VEHICLE_POS.BACK:
                    imageName = "acc_upside_down.png";
                    break;
            }

            if (imageName != null)
            {
                return System.IO.Path.Combine(Application.StartupPath, "Resources", imageName);
            }

            return null;
        }

        private void UpdateOrientationImage(MAVLink.ACCELCAL_VEHICLE_POS position)
        {
            try
            {
                string imagePath = GetOrientationImagePath(position);
                if (!string.IsNullOrEmpty(imagePath) && System.IO.File.Exists(imagePath))
                {
                    pictureBoxOrientation.Image?.Dispose();
                    pictureBoxOrientation.Image = System.Drawing.Image.FromFile(imagePath);
                    pictureBoxOrientation.Height = 400;
                    pictureBoxOrientation.Visible = true;
                }
                else
                {
                    ClearOrientationImage();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                ClearOrientationImage();
            }
        }

        private void BUT_calib_accell_Click(object sender, EventArgs e)
        {
            if (_incalibrate)
            {
                count++;
                try
                {
                    // old
                    //MainV2.comPort.sendPacket(new MAVLink.mavlink_command_ack_t { command = 1, result = count },
                        //MainV2.comPort.sysidcurrent, MainV2.comPort.compidcurrent);
                    // new
                    MainV2.comPort.sendPacket(new MAVLink.mavlink_command_long_t { param1 = (float)pos, command = (ushort)MAVLink.MAV_CMD.ACCELCAL_VEHICLE_POS },
                        MainV2.comPort.sysidcurrent, MainV2.comPort.compidcurrent);
                }
                catch
                {
                    CustomMessageBox.Show(Strings.CommandFailed, Strings.ERROR);
                    return;
                }

                return;
            }

            try
            {
                count = 0;

                Log.Info("Sending accel command (mavlink 1.0)");

                if (MainV2.comPort.doCommand((byte) MainV2.comPort.sysidcurrent, (byte) MainV2.comPort.compidcurrent,
                    MAVLink.MAV_CMD.PREFLIGHT_CALIBRATION, 0, 0, 0, 0, 1, 0, 0))
                {
                    _incalibrate = true;

                    sub1 = MainV2.comPort.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.STATUSTEXT, receivedPacket, (byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent);
                    sub2 = MainV2.comPort.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.COMMAND_LONG, receivedPacket, (byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent);

                    BUT_calib_accell.Text = Strings.Click_when_Done;
                }
                else
                {
                    CustomMessageBox.Show(Strings.CommandFailed, Strings.ERROR);
                }
            }
            catch (Exception ex)
            {
                _incalibrate = false;
                Log.Error("Exception on level", ex);
                CustomMessageBox.Show("Failed to level", Strings.ERROR);
            }
        }

        private bool receivedPacket(MAVLink.MAVLinkMessage arg)
        {
            if (arg.msgid == (uint)MAVLink.MAVLINK_MSG_ID.STATUSTEXT)
            {
                var message = Encoding.ASCII.GetString(arg.ToStructure<MAVLink.mavlink_statustext_t>().text);

                UpdateUserMessage(message);

                if (message.ToLower().Contains("calibration successful") ||
                 message.ToLower().Contains("calibration failed"))
                {
                    try
                    {
                        Invoke((MethodInvoker)delegate
                        {
                            BUT_calib_accell.Text = Strings.Done;
                            BUT_calib_accell.Enabled = false;
                            ClearOrientationImage();
                            groupBoxAccelCal.PerformLayout();
                        });

                        _incalibrate = false;
                        MainV2.comPort.UnSubscribeToPacketType(sub1);
                        MainV2.comPort.UnSubscribeToPacketType(sub2);
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
                    pos = (MAVLink.ACCELCAL_VEHICLE_POS)message.param1;

                    UpdateUserMessage("Please place vehicle " + pos.ToString());

                    Invoke((MethodInvoker)delegate
                    {
                        UpdateOrientationImage(pos);
                    });
                }
            }

            return true;
        }

        public void UpdateUserMessage(string message)
        {
            Invoke((MethodInvoker)delegate
           {
               if (message.ToLower().Contains("place vehicle") || message.ToLower().Contains("calibration"))
                   lbl_Accel_user.Text = message;
           });
        }

        private void BUT_level_Click(object sender, EventArgs e)
        {
            try
            {
                Log.Info("Sending level command (mavlink 1.0)");
                if (MainV2.comPort.doCommand((byte) MainV2.comPort.sysidcurrent, (byte) MainV2.comPort.compidcurrent,
                    MAVLink.MAV_CMD.PREFLIGHT_CALIBRATION, 0, 0, 0, 0, 2, 0, 0))
                {
                    BUT_level.Text = Strings.Completed;
                }
                else
                {
                    CustomMessageBox.Show(Strings.CommandFailed, Strings.ERROR);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Exception on level", ex);
                CustomMessageBox.Show("Failed to level", Strings.ERROR);
            }
        }

        private void BUT_simpleAccelCal_Click(object sender, EventArgs e)
        {
            try
            {
                Log.Info("Sending simple accelerometer calibration command (mavlink 1.0)");
                if (MainV2.comPort.doCommand((byte) MainV2.comPort.sysidcurrent, (byte) MainV2.comPort.compidcurrent,
                    MAVLink.MAV_CMD.PREFLIGHT_CALIBRATION, 0, 0, 0, 0, 4, 0, 0))
                {
                    BUT_simpleAccelCal.Text = Strings.Completed;
                }
                else
                {
                    CustomMessageBox.Show(Strings.CommandFailed, Strings.ERROR);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Exception on simple accelerometer calibration", ex);
                CustomMessageBox.Show("Failed to simple accelerometer calibration", Strings.ERROR);
            }
        }
    }
}