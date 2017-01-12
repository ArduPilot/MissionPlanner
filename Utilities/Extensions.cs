using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace MissionPlanner.Utilities
{
    public static class Extensions
    {
        public static void LogInfoFormat(this Control ctl, string format, params object[] args)
        {
            ILog log = LogManager.GetLogger(ctl.GetType().FullName);

            log.InfoFormat(format, args);
        }

        public static void LogErrorFormat(this Control ctl, string format, params object[] args)
        {
            ILog log = LogManager.GetLogger(ctl.GetType().FullName);

            log.ErrorFormat(format, args);
        }

        public static void LogInfo(this Control ctl, object ex)
        {
            ILog log = LogManager.GetLogger(ctl.GetType().FullName);

            log.Info(ex);
        }

        public static void LogError(this Control ctl, object ex)
        {
            ILog log = LogManager.GetLogger(ctl.GetType().FullName);

            log.Error(ex);
        }

        public static void ShowUserControl(this UserControl ctl)
        {
            Form frm = new Form();
            int header = frm.Height - frm.ClientRectangle.Height;
            frm.Text = ctl.Text;
            frm.Size = ctl.Size;
            // add the header height
            frm.Height += header;
            frm.Tag = ctl;
            ctl.Dock = DockStyle.Fill;
            frm.Controls.Add(ctl);
            frm.Load += Frm_Load;
            frm.Closing += Frm_Closing;
            frm.Show();
        }

        private static void Frm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (((Form)sender).Tag is MissionPlanner.Controls.IDeactivate)
            {
                ((MissionPlanner.Controls.IDeactivate)((Form)sender).Tag).Deactivate();
            }
        }

        private static void Frm_Load(object sender, EventArgs e)
        {
            if (((Form)sender).Tag is MissionPlanner.Controls.IActivate)
            {
                ((MissionPlanner.Controls.IActivate)((Form)sender).Tag).Activate();
            }
        }
    }
}
