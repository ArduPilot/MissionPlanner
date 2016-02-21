using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using MissionPlanner;
using MissionPlanner.Utilities;
using MissionPlanner.GCSViews;

namespace MissionPlanner.Plugin
{
    public abstract class Plugin
    {
        public Assembly Assembly = null;

        public PluginHost Host { get; internal set; }

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
        public virtual bool SetupUI(int gui = 0, object data = null)
        {
            return true;
        }

        /// <summary>
        /// Run at NextRun time - loop is run in a background thread. and is shared with other plugins
        /// </summary>
        /// <returns></returns>
        public virtual bool Loop()
        {
            return true;
        }

        /// <summary>
        /// run at a specific hz rate.
        /// </summary>
        public virtual float loopratehz { get; set; }

        /// <summary>
        /// Exit is only called on plugin unload. not app exit
        /// </summary>
        /// <returns></returns>
        public abstract bool Exit();
    }

    public class PluginHost
    {
        /// <summary>
        /// Device change event
        /// </summary>
        public event MainV2.WMDeviceChangeEventHandler DeviceChanged;

        internal void ProcessDeviceChanged(MainV2.WM_DEVICECHANGE_enum dc)
        {
            if (DeviceChanged != null)
            {
                try
                {
                    DeviceChanged(dc);
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// access to the main form, this is on a diffrent thread.
        /// </summary>
        public MainV2 MainForm
        {
            get
            {
                return MainV2.instance;
            }
        }

        /// <summary>
        /// access to all the current stats of the mav
        /// </summary>
        public CurrentState cs
        {
            get { return MainV2.comPort.MAV.cs; }
        }

        /// <summary>
        /// access to mavlink functions
        /// </summary>
        public MAVLinkInterface comPort
        {
            get { return MainV2.comPort; }
        }

        /// <summary>
        /// access to mp settings
        /// </summary>
        public Settings config
        {
            get { return Settings.Instance; }
        }

        /// <summary>
        /// add things to flightdata map menu
        /// </summary>
        public ContextMenuStrip FDMenuMap
        {
            get { return MainV2.instance.FlightData.contextMenuStripMap; }
        }

        /// <summary>
        /// The point where the menu was drawn
        /// </summary>
        public PointLatLng FDMenuMapPosition
        {
            get { return MainV2.instance.FlightData.MouseDownStart; }
        }

        public GMapProvider FDMapType
        {
            get { return FlightData.mymap.MapProvider; }
        }

        /// <summary>
        /// add things to flightdata hud menu
        /// </summary>
        public ContextMenuStrip FDMenuHud
        {
            get { return MainV2.instance.FlightData.contextMenuStripHud; }
        }

        /// <summary>
        /// add things to flightplanner map menu
        /// </summary>
        public ContextMenuStrip FPMenuMap
        {
            get { return MainV2.instance.FlightPlanner.contextMenuStrip1; }
        }

        /// <summary>
        /// The point where the menu was drawn
        /// </summary>
        public PointLatLng FPMenuMapPosition
        {
            get { return MainV2.instance.FlightPlanner.MouseDownEnd; }
        }

        /// <summary>
        /// The polygon drawn by the user on the FP page
        /// </summary>
        public GMapPolygon FPDrawnPolygon
        {
            get
            {
                return new GMapPolygon(new List<PointLatLng>(MainV2.instance.FlightPlanner.drawnpolygon.Points),
                    "Poly Copy") {Stroke = MainV2.instance.FlightPlanner.drawnpolygon.Stroke};
            }
        }

        public void RedrawFPPolygon(List<PointLatLngAlt> list)
        {
            MainV2.instance.FlightPlanner.redrawPolygonSurvey(list);
        }

        /// <summary>
        /// the map control in flightplanner
        /// </summary>
        public GMapControl FPGMapControl
        {
            get
            {
                return MainV2.instance.FlightPlanner.MainMap;
            }
        }

        /// <summary>
        /// the map control in flightdata
        /// </summary>
        public GMapControl FDGMapControl
        {
            get
            {
                return MainV2.instance.FlightData.gMapControl1;
            }
        }

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
        public void AddWPtoList(MAVLink.MAV_CMD cmd, double p1, double p2, double p3, double p4, double x, double y,
            double z)
        {
            MainV2.instance.FlightPlanner.AddCommand(cmd, p1, p2, p3, p4, x, y, z);
        }

        public void InsertWP(int idx, MAVLink.MAV_CMD cmd, double p1, double p2, double p3, double p4, double x, double y,
            double z)
        {
            MainV2.instance.FlightPlanner.InsertCommand(idx, cmd, p1, p2, p3, p4, x, y, z);
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