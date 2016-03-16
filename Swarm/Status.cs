using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MissionPlanner.Swarm
{
    public partial class Status : UserControl
    {
        public Label Armed
        {
            get { return this.lbl_armed; }
        }

        public Label GPS
        {
            get { return this.lbl_gps; }
        }

        public Label Mode
        {
            get { return this.lbl_mode; }
        }

        public Label MAV
        {
            get { return this.lbl_mav; }
        }

        public Label Guided
        {
            get { return this.lbl_guided; }
        }

        public Status()
        {
            InitializeComponent();
        }
    }
}