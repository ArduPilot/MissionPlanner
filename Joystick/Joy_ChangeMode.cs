using System;
using System.Windows.Forms;

namespace MissionPlanner.Joystick
{
    public partial class Joy_ChangeMode : Form
    {
        public Joy_ChangeMode(string name)
        {
            InitializeComponent();

            Utilities.ThemeManager.ApplyThemeTo(this);

            this.Tag = name;

            comboBox1.DataSource = Common.getModesList(MainV2.comPort.MAV.cs.firmware);
            comboBox1.ValueMember = "Key";
            comboBox1.DisplayMember = "Value";

            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);

            var config = MainV2.joystick.getButton(int.Parse(name));

            comboBox1.Text = config.mode;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // get the index
            int name = int.Parse(this.Tag.ToString());

            // get existing config
            var config = MainV2.joystick.getButton(name);

            // change what we modified
            config.function = Joystick.buttonfunction.ChangeMode;
            config.mode = comboBox1.Text.ToString();

            // update entry
            MainV2.joystick.setButton(name, config);
        }
    }
}