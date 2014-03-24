using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MissionPlanner.Controls.BackstageView;
using MissionPlanner.Utilities;
using MissionPlanner.GCSViews.ConfigurationView;
using log4net;
using System.Reflection;
using MissionPlanner.Controls;

namespace MissionPlanner.GCSViews
{
    public partial class InitialSetup : MyUserControl, IActivate
    {
        internal static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static string lastpagename = "";

        public bool isConnected { get { return MainV2.comPort.BaseStream.IsOpen; } }

        public bool isDisConnected { get { return !MainV2.comPort.BaseStream.IsOpen; } }

        public bool isCopter { get { return isConnected && MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduCopter2; } }

        public bool isHeli { get { return isConnected && MainV2.comPort.MAV.param["H_SWASH_TYPE"] != null; } }

        public bool isPlane { get { return isConnected && (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduPlane || MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.Ateryx); } }

        public bool isRover { get { return isConnected && MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduRover; } }

        public InitialSetup()
        {
            InitializeComponent();
        }

        public void Activate()
        {
            initialSetupBindingSource.DataSource = this;
        }

        private void HardwareConfig_Load(object sender, EventArgs e)
        {
            // remeber last page accessed
            foreach (BackstageViewPage page in backstageView.Pages)
            {
                if (page.LinkText == lastpagename && page.Show)
                {
                    this.backstageView.ActivatePage(page);
                    break;
                }
            }

            ThemeManager.ApplyThemeTo(this);
        }

        private void HardwareConfig_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (backstageView.SelectedPage != null)
                lastpagename = backstageView.SelectedPage.LinkText;

            backstageView.Close();
        }
    }
}
