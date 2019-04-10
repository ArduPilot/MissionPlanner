using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigHWIDs : MyUserControl, IActivate
    {
        public ConfigHWIDs()
        {
            InitializeComponent();
        }

        public void Activate()
        {
            if (!MainV2.comPort.BaseStream.IsOpen)
            {
                Enabled = false;
            }
            Enabled = true;

            var all_dev_ids = MainV2.comPort.MAV.param.Where(a => a.Name.Contains("_ID"));

            var b = 0;

            foreach (var dev in all_dev_ids)
            {
                var devid = new Device.DeviceStructure((uint)dev.Value);

                var ans = devid.ToString();

                var gr = CreateGraphics();

                gr.DrawString(dev.Name + " " + ans, Font, new SolidBrush(ForeColor), 5, 5 + b * 25);

                b++;
            }
        }
        
    }
}