using log4net;
using MissionPlanner.Utilities;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace MissionPlanner.Controls
{
    public partial class Loading : Form
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static Loading Instance;

        static object locker = new object();

        public Loading()
        {
            InitializeComponent();
        }

        ~Loading()
        {
            Instance = null;
        }

        public new string Text { get; set; }

        public new static void Close()
        {
            log.Info("Loading.Close()");
            lock (locker)
            {
                if (Instance != null)
                {
                    if (!Instance.IsDisposed)
                    {
                        if (Instance.IsHandleCreated)
                        {
                            MainV2.instance.BeginInvoke((MethodInvoker)delegate
                           {
                               if (Instance == null)
                                   return;
                               uiSemaphoreSlim.Wait();
                               try
                               {
                                   ((Form)Instance).Close();
                               }
                               finally
                               {
                                   uiSemaphoreSlim.Release();
                               }
                               Instance = null;
                           });
                        }
                    }
                }
            }
        }

        static SemaphoreSlim uiSemaphoreSlim = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Create a new dialog or use an existing one if its still valid
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static void ShowLoading(string Text, IWin32Window owner = null)
        {
            log.Info(Text);
            // create form on ui thread
            MainV2.instance.BeginInvokeIfRequired((Action)delegate
           {
               uiSemaphoreSlim.Wait();
               try
               {
                   if (Instance != null && !Instance.IsDisposed)
                   {
                       Instance.Text = Text;
                       return;
                   }

                   log.Info("Create Instance");

                   Loading frm = new Loading();
                   if (owner == null)
                       frm.TopMost = true;
                   frm.StartPosition = FormStartPosition.CenterParent;
                   frm.Closing += Frm_Closing;

                    // set instance
                    Instance = frm;
                    // set text
                    Instance.label1.Text = Text;

                   ThemeManager.ApplyThemeTo(frm);
                   frm.Show(owner);
                   frm.Focus();
               }
               finally
               {
                   uiSemaphoreSlim.Release();
               }
           });
        }

        private static void Frm_Closing(object sender, CancelEventArgs e)
        {
            Instance = null;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label1.Text = Text;
        }
    }
}
