using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IronPython.Runtime;
using MissionPlanner.Utilities;

namespace MissionPlanner.Controls
{
    public partial class Loading : Form
    {
        static Loading Instance;

        public Loading()
        {
            InitializeComponent();
        }

        public string Text 
        {
            get { return label1.Text; }
            set
            {
                label1.Text = value;
                if (this.IsHandleCreated)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        Application.DoEvents();
                    });
                }
            }
        }

        public static void Close()
        {
            if (Instance != null)
            {
                if (!Instance.IsDisposed)
                {
                    if (Instance.IsHandleCreated)
                    {
                        ((Form) Instance).Close();
                        Instance = null;
                    }
                }
            }
        }

        /// <summary>
        /// Create a new dialog or use an existing one if its still valid
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static Loading ShowLoading(string Text, IWin32Window owner = null)
        {
            if (Instance != null && !Instance.IsDisposed)
            {
                Instance.Text = Text;
                return Instance;
            }

            Loading frm = new Loading();
            frm.Text = Text;
            frm.TopMost = true;

            ThemeManager.ApplyThemeTo(frm);

            MainV2.instance.Invoke((MethodInvoker) delegate {
                frm.Show(owner);
                Application.DoEvents();
            });

            Instance = frm;

            return frm;
        }
    }
}
