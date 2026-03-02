using MissionPlanner.Utilities;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MissionPlanner.Joystick
{
    public partial class Joy_Mount_Mode : Form
    {
        public Joy_Mount_Mode(string name)
        {
            InitializeComponent();

            Utilities.ThemeManager.ApplyThemeTo(this);

            this.Tag = name;

            var mnt_mode_paramnames = new List<string> { "MNT1_DEFLT_MODE", "MNT_DEFLT_MODE", "MNT_MODE" };
            foreach (var paramname in mnt_mode_paramnames)
            {
                var item = ParameterMetaDataRepository.GetParameterOptionsInt(paramname, MainV2.comPort.MAV.cs.firmware.ToString());
                if (item.Count > 0)
                {
                    comboBox1.DataSource = item;
                    comboBox1.DisplayMember = "Value";
                    comboBox1.ValueMember = "Key";
                    break;
                }
            }

            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);

            var config = MainV2.joystick.getButton(int.Parse(name));

            comboBox1.SelectedValue = (int)config.p1;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // get the index
            int name = int.Parse(this.Tag.ToString());

            // get existing config
            var config = MainV2.joystick.getButton(name);

            // change what we modified
            config.function = buttonfunction.Mount_Mode;
            config.p1 = (int)comboBox1.SelectedValue;

            // update entry
            MainV2.joystick.setButton(name, config);
        }
    }
}