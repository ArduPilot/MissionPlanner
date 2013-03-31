using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ArdupilotMega.Controls
{
    public partial class Firmware_Board : Form
    {
        public static Firmware fw = Firmware.apm2_5;

        public Firmware_Board()
        {
            InitializeComponent();

            imageLabelapm1.Text = "APM 1";
            imageLabelapm2.Text = "APM 2";
            imageLabelapm2_5.Text = "APM 2.5";
            imageLabelpx4.Text = "PX4";

            ArdupilotMega.Utilities.ThemeManager.ApplyThemeTo(this);
        }

        public enum Firmware
        {
            apm1,
            apm2,
            apm2_5,
            px4
        }

        private void imageLabelapm1_Click(object sender, EventArgs e)
        {
            fw = Firmware.apm1;
            this.Close();
        }

        private void imageLabelapm2_Click(object sender, EventArgs e)
        {
            fw = Firmware.apm2;
            this.Close();
        }

        private void imageLabelapm2_5_Click(object sender, EventArgs e)
        {
            fw = Firmware.apm2_5;
            this.Close();
        }

        private void imageLabelpx4_Click(object sender, EventArgs e)
        {
            fw = Firmware.px4;
            this.Close();
        }
    }
}
