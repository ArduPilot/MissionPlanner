using log4net;
using MissionPlanner.Controls;
using Newtonsoft.Json;
using System;
using System.Windows.Forms;

namespace MissionPlanner.Utilities
{
    public static class ExtensionsMP
    {
        public static void SetCheckboxFromConfig(this MyUserControl me, string configKey, System.Windows.Forms.CheckBox chk)
        {
            if (Settings.Instance[configKey] != null)
                chk.Checked = Settings.Instance.GetBoolean(configKey);
        }

        public static Action<T> UpdateDataSource<T>(this BindingSource ctl, T input)
        {
            return obj =>
            {
                if (ctl.DataSource != (object)input)
                    ctl.DataSource = input;
                ctl.ResetBindings(false);
            };
        }

        public static int GetPercent(this Control ctl, int current, bool height = false)
        {
            if (height)
            {
                return (int)((current / (double)ctl.Height) * 100.0);
            }
            else
            {
                return (int)((current / (double)ctl.Width) * 100.0);
            }
        }

        public static int GetPixel(this Control ctl, int current, bool height = false)
        {
            if (height)
            {
                return (int)((current / 100.0 * (double)ctl.Height));
            }
            else
            {
                return (int)((current / 100.0 * (double)ctl.Width));
            }
        }

        public static void LogInfoFormat(this Control ctl, string format, params object[] args)
        {
            ILog log = LogManager.GetLogger(ctl.GetType());

            log.InfoFormat(format, args);
        }

        public static void LogErrorFormat(this Control ctl, string format, params object[] args)
        {
            ILog log = LogManager.GetLogger(ctl.GetType());

            log.ErrorFormat(format, args);
        }

        public static void LogInfo(this Control ctl, object ex)
        {
            ILog log = LogManager.GetLogger(ctl.GetType());

            log.Info(ex);
        }

        public static void LogError(this Control ctl, object ex)
        {
            ILog log = LogManager.GetLogger(ctl.GetType());

            log.Error(ex);
        }

        public static Form ShowUserControl(this Control ctl, bool showit = true)
        {
            Form frm = new Form();
            int header = frm.Height - frm.ClientRectangle.Height;
            frm.Text = ctl.Text;
            frm.Size = ctl.Size;
            // add the header height
            frm.Height += header;
            frm.Tag = ctl;
            ctl.Dock = DockStyle.Fill;
            ctl.SizeChanged += Ctl_SizeChanged;
            ctl.Tag = frm;
            frm.MinimumSize = ctl.MinimumSize;
            frm.MaximumSize = ctl.MaximumSize;
            frm.Controls.Add(ctl);
            frm.Load += Frm_Load;
            frm.Closing += Frm_Closing;
            ThemeManager.ApplyThemeTo(frm);
            if (showit)
                frm.Show();

            return frm;
        }

        private static void Ctl_SizeChanged(object sender, EventArgs e)
        {
            var ctl = (sender as Control);
            if (ctl == null)
                return;
            var frm = ctl.Tag as Form;
            if (frm == null)
                return;
            if (frm.WindowState == FormWindowState.Normal)
                frm.ClientSize = ctl.ClientSize;
        }

        private static void Frm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (((Form)sender).Tag is MissionPlanner.Controls.IDeactivate)
            {
                ((MissionPlanner.Controls.IDeactivate)((Form)sender).Tag).Deactivate();
            }

            if (((Form)sender).Tag is MyUserControl)
            {
                (((Form)sender).Tag as MyUserControl).Close();
            }
        }

        private static void Frm_Load(object sender, EventArgs e)
        {
            if (((Form)sender).Tag is MissionPlanner.Controls.IActivate)
            {
                ((MissionPlanner.Controls.IActivate)((Form)sender).Tag).Activate();
            }
        }

        public static string Serialize(this DataGridView myDataGridView)
        {
            // row -1 as new blank row is counted
            object[,] dataGridViewObjectsArray = new object[myDataGridView.Rows.Count-1, myDataGridView.Columns.Count];
            for (int x = 0; x < myDataGridView.Rows.Count-1; x++)
                for (int y = 0; y < myDataGridView.Columns.Count; y++)
                    dataGridViewObjectsArray[x, y] = myDataGridView.Rows[x].Cells[y].Value;

            return dataGridViewObjectsArray.ToJSON();
        }

        public static void Deserialize(this DataGridView myDataGridView, string input)
        {
            if (input == null)
                return;
            var dataGridViewObjectsArray = JsonConvert.DeserializeObject<object[,]>(input);
            for (int x = 0; x < dataGridViewObjectsArray.GetLength(0); x++)
            {
                myDataGridView.Rows.Add();
                for (int y = 0; y < dataGridViewObjectsArray.GetLength(1); y++)
                    myDataGridView.Rows[x].Cells[y].Value = dataGridViewObjectsArray[x, y];
            }
        }
    }
}
