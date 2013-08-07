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

        int loopratehz { get; set; }

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


    }
}
