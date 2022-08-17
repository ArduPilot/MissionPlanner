using MissionPlanner.Grid;
using System;
using System.Windows.Forms;

namespace MissionPlanner
{
    public class FaceMapOpenDroneID_Plugin : MissionPlanner.Plugin.Plugin
    {
        ToolStripMenuItem but;

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
            return true;
        }

        public override bool Loaded()
        {

            return true;
        }

        public override bool Exit()
        {
            return true;
        }
    }
}
