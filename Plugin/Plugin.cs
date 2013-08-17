using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ArdupilotMega.Plugin
{
    public abstract class Plugin
    {
        public PluginHost Host { get; set; }

        public abstract string Name { get; }
        public abstract string Version { get; }
        public abstract string Author { get; }

        /// <summary>
        /// this is the datetime loop will run next and can be set in loop, to override the loophzrate
        /// </summary>
        public virtual DateTime NextRun { get; set; }

        /// <summary>
        /// Run First, checking plugin
        /// </summary>
        /// <returns></returns>
        public abstract bool Init();

        /// <summary>
        /// Load your own code here, this is only run once on loading
        /// </summary>
        /// <returns></returns>
        public abstract bool Loaded();

        /// <summary>
        /// for future expansion
        /// </summary>
        /// <param name="gui"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual bool SetupUI(int gui = 0, object data = null) { return true; }

        /// <summary>
        /// Run at NextRun time - loop is run in a background thread. and is shared with other plugins
        /// </summary>
        /// <returns></returns>
        public virtual bool Loop() { return true; }

        /// <summary>
        /// run at a specific hz rate.
        /// </summary>
        public virtual float loopratehz { get; set; }

        public abstract bool Exit();

    }

    public class PluginHost
    {
        /// <summary>
        /// Device change event
        /// </summary>
        public event ArdupilotMega.MainV2.WMDeviceChangeEventHandler DeviceChanged;

        internal void ProcessDeviceChanged(ArdupilotMega.MainV2.WM_DEVICECHANGE_enum dc) {
            if (DeviceChanged != null)
            {
                try
                {
                    DeviceChanged(dc);
                }
                catch {  }
            }
        }

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
