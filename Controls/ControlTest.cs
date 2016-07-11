using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MissionPlanner.Controls
{
    public partial class ControlTest : Form
    {
        Control _ctl;

        public ControlTest(Control ctl)
        {
            InitializeComponent();

            _ctl = ctl;
            _ctl.Dock = DockStyle.Fill;
            Controls.Add(_ctl);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            _ctl.Refresh();
        }
    }
}
