using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using MissionPlanner.ArduPilot;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigFailSafe : MyUserControl, IActivate, IDeactivate
    {
        private readonly Timer timer = new Timer();
        //

        public ConfigFailSafe()
        {
            InitializeComponent();

            // setup rc update
            timer.Tick += timer_Tick;
        }

        public void Activate()
        {
            mavlinkComboBox_fs_thr_enable.setup(
                ParameterMetaDataRepository.GetParameterOptionsInt("FS_THR_ENABLE",
                    MainV2.comPort.MAV.cs.firmware.ToString()), "FS_THR_ENABLE", MainV2.comPort.MAV.param);

            // arducopter
            if (MainV2.comPort.MAV.param.ContainsKey("BATT_FS_LOW_ACT"))
            {
                mavlinkComboBoxfs_batt_enable.setup(
                ParameterMetaDataRepository.GetParameterOptionsInt("BATT_FS_LOW_ACT",
                    MainV2.comPort.MAV.cs.firmware.ToString()), "BATT_FS_LOW_ACT", MainV2.comPort.MAV.param);
            }
            else
            {
                mavlinkComboBoxfs_batt_enable.setup(
                ParameterMetaDataRepository.GetParameterOptionsInt("FS_BATT_ENABLE",
                    MainV2.comPort.MAV.cs.firmware.ToString()), "FS_BATT_ENABLE", MainV2.comPort.MAV.param);
            }
            mavlinkNumericUpDownfs_thr_value.setup(800, 1200, 1, 1, "FS_THR_VALUE", MainV2.comPort.MAV.param);

            // low battery
            if (MainV2.comPort.MAV.param.ContainsKey("LOW_VOLT"))
            {
                mavlinkNumericUpDownlow_voltage.setup(6, 99, 1, 0.1f, "LOW_VOLT", MainV2.comPort.MAV.param, PNL_low_bat);
            }
            else if (MainV2.comPort.MAV.param.ContainsKey("FS_BATT_VOLTAGE"))
            {
                mavlinkNumericUpDownlow_voltage.setup(6, 99, 1, 0.1f, "FS_BATT_VOLTAGE", MainV2.comPort.MAV.param,
                    PNL_low_bat);
            }
            else
            {
                mavlinkNumericUpDownlow_voltage.setup(6, 99, 1, 0.1f, "BATT_LOW_VOLT", MainV2.comPort.MAV.param,
                    PNL_low_bat);
            }

            if (MainV2.comPort.MAV.param.ContainsKey("FS_BATT_MAH"))
            {
                mavlinkNumericUpDownFS_BATT_MAH.setup(1000, 99999, 1, 1, "FS_BATT_MAH", MainV2.comPort.MAV.param, pnlmah);
            }
            else
            {
                mavlinkNumericUpDownFS_BATT_MAH.setup(1000, 99999, 1, 1, "BATT_LOW_MAH", MainV2.comPort.MAV.param, pnlmah);
            }

            // removed at randys request
            //mavlinkCheckBoxfs_gps_enable.setup(1, 0, "FS_GPS_ENABLE", MainV2.comPort.MAV.param);
            mavlinkCheckBoxFS_GCS_ENABLE.setup(1, 0, "FS_GCS_ENABLE", MainV2.comPort.MAV.param);

            // plane
            mavlinkCheckBoxthr_fs.setup(1, 0, "THR_FAILSAFE", MainV2.comPort.MAV.param, mavlinkNumericUpDownthr_fs_value);
            mavlinkNumericUpDownthr_fs_value.setup(800, 1200, 1, 1, "THR_FS_VALUE", MainV2.comPort.MAV.param);
            mavlinkCheckBoxthr_fs_action.setup(1, 0, "THR_FS_ACTION", MainV2.comPort.MAV.param);
            mavlinkCheckBoxgcs_fs.setup(1, 0, "FS_GCS_ENABL", MainV2.comPort.MAV.param);
            mavlinkCheckBoxshort_fs.setup(1, 0, "FS_SHORT_ACTN", MainV2.comPort.MAV.param);
            mavlinkCheckBoxlong_fs.setup(1, 0, "FS_LONG_ACTN", MainV2.comPort.MAV.param);

            timer.Enabled = true;
            timer.Interval = 100;
            timer.Start();

            CustomMessageBox.Show("Ensure your props are not on the Plane/Quad", "FailSafe", MessageBoxButtons.OK,
                MessageBoxIcon.Exclamation);
        }

        public void Deactivate()
        {
            timer.Stop();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            // update all linked controls - 10hz
            try
            {
                MainV2.comPort.MAV.cs.UpdateCurrentSettings(currentStateBindingSource.UpdateDataSource(MainV2.comPort.MAV.cs));
            }
            catch
            {
            }
        }

        private void LNK_wiki_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (MainV2.comPort.MAV.cs.firmware == Firmwares.ArduCopter2)
            {
                Process.Start(new ProcessStartInfo("http://ardupilot.org/copter/docs/failsafe-landing-page.html"));
            }
            else
            {
                Process.Start(new ProcessStartInfo("http://ardupilot.org/plane/docs/advanced-failsafe-configuration.html"));
            }
        }

        private void lbl_armed_Paint(object sender, PaintEventArgs e)
        {
            lbl_armed.SuspendLayout();
            if (lbl_armed.Text == "True")
            {
                lbl_armed.Text = "Armed";
            }
            else if (lbl_armed.Text == "False")
            {
                lbl_armed.Text = "Disarmed";
            }
            lbl_armed.ResumeLayout();
        }

        private void lbl_gpslock_Paint(object sender, PaintEventArgs e)
        {
            var _gpsfix = 0;
            try
            {
                if (!int.TryParse(lbl_gpslock.Text, out _gpsfix))
                    return;
            }
            catch
            {
                return;
            }
            var gps = "";

            if (_gpsfix == 0)
            {
                gps = ("GPS: No GPS");
            }
            else if (_gpsfix == 1)
            {
                gps = ("GPS: No Fix");
            }
            else if (_gpsfix == 2)
            {
                gps = ("GPS: 3D Fix");
            }
            else if (_gpsfix == 3)
            {
                gps = ("GPS: 3D Fix");
            }
            lbl_gpslock.SuspendLayout();
            lbl_gpslock.Text = gps;
            lbl_gpslock.ResumeLayout();
        }

        private void lbl_currentmode_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (MainV2.comPort.MAV.param.ContainsKey("FS_THR_VALUE"))
                {
                    if (MainV2.comPort.MAV.cs.ch3in < (float) MainV2.comPort.MAV.param["FS_THR_VALUE"])
                    {
                        lbl_currentmode.ForeColor = Color.Red;
                    }
                    else
                    {
                        lbl_currentmode.ForeColor = Color.White;
                    }
                }
            }
            catch
            {
            }
        }
    }
}