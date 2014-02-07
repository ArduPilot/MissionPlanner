﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MissionPlanner.Controls;
using System.IO;
using MissionPlanner.Utilities;
using System.Collections;
using log4net;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigDefaultSettings : UserControl, IActivate
    {
        private static readonly ILog log =
         LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        List<GitHubContent.FileInfo> paramfiles;

        public ConfigDefaultSettings()
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
                    paramfiles = GitHubContent.GetDirContent("diydrones", "ardupilot", "/Tools/Frame_params/",".param");
                }

                this.BeginInvoke((Action)delegate
                {
                    CMB_paramfiles.DataSource = paramfiles.ToArray();
                    CMB_paramfiles.DisplayMember = "name";
                    CMB_paramfiles.Enabled = true;
                    BUT_paramfileload.Enabled = true;
                });
            }
            catch (Exception ex) { log.Error(ex); }
        }

        private void BUT_paramfileload_Click(object sender, EventArgs e)
        {
            string filepath = Application.StartupPath + Path.DirectorySeparatorChar + CMB_paramfiles.Text;

            byte[] data = GitHubContent.GetFileContent("diydrones", "ardupilot", ((GitHubContent.FileInfo)CMB_paramfiles.SelectedValue).path);

            File.WriteAllBytes(filepath, data);

            Hashtable param2 = Utilities.ParamFile.loadParamFile(filepath);

            Form paramCompareForm = new ParamCompare(null, MainV2.comPort.MAV.param, param2);

            ThemeManager.ApplyThemeTo(paramCompareForm);
            paramCompareForm.ShowDialog();

            CustomMessageBox.Show("Loaded parameters!", "Loaded");

            this.Activate();
        }
    }
}
