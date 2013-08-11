using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ArdupilotMega.Controls;
using ArdupilotMega.Controls.BackstageView;

namespace ArdupilotMega.Wizard
{
    public partial class _7BatteryMonitor : MyUserControl, IWizard, IActivate
    {
        List<sensor> sensorlist = new List<sensor>();
        bool startup = false;

        class sensor
        {
            public string Name = "";

            public float maxvolt = 0;
            public float maxamps = 0;
            public float mvpervolt = 0;
            public float mvperamp = 0;

            public float topvolt { get { return (maxvolt * mvpervolt) / 1000.0f; } }
            public float topamps { get { return (maxamps * mvperamp) / 1000.0f; } }

            public float voltspervolt { get { return (maxvolt / topvolt); } }
            public float ampspervolt { get { return (maxamps / topamps); } }

            public override string ToString() {
                return Name;
            }
        }

        public _7BatteryMonitor()
        {
            InitializeComponent();

            sensorlist.Add(new sensor()
            {
                Name = "None",
                maxvolt = 50f,
                maxamps = 90f,
                mvpervolt = 100f,
                mvperamp = 55.55f
            });

            sensorlist.Add(new sensor()
            {
                Name = "3DR Power Module",
                maxvolt = 50f,
                maxamps = 90f,
                mvpervolt = 100f,
                mvperamp = 55.55f
            });
            sensorlist.Add(new sensor()
            {
                Name = "Atto 45",
                maxvolt = 13.6f,
                maxamps = 44.7f,
                mvpervolt = 242.3f,
                mvperamp = 73.20f
            });
            sensorlist.Add(new sensor()
            {
                Name = "Atto 90",
                maxvolt = 50f,
                maxamps = 89.4f,
                mvpervolt = 63.69f,
                mvperamp = 36.60f
            });
            sensorlist.Add(new sensor()
            {
                Name = "Atto 180",
                maxvolt = 50f,
                maxamps = 178.8f,
                mvpervolt = 63.69f,
                mvperamp = 18.30f
            });

        }

        public void Activate()
        {
            startup = true;

            CMB_sensor.DataSource = sensorlist;

            txt_mah.Text = MainV2.comPort.param["BATT_CAPACITY"].ToString();

            startup = false;
        }

        public int WizardValidate()
        {
            try
            {
                float batterysize = float.Parse(txt_mah.Text);
                MainV2.comPort.setParam("BATT_CAPACITY", batterysize);
            }
            catch { CustomMessageBox.Show("Failed to set battery size, please check your input"); return 0; }

            return 1;
        }

        private void CMB_apmversion_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (startup)
                return;

            int selection = int.Parse(CMB_apmversion.Text.Substring(0, 1));

            try
            {
                if (selection == 0)
                {
                    // apm1
                    MainV2.comPort.setParam("BATT_VOLT_PIN", 0);
                    MainV2.comPort.setParam("BATT_CURR_PIN", 1);
                }
                else if (selection == 1)
                {
                    // apm2
                    MainV2.comPort.setParam("BATT_VOLT_PIN", 1);
                    MainV2.comPort.setParam("BATT_CURR_PIN", 2);
                }
                else if (selection == 2)
                {
                    //apm2.5
                    MainV2.comPort.setParam("BATT_VOLT_PIN", 13);
                    MainV2.comPort.setParam("BATT_CURR_PIN", 12);
                }
                else if (selection == 3)
                {
                    //px4
                    MainV2.comPort.setParam("BATT_VOLT_PIN", 100);
                    MainV2.comPort.setParam("BATT_CURR_PIN", 101);
                }
            }
            catch { CustomMessageBox.Show("Set BATT_????_PIN Failed"); }
        }

        private void CMB_sensor_SelectedIndexChanged(object sender, EventArgs e)
        {
            sensor sensorselected = ((sensor)((ComboBox)sender).SelectedValue);

            if (sensorselected.Name != "None")
            {

                MainV2.comPort.setParam("AMP_PER_VOLT", sensorselected.ampspervolt);

                MainV2.comPort.setParam("VOLT_DIVIDER", sensorselected.voltspervolt);

                // enable volt and current
                MainV2.comPort.setParam("BATT_MONITOR", 4);
            }
            else
            {
                // disable volt and current
                MainV2.comPort.setParam("BATT_MONITOR", 0);
            }
        }
    }
}
