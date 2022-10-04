using MissionPlanner.Utilities;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MissionPlanner.Controls
{
    public partial class AuxOptions : UserControl
    {
        static int index = 0;
        public int thisaux;
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            try
            {
                var paramname = "RC6_OPTION";
                var source = ParameterMetaDataRepository.GetParameterOptionsInt(paramname, MainV2.comPort.MAV.cs.firmware.ToString());

                mavlinkComboBox1.SelectedValueChanged -= MavlinkComboBox1_SelectedValueChanged;

                mavlinkComboBox1.DisplayMember = "Value";
                mavlinkComboBox1.ValueMember = "Key";
                mavlinkComboBox1.DataSource = source;

                mavlinkComboBox1.ParamName = paramname;
                mavlinkComboBox1.Name = paramname;

                mavlinkComboBox1.Enabled = true;
                mavlinkComboBox1.Visible = true;

                try
                {
                    mavlinkComboBox1.SelectedValue = int.Parse(Settings.Instance["Aux" + thisaux + "_desc", "0"]);
                }
                catch { }
                mavlinkComboBox1.SelectedValueChanged += MavlinkComboBox1_SelectedValueChanged;
            }
            catch { }
        }

        private void MavlinkComboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            Settings.Instance["Aux" + thisaux + "_desc"] = mavlinkComboBox1.SelectedValue.ToString();
        }

        public AuxOptions()
        {
            InitializeComponent();

            loadSettings();

            TXT_rcchannel.BackColor = Color.Gray;

            thisaux = index++;

            TXT_rcchannel.Text = thisaux.ToString();
        }

        void loadSettings()
        {
            string desc = Settings.Instance["Aux" + thisaux + "_desc"];
            string low = Settings.Instance["Aux" + thisaux + "_low"];
            string mid = Settings.Instance["Aux" + thisaux + "_mid"];
            string high = Settings.Instance["Aux" + thisaux + "_high"];

            string highdesc = Settings.Instance["Aux" + thisaux + "_highdesc"];
            string lowdesc = Settings.Instance["Aux" + thisaux + "_lowdesc"];

            if (!string.IsNullOrEmpty(low))
            {
                TXT_low_value.Text = low;
            }

            if (!string.IsNullOrEmpty(mid))
            {
                txt_midvalue.Text = mid;
            }

            if (!string.IsNullOrEmpty(high))
            {
                TXT_highvalue.Text = high;
            }

            if (!string.IsNullOrEmpty(desc))
            {
                TXT_rcchannel.Text = desc;
            }

            if (!string.IsNullOrEmpty(highdesc))
            {
                BUT_High.Text = highdesc;
            }

            if (!string.IsNullOrEmpty(lowdesc))
            {
                BUT_Low.Text = lowdesc;
            }
        }

        private void BUT_Low_Click(object sender, EventArgs e)
        {
            try
            {
                if (MainV2.comPort.doCommand((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, MAVLink.MAV_CMD.DO_AUX_FUNCTION, (int)mavlinkComboBox1.SelectedValue, int.Parse(TXT_low_value.Text), 0, 0,
                    0, 0, 0))
                {
                    TXT_rcchannel.BackColor = Color.Red;
                }
                else
                {
                    CustomMessageBox.Show(Strings.CommandFailed, Strings.ERROR);
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(Strings.CommandFailed + ex.ToString(), Strings.ERROR);
            }
        }

        private void BUT_High_Click(object sender, EventArgs e)
        {
            try
            {
                if (MainV2.comPort.doCommand((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, MAVLink.MAV_CMD.DO_AUX_FUNCTION, (int)mavlinkComboBox1.SelectedValue, int.Parse(TXT_highvalue.Text), 0, 0,
                    0, 0, 0))
                {
                    TXT_rcchannel.BackColor = Color.Green;
                }
                else
                {
                    CustomMessageBox.Show(Strings.CommandFailed, Strings.ERROR);
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(Strings.CommandFailed + ex.ToString(), Strings.ERROR);
            }
        }
        private void TXT_pwm_low_TextChanged(object sender, EventArgs e)
        {
            Settings.Instance["Aux" + thisaux + "_low"] = TXT_low_value.Text;
        }

        private void TXT_pwm_high_TextChanged(object sender, EventArgs e)
        {
            Settings.Instance["Aux" + thisaux + "_high"] = TXT_highvalue.Text;
        }

        private void But_mid_Click(object sender, EventArgs e)
        {
            try
            {
                if (MainV2.comPort.doCommand((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, MAVLink.MAV_CMD.DO_AUX_FUNCTION, (int)mavlinkComboBox1.SelectedValue, (int.Parse(TXT_highvalue.Text) - int.Parse(TXT_low_value.Text)) / 2 + int.Parse(TXT_low_value.Text), 0, 0,
                    0, 0, 0))
                {
                    TXT_rcchannel.BackColor = Color.Orange;
                }
                else
                {
                    CustomMessageBox.Show(Strings.CommandFailed, Strings.ERROR);
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(Strings.CommandFailed + ex.ToString(), Strings.ERROR);
            }
        }

        private void txt_midvalue_TextChanged(object sender, EventArgs e)
        {
            Settings.Instance["Aux" + thisaux + "_mid"] = txt_midvalue.Text;
        }
    }
}