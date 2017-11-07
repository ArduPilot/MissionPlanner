using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MissionPlanner.Models
{
    public class BattMonitor
    {
        public List<Sensor> sensorlist = new List<Sensor>();

        public List<PinNumbers> pinlist = new List<PinNumbers>();

        public List<MonitorModes> modeslist = new List<MonitorModes>();

        public BattMonitor()
        {
            sensorlist.Add(new Sensor()
            {
                Name = "Other",
                maxvolt = 50f,
                maxamps = 90f,
                mvpervolt = 0,
                mvperamp = 0
            });

            sensorlist.Add(new Sensor()
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

            sensorlist.Add(new Sensor()
            {
                Name = "3DR Power Module",
                maxvolt = 50f,
                maxamps = 90f,
                mvpervolt = 99f,
                mvperamp = 55.55f
            });
            sensorlist.Add(new Sensor()
            {
                Name = "Atto 45",
                maxvolt = 13.6f,
                maxamps = 44.7f,
                mvpervolt = 242.3f,
                mvperamp = 73.20f
            });
            sensorlist.Add(new Sensor()
            {
                Name = "Atto 90",
                maxvolt = 50f,
                maxamps = 89.4f,
                mvpervolt = 63.69f,
                mvperamp = 36.60f
            });
            sensorlist.Add(new Sensor()
            {
                Name = "Atto 180",
                maxvolt = 50f,
                maxamps = 178.8f,
                mvpervolt = 63.69f,
                mvperamp = 18.30f
            });


            pinlist.Add(new PinNumbers()
            {
                Name = "APM1",
                volpin = 0,
                curpin = 1
            });

            pinlist.Add(new PinNumbers()
            {
                Name = "APM2+ - Custom Sensor",
                volpin = 1,
                curpin = 2
            });
            pinlist.Add(new PinNumbers()
            {
                Name = "APM2+ - 3DR Power Module",
                volpin = 13,
                curpin = 12
            });
            pinlist.Add(new PinNumbers()
            {
                Name = "PX4",
                volpin = 100,
                curpin = 101
            });
            pinlist.Add(new PinNumbers()
            {
                Name = "PIXHAWK",
                volpin = 2,
                curpin = 3
            });

            modeslist.Add(new MonitorModes()
            {
                Name = "Disabled",
                value = 0
            });
            modeslist.Add(new MonitorModes()
            {
                Name = "Battery Volts",
                value = 3
            });
            modeslist.Add(new MonitorModes()
            {
                Name = "Voltage and Current",
                value = 4
            });
        }

        public void SetPins(PinNumbers pins)
        {
            MainV2.comPort.setParam("BATT_VOLT_PIN", pins.volpin);
            MainV2.comPort.setParam("BATT_CURR_PIN", pins.curpin);
        }

        public void SetSensor(Sensor sensorselected)
        {
            MainV2.comPort.setParam("BATT_APM_PERVOLT", sensorselected.ampspervolt);

            MainV2.comPort.setParam("BATT_VOLT_MULT", sensorselected.voltspervolt);
        }

        public void SetMode(MonitorModes modes)
        {
            MainV2.comPort.setParam("BATT_MONITOR", modes.value);
        }

        public void SetBattCapacity(int mah)
        {
            MainV2.comPort.setParam("BATT_CAPACITY", mah);
        }

        public void SetAmpOffset(float offset)
        {
            MainV2.comPort.setParam("BATT_AMP_OFFSET", offset);
        }

        public class MonitorModes
        {
            public string Name = "";
            public int value = -1;

            public override string ToString()
            {
                return Name;
            }
        }

        public class PinNumbers
        {
            public string Name = "";
            public int volpin = -1;
            public int curpin = -1;

            public override string ToString()
            {
                return Name;
            }
        }

        public class Sensor
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
    }
}