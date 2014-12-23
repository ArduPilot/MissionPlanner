using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MissionPlanner.Controls.BackstageView;
using MissionPlanner.Controls;
using log4net;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigHWBT : UserControl, IActivate
    {
        const float rad2deg = (float)(180 / Math.PI);
        const float deg2rad = (float)(1.0 / rad2deg);

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        

        public ConfigHWBT()
        {
            InitializeComponent();
        }

        public void Activate()
        {
            if (MainV2.comPort.BaseStream.IsOpen)
            {
                this.Enabled = false;
                return;
            }
            else
            {
                this.Enabled = true;
            }
        }

        Dictionary<int, int> baudmap = new Dictionary<int, int>
        {
            {57600 , 7},
            {38400 , 6},
            {9600 , 4},
            {19200 , 5},
            {115200 , 8},
            {1200 , 1},
            {2400 , 2},
            {4800 , 3},
        };

        private void BUT_btsettings_Click(object sender, EventArgs e)
        {
            string[] commands = new string[] 
            {
                "AT",
                "AT+VERSION",
                string.Format("AT+ROLE={0}\r\n",0),
                string.Format("AT+NAME{0}",txt_name.Text),
                string.Format("AT+NAME={0}\r\n",txt_name.Text),
                string.Format("AT+BAUD{0}",baudmap[int.Parse(cmb_baud.Text)]),
                string.Format("AT+BAUD={0}\r\n",cmb_baud.Text),
                string.Format("AT+PIN{0}",txt_pin.Text),
                string.Format("AT+PSWD={0}\r\n",txt_pin.Text),
                "AT+RESET"
            };

            bool pass = false;

            foreach (var baud in baudmap)
            {
                log.Info("Try baud " + baud);
                using (System.IO.Ports.SerialPort port = new System.IO.Ports.SerialPort(MainV2.comPortName, baud.Key))
                {
                    port.Open();

                    port.Write("AT");

                    System.Threading.Thread.Sleep(1100);

                    port.Write("\r\n");

                    System.Threading.Thread.Sleep(200);

                    string isok = port.ReadExisting();

                    if (isok.Contains("OK"))
                    {
                        log.Info("Valid Answer");

                        foreach (var cmd in commands)
                        {
                            log.Info("Sending " + cmd);
                            port.Write(cmd);
                            System.Threading.Thread.Sleep(1000);
                            log.Info("Resp "+port.ReadExisting());
                        }

                        pass = true;
                        break;
                    }
                    else
                    {
                        log.Info("No Answer");
                    }
                }
            }

            if (!pass)
                CustomMessageBox.Show(Strings.ErrorSettingParameter, Strings.ERROR);
            else
                CustomMessageBox.Show(Strings.ProgrammedOK);
        }
    }
}