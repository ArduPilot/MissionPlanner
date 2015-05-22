using System;
using System.Reflection;
using System.Windows.Forms;
using MissionPlanner.Controls;
using log4net;
using Transitions;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigAccelerometerCalibrationPlane : UserControl, IActivate, IDeactivate
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private const float DisabledOpacity = 0.2F;
        private const float EnabledOpacity = 1.0F;

        public ConfigAccelerometerCalibrationPlane()
        {
            InitializeComponent();
        }

        public void Activate()
        {

            BUT_calib_accell.Enabled = true;
        }

        public void Deactivate()
        {
            MainV2.comPort.giveComport = false;

        }

        byte count = 0;

        private void BUT_calib_accell_Click(object sender, EventArgs e)
        {
            if (MainV2.comPort.giveComport == true)
            {
                    count++;
                    try
                    {
                        MainV2.comPort.sendPacket(new MAVLink.mavlink_command_ack_t() { command = 1, result = count });// doCommand(MAVLink.MAV_CMD.PREFLIGHT_CALIBRATION, 0, 0, 0, 0, 1, 0, 0);
                    }
                    catch { CustomMessageBox.Show(Strings.CommandFailed, Strings.ERROR); return; }
        
                return;
            }

            try
            {
                count = 0;

                Log.Info("Sending accel command (mavlink 1.0)");
                MainV2.comPort.giveComport = true;

                MainV2.comPort.Write("\n\n\n\n\n\n\n\n\n\n\n");
                System.Threading.Thread.Sleep(200);

                MainV2.comPort.doCommand(MAVLink.MAV_CMD.PREFLIGHT_CALIBRATION, 0, 0, 0, 0, 1, 0, 0);
                MainV2.comPort.giveComport = true;

                System.Threading.ThreadPool.QueueUserWorkItem(readmessage,this);

                BUT_calib_accell.Text = "Click When Done";
            }
            catch (Exception ex)
            {
                MainV2.comPort.giveComport = false;
                Log.Error("Exception on level", ex);
                CustomMessageBox.Show("Failed to level : ac2 2.0.37+ is required", Strings.ERROR);
            }
        }

        static void readmessage(object item)
        {
            var local = (ConfigAccelerometerCalibrationPlane)item;

            // clean up history
            MainV2.comPort.MAV.cs.messages.Clear();

            while (!(MainV2.comPort.MAV.cs.message.ToLower().Contains("calibration successful") || MainV2.comPort.MAV.cs.message.ToLower().Contains("calibration failed")))
            {
                try
                {
                    System.Threading.Thread.Sleep(10);
                    // read the message
                    MainV2.comPort.readPacket();
                    // update cs with the message
                    MainV2.comPort.MAV.cs.UpdateCurrentSettings(null);
                    // update user display
                    local.UpdateUserMessage();
                }
                catch { break; }
            }

            MainV2.comPort.giveComport = false;

            try
            {
                local.Invoke((MethodInvoker)delegate()
            {
                local.BUT_calib_accell.Text = "Done";
                local.BUT_calib_accell.Enabled = false;
            });
            }
            catch { }
        }

        public void UpdateUserMessage()
        {
            this.Invoke((MethodInvoker)delegate()
            {
                if (!MainV2.comPort.MAV.cs.message.ToLower().Contains("initi"))
                    lbl_Accel_user.Text = MainV2.comPort.MAV.cs.message;
            });
        }

        private void BUT_level_Click(object sender, EventArgs e)
        {
            try
            {
                if ((float)MainV2.comPort.MAV.param["AHRS_EKF_USE"] > 0)
                {
                    CustomMessageBox.Show("WARNING: 1D calibration should not be used when the EKF is enabled");
                    return;
                }
            }
            catch { }

            try
            {
                Log.Info("Sending level command (mavlink 1.0)");
                MainV2.comPort.doCommand(MAVLink.MAV_CMD.PREFLIGHT_CALIBRATION, 1, 0, 0, 0, 0, 0, 0);

                //BUT_level.Text = "Complete";
            }
            catch (Exception ex)
            {
                Log.Error("Exception on level", ex);
                CustomMessageBox.Show("Failed to level : ac2 2.0.37+ is required", Strings.ERROR);
            }
        }


    }
}
