using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MissionPlanner.Controls.Waypoints
{
    public partial class Command : UserControl
    {
        public MAVLink.mavlink_mission_item_t missionitem { get; set; }

        public MAVLink.MAV_CMD MissionCommand { get { return (MAVLink.MAV_CMD)CMB_missioncommand.SelectedValue; } set { CMB_missioncommand.SelectedValue = value; } }

        public double param1
        {
            get
            {
                return double.Parse(txt_param1.Text);
            }
            set
            {
                txt_param1.Text = value.ToString();
            }
        }
        public double param2
        {
            get
            {
                return double.Parse(txt_param2.Text);
            }
            set
            {
                txt_param2.Text = value.ToString();
            }
        }
        public double param3
        {
            get
            {
                return double.Parse(txt_param3.Text);
            }
            set
            {
                txt_param3.Text = value.ToString();
            }
        }
        public double param4
        {
            get
            {
                return double.Parse(txt_param4.Text);
            }
            set
            {
                txt_param4.Text = value.ToString();
            }
        }
        public double x
        {
            get
            {
                return double.Parse(txtx.Text);
            }
            set
            {
                txtx.Text = value.ToString();
            }
        }
        public double y
        {
            get
            {
                return double.Parse(txty.Text);
            }
            set
            {
                txty.Text = value.ToString();
            }
        }
        public double z
        {
            get
            {
                return double.Parse(txtz.Text);
            }
            set
            {
                txtz.Text = value.ToString();
            }
        }


        public Command()
        {
            InitializeComponent();
        }

        private void BUT_del_Click(object sender, EventArgs e)
        {

        }

        private void BUT_UP_Click(object sender, EventArgs e)
        {

        }

        private void BUT_DOWN_Click(object sender, EventArgs e)
        {

        }


    }
}
