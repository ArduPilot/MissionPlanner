﻿using System;
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

        static object locker = new object();

        public Loading()
        {
            InitializeComponent();
        }

        public new string Text 
        {
            get { return label1.Text; }
            set
            {
                if (this.IsHandleCreated && !IsDisposed)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        label1.Text = value;
                        Application.DoEvents();
                    });
                }
            }
        }

        public new static void Close()
        {
            if (Instance != null)
            {
                if (!Instance.IsDisposed)
                {
                    if (Instance.IsHandleCreated)
                    {
                        Instance.Invoke((MethodInvoker)delegate
                        {
                            ((Form) Instance).Close();
                        });
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
            // ensure we only have one instance at a time
            lock (locker)
            {
                if (Instance != null && !Instance.IsDisposed)
                {
                    Instance.Text = Text;
                    return Instance;
                }

                Loading frm = new Loading();
                frm.TopMost = true;
                frm.StartPosition = FormStartPosition.CenterParent;
                frm.Closing += Frm_Closing;

                // set instance
                Instance = frm;
                // set text
                Instance.label1.Text = Text;

                ThemeManager.ApplyThemeTo(frm);

                // display on ui thread
                if (MainV2.instance.InvokeRequired)
                {
                    MainV2.instance.Invoke((MethodInvoker) delegate
                    {
                        frm.Show(owner);
                        Application.DoEvents();
                    });
                }
                else
                {
                    frm.Show(owner);
                    Application.DoEvents();
                }

                return frm;
            }
        }

        private static void Frm_Closing(object sender, CancelEventArgs e)
        {
            Instance = null;
        }
    }
}
