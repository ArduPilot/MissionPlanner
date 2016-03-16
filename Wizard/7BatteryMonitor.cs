using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MissionPlanner.Controls;
using MissionPlanner.Controls.BackstageView;

namespace MissionPlanner.Wizard
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

            public float topvolt
            {
                get { return (maxvolt*mvpervolt)/1000.0f; }
            }

            public float topamps
            {
                get { return (maxamps*mvperamp)/1000.0f; }
            }

            public float voltspervolt
            {
                get { return (maxvolt/topvolt); }
            }

            public float ampspervolt
            {
                get { return (maxamps/topamps); }
            }

            public override string ToString()
            {
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
                mvpervolt = 101f,
                mvperamp = 55.55f
            });

            sensorlist.Add(new sensor()
            {
                Name = "3DR 4in 1 ESC",
                maxvolt = 39.67f,
                maxamps = 56.1f,
                mvpervolt = 83.19f,
                mvperamp = 58.82f

                //Sensor 3DR 4in 1 ESC with 

                //AMP_PER_VOLT: 17
//VOLT_DIVIDER: 12.02
            });

            sensorlist.Add(new sensor()
            {
                Name = "3DR Power Module",
                maxvolt = 50f,
                maxamps = 90f,
                mvpervolt = 99f,
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
            try
            {
                if (!MainV2.comPort.MAV.param.ContainsKey("BATT_CAPACITY"))
                {
                    CustomMessageBox.Show("Missing BATT_CAPACITY param, something is wrong.", Strings.ERROR);
                    MainV2.comPort.getParamList();
                }

                txt_mah.Text = MainV2.comPort.MAV.param["BATT_CAPACITY"].ToString();
            }
            catch
            {
                Console.WriteLine("no BATT_CAPACITY param");
                this.Close();
            }

            switch (MainV2.comPort.MAV.Product_ID)
            {
                case Common.ap_product.AP_PRODUCT_ID_APM1_1280:
                    CMB_apmversion.SelectedIndex = 0;
                    break;
                case Common.ap_product.AP_PRODUCT_ID_APM1_2560:
                    CMB_apmversion.SelectedIndex = 0;
                    break;
                case Common.ap_product.AP_PRODUCT_ID_PX4:
                    CMB_apmversion.SelectedIndex = 3;
                    break;
                case Common.ap_product.AP_PRODUCT_ID_PX4_V2:
                    CMB_apmversion.SelectedIndex = 4;
                    break;
                case Common.ap_product.AP_PRODUCT_ID_SITL:
                    CMB_apmversion.SelectedIndex = 1;
                    break;
                default:
                    if (MainV2.comPort.MAV.Product_ID >= Common.ap_product.AP_PRODUCT_ID_APM2_REV_C4 &&
                        MainV2.comPort.MAV.Product_ID <= Common.ap_product.AP_PRODUCT_ID_APM2_REV_D9)
                        CMB_apmversion.SelectedIndex = 2;
                    break;
            }

            startup = false;
        }

        public int WizardValidate()
        {
            try
            {
                float batterysize = float.Parse(txt_mah.Text);
                if (!MainV2.comPort.setParam("BATT_CAPACITY", batterysize))
                    throw new Exception("BATT_CAPACITY Not Set");
            }
            catch
            {
                CustomMessageBox.Show("Failed to set battery size, please check your input");
                return 0;
            }

            return 1;
        }

        public bool WizardBusy()
        {
            return false;
        }

        private void CMB_apmversion_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (startup)
                return;

            if (CMB_apmversion.Text.Length == 0)
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
                else if (selection == 4)
                {
                    //pixhawk
                    MainV2.comPort.setParam("BATT_VOLT_PIN", 2);
                    MainV2.comPort.setParam("BATT_CURR_PIN", 3);
                }
            }
            catch
            {
                CustomMessageBox.Show("Set BATT_????_PIN Failed");
            }
        }

        private void CMB_sensor_SelectedIndexChanged(object sender, EventArgs e)
        {
            sensor sensorselected = ((sensor) ((ComboBox) sender).SelectedValue);

            try
            {
                if (sensorselected.Name != "None")
                {
                    MainV2.comPort.setParam("BATT_AMP_PERVOLT", sensorselected.ampspervolt);

                    MainV2.comPort.setParam("BATT_VOLT_MULT", sensorselected.voltspervolt);

                    // enable volt and current
                    MainV2.comPort.setParam("BATT_MONITOR", 4);
                }
                else
                {
                    // disable volt and current
                    MainV2.comPort.setParam("BATT_MONITOR", 0);
                }
            }
            catch
            {
                CustomMessageBox.Show("Set BATT_MONITOR Failed");
            }
        }
    }
}