using System;
using System.Drawing;
using System.Windows.Forms;

namespace MissionPlanner.Controls
{
    public partial class ControlSensorsStatus : UserControl
    {
        public ControlSensorsStatus()
        {
            InitializeComponent();

            var names = Enum.GetNames(typeof (MAVLink.MAV_SYS_STATUS_SENSOR));

            tableLayoutPanel1.ColumnCount = names.Length;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            update();

            timer1.Start();
        }

        public void update()
        {
            var names = Enum.GetNames(typeof (MAVLink.MAV_SYS_STATUS_SENSOR));

            tableLayoutPanel1.ColumnCount = names.Length;

            var a = 0;
            foreach (var name in names)
            {
                if (tableLayoutPanel1.GetControlFromPosition(0, a) != null)
                {
                    continue;
                }

                tableLayoutPanel1.Controls.Add(
                    new Label() { Text = name.Replace("MAV_SYS_STATUS", "").Replace("_", " "), Font = new Font(Font.FontFamily, 5), Margin = new Padding(0), Padding = new Padding(0) }, 0, a);
                a++;
            }

            // enabled
            a = 0;
            var mask = 1;
            foreach (var name in names)
            {
                if ((MainV2.comPort.MAV.cs.sensors_enabled.Value & mask) > 0)
                {
                    updateLabel(1, a, "En", Color.Green);
                }
                else
                {
                    updateLabel(1, a, "Dis", Color.Red);
                }
                mask = mask << 1;
                a++;
            }

            // present
            a = 0;
            mask = 1;
            foreach (var name in names)
            {
                if ((MainV2.comPort.MAV.cs.sensors_present.Value & mask) > 0)
                {
                    updateLabel(2, a, "Present", Color.Green);
                }
                else
                {
                    updateLabel(2, a, "No", Color.Red);
                }
                mask = mask << 1;
                a++;
            }

            // present
            a = 0;
            mask = 1;
            foreach (var name in names)
            {
                if ((MainV2.comPort.MAV.cs.sensors_health.Value & mask) > 0)
                {
                    updateLabel(3, a, "Ok", Color.Green);
                }
                else
                {
                    updateLabel(3, a, "Bad", Color.Red);
                }
                mask = mask << 1;
                a++;
            }
        }

        private void updateLabel(int coloum, int row, string text, Color color)
        {
            var ctl = tableLayoutPanel1.GetControlFromPosition(coloum, row);

            if (ctl == null)
            {
                ctl = new Label() { Font = new Font(Font.FontFamily, 5), Margin = new Padding(0), Padding = new Padding(0) };
                tableLayoutPanel1.Controls.Add(ctl, coloum, row);
            }

            ctl.Text = text;
            if (color != null)
                ctl.BackColor = color;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            update();
        }
    }
}