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
    public partial class EKFStatus : Form
    {
        public EKFStatus()
        {
            InitializeComponent();

            Utilities.ThemeManager.ApplyThemeTo(this);

            timer1.Start();
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ekfvel.Value = (int) (MainV2.comPort.MAV.cs.ekfvelv*100);
            ekfposh.Value = (int) (MainV2.comPort.MAV.cs.ekfposhor*100);
            ekfposv.Value = (int) (MainV2.comPort.MAV.cs.ekfposvert*100);
            ekfcompass.Value = (int) (MainV2.comPort.MAV.cs.ekfcompv*100);
            ekfterrain.Value = (int) (MainV2.comPort.MAV.cs.ekfteralt*100);

            // restore colours
            Utilities.ThemeManager.ApplyThemeTo(this);

            foreach (var item in new VerticalProgressBar2[] {ekfvel, ekfposh, ekfposv, ekfcompass, ekfterrain})
            {
                if (item.Value > 50)
                    item.ValueColor = Color.Orange;

                if (item.Value > 80)
                    item.ValueColor = Color.Red;
            }

            label7.Text = "";

            for (int a = 1; a < (int) MAVLink.EKF_STATUS_FLAGS.ENUM_END; a = a << 1)
            {
                int currentbit = (MainV2.comPort.MAV.cs.ekfflags & a);

                var currentflag = (MAVLink.EKF_STATUS_FLAGS) Enum.Parse(typeof (MAVLink.EKF_STATUS_FLAGS), a.ToString());

                label7.Text += currentflag.ToString().Replace("EKF_", "").ToLower() + " " +
                               (currentbit > 0 ? "On " : "Off") + "\r\n";
            }
        }
    }
}