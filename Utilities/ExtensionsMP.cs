using log4net;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using MissionPlanner.test;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WinForms;

namespace MissionPlanner.Utilities
{
    public static class ExtensionsMP
    {
        public static void Stop(this System.Threading.Timer timer)
        {
            timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public static void Start(this System.Threading.Timer timer, int intervalms)
        {
            timer.Change(intervalms, intervalms);
        }

        public static Action<T> UpdateDataSource<T>(this BindingSource ctl, T input)
        {
            return obj =>
            {
                ctl.DataSource = input;
                ctl.ResetBindings(false);
            };
        }

        public static int GetPercent(this Control ctl, int current, bool height = false)
        {
            if (height)
            {
                return (int) ((current / (double) ctl.Height) * 100.0);
            }
            else
            {
                return (int) ((current / (double) ctl.Width) * 100.0);
            }
        }

        public static int GetPixel(this Control ctl, int current, bool height = false)
        {
            if (height)
            {
                return (int)((current/100.0 * (double)ctl.Height));
            }
            else
            {
                return (int)((current/100.0 * (double)ctl.Width) );
            }
        }

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

        public static Form ShowXamarinControl(this ContentPage ctl, int Width, int Height)
        {
            var f = new Xamarin.Forms.Platform.WinForms.PlatformRenderer();
            Xamarin.Forms.Platform.WinForms.Forms.Init(f);

            f.Width = Width;
            f.Height = Height;
            var app = new Xamarin.Forms.Application() {MainPage = ctl};
            f.LoadApplication(app);
            ThemeManager.ApplyThemeTo(f);
            if (ctl is IClose)
            {
                ((IClose)ctl).CloseAction = () => f.Close();
            }

            f.ShowDialog();

            return f;
        }


        public static Form ShowUserControl(this Control ctl)
        {
            Form frm = new Form();
            int header = frm.Height - frm.ClientRectangle.Height;
            frm.Text = ctl.Text;
            frm.Size = ctl.Size;
            // add the header height
            frm.Height += header;
            frm.Tag = ctl;
            ctl.Dock = DockStyle.Fill;
            frm.MinimumSize = ctl.MinimumSize;
            frm.MaximumSize = ctl.MaximumSize;
            frm.Controls.Add(ctl);
            frm.Load += Frm_Load;
            frm.Closing += Frm_Closing;
            frm.Show();

            return frm;
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
