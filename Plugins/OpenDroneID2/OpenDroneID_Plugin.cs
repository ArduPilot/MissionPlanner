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
            get { return "0.04"; }
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
            forceSettings();

            //TODO Uncomment once Beta is updates
            //Host.MainForm.FlightData.TabListOriginal.Add(tab);

            tabctrl = Host.MainForm.FlightData.tabControlactions;
            // set the display name
            tab.Text = "Drone ID";
            // set the internal id
            tab.Name = "tabDroneID";
            // add the usercontrol to the tabpage
            tab.Controls.Add(myODID_UI);

            tabctrl.TabPages.Insert(5, tab);

            //Host.MainForm.FlightPlanner.updateDisplayView();

            ThemeManager.ApplyThemeTo(tab);

            myODID_UI.setVer("Ver: " + Version);

            return true;
        }


        private void forceSettings()
        {
            

            string tabs = Settings.Instance["tabcontrolactions"];

            // setup default if doesnt exist
            if (tabs == null)
            {
                CustomMessageBox.Show("Restart Mission Planner to enable Drone ID Tab. Disable Plugin if Not Required CTRL-P");
                Host.MainForm.FlightData.saveTabControlActions();
                tabs = Settings.Instance["tabcontrolactions"];
                Settings.Instance.Save();
            }
        }


        public override bool Exit()
        {
            return true;
        }
    }
}
