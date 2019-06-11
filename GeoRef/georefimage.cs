using System;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Globalization;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using MissionPlanner.Utilities;

namespace MissionPlanner.GeoRef
{
    partial class Georefimage : Form
    {
        GeoRefImageBase georef = new GeoRefImageBase();

        private CheckBox chk_cammsg;
        private TextBox txt_basealt;
        private Label label28;

        public Georefimage()
        {
            InitializeComponent();

            CHECK_AMSLAlt_Use.Checked = true;
            PANEL_TIME_OFFSET.Enabled = false;

            georef.useAMSLAlt = CHECK_AMSLAlt_Use.Checked;

            selectedProcessingMode = PROCESSING_MODE.CAM_MSG;

            MissionPlanner.Utilities.Tracking.AddPage(this.GetType().ToString(), this.Text);

            myGMAP1.MapProvider = MainV2.instance.FlightData.gMapControl1.MapProvider;
            myGMAP1.MinZoom = MainV2.instance.FlightData.gMapControl1.MinZoom;
            myGMAP1.MaxZoom = MainV2.instance.FlightData.gMapControl1.MaxZoom;

            GMapOverlay overlay = new GMapOverlay();
            myGMAP1.Overlays.Add(overlay);

            myGMAP1.OnMarkerClick += MyGMAP1_OnMarkerClick;
        }

        private void MyGMAP1_OnMarkerClick(GMapMarker item, object ei)
        {
            var e = ei as MouseEventArgs;
            foreach (var pictureInformation in georef.picturesInfo)
            {
                if (item.ToolTipText == Path.GetFileName(pictureInformation.Value.Path))
                {
                    //pictureBox1.Image = new Bitmap(pictureInformation.Value.Path);
                    pictureBox1.ImageLocation = pictureInformation.Value.Path;
                    pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                    return;
                }
            }
        }

        public string UseGpsorGPS2()
        {
            if (chk_usegps2.Checked)
                return "GPS2";

            return "GPS";
        }


        private void AppendText(string text)
        {
            var inv = new MethodInvoker(delegate {
                TXT_outputlog.AppendText(text);
                TXT_outputlog.Refresh();
            });

            this.Invoke(inv);
        }

        private void BUT_browselog_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Logs|*.log;*.tlog;*.bin;*.BIN";
            openFileDialog1.ShowDialog();

            if (File.Exists(openFileDialog1.FileName))
            {
                TXT_logfile.Text = openFileDialog1.FileName;
                TXT_jpgdir.Text = Path.GetDirectoryName(TXT_logfile.Text);
            }
        }

        private void BUT_browsedir_Click(object sender, EventArgs e)
        {
            try
            {
                folderBrowserDialog1.SelectedPath = Path.GetDirectoryName(TXT_logfile.Text);
            }
            catch
            {
            }

            folderBrowserDialog1.ShowDialog();

            if (folderBrowserDialog1.SelectedPath != "")
            {
                TXT_jpgdir.Text = folderBrowserDialog1.SelectedPath;

                string file = folderBrowserDialog1.SelectedPath + Path.DirectorySeparatorChar + "location.txt";

                if (File.Exists(file))
                {
                    try
                    {
                        using (StreamReader sr = new StreamReader(file))
                        {
                            string cotent = sr.ReadToEnd();

                            Match match = Regex.Match(cotent, "seconds_offset: ([0-9]+)");

                            if (match.Success)
                            {
                                TXT_offsetseconds.Text = match.Groups[1].Value;
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }

        private void BUT_doit_Click(object sender, EventArgs e)
        {
            string dirPictures = TXT_jpgdir.Text;
            string logFilePath = TXT_logfile.Text;

            if (!File.Exists(logFilePath))
                return;
            if (!Directory.Exists(dirPictures))
                return;

            float seconds = 0;
            if (selectedProcessingMode == PROCESSING_MODE.TIME_OFFSET)
            {
                if (
                    float.TryParse(TXT_offsetseconds.Text, NumberStyles.Float, CultureInfo.InvariantCulture,
                        out seconds) ==
                    false)
                {
                    AppendText("Offset number not in correct format. Use . as decimal separator\n");
                    return;
                }
            }

            BUT_doit.Enabled = false;
            TXT_outputlog.Clear();

            try
            {
                switch (selectedProcessingMode)
                {
                    case PROCESSING_MODE.TIME_OFFSET:
                        georef.picturesInfo = georef.doworkGPSOFFSET(logFilePath, dirPictures, seconds, UseGpsorGPS2(),
                            chk_cammsg.Checked, AppendText);
                        if (georef.picturesInfo != null)
                            georef.CreateReportFiles(georef.picturesInfo, dirPictures, seconds,
                                (double) num_camerarotation.Value, (double) num_hfov.Value, (double) num_vfov.Value,
                                AppendText, httpGeoRefKML);
                        break;
                    case PROCESSING_MODE.CAM_MSG:
                        georef.picturesInfo = georef.doworkCAM(logFilePath, dirPictures, UseGpsorGPS2(), AppendText);
                        if (georef.picturesInfo != null)
                            georef.CreateReportFiles(georef.picturesInfo, dirPictures, seconds,
                                (double) num_camerarotation.Value, (double) num_hfov.Value, (double) num_vfov.Value,
                                AppendText, httpGeoRefKML, chk_camusegpsalt.Checked);
                        break;
                    case PROCESSING_MODE.TRIG:
                        georef.picturesInfo = georef.doworkTRIG(logFilePath, dirPictures, UseGpsorGPS2(), AppendText);
                        if (georef.picturesInfo != null)
                            georef.CreateReportFiles(georef.picturesInfo, dirPictures, seconds,
                                (double) num_camerarotation.Value, (double) num_hfov.Value, (double) num_vfov.Value,
                                AppendText, httpGeoRefKML, chk_trigusergpsalt.Checked);
                        break;
                }
            }
            catch (Exception ex)
            {
                AppendText("Error " + ex.ToString());
            }


            GMapRoute route = new GMapRoute("vehicle");
            if (georef.vehicleLocations != null)
            {
                foreach (var vehicleLocation in georef.vehicleLocations)
                {
                    route.Points.Add(new PointLatLngAlt(vehicleLocation.Value.Lat, vehicleLocation.Value.Lon,
                        vehicleLocation.Value.AltAMSL));
                }
            }

            myGMAP1.Overlays[0].Markers.Clear();
            if (georef.picturesInfo != null)
            {
                foreach (var pictureLocation in georef.picturesInfo)
                {
                    myGMAP1.Overlays[0].Markers.Add(
                        new GMarkerGoogle(new PointLatLngAlt(pictureLocation.Value.Lat, pictureLocation.Value.Lon,
                            pictureLocation.Value.AltAMSL), GMarkerGoogleType.green)
                        {
                            IsHitTestVisible = true,
                            ToolTipMode = MarkerTooltipMode.OnMouseOver,
                            ToolTipText = Path.GetFileName(pictureLocation.Value.Path)
                        });
                }
            }

            myGMAP1.Overlays[0].Routes.Clear();
            myGMAP1.Overlays[0].Routes.Add(route);

            myGMAP1.ZoomAndCenterRoutes(myGMAP1.Overlays[0].Id);

            BUT_doit.Enabled = true;
            BUT_Geotagimages.Enabled = true;
        }

        private void httpGeoRefKML(string obj)
        {
            httpserver.georefimagepath = TXT_jpgdir.Text + Path.DirectorySeparatorChar;
            httpserver.georefkml = obj;
        }

        private void BUT_estoffset_Click(object sender, EventArgs e)
        {
            TXT_outputlog.Clear();

            double offset = georef.EstimateOffset(TXT_logfile.Text, TXT_jpgdir.Text, UseGpsorGPS2(), chk_cammsg.Checked,
                AppendText);

            AppendText("Offset around :  " + offset.ToString(CultureInfo.InvariantCulture) + "\n\n");
        }

        private void BUT_Geotagimages_Click(object sender, EventArgs e)
        {
            // Save file into Geotag folder
            string rootFolder = TXT_jpgdir.Text;
            string geoTagFolder = rootFolder + Path.DirectorySeparatorChar + "geotagged";

            bool isExists = System.IO.Directory.Exists(geoTagFolder);

            // delete old files and folder
            if (isExists)
                Directory.Delete(geoTagFolder, true);

            // create it again
            System.IO.Directory.CreateDirectory(geoTagFolder);

            if (georef.picturesInfo == null)
            {
                AppendText("no valid matchs");
                return;
            }

            foreach (PictureInformation picInfo in georef.picturesInfo.Values)
            {
                // use cam gpsalt
                if (chk_camusegpsalt.Checked && RDIO_CAMMsgSynchro.Checked)
                {
                    georef.WriteCoordinatesToImage(picInfo.Path, picInfo.Lat, picInfo.Lon, picInfo.GPSAlt,
                        TXT_jpgdir.Text, AppendText);
                }
                // use trig gpsalt
                else if (chk_trigusergpsalt.Checked && RDIO_trigmsg.Checked)
                {
                    georef.WriteCoordinatesToImage(picInfo.Path, picInfo.Lat, picInfo.Lon, picInfo.GPSAlt,
                        TXT_jpgdir.Text, AppendText);
                }
                // use altamsl
                else if (georef.useAMSLAlt)
                {
                    georef.WriteCoordinatesToImage(picInfo.Path, picInfo.Lat, picInfo.Lon,
                        double.Parse(txt_basealt.Text) + picInfo.AltAMSL, TXT_jpgdir.Text, AppendText);
                }
                // use relalt
                else
                {
                    georef.WriteCoordinatesToImage(picInfo.Path, picInfo.Lat, picInfo.Lon, picInfo.RelAlt,
                        TXT_jpgdir.Text,
                        AppendText);
                }
            }

            AppendText("GeoTagging FINISHED \n\n");
        }

        private void BUT_networklinkgeoref_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Settings.GetRunningDirectory() + "m3u" + Path.DirectorySeparatorChar +
                                             "GeoRefnetworklink.kml");
        }

        private void TXT_logfile_TextChanged(object sender, EventArgs e)
        {
            if (georef.vehicleLocations != null)
                georef.vehicleLocations.Clear();
            if (georef.picturesInfo != null)
                georef.picturesInfo.Clear();

            BUT_Geotagimages.Enabled = false;
        }

        private void ProcessType_CheckedChanged(object sender, EventArgs e)
        {
            if (RDIO_CAMMsgSynchro.Checked)
            {
                selectedProcessingMode = PROCESSING_MODE.CAM_MSG;
                PANEL_TIME_OFFSET.Enabled = false;
                PANEL_SHUTTER_LAG.Enabled = true;
            }
            else if (RDIO_trigmsg.Checked)
            {
                selectedProcessingMode = PROCESSING_MODE.TRIG;
                PANEL_TIME_OFFSET.Enabled = false;
                PANEL_SHUTTER_LAG.Enabled = false;
            }
            else
            {
                selectedProcessingMode = PROCESSING_MODE.TIME_OFFSET;
                PANEL_TIME_OFFSET.Enabled = true;
                PANEL_SHUTTER_LAG.Enabled = false;
            }
        }


        private void TXT_shutterLag_TextChanged(object sender, EventArgs e)
        {
            bool convertedOK = int.TryParse(TXT_shutterLag.Text, NumberStyles.Integer, CultureInfo.InvariantCulture,
                out georef.millisShutterLag);

            if (!convertedOK)
                TXT_shutterLag.Text = "0";
        }

        private void CHECK_AMSLAlt_Use_CheckedChanged(object sender, EventArgs e)
        {
            georef.useAMSLAlt = ((CheckBox) sender).Checked;

            txt_basealt.Enabled = !georef.useAMSLAlt;
        }

        private void chk_cammsg_CheckedChanged(object sender, EventArgs e)
        {
            if (georef.vehicleLocations != null)
                georef.vehicleLocations.Clear();
        }

        private void chk_usegps2_CheckedChanged(object sender, EventArgs e)
        {
            if (georef.vehicleLocations != null)
                georef.vehicleLocations.Clear();
        }
    }
}