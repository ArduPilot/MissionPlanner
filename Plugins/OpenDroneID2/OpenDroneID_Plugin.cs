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
            get { return "0.01"; }
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

            tabctrl = Host.MainForm.FlightData.tabControlactions;
            tab.Text = "Drone ID";
            tab.Controls.Add(myODID_UI);
            tabctrl.TabPages.Insert(5,tab);
            
            Host.MainForm.FlightPlanner.updateDisplayView();
            ThemeManager.ApplyThemeTo(tab);
            

            return true;
        }

        public override bool Exit()
        {
            return true;
        }
    }
}
