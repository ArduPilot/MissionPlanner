using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
using MissionPlanner.Utilities;
using log4net;

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

                            MainV2.instance.Invoke((MethodInvoker) delegate { ((Form) Instance).Close(); });

                            Instance = null;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Create a new dialog or use an existing one if its still valid
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static void ShowLoading(string Text, IWin32Window owner = null)
        {
            //if (MainV2.MONO)
            {
                log.Info(Text);
                //return;
            }

            // ensure we only have one instance at a time
            lock (locker)
            {
                if (Instance != null && !Instance.IsDisposed)
                {
                    Instance.Text = Text;
                    return;
                }

                log.Info("Create Instance");
                // create form on ui thread
                MainV2.instance.Invoke((MethodInvoker) delegate
                {
                    Loading frm = new Loading();
                    if(owner == null)
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
                });
            }
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
