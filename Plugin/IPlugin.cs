using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ArdupilotMega.Utilities;

namespace ArdupilotMega.Plugin
{
    public interface IPlugin
    {
        PluginHost Host { get; set; }

        string Name { get; }
        string Version { get;  }
        string Author { get;  }

        DateTime NextRun { get; set; }

        bool Init();

        bool Loaded();

        bool SetupUI(int gui = 0);

        bool Loop();

        float loopratehz { get; set; }

        bool Exit();

    }

    public class PluginHost
    {
        /// <summary>
        /// access to all the current stats of the mav
        /// </summary>
        public CurrentState cs { get { return MainV2.comPort.MAV.cs; } }

        /// <summary>
        /// access to mavlink functions
        /// </summary>
        public MAVLink comPort { get { return MainV2.comPort; } }

        /// <summary>
        /// add things to flightdata map menu
        /// </summary>
        public ContextMenuStrip FDMenuMap { get { return MainV2.instance.FlightData.contextMenuStripMap; } }

        /// <summary>
        /// add things to flightdata hud menu
        /// </summary>
        public ContextMenuStrip FDMenuHud { get { return MainV2.instance.FlightData.contextMenuStripHud; } }

        /// <summary>
        /// add things to flightplanner map menu
        /// </summary>
        public ContextMenuStrip FPMenuMap { get { return MainV2.instance.FlightPlanner.contextMenuStrip1; } }

        /// <summary>
        /// add wp to command queue - dont upload to mav
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="p4"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void AddWPtoList(MAVLink.MAV_CMD cmd, float p1, float p2, float p3, float p4, float x, float y, float z)
        {
            MainV2.instance.FlightPlanner.AddCommand(cmd, p1, p2, p3, p4, x, y, z);
        }

        /// <summary>
        /// refresh command list on flight planner tab from autopilot
        /// </summary>
        public void GetWPs()
        {
            MainV2.instance.FlightPlanner.BUT_read_Click(null, null);
        }
    }
}
