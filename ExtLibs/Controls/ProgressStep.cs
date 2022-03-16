using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MissionPlanner.Controls
{
    public partial class ProgressStep : MyUserControl
    {
        public int Maximum { get { return _maximum; } set { _maximum = value; progressBar1.Maximum = value; this.Invalidate(); } }
        public int Step { 
            get { return _step; } 
            set { _step = value; progressBar1.Value = value; label1.Text = string.Format("Progress... {0} of {1}", value, _maximum); this.Invalidate(); } 
        }

        int _maximum = 0;
        int _step = 0;

        public ProgressStep()
        {
            InitializeComponent();

           this.BackColor = Color.Transparent;
        }
    }
}
