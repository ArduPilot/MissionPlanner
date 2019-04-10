using System;
using System.Drawing;
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

            int idx = 0;
            for (int bitvalue = 1; bitvalue <= (int) MAVLink.EKF_STATUS_FLAGS.EKF_PRED_POS_HORIZ_ABS; bitvalue = bitvalue << 1)
            {
                int currentbit = (MainV2.comPort.MAV.cs.ekfflags & bitvalue);

                var currentflag = (MAVLink.EKF_STATUS_FLAGS) Enum.Parse(typeof (MAVLink.EKF_STATUS_FLAGS), bitvalue.ToString());

                if (flowLayoutPanel1.Controls.Count <= idx)
                {
                    flowLayoutPanel1.Controls.Add(new Label() {Height = 13, Width = flowLayoutPanel1.Width});
                }
                
                flowLayoutPanel1.Controls[idx].Text = currentflag.ToString().Replace("EKF_", "").ToLower() + " " +
                                                         (currentbit > 0 ? "On " : "Off") + "\r\n";

                flowLayoutPanel1.Controls[idx].ForeColor = ForeColor;

                if ((currentflag == MAVLink.EKF_STATUS_FLAGS.EKF_VELOCITY_HORIZ ||
                     currentflag == MAVLink.EKF_STATUS_FLAGS.EKF_POS_HORIZ_ABS ||
                     currentflag == MAVLink.EKF_STATUS_FLAGS.EKF_POS_VERT_ABS) && currentbit == 0)
                {
                    flowLayoutPanel1.Controls[idx].ForeColor = Color.Red;
                }

                idx++;
            }
        }
    }
}