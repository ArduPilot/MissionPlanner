using MissionPlanner.Grid;
using System;
using System.Windows.Forms;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;

namespace OpenDroneID_Plugin
{
    public class OpenDroneID_Plugin : MissionPlanner.Plugin.Plugin
    {
        //TabPage
        private System.Windows.Forms.TabPage tab = new System.Windows.Forms.TabPage();
        private TabControl tabctrl;
        private OpenDroneID_UI myODID_UI = new OpenDroneID_UI();

        public override string Name
        {
            get { return "Open Drone ID"; }
        }

        public override string Version
        {
            get { return "0.02"; }
        }

        public override string Author
        {
            get { return "Steven Borenstein"; }
        }

        public override bool Init()
        {
            myODID_UI.setHost(Host);

            

            return true;
        }

        public override bool Loaded()
        {
            if (String.IsNullOrEmpty(Settings.Instance["tabcontrolactions"]))
            { 
                // no action - ie add it
            }
            else if(String.IsNullOrEmpty(Settings.Instance["OpenDroneID_init"]))
            {
                // first init, force add
                Settings.Instance["tabcontrolactions"] += "tabDroneID";
                Settings.Instance["OpenDroneID_init"] = true.ToString();
            }
            tabctrl = Host.MainForm.FlightData.tabControlactions;
            // set the display name
            tab.Text = "Drone ID";
            // set the internal id
            tab.Name = "tabDroneID";
            // add the usercontrol to the tabpage
            tab.Controls.Add(myODID_UI);

            // add it to the list of options
            Host.MainForm.FlightData.TabListOriginal.Add(tab);

            // refilter the display list based  on user selection
            Host.MainForm.FlightData.loadTabControlActions();

            ThemeManager.ApplyThemeTo(tab);
            

            return true;
        }

        public override bool Exit()
        {
            return true;
        }
    }
}
