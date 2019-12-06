using System;
using System.Windows.Forms;

namespace MissionPlanner.Joystick
{
    public partial class Joy_Do_Repeat_Relay : Form
    {
        public Joy_Do_Repeat_Relay(string name)
        {
            InitializeComponent();

            Utilities.ThemeManager.ApplyThemeTo(this);

            this.Tag = name;

            var config = MainV2.joystick.getButton(int.Parse(name));

            numericUpDown1.Text = config.p1.ToString();
            numericUpDown2.Text = config.p2.ToString();
            numericUpDown3.Text = config.p3.ToString();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            var config = MainV2.joystick.getButton(int.Parse(this.Tag.ToString()));

            config.p1 = (float) numericUpDown1.Value;
            config.p2 = (float) numericUpDown2.Value;
            config.p3 = (float) numericUpDown3.Value;

            MainV2.joystick.setButton(int.Parse(this.Tag.ToString()), config);
        }
    }
}