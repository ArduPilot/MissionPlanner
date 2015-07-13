using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MissionPlanner.Keyboard
{
    public partial class Keyboard_Help : Form
    {
        public Keyboard_Help()
        {
            InitializeComponent();
            Utilities.ThemeManager.ApplyThemeTo(this);

            LBL_controls.Text = "- While disabled, it is possible to edit which key to use in each parameter of the keyboard controller" + Environment.NewLine +
                               Environment.NewLine + "- Click on the boxes to change the keyboard controller keys" + Environment.NewLine +
                               Environment.NewLine + "- Press Escape to cancel";
                               

            LBL_factors.Text = "- Higher sensitivity means you will be able to move that axis faster" + Environment.NewLine +
                                Environment.NewLine + "- When enabled, it is possible to press CTRL + any directional key (steer, roll or pitch) to reach max/min value";
                               
        
        }
    }
}
