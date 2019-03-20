using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using DotSpatial.Data;
using DotSpatial.Projections;
using GeoUtility.GeoSystem;
using GeoUtility.GeoSystem.Base;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using Ionic.Zip;
using log4net;
using MissionPlanner.Controls;
using MissionPlanner.Maps;
using MissionPlanner.Properties;
using MissionPlanner.Utilities;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using SharpKml.Base;
using SharpKml.Dom;
using Feature = SharpKml.Dom.Feature;
using ILog = log4net.ILog;
using Placemark = SharpKml.Dom.Placemark;
using Point = System.Drawing.Point;
using GDAL;
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;
using MissionPlanner.ArduPilot;
using MissionPlanner.Grid;
using MissionPlanner.Plugin;

namespace MissionPlanner.GCSViews
{
    public partial class FlightPlanner : MyUserControl, IDeactivate, IActivate
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        int selectedrow;
        public bool quickadd;
        bool sethome;
        bool polygongridmode;
        bool splinemode;
        altmode currentaltmode = altmode.Relative;

        bool grid;

        public static FlightPlanner instance;

        public List<PointLatLngAlt> pointlist { get; set; } = new List<PointLatLngAlt>();

        static public Object thisLock = new Object();
        private ComponentResourceManager rm = new ComponentResourceManager(typeof (FlightPlanner));

        private Dictionary<string, string[]> cmdParamNames = new Dictionary<string, string[]>();

        List<List<Locationwp>> history = new List<List<Locationwp>>();

        List<int> groupmarkers = new List<int>();

        public enum altmode
        {
            Relative = MAVLink.MAV_FRAME.GLOBAL_RELATIVE_ALT,
            Absolute = MAVLink.MAV_FRAME.GLOBAL,
            Terrain = MAVLink.MAV_FRAME.GLOBAL_TERRAIN_ALT
        }

        private void poieditToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentGMapMarker == null || !(CurrentGMapMarker is GMapMarkerPOI))
                return;

            POI.POIEdit(CurrentPOIMarker);
        }

        private void poideleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentPOIMarker  == null)
                return;

            POI.POIDelete(CurrentPOIMarker);
        }

        private void poiaddToolStripMenuItem_Click(object sender, EventArgs e)
        {
            POI.POIAdd(MouseDownStart);
        }

        /// <summary>
        /// used to adjust existing point in the datagrid including "H"
        /// </summary>
        /// <param name="pointno"></param>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <param name="alt"></param>
        public void callMeDrag(string pointno, double lat, double lng, int alt)
        {
            if (pointno == "")
            {
                return;
            }

            // dragging a WP
            if (pointno == "H")
            {
                // auto update home alt
                TXT_homealt.Text = (srtm.getAltitude(lat, lng).alt * CurrentState.multiplieralt).ToString();

                TXT_homelat.Text = lat.ToString();
                TXT_homelng.Text = lng.ToString();
                return;
            }

            if (pointno == "Tracker Home")
            {
                MainV2.comPort.MAV.cs.TrackerLocation = new PointLatLngAlt(lat, lng, alt, "");
                return;
            }

            try
            {
                selectedrow = int.Parse(pointno) - 1;
                Commands.CurrentCell = Commands[1, selectedrow];
                // depending on the dragged item, selectedrow can be reset 
                selectedrow = int.Parse(pointno) - 1;
            }
            catch
            {
                return;
            }

            setfromMap(lat, lng, alt);
        }

        /// <summary>
        /// Actualy Sets the values into the datagrid and verifys height if turned on
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <param name="alt"></param>
        public void setfromMap(double lat, double lng, int alt, double p1 = 0)
        {
            if (selectedrow > Commands.RowCount)
            {
                CustomMessageBox.Show("Invalid coord, How did you do this?");
                return;
            }

            try
            {
                if (!quickadd)
                {
                    // get current command list
                    var currentlist = GetCommandList();
                    // add history
                    history.Add(currentlist);
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("A invalid entry has been detected\n" + ex.Message, Strings.ERROR);
            }

            // remove more than 20 revisions
            if (history.Count > 20)
            {
                history.RemoveRange(0, history.Count - 20);
            }

            DataGridViewTextBoxCell cell;
            if (alt == -2 && Commands.Columns[Alt.Index].HeaderText.Equals(cmdParamNames["WAYPOINT"][6] /*"Alt"*/))
            {
                if (CHK_verifyheight.Checked && (altmode)CMB_altmode.SelectedValue != altmode.Terrain) //Drag with verifyheight // use srtm data
                {
                    cell = Commands.Rows[selectedrow].Cells[Alt.Index] as DataGridViewTextBoxCell;
                    float ans;
                    if (float.TryParse(cell.Value.ToString(), out ans))
                    {
                        ans = (int) ans;

                        DataGridViewTextBoxCell celllat =
                            Commands.Rows[selectedrow].Cells[Lat.Index] as DataGridViewTextBoxCell;
                        DataGridViewTextBoxCell celllon =
                            Commands.Rows[selectedrow].Cells[Lon.Index] as DataGridViewTextBoxCell;
                        int oldsrtm =
                            (int)
                                ((srtm.getAltitude(double.Parse(celllat.Value.ToString()),
                                    double.Parse(celllon.Value.ToString())).alt)*CurrentState.multiplieralt);
                        int newsrtm = (int) ((srtm.getAltitude(lat, lng).alt)*CurrentState.multiplieralt);
                        int newh = (int) (ans + newsrtm - oldsrtm);

                        cell.Value = newh;

                        cell.DataGridView.EndEdit();
                    }
                }
            }
            if (Commands.Columns[Lat.Index].HeaderText.Equals(cmdParamNames["WAYPOINT"][4] /*"Lat"*/))
            {
                cell = Commands.Rows[selectedrow].Cells[Lat.Index] as DataGridViewTextBoxCell;
                cell.Value = lat.ToString("0.0000000");
                cell.DataGridView.EndEdit();
            }
            if (Commands.Columns[Lon.Index].HeaderText.Equals(cmdParamNames["WAYPOINT"][5] /*"Long"*/))
            {
                cell = Commands.Rows[selectedrow].Cells[Lon.Index] as DataGridViewTextBoxCell;
                cell.Value = lng.ToString("0.0000000");
                cell.DataGridView.EndEdit();
            }
            if (alt != -1 && alt != -2 &&
                Commands.Columns[Alt.Index].HeaderText.Equals(cmdParamNames["WAYPOINT"][6] /*"Alt"*/))
            {
                cell = Commands.Rows[selectedrow].Cells[Alt.Index] as DataGridViewTextBoxCell;

                {
                    double result;
                    bool pass = double.TryParse(TXT_homealt.Text, out result);

                    if (pass == false)
                    {
                        CustomMessageBox.Show("You must have a home altitude");
                        string homealt = "100";
                        if (DialogResult.Cancel == InputBox.Show("Home Alt", "Home Altitude", ref homealt))
                            return;
                        TXT_homealt.Text = homealt;
                    }
                    int results1;
                    if (!int.TryParse(TXT_DefaultAlt.Text, out results1))
                    {
                        CustomMessageBox.Show("Your default alt is not valid");
                        return;
                    }

                    if (results1 == 0)
                    {
                        string defalt = "100";
                        if (DialogResult.Cancel == InputBox.Show("Default Alt", "Default Altitude", ref defalt))
                            return;
                        TXT_DefaultAlt.Text = defalt;
                    }
                }

                cell.Value = TXT_DefaultAlt.Text;

                float ans;
                if (float.TryParse(cell.Value.ToString(), out ans))
                {
                    ans = (int) ans;
                    if (alt != 0) // use passed in value;
                        cell.Value = alt.ToString();
                    if (ans == 0) // default
                        cell.Value = 50;
                    if (ans == 0 && (MainV2.comPort.MAV.cs.firmware == Firmwares.ArduCopter2))
                        cell.Value = 15;

                    // not online and verify alt via srtm
                    if (CHK_verifyheight.Checked) // use srtm data
                    {
                        // is absolute but no verify
                        if ((altmode) CMB_altmode.SelectedValue == altmode.Absolute)
                        {
                            //abs
                            cell.Value =
                                ((srtm.getAltitude(lat, lng).alt)*CurrentState.multiplieralt +
                                 int.Parse(TXT_DefaultAlt.Text)).ToString();
                        }
                        else if ((altmode) CMB_altmode.SelectedValue == altmode.Terrain)
                        {
                            cell.Value = int.Parse(TXT_DefaultAlt.Text);
                        }
                        else
                        {
                            //relative and verify
                            cell.Value =
                                ((int) (srtm.getAltitude(lat, lng).alt)*CurrentState.multiplieralt +
                                 int.Parse(TXT_DefaultAlt.Text) -
                                 (int)
                                     srtm.getAltitude(MainV2.comPort.MAV.cs.HomeLocation.Lat,
                                         MainV2.comPort.MAV.cs.HomeLocation.Lng).alt*CurrentState.multiplieralt)
                                    .ToString();
                        }
                    }

                    cell.DataGridView.EndEdit();
                }
                else
                {
                    CustomMessageBox.Show("Invalid Home or wp Alt");
                    cell.Style.BackColor = Color.Red;
                }
            }

            // convert to utm
            convertFromGeographic(lat, lng);

            // Add more for other params
            if (Commands.Columns[Param1.Index].HeaderText.Equals(cmdParamNames["WAYPOINT"][1] /*"Delay"*/))
            {
                cell = Commands.Rows[selectedrow].Cells[Param1.Index] as DataGridViewTextBoxCell;
                cell.Value = p1;
                cell.DataGridView.EndEdit();
            }

            writeKML();
            Commands.EndEdit();
        }

        private void convertFromGeographic(double lat, double lng)
        {
            if (lat == 0 && lng == 0)
            {
                return;
            }

            // always update other systems, incase user switchs while planning
            try
            {
                //UTM
                var temp = new PointLatLngAlt(lat, lng);
                int zone = temp.GetUTMZone();
                var temp2 = temp.ToUTM();
                Commands[coordZone.Index, selectedrow].Value = zone;
                Commands[coordEasting.Index, selectedrow].Value = temp2[0].ToString("0.000");
                Commands[coordNorthing.Index, selectedrow].Value = temp2[1].ToString("0.000");
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            try
            {
                //MGRS
                Commands[MGRS.Index, selectedrow].Value = ((MGRS) new Geographic(lng, lat)).ToString();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        void convertFromUTM(int rowindex)
        {
            try
            {
                var zone = int.Parse(Commands[coordZone.Index, rowindex].Value.ToString());

                var east = double.Parse(Commands[coordEasting.Index, rowindex].Value.ToString());

                var north = double.Parse(Commands[coordNorthing.Index, rowindex].Value.ToString());

                if (east == 0 && north == 0)
                {
                    return;
                }

                var utm = new utmpos(east, north, zone);

                Commands[Lat.Index, rowindex].Value = utm.ToLLA().Lat;
                Commands[Lon.Index, rowindex].Value = utm.ToLLA().Lng;

            }
            catch (Exception ex)
            {
                log.Error(ex);
                return;
            }
        }

        void convertFromMGRS(int rowindex)
        {
            try
            {
                var mgrs = Commands[MGRS.Index, rowindex].Value.ToString();

                MGRS temp = new MGRS(mgrs);

                var convert = temp.ConvertTo<Geographic>();

                if (convert.Latitude == 0 || convert.Longitude == 0)
                    return;

                Commands[Lat.Index, rowindex].Value = convert.Latitude.ToString();
                Commands[Lon.Index, rowindex].Value = convert.Longitude.ToString();

            }
            catch (Exception ex)
            {
                log.Error(ex);
                return;
            }
        }

        PointLatLngAlt mouseposdisplay = new PointLatLngAlt(0, 0);

        /// <summary>
        /// Used for current mouse position
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <param name="alt"></param>
        public void SetMouseDisplay(double lat, double lng, int alt)
        {
            mouseposdisplay.Lat = lat;
            mouseposdisplay.Lng = lng;
            mouseposdisplay.Alt = alt;

            coords1.Lat = mouseposdisplay.Lat;
            coords1.Lng = mouseposdisplay.Lng;
            var altdata = srtm.getAltitude(mouseposdisplay.Lat, mouseposdisplay.Lng, MainMap.Zoom);
            coords1.Alt = altdata.alt;
            coords1.AltSource = altdata.altsource;

            try
            {
                PointLatLng last;

                if (pointlist.Count == 0 || pointlist[pointlist.Count - 1] == null)
                    return;

                last = pointlist[pointlist.Count - 1];

                double lastdist = MainMap.MapProvider.Projection.GetDistance(last, currentMarker.Position);

                double lastbearing = 0;

                if (pointlist.Count > 0)
                {
                    lastbearing = MainMap.MapProvider.Projection.GetBearing(last, currentMarker.Position);
                }

                lbl_prevdist.Text = rm.GetString("lbl_prevdist.Text") + ": " + FormatDistance(lastdist, true) + " AZ: " +
                                    lastbearing.ToString("0");

                // 0 is home
                if (pointlist[0] != null)
                {
                    double homedist = MainMap.MapProvider.Projection.GetDistance(currentMarker.Position, pointlist[0]);

                    lbl_homedist.Text = rm.GetString("lbl_homedist.Text") + ": " + FormatDistance(homedist, true);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        /// <summary>
        /// Used to create a new WP
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <param name="alt"></param>
        public void AddWPToMap(double lat, double lng, int alt)
        {
            if (polygongridmode)
            {
                addPolygonPointToolStripMenuItem_Click(null, null);
                return;
            }

            if (sethome)
            {
                sethome = false;
                callMeDrag("H", lat, lng, alt);
                return;
            }
            // creating a WP

            selectedrow = Commands.Rows.Add();

            if (splinemode)
            {
                Commands.Rows[selectedrow].Cells[Command.Index].Value = MAVLink.MAV_CMD.SPLINE_WAYPOINT.ToString();
                ChangeColumnHeader(MAVLink.MAV_CMD.SPLINE_WAYPOINT.ToString());
            }
            else
            {
                Commands.Rows[selectedrow].Cells[Command.Index].Value = MAVLink.MAV_CMD.WAYPOINT.ToString();
                ChangeColumnHeader(MAVLink.MAV_CMD.WAYPOINT.ToString());
            }

            setfromMap(lat, lng, alt);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // undo
            if (keyData == (Keys.Control | Keys.Z))
            {
                if (history.Count > 0)
                {
                    int no = history.Count - 1;
                    var pop = history[no];
                    history.RemoveAt(no);
                    WPtoScreen(pop);
                }
                return true;
            }

            // open wp file
            if (keyData == (Keys.Control | Keys.O))
            {
                loadWPFileToolStripMenuItem_Click(null, null);
                return true;
            }

            // save wp file
            if (keyData == (Keys.Control | Keys.S))
            {
                saveWPFileToolStripMenuItem_Click(null, null);
                return true;
            }

            if (keyData == (Keys.Control | Keys.M))
            {
                // get the command list from the datagrid
                var commandlist = GetCommandList();
                MainV2.comPort.MAV.wps[0] = new Locationwp().Set(MainV2.comPort.MAV.cs.HomeLocation.Lat,
                    MainV2.comPort.MAV.cs.HomeLocation.Lng, MainV2.comPort.MAV.cs.HomeLocation.Alt, 0);
                int a = 1;
                commandlist.ForEach(i =>
                {
                    MAVLink.mavlink_mission_item_t item = (MAVLink.mavlink_mission_item_t) i;
                    item.seq = (ushort)a;
                    MainV2.comPort.MAV.wps[a] = item;
                    a++;
                });

                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        public FlightPlanner()
        {
            instance = this;

            InitializeComponent();

            // config map             
            MainMap.CacheLocation = Settings.GetDataDirectory() +
                                    "gmapcache" + Path.DirectorySeparatorChar;

            // map events
            MainMap.OnPositionChanged += MainMap_OnCurrentPositionChanged;
            MainMap.OnTileLoadStart += MainMap_OnTileLoadStart;
            MainMap.OnTileLoadComplete += MainMap_OnTileLoadComplete;
            MainMap.OnMarkerClick += MainMap_OnMarkerClick;
            MainMap.OnMapZoomChanged += MainMap_OnMapZoomChanged;
            MainMap.OnMapTypeChanged += MainMap_OnMapTypeChanged;
            MainMap.MouseMove += MainMap_MouseMove;
            MainMap.MouseDown += MainMap_MouseDown;
            MainMap.MouseUp += MainMap_MouseUp;
            MainMap.OnMarkerEnter += MainMap_OnMarkerEnter;
            MainMap.OnMarkerLeave += MainMap_OnMarkerLeave;

            MainMap.MapScaleInfoEnabled = false;
            MainMap.ScalePen = new Pen(Color.Red);

            MainMap.DisableFocusOnMouseEnter = true;

            MainMap.ForceDoubleBuffer = false;

            //WebRequest.DefaultWebProxy.Credentials = System.Net.CredentialCache.DefaultCredentials;

            // get map type
            comboBoxMapType.ValueMember = "Name";
            comboBoxMapType.DataSource = GMapProviders.List.ToArray();
            comboBoxMapType.SelectedItem = MainMap.MapProvider;

            comboBoxMapType.SelectedValueChanged += comboBoxMapType_SelectedValueChanged;

            MainMap.RoutesEnabled = true;

            //MainMap.MaxZoom = 18;

            // get zoom  
            MainMap.MinZoom = 0;
            MainMap.MaxZoom = 24;

            // draw this layer first
            kmlpolygonsoverlay = new GMapOverlay("kmlpolygons");
            MainMap.Overlays.Add(kmlpolygonsoverlay);

            geofenceoverlay = new GMapOverlay("geofence");
            MainMap.Overlays.Add(geofenceoverlay);

            rallypointoverlay = new GMapOverlay("rallypoints");
            MainMap.Overlays.Add(rallypointoverlay);

            routesoverlay = new GMapOverlay("routes");
            MainMap.Overlays.Add(routesoverlay);

            polygonsoverlay = new GMapOverlay("polygons");
            MainMap.Overlays.Add(polygonsoverlay);

            airportsoverlay = new GMapOverlay("airports");
            MainMap.Overlays.Add(airportsoverlay);

            objectsoverlay = new GMapOverlay("objects");
            MainMap.Overlays.Add(objectsoverlay);

            drawnpolygonsoverlay = new GMapOverlay("drawnpolygons");
            MainMap.Overlays.Add(drawnpolygonsoverlay);

            MainMap.Overlays.Add(poioverlay);

            top = new GMapOverlay("top");
            //MainMap.Overlays.Add(top);

            objectsoverlay.Markers.Clear();

            // set current marker
            currentMarker = new GMarkerGoogle(MainMap.Position, GMarkerGoogleType.red);
            //top.Markers.Add(currentMarker);

            // map center
            center = new GMarkerGoogle(MainMap.Position, GMarkerGoogleType.none);
            top.Markers.Add(center);

            MainMap.Zoom = 3;

            CMB_altmode.DisplayMember = "Value";
            CMB_altmode.ValueMember = "Key";
            CMB_altmode.DataSource = EnumTranslator.EnumToList<altmode>();

            //set default
            CMB_altmode.SelectedItem = altmode.Relative;

            updateCMDParams();

            foreach (DataGridViewColumn commandsColumn in Commands.Columns)
            {
                if(commandsColumn is DataGridViewTextBoxColumn)
                    commandsColumn.CellTemplate.Value = "0";
            }

            Commands.Columns[Delete.Index].CellTemplate.Value = "X";
            Commands.Columns[Up.Index].CellTemplate.Value = Resources.up;
            Commands.Columns[Down.Index].CellTemplate.Value = Resources.down;

            Up.Image = Resources.up;
            Down.Image = Resources.down;

            updateMapType(null, null);

            // hide the map to prevent redraws when its loaded
            panelMap.Visible = false;


            // setup geofence
            List<PointLatLng> polygonPoints = new List<PointLatLng>();
            geofencepolygon = new GMapPolygon(polygonPoints, "geofence");
            geofencepolygon.Stroke = new Pen(Color.Pink, 5);
            geofencepolygon.Fill = Brushes.Transparent;

            //setup drawnpolgon
            List<PointLatLng> polygonPoints2 = new List<PointLatLng>();
            drawnpolygon = new GMapPolygon(polygonPoints2, "drawnpoly");
            drawnpolygon.Stroke = new Pen(Color.Red, 2);
            drawnpolygon.Fill = Brushes.Transparent;

            /*
            var timer = new System.Timers.Timer();

            // 2 second
            timer.Interval = 2000;
            timer.Elapsed += updateMapType;

            timer.Start();
            */
        }

        void updateMapType(object sender, System.Timers.ElapsedEventArgs e)
        {
            log.Info("updateMapType invoke req? " + comboBoxMapType.InvokeRequired);

            if (sender is System.Timers.Timer)
                ((System.Timers.Timer)sender).Stop();

            string mapType = Settings.Instance["MapType"];
            if (!string.IsNullOrEmpty(mapType))
            {
                try
                {
                    var index = GMapProviders.List.FindIndex(x => (x.Name == mapType));

                    if (index != -1)
                        comboBoxMapType.SelectedIndex = index;
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }
            else
            {
                if (L10N.ConfigLang.IsChildOf(CultureInfo.GetCultureInfo("zh-Hans")))
                {
                    CustomMessageBox.Show(
                        "亲爱的中国用户，为保证地图使用正常，已为您将默认地图自动切换到具有中国特色的【谷歌中国卫星地图】！\r\n与默认【谷歌卫星地图】的区别：使用.cn服务器，加入火星坐标修正\r\n如果您所在的地区仍然无法使用，天书同时推荐必应或高德地图，其它地图由于没有加入坐标修正功能，为确保飞行安全，请谨慎选择",
                        "默认地图已被切换");

                    try
                    {
                        var index = GMapProviders.List.FindIndex(x => (x.Name == "谷歌中国卫星地图"));

                        if (index != -1)
                            comboBoxMapType.SelectedIndex = index;
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }
                }
                else
                {
                    mapType = "GoogleSatelliteMap";
                    // set default
                    try
                    {
                        var index = GMapProviders.List.FindIndex(x => (x.Name == mapType));

                        if (index != -1)
                            comboBoxMapType.SelectedIndex = index;
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }
                }
            }
        }

        void updateCMDParams()
        {
            cmdParamNames = readCMDXML();

            List<string> cmds = new List<string>();

            foreach (string item in cmdParamNames.Keys)
            {
                cmds.Add(item);
            }

            cmds.Add("UNKNOWN");

            Command.DataSource = cmds;
        }

        Dictionary<string, string[]> readCMDXML()
        {
            Dictionary<string, string[]> cmd = new Dictionary<string, string[]>();

            // do lang stuff here

            string file = Settings.GetRunningDirectory() + "mavcmd.xml";

            if (!File.Exists(file))
            {
                CustomMessageBox.Show("Missing mavcmd.xml file");
                return cmd;
            }

            log.Info("Reading MAV_CMD for " + MainV2.comPort.MAV.cs.firmware);

            using (XmlReader reader = XmlReader.Create(file))
            {
                reader.Read();
                reader.ReadStartElement("CMD");
                if (MainV2.comPort.MAV.cs.firmware == Firmwares.ArduPlane ||
                    MainV2.comPort.MAV.cs.firmware == Firmwares.Ateryx)
                {
                    reader.ReadToFollowing("APM");
                }
                else if (MainV2.comPort.MAV.cs.firmware == Firmwares.ArduRover)
                {
                    reader.ReadToFollowing("APRover");
                }
                else
                {
                    reader.ReadToFollowing("AC2");
                }

                XmlReader inner = reader.ReadSubtree();

                inner.Read();

                inner.MoveToElement();

                inner.Read();

                while (inner.Read())
                {
                    inner.MoveToElement();
                    if (inner.IsStartElement())
                    {
                        string cmdname = inner.Name;
                        string[] cmdarray = new string[7];
                        int b = 0;

                        XmlReader inner2 = inner.ReadSubtree();

                        inner2.Read();

                        while (inner2.Read())
                        {
                            inner2.MoveToElement();
                            if (inner2.IsStartElement())
                            {
                                cmdarray[b] = inner2.ReadString();
                                b++;
                            }
                        }

                        cmd[cmdname] = cmdarray;
                    }
                }
            }

            return cmd;
        }

        void Commands_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            log.Info(e.Exception + " " + e.Context + " col " + e.ColumnIndex);
            e.Cancel = false;
            e.ThrowException = false;
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Adds a new row to the datagrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BUT_Add_Click(object sender, EventArgs e)
        {
            if (Commands.CurrentRow == null)
            {
                selectedrow = 0;
            }
            else
            {
                selectedrow = Commands.CurrentRow.Index;
            }

            if (Commands.RowCount <= 1)
            {
                selectedrow = Commands.Rows.Add();
            }
            else
            {
                if (Commands.RowCount == selectedrow + 1)
                {
                    DataGridViewRow temp = Commands.Rows[selectedrow];
                    selectedrow = Commands.Rows.Add();
                }
                else
                {
                    Commands.Rows.Insert(selectedrow + 1, 1);
                }
            }
            writeKML();
        }

        private void FlightPlanner_Load(object sender, EventArgs e)
        {
            quickadd = true;

            Visible = false;

            config(false);

            quickadd = false;

            POI.POIModified += POI_POIModified;

            if (Settings.Instance["WMSserver"] != null)
                WMSProvider.CustomWMSURL = Settings.Instance["WMSserver"];

            trackBar1.Value = (int) MainMap.Zoom;

            updateCMDParams();

            panelMap.Visible = false;

            // mono
            panelMap.Dock = DockStyle.None;
            panelMap.Dock = DockStyle.Fill;
            panelMap_Resize(null, null);

            //set home
            try
            {
                if (TXT_homelat.Text != "")
                {
                    MainMap.Position = new PointLatLng(double.Parse(TXT_homelat.Text), double.Parse(TXT_homelng.Text));
                    MainMap.Zoom = 16;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            panelMap.Refresh();

            panelMap.Visible = true;

            writeKML();

            // switch the action and wp table
            if (Settings.Instance["FP_docking"] == "Bottom")
            {
                switchDockingToolStripMenuItem_Click(null, null);
            }

            Visible = true;
            updateDisplayView();

            timer1.Start();
        }


        public void updateDisplayView()
        {
            rallyPointsToolStripMenuItem.Visible = MainV2.DisplayConfiguration.displayRallyPointsMenu;
            geoFenceToolStripMenuItem.Visible = MainV2.DisplayConfiguration.displayGeoFenceMenu;
            createSplineCircleToolStripMenuItem.Visible = MainV2.DisplayConfiguration.displaySplineCircleAutoWp;
            textToolStripMenuItem.Visible = MainV2.DisplayConfiguration.displayTextAutoWp;
            createCircleSurveyToolStripMenuItem.Visible = MainV2.DisplayConfiguration.displayCircleSurveyAutoWp;
            pOIToolStripMenuItem.Visible = MainV2.DisplayConfiguration.displayPoiMenu;
            trackerHomeToolStripMenuItem.Visible = MainV2.DisplayConfiguration.displayTrackerHomeMenu;
            CHK_verifyheight.Visible = MainV2.DisplayConfiguration.displayCheckHeightBox;

            //hide dynamically generated toolstrip items in the auto WP dropdown (these do not have name objects populated)
            foreach (ToolStripItem item in autoWPToolStripMenuItem.DropDownItems)
            {
                if (item.Name.Equals(""))
                {
                    item.Visible = MainV2.DisplayConfiguration.displayPluginAutoWp;
                }
            }
        }

        void POI_POIModified(object sender, EventArgs e)
        {
            POI.UpdateOverlay(poioverlay);
        }

        void parser_ElementAdded(object sender, ElementEventArgs e)
        {
            processKML(e.Element);
        }

        private void processKML(Element Element)
        {
            Document doc = Element as Document;
            Placemark pm = Element as Placemark;
            Folder folder = Element as Folder;
            Polygon polygon = Element as Polygon;
            LineString ls = Element as LineString;
            MultipleGeometry geom = Element as MultipleGeometry;

            if (doc != null)
            {
                foreach (var feat in doc.Features)
                {
                    //Console.WriteLine("feat " + feat.GetType());
                    //processKML((Element)feat);
                }
            }
            else if (folder != null)
            {
                foreach (Feature feat in folder.Features)
                {
                    //Console.WriteLine("feat "+feat.GetType());
                    //processKML(feat);
                }
            }
            else if (pm != null)
            {
            }
            else if (polygon != null)
            {
                GMapPolygon kmlpolygon = new GMapPolygon(new List<PointLatLng>(), "kmlpolygon");

                kmlpolygon.Stroke.Color = Color.Purple;
                kmlpolygon.Fill = Brushes.Transparent;

                foreach (var loc in polygon.OuterBoundary.LinearRing.Coordinates)
                {
                    kmlpolygon.Points.Add(new PointLatLng(loc.Latitude, loc.Longitude));
                }

                kmlpolygonsoverlay.Polygons.Add(kmlpolygon);
            }
            else if (ls != null)
            {
                GMapRoute kmlroute = new GMapRoute(new List<PointLatLng>(), "kmlroute");

                kmlroute.Stroke.Color = Color.Purple;

                foreach (var loc in ls.Coordinates)
                {
                    kmlroute.Points.Add(new PointLatLng(loc.Latitude, loc.Longitude));
                }

                kmlpolygonsoverlay.Routes.Add(kmlroute);
            }
            else if (geom != null)
            {
                foreach (var geometry in geom.Geometry)
                {
                    processKML(geometry);
                }
            }
        }

        private void ChangeColumnHeader(string command)
        {
            try
            {
                if (cmdParamNames.ContainsKey(command))
                    for (int i = 1; i <= 7; i++)
                        Commands.Columns[i].HeaderText = cmdParamNames[command][i - 1];
                else
                    for (int i = 1; i <= 7; i++)
                        Commands.Columns[i].HeaderText = "setme";
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// Used to update column headers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Commands_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (quickadd)
                return;
            try
            {
                selectedrow = e.RowIndex;
                string option = Commands[Command.Index, selectedrow].EditedFormattedValue.ToString();
                string cmd;
                try
                {
                    if (Commands[Command.Index, selectedrow].Value != null)
                        cmd = Commands[Command.Index, selectedrow].Value.ToString();
                    else
                        cmd = option;
                }
                catch
                {
                    cmd = option;
                }
                //Console.WriteLine("editformat " + option + " value " + cmd);
                ChangeColumnHeader(cmd);

                if (cmd == "WAYPOINT")
                {
                }

                //  writeKML();
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(ex.ToString());
            }
        }

        private void Commands_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            for (int i = 0; i < Commands.ColumnCount; i++)
            {
                DataGridViewCell tcell = Commands.Rows[e.RowIndex].Cells[i];
                if (tcell.GetType() == typeof (DataGridViewTextBoxCell))
                {
                    if (tcell.Value == null)
                        tcell.Value = "0";
                }
            }

            DataGridViewComboBoxCell cell = Commands.Rows[e.RowIndex].Cells[Command.Index] as DataGridViewComboBoxCell;
            if (cell.Value == null)
            {
                cell.Value = "WAYPOINT";
                cell.DropDownWidth = 200;
                Commands.Rows[e.RowIndex].Cells[Delete.Index].Value = "X";
                if (!quickadd)
                {
                    Commands_RowEnter(sender, new DataGridViewCellEventArgs(0, e.RowIndex - 0)); // do header labels
                    Commands_RowValidating(sender, new DataGridViewCellCancelEventArgs(0, e.RowIndex));
                        // do default values
                }
            }

            if (quickadd)
                return;

            try
            {
                Commands.CurrentCell = Commands.Rows[e.RowIndex].Cells[0];

                if (Commands.Rows.Count > 1)
                {
                    if (Commands.Rows[e.RowIndex - 1].Cells[Command.Index].Value.ToString() == "WAYPOINT")
                    {
                        Commands.Rows[e.RowIndex].Selected = true; // highlight row
                    }
                    else
                    {
                        Commands.CurrentCell = Commands[1, e.RowIndex - 1];
                        //Commands_RowEnter(sender, new DataGridViewCellEventArgs(0, e.RowIndex-1));
                    }
                }
            }
            catch (Exception)
            {
            }
            // Commands.EndEdit();
        }

        private void Commands_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            selectedrow = e.RowIndex;
            Commands_RowEnter(sender, new DataGridViewCellEventArgs(0, e.RowIndex - 0));
                // do header labels - encure we dont 0 out valid colums
            int cols = Commands.Columns.Count;
            for (int a = 1; a < cols; a++)
            {
                DataGridViewTextBoxCell cell;
                cell = Commands.Rows[selectedrow].Cells[a] as DataGridViewTextBoxCell;

                if (Commands.Columns[a].HeaderText.Equals("") && cell != null && cell.Value == null)
                {
                    cell.Value = "0";
                }
                else
                {
                    if (cell != null && (cell.Value == null || cell.Value.ToString() == ""))
                    {
                        cell.Value = "?";
                    }
                }
            }
        }

        /// <summary>
        /// used to add a marker to the map display
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="lng"></param>
        /// <param name="lat"></param>
        /// <param name="alt"></param>
        private void addpolygonmarker(string tag, double lng, double lat, double alt, Color? color)
        {
            try
            {
                PointLatLng point = new PointLatLng(lat, lng);
                GMapMarkerWP m = new GMapMarkerWP(point, tag);
                m.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                m.ToolTipText = "Alt: " + alt.ToString("0");
                m.Tag = tag;

                int wpno = -1;
                if (int.TryParse(tag, out wpno))
                {
                    // preselect groupmarker
                    if (groupmarkers.Contains(wpno))
                        m.selected = true;
                }

                //MissionPlanner.GMapMarkerRectWPRad mBorders = new MissionPlanner.GMapMarkerRectWPRad(point, (int)float.Parse(TXT_WPRad.Text), MainMap);
                GMapMarkerRect mBorders = new GMapMarkerRect(point);
                {
                    mBorders.InnerMarker = m;
                    mBorders.Tag = tag;
                    mBorders.wprad = (int) (float.Parse(TXT_WPRad.Text)/CurrentState.multiplierdist);
                    if (color.HasValue)
                    {
                        mBorders.Color = color.Value;
                    }
                }

                objectsoverlay.Markers.Add(m);
                objectsoverlay.Markers.Add(mBorders);
            }
            catch (Exception)
            {
            }
        }

        private void addpolygonmarkergrid(string tag, double lng, double lat, int alt)
        {
            try
            {
                PointLatLng point = new PointLatLng(lat, lng);
                GMarkerGoogle m = new GMarkerGoogle(point, GMarkerGoogleType.red);
                m.ToolTipMode = MarkerTooltipMode.Never;
                m.ToolTipText = "grid" + tag;
                m.Tag = "grid" + tag;

                //MissionPlanner.GMapMarkerRectWPRad mBorders = new MissionPlanner.GMapMarkerRectWPRad(point, (int)float.Parse(TXT_WPRad.Text), MainMap);
                GMapMarkerRect mBorders = new GMapMarkerRect(point);
                {
                    mBorders.InnerMarker = m;
                }

                drawnpolygonsoverlay.Markers.Add(m);
                drawnpolygonsoverlay.Markers.Add(mBorders);
            }
            catch (Exception ex)
            {
                log.Info(ex.ToString());
            }
        }

        void updateRowNumbers()
        {
            if (this.IsDisposed || this.Disposing)
                return;

            // number rows 
            this.BeginInvoke((MethodInvoker) delegate
            {
                // thread for updateing row numbers
                for (int a = 0; a < Commands.Rows.Count - 0; a++)
                {
                    if (this.IsDisposed || this.Disposing)
                        return;
                    try
                    {
                        if (Commands.Rows[a].HeaderCell.Value == null)
                        {
                            //Commands.Rows[a].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
                            Commands.Rows[a].HeaderCell.Value = (a + 1).ToString();
                        }
                        // skip rows with the correct number
                        string rowno = Commands.Rows[a].HeaderCell.Value.ToString();
                        if (!rowno.Equals((a + 1).ToString()))
                        {
                            // this code is where the delay is when deleting.
                            Commands.Rows[a].HeaderCell.Value = (a + 1).ToString();
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            });
        }

        /// <summary>
        /// used to write a KML, update the Map view polygon, and update the row headers
        /// </summary>
        public void writeKML()
        {
            // quickadd is for when loading wps from eeprom or file, to prevent slow, loading times
            if (quickadd)
                return;

            if (Disposing)
                return;

            updateRowNumbers();

            PointLatLngAlt home = new PointLatLngAlt();
            if (TXT_homealt.Text != "" && TXT_homelat.Text != "" && TXT_homelng.Text != "")
            {
                try
                {
                    home = new PointLatLngAlt(
                        double.Parse(TXT_homelat.Text), double.Parse(TXT_homelng.Text),
                        double.Parse(TXT_homealt.Text) / CurrentState.multiplieralt, "H")
                    { Tag2 = CMB_altmode.SelectedValue.ToString() };
                }
                catch (Exception ex)
                {
                    CustomMessageBox.Show(Strings.ERROR, Strings.Invalid_home_location);
                    log.Error(ex);
                }
            }

            try
            {
                var commandlist = GetCommandList();

                var overlay = new WPOverlay();

                try
                {
                    overlay.CreateOverlay((MAVLink.MAV_FRAME) (altmode) CMB_altmode.SelectedValue, home, commandlist,
                        double.Parse(TXT_WPRad.Text) / CurrentState.multiplieralt,
                        double.Parse(TXT_loiterrad.Text) / CurrentState.multiplieralt);
                }
                catch (FormatException ex)
                {
                    CustomMessageBox.Show(Strings.ERROR, Strings.InvalidNumberEntered + "\n" +"WP Radius or Loiter Radius");
                }

                MainMap.HoldInvalidation = true;

                var existing = MainMap.Overlays.Where(a => a.Id == overlay.overlay.Id).ToList();
                foreach (var b in existing)
                {
                    MainMap.Overlays.Remove(b);
                }

                MainMap.Overlays.Insert(1, overlay.overlay);

                overlay.overlay.ForceUpdate();

                lbl_distance.Text = rm.GetString("lbl_distance.Text") + ": " +
                                    FormatDistance((
                                                       overlay.route.Points.Select(a => (PointLatLngAlt) a)
                                                           .Aggregate(0.0, (d, p1, p2) => d + p1.GetDistance(p2)) +
                                                       overlay.homeroute.Points.Select(a => (PointLatLngAlt) a)
                                                           .Aggregate(0.0, (d, p1, p2) => d + p1.GetDistance(p2))) /
                                                   1000.0, false);

                setgradanddistandaz(overlay.pointlist, home);

                if (overlay.pointlist.Count <= 1)
                {
                    RectLatLng? rect = MainMap.GetRectOfAllMarkers(overlay.overlay.Id);
                    if (rect.HasValue)
                    {
                        MainMap.Position = rect.Value.LocationMiddle;
                    }

                    MainMap_OnMapZoomChanged();
                }

                pointlist = overlay.pointlist;

                MainMap.Refresh();
            }
            catch (FormatException ex)
            {
                CustomMessageBox.Show(Strings.ERROR, Strings.InvalidNumberEntered + "\n" + ex.Message);
            }
        }

        void setgradanddistandaz(List<PointLatLngAlt> pointlist, PointLatLngAlt HomeLocation)
        {
            int a = 0;
            PointLatLngAlt last = HomeLocation;
            foreach (var lla in pointlist)
            {
                if (lla == null)
                    continue;
                try
                {
                    if (lla.Tag != null && lla.Tag != "H" && !lla.Tag.Contains("ROI"))
                    {
                        double height = lla.Alt - last.Alt;
                        double distance = lla.GetDistance(last) * CurrentState.multiplierdist;
                        double grad = height / distance;

                        Commands.Rows[int.Parse(lla.Tag) - 1].Cells[Grad.Index].Value =
                            (grad * 100).ToString("0.0");

                        Commands.Rows[int.Parse(lla.Tag) - 1].Cells[Angle.Index].Value =
                            ((180.0 / Math.PI) * Math.Atan(grad)).ToString("0.0");

                        Commands.Rows[int.Parse(lla.Tag) - 1].Cells[Dist.Index].Value =
                            (Math.Sqrt(Math.Pow(distance,2) + Math.Pow(height,2))).ToString("0.0");

                        Commands.Rows[int.Parse(lla.Tag) - 1].Cells[AZ.Index].Value =
                            ((lla.GetBearing(last) + 180)%360).ToString("0");
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
                a++;
                last = lla;
            }
        }

        /// <summary>
        /// Saves a waypoint writer file
        /// </summary>
        private void savewaypoints()
        {
            using (SaveFileDialog fd = new SaveFileDialog())
            {
                fd.Filter = "Mission|*.waypoints;*.txt|Mission JSON|*.mission";
                fd.DefaultExt = ".waypoints";
                fd.FileName = wpfilename;
                DialogResult result = fd.ShowDialog();
                string file = fd.FileName;
                if (file != "" && result == DialogResult.OK)
                {
                    try
                    {
                        if (file.EndsWith(".mission"))
                        {
                            var list = GetCommandList();
                            Locationwp home = new Locationwp();
                            try
                            {
                                home.id = (ushort)MAVLink.MAV_CMD.WAYPOINT;
                                home.lat = (double.Parse(TXT_homelat.Text));
                                home.lng = (double.Parse(TXT_homelng.Text));
                                home.alt = (float.Parse(TXT_homealt.Text) / CurrentState.multiplieralt); // use saved home
                            }
                            catch { }

                            list.Insert(0, home);

                            var format = MissionFile.ConvertFromLocationwps(list, (byte)(altmode)CMB_altmode.SelectedValue);

                            MissionFile.WriteFile(file, format);
                            return;
                        }

                        StreamWriter sw = new StreamWriter(file);
                        sw.WriteLine("QGC WPL 110");
                        try
                        {
                            sw.WriteLine("0\t1\t0\t16\t0\t0\t0\t0\t" +
                                         double.Parse(TXT_homelat.Text).ToString("0.000000", new CultureInfo("en-US")) +
                                         "\t" +
                                         double.Parse(TXT_homelng.Text).ToString("0.000000", new CultureInfo("en-US")) +
                                         "\t" +
                                         double.Parse(TXT_homealt.Text).ToString("0.000000", new CultureInfo("en-US")) +
                                         "\t1");
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex);
                            sw.WriteLine("0\t1\t0\t0\t0\t0\t0\t0\t0\t0\t0\t1");
                        }
                        for (int a = 0; a < Commands.Rows.Count - 0; a++)
                        {
                            ushort mode = 0;

                            if (Commands.Rows[a].Cells[0].Value.ToString() == "UNKNOWN")
                            {
                                mode = (ushort)Commands.Rows[a].Cells[Command.Index].Tag;
                            }
                            else
                            {
                                mode =
                                (ushort)
                                    (MAVLink.MAV_CMD)
                                        Enum.Parse(typeof(MAVLink.MAV_CMD), Commands.Rows[a].Cells[Command.Index].Value.ToString());
                            }

                            sw.Write((a + 1)); // seq
                            sw.Write("\t" + 0); // current
                            sw.Write("\t" + CMB_altmode.SelectedValue); //frame 
                            sw.Write("\t" + mode);
                            sw.Write("\t" +
                                     double.Parse(Commands.Rows[a].Cells[Param1.Index].Value.ToString())
                                         .ToString("0.00000000", new CultureInfo("en-US")));
                            sw.Write("\t" +
                                     double.Parse(Commands.Rows[a].Cells[Param2.Index].Value.ToString())
                                         .ToString("0.00000000", new CultureInfo("en-US")));
                            sw.Write("\t" +
                                     double.Parse(Commands.Rows[a].Cells[Param3.Index].Value.ToString())
                                         .ToString("0.00000000", new CultureInfo("en-US")));
                            sw.Write("\t" +
                                     double.Parse(Commands.Rows[a].Cells[Param4.Index].Value.ToString())
                                         .ToString("0.00000000", new CultureInfo("en-US")));
                            sw.Write("\t" +
                                     double.Parse(Commands.Rows[a].Cells[Lat.Index].Value.ToString())
                                         .ToString("0.00000000", new CultureInfo("en-US")));
                            sw.Write("\t" +
                                     double.Parse(Commands.Rows[a].Cells[Lon.Index].Value.ToString())
                                         .ToString("0.00000000", new CultureInfo("en-US")));
                            sw.Write("\t" +
                                     (double.Parse(Commands.Rows[a].Cells[Alt.Index].Value.ToString())/
                                      CurrentState.multiplieralt).ToString("0.000000", new CultureInfo("en-US")));
                            sw.Write("\t" + 1);
                            sw.WriteLine("");
                        }
                        sw.Close();

                        lbl_wpfile.Text = "Saved " + Path.GetFileName(file);
                    }
                    catch (Exception)
                    {
                        CustomMessageBox.Show(Strings.ERROR);
                    }
                }
            }
        }

        private void SaveFile_Click(object sender, EventArgs e)
        {
            savewaypoints();
            writeKML();
        }

        /// <summary>
        /// Reads the EEPROM from a com port
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void BUT_read_Click(object sender, EventArgs e)
        {
            if (Commands.Rows.Count > 0)
            {
                if (sender is FlightData)
                {
                }
                else
                {
                    if (
                        CustomMessageBox.Show("This will clear your existing planned mission, Continue?", "Confirm",
                            MessageBoxButtons.OKCancel) != (int)DialogResult.OK)
                    {
                        return;
                    }
                }
            }

            IProgressReporterDialogue frmProgressReporter = new ProgressReporterDialogue
            {
                StartPosition = FormStartPosition.CenterScreen,
                Text = "Receiving WP's"
            };

            frmProgressReporter.DoWork += getWPs;
            frmProgressReporter.UpdateProgressAndStatus(-1, "Receiving WP's");

            ThemeManager.ApplyThemeTo(frmProgressReporter);

            frmProgressReporter.RunBackgroundOperationAsync();

            frmProgressReporter.Dispose();
        }

        void getWPs(IProgressReporterDialogue sender)
        {
            List<Locationwp> cmds = new List<Locationwp>();

            try
            {
                MAVLinkInterface port = MainV2.comPort;

                if (!port.BaseStream.IsOpen)
                {
                    throw new Exception("Please Connect First!");
                }

                MainV2.comPort.giveComport = true;

                log.Info("Getting Home");

                ((ProgressReporterDialogue) sender).UpdateProgressAndStatus(0, "Getting WP count");

                if (port.MAV.apname == MAVLink.MAV_AUTOPILOT.PX4)
                {
                    try
                    {
                        cmds.Add(port.getHomePosition());
                    }
                    catch (TimeoutException)
                    {
                        // blank home
                        cmds.Add(new Locationwp() { id = (ushort)MAVLink.MAV_CMD.WAYPOINT });
                    }
                }

                log.Info("Getting WP #");

                int cmdcount = port.getWPCount();

                for (ushort a = 0; a < cmdcount; a++)
                {
                    if (((ProgressReporterDialogue) sender).doWorkArgs.CancelRequested)
                    {
                        ((ProgressReporterDialogue) sender).doWorkArgs.CancelAcknowledged = true;
                        throw new Exception("Cancel Requested");
                    }

                    log.Info("Getting WP" + a);
                    ((ProgressReporterDialogue) sender).UpdateProgressAndStatus(a*100/cmdcount, "Getting WP " + a);
                    cmds.Add(port.getWP(a));
                }

                port.setWPACK();

                ((ProgressReporterDialogue) sender).UpdateProgressAndStatus(100, "Done");

                log.Info("Done");
            }
            catch
            {
                throw;
            }

            WPtoScreen(cmds);
        }

        public void WPtoScreen(List<Locationwp> cmds, bool withrally = true)
        {
            try
            {
                Invoke((MethodInvoker) delegate
                {
                    try
                    {
                        log.Info("Process " + cmds.Count);
                        processToScreen(cmds);
                    }
                    catch (Exception exx)
                    {
                        log.Info(exx.ToString());
                    }

                    try
                    {
                        if (withrally && MainV2.comPort.MAV.param.ContainsKey("RALLY_TOTAL") &&
                            int.Parse(MainV2.comPort.MAV.param["RALLY_TOTAL"].ToString()) >= 1)
                            getRallyPointsToolStripMenuItem_Click(null, null);
                    }
                    catch
                    {
                    }

                    MainV2.comPort.giveComport = false;

                    BUT_read.Enabled = true;

                    writeKML();
                });
            }
            catch (Exception exx)
            {
                log.Info(exx.ToString());
            }
        }

        /// <summary>
        /// Writes the mission from the datagrid and values to the EEPROM
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void BUT_write_Click(object sender, EventArgs e)
        {
            if ((altmode) CMB_altmode.SelectedValue == altmode.Absolute)
            {
                if ((int)DialogResult.No ==
                    CustomMessageBox.Show("Absolute Alt is selected are you sure?", "Alt Mode", MessageBoxButtons.YesNo))
                {
                    CMB_altmode.SelectedValue = (int) altmode.Relative;
                }
            }

            // check home
            Locationwp home = new Locationwp();
            try
            {
                home.id = (ushort)MAVLink.MAV_CMD.WAYPOINT;
                home.lat = (double.Parse(TXT_homelat.Text));
                home.lng = (double.Parse(TXT_homelng.Text));
                home.alt = (float.Parse(TXT_homealt.Text) / CurrentState.multiplierdist); // use saved home
            }
            catch
            {
                CustomMessageBox.Show("Your home location is invalid", Strings.ERROR);
                return;
            }

            // check for invalid grid data
            for (int a = 0; a < Commands.Rows.Count - 0; a++)
            {
                for (int b = 0; b < Commands.ColumnCount - 0; b++)
                {
                    double answer;
                    if (b >= 1 && b <= 7)
                    {
                        if (!double.TryParse(Commands[b, a].Value.ToString(), out answer))
                        {
                            CustomMessageBox.Show("There are errors in your mission");
                            return;
                        }
                    }

                    if (TXT_altwarn.Text == "")
                        TXT_altwarn.Text = (0).ToString();

                    if (Commands.Rows[a].Cells[Command.Index].Value.ToString().Contains("UNKNOWN"))
                        continue;

                    ushort cmd =
                        (ushort)
                                Enum.Parse(typeof (MAVLink.MAV_CMD),
                                    Commands.Rows[a].Cells[Command.Index].Value.ToString(), false);

                    if (cmd < (ushort) MAVLink.MAV_CMD.LAST &&
                        double.Parse(Commands[Alt.Index, a].Value.ToString()) < double.Parse(TXT_altwarn.Text))
                    {
                        if (cmd != (ushort) MAVLink.MAV_CMD.TAKEOFF &&
                            cmd != (ushort) MAVLink.MAV_CMD.LAND &&
                            cmd != (ushort) MAVLink.MAV_CMD.RETURN_TO_LAUNCH)
                        {
                            CustomMessageBox.Show("Low alt on WP#" + (a + 1) +
                                                  "\nPlease reduce the alt warning, or increase the altitude");
                            return;
                        }
                    }
                }
            }

            IProgressReporterDialogue frmProgressReporter = new ProgressReporterDialogue
            {
                StartPosition = FormStartPosition.CenterScreen,
                Text = "Sending WP's"
            };

            frmProgressReporter.DoWork += saveWPs;

            frmProgressReporter.UpdateProgressAndStatus(-1, "Sending WP's");

            ThemeManager.ApplyThemeTo(frmProgressReporter);

            frmProgressReporter.RunBackgroundOperationAsync();

            frmProgressReporter.Dispose();

            MainMap.Focus();
        }

        Locationwp DataViewtoLocationwp(int a)
        {
            try
            {
                Locationwp temp = new Locationwp();
                if (Commands.Rows[a].Cells[Command.Index].Value.ToString().Contains("UNKNOWN"))
                {
                    temp.id = (ushort)Commands.Rows[a].Cells[Command.Index].Tag;
                }
                else
                {
                    temp.id =
                        (ushort) 
                                Enum.Parse(typeof (MAVLink.MAV_CMD),
                                    Commands.Rows[a].Cells[Command.Index].Value.ToString(),
                                    false);
                }
                temp.p1 = float.Parse(Commands.Rows[a].Cells[Param1.Index].Value.ToString());

                temp.alt =
                    (float)
                        (double.Parse(Commands.Rows[a].Cells[Alt.Index].Value.ToString())/CurrentState.multiplieralt);
                temp.lat = (double.Parse(Commands.Rows[a].Cells[Lat.Index].Value.ToString()));
                temp.lng = (double.Parse(Commands.Rows[a].Cells[Lon.Index].Value.ToString()));

                temp.p2 = (float) (double.Parse(Commands.Rows[a].Cells[Param2.Index].Value.ToString()));
                temp.p3 = (float) (double.Parse(Commands.Rows[a].Cells[Param3.Index].Value.ToString()));
                temp.p4 = (float) (double.Parse(Commands.Rows[a].Cells[Param4.Index].Value.ToString()));

                temp.Tag = Commands.Rows[a].Cells[TagData.Index].Value;

                return temp;
            }
            catch (Exception ex)
            {
                throw new FormatException("Invalid number on row " + (a + 1).ToString(), ex);
            }
        }

        internal IList<Locationwp> GetFlightPlanLocations()
        {
            return GetCommandList().AsReadOnly();
        }

        List<Locationwp> GetCommandList()
        {
            List<Locationwp> commands = new List<Locationwp>();

            for (int a = 0; a < Commands.Rows.Count - 0; a++)
            {
                var temp = DataViewtoLocationwp(a);

                commands.Add(temp);
            }

            return commands;
        }

        void saveWPsFast(IProgressReporterDialogue sender)
        {
            var totalwpcountforupload = (ushort) (Commands.RowCount + 1);
            var reqno = 0;
            MAVLink.MAV_MISSION_RESULT result = MAVLink.MAV_MISSION_RESULT.MAV_MISSION_ACCEPTED;

            var sub1 = MainV2.comPort.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.MISSION_ACK,
                message =>
                {
                    var data = ((MAVLink.mavlink_mission_ack_t) message.data);
                    var ans = (MAVLink.MAV_MISSION_RESULT) data.type;
                    if (MainV2.comPort.MAV.sysid != message.sysid &&
                        MainV2.comPort.MAV.compid != message.compid)
                        return true;
                    result = ans;
                    Console.WriteLine("MISSION_ACK " + ans);
                    return true;
                });

            var sub2 = MainV2.comPort.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.MISSION_REQUEST,
                message =>
                {
                    var data = ((MAVLink.mavlink_mission_request_t)message.data);
                    // check what we sent is what the message is.
                    if (MainV2.comPort.MAV.sysid != message.sysid &&
                        MainV2.comPort.MAV.compid != message.compid)
                        return true;
                    // check this gcs sent it
                    if (data.target_system != MAVLinkInterface.gcssysid ||
                        data.target_component != (byte) MAVLink.MAV_COMPONENT.MAV_COMP_ID_MISSIONPLANNER)
                        return true;
                    reqno = data.seq;
                    Console.WriteLine("MISSION_REQUEST " + reqno);
                    return true;
                });

            ((ProgressReporterDialogue)sender).UpdateProgressAndStatus(0, "Set total wps ");
            MainV2.comPort.setWPTotal(totalwpcountforupload);

            // define the home point
            Locationwp home = new Locationwp();
            try
            {
                home.id = (ushort)MAVLink.MAV_CMD.WAYPOINT;
                home.lat = (double.Parse(TXT_homelat.Text));
                home.lng = (double.Parse(TXT_homelng.Text));
                home.alt = (float.Parse(TXT_homealt.Text) / CurrentState.multiplieralt); // use saved home
            }
            catch
            {
                MainV2.comPort.UnSubscribeToPacketType(sub1);
                MainV2.comPort.UnSubscribeToPacketType(sub2);
                throw new Exception("Your home location is invalid");
            }

            // define the default frame.
            MAVLink.MAV_FRAME frame = MAVLink.MAV_FRAME.GLOBAL_RELATIVE_ALT;

            // get the command list from the datagrid
            var commandlist = GetCommandList();

            commandlist.Insert(0, home);

            // process commandlist to the mav
            for (var a = 0; a < commandlist.Count; a++)
            {
                if (a % 10 == 0 && a != 0)
                {
                    var start = DateTime.Now;
                    while (true)
                    {
                        if (sender.doWorkArgs.CancelRequested)
                        {
                            MainV2.comPort.setWPTotal(0);
                            MainV2.comPort.UnSubscribeToPacketType(sub1);
                            MainV2.comPort.UnSubscribeToPacketType(sub2);
                            return;
                        }

                        if (reqno == a)
                        {
                            // all received
                            break;
                        }

                        if (start.AddSeconds(1.1) < DateTime.Now)
                        {
                            // do next 10 starting at reqno
                            a = reqno;
                            break;
                        }

                        if (result == MAVLink.MAV_MISSION_RESULT.MAV_MISSION_INVALID_SEQUENCE)
                            Thread.Sleep(500);

                        if (result == MAVLink.MAV_MISSION_RESULT.MAV_MISSION_ERROR)
                        {
                            // resend for partial upload
                            MainV2.comPort.setWPPartialUpdate((ushort) (reqno), totalwpcountforupload);
                            a = reqno;
                            break;
                        }

                        if (result == MAVLink.MAV_MISSION_RESULT.MAV_MISSION_NO_SPACE)
                        {
                            sender.doWorkArgs.ErrorMessage = "Upload failed, please reduce the number of wp's";
                            MainV2.comPort.UnSubscribeToPacketType(sub1);
                            MainV2.comPort.UnSubscribeToPacketType(sub2);
                            return;
                        }
                        if (result == MAVLink.MAV_MISSION_RESULT.MAV_MISSION_INVALID)
                        {
                            sender.doWorkArgs.ErrorMessage =
                                "Upload failed, mission was rejected byt the Mav,\n item had a bad option wp# " + a + " " +
                                result;
                            MainV2.comPort.UnSubscribeToPacketType(sub1);
                            MainV2.comPort.UnSubscribeToPacketType(sub2);
                            return;
                        }
                        if (result != MAVLink.MAV_MISSION_RESULT.MAV_MISSION_ACCEPTED)
                        {
                            sender.doWorkArgs.ErrorMessage = "Upload wps failed " + reqno +
                                             " " + Enum.Parse(typeof(MAVLink.MAV_MISSION_RESULT), result.ToString());
                            MainV2.comPort.UnSubscribeToPacketType(sub1);
                            MainV2.comPort.UnSubscribeToPacketType(sub2);
                            return;
                        }

                        System.Threading.Thread.Sleep(10);
                    }
                }

                var loc = commandlist[a];

                // make sure we are using the correct frame for these commands
                if (loc.id < (ushort)MAVLink.MAV_CMD.LAST || loc.id == (ushort)MAVLink.MAV_CMD.DO_SET_HOME)
                {
                    var mode = currentaltmode;

                    if (mode == altmode.Terrain)
                    {
                        frame = MAVLink.MAV_FRAME.GLOBAL_TERRAIN_ALT;
                    }
                    else if (mode == altmode.Absolute)
                    {
                        frame = MAVLink.MAV_FRAME.GLOBAL;
                    }
                    else
                    {
                        frame = MAVLink.MAV_FRAME.GLOBAL_RELATIVE_ALT;
                    }
                }

                MAVLink.mavlink_mission_item_int_t req = new MAVLink.mavlink_mission_item_int_t();

                req.target_system = MainV2.comPort.MAV.sysid;
                req.target_component = MainV2.comPort.MAV.compid;

                req.command = loc.id;

                req.current = 0;
                req.autocontinue = 1;

                req.frame = (byte)frame;
                if (loc.id == (ushort)MAVLink.MAV_CMD.DO_DIGICAM_CONTROL || loc.id == (ushort)MAVLink.MAV_CMD.DO_DIGICAM_CONFIGURE)
                {
                    req.y = (int)(loc.lng);
                    req.x = (int)(loc.lat);
                }
                else
                {
                    req.y = (int)(loc.lng * 1.0e7);
                    req.x = (int)(loc.lat * 1.0e7);
                }
                req.z = (float)(loc.alt);

                req.param1 = loc.p1;
                req.param2 = loc.p2;
                req.param3 = loc.p3;
                req.param4 = loc.p4;

                req.seq = (ushort)a;

                ((ProgressReporterDialogue) sender).UpdateProgressAndStatus(a * 100 / Commands.Rows.Count,
                    "Setting WP " + a);
                Console.WriteLine("WP no " + a);

                MainV2.comPort.sendPacket(req, MainV2.comPort.MAV.sysid, MainV2.comPort.MAV.compid);
            }

            MainV2.comPort.UnSubscribeToPacketType(sub1);
            MainV2.comPort.UnSubscribeToPacketType(sub2);

            MainV2.comPort.setWPACK();
        }

        void saveWPs(IProgressReporterDialogue sender)
        {
            try
            {
                MAVLinkInterface port = MainV2.comPort;

                if (!port.BaseStream.IsOpen)
                {
                    throw new Exception("Please connect first!");
                }

                MainV2.comPort.giveComport = true;
                int a = 0;

                // define the home point
                Locationwp home = new Locationwp();
                try
                {
                    home.id = (ushort)MAVLink.MAV_CMD.WAYPOINT;
                    home.lat = (double.Parse(TXT_homelat.Text));
                    home.lng = (double.Parse(TXT_homelng.Text));
                    home.alt = (float.Parse(TXT_homealt.Text)/CurrentState.multiplieralt); // use saved home
                }
                catch
                {
                    throw new Exception("Your home location is invalid");
                }

                // log
                log.Info("wps values " + MainV2.comPort.MAV.wps.Values.Count);
                log.Info("cmd rows " + (Commands.Rows.Count + 1)); // + home

                // check for changes / future mod to send just changed wp's
                if (MainV2.comPort.MAV.wps.Values.Count == (Commands.Rows.Count + 1))
                {
                    Hashtable wpstoupload = new Hashtable();

                    a = -1;
                    foreach (var item in MainV2.comPort.MAV.wps.Values)
                    {
                        // skip home
                        if (a == -1)
                        {
                            a++;
                            continue;
                        }

                        MAVLink.mavlink_mission_item_t temp = DataViewtoLocationwp(a);

                        if (temp.command == item.command &&
                            temp.x == item.x &&
                            temp.y == item.y &&
                            temp.z == item.z &&
                            temp.param1 == item.param1 &&
                            temp.param2 == item.param2 &&
                            temp.param3 == item.param3 &&
                            temp.param4 == item.param4
                            )
                        {
                            log.Info("wp match " + (a + 1));
                        }
                        else
                        {
                            log.Info("wp no match" + (a + 1));
                            wpstoupload[a] = "";
                        }

                        a++;
                    }
                }

                bool use_int = (port.MAV.cs.capabilities & (uint)MAVLink.MAV_PROTOCOL_CAPABILITY.MISSION_INT) > 0;

                // set wp total
                ((ProgressReporterDialogue)sender).UpdateProgressAndStatus(0, "Set total wps ");

                ushort totalwpcountforupload = (ushort)(Commands.Rows.Count + 1);

                if (port.MAV.apname == MAVLink.MAV_AUTOPILOT.PX4)
                {
                    totalwpcountforupload--;
                }

                port.setWPTotal(totalwpcountforupload); // + home

                // set home location - overwritten/ignored depending on firmware.
                ((ProgressReporterDialogue)sender).UpdateProgressAndStatus(0, "Set home");

                // upload from wp0
                a = 0;

                if (port.MAV.apname != MAVLink.MAV_AUTOPILOT.PX4)
                {
                    try
                    {
                        var homeans = port.setWP(home, (ushort)a, MAVLink.MAV_FRAME.GLOBAL, 0, 1, use_int);
                        if (homeans != MAVLink.MAV_MISSION_RESULT.MAV_MISSION_ACCEPTED)
                        {
                            if (homeans != MAVLink.MAV_MISSION_RESULT.MAV_MISSION_INVALID_SEQUENCE)
                            {
                                CustomMessageBox.Show(Strings.ErrorRejectedByMAV, Strings.ERROR);
                                return;
                            }
                        }
                        a++;
                    }
                    catch (TimeoutException)
                    {
                        use_int = false;
                        // added here to prevent timeout errors
                        port.setWPTotal(totalwpcountforupload);
                        var homeans = port.setWP(home, (ushort)a, MAVLink.MAV_FRAME.GLOBAL, 0, 1, use_int);
                        if (homeans != MAVLink.MAV_MISSION_RESULT.MAV_MISSION_ACCEPTED)
                        {
                            if (homeans != MAVLink.MAV_MISSION_RESULT.MAV_MISSION_INVALID_SEQUENCE)
                            {
                                CustomMessageBox.Show(Strings.ErrorRejectedByMAV, Strings.ERROR);
                                return;
                            }
                        }
                        a++;
                    }
                }
                else
                {
                    use_int = false;
                }

                // define the default frame.
                MAVLink.MAV_FRAME frame = MAVLink.MAV_FRAME.GLOBAL_RELATIVE_ALT;

                // get the command list from the datagrid
                var commandlist = GetCommandList();

                // process commandlist to the mav
                for (a = 1; a <= commandlist.Count; a++)
                {
                    var temp = commandlist[a-1];

                    ((ProgressReporterDialogue) sender).UpdateProgressAndStatus(a*100/Commands.Rows.Count,
                        "Setting WP " + a);

                    // make sure we are using the correct frame for these commands
                    if (temp.id < (ushort)MAVLink.MAV_CMD.LAST || temp.id == (ushort)MAVLink.MAV_CMD.DO_SET_HOME)
                    {
                        var mode = currentaltmode;

                        if (mode == altmode.Terrain)
                        {
                            frame = MAVLink.MAV_FRAME.GLOBAL_TERRAIN_ALT;
                        }
                        else if (mode == altmode.Absolute)
                        {
                            frame = MAVLink.MAV_FRAME.GLOBAL;
                        }
                        else
                        {
                            frame = MAVLink.MAV_FRAME.GLOBAL_RELATIVE_ALT;
                        }
                    }

                    // handle current wp upload number
                    int uploadwpno = a;
                    if (port.MAV.apname == MAVLink.MAV_AUTOPILOT.PX4)
                        uploadwpno--;

                    // try send the wp
                    MAVLink.MAV_MISSION_RESULT ans = port.setWP(temp, (ushort)(uploadwpno), frame, 0, 1, use_int);

                    // we timed out while uploading wps/ command wasnt replaced/ command wasnt added
                    if (ans == MAVLink.MAV_MISSION_RESULT.MAV_MISSION_ERROR)
                    {
                        // resend for partial upload
                        port.setWPPartialUpdate((ushort) (uploadwpno), totalwpcountforupload);
                        // reupload this point.
                        ans = port.setWP(temp, (ushort) (uploadwpno), frame, 0, 1, use_int);
                    }

                    if (ans == MAVLink.MAV_MISSION_RESULT.MAV_MISSION_NO_SPACE)
                    {
                        sender.doWorkArgs.ErrorMessage = "Upload failed, please reduce the number of wp's";
                        return;
                    }
                    if (ans == MAVLink.MAV_MISSION_RESULT.MAV_MISSION_INVALID)
                    {
                        sender.doWorkArgs.ErrorMessage =
                            "Upload failed, mission was rejected byt the Mav,\n item had a bad option wp# " + a + " " +
                            ans;
                        return;
                    }
                    if (ans == MAVLink.MAV_MISSION_RESULT.MAV_MISSION_INVALID_SEQUENCE)
                    {
                        // invalid sequence can only occur if we failed to see a response from the apm when we sent the request.
                        // or there is io lag and we send 2 mission_items and get 2 responces, one valid, one a ack of the second send
                        
                        // the ans is received via mission_ack, so we dont know for certain what our current request is for. as we may have lost the mission_request

                        // get requested wp no - 1;
                        a = port.getRequestedWPNo() - 1;

                        continue;
                    }
                    if (ans != MAVLink.MAV_MISSION_RESULT.MAV_MISSION_ACCEPTED)
                    {
                        sender.doWorkArgs.ErrorMessage = "Upload wps failed " + Enum.Parse(typeof (MAVLink.MAV_CMD), temp.id.ToString()) +
                                         " " + Enum.Parse(typeof (MAVLink.MAV_MISSION_RESULT), ans.ToString());
                        return;
                    }
                }

                port.setWPACK();

                ((ProgressReporterDialogue) sender).UpdateProgressAndStatus(95, "Setting params");

                // m
                port.setParam("WP_RADIUS", float.Parse(TXT_WPRad.Text)/CurrentState.multiplierdist);

                // cm's
                port.setParam("WPNAV_RADIUS", float.Parse(TXT_WPRad.Text)/CurrentState.multiplierdist*100.0);

                try
                {
                    port.setParam(new[] {"LOITER_RAD", "WP_LOITER_RAD"},
                        float.Parse(TXT_loiterrad.Text)/CurrentState.multiplierdist);
                }
                catch
                {
                }

                ((ProgressReporterDialogue) sender).UpdateProgressAndStatus(100, "Done.");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                MainV2.comPort.giveComport = false;
                throw;
            }

            MainV2.comPort.giveComport = false;
        }

        /// <summary>
        /// Processes a loaded EEPROM to the map and datagrid
        /// </summary>
        void processToScreen(List<Locationwp> cmds, bool append = false)
        {
            quickadd = true;


            // mono fix
            Commands.CurrentCell = null;

            while (Commands.Rows.Count > 0 && !append)
                Commands.Rows.Clear();

            if (cmds.Count == 0)
            {
                quickadd = false;
                return;
            }

            Commands.SuspendLayout();
            Commands.Enabled = false;

            int i = Commands.Rows.Count - 1;
            int cmdidx = -1;
            foreach (Locationwp temp in cmds)
            {
                i++;
                cmdidx++;
                //Console.WriteLine("FP processToScreen " + i);
                if (temp.id == 0 && i != 0) // 0 and not home
                    break;
                if (temp.id == 255 && i != 0) // bad record - never loaded any WP's - but have started the board up.
                    break;
                if (cmdidx == 0 && append)
                {
                    // we dont want to add home again.
                    i--;
                    continue;
                }
                if (i + 1 >= Commands.Rows.Count)
                {
                    selectedrow = Commands.Rows.Add();
                }
                //if (i == 0 && temp.alt == 0) // skip 0 home
                //  continue;
                DataGridViewTextBoxCell cell;
                DataGridViewComboBoxCell cellcmd;
                cellcmd = Commands.Rows[i].Cells[Command.Index] as DataGridViewComboBoxCell;
                cellcmd.Value = "UNKNOWN";
                cellcmd.Tag = temp.id;

                foreach (object value in Enum.GetValues(typeof (MAVLink.MAV_CMD)))
                {
                    if ((ushort) value == temp.id)
                    {
                        cellcmd.Value = value.ToString();
                        break;
                    }
                }

                // from ap_common.h
                if (temp.id == (ushort)MAVLink.MAV_CMD.WAYPOINT || temp.id == (ushort)MAVLink.MAV_CMD.SPLINE_WAYPOINT ||
                    temp.id == (ushort)MAVLink.MAV_CMD.TAKEOFF || temp.id == (ushort) MAVLink.MAV_CMD.DO_SET_HOME)
                {
                    // not home
                    if (i != 0)
                    {
                        // check relative and terrain flags
                        if ((temp.options & 0x9) == 0)
                        {
                            CMB_altmode.SelectedValue = (int) altmode.Absolute;
                        } // check terrain flag
                        else if ((temp.options & 0x8) != 0)
                        {
                            CMB_altmode.SelectedValue = (int) altmode.Terrain;
                        } // check relative flag
                        else if ((temp.options & 0x1) != 0 )
                        {
                            CMB_altmode.SelectedValue = (int) altmode.Relative;
                        }
                    }
                }

                cell = Commands.Rows[i].Cells[Alt.Index] as DataGridViewTextBoxCell;
                cell.Value = temp.alt*CurrentState.multiplieralt;
                cell = Commands.Rows[i].Cells[Lat.Index] as DataGridViewTextBoxCell;
                cell.Value = temp.lat;
                cell = Commands.Rows[i].Cells[Lon.Index] as DataGridViewTextBoxCell;
                cell.Value = temp.lng;

                cell = Commands.Rows[i].Cells[Param1.Index] as DataGridViewTextBoxCell;
                cell.Value = temp.p1;
                cell = Commands.Rows[i].Cells[Param2.Index] as DataGridViewTextBoxCell;
                cell.Value = temp.p2;
                cell = Commands.Rows[i].Cells[Param3.Index] as DataGridViewTextBoxCell;
                cell.Value = temp.p3;
                cell = Commands.Rows[i].Cells[Param4.Index] as DataGridViewTextBoxCell;
                cell.Value = temp.p4;

                // convert to utm/other
                convertFromGeographic(temp.lat, temp.lng);
            }

            Commands.Enabled = true;
            Commands.ResumeLayout();

            setWPParams();

            if (!append)
            {
                try
                {
                    DataGridViewTextBoxCell cellhome;
                    cellhome = Commands.Rows[0].Cells[Lat.Index] as DataGridViewTextBoxCell;
                    if (cellhome.Value != null)
                    {
                        if (cellhome.Value.ToString() != TXT_homelat.Text && cellhome.Value.ToString() != "0")
                        {
                            var dr = CustomMessageBox.Show("Reset Home to loaded coords", "Reset Home Coords",
                                MessageBoxButtons.YesNo);

                            if (dr == (int)DialogResult.Yes)
                            {
                                TXT_homelat.Text = (double.Parse(cellhome.Value.ToString())).ToString();
                                cellhome = Commands.Rows[0].Cells[Lon.Index] as DataGridViewTextBoxCell;
                                TXT_homelng.Text = (double.Parse(cellhome.Value.ToString())).ToString();
                                cellhome = Commands.Rows[0].Cells[Alt.Index] as DataGridViewTextBoxCell;
                                TXT_homealt.Text =
                                    (double.Parse(cellhome.Value.ToString())*CurrentState.multiplieralt).ToString();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex.ToString());
                } // if there is no valid home

                if (Commands.RowCount > 0)
                {
                    log.Info("remove home from list");
                    Commands.Rows.Remove(Commands.Rows[0]); // remove home row
                }
            }

            quickadd = false;

            writeKML();

            MainMap.ZoomAndCenterMarkers("WPOverlay");

            MainMap_OnMapZoomChanged();
        }

        void setWPParams()
        {
            try
            {
                log.Info("Loading wp params");

                Dictionary<string, double> param = new Dictionary<string, double>((Dictionary<string, double>) MainV2.comPort.MAV.param);

                if (param.ContainsKey("WP_RADIUS"))
                {
                    TXT_WPRad.Text = (((double) param["WP_RADIUS"]*CurrentState.multiplierdist)).ToString();
                }
                if (param.ContainsKey("WPNAV_RADIUS") )
                {
                    TXT_WPRad.Text = (((double) param["WPNAV_RADIUS"]*CurrentState.multiplierdist/100.0)).ToString();
                }

                log.Info("param WP_RADIUS " + TXT_WPRad.Text);

                try
                {
                    TXT_loiterrad.Enabled = false;
                    if (param.ContainsKey("LOITER_RADIUS"))
                    {
                        TXT_loiterrad.Text = (((double) param["LOITER_RADIUS"]*CurrentState.multiplierdist)).ToString();
                        TXT_loiterrad.Enabled = true;
                    }
                    else if (param.ContainsKey("WP_LOITER_RAD"))
                    {
                        TXT_loiterrad.Text = (((double) param["WP_LOITER_RAD"]*CurrentState.multiplierdist)).ToString();
                        TXT_loiterrad.Enabled = true;
                    }

                    log.Info("param LOITER_RADIUS " + TXT_loiterrad.Text);
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        /// <summary>
        /// Saves this forms config to MAIN, where it is written in a global config
        /// </summary>
        /// <param name="write">true/false</param>
        private void config(bool write)
        {
            if (write)
            {
                Settings.Instance["TXT_homelat"] = TXT_homelat.Text;
                Settings.Instance["TXT_homelng"] = TXT_homelng.Text;
                Settings.Instance["TXT_homealt"] = TXT_homealt.Text;


                Settings.Instance["TXT_WPRad"] = TXT_WPRad.Text;

                Settings.Instance["TXT_loiterrad"] = TXT_loiterrad.Text;

                Settings.Instance["TXT_DefaultAlt"] = TXT_DefaultAlt.Text;

                Settings.Instance["CMB_altmode"] = CMB_altmode.Text;

                Settings.Instance["fpminaltwarning"] = TXT_altwarn.Text;

                Settings.Instance["fpcoordmouse"] = coords1.System;
            }
            else
            {
                foreach (string key in Settings.Instance.Keys)
                {
                    switch (key)
                    {
                        case "TXT_WPRad":
                            TXT_WPRad.Text = "" + Settings.Instance[key];
                            break;
                        case "TXT_loiterrad":
                            TXT_loiterrad.Text = "" + Settings.Instance[key];
                            break;
                        case "TXT_DefaultAlt":
                            TXT_DefaultAlt.Text = "" + Settings.Instance[key];
                            break;
                        case "CMB_altmode":
                            CMB_altmode.Text = "" + Settings.Instance[key];
                            break;
                        case "fpminaltwarning":
                            TXT_altwarn.Text = "" + Settings.Instance["fpminaltwarning"];
                            break;
                        case "fpcoordmouse":
                            coords1.System = "" + Settings.Instance[key];
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void TXT_WPRad_KeyPress(object sender, KeyPressEventArgs e)
        {
            float isNumber = 0;
            if (e.KeyChar.ToString() == "\b")
                return;
            e.Handled = !float.TryParse(e.KeyChar.ToString(), out isNumber);
        }

        private void TXT_WPRad_Leave(object sender, EventArgs e)
        {
            float isNumber = 0;
            if (!float.TryParse(TXT_WPRad.Text, out isNumber))
            {
                TXT_WPRad.Text = "30";
            }
            if (isNumber > (127*CurrentState.multiplierdist))
            {
                //CustomMessageBox.Show("The value can only be between 0 and 127 m");
                //TXT_WPRad.Text = (127 * CurrentState.multiplierdist).ToString();
            }
            writeKML();
        }

        private void TXT_loiterrad_KeyPress(object sender, KeyPressEventArgs e)
        {
            float isNumber = 0;
            if (e.KeyChar.ToString() == "\b")
                return;

            if (e.KeyChar == '-')
                return;

            e.Handled = !float.TryParse(e.KeyChar.ToString(), out isNumber);
        }

        private void TXT_loiterrad_Leave(object sender, EventArgs e)
        {
            float isNumber = 0;
            if (!float.TryParse(TXT_loiterrad.Text, out isNumber))
            {
                TXT_loiterrad.Text = "45";
            }
        }

        private void TXT_DefaultAlt_KeyPress(object sender, KeyPressEventArgs e)
        {
            float isNumber = 0;
            if (e.KeyChar.ToString() == "\b")
                return;
            e.Handled = !float.TryParse(e.KeyChar.ToString(), out isNumber);
        }

        private void TXT_DefaultAlt_Leave(object sender, EventArgs e)
        {
            float isNumber = 0;
            if (!float.TryParse(TXT_DefaultAlt.Text, out isNumber))
            {
                TXT_DefaultAlt.Text = "100";
            }
        }


        /// <summary>
        /// used to control buttons in the datagrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Commands_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0)
                    return;
                if (e.ColumnIndex == Delete.Index && (e.RowIndex + 0) < Commands.RowCount) // delete
                {
                    quickadd = true;
                    // mono fix
                    Commands.CurrentCell = null;
                    Commands.Rows.RemoveAt(e.RowIndex);
                    quickadd = false;
                    writeKML();
                }
                if (e.ColumnIndex == Up.Index && e.RowIndex != 0) // up
                {
                    DataGridViewRow myrow = Commands.CurrentRow;
                    Commands.Rows.Remove(myrow);
                    Commands.Rows.Insert(e.RowIndex - 1, myrow);
                    writeKML();
                }
                if (e.ColumnIndex == Down.Index && e.RowIndex < Commands.RowCount - 1) // down
                {
                    DataGridViewRow myrow = Commands.CurrentRow;
                    Commands.Rows.Remove(myrow);
                    Commands.Rows.Insert(e.RowIndex + 1, myrow);
                    writeKML();
                }
            }
            catch (Exception)
            {
                CustomMessageBox.Show("Row error");
            }
        }

        private void Commands_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells[Delete.Index].Value = "X";
            e.Row.Cells[Up.Index].Value = Resources.up;
            e.Row.Cells[Down.Index].Value = Resources.down;
        }

        private void TXT_homelat_TextChanged(object sender, EventArgs e)
        {
            sethome = false;
            try
            {
                MainV2.comPort.MAV.cs.HomeLocation.Lat = double.Parse(TXT_homelat.Text);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            writeKML();
        }

        private void TXT_homelng_TextChanged(object sender, EventArgs e)
        {
            sethome = false;
            try
            {
                MainV2.comPort.MAV.cs.HomeLocation.Lng = double.Parse(TXT_homelng.Text);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            writeKML();
        }

        private void TXT_homealt_TextChanged(object sender, EventArgs e)
        {
            sethome = false;
            try
            {
                MainV2.comPort.MAV.cs.HomeLocation.Alt = double.Parse(TXT_homealt.Text);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            writeKML();
        }

        private void BUT_loadwpfile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog fd = new OpenFileDialog())
            {
                fd.Filter = "All Supported Types|*.txt;*.waypoints;*.shp;*.plan";
                DialogResult result = fd.ShowDialog();
                string file = fd.FileName;

                if (File.Exists(file))
                {
                    if (file.ToLower().EndsWith(".shp"))
                    {
                        LoadSHPFile(file);
                    }
                    else
                    {
                        string line = "";
                        using (var fs = File.OpenText(file))
                        {
                            line = fs.ReadLine();
                        }

                        if (line.StartsWith("{"))
                        {
                            var format = MissionFile.ReadFile(file);

                            var cmds = MissionFile.ConvertToLocationwps(format);

                            processToScreen(cmds);

                            writeKML();

                            MainMap.ZoomAndCenterMarkers("WPOverlay");
                        }
                        else
                        {
                            wpfilename = file;
                            readQGC110wpfile(file);
                        }
                    }

                    lbl_wpfile.Text = "Loaded " + Path.GetFileName(file);
                }
            }
        }

        public void readQGC110wpfile(string file, bool append = false)
        {
            int wp_count = 0;
            bool error = false;
            List<Locationwp> cmds = new List<Locationwp>();

            try
            {
                StreamReader sr = new StreamReader(file); //"defines.h"
                string header = sr.ReadLine();
                if (header == null || !header.Contains("QGC WPL"))
                {
                    CustomMessageBox.Show("Invalid Waypoint file");
                    return;
                }

                while (!error && !sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    // waypoints

                    if (line.StartsWith("#"))
                        continue;

                    //seq/cur/frame/mode
                    string[] items = line.Split(new[] {'\t', ' ', ','}, StringSplitOptions.RemoveEmptyEntries);

                    if (items.Length <= 9)
                        continue;

                    try
                    {
                        // check to see if the first wp is index 0/home.
                        // if it is not index 0, add a blank home point
                        if (wp_count == 0 && items[0] != "0")
                        {
                            cmds.Add(new Locationwp());
                        }

                        Locationwp temp = new Locationwp();
                        if (items[2] == "3")
                        {
                            // abs MAV_FRAME_GLOBAL_RELATIVE_ALT=3
                            temp.options = 1;
                        }
                        else if (items[2] == "10")
                        {
                            temp.options = 8;
                        }
                        else
                        {
                            temp.options = 0;
                        }
                        temp.id = (ushort)Enum.Parse(typeof (MAVLink.MAV_CMD), items[3], false);
                        temp.p1 = float.Parse(items[4], new CultureInfo("en-US"));

                        if (temp.id == 99)
                            temp.id = 0;

                        temp.alt = (float) (double.Parse(items[10], new CultureInfo("en-US")));
                        temp.lat = (double.Parse(items[8], new CultureInfo("en-US")));
                        temp.lng = (double.Parse(items[9], new CultureInfo("en-US")));

                        temp.p2 = (float) (double.Parse(items[5], new CultureInfo("en-US")));
                        temp.p3 = (float) (double.Parse(items[6], new CultureInfo("en-US")));
                        temp.p4 = (float) (double.Parse(items[7], new CultureInfo("en-US")));

                        cmds.Add(temp);

                        wp_count++;
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                        CustomMessageBox.Show("Line invalid\n" + line);
                    }
                }

                sr.Close();

                processToScreen(cmds, append);

                writeKML();

                MainMap.ZoomAndCenterMarkers("WPOverlay");
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Can't open file! " + ex);
            }
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                lock (thisLock)
                {
                    MainMap.Zoom = trackBar1.Value;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        double calcpolygonarea(List<PointLatLng> polygon)
        {
            // should be a closed polygon
            // coords are in lat long
            // need utm to calc area

            if (polygon.Count == 0)
            {
                CustomMessageBox.Show("Please define a polygon!");
                return 0;
            }

            // close the polygon
            if (polygon[0] != polygon[polygon.Count - 1])
                polygon.Add(polygon[0]); // make a full loop

            CoordinateTransformationFactory ctfac = new CoordinateTransformationFactory();

            IGeographicCoordinateSystem wgs84 = GeographicCoordinateSystem.WGS84;

            int utmzone = (int) ((polygon[0].Lng - -186.0)/6.0);

            IProjectedCoordinateSystem utm = ProjectedCoordinateSystem.WGS84_UTM(utmzone,
                polygon[0].Lat < 0 ? false : true);

            ICoordinateTransformation trans = ctfac.CreateFromCoordinateSystems(wgs84, utm);

            double prod1 = 0;
            double prod2 = 0;

            for (int a = 0; a < (polygon.Count - 1); a++)
            {
                double[] pll1 = {polygon[a].Lng, polygon[a].Lat};
                double[] pll2 = {polygon[a + 1].Lng, polygon[a + 1].Lat};

                double[] p1 = trans.MathTransform.Transform(pll1);
                double[] p2 = trans.MathTransform.Transform(pll2);

                prod1 += p1[0]*p2[1];
                prod2 += p1[1]*p2[0];
            }

            double answer = (prod1 - prod2)/2;

            if (polygon[0] == polygon[polygon.Count - 1])
                polygon.RemoveAt(polygon.Count - 1); // unmake a full loop

            return answer;
        }

        // marker
        GMapMarker currentMarker;
        GMapMarker center = new GMarkerGoogle(new PointLatLng(0.0, 0.0), GMarkerGoogleType.none);

        // polygons
        GMapPolygon wppolygon;
        internal GMapPolygon drawnpolygon;
        GMapPolygon geofencepolygon;


        // layers
        GMapOverlay top; // not currently used
        public static GMapOverlay objectsoverlay; // where the markers a drawn
        public static GMapOverlay routesoverlay; // static so can update from gcs
        public static GMapOverlay polygonsoverlay; // where the track is drawn
        public static GMapOverlay airportsoverlay;
        public static GMapOverlay poioverlay = new GMapOverlay("POI"); // poi layer
        GMapOverlay drawnpolygonsoverlay;
        GMapOverlay kmlpolygonsoverlay;
        GMapOverlay geofenceoverlay;
        static GMapOverlay rallypointoverlay;

        // etc
        readonly Random rnd = new Random();
        string mobileGpsLog = string.Empty;
        GMapMarkerRect CurentRectMarker;
        GMapMarkerRallyPt CurrentRallyPt;
        GMapMarkerPOI CurrentPOIMarker;
        GMapMarker CurrentGMapMarker;
        bool isMouseDown;
        bool isMouseDraging;
        bool isMouseClickOffMenu;
        PointLatLng MouseDownStart;
        internal PointLatLng MouseDownEnd;

        //public long ElapsedMilliseconds;

        #region -- map events --

        void MainMap_OnMarkerLeave(GMapMarker item)
        {
            if (!isMouseDown)
            {
                if (item is GMapMarkerRect)
                {
                    CurentRectMarker = null;
                    GMapMarkerRect rc = item as GMapMarkerRect;
                    rc.ResetColor();
                    MainMap.Invalidate(false);
                }
                if (item is GMapMarkerRallyPt)
                {
                    CurrentRallyPt = null;
                }
                if (item is GMapMarkerPOI)
                {
                    CurrentPOIMarker = null;
                }
                if (item is GMapMarker)
                {
                    // when you click the context menu this triggers and causes problems
                    CurrentGMapMarker = null;
                }
            }
        }

        void MainMap_OnMarkerEnter(GMapMarker item)
        {
            if (!isMouseDown)
            {
                if (item is GMapMarkerRect)
                {
                    GMapMarkerRect rc = item as GMapMarkerRect;
                    rc.Pen.Color = Color.Red;
                    MainMap.Invalidate(false);

                    int answer;
                    if (item.Tag != null && rc.InnerMarker != null &&
                        int.TryParse(rc.InnerMarker.Tag.ToString(), out answer))
                    {
                        try
                        {
                            Commands.CurrentCell = Commands[0, answer - 1];
                            item.ToolTipText = "Alt: " + Commands[Alt.Index, answer - 1].Value;
                            item.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex);
                        }
                    }

                    CurentRectMarker = rc;
                }
                if (item is GMapMarkerRallyPt)
                {
                    CurrentRallyPt = item as GMapMarkerRallyPt;
                }
                if (item is GMapMarkerAirport)
                {
                    // do nothing - readonly
                    return;
                }
                if (item is GMapMarkerPOI)
                {
                    CurrentPOIMarker = item as GMapMarkerPOI;
                }
                if (item is GMapMarkerWP)
                {
                    //CurrentGMapMarker = item;
                }
                if (item is GMapMarker)
                {
                    //CurrentGMapMarker = item;
                }
            }
        }

        // click on some marker
        void MainMap_OnMarkerClick(GMapMarker item, object ei)
        {
            var e = ei as MouseEventArgs;
            int answer;
            try // when dragging item can sometimes be null
            {
                if (item.Tag == null)
                {
                    // home.. etc
                    return;
                }

                if (ModifierKeys == Keys.Control)
                {
                    try
                    {
                        groupmarkeradd(item);

                        log.Info("add marker to group");
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }
                }
                if (int.TryParse(item.Tag.ToString(), out answer))
                {
                    Commands.CurrentCell = Commands[0, answer - 1];
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        void MainMap_OnMapTypeChanged(GMapProvider type)
        {
            comboBoxMapType.SelectedItem = MainMap.MapProvider;

            if (type == WMSProvider.Instance)
            {
                string url = "";
                if (Settings.Instance["WMSserver"] != null)
                    url = Settings.Instance["WMSserver"];
                if (DialogResult.Cancel == InputBox.Show("WMS Server", "Enter the WMS server URL", ref url))
                    return;

                // Build get capability request.
                string szCapabilityRequest = BuildGetCapabilitityRequest(url);

                XmlDocument xCapabilityResponse = MakeRequest(szCapabilityRequest);
                ProcessWmsCapabilitesRequest(xCapabilityResponse);

                Settings.Instance["WMSserver"] = url;
                WMSProvider.CustomWMSURL = url;
            }
        }

        /// <summary>
        /// Builds the get Capability request.
        /// </summary>
        /// <param name="serverUrl">The server URL.</param>
        /// <returns></returns>
        private string BuildGetCapabilitityRequest(string serverUrl)
        {
            // What happens if the URL already has  '?'. 
            // For example: http://foo.com?Token=yyyy
            // In this example, the get capability request should be 
            // http://foo.com?Token=yyyy&version=1.1.0&Request=GetCapabilities&service=WMS but not
            // http://foo.com?Token=yyyy?version=1.1.0&Request=GetCapabilities&service=WMS

            // If the URL doesn't contain '?', append it.
            if (!serverUrl.Contains("?"))
            {
                serverUrl += "?";
            }
            else
            {
                // Check if the URL already has query strings.
                // If the URL doesn't have query strings, '?' comes at the end.
                if (!serverUrl.EndsWith("?"))
                {
                    // Already have query string, so add '&' before adding other query strings.
                    serverUrl += "&";
                }
            }
            return serverUrl + "version=1.1.0&Request=GetCapabilities&service=WMS";
        }

        /**
        * This function requests an XML document from a webserver.
        * @param requestUrl The request url as a string including. Example: http://129.206.228.72/cached/hillshade?Request=GetCapabilities
        * @return An XML document containing the response.
        */

        private XmlDocument MakeRequest(string requestUrl)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;


                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(response.GetResponseStream());
                return (xmlDoc);
            }
            catch (Exception e)
            {
                CustomMessageBox.Show("Failed to make WMS Server request: " + e.Message);
                return null;
            }
        }


        /**
         * This function parses a WMS server capabilites response.
         */

        private void ProcessWmsCapabilitesRequest(XmlDocument xCapabilitesResponse)
        {
            //Create namespace manager
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xCapabilitesResponse.NameTable);

            //check if the response is a valid xml document - if not, the server might still be able to serve us but all the checks below would fail. example: http://tiles.kartat.kapsi.fi/peruskartta
            //best sign is that there is no node WMT_MS_Capabilities
            if (xCapabilitesResponse.SelectNodes("//WMT_MS_Capabilities", nsmgr).Count == 0)
                return;


            //first, we have to make sure that the server is able to send us png imagery
            bool bPngCapable = false;
            XmlNodeList getMapElements = xCapabilitesResponse.SelectNodes("//GetMap", nsmgr);
            if (getMapElements.Count != 1)
                CustomMessageBox.Show("Invalid WMS Server response: Invalid number of GetMap elements.");
            else
            {
                XmlNode getMapNode = getMapElements.Item(0);
                //search through all format nodes for image/png
                foreach (XmlNode formatNode in getMapNode.SelectNodes("//Format", nsmgr))
                {
                    if (formatNode.InnerText.Contains("image/png"))
                    {
                        bPngCapable = true;
                        break;
                    }
                }
            }


            if (!bPngCapable)
            {
                CustomMessageBox.Show("Invalid WMS Server response: Server unable to return PNG images.");
                return;
            }


            //now search through all layer -> srs nodes for EPSG:4326 compatibility
            bool bEpsgCapable = false;
            XmlNodeList srsELements = xCapabilitesResponse.SelectNodes("//SRS", nsmgr);
            foreach (XmlNode srsNode in srsELements)
            {
                if (srsNode.InnerText.Contains("EPSG:4326"))
                {
                    bEpsgCapable = true;
                    break;
                }
            }


            if (!bEpsgCapable)
            {
                CustomMessageBox.Show(
                    "Invalid WMS Server response: Server unable to return EPSG:4326 / WGS84 compatible images.");
                return;
            }


            // the server is capable of serving our requests - now check if there is a layer to be selected
            // Display layer title in the input box instead of layer name.
            // format: layer -> layer -> name
            //         layer -> layer -> title
            string szLayerSelection = "";
            int iSelect = 0;
            List<string> szListLayerName = new List<string>();
            // Loop through all layers.
            XmlNodeList layerElements = xCapabilitesResponse.SelectNodes("//Layer/Layer", nsmgr);
            foreach (XmlNode layerElement in layerElements)
            {
                // Get Name element.
                var nameNode = layerElement.SelectSingleNode("Name", nsmgr);

                // Skip if no name element is found.
                if (nameNode != null)
                {
                    var name = nameNode.InnerText;
                    // Set the default title as the layer name. 
                    var title = name;
                    // Get Title element.
                    var titleNode = layerElement.SelectSingleNode("Title", nsmgr);
                    if (titleNode != null)
                    {
                        var titleText = titleNode.InnerText;
                        if (!string.IsNullOrWhiteSpace(titleText))
                        {
                            title = titleText;
                        }
                    }
                    szListLayerName.Add(name);

                    szLayerSelection += string.Format("{0}: {1}\n ", iSelect, title);
                    //mixing control and formatting is not optimal...
                    iSelect++;
                }
            }
           
            //only select layer if there is one
            if (szListLayerName.Count != 0)
            {
                //now let the user select a layer
                string szUserSelection = "";
                if (DialogResult.Cancel ==
                    InputBox.Show("WMS Server",
                        "The following layers were detected:\n " + szLayerSelection +
                        "Please choose one by typing the associated number.", ref szUserSelection))
                    return;
                int iUserSelection = 0;
                try
                {
                    iUserSelection = Convert.ToInt32(szUserSelection);
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                    iUserSelection = 0; //ignore all errors and default to first layer
                }

                WMSProvider.szWmsLayer = szListLayerName[iUserSelection];
            }
        }

        void groupmarkeradd(GMapMarker marker)
        {
            System.Diagnostics.Debug.WriteLine("add marker " + marker.Tag.ToString());
            groupmarkers.Add(int.Parse(marker.Tag.ToString()));
            if (marker is GMapMarkerWP)
            {
                ((GMapMarkerWP) marker).selected = true;
            }
            if (marker is GMapMarkerRect)
            {
                ((GMapMarkerWP) ((GMapMarkerRect) marker).InnerMarker).selected = true;
            }
        }

        void MainMap_MouseUp(object sender, MouseEventArgs e)
        {
            if (isMouseClickOffMenu)
            {
                isMouseClickOffMenu = false;
                return;
            }

            // check if the mouse up happend over our button
            if (polyicon.Rectangle.Contains(e.Location))
            {
                polyicon.IsSelected = !polyicon.IsSelected;

                if (e.Button == MouseButtons.Right)
                {
                    polyicon.IsSelected = false;
                    clearPolygonToolStripMenuItem_Click(this, null);

                    contextMenuStrip1.Visible = false;

                    return;
                }

                if (polyicon.IsSelected)
                {
                    polygongridmode = true;
                }
                else
                {
                    polygongridmode = false;
                }

                return;
            }

            MouseDownEnd = MainMap.FromLocalToLatLng(e.X, e.Y);

            // Console.WriteLine("MainMap MU");

            if (e.Button == MouseButtons.Right) // ignore right clicks
            {
                return;
            }

            if (isMouseDown) // mouse down on some other object and dragged to here.
            {
                // drag finished, update poi db
                if (CurrentPOIMarker != null)
                {
                    POI.POIMove(CurrentPOIMarker);
                    CurrentPOIMarker = null;
                }

                if (e.Button == MouseButtons.Left)
                {
                    isMouseDown = false;
                }
                if (ModifierKeys == Keys.Control)
                {
                    // group select wps
                    GMapPolygon poly = new GMapPolygon(new List<PointLatLng>(), "temp");

                    poly.Points.Add(MouseDownStart);
                    poly.Points.Add(new PointLatLng(MouseDownStart.Lat, MouseDownEnd.Lng));
                    poly.Points.Add(MouseDownEnd);
                    poly.Points.Add(new PointLatLng(MouseDownEnd.Lat, MouseDownStart.Lng));

                    foreach (var marker in MainMap.Overlays.First(a => a.Id == "WPOverlay").Markers)
                    {
                        if (poly.IsInside(marker.Position))
                        {
                            try
                            {
                                if (marker.Tag != null)
                                {
                                    groupmarkeradd(marker);
                                }
                            }
                            catch (Exception ex)
                            {
                                log.Error(ex);
                            }
                        }
                    }

                    isMouseDraging = false;
                    return;
                }
                if (!isMouseDraging)
                {
                    if (CurentRectMarker != null)
                    {
                        // cant add WP in existing rect
                    }
                    else
                    {
                        AddWPToMap(currentMarker.Position.Lat, currentMarker.Position.Lng, 0);
                    }
                }
                else
                {
                    if (groupmarkers.Count > 0)
                    {
                        Dictionary<string, PointLatLng> dest = new Dictionary<string, PointLatLng>();

                        var markers = MainMap.Overlays.First(a => a.Id == "WPOverlay");
                        
                        foreach (var markerid in groupmarkers.Distinct())
                        {
                            for (int a = 0; a < markers.Markers.Count; a++)
                            {
                                var marker = markers.Markers[a];

                                if (marker.Tag != null && marker.Tag.ToString() == markerid.ToString())
                                {
                                    dest[marker.Tag.ToString()] = marker.Position;
                                    break;
                                }
                            }
                        }

                        foreach (KeyValuePair<string, PointLatLng> item in dest)
                        {
                            var value = item.Value;
                            quickadd = true;
                            callMeDrag(item.Key, value.Lat, value.Lng, -1);
                            quickadd = false;
                        }

                        MainMap.SelectedArea = RectLatLng.Empty;
                        groupmarkers.Clear();
                        // redraw to remove selection
                        writeKML();

                        CurentRectMarker = null;
                    }

                    if (CurentRectMarker != null && CurentRectMarker.InnerMarker != null)
                    {
                        if (CurentRectMarker.InnerMarker.Tag.ToString().Contains("grid"))
                        {
                            try
                            {
                                drawnpolygon.Points[
                                    int.Parse(CurentRectMarker.InnerMarker.Tag.ToString().Replace("grid", "")) - 1] =
                                    new PointLatLng(MouseDownEnd.Lat, MouseDownEnd.Lng);
                                MainMap.UpdatePolygonLocalPosition(drawnpolygon);
                                MainMap.Invalidate();
                            }
                            catch (Exception ex)
                            {
                                log.Error(ex);
                            }
                        }
                        else
                        {
                            callMeDrag(CurentRectMarker.InnerMarker.Tag.ToString(), currentMarker.Position.Lat,
                                currentMarker.Position.Lng, -2);
                        }
                        CurentRectMarker = null;
                    }
                }
            }

            isMouseDraging = false;
        }

        void MainMap_MouseDown(object sender, MouseEventArgs e)
        {
            if (isMouseClickOffMenu)
                return;

            MouseDownStart = MainMap.FromLocalToLatLng(e.X, e.Y);

            //   Console.WriteLine("MainMap MD");

            if (e.Button == MouseButtons.Left && (groupmarkers.Count > 0 || ModifierKeys == Keys.Control))
            {
                // group move
                isMouseDown = true;
                isMouseDraging = false;

                return;
            }

            if (e.Button == MouseButtons.Left && ModifierKeys != Keys.Alt && ModifierKeys != Keys.Control)
            {
                isMouseDown = true;
                isMouseDraging = false;

                if (currentMarker.IsVisible)
                {
                    currentMarker.Position = MainMap.FromLocalToLatLng(e.X, e.Y);
                }
            }
        }

        // move current marker with left holding
        void MainMap_MouseMove(object sender, MouseEventArgs e)
        {
            PointLatLng point = MainMap.FromLocalToLatLng(e.X, e.Y);

            if (MouseDownStart == point)
                return;

            //  Console.WriteLine("MainMap MM " + point);

            currentMarker.Position = point;

            if (!isMouseDown)
            {
                // update mouse pos display
                SetMouseDisplay(point.Lat, point.Lng, 0);
            }

            //draging
            if (e.Button == MouseButtons.Left && isMouseDown)
            {
                isMouseDraging = true;
                if (CurrentRallyPt != null)
                {
                    PointLatLng pnew = MainMap.FromLocalToLatLng(e.X, e.Y);

                    CurrentRallyPt.Position = pnew;
                }
                else if (groupmarkers.Count > 0)
                {
                    // group drag

                    double latdif = MouseDownStart.Lat - point.Lat;
                    double lngdif = MouseDownStart.Lng - point.Lng;

                    MouseDownStart = point;

                    var markers = MainMap.Overlays.First(a => a.Id == "WPOverlay");

                    Hashtable seen = new Hashtable();

                    foreach (var markerid in groupmarkers)
                    {
                        if (seen.ContainsKey(markerid))
                            continue;
                      
                        seen[markerid] = 1;
                        for (int a = 0; a < markers.Markers.Count; a++)
                        {
                            var marker = markers.Markers[a];

                            if (marker.Tag != null && marker.Tag.ToString() == markerid.ToString())
                            {
                                var temp = new PointLatLng(marker.Position.Lat, marker.Position.Lng);
                                temp.Offset(latdif, -lngdif);
                                marker.Position = temp;
                            }
                        }
                    }
                }
                else if (CurentRectMarker != null) // left click pan
                {
                    try
                    {
                        // check if this is a grid point
                        if (CurentRectMarker.InnerMarker.Tag.ToString().Contains("grid"))
                        {
                            drawnpolygon.Points[
                                int.Parse(CurentRectMarker.InnerMarker.Tag.ToString().Replace("grid", "")) - 1] =
                                new PointLatLng(point.Lat, point.Lng);
                            MainMap.UpdatePolygonLocalPosition(drawnpolygon);
                            MainMap.Invalidate();
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }

                    PointLatLng pnew = MainMap.FromLocalToLatLng(e.X, e.Y);

                    // adjust polyline point while we drag
                    try
                    {
                        if (CurrentGMapMarker != null && CurrentGMapMarker.Tag is int)
                        {
                            int? pIndex = (int?) CurentRectMarker.Tag;
                            if (pIndex.HasValue)
                            {
                                if (pIndex < wppolygon.Points.Count)
                                {
                                    wppolygon.Points[pIndex.Value] = pnew;
                                    lock (thisLock)
                                    {
                                        MainMap.UpdatePolygonLocalPosition(wppolygon);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }

                    // update rect and marker pos.
                    if (currentMarker.IsVisible)
                    {
                        currentMarker.Position = pnew;
                    }
                    CurentRectMarker.Position = pnew;

                    if (CurentRectMarker.InnerMarker != null)
                    {
                        CurentRectMarker.InnerMarker.Position = pnew;
                    }
                }
                else if (CurrentPOIMarker != null)
                {
                    PointLatLng pnew = MainMap.FromLocalToLatLng(e.X, e.Y);

                    CurrentPOIMarker.Position = pnew;
                }
                else if (CurrentGMapMarker != null)
                {
                    PointLatLng pnew = MainMap.FromLocalToLatLng(e.X, e.Y);

                    CurrentGMapMarker.Position = pnew;
                }
                else if (ModifierKeys == Keys.Control)
                {
                    // draw selection box
                    double latdif = MouseDownStart.Lat - point.Lat;
                    double lngdif = MouseDownStart.Lng - point.Lng;

                    MainMap.SelectedArea = new RectLatLng(Math.Max(MouseDownStart.Lat, point.Lat),
                        Math.Min(MouseDownStart.Lng, point.Lng), Math.Abs(lngdif), Math.Abs(latdif));
                }
                else // left click pan
                {
                    double latdif = MouseDownStart.Lat - point.Lat;
                    double lngdif = MouseDownStart.Lng - point.Lng;

                    try
                    {
                        lock (thisLock)
                        {
                            if (!isMouseClickOffMenu)
                                MainMap.Position = new PointLatLng(center.Position.Lat + latdif,
                                    center.Position.Lng + lngdif);
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }
                }
            }
            else if (e.Button == MouseButtons.None)
            {
                isMouseDown = false;
            }
        }

        // MapZoomChanged
        void MainMap_OnMapZoomChanged()
        {
            if (MainMap.Zoom > 0)
            {
                try
                {
                    trackBar1.Value = (int) (MainMap.Zoom);
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
                //textBoxZoomCurrent.Text = MainMap.Zoom.ToString();
                center.Position = MainMap.Position;
            }
        }


        // loader start loading tiles
        void MainMap_OnTileLoadStart()
        {
            MethodInvoker m = delegate { lbl_status.Text = "Status: loading tiles..."; };
            try
            {
                if (IsHandleCreated)
                    BeginInvoke(m);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        // loader end loading tiles
        void MainMap_OnTileLoadComplete(long ElapsedMilliseconds)
        {
            //MainMap.ElapsedMilliseconds = ElapsedMilliseconds;

            MethodInvoker m = delegate
            {
                lbl_status.Text = "Status: loaded tiles";

                //panelMenu.Text = "Menu, last load in " + MainMap.ElapsedMilliseconds + "ms";

                //textBoxMemory.Text = string.Format(CultureInfo.InvariantCulture, "{0:0.00}MB of {1:0.00}MB", MainMap.Manager.MemoryCacheSize, MainMap.Manager.MemoryCacheCapacity);
            };
            try
            {
                if (!IsDisposed && IsHandleCreated)
                    BeginInvoke(m);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        // current point changed
        void MainMap_OnCurrentPositionChanged(PointLatLng point)
        {
            if (point.Lat > 90)
            {
                point.Lat = 90;
            }
            if (point.Lat < -90)
            {
                point.Lat = -90;
            }
            if (point.Lng > 180)
            {
                point.Lng = 180;
            }
            if (point.Lng < -180)
            {
                point.Lng = -180;
            }
            center.Position = point;

            coords1.Lat = point.Lat;
            coords1.Lng = point.Lng;

            // always show on planner view
            //if (MainV2.ShowAirports)
            {
                airportsoverlay.Clear();
                foreach (var item in Airports.getAirports(MainMap.Position))
                {
                    airportsoverlay.Markers.Add(new GMapMarkerAirport(item)
                    {
                        ToolTipText = item.Tag,
                        ToolTipMode = MarkerTooltipMode.OnMouseOver
                    });
                }
            }
        }

        // center markers on start
        private void MainForm_Load(object sender, EventArgs e)
        {
            if (objectsoverlay.Markers.Count > 0)
            {
                MainMap.ZoomAndCenterMarkers(null);
            }
            trackBar1.Value = (int) MainMap.Zoom;
        }

        // ensure focus on map, trackbar can have it too
        private void MainMap_MouseEnter(object sender, EventArgs e)
        {
            // MainMap.Focus();
        }

        #endregion

        private void comboBoxMapType_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                MainMap.MapProvider = (GMapProvider) comboBoxMapType.SelectedItem;
                FlightData.mymap.MapProvider = (GMapProvider) comboBoxMapType.SelectedItem;
                Settings.Instance["MapType"] = comboBoxMapType.Text;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                CustomMessageBox.Show("Map change failed. try zooming out first.");
            }
        }

        private void Commands_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control.GetType() == typeof (DataGridViewComboBoxEditingControl))
            {
                var temp = ((ComboBox) e.Control);
                ((ComboBox) e.Control).SelectionChangeCommitted -= Commands_SelectionChangeCommitted;
                ((ComboBox) e.Control).SelectionChangeCommitted += Commands_SelectionChangeCommitted;
                ((ComboBox) e.Control).ForeColor = Color.White;
                ((ComboBox) e.Control).BackColor = Color.FromArgb(0x43, 0x44, 0x45);
                Debug.WriteLine("Setting event handle");
            }
        }

        void Commands_SelectionChangeCommitted(object sender, EventArgs e)
        {
            // update row headers
            ((ComboBox) sender).ForeColor = Color.White;
            ChangeColumnHeader(((ComboBox) sender).Text);
            try
            {
                // default takeoff to non 0 alt
                if (((ComboBox) sender).Text == "TAKEOFF")
                {
                    if (Commands.Rows[selectedrow].Cells[Alt.Index].Value != null &&
                        Commands.Rows[selectedrow].Cells[Alt.Index].Value.ToString() == "0")
                        Commands.Rows[selectedrow].Cells[Alt.Index].Value = TXT_DefaultAlt.Text;
                }

                // default to take shot
                if (((ComboBox) sender).Text == "DO_DIGICAM_CONTROL")
                {
                    if (Commands.Rows[selectedrow].Cells[Lat.Index].Value != null &&
                        Commands.Rows[selectedrow].Cells[Lat.Index].Value.ToString() == "0")
                        Commands.Rows[selectedrow].Cells[Lat.Index].Value = (1).ToString();
                }

                if (((ComboBox)sender).Text == "UNKNOWN")
                {
                    string cmdid = "-1";
                    if (InputBox.Show("Mavlink ID", "Please enter the command ID", ref cmdid) == DialogResult.OK)
                    {
                        if (cmdid != "-1")
                        {
                            Commands.Rows[selectedrow].Cells[Command.Index].Tag = ushort.Parse(cmdid);
                        }
                    }
                }

                for (int i = 0; i < Commands.ColumnCount; i++)
                {
                    DataGridViewCell tcell = Commands.Rows[selectedrow].Cells[i];
                    if (tcell.GetType() == typeof (DataGridViewTextBoxCell))
                    {
                        if (tcell.Value.ToString() == "?")
                            tcell.Value = "0";
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        /// <summary>
        /// Get the Google earth ALT for a given coord
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <returns>Altitude</returns>
        double getGEAlt(double lat, double lng)
        {
            double alt = 0;
            //http://maps.google.com/maps/api/elevation/xml

            try
            {
                using (
                    XmlTextReader xmlreader =
                        new XmlTextReader("http://maps.google.com/maps/api/elevation/xml?locations=" +
                                          lat.ToString(new CultureInfo("en-US")) + "," +
                                          lng.ToString(new CultureInfo("en-US")) + "&sensor=true"))
                {
                    while (xmlreader.Read())
                    {
                        xmlreader.MoveToElement();
                        switch (xmlreader.Name)
                        {
                            case "elevation":
                                alt = double.Parse(xmlreader.ReadString(), new CultureInfo("en-US"));
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return alt*CurrentState.multiplieralt;
        }

        private void TXT_homelat_Enter(object sender, EventArgs e)
        {
            if (!sethome)
                CustomMessageBox.Show("Click on the Map to set Home ");
            sethome = true;
            
        }

        private void Planner_Resize(object sender, EventArgs e)
        {
            MainMap.Zoom = trackBar1.Value;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            try
            {
                base.OnPaint(pe);
            }
            catch (Exception)
            {
            }
        }

        private void Commands_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // we have modified a utm coords
            if (e.ColumnIndex == coordZone.Index ||
                e.ColumnIndex == coordNorthing.Index ||
                e.ColumnIndex == coordEasting.Index)
            {
                convertFromUTM(e.RowIndex);
            }

            if(e.ColumnIndex == MGRS.Index)
            {
                convertFromMGRS(e.RowIndex);
            }

            // we have modified a ll coord
            if (e.ColumnIndex == Lat.Index ||
                e.ColumnIndex == Lon.Index)
            {
                try
                {
                    var lat = double.Parse(Commands.Rows[e.RowIndex].Cells[Lat.Index].Value.ToString());
                    var lng = double.Parse(Commands.Rows[e.RowIndex].Cells[Lon.Index].Value.ToString());
                    convertFromGeographic(lat, lng);
                } catch (Exception ex)
                {
                    log.Error(ex);
                    CustomMessageBox.Show("Invalid Lat/Long, please fix",Strings.ERROR);
                }
            }

            Commands_RowEnter(null,
                new DataGridViewCellEventArgs(Commands.CurrentCell.ColumnIndex, Commands.CurrentCell.RowIndex));

            try
            {
                writeKML();
            }
            catch (FormatException)
            {
                CustomMessageBox.Show(Strings.ERROR, Strings.InvalidNumberEntered);
            }
        }

        private void MainMap_Resize(object sender, EventArgs e)
        {
            MainMap.Zoom = MainMap.Zoom + 0.01;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            try
            {
                lock (thisLock)
                {
                    MainMap.Zoom = trackBar1.Value;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }


        private void BUT_Prefetch_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// from http://stackoverflow.com/questions/1119451/how-to-tell-if-a-line-intersects-a-polygon-in-c
        /// </summary>
        /// <param name="start1"></param>
        /// <param name="end1"></param>
        /// <param name="start2"></param>
        /// <param name="end2"></param>
        /// <returns></returns>
        public PointLatLng FindLineIntersection(PointLatLng start1, PointLatLng end1, PointLatLng start2,
            PointLatLng end2)
        {
            double denom = ((end1.Lng - start1.Lng)*(end2.Lat - start2.Lat)) -
                           ((end1.Lat - start1.Lat)*(end2.Lng - start2.Lng));
            //  AB & CD are parallel         
            if (denom == 0)
                return PointLatLng.Empty;
            double numer = ((start1.Lat - start2.Lat)*(end2.Lng - start2.Lng)) -
                           ((start1.Lng - start2.Lng)*(end2.Lat - start2.Lat));
            double r = numer/denom;
            double numer2 = ((start1.Lat - start2.Lat)*(end1.Lng - start1.Lng)) -
                            ((start1.Lng - start2.Lng)*(end1.Lat - start1.Lat));
            double s = numer2/denom;
            if ((r < 0 || r > 1) || (s < 0 || s > 1))
                return PointLatLng.Empty;
            // Find intersection point      
            PointLatLng result = new PointLatLng();
            result.Lng = start1.Lng + (r*(end1.Lng - start1.Lng));
            result.Lat = start1.Lat + (r*(end1.Lat - start1.Lat));
            return result;
        }

        RectLatLng getPolyMinMax(GMapPolygon poly)
        {
            if (poly.Points.Count == 0)
                return new RectLatLng();

            double minx, miny, maxx, maxy;

            minx = maxx = poly.Points[0].Lng;
            miny = maxy = poly.Points[0].Lat;

            foreach (PointLatLng pnt in poly.Points)
            {
                //Console.WriteLine(pnt.ToString());
                minx = Math.Min(minx, pnt.Lng);
                maxx = Math.Max(maxx, pnt.Lng);

                miny = Math.Min(miny, pnt.Lat);
                maxy = Math.Max(maxy, pnt.Lat);
            }

            return new RectLatLng(maxy, minx, Math.Abs(maxx - minx), Math.Abs(miny - maxy));
        }

        private void BUT_grid_Click(object sender, EventArgs e)
        {
        }

        private void label4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (MainV2.comPort.MAV.cs.lat != 0)
            {
                TXT_homealt.Text = (MainV2.comPort.MAV.cs.altasl).ToString("0");
                TXT_homelat.Text = MainV2.comPort.MAV.cs.lat.ToString();
                TXT_homelng.Text = MainV2.comPort.MAV.cs.lng.ToString();
            }
            else
            {
                CustomMessageBox.Show(
                    "If you're at the field, connect to your APM and wait for GPS lock. Then click 'Home Location' link to set home to your location");
            }
        }


        /// <summary>
        /// Format distance according to prefer distance unit
        /// </summary>
        /// <param name="distInKM">distance in kilometers</param>
        /// <param name="toMeterOrFeet">convert distance to meter or feet if true, covert to km or miles if false</param>
        /// <returns>formatted distance with unit</returns>
        private string FormatDistance(double distInKM, bool toMeterOrFeet)
        {
            string sunits = Settings.Instance["distunits"];
            distances units = distances.Meters;

            if (sunits != null)
                try
                {
                    units = (distances) Enum.Parse(typeof (distances), sunits);
                }
                catch (Exception)
                {
                }

            switch (units)
            {
                case distances.Feet:
                    return toMeterOrFeet
                        ? string.Format((distInKM*3280.8399).ToString("0.00 ft"))
                        : string.Format((distInKM*0.621371).ToString("0.0000 miles"));
                case distances.Meters:
                default:
                    return toMeterOrFeet
                        ? string.Format((distInKM*1000).ToString("0.00 m"))
                        : string.Format(distInKM.ToString("0.0000 km"));
            }
        }

        PointLatLng startmeasure;

        private void ContextMeasure_Click(object sender, EventArgs e)
        {
            if (startmeasure.IsEmpty)
            {
                startmeasure = MouseDownStart;
                polygonsoverlay.Markers.Add(new GMarkerGoogle(MouseDownStart, GMarkerGoogleType.red));
                MainMap.Invalidate();
                Common.MessageShowAgain("Measure Dist",
                    "You can now pan/zoom around.\nClick this option again to get the distance.");
            }
            else
            {
                List<PointLatLng> polygonPoints = new List<PointLatLng>
                {
                    startmeasure,
                    MouseDownStart
                };

                GMapPolygon line = new GMapPolygon(polygonPoints, "measure dist");
                line.Stroke.Color = Color.Green;

                polygonsoverlay.Polygons.Add(line);

                polygonsoverlay.Markers.Add(new GMarkerGoogle(MouseDownStart, GMarkerGoogleType.red));
                MainMap.Invalidate();
                CustomMessageBox.Show("Distance: " +
                                      FormatDistance(
                                          MainMap.MapProvider.Projection.GetDistance(startmeasure, MouseDownStart), true) +
                                      " AZ: " +
                                      (MainMap.MapProvider.Projection.GetBearing(startmeasure, MouseDownStart)
                                          .ToString("0")));
                polygonsoverlay.Polygons.Remove(line);
                polygonsoverlay.Markers.Clear();
                startmeasure = new PointLatLng();
            }
        }

        private void rotateMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string heading = "0";
            if (DialogResult.Cancel == InputBox.Show("Rotate map to heading", "Enter new UP heading", ref heading))
                return;
            float ans = 0;
            if (float.TryParse(heading, out ans))
            {
                MainMap.Bearing = ans;
            }
        }

        private void addPolygonPointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (polygongridmode == false)
            {
                CustomMessageBox.Show(
                    "You will remain in polygon mode until you clear the polygon or create a grid/upload a fence");
            }

            polygongridmode = true;

            List<PointLatLng> polygonPoints = new List<PointLatLng>();
            if (drawnpolygonsoverlay.Polygons.Count == 0)
            {
                drawnpolygon.Points.Clear();
                drawnpolygonsoverlay.Polygons.Add(drawnpolygon);
            }

            drawnpolygon.Fill = Brushes.Transparent;

            // remove full loop is exists
            if (drawnpolygon.Points.Count > 1 &&
                drawnpolygon.Points[0] == drawnpolygon.Points[drawnpolygon.Points.Count - 1])
                drawnpolygon.Points.RemoveAt(drawnpolygon.Points.Count - 1); // unmake a full loop

            drawnpolygon.Points.Add(new PointLatLng(MouseDownStart.Lat, MouseDownStart.Lng));

            addpolygonmarkergrid(drawnpolygon.Points.Count.ToString(), MouseDownStart.Lng, MouseDownStart.Lat, 0);

            MainMap.UpdatePolygonLocalPosition(drawnpolygon);

            MainMap.Invalidate();
        }

        public void redrawPolygonSurvey(List<PointLatLngAlt> list)
        {
            drawnpolygon.Points.Clear();
            drawnpolygonsoverlay.Clear();

            int tag = 0;
            list.ForEach(x =>
            {
                tag++;
                drawnpolygon.Points.Add(x);
                addpolygonmarkergrid(tag.ToString(), x.Lng, x.Lat, 0);
            });

            drawnpolygonsoverlay.Polygons.Add(drawnpolygon);
            MainMap.UpdatePolygonLocalPosition(drawnpolygon);
            MainMap.Invalidate();
        }

        private void clearPolygonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            polygongridmode = false;
            if (drawnpolygon == null)
                return;
            drawnpolygon.Points.Clear();
            drawnpolygonsoverlay.Markers.Clear();
            MainMap.Invalidate();

            writeKML();
        }

        private void clearMissionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            quickadd = true;

            // mono fix
            try
            {
                Commands.CurrentCell = null;
            } catch { }

            Commands.Rows.Clear();

            selectedrow = 0;
            quickadd = false;
            writeKML();
        }

        private void loiterForeverToolStripMenuItem_Click(object sender, EventArgs e)
        {
            selectedrow = Commands.Rows.Add();

            Commands.Rows[selectedrow].Cells[Command.Index].Value = MAVLink.MAV_CMD.LOITER_UNLIM.ToString();

            ChangeColumnHeader(MAVLink.MAV_CMD.LOITER_UNLIM.ToString());

            setfromMap(MouseDownEnd.Lat, MouseDownEnd.Lng, (int) float.Parse(TXT_DefaultAlt.Text));

            writeKML();
        }

        private void jumpstartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string repeat = "5";
            if (DialogResult.Cancel == InputBox.Show("Jump repeat", "Number of times to Repeat", ref repeat))
                return;

            selectedrow = Commands.Rows.Add();

            Commands.Rows[selectedrow].Cells[Command.Index].Value = MAVLink.MAV_CMD.DO_JUMP.ToString();

            Commands.Rows[selectedrow].Cells[Param1.Index].Value = 1;

            Commands.Rows[selectedrow].Cells[Param2.Index].Value = repeat;

            writeKML();
        }

        private void jumpwPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string wp = "1";
            if (DialogResult.Cancel == InputBox.Show("WP No", "Jump to WP no?", ref wp))
                return;
            string repeat = "5";
            if (DialogResult.Cancel == InputBox.Show("Jump repeat", "Number of times to Repeat", ref repeat))
                return;

            selectedrow = Commands.Rows.Add();

            Commands.Rows[selectedrow].Cells[Command.Index].Value = MAVLink.MAV_CMD.DO_JUMP.ToString();

            Commands.Rows[selectedrow].Cells[Param1.Index].Value = wp;

            Commands.Rows[selectedrow].Cells[Param2.Index].Value = repeat;

            writeKML();
        }

        private void deleteWPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int no = 0;
            if (CurentRectMarker != null)
            {
                if (int.TryParse(CurentRectMarker.InnerMarker.Tag.ToString(), out no))
                {
                    try
                    {
                        Commands.Rows.RemoveAt(no - 1); // home is 0
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                        CustomMessageBox.Show("error selecting wp, please try again.");
                    }
                }
                else if (int.TryParse(CurentRectMarker.InnerMarker.Tag.ToString().Replace("grid", ""), out no))
                {
                    try
                    {
                        drawnpolygon.Points.RemoveAt(no - 1);
                        drawnpolygonsoverlay.Markers.Clear();

                        int a = 1;
                        foreach (PointLatLng pnt in drawnpolygon.Points)
                        {
                            addpolygonmarkergrid(a.ToString(), pnt.Lng, pnt.Lat, 0);
                            a++;
                        }

                        MainMap.UpdatePolygonLocalPosition(drawnpolygon);

                        MainMap.Invalidate();
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                        CustomMessageBox.Show("Remove point Failed. Please try again.");
                    }
                }
            }
            else if (CurrentRallyPt != null)
            {
                rallypointoverlay.Markers.Remove(CurrentRallyPt);
                MainMap.Invalidate(true);

                CurrentRallyPt = null;
            }
            else if (groupmarkers.Count > 0)
            {
                for (int a = Commands.Rows.Count; a > 0; a--)
                {
                    try
                    {
                        if (groupmarkers.Contains(a))
                            Commands.Rows.RemoveAt(a - 1); // home is 0
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                        CustomMessageBox.Show("error selecting wp, please try again.");
                    }
                }

                groupmarkers.Clear();
            }


            if (currentMarker != null)
                CurentRectMarker = null;

            writeKML();
        }

        private void loitertimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string time = "5";
            if (DialogResult.Cancel == InputBox.Show("Loiter Time", "Loiter Time", ref time))
                return;

            selectedrow = Commands.Rows.Add();

            Commands.Rows[selectedrow].Cells[Command.Index].Value = MAVLink.MAV_CMD.LOITER_TIME.ToString();

            Commands.Rows[selectedrow].Cells[Param1.Index].Value = time;

            ChangeColumnHeader(MAVLink.MAV_CMD.LOITER_TIME.ToString());

            setfromMap(MouseDownEnd.Lat, MouseDownEnd.Lng, (int) float.Parse(TXT_DefaultAlt.Text));

            writeKML();
        }

        private void loitercirclesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string turns = "3";
            if (DialogResult.Cancel == InputBox.Show("Loiter Turns", "Loiter Turns", ref turns))
                return;

            selectedrow = Commands.Rows.Add();

            Commands.Rows[selectedrow].Cells[Command.Index].Value = MAVLink.MAV_CMD.LOITER_TURNS.ToString();

            Commands.Rows[selectedrow].Cells[Param1.Index].Value = turns;

            ChangeColumnHeader(MAVLink.MAV_CMD.LOITER_TURNS.ToString());

            setfromMap(MouseDownEnd.Lat, MouseDownEnd.Lng, (int) float.Parse(TXT_DefaultAlt.Text));

            writeKML();
        }

        private void panelMap_Resize(object sender, EventArgs e)
        {
            // this is a mono fix for the zoom bar
            //Console.WriteLine("panelmap "+panelMap.Size.ToString());
            MainMap.Size = new Size(panelMap.Size.Width - 50, panelMap.Size.Height);
            trackBar1.Location = new Point(panelMap.Size.Width - 50, trackBar1.Location.Y);
            trackBar1.Size = new Size(trackBar1.Size.Width, panelMap.Size.Height - trackBar1.Location.Y);
            label11.Location = new Point(panelMap.Size.Width - 50, label11.Location.Y);
        }

        DateTime mapupdate = DateTime.MinValue;

        /// <summary>
        /// Draw an mav icon, and update tracker location icon and guided mode wp on FP screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (isMouseDown || CurentRectMarker != null)
                    return;

                routesoverlay.Markers.Clear();

                if (MainV2.comPort.MAV.cs.TrackerLocation != MainV2.comPort.MAV.cs.HomeLocation &&
                    MainV2.comPort.MAV.cs.TrackerLocation.Lng != 0)
                {
                    addpolygonmarker("Tracker Home", MainV2.comPort.MAV.cs.TrackerLocation.Lng,
                        MainV2.comPort.MAV.cs.TrackerLocation.Lat, (int) MainV2.comPort.MAV.cs.TrackerLocation.Alt,
                        Color.Blue, routesoverlay);
                }

                if (MainV2.comPort.MAV.cs.lat == 0 || MainV2.comPort.MAV.cs.lng == 0)
                    return;

                var marker = ArduPilot.Common.getMAVMarker(MainV2.comPort.MAV);

                routesoverlay.Markers.Add(marker);

                if (MainV2.comPort.MAV.cs.mode.ToLower() == "guided" && MainV2.comPort.MAV.GuidedMode.x != 0)
                {
                    addpolygonmarker("Guided Mode", MainV2.comPort.MAV.GuidedMode.y, MainV2.comPort.MAV.GuidedMode.x,
                        (int) MainV2.comPort.MAV.GuidedMode.z, Color.Blue, routesoverlay);
                }
            }
            catch (Exception ex)
            {
                log.Warn(ex);
            }
        }

        /// <summary>
        /// Try to reduce the number of map position changes generated by the code
        /// </summary>
        DateTime lastmapposchange = DateTime.MinValue;

        private void updateMapPosition(PointLatLng currentloc)
        {
            BeginInvoke((MethodInvoker) delegate
            {
                try
                {
                    if (lastmapposchange.Second != DateTime.Now.Second)
                    {
                        MainMap.Position = currentloc;
                        lastmapposchange = DateTime.Now;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            });
        }

        private void addpolygonmarker(string tag, double lng, double lat, int alt, Color? color, GMapOverlay overlay)
        {
            try
            {
                PointLatLng point = new PointLatLng(lat, lng);
                GMarkerGoogle m = new GMarkerGoogle(point, GMarkerGoogleType.green);
                m.ToolTipMode = MarkerTooltipMode.Always;
                m.ToolTipText = tag;
                m.Tag = tag;

                GMapMarkerRect mBorders = new GMapMarkerRect(point);
                {
                    mBorders.InnerMarker = m;
                    try
                    {
                        mBorders.wprad =
                            (int) (Settings.Instance.GetFloat("TXT_WPRad")/CurrentState.multiplieralt);
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }
                    if (color.HasValue)
                    {
                        mBorders.Color = color.Value;
                    }
                }

                overlay.Markers.Add(m);
                overlay.Markers.Add(mBorders);
            }
            catch (Exception)
            {
            }
        }

        private void GeoFenceuploadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            polygongridmode = false;
            //FENCE_ENABLE ON COPTER
            //FENCE_ACTION ON PLANE
            if (!MainV2.comPort.MAV.param.ContainsKey("FENCE_ENABLE") && !MainV2.comPort.MAV.param.ContainsKey("FENCE_ACTION"))
            {
                CustomMessageBox.Show("Not Supported");
                return;
            }

            if (drawnpolygon == null)
            {
                CustomMessageBox.Show("No polygon to upload");
                return;
            }

            if (geofenceoverlay.Markers.Count == 0)
            {
                CustomMessageBox.Show("No return location set");
                return;
            }

            if (drawnpolygon.Points.Count == 0)
            {
                CustomMessageBox.Show("No polygon drawn");
                return;
            }

            // check if return is inside polygon
            List<PointLatLng> plll = new List<PointLatLng>(drawnpolygon.Points.ToArray());
            // close it
            plll.Add(plll[0]);
            // check it
            if (
                !pnpoly(plll.ToArray(), geofenceoverlay.Markers[0].Position.Lat, geofenceoverlay.Markers[0].Position.Lng))
            {
                CustomMessageBox.Show("Your return location is outside the polygon");
                return;
            }

            int minalt = 0;
            int maxalt = 0;

            if (MainV2.comPort.MAV.param.ContainsKey("FENCE_MINALT"))
            {
                string minalts =
                    (int.Parse(MainV2.comPort.MAV.param["FENCE_MINALT"].ToString())*CurrentState.multiplieralt)
                        .ToString(
                            "0");
                if (DialogResult.Cancel == InputBox.Show("Min Alt", "Box Minimum Altitude?", ref minalts))
                    return;

                if (!int.TryParse(minalts, out minalt))
                {
                    CustomMessageBox.Show("Bad Min Alt");
                    return;
                }
            }

            if (MainV2.comPort.MAV.param.ContainsKey("FENCE_MAXALT"))
            {
                string maxalts =
                    (int.Parse(MainV2.comPort.MAV.param["FENCE_MAXALT"].ToString())*CurrentState.multiplieralt)
                        .ToString(
                            "0");
                if (DialogResult.Cancel == InputBox.Show("Max Alt", "Box Maximum Altitude?", ref maxalts))
                    return;

                if (!int.TryParse(maxalts, out maxalt))
                {
                    CustomMessageBox.Show("Bad Max Alt");
                    return;
                }
            }

            try
            {
                if (MainV2.comPort.MAV.param.ContainsKey("FENCE_MINALT"))
                    MainV2.comPort.setParam("FENCE_MINALT", minalt);
                if (MainV2.comPort.MAV.param.ContainsKey("FENCE_MAXALT"))
                    MainV2.comPort.setParam("FENCE_MAXALT", maxalt);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                CustomMessageBox.Show("Failed to set min/max fence alt");
                return;
            }

            float oldaction = (float) MainV2.comPort.MAV.param["FENCE_ACTION"];

            try
            {
                MainV2.comPort.setParam("FENCE_ACTION", 0);
            }
            catch
            {
                CustomMessageBox.Show("Failed to set FENCE_ACTION");
                return;
            }

            // points + return + close
            byte pointcount = (byte) (drawnpolygon.Points.Count + 2);


            try
            {
                MainV2.comPort.setParam("FENCE_TOTAL", pointcount);
            }
            catch
            {
                CustomMessageBox.Show("Failed to set FENCE_TOTAL");
                return;
            }

            try
            {
                byte a = 0;
                // add return loc
                MainV2.comPort.setFencePoint(a, new PointLatLngAlt(geofenceoverlay.Markers[0].Position), pointcount);
                a++;
                // add points
                foreach (var pll in drawnpolygon.Points)
                {
                    MainV2.comPort.setFencePoint(a, new PointLatLngAlt(pll), pointcount);
                    a++;
                }

                // add polygon close
                MainV2.comPort.setFencePoint(a, new PointLatLngAlt(drawnpolygon.Points[0]), pointcount);

                try
                {
                    MainV2.comPort.setParam("FENCE_ACTION", oldaction);
                }
                catch
                {
                    CustomMessageBox.Show("Failed to restore FENCE_ACTION");
                    return;
                }

                // clear everything
                drawnpolygonsoverlay.Polygons.Clear();
                drawnpolygonsoverlay.Markers.Clear();
                geofenceoverlay.Polygons.Clear();
                geofencepolygon.Points.Clear();

                // add polygon
                geofencepolygon.Points.AddRange(drawnpolygon.Points.ToArray());

                drawnpolygon.Points.Clear();

                geofenceoverlay.Polygons.Add(geofencepolygon);

                // update flightdata
                FlightData.geofence.Markers.Clear();
                FlightData.geofence.Polygons.Clear();
                FlightData.geofence.Polygons.Add(new GMapPolygon(geofencepolygon.Points, "gf fd")
                {
                    Stroke = geofencepolygon.Stroke,
                    Fill = Brushes.Transparent
                });
                FlightData.geofence.Markers.Add(new GMarkerGoogle(geofenceoverlay.Markers[0].Position,
                    GMarkerGoogleType.red)
                {
                    ToolTipText = geofenceoverlay.Markers[0].ToolTipText,
                    ToolTipMode = geofenceoverlay.Markers[0].ToolTipMode
                });

                MainMap.UpdatePolygonLocalPosition(geofencepolygon);
                MainMap.UpdateMarkerLocalPosition(geofenceoverlay.Markers[0]);

                MainMap.Invalidate();
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Failed to send new fence points " + ex, Strings.ERROR);
            }
        }

        public void GeoFencedownloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            polygongridmode = false;
            int count = 1;

            if (MainV2.comPort.MAV.param["FENCE_ACTION"] == null || MainV2.comPort.MAV.param["FENCE_TOTAL"] == null)
            {
                CustomMessageBox.Show("Not Supported");
                return;
            }

            if (int.Parse(MainV2.comPort.MAV.param["FENCE_TOTAL"].ToString()) <= 1)
            {
                CustomMessageBox.Show("Nothing to download");
                return;
            }

            geofenceoverlay.Polygons.Clear();
            geofenceoverlay.Markers.Clear();
            geofencepolygon.Points.Clear();


            for (int a = 0; a < count; a++)
            {
                try
                {
                    PointLatLngAlt plla = MainV2.comPort.getFencePoint(a, ref count);
                    geofencepolygon.Points.Add(new PointLatLng(plla.Lat, plla.Lng));
                }
                catch
                {
                    CustomMessageBox.Show("Failed to get fence point", Strings.ERROR);
                    return;
                }
            }

            // do return location
            geofenceoverlay.Markers.Add(
                new GMarkerGoogle(new PointLatLng(geofencepolygon.Points[0].Lat, geofencepolygon.Points[0].Lng),
                    GMarkerGoogleType.red)
                {
                    ToolTipMode = MarkerTooltipMode.OnMouseOver,
                    ToolTipText = "GeoFence Return"
                });
            geofencepolygon.Points.RemoveAt(0);

            // add now - so local points are calced
            geofenceoverlay.Polygons.Add(geofencepolygon);

            // update flight data
            FlightData.geofence.Markers.Clear();
            FlightData.geofence.Polygons.Clear();
            FlightData.geofence.Polygons.Add(new GMapPolygon(geofencepolygon.Points, "gf fd")
            {
                Stroke = geofencepolygon.Stroke,
                Fill = Brushes.Transparent
            });
            FlightData.geofence.Markers.Add(new GMarkerGoogle(geofenceoverlay.Markers[0].Position, GMarkerGoogleType.red)
            {
                ToolTipText = geofenceoverlay.Markers[0].ToolTipText,
                ToolTipMode = geofenceoverlay.Markers[0].ToolTipMode
            });

            MainMap.UpdatePolygonLocalPosition(geofencepolygon);
            MainMap.UpdateMarkerLocalPosition(geofenceoverlay.Markers[0]);

            MainMap.Invalidate();
        }

        private void setReturnLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            geofenceoverlay.Markers.Clear();
            geofenceoverlay.Markers.Add(new GMarkerGoogle(new PointLatLng(MouseDownStart.Lat, MouseDownStart.Lng),
                GMarkerGoogleType.red) {ToolTipMode = MarkerTooltipMode.OnMouseOver, ToolTipText = "GeoFence Return"});

            MainMap.Invalidate();
        }

        /// <summary>
        /// from http://www.ecse.rpi.edu/Homepages/wrf/Research/Short_Notes/pnpoly.html
        /// </summary>
        /// <param name="array"> a closed polygon</param>
        /// <param name="testx"></param>
        /// <param name="testy"></param>
        /// <returns> true = outside</returns>
        bool pnpoly(PointLatLng[] array, double testx, double testy)
        {
            int nvert = array.Length;
            int i, j = 0;
            bool c = false;
            for (i = 0, j = nvert - 1; i < nvert; j = i++)
            {
                if (((array[i].Lng > testy) != (array[j].Lng > testy)) &&
                    (testx <
                     (array[j].Lat - array[i].Lat)*(testy - array[i].Lng)/(array[j].Lng - array[i].Lng) + array[i].Lat))
                    c = !c;
            }
            return c;
        }

        private void loadFromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog fd = new OpenFileDialog())
            {
                fd.Filter = "Fence (*.fen)|*.fen";
                fd.ShowDialog();
                if (File.Exists(fd.FileName))
                {
                    StreamReader sr = new StreamReader(fd.OpenFile());

                    drawnpolygonsoverlay.Markers.Clear();
                    drawnpolygonsoverlay.Polygons.Clear();
                    drawnpolygon.Points.Clear();

                    int a = 0;

                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        if (line.StartsWith("#"))
                        {
                        }
                        else
                        {
                            string[] items = line.Split(new[] {' ', '\t'}, StringSplitOptions.RemoveEmptyEntries);

                            if (a == 0)
                            {
                                geofenceoverlay.Markers.Clear();
                                geofenceoverlay.Markers.Add(
                                    new GMarkerGoogle(new PointLatLng(double.Parse(items[0], CultureInfo.InvariantCulture), double.Parse(items[1], CultureInfo.InvariantCulture)),
                                        GMarkerGoogleType.red)
                                    {
                                        ToolTipMode = MarkerTooltipMode.OnMouseOver,
                                        ToolTipText = "GeoFence Return"
                                    });
                                MainMap.UpdateMarkerLocalPosition(geofenceoverlay.Markers[0]);
                            }
                            else
                            {
                                drawnpolygon.Points.Add(new PointLatLng(
                                    double.Parse(items[0], CultureInfo.InvariantCulture),
                                    double.Parse(items[1], CultureInfo.InvariantCulture)));
                                addpolygonmarkergrid(drawnpolygon.Points.Count.ToString(),
                                    double.Parse(items[1], CultureInfo.InvariantCulture),
                                    double.Parse(items[0], CultureInfo.InvariantCulture), 0);
                            }
                            a++;
                        }
                    }

                    // remove loop close
                    if (drawnpolygon.Points.Count > 1 &&
                        drawnpolygon.Points[0] == drawnpolygon.Points[drawnpolygon.Points.Count - 1])
                    {
                        drawnpolygon.Points.RemoveAt(drawnpolygon.Points.Count - 1);
                    }

                    drawnpolygonsoverlay.Polygons.Add(drawnpolygon);

                    MainMap.UpdatePolygonLocalPosition(drawnpolygon);

                    MainMap.Invalidate();
                }
            }
        }

        private void saveToFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (geofenceoverlay.Markers.Count == 0)
            {
                CustomMessageBox.Show("Please set a return location");
                return;
            }


            using (SaveFileDialog sf = new SaveFileDialog())
            {
                sf.Filter = "Fence (*.fen)|*.fen";
                var result = sf.ShowDialog();
                if (sf.FileName != "" && result == DialogResult.OK)
                {
                    try
                    {
                        StreamWriter sw = new StreamWriter(sf.OpenFile());

                        sw.WriteLine("#saved by APM Planner " + Application.ProductVersion);

                        sw.WriteLine(geofenceoverlay.Markers[0].Position.Lat.ToString(CultureInfo.InvariantCulture) + " " +
                                     geofenceoverlay.Markers[0].Position.Lng.ToString(CultureInfo.InvariantCulture));
                        if (drawnpolygon.Points.Count > 0)
                        {
                            foreach (var pll in drawnpolygon.Points)
                            {
                                sw.WriteLine(pll.Lat.ToString(CultureInfo.InvariantCulture) + " " + pll.Lng.ToString(CultureInfo.InvariantCulture));
                            }

                            PointLatLng pll2 = drawnpolygon.Points[0];

                            sw.WriteLine(pll2.Lat.ToString(CultureInfo.InvariantCulture) + " " + pll2.Lng.ToString(CultureInfo.InvariantCulture));
                        }
                        else
                        {
                            foreach (var pll in geofencepolygon.Points)
                            {
                                sw.WriteLine(pll.Lat.ToString(CultureInfo.InvariantCulture) + " " + pll.Lng.ToString(CultureInfo.InvariantCulture));
                            }

                            PointLatLng pll2 = geofencepolygon.Points[0];

                            sw.WriteLine(pll2.Lat.ToString(CultureInfo.InvariantCulture) + " " + pll2.Lng.ToString(CultureInfo.InvariantCulture));
                        }

                        sw.Close();
                    }
                    catch
                    {
                        CustomMessageBox.Show("Failed to write fence file");
                    }
                }
            }
        }

        public T DeepClone<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();

                formatter.Serialize(ms, obj);

                ms.Position = 0;

                return (T) formatter.Deserialize(ms);
            }
        }

        private void createWpCircleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string RadiusIn = "50";
            if (DialogResult.Cancel == InputBox.Show("Radius", "Radius", ref RadiusIn))
                return;

            string Pointsin = "20";
            if (DialogResult.Cancel == InputBox.Show("Points", "Number of points to generate Circle", ref Pointsin))
                return;

            string Directionin = "1";
            if (DialogResult.Cancel == InputBox.Show("Points", "Direction of circle (-1 or 1)", ref Directionin))
                return;

            string startanglein = "0";
            if (DialogResult.Cancel == InputBox.Show("angle", "Angle of first point (whole degrees)", ref startanglein))
                return;

            int Points = 0;
            int Radius = 0;
            int Direction = 1;
            int startangle = 0;

            if (!int.TryParse(RadiusIn, out Radius))
            {
                CustomMessageBox.Show("Bad Radius");
                return;
            }

            Radius = (int)(Radius / CurrentState.multiplierdist);

            if (!int.TryParse(Pointsin, out Points))
            {
                CustomMessageBox.Show("Bad Point value");
                return;
            }

            if (!int.TryParse(Directionin, out Direction))
            {
                CustomMessageBox.Show("Bad Direction value");
                return;
            }

            if (!int.TryParse(startanglein, out startangle))
            {
                CustomMessageBox.Show("Bad start angle value");
                return;
            }

            double a = startangle;
            double step = 360.0f/Points;
            if (Direction == -1)
            {
                a += 360;
                step *= -1;
            }

            quickadd = true;

            for (; a <= (startangle + 360) && a >= 0; a += step)
            {
                selectedrow = Commands.Rows.Add();

                Commands.Rows[selectedrow].Cells[Command.Index].Value = MAVLink.MAV_CMD.WAYPOINT.ToString();

                ChangeColumnHeader(MAVLink.MAV_CMD.WAYPOINT.ToString());

                float d = Radius;
                float R = 6371000;

                var lat2 = Math.Asin(Math.Sin(MouseDownEnd.Lat*MathHelper.deg2rad)*Math.Cos(d/R) +
                                     Math.Cos(MouseDownEnd.Lat*MathHelper.deg2rad)*Math.Sin(d/R)*Math.Cos(a*MathHelper.deg2rad));
                var lon2 = MouseDownEnd.Lng*MathHelper.deg2rad +
                           Math.Atan2(Math.Sin(a*MathHelper.deg2rad)*Math.Sin(d/R)*Math.Cos(MouseDownEnd.Lat*MathHelper.deg2rad),
                               Math.Cos(d/R) - Math.Sin(MouseDownEnd.Lat*MathHelper.deg2rad)*Math.Sin(lat2));

                PointLatLng pll = new PointLatLng(lat2*MathHelper.rad2deg, lon2*MathHelper.rad2deg);

                setfromMap(pll.Lat, pll.Lng, (int) float.Parse(TXT_DefaultAlt.Text));
            }

            quickadd = false;
            writeKML();
        }

        public void Activate()
        {
            timer1.Start();

            // set the firmware type if we are not connected. this allows overrideing
            if (!MainV2.comPort.BaseStream.IsOpen)
            {
                MainV2.comPort.MAV.cs.firmware = (Firmwares) MainV2._connectionControl.TOOL_APMFirmware.SelectedItem;
            }

            // hide altmode if old copter version
            if (MainV2.comPort.BaseStream.IsOpen && MainV2.comPort.MAV.cs.firmware == Firmwares.ArduCopter2 &&
                MainV2.comPort.MAV.cs.version < new Version(3, 3))
            {
                CMB_altmode.Visible = false;
            }
            else
            {
                CMB_altmode.Visible = true;
            }

            // hide spline wp options if not arducopter
            if (MainV2.comPort.MAV.cs.firmware == Firmwares.ArduCopter2)
                CHK_splinedefault.Visible = true;
            else
                CHK_splinedefault.Visible = false;

            updateHome();

            setWPParams();

            updateCMDParams();

            try
            {
                int.Parse(TXT_DefaultAlt.Text);
            }
            catch
            {
                CustomMessageBox.Show("Please fix your default alt value");
                TXT_DefaultAlt.Text = (50*CurrentState.multiplieralt).ToString("0");
            }
        }

        public void updateHome()
        {
            quickadd = true;
            updateHomeText();
            quickadd = false;
        }

        private void updateHomeText()
        {
            // set home location
            if (MainV2.comPort.MAV.cs.HomeLocation.Lat != 0 && MainV2.comPort.MAV.cs.HomeLocation.Lng != 0)
            {
                TXT_homelat.Text = MainV2.comPort.MAV.cs.HomeLocation.Lat.ToString();

                TXT_homelng.Text = MainV2.comPort.MAV.cs.HomeLocation.Lng.ToString();

                TXT_homealt.Text = MainV2.comPort.MAV.cs.HomeLocation.Alt.ToString();

                writeKML();
            }
        }

        public void Deactivate()
        {
            config(true);
            timer1.Stop();
        }

        private void FlightPlanner_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Stop();
        }

        private void setROIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!cmdParamNames.ContainsKey("DO_SET_ROI"))
            {
                CustomMessageBox.Show(Strings.ErrorFeatureNotEnabled, Strings.ERROR);
                return;
            }

            selectedrow = Commands.Rows.Add();

            Commands.Rows[selectedrow].Cells[Command.Index].Value = MAVLink.MAV_CMD.DO_SET_ROI.ToString();

            //Commands.Rows[selectedrow].Cells[Param1.Index].Value = time;

            ChangeColumnHeader(MAVLink.MAV_CMD.DO_SET_ROI.ToString());

            setfromMap(MouseDownEnd.Lat, MouseDownEnd.Lng, (int) float.Parse(TXT_DefaultAlt.Text));

            writeKML();
        }

        private void zoomToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string place = "Perth Airport, Australia";
            if (DialogResult.OK == InputBox.Show("Location", "Enter your location", ref place))
            {
                GeoCoderStatusCode status = MainMap.SetPositionByKeywords(place);
                if (status != GeoCoderStatusCode.G_GEO_SUCCESS)
                {
                    CustomMessageBox.Show("Google Maps Geocoder can't find: '" + place + "', reason: " + status,
                        "GMap.NET", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    MainMap.Zoom = 15;
                }
            }
        }

        bool fetchpathrip;

        private void FetchPath()
        {
            PointLatLngAlt lastpnt = null;

            string maxzoomstring = "20";
            if(InputBox.Show("max zoom", "Enter the max zoom to prefetch to.", ref maxzoomstring) != DialogResult.OK)
                return;

            int maxzoom = 20;
            if (!int.TryParse(maxzoomstring, out maxzoom))
            {
                CustomMessageBox.Show(Strings.InvalidNumberEntered, Strings.ERROR);
                return;
            }

            fetchpathrip = true;

            maxzoom = Math.Min(maxzoom, MainMap.MaxZoom);

            // zoom
            for (int i = 1; i <= maxzoom; i++)
            {
                // exit if reqested
                if (!fetchpathrip)
                    break;

                lastpnt = null;
                // location
                foreach (var pnt in pointlist)
                {
                    if (pnt == null)
                        continue;

                    // exit if reqested
                    if (!fetchpathrip)
                        break;

                    // setup initial enviroment
                    if (lastpnt == null)
                    {
                        lastpnt = pnt;
                        continue;
                    }

                    RectLatLng area = new RectLatLng();
                    double top = Math.Max(lastpnt.Lat, pnt.Lat);
                    double left = Math.Min(lastpnt.Lng, pnt.Lng);
                    double bottom = Math.Min(lastpnt.Lat, pnt.Lat);
                    double right = Math.Max(lastpnt.Lng, pnt.Lng);

                    area.LocationTopLeft = new PointLatLng(top, left);
                    area.HeightLat = top - bottom;
                    area.WidthLng = right - left;

                    TilePrefetcher obj = new TilePrefetcher();
                    ThemeManager.ApplyThemeTo(obj);
                    obj.KeyDown += obj_KeyDown;
                    obj.ShowCompleteMessage = false;
                    obj.Start(area, i, MainMap.MapProvider, 0, 0);

                    if (obj.UserAborted)
                    {
                        fetchpathrip = false;
                        break;
                    }

                    lastpnt = pnt;
                }
            }
        }

        void obj_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                fetchpathrip = false;
            }
        }

        private void prefetchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RectLatLng area = MainMap.SelectedArea;
            if (area.IsEmpty)
            {
                var res = CustomMessageBox.Show("No ripp area defined, ripp displayed on screen?", "Rip",
                    MessageBoxButtons.YesNo);
                if (res == (int)DialogResult.Yes)
                {
                    area = MainMap.ViewArea;
                }
            }

            if (!area.IsEmpty)
            {
                string maxzoomstring = "20";
                if (InputBox.Show("max zoom", "Enter the max zoom to prefetch to.", ref maxzoomstring) != DialogResult.OK)
                    return;

                int maxzoom = 20;
                if (!int.TryParse(maxzoomstring, out maxzoom))
                {
                    CustomMessageBox.Show(Strings.InvalidNumberEntered, Strings.ERROR);
                    return;
                }

                maxzoom = Math.Min(maxzoom, MainMap.MaxZoom);

                for (int i = 1; i <= maxzoom; i++)
                {
                    TilePrefetcher obj = new TilePrefetcher();
                    ThemeManager.ApplyThemeTo(obj);
                    obj.ShowCompleteMessage = false;
                    obj.Start(area, i, MainMap.MapProvider, 0, 0);

                    if (obj.UserAborted)
                    {
                        obj.Dispose();
                        break;
                    }

                    obj.Dispose();
                }
            }
            else
            {
                CustomMessageBox.Show("Select map area holding ALT", "GMap.NET", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
            }
        }

        private void kMLOverlayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog fd = new OpenFileDialog())
            {
                fd.Filter = "All Supported|*.kml;*.kmz;*.dxf;*.gpkg|Google Earth KML|*.kml;*.kmz|AutoCad DXF|*.dxf|GeoPackage|*.gpkg";
                DialogResult result = fd.ShowDialog();
                string file = fd.FileName;
                if (file != "")
                {
                    kmlpolygonsoverlay.Polygons.Clear();
                    kmlpolygonsoverlay.Routes.Clear();

                    FlightData.kmlpolygons.Routes.Clear();
                    FlightData.kmlpolygons.Polygons.Clear();
                    if (file.ToLower().EndsWith("gpkg"))
                    {
                        using (var ogr = OGR.Open(file))
                        {
                            ogr.NewPoint += pnt =>
                            {
                                var mark = new GMarkerGoogle(new PointLatLngAlt(pnt), GMarkerGoogleType.brown_small);
                                FlightData.kmlpolygons.Markers.Add(mark);
                                kmlpolygonsoverlay.Markers.Add(mark);
                            };
                            ogr.NewLineString += ls =>
                            {
                                var route =
                                    new GMapRoute(ls.Select(a => new PointLatLngAlt(a.y, a.x, a.z).Point()), "")
                                    {
                                        IsHitTestVisible = false,
                                        Stroke = new Pen(Color.Red)
                                    };
                                FlightData.kmlpolygons.Routes.Add(route);
                                kmlpolygonsoverlay.Routes.Add(route);
                            };
                            ogr.NewPolygon += ls =>
                            {
                                var polygon =
                                    new GMapPolygon(ls.Select(a => new PointLatLngAlt(a.y, a.x, a.z).Point()).ToList(), "")
                                    {
                                        Fill = Brushes.Transparent,
                                        IsHitTestVisible = false,
                                        Stroke = new Pen(Color.Red)
                                    };
                                FlightData.kmlpolygons.Polygons.Add(polygon);
                                kmlpolygonsoverlay.Polygons.Add(polygon);
                            };

                            ogr.Process();
                        }
                    }
                    else if (file.ToLower().EndsWith("dxf"))
                    {
                        string zone = "-99";
                        InputBox.Show("Zone", "Please enter the UTM zone, or cancel to not change", ref zone);

                        dxf dxf = new dxf();
                        if (zone != "-99")
                            dxf.Tag = zone;

                        dxf.newLine += Dxf_newLine;
                        dxf.newPolyLine += Dxf_newPolyLine;
                        dxf.newLwPolyline += Dxf_newLwPolyline;
                        dxf.newMLine += Dxf_newMLine;
                        dxf.Read(file);
                    }
                    else
                    {
                        try
                        {
                            string kml = "";
                            string tempdir = "";
                            if (file.ToLower().EndsWith("kmz"))
                            {
                                ZipFile input = new ZipFile(file);

                                tempdir = Path.GetTempPath() + Path.DirectorySeparatorChar + Path.GetRandomFileName();
                                input.ExtractAll(tempdir, ExtractExistingFileAction.OverwriteSilently);

                                string[] kmls = Directory.GetFiles(tempdir, "*.kml");

                                if (kmls.Length > 0)
                                {
                                    file = kmls[0];

                                    input.Dispose();
                                }
                                else
                                {
                                    input.Dispose();
                                    return;
                                }
                            }

                            var sr = new StreamReader(File.OpenRead(file));
                            kml = sr.ReadToEnd();
                            sr.Close();

                            // cleanup after out
                            if (tempdir != "")
                                Directory.Delete(tempdir, true);

                            kml = kml.Replace("<Snippet/>", "");

                            var parser = new Parser();

                            parser.ElementAdded += parser_ElementAdded;
                            parser.ParseString(kml, false);

                            if ((int)DialogResult.Yes ==
                                CustomMessageBox.Show(Strings.Do_you_want_to_load_this_into_the_flight_data_screen, Strings.Load_data,
                                    MessageBoxButtons.YesNo))
                            {
                                foreach (var temp in kmlpolygonsoverlay.Polygons)
                                {
                                    FlightData.kmlpolygons.Polygons.Add(temp);
                                }
                                foreach (var temp in kmlpolygonsoverlay.Routes)
                                {
                                    FlightData.kmlpolygons.Routes.Add(temp);
                                }
                            }

                            if (
                                CustomMessageBox.Show(Strings.Zoom_To, Strings.Zoom_to_the_center_or_the_loaded_file, MessageBoxButtons.YesNo) ==
                                (int)DialogResult.Yes)
                            {
                                MainMap.SetZoomToFitRect(GetBoundingLayer(kmlpolygonsoverlay));
                            }
                        }
                        catch (Exception ex)
                        {
                            CustomMessageBox.Show(Strings.Bad_KML_File + ex);
                        }
                    }
                }
            }
        }

        private void Dxf_newMLine(dxf sender, netDxf.Entities.MLine pline)
        {
            var route = new GMapRoute(pline.Handle);
            foreach (var item in pline.Vertexes)
            {
                route.Points.Add(new PointLatLng(item.Location.Y, item.Location.X));
            }

            route.Stroke = new Pen(Color.FromArgb(pline.Color.R, pline.Color.G, pline.Color.B));

            if (sender.Tag != null)
                ConvertUTMCoords(route, int.Parse(sender.Tag.ToString()));

            kmlpolygonsoverlay.Routes.Add(route);
        }

        private void Dxf_newLwPolyline(dxf sender, netDxf.Entities.LwPolyline pline)
        {
            var route = new GMapRoute(pline.Handle);
            foreach (var item in pline.Vertexes)
            {
                route.Points.Add(new PointLatLng(item.Position.Y, item.Position.X));
            }

            route.Stroke = new Pen(Color.FromArgb(pline.Color.R, pline.Color.G, pline.Color.B));

            if (sender.Tag != null)
                ConvertUTMCoords(route, int.Parse(sender.Tag.ToString()));

            kmlpolygonsoverlay.Routes.Add(route);
        }

        private void Dxf_newPolyLine(dxf sender, netDxf.Entities.Polyline pline)
        {
            var route = new GMapRoute(pline.Handle);
            foreach (var item in pline.Vertexes)
            {
                route.Points.Add(new PointLatLng(item.Position.Y, item.Position.X));
            }

            route.Stroke = new Pen(Color.FromArgb(pline.Color.R, pline.Color.G, pline.Color.B));

            if (sender.Tag != null)
                ConvertUTMCoords(route, int.Parse(sender.Tag.ToString()));

            kmlpolygonsoverlay.Routes.Add(route);
        }

        private void Dxf_newLine(dxf sender, netDxf.Entities.Line line)
        {
            var route = new GMapRoute(line.Handle);
            route.Points.Add(new PointLatLng(line.StartPoint.Y,line.StartPoint.X));
            route.Points.Add(new PointLatLng(line.EndPoint.Y, line.EndPoint.X));

            route.Stroke = new Pen(Color.FromArgb(line.Color.R, line.Color.G, line.Color.B));

            if (sender.Tag != null)
                ConvertUTMCoords(route, int.Parse(sender.Tag.ToString()));

            kmlpolygonsoverlay.Routes.Add(route);
        }

        void ConvertUTMCoords(GMapRoute route, int zone = -9999)
        {
            for (int i =0;i < route.Points.Count;i++)
            {
                var pnt = route.Points[i];
                // load input
                utmpos pos = new utmpos(pnt.Lng, pnt.Lat, zone);
                // convert to geo
                var llh = pos.ToLLA();
                // save it back
                route.Points[i] = llh;
                //route.Points[i].Lng = llh.Lng;
            }
        }


        public static RectLatLng GetBoundingLayer(GMapOverlay o)
        {
            RectLatLng ret = RectLatLng.Empty;

            double left = double.MaxValue;
            double top = double.MinValue;
            double right = double.MinValue;
            double bottom = double.MaxValue;

            if (o.IsVisibile)
            {
                foreach (var m in o.Markers)
                {
                    if (m.IsVisible)
                    {
                        // left
                        if (m.Position.Lng < left)
                        {
                            left = m.Position.Lng;
                        }

                        // top
                        if (m.Position.Lat > top)
                        {
                            top = m.Position.Lat;
                        }

                        // right
                        if (m.Position.Lng > right)
                        {
                            right = m.Position.Lng;
                        }

                        // bottom
                        if (m.Position.Lat < bottom)
                        {
                            bottom = m.Position.Lat;
                        }
                    }
                }
                foreach (GMapRoute route in o.Routes)
                {
                    if (route.IsVisible && route.From.HasValue && route.To.HasValue)
                    {
                        foreach (PointLatLng p in route.Points)
                        {
                            // left
                            if (p.Lng < left)
                            {
                                left = p.Lng;
                            }

                            // top
                            if (p.Lat > top)
                            {
                                top = p.Lat;
                            }

                            // right
                            if (p.Lng > right)
                            {
                                right = p.Lng;
                            }

                            // bottom
                            if (p.Lat < bottom)
                            {
                                bottom = p.Lat;
                            }
                        }
                    }
                }
                foreach (GMapPolygon polygon in o.Polygons)
                {
                    if (polygon.IsVisible)
                    {
                        foreach (PointLatLng p in polygon.Points)
                        {
                            // left
                            if (p.Lng < left)
                            {
                                left = p.Lng;
                            }

                            // top
                            if (p.Lat > top)
                            {
                                top = p.Lat;
                            }

                            // right
                            if (p.Lng > right)
                            {
                                right = p.Lng;
                            }

                            // bottom
                            if (p.Lat < bottom)
                            {
                                bottom = p.Lat;
                            }
                        }
                    }
                }
            }

            if (left != double.MaxValue && right != double.MinValue && top != double.MinValue && bottom != double.MaxValue)
            {
                ret = RectLatLng.FromLTRB(left, top, right, bottom);
            }

            return ret;
        }

        private void elevationGraphToolStripMenuItem_Click(object sender, EventArgs e)
        {
            writeKML();
            double homealt = MainV2.comPort.MAV.cs.HomeAlt;
            Form temp = new ElevationProfile(pointlist, homealt, (altmode) CMB_altmode.SelectedValue);
            ThemeManager.ApplyThemeTo(temp);
            temp.ShowDialog();
        }

        private void rTLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            selectedrow = Commands.Rows.Add();

            Commands.Rows[selectedrow].Cells[Command.Index].Value = MAVLink.MAV_CMD.RETURN_TO_LAUNCH.ToString();

            //Commands.Rows[selectedrow].Cells[Param1.Index].Value = time;

            ChangeColumnHeader(MAVLink.MAV_CMD.RETURN_TO_LAUNCH.ToString());

            writeKML();
        }

        private void landToolStripMenuItem_Click(object sender, EventArgs e)
        {
            selectedrow = Commands.Rows.Add();

            Commands.Rows[selectedrow].Cells[Command.Index].Value = MAVLink.MAV_CMD.LAND.ToString();

            //Commands.Rows[selectedrow].Cells[Param1.Index].Value = time;

            ChangeColumnHeader(MAVLink.MAV_CMD.LAND.ToString());

            setfromMap(MouseDownEnd.Lat, MouseDownEnd.Lng, 1);

            writeKML();
        }

        private void AddDigicamControlPhoto()
        {
            selectedrow = Commands.Rows.Add();

            Commands.Rows[selectedrow].Cells[Command.Index].Value = MAVLink.MAV_CMD.DO_DIGICAM_CONTROL.ToString();

            ChangeColumnHeader(MAVLink.MAV_CMD.DO_DIGICAM_CONTROL.ToString());

            writeKML();
        }

        public int AddCommand(MAVLink.MAV_CMD cmd, double p1, double p2, double p3, double p4, double x, double y,
            double z, object tag = null)
        {
            selectedrow = Commands.Rows.Add();

            FillCommand(this.selectedrow, cmd, p1, p2, p3, p4, x, y, z, tag);

            writeKML();

            return selectedrow;
        }

        public void InsertCommand(int rowIndex, MAVLink.MAV_CMD cmd, double p1, double p2, double p3, double p4, double x, double y,
            double z, object tag = null)
        {
            if (Commands.Rows.Count <= rowIndex)
            {
                AddCommand(cmd, p1, p2, p3, p4, x, y, z, tag);
                return;
            }

            Commands.Rows.Insert(rowIndex);

            this.selectedrow = rowIndex;

            FillCommand(this.selectedrow, cmd, p1, p2, p3, p4, x, y, z, tag);

            writeKML();
        }

        private void FillCommand(int rowIndex, MAVLink.MAV_CMD cmd, double p1, double p2, double p3, double p4, double x,
            double y, double z, object tag = null)
        {
            Commands.Rows[rowIndex].Cells[Command.Index].Value = cmd.ToString();
            Commands.Rows[rowIndex].Cells[TagData.Index].Tag = tag;
            Commands.Rows[rowIndex].Cells[TagData.Index].Value = tag;

            ChangeColumnHeader(cmd.ToString());

            // switch wp to spline if spline checked
            if (splinemode && cmd == MAVLink.MAV_CMD.WAYPOINT)
            {
                Commands.Rows[rowIndex].Cells[Command.Index].Value = MAVLink.MAV_CMD.SPLINE_WAYPOINT.ToString();
                ChangeColumnHeader(MAVLink.MAV_CMD.SPLINE_WAYPOINT.ToString());
            }

            if (cmd == MAVLink.MAV_CMD.WAYPOINT)
            {
                // add delay if supplied
                Commands.Rows[rowIndex].Cells[Param1.Index].Value = p1;

                setfromMap(y, x, (int)z, Math.Round(p1, 1));
            }
            else if (cmd == MAVLink.MAV_CMD.LOITER_UNLIM)
            {
                setfromMap(y, x, (int)z);
            }
            else
            {
                Commands.Rows[rowIndex].Cells[Param1.Index].Value = p1;
                Commands.Rows[rowIndex].Cells[Param2.Index].Value = p2;
                Commands.Rows[rowIndex].Cells[Param3.Index].Value = p3;
                Commands.Rows[rowIndex].Cells[Param4.Index].Value = p4;
                Commands.Rows[rowIndex].Cells[Lat.Index].Value = y;
                Commands.Rows[rowIndex].Cells[Lon.Index].Value = x;
                Commands.Rows[rowIndex].Cells[Alt.Index].Value = z;
            }
        }

        private void takeoffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // altitude
            string alt = "10";

            if (DialogResult.Cancel == InputBox.Show("Altitude", "Please enter your takeoff altitude", ref alt))
                return;

            int alti = -1;

            if (!int.TryParse(alt, out alti))
            {
                MessageBox.Show("Bad Alt");
                return;
            }

            // take off pitch
            int topi = 0;

            if (MainV2.comPort.MAV.cs.firmware == Firmwares.ArduPlane ||
                MainV2.comPort.MAV.cs.firmware == Firmwares.Ateryx)
            {
                string top = "15";

                if (DialogResult.Cancel == InputBox.Show("Takeoff Pitch", "Please enter your takeoff pitch", ref top))
                    return;

                if (!int.TryParse(top, out topi))
                {
                    MessageBox.Show("Bad Takeoff pitch");
                    return;
                }
            }

            selectedrow = Commands.Rows.Add();

            Commands.Rows[selectedrow].Cells[Command.Index].Value = MAVLink.MAV_CMD.TAKEOFF.ToString();

            Commands.Rows[selectedrow].Cells[Param1.Index].Value = topi;

            Commands.Rows[selectedrow].Cells[Alt.Index].Value = alti;

            ChangeColumnHeader(MAVLink.MAV_CMD.TAKEOFF.ToString());

            writeKML();
        }

        internal string wpfilename;

        private void loadWPFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BUT_loadwpfile_Click(null, null);
        }

        private void saveWPFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFile_Click(null, null);
        }

        private void trackerHomeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MainV2.comPort.MAV.cs.TrackerLocation = new PointLatLngAlt(MouseDownEnd)
            {
                Alt = MainV2.comPort.MAV.cs.HomeAlt
            };
        }

        private void reverseWPsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataGridViewRowCollection rows = Commands.Rows;
            //Commands.Rows.Clear();

            int count = rows.Count;

            quickadd = true;

            for (int a = count; a > 0; a--)
            {
                DataGridViewRow row = Commands.Rows[a - 1];
                Commands.Rows.Remove(row);
                Commands.Rows.Add(row);
            }

            quickadd = false;

            writeKML();
        }

        private void loadAndAppendToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog fd = new OpenFileDialog())
            {
                fd.Filter = "Ardupilot Mission|*.waypoints;*.txt";
                fd.DefaultExt = ".waypoints";
                DialogResult result = fd.ShowDialog();
                string file = fd.FileName;
                if (file != "")
                {
                    readQGC110wpfile(file, true);
                }
            }
        }

        private void savePolygonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (drawnpolygon.Points.Count == 0)
            {
                return;
            }


            using (SaveFileDialog sf = new SaveFileDialog())
            {
                sf.Filter = "Polygon (*.poly)|*.poly";
                var result = sf.ShowDialog();
                if (sf.FileName != "" && result == DialogResult.OK)
                {
                    try
                    {
                        StreamWriter sw = new StreamWriter(sf.OpenFile());

                        sw.WriteLine("#saved by Mission Planner " + Application.ProductVersion);

                        if (drawnpolygon.Points.Count > 0)
                        {
                            foreach (var pll in drawnpolygon.Points)
                            {
                                sw.WriteLine(pll.Lat.ToString(CultureInfo.InvariantCulture) + " " + pll.Lng.ToString(CultureInfo.InvariantCulture));
                            }

                            PointLatLng pll2 = drawnpolygon.Points[0];

                            sw.WriteLine(pll2.Lat.ToString(CultureInfo.InvariantCulture) + " " + pll2.Lng.ToString(CultureInfo.InvariantCulture));
                        }

                        sw.Close();
                    }
                    catch
                    {
                        CustomMessageBox.Show("Failed to write fence file");
                    }
                }
            }
        }

        private void loadPolygonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog fd = new OpenFileDialog())
            {
                fd.Filter = "Polygon (*.poly)|*.poly";
                fd.ShowDialog();
                if (File.Exists(fd.FileName))
                {
                    StreamReader sr = new StreamReader(fd.OpenFile());

                    drawnpolygonsoverlay.Markers.Clear();
                    drawnpolygonsoverlay.Polygons.Clear();
                    drawnpolygon.Points.Clear();

                    int a = 0;

                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        if (line.StartsWith("#"))
                        {
                        }
                        else
                        {
                            string[] items = line.Split(new[] {' ', '\t'}, StringSplitOptions.RemoveEmptyEntries);

                            if (items.Length < 2)
                                continue;

                            drawnpolygon.Points.Add(new PointLatLng(
                                double.Parse(items[0], CultureInfo.InvariantCulture),
                                double.Parse(items[1], CultureInfo.InvariantCulture)));
                            addpolygonmarkergrid(drawnpolygon.Points.Count.ToString(),
                                double.Parse(items[1], CultureInfo.InvariantCulture),
                                double.Parse(items[0], CultureInfo.InvariantCulture), 0);

                            a++;
                        }
                    }

                    // remove loop close
                    if (drawnpolygon.Points.Count > 1 &&
                        drawnpolygon.Points[0] == drawnpolygon.Points[drawnpolygon.Points.Count - 1])
                    {
                        drawnpolygon.Points.RemoveAt(drawnpolygon.Points.Count - 1);
                    }

                    drawnpolygonsoverlay.Polygons.Add(drawnpolygon);

                    MainMap.UpdatePolygonLocalPosition(drawnpolygon);

                    MainMap.Invalidate();

                    MainMap.ZoomAndCenterMarkers(drawnpolygonsoverlay.Id);
                }
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (CurentRectMarker == null && CurrentRallyPt == null && groupmarkers.Count == 0)
            {
                deleteWPToolStripMenuItem.Enabled = false;
            }
            else
            {
                deleteWPToolStripMenuItem.Enabled = true;
            }

            isMouseClickOffMenu = false; // Just incase
        }

        private void contextMenuStrip1_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            if (e.CloseReason.ToString() == "AppClicked" || e.CloseReason.ToString() == "AppFocusChange")
                isMouseClickOffMenu = true;
        }

        private void areaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double aream2 = Math.Abs(calcpolygonarea(drawnpolygon.Points));

            double areaa = aream2*0.000247105;

            double areaha = aream2*1e-4;

            double areasqf = aream2*10.7639;

            CustomMessageBox.Show(
                "Area: " + aream2.ToString("0") + " m2\n\t" + areaa.ToString("0.00") + " Acre\n\t" +
                areaha.ToString("0.00") + " Hectare\n\t" + areasqf.ToString("0") + " sqf", "Area");
        }

        private void MainMap_Paint(object sender, PaintEventArgs e)
        {
            // draw utm grid
            if (grid)
            {
                if (MainMap.Zoom < 10)
                    return;

                var rect = MainMap.ViewArea;

                var plla1 = new PointLatLngAlt(rect.LocationTopLeft);
                var plla2 = new PointLatLngAlt(rect.LocationRightBottom);

                var center = new PointLatLngAlt(rect.LocationMiddle);

                var zone = center.GetUTMZone();

                var utm1 = plla1.ToUTM(zone);
                var utm2 = plla2.ToUTM(zone);

                var deltax = utm1[0] - utm2[0];
                var deltay = utm1[1] - utm2[1];

                //if (deltax)

                var gridsize = 1000.0;


                if (Math.Abs(deltax)/100000 < 40)
                    gridsize = 100000;

                if (Math.Abs(deltax)/10000 < 40)
                    gridsize = 10000;

                if (Math.Abs(deltax)/1000 < 40)
                    gridsize = 1000;

                if (Math.Abs(deltax)/100 < 40)
                    gridsize = 100;

                if (Math.Abs(deltax) / 10 < 40)
                    gridsize = 10;

                if (Math.Abs(deltax) / 1 < 40)
                    gridsize = 1;

                // round it - x
                utm1[0] = utm1[0] - (utm1[0]%gridsize);
                // y
                utm2[1] = utm2[1] - (utm2[1]%gridsize);

                // x's
                for (double x = utm1[0]; x < utm2[0]; x += gridsize)
                {
                    var p1 = MainMap.FromLatLngToLocal(PointLatLngAlt.FromUTM(zone, x, utm1[1]));
                    var p2 = MainMap.FromLatLngToLocal(PointLatLngAlt.FromUTM(zone, x, utm2[1]));

                    int x1 = (int) p1.X;
                    int y1 = (int) p1.Y;
                    int x2 = (int) p2.X;
                    int y2 = (int) p2.Y;

                    e.Graphics.DrawLine(new Pen(MainMap.SelectionPen.Color, 1), x1, y1, x2, y2);
                }

                // y's
                for (double y = utm2[1]; y < utm1[1]; y += gridsize)
                {
                    var p1 = MainMap.FromLatLngToLocal(PointLatLngAlt.FromUTM(zone, utm1[0], y));
                    var p2 = MainMap.FromLatLngToLocal(PointLatLngAlt.FromUTM(zone, utm2[0], y));

                    int x1 = (int) p1.X;
                    int y1 = (int) p1.Y;
                    int x2 = (int) p2.X;
                    int y2 = (int) p2.Y;

                    e.Graphics.DrawLine(new Pen(MainMap.SelectionPen.Color, 1), x1, y1, x2, y2);
                }
            }

            e.Graphics.ResetTransform();

            polyicon.Location = new Point(10,100);
            polyicon.Paint(e.Graphics);

            e.Graphics.ResetTransform();
        }

        MissionPlanner.Controls.Icon.Polygon polyicon = new MissionPlanner.Controls.Icon.Polygon();

        private void chk_grid_CheckedChanged(object sender, EventArgs e)
        {
            grid = chk_grid.Checked;
        }

        private void insertWpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string wpno = (selectedrow + 1).ToString("0");
            if (InputBox.Show("Insert WP", "Insert WP after wp#", ref wpno) == DialogResult.OK)
            {
                try
                {
                    Commands.Rows.Insert(int.Parse(wpno), 1);
                }
                catch
                {
                    CustomMessageBox.Show("Invalid insert position", Strings.ERROR);
                    return;
                }

                selectedrow = int.Parse(wpno);

                ChangeColumnHeader(MAVLink.MAV_CMD.WAYPOINT.ToString());

                setfromMap(MouseDownStart.Lat, MouseDownStart.Lng, (int) float.Parse(TXT_DefaultAlt.Text));
            }
        }

        public void getRallyPointsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MainV2.comPort.MAV.param["RALLY_TOTAL"] == null)
            {
                CustomMessageBox.Show("Not Supported");
                return;
            }

            if (int.Parse(MainV2.comPort.MAV.param["RALLY_TOTAL"].ToString()) < 1)
            {
                CustomMessageBox.Show("Rally points - Nothing to download");
                return;
            }

            rallypointoverlay.Markers.Clear();

            int count = int.Parse(MainV2.comPort.MAV.param["RALLY_TOTAL"].ToString());

            for (int a = 0; a < (count); a++)
            {
                try
                {
                    PointLatLngAlt plla = MainV2.comPort.getRallyPoint(a, ref count);
                    rallypointoverlay.Markers.Add(new GMapMarkerRallyPt(new PointLatLng(plla.Lat, plla.Lng))
                    {
                        Alt = (int) plla.Alt,
                        ToolTipMode = MarkerTooltipMode.OnMouseOver,
                        ToolTipText = "Rally Point" + "\nAlt: " + (plla.Alt*CurrentState.multiplieralt)
                    });
                }
                catch
                {
                    CustomMessageBox.Show("Failed to get rally point", Strings.ERROR);
                    return;
                }
            }

            MainMap.UpdateMarkerLocalPosition(rallypointoverlay.Markers[0]);

            MainMap.Invalidate();
        }

        private void saveRallyPointsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            byte count = 0;

            MainV2.comPort.setParam("RALLY_TOTAL", rallypointoverlay.Markers.Count);

            foreach (GMapMarkerRallyPt pnt in rallypointoverlay.Markers)
            {
                try
                {
                    MainV2.comPort.setRallyPoint(count, new PointLatLngAlt(pnt.Position) {Alt = pnt.Alt}, 0, 0, 0,
                        (byte) (float) MainV2.comPort.MAV.param["RALLY_TOTAL"]);
                    count++;
                }
                catch
                {
                    CustomMessageBox.Show("Failed to save rally point", Strings.ERROR);
                    return;
                }
            }
        }

        private void setRallyPointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string altstring = TXT_DefaultAlt.Text;

            if (InputBox.Show("Altitude", "Altitude", ref altstring) == DialogResult.Cancel)
                return;

            int alt = 0;

            if (int.TryParse(altstring, out alt))
            {
                PointLatLngAlt rallypt = new PointLatLngAlt(MouseDownStart.Lat, MouseDownStart.Lng,
                    alt/CurrentState.multiplieralt, "Rally Point");
                rallypointoverlay.Markers.Add(
                    new GMapMarkerRallyPt(rallypt)
                    {
                        ToolTipMode = MarkerTooltipMode.OnMouseOver,
                        ToolTipText = "Rally Point" + "\nAlt: " + alt,
                        Tag = rallypointoverlay.Markers.Count,
                        Alt = (int) rallypt.Alt
                    }
                    );
            }
            else
            {
                CustomMessageBox.Show(Strings.InvalidAlt, Strings.ERROR);
            }

            isMouseClickOffMenu = false;
        }

        private void clearRallyPointsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                MainV2.comPort.setParam("RALLY_TOTAL", 0);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            rallypointoverlay.Markers.Clear();
            MainV2.comPort.MAV.rallypoints.Clear();
        }

        private void loadKMLFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog fd = new OpenFileDialog())
            {
                fd.Filter = "Google Earth KML |*.kml;*.kmz";
                DialogResult result = fd.ShowDialog();
                string file = fd.FileName;
                if (file != "")
                {
                    try
                    {
                        string kml = "";
                        string tempdir = "";
                        if (file.ToLower().EndsWith("kmz"))
                        {
                            ZipFile input = new ZipFile(file);

                            tempdir = Path.GetTempPath() + Path.DirectorySeparatorChar + Path.GetRandomFileName();
                            input.ExtractAll(tempdir, ExtractExistingFileAction.OverwriteSilently);

                            string[] kmls = Directory.GetFiles(tempdir, "*.kml");

                            if (kmls.Length > 0)
                            {
                                file = kmls[0];

                                input.Dispose();
                            }
                            else
                            {
                                input.Dispose();
                                return;
                            }
                        }

                        var sr = new StreamReader(File.OpenRead(file));
                        kml = sr.ReadToEnd();
                        sr.Close();

                        // cleanup after out
                        if (tempdir != "")
                            Directory.Delete(tempdir, true);

                        kml = kml.Replace("<Snippet/>", "");

                        var parser = new Parser();

                        parser.ElementAdded += processKMLMission;
                        parser.ParseString(kml, false);
                    }
                    catch (Exception ex)
                    {
                        CustomMessageBox.Show(Strings.Bad_KML_File + ex);
                    }
                }
            }
        }

        private void processKMLMission(object sender, ElementEventArgs e)
        {
            Element element = e.Element;
            try
            {
                //  log.Info(Element.ToString() + " " + Element.Parent);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            Document doc = element as Document;
            Placemark pm = element as Placemark;
            Folder folder = element as Folder;
            Polygon polygon = element as Polygon;
            LineString ls = element as LineString;

            if (doc != null)
            {
                foreach (var feat in doc.Features)
                {
                    //Console.WriteLine("feat " + feat.GetType());
                    //processKML((Element)feat);
                }
            }
            else if (folder != null)
            {
                foreach (Feature feat in folder.Features)
                {
                    //Console.WriteLine("feat "+feat.GetType());
                    //processKML(feat);
                }
            }
            else if (pm != null)
            {
                if (pm.Geometry is SharpKml.Dom.Point)
                {
                    var point = ((SharpKml.Dom.Point)pm.Geometry).Coordinate;
                    POI.POIAdd(new PointLatLngAlt(point.Latitude, point.Longitude), pm.Name);
                }
            }
            else if (polygon != null)
            {
            }
            else if (ls != null)
            {
                foreach (var loc in ls.Coordinates)
                {
                    selectedrow = Commands.Rows.Add();
                    setfromMap(loc.Latitude, loc.Longitude, (int) loc.Altitude);
                }
            }
        }

        private void lnk_kml_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start("http://127.0.0.1:56781/network.kml");
            }
            catch
            {
                CustomMessageBox.Show("Failed to open url http://127.0.0.1:56781/network.kml");
            }
        }

        private void modifyAltToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string altdif = "0";
            InputBox.Show("Alt Change", "Please enter the alitude change you require.\n(20 = up 20, *2 = up by alt * 2)",
                ref altdif);

            int altchange = 0;
            float multiplyer = 1;

            try
            {
                if (altdif.Contains("*"))
                {
                    multiplyer = float.Parse(altdif.Replace('*', ' '));
                }
                else
                {
                    altchange = int.Parse(altdif);
                }
            }
            catch
            {
                CustomMessageBox.Show(Strings.InvalidNumberEntered, Strings.ERROR);
                return;
            }


            foreach (DataGridViewRow line in Commands.Rows)
            {
                line.Cells[Alt.Index].Value =
                    (int) (float.Parse(line.Cells[Alt.Index].Value.ToString())*multiplyer + altchange);
            }
        }

        private void saveToFileToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (rallypointoverlay.Markers.Count == 0)
            {
                CustomMessageBox.Show("Please set some rally points");
                return;
            }
            /*
Column 1: Field type (RALLY is the only one at the moment -- may have RALLY_LAND in the future)
 Column 2,3: Lat, lon
 Column 4: Loiter altitude
 Column 5: Break altitude (when landing from rally is implemented, this is the altitude to break out of loiter from)
 Column 6: Landing heading (also for future when landing from rally is implemented)
 Column 7: Flags (just 0 for now, also future use).
             */

            using (SaveFileDialog sf = new SaveFileDialog())
            {
                sf.Filter = "Rally (*.ral)|*.ral";
                var result = sf.ShowDialog();
                if (sf.FileName != "" && result == DialogResult.OK)
                {
                    try
                    {
                        using (StreamWriter sw = new StreamWriter(sf.OpenFile()))
                        {
                            sw.WriteLine("#saved by Mission Planner " + Application.ProductVersion);


                            foreach (GMapMarkerRallyPt mark in rallypointoverlay.Markers)
                            {
                                sw.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}", "RALLY", mark.Position.Lat.ToString(CultureInfo.InvariantCulture),
                                    mark.Position.Lng.ToString(CultureInfo.InvariantCulture), mark.Alt.ToString(CultureInfo.InvariantCulture), 0, 0, 0);
                            }
                        }
                    }
                    catch
                    {
                        CustomMessageBox.Show("Failed to write rally file");
                    }
                }
            }
        }

        private void loadFromFileToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog fd = new OpenFileDialog())
            {
                fd.Filter = "Rally (*.ral)|*.ral";
                fd.ShowDialog();
                if (File.Exists(fd.FileName))
                {
                    StreamReader sr = new StreamReader(fd.OpenFile());

                    int a = 0;

                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        if (line.StartsWith("#"))
                        {
                        }
                        else
                        {
                            string[] items = line.Split(new[] {' ', '\t'}, StringSplitOptions.RemoveEmptyEntries);

                            MAVLink.mavlink_rally_point_t rally = new MAVLink.mavlink_rally_point_t();

                            rally.lat = (int) (float.Parse(items[1],CultureInfo.InvariantCulture)*1e7);
                            rally.lng = (int) (float.Parse(items[2], CultureInfo.InvariantCulture) *1e7);
                            rally.alt = (short) float.Parse(items[3], CultureInfo.InvariantCulture);
                            rally.break_alt = (short) float.Parse(items[4], CultureInfo.InvariantCulture);
                            rally.land_dir = (ushort) float.Parse(items[5], CultureInfo.InvariantCulture);
                            rally.flags = byte.Parse(items[6], CultureInfo.InvariantCulture);

                            if (a == 0)
                            {
                                rallypointoverlay.Markers.Clear();

                                rallypointoverlay.Markers.Add(new GMapMarkerRallyPt(rally));
                            }
                            else
                            {
                                rallypointoverlay.Markers.Add(new GMapMarkerRallyPt(rally));
                            }
                            a++;
                        }
                    }
                    MainMap.Invalidate();
                }
            }
        }

        private void prefetchWPPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FetchPath();
        }

        static string zone = "50s";

        private void enterUTMCoordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string easting = "578994";
            string northing = "6126244";

            if (InputBox.Show("Zone", "Enter Zone. (eg 50S, 11N)", ref zone) != DialogResult.OK)
                return;
            if (InputBox.Show("Easting", "Easting", ref easting) != DialogResult.OK)
                return;
            if (InputBox.Show("Northing", "Northing", ref northing) != DialogResult.OK)
                return;

            string newzone = zone.ToLower().Replace('s', ' ');
            newzone = newzone.ToLower().Replace('n', ' ');

            int zoneint = int.Parse(newzone);

            UTM utm = new UTM(zoneint, double.Parse(easting), double.Parse(northing),
                zone.ToLower().Contains("N") ? Geocentric.Hemisphere.North : Geocentric.Hemisphere.South);

            PointLatLngAlt ans = ((Geographic) utm);

            selectedrow = Commands.Rows.Add();

            ChangeColumnHeader(MAVLink.MAV_CMD.WAYPOINT.ToString());

            setfromMap(ans.Lat, ans.Lng, (int) ans.Alt);
        }

        private void loadSHPFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog fd = new OpenFileDialog())
            {
                fd.Filter = "Shape file|*.shp";
                DialogResult result = fd.ShowDialog();
                string file = fd.FileName;

                LoadSHPFile(file);
            }
        }

        private void LoadSHPFile(string file)
        {
            ProjectionInfo pStart = new ProjectionInfo();
            ProjectionInfo pESRIEnd = KnownCoordinateSystems.Geographic.World.WGS1984;
            bool reproject = false;

            if (File.Exists(file))
            {
                string prjfile = Path.GetDirectoryName(file) + Path.DirectorySeparatorChar +
                                 Path.GetFileNameWithoutExtension(file) + ".prj";
                if (File.Exists(prjfile))
                {
                    using (
                        StreamReader re =
                            File.OpenText(Path.GetDirectoryName(file) + Path.DirectorySeparatorChar +
                                          Path.GetFileNameWithoutExtension(file) + ".prj"))
                    {
                        pStart.ParseEsriString(re.ReadLine());

                        reproject = true;
                    }
                }

                IFeatureSet fs = FeatureSet.Open(file);

                fs.FillAttributes();

                int rows = fs.NumRows();

                DataTable dtOriginal = fs.DataTable;
                for (int row = 0; row < dtOriginal.Rows.Count; row++)
                {
                    object[] original = dtOriginal.Rows[row].ItemArray;
                }

                foreach (DataColumn col in dtOriginal.Columns)
                {
                    Console.WriteLine(col.ColumnName + " " + col.DataType);
                }

                quickadd = true;

                bool dosort = false;

                List<PointLatLngAlt> wplist = new List<PointLatLngAlt>();

                for (int row = 0; row < dtOriginal.Rows.Count; row++)
                {
                    double x = fs.Vertex[row*2];
                    double y = fs.Vertex[row*2 + 1];

                    double z = -1;
                    float wp = 0;

                    try
                    {
                        if (dtOriginal.Columns.Contains("ELEVATION"))
                            z = (float) Convert.ChangeType(dtOriginal.Rows[row]["ELEVATION"], TypeCode.Single);
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }

                    try
                    {
                        if (z == -1 && dtOriginal.Columns.Contains("alt"))
                            z = (float) Convert.ChangeType(dtOriginal.Rows[row]["alt"], TypeCode.Single);
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }

                    try
                    {
                        if (z == -1)
                            z = fs.Z[row];
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }


                    try
                    {
                        if (dtOriginal.Columns.Contains("wp"))
                        {
                            wp = (float) Convert.ChangeType(dtOriginal.Rows[row]["wp"], TypeCode.Single);
                            dosort = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }

                    if (reproject)
                    {
                        double[] xyarray = {x, y};
                        double[] zarray = {z};

                        Reproject.ReprojectPoints(xyarray, zarray, pStart, pESRIEnd, 0, 1);


                        x = xyarray[0];
                        y = xyarray[1];
                        z = zarray[0];
                    }

                    PointLatLngAlt pnt = new PointLatLngAlt(x, y, z, wp.ToString());

                    wplist.Add(pnt);
                }

                if (dosort)
                    wplist.Sort();

                foreach (var item in wplist)
                {
                    AddCommand(MAVLink.MAV_CMD.WAYPOINT, 0, 0, 0, 0, item.Lat, item.Lng, item.Alt);
                }

                quickadd = false;

                writeKML();

                MainMap.ZoomAndCenterMarkers("WPOverlay");
            }
        }

        private void BUT_saveWPFile_Click(object sender, EventArgs e)
        {
            SaveFile_Click(null, null);
        }

        private void switchDockingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (panelAction.Dock == DockStyle.Bottom)
            {
                panelAction.Dock = DockStyle.Right;
                panelWaypoints.Dock = DockStyle.Bottom;
            }
            else
            {
                panelAction.Dock = DockStyle.Bottom;
                panelAction.Height = 120;
                panelWaypoints.Dock = DockStyle.Right;
                panelWaypoints.Width = Width/2;
            }

            Settings.Instance["FP_docking"] = panelAction.Dock.ToString();
        }

        private void insertSplineWPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string wpno = (selectedrow + 1).ToString("0");
            if (InputBox.Show("Insert WP", "Insert WP after wp#", ref wpno) == DialogResult.OK)
            {
                try
                {
                    Commands.Rows.Insert(int.Parse(wpno), 1);
                }
                catch
                {
                    CustomMessageBox.Show(Strings.InvalidNumberEntered, Strings.ERROR);
                    return;
                }

                selectedrow = int.Parse(wpno);

                try
                {
                    Commands.Rows[selectedrow].Cells[Command.Index].Value = MAVLink.MAV_CMD.SPLINE_WAYPOINT.ToString();
                }
                catch
                {
                    CustomMessageBox.Show("SPLINE_WAYPOINT command not supported.");
                    Commands.Rows.RemoveAt(selectedrow);
                    return;
                }

                ChangeColumnHeader(MAVLink.MAV_CMD.SPLINE_WAYPOINT.ToString());

                setfromMap(MouseDownStart.Lat, MouseDownStart.Lng, (int) float.Parse(TXT_DefaultAlt.Text));
            }
        }

        private void CHK_splinedefault_CheckedChanged(object sender, EventArgs e)
        {
            splinemode = CHK_splinedefault.Checked;
        }

        private void createSplineCircleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string RadiusIn = "50";
            if (DialogResult.Cancel == InputBox.Show("Radius", "Radius", ref RadiusIn))
                return;

            string minaltin = "5";
            if (DialogResult.Cancel == InputBox.Show("min alt", "Min Alt", ref minaltin))
                return;

            string maxaltin = "20";
            if (DialogResult.Cancel == InputBox.Show("max alt", "Max Alt", ref maxaltin))
                return;

            string altstepin = "5";
            if (DialogResult.Cancel == InputBox.Show("alt step", "alt step", ref altstepin))
                return;


            string startanglein = "0";
            if (DialogResult.Cancel == InputBox.Show("angle", "Angle of first point (whole degrees)", ref startanglein))
                return;

            int Points = 4;
            int Radius = 0;
            int startangle = 0;
            int minalt = 5;
            int maxalt = 20;
            int altstep = 5;
            if (!int.TryParse(RadiusIn, out Radius))
            {
                CustomMessageBox.Show("Bad Radius");
                return;
            }

            if (!int.TryParse(minaltin, out minalt))
            {
                CustomMessageBox.Show("Bad min alt");
                return;
            }
            if (!int.TryParse(maxaltin, out maxalt))
            {
                CustomMessageBox.Show("Bad maxalt");
                return;
            }
            if (!int.TryParse(altstepin, out altstep))
            {
                CustomMessageBox.Show("Bad alt step");
                return;
            }

            double a = startangle;
            double step = 360.0f/Points;

            quickadd = true;

            AddCommand(MAVLink.MAV_CMD.DO_SET_ROI, 0, 0, 0, 0, MouseDownStart.Lng, MouseDownStart.Lat, 0);

            bool startup = true;

            for (int stepalt = minalt; stepalt <= maxalt;)
            {
                for (a = 0; a <= (startangle + 360) && a >= 0; a += step)
                {
                    selectedrow = Commands.Rows.Add();

                    Commands.Rows[selectedrow].Cells[Command.Index].Value = MAVLink.MAV_CMD.SPLINE_WAYPOINT.ToString();

                    ChangeColumnHeader(MAVLink.MAV_CMD.SPLINE_WAYPOINT.ToString());

                    float d = Radius;
                    float R = 6371000;

                    var lat2 = Math.Asin(Math.Sin(MouseDownEnd.Lat*MathHelper.deg2rad)*Math.Cos(d/R) +
                                         Math.Cos(MouseDownEnd.Lat*MathHelper.deg2rad)*Math.Sin(d/R)*Math.Cos(a*MathHelper.deg2rad));
                    var lon2 = MouseDownEnd.Lng*MathHelper.deg2rad +
                               Math.Atan2(Math.Sin(a*MathHelper.deg2rad)*Math.Sin(d/R)*Math.Cos(MouseDownEnd.Lat*MathHelper.deg2rad),
                                   Math.Cos(d/R) - Math.Sin(MouseDownEnd.Lat*MathHelper.deg2rad)*Math.Sin(lat2));

                    PointLatLng pll = new PointLatLng(lat2*MathHelper.rad2deg, lon2*MathHelper.rad2deg);

                    setfromMap(pll.Lat, pll.Lng, stepalt);

                    if (!startup)
                        stepalt += altstep/Points;
                }

                // reset back to the start
                if (startup)
                    stepalt = minalt;

                // we have finsihed the first run
                startup = false;
            }

            quickadd = false;
            writeKML();
        }

        private void CMB_altmode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CMB_altmode.SelectedValue == null)
            {
                CMB_altmode.SelectedIndex = 0;
            }
            else
            {
                currentaltmode = (altmode) CMB_altmode.SelectedValue;
            }
        }

        private void fromSHPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog fd = new OpenFileDialog())
            {
                fd.Filter = "Shape file|*.shp";
                DialogResult result = fd.ShowDialog();
                string file = fd.FileName;
                ProjectionInfo pStart = new ProjectionInfo();
                ProjectionInfo pESRIEnd = KnownCoordinateSystems.Geographic.World.WGS1984;
                bool reproject = false;
                // Poly Clear
                drawnpolygonsoverlay.Markers.Clear();
                drawnpolygonsoverlay.Polygons.Clear();
                drawnpolygon.Points.Clear();
                if (File.Exists(file))
                {
                    string prjfile = Path.GetDirectoryName(file) + Path.DirectorySeparatorChar +
                                     Path.GetFileNameWithoutExtension(file) + ".prj";
                    if (File.Exists(prjfile))
                    {
                        using (
                            StreamReader re =
                                File.OpenText(Path.GetDirectoryName(file) + Path.DirectorySeparatorChar +
                                              Path.GetFileNameWithoutExtension(file) + ".prj"))
                        {
                            pStart.ParseEsriString(re.ReadLine());
                            reproject = true;
                        }
                    }
                    try
                    {
                        IFeatureSet fs = FeatureSet.Open(file);
                        fs.FillAttributes();
                        int rows = fs.NumRows();
                        DataTable dtOriginal = fs.DataTable;
                        for (int row = 0; row < dtOriginal.Rows.Count; row++)
                        {
                            object[] original = dtOriginal.Rows[row].ItemArray;
                        }
                        string path = Path.GetDirectoryName(file);
                        foreach (var feature in fs.Features)
                        {
                            foreach (var point in feature.Coordinates)
                            {
                                if (reproject)
                                {
                                    double[] xyarray = {point.X, point.Y};
                                    double[] zarray = {point.Z};
                                    Reproject.ReprojectPoints(xyarray, zarray, pStart, pESRIEnd, 0, 1);
                                    point.X = xyarray[0];
                                    point.Y = xyarray[1];
                                    point.Z = zarray[0];
                                }
                                drawnpolygon.Points.Add(new PointLatLng(point.Y, point.X));
                                addpolygonmarkergrid(drawnpolygon.Points.Count.ToString(), point.X, point.Y, 0);
                            }
                            // remove loop close
                            if (drawnpolygon.Points.Count > 1 &&
                                drawnpolygon.Points[0] == drawnpolygon.Points[drawnpolygon.Points.Count - 1])
                            {
                                drawnpolygon.Points.RemoveAt(drawnpolygon.Points.Count - 1);
                            }
                            drawnpolygonsoverlay.Polygons.Add(drawnpolygon);
                            MainMap.UpdatePolygonLocalPosition(drawnpolygon);
                            MainMap.Invalidate();
                            MainMap.ZoomAndCenterMarkers(drawnpolygonsoverlay.Id);
                        }
                    }
                    catch (Exception ex)
                    {
                        CustomMessageBox.Show(Strings.ERROR + "\n" + ex, Strings.ERROR);
                    }
                }
            }
        }

        private void panelWaypoints_ExpandClick(object sender, EventArgs e)
        {
            Commands.AutoResizeColumns();
        }

        private void textToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string text = "";
            InputBox.Show("Enter String", "Enter String (requires 1CamBam_Stick_3 font)", ref text);
            string size = "5";
            InputBox.Show("Enter size", "Enter size", ref size);

            using (Font font = new System.Drawing.Font("1CamBam_Stick_3", float.Parse(size) * 1.35f, FontStyle.Regular))
            using (GraphicsPath gp = new GraphicsPath())
            using (StringFormat sf = new StringFormat())
            {
                sf.Alignment = StringAlignment.Near;
                sf.LineAlignment = StringAlignment.Near;
                gp.AddString(text, font.FontFamily, (int) font.Style, font.Size, new PointF(0, 0), sf);

                utmpos basepos = new utmpos(MouseDownStart);

                try
                {

                    foreach (var pathPoint in gp.PathPoints)
                    {
                        utmpos newpos = new utmpos(basepos);

                        newpos.x += pathPoint.X;
                        newpos.y += -pathPoint.Y;

                        var newlla = newpos.ToLLA();
                        quickadd = true;
                        AddWPToMap(newlla.Lat, newlla.Lng, int.Parse(TXT_DefaultAlt.Text));

                    }
                }
                catch (ArgumentException ex)
                {
                    CustomMessageBox.Show("Bad input options, please try again\n" + ex.ToString(), Strings.ERROR);
                }

                quickadd = false;
                writeKML();
            }
        }

        private void setHomeHereToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TXT_homealt.Text = (srtm.getAltitude(MouseDownStart.Lat, MouseDownStart.Lng).alt * CurrentState.multiplieralt).ToString("0");
            TXT_homelat.Text = MouseDownStart.Lat.ToString();
            TXT_homelng.Text = MouseDownStart.Lng.ToString();
        }

        private void coords1_SystemChanged(object sender, EventArgs e)
        {
            if (coords1.System == Coords.CoordsSystems.GEO.ToString())
            {
                Lat.Visible = true;
                Lon.Visible = true;

                coordZone.Visible = false;
                coordEasting.Visible = false;
                coordNorthing.Visible = false;
                MGRS.Visible = false;
            }
            else if (coords1.System == Coords.CoordsSystems.MGRS.ToString())
            {
                Lat.Visible = false;
                Lon.Visible = false;

                coordZone.Visible = false;
                coordEasting.Visible = false;
                coordNorthing.Visible = false;
                MGRS.Visible = true;
            }
            else if (coords1.System == Coords.CoordsSystems.UTM.ToString())
            {
                Lat.Visible = false;
                Lon.Visible = false;

                coordZone.Visible = true;
                coordEasting.Visible = true;
                coordNorthing.Visible = true;
                MGRS.Visible = false;
            }
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //FENCE_ENABLE ON COPTER
            //FENCE_ACTION ON PLANE

            try
            {
                MainV2.comPort.setParam("FENCE_ENABLE", 0);
            }
            catch
            {
                CustomMessageBox.Show("Failed to set FENCE_ENABLE");
                return;
            }

            try
            {
                MainV2.comPort.setParam("FENCE_ACTION", 0);
            }
            catch
            {
                CustomMessageBox.Show("Failed to set FENCE_ACTION");
                return;
            }

            try
            {
                MainV2.comPort.setParam("FENCE_TOTAL", 0);
            }
            catch
            {
                CustomMessageBox.Show("Failed to set FENCE_TOTAL");
                return;
            }

            // clear all
            drawnpolygonsoverlay.Polygons.Clear();
            drawnpolygonsoverlay.Markers.Clear();
            geofenceoverlay.Polygons.Clear();
            geofencepolygon.Points.Clear();
        }

        private void createCircleSurveyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Utilities.CircleSurveyMission.createGrid(MouseDownEnd);
        }

        private void currentPositionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddWPToMap(MainV2.comPort.MAV.cs.lat, MainV2.comPort.MAV.cs.lng, (int) MainV2.comPort.MAV.cs.alt);
        }

        private void surveyGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GridPlugin grid = new GridPlugin();
            grid.Host = new PluginHost();
            grid.but_Click(sender, e);
        }
        
        private void Commands_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            writeKML();
        }

        private void but_writewpfast_Click(object sender, EventArgs e)
        {
            if ((altmode)CMB_altmode.SelectedValue == altmode.Absolute)
            {
                if ((int)DialogResult.No ==
                    CustomMessageBox.Show("Absolute Alt is selected are you sure?", "Alt Mode", MessageBoxButtons.YesNo))
                {
                    CMB_altmode.SelectedValue = (int)altmode.Relative;
                }
            }

            // check home
            Locationwp home = new Locationwp();
            try
            {
                home.id = (ushort)MAVLink.MAV_CMD.WAYPOINT;
                home.lat = (double.Parse(TXT_homelat.Text));
                home.lng = (double.Parse(TXT_homelng.Text));
                home.alt = (float.Parse(TXT_homealt.Text) / CurrentState.multiplierdist); // use saved home
            }
            catch
            {
                CustomMessageBox.Show("Your home location is invalid", Strings.ERROR);
                return;
            }

            // check for invalid grid data
            for (int a = 0; a < Commands.Rows.Count - 0; a++)
            {
                for (int b = 0; b < Commands.ColumnCount - 0; b++)
                {
                    double answer;
                    if (b >= 1 && b <= 7)
                    {
                        if (!double.TryParse(Commands[b, a].Value.ToString(), out answer))
                        {
                            CustomMessageBox.Show("There are errors in your mission");
                            return;
                        }
                    }

                    if (TXT_altwarn.Text == "")
                        TXT_altwarn.Text = (0).ToString();

                    if (Commands.Rows[a].Cells[Command.Index].Value.ToString().Contains("UNKNOWN"))
                        continue;

                    ushort cmd =
                        (ushort)
                                Enum.Parse(typeof(MAVLink.MAV_CMD),
                                    Commands.Rows[a].Cells[Command.Index].Value.ToString(), false);

                    if (cmd < (ushort)MAVLink.MAV_CMD.LAST &&
                        double.Parse(Commands[Alt.Index, a].Value.ToString()) < double.Parse(TXT_altwarn.Text))
                    {
                        if (cmd != (ushort)MAVLink.MAV_CMD.TAKEOFF &&
                            cmd != (ushort)MAVLink.MAV_CMD.LAND &&
                            cmd != (ushort)MAVLink.MAV_CMD.RETURN_TO_LAUNCH)
                        {
                            CustomMessageBox.Show("Low alt on WP#" + (a + 1) +
                                                  "\nPlease reduce the alt warning, or increase the altitude");
                            return;
                        }
                    }
                }
            }

            IProgressReporterDialogue frmProgressReporter = new ProgressReporterDialogue
            {
                StartPosition = FormStartPosition.CenterScreen,
                Text = "Sending WP's"
            };

            frmProgressReporter.DoWork += saveWPsFast;

            frmProgressReporter.UpdateProgressAndStatus(-1, "Sending WP's");

            ThemeManager.ApplyThemeTo(frmProgressReporter);

            frmProgressReporter.RunBackgroundOperationAsync();

            frmProgressReporter.Dispose();

            MainMap.Focus();
        }
    }
}