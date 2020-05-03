using System;
using System.Configuration;
using System.IO;
using MissionPlanner.Controls;
using System.Windows.Forms;
using MissionPlanner.Utilities;
using MissionPlanner.Warnings;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigAdvanced : MyUserControl, IActivate
    {
        public ConfigAdvanced()
        {
            InitializeComponent();
        }

        public void Activate()
        {
        }

        private void but_warningmanager_Click(object sender, System.EventArgs e)
        {
            new WarningsManager().Show();
        }

        private void but_mavinspector_Click(object sender, System.EventArgs e)
        {
            new MAVLinkInspector(MainV2.comPort).Show();
        }

        private void BUT_outputMavlink_Click(object sender, System.EventArgs e)
        {
            new SerialOutputPass().Show();
        }

        private void but_signkey_Click(object sender, System.EventArgs e)
        {
            new AuthKeys().Show();
        }

        private void but_proximity_Click(object sender, System.EventArgs e)
        {
            new ProximityControl(MainV2.comPort.MAV).Show();
        }

        private void BUT_outputnmea_Click(object sender, System.EventArgs e)
        {
            new SerialOutputNMEA().Show();
        }

        private void BUT_follow_me_Click(object sender, System.EventArgs e)
        {
            new FollowMe().Show();
        }

        private void BUT_paramgen_Click(object sender, System.EventArgs e)
        {
            ProgressReporterDialogue prd = new ProgressReporterDialogue();

            prd.DoWork += dialogue =>
            {
                prd.UpdateProgressAndStatus(-1, "Downloading updated data");
                ParameterMetaDataParser.GetParameterInformation(
                    ConfigurationManager.AppSettings["ParameterLocationsBleeding"] + ";" +
                    ConfigurationManager.AppSettings["ParameterLocations"] + ";"
                    + "https://raw.githubusercontent.com/ArduPilot/ardupilot/Copter-3.6/ArduCopter/Parameters.cpp;" +
                    "https://raw.githubusercontent.com/ArduPilot/ardupilot/Copter-3.5/ArduCopter/Parameters.cpp;" +
                    "https://raw.githubusercontent.com/ArduPilot/ardupilot/plane3.9/ArduCopter/Parameters.cpp;" +
                    "https://raw.githubusercontent.com/ArduPilot/ardupilot/plane3.8/ArduCopter/Parameters.cpp;");

                ParameterMetaDataRepositoryAPM.Reload();
            };

            prd.doWorkArgs.CancelRequestChanged += (sender2, args) => { prd.doWorkArgs.CancelAcknowledged = true; };

            prd.doWorkArgs.ForceExit = true;

            prd.RunBackgroundOperationAsync();
        }

        private void BUT_movingbase_Click(object sender, System.EventArgs e)
        {
            new MovingBase().Show();
        }

        private void but_anonlog_Click(object sender, System.EventArgs e)
        {
            CustomMessageBox.Show("This is beta, please confirm the output file");
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "tlog or bin/log|*.tlog;*.bin;*.log";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    var ext = Path.GetExtension(ofd.FileName).ToLower();
                    if (ext == ".bin")
                        ext = ".log";
                    using (SaveFileDialog sfd = new SaveFileDialog())
                    {
                        sfd.DefaultExt = ext;
                        sfd.FileName = Path.GetFileNameWithoutExtension(ofd.FileName) + "-anon" + ext;
                        if (sfd.ShowDialog() == DialogResult.OK)
                        {
                            Privacy.anonymise(ofd.FileName, sfd.FileName);
                        }
                    }
                }
            }
        }
    }
}