using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ArdupilotMega.Comms;

namespace ArdupilotMega
{
    public partial class SerialOutputPass: Form
    {
        public SerialOutputPass()
        {
            InitializeComponent();

            CMB_serialport.DataSource = SerialPort.GetPortNames();

            if (MainV2.comPort.MirrorStream != null && MainV2.comPort.MirrorStream.IsOpen)
            {
                BUT_connect.Text = "Stop";
            }
        }

        private void BUT_connect_Click(object sender, EventArgs e)
        {
            if (MainV2.comPort.MirrorStream != null && MainV2.comPort.MirrorStream.IsOpen)
            {
                MainV2.comPort.MirrorStream.Close();
                BUT_connect.Text = "Connect";
            }
            else
            {
                try
                {
                    if (MainV2.comPort.MirrorStream == null)
                        MainV2.comPort.MirrorStream = new SerialPort();

                    MainV2.comPort.MirrorStream.PortName = CMB_serialport.Text;
                }
                catch { CustomMessageBox.Show("Invalid PortName"); return; }
                try {
                    MainV2.comPort.MirrorStream.BaudRate = int.Parse(CMB_baudrate.Text);
                } catch {CustomMessageBox.Show("Invalid BaudRate"); return;}
                try {
                    MainV2.comPort.MirrorStream.Open();
                } catch {CustomMessageBox.Show("Error Connecting\nif using com0com please rename the ports to COM??"); return;}
            }
        }
    }
}
