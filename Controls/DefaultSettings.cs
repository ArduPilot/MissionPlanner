using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using MissionPlanner.Utilities;
using log4net;

namespace MissionPlanner.Controls
{
    public partial class DefaultSettings : UserControl, IActivate
    {
        private static readonly ILog log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        List<GitHubContent.FileInfo> paramfiles;

        public event EventHandler OnChange;

        public DefaultSettings()
        {
            InitializeComponent();
        }

        public void Activate()
        {
            CMB_paramfiles.Enabled = false;
            BUT_paramfileload.Enabled = false;

            System.Threading.ThreadPool.QueueUserWorkItem(updatedefaultlist);
        }

        void updatedefaultlist(object crap)
        {
            try
            {
                if (paramfiles == null)
                {
                    paramfiles = GitHubContent.GetDirContent("ardupilot", "ardupilot", "/Tools/Frame_params/", ".param");
                }

                this.BeginInvoke((Action) delegate
                {
                    CMB_paramfiles.DataSource = paramfiles.ToArray();
                    CMB_paramfiles.DisplayMember = "name";
                    CMB_paramfiles.Enabled = true;
                    BUT_paramfileload.Enabled = true;
                });
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private void BUT_paramfileload_Click(object sender, EventArgs e)
        {
            string filepath = Settings.GetUserDataDirectory() + CMB_paramfiles.Text;

            if (CMB_paramfiles.SelectedValue == null)
            {
                CustomMessageBox.Show("Please select an option first");
                return;
            }

            try
            {
                byte[] data = GitHubContent.GetFileContent("ardupilot", "ardupilot",
                    ((GitHubContent.FileInfo) CMB_paramfiles.SelectedValue).path);

                File.WriteAllBytes(filepath, data);

                var param2 = Utilities.ParamFile.loadParamFile(filepath);

                Form paramCompareForm = new ParamCompare(null, MainV2.comPort.MAV.param, param2);

                ThemeManager.ApplyThemeTo(paramCompareForm);
                if (paramCompareForm.ShowDialog() == DialogResult.OK)
                {
                    CustomMessageBox.Show("Loaded parameters!", "Loaded");
                }

                if (OnChange != null)
                {
                    OnChange(null, null);
                    return;
                }

                this.Activate();
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Failed to load file.\n" + ex);
            }
        }

        private void ConfigDefaultSettings_Load(object sender, EventArgs e)
        {
            Activate();
        }
    }
}