using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MissionPlanner.Joystick
{
    /// <summary>
    /// Settings for a joystick button mapped to an ArduPilot RCx_OPTION auxiliary function.
    /// Lets the user pick the function (filtered to the last connected vehicle) and how the
    /// button press/release maps onto the switch position sent via MAV_CMD_DO_AUX_FUNCTION.
    /// </summary>
    public partial class Joy_Aux_Function : Form
    {
        public Joy_Aux_Function(string name)
        {
            InitializeComponent();

            Utilities.ThemeManager.ApplyThemeTo(this);

            this.Tag = name;

            var firmware = MainV2.comPort.MAV.cs.firmware;
            this.Text = "Aux Function (" + firmware + ")";

            var config = MainV2.joystick.getButton(int.Parse(name));

            var functions = AuxFunction.GetList(firmware)
                .Select(a => new KeyValuePair<int, string>(a.Key, a.Key + ": " + a.Value))
                .ToList();

            // keep a saved option visible even if this vehicles metadata does not list it
            if (!functions.Any(a => a.Key == (int) config.p1))
                functions.Add(new KeyValuePair<int, string>((int) config.p1, (int) config.p1 + ": (unknown for " + firmware + ")"));

            comboBoxFunction.DataSource = functions;
            comboBoxFunction.ValueMember = "Key";
            comboBoxFunction.DisplayMember = "Value";

            comboBoxTrigger.DataSource = AuxFunction.GetTriggerList();
            comboBoxTrigger.ValueMember = "Key";
            comboBoxTrigger.DisplayMember = "Value";

            comboBoxFunction.SelectedValue = (int) config.p1;
            comboBoxTrigger.SelectedValue = (int) config.p2;
            if (comboBoxTrigger.SelectedIndex == -1)
            {
                // unknown or legacy trigger value: fall back to the default and save it, otherwise
                // the button would stay invalid and send nothing after this dialog closes
                comboBoxTrigger.SelectedValue = (int) auxfunctiontrigger.HighOnPress;
                config.function = buttonfunction.Aux_Function;
                config.p2 = (int) auxfunctiontrigger.HighOnPress;
                MainV2.joystick.setButton(int.Parse(name), config);
            }

            // attach after the initial selection so we do not rewrite the config while loading
            comboBoxFunction.SelectedValueChanged += comboBoxFunction_SelectedValueChanged;
            comboBoxTrigger.SelectedValueChanged += comboBoxTrigger_SelectedValueChanged;
        }

        private void comboBoxFunction_SelectedValueChanged(object sender, EventArgs e)
        {
            if (!(comboBoxFunction.SelectedValue is int))
                return;

            int name = int.Parse(this.Tag.ToString());
            var config = MainV2.joystick.getButton(name);

            config.function = buttonfunction.Aux_Function;
            config.p1 = (int) comboBoxFunction.SelectedValue;

            MainV2.joystick.setButton(name, config);
        }

        private void comboBoxTrigger_SelectedValueChanged(object sender, EventArgs e)
        {
            if (!(comboBoxTrigger.SelectedValue is int))
                return;

            int name = int.Parse(this.Tag.ToString());
            var config = MainV2.joystick.getButton(name);

            config.function = buttonfunction.Aux_Function;
            config.p2 = (int) comboBoxTrigger.SelectedValue;

            MainV2.joystick.setButton(name, config);
        }
    }
}
