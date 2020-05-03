using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MissionPlanner.Controls
{
    public partial class DevopsUI : UserControl
    {
        public DevopsUI()
        {
            InitializeComponent();
        }

        private void but_doit_Click(object sender, EventArgs e)
        {
            var buffer = new byte[Convert.ToByte(num_count.Text)];
            MainV2.comPort.device_op(Convert.ToByte(num_sysid.Text), Convert.ToByte(num_compid.Text), out buffer, 
                dom_bustype.Text == "SPI" ? MAVLink.DEVICE_OP_BUSTYPE.SPI : MAVLink.DEVICE_OP_BUSTYPE.I2C,
                txt_spiname.Text, Convert.ToByte(num_busno.Text), Convert.ToByte(num_address.Text), 
                Convert.ToByte(num_regstart.Text), Convert.ToByte(num_count.Text));

            textBox1.AppendText(buffer.Select(a => a.ToString("X2")).Aggregate((a, b) => a + b) + "\r\n");
        }

        private void dom_bustype_SelectedItemChanged(object sender, EventArgs e)
        {
            if(dom_bustype.Text == "SPI")
            {
                txt_spiname.Enabled = true;

                num_busno.Enabled = false;
                num_address.Enabled = false;

            } else
            {
                num_busno.Enabled = true;
                num_address.Enabled = true;

                txt_spiname.Enabled = false;
            }
        }

        private void but_test_Click(object sender, EventArgs e)
        {
            var buffer = new byte[2];

            MainV2.comPort.device_op(1, 1, out buffer, MAVLink.DEVICE_OP_BUSTYPE.SPI, txt_spiname.Text, 0, 0, 0xff, 2, new byte[] { 0x72, 0x00 });

            MainV2.comPort.device_op(1, 1, out buffer, MAVLink.DEVICE_OP_BUSTYPE.SPI, txt_spiname.Text, 0, 0, 0xff, 2);

            
            textBox1.AppendText(buffer.Select(a => a.ToString("X2")).Aggregate((a, b) => a + b) + "\r\n");
        }
    }
}
