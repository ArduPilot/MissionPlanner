using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using DotSpatial.Data;
using DotSpatial.Projections;
using DotSpatial.Symbology;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using log4net;
using Microsoft.Scripting.Utils;
using MissionPlanner.Comms;
using MissionPlanner.Controls;
using MissionPlanner.GCSViews;
using MissionPlanner.GeoRef;
using MissionPlanner.Log;
using MissionPlanner.Maps;
using MissionPlanner.Swarm;
using MissionPlanner.Utilities;
using MissionPlanner.Warnings;
using resedit;
using static MissionPlanner.Utilities.Firmware;
using ILog = log4net.ILog;

namespace MissionPlanner
{
    public partial class temp : Form
    {
        private static readonly ILog log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static MAVLinkSerialPort comport;

        private static TcpListener listener;

        private static TcpClient client;

        public temp()
        {
            InitializeComponent();

            Tracking.AddPage(
                MethodBase.GetCurrentMethod().DeclaringType.ToString(),
                MethodBase.GetCurrentMethod().Name);
        }

        //private static Factory factory; 
        //private static VideoPlayer player; 
        //private static Renderer renderer; 
        //private Media media; 


        private void temp_Load(object sender, EventArgs e)
        {
        }

        private void BUT_geinjection_Click(object sender, EventArgs e)
        {
            var MainMap = new GMapControl();

            MainMap.MapProvider = GoogleSatelliteMapProvider.Instance;

            MainMap.CacheLocation = Path.GetDirectoryName(Application.ExecutablePath) + "/gmapcache/";

            var fbd = new FolderBrowserDialog();

            try
            {
                fbd.SelectedPath = @"C:\Users\hog\Documents\albany 2011\New folder";
            }
            catch
            {
            }

            if (fbd.ShowDialog() != DialogResult.OK)
                return;

            if (fbd.SelectedPath != "")
            {
                var files = Directory.GetFiles(fbd.SelectedPath, "*.jpg", SearchOption.AllDirectories);
                var files1 = Directory.GetFiles(fbd.SelectedPath, "*.png", SearchOption.AllDirectories);

                var origlength = files.Length;
                Array.Resize(ref files, origlength + files1.Length);
                Array.Copy(files1, 0, files, origlength, files1.Length);

                foreach (var file in files)
                {
                    log.Info(DateTime.Now.Millisecond + " Doing " + file);
                    var reg = new Regex(@"Z([0-9]+)\\([0-9]+)\\([0-9]+)");

                    var mat = reg.Match(file);

                    if (mat.Success == false)
                        continue;

                    var temp = 1 << int.Parse(mat.Groups[1].Value);

                    var pnt = new GPoint(int.Parse(mat.Groups[3].Value), int.Parse(mat.Groups[2].Value));

                    BUT_geinjection.Text = file;
                    BUT_geinjection.Refresh();

                    //MainMap.Projection.

                    var tile = new MemoryStream();

                    var Img = Image.FromFile(file);
                    Img.Save(tile, ImageFormat.Jpeg);

                    tile.Seek(0, SeekOrigin.Begin);
                    log.Info(pnt.X + " " + pnt.Y);

                    Application.DoEvents();

                    GMaps.Instance.PrimaryCache.PutImageToCache(tile.ToArray(), Custom.Instance.DbId, pnt,
                        int.Parse(mat.Groups[1].Value));

                    // Application.DoEvents();
                }
            }

            MainMap.Dispose();
        }

        private void BUT_clearcustommaps_Click(object sender, EventArgs e)
        {
            var MainMap = new GMapControl();
            MainMap.MapProvider = GoogleSatelliteMapProvider.Instance;

            var removed = MainMap.Manager.PrimaryCache.DeleteOlderThan(DateTime.Now, Custom.Instance.DbId);

            CustomMessageBox.Show("Removed " + removed + " images");

            log.InfoFormat("Removed {0} images", removed);

            MainMap.Dispose();
        }

        private void BUT_lang_edit_Click(object sender, EventArgs e)
        {
            new Form1().Show();
        }

        private void BUT_georefimage_Click(object sender, EventArgs e)
        {
            new Georefimage().Show();
        }

        private void BUT_follow_me_Click(object sender, EventArgs e)
        {
            var si = new FollowMe();
            ThemeManager.ApplyThemeTo(si);
            si.Show();
        }

        private void BUT_paramgen_Click(object sender, EventArgs e)
        {
            if(MissionPlanner.Utilities.Update.dobeta)
                ParameterMetaDataParser.GetParameterInformation(ConfigurationManager.AppSettings["ParameterLocationsBleeding"]);
            else
                ParameterMetaDataParser.GetParameterInformation(ConfigurationManager.AppSettings["ParameterLocations"]);

            ParameterMetaDataRepositoryAPM.Reload();
        }

        private void myButton1_Click(object sender, EventArgs e)
        {
            new SerialOutputMD().Show();
        }

        private void but_osdvideo_Click(object sender, EventArgs e)
        {
            new OSDVideo().Show();
        }

        private void BUT_swarm_Click(object sender, EventArgs e)
        {
            new FormationControl().Show();
        }

        private void BUT_outputMavlink_Click(object sender, EventArgs e)
        {
            new SerialOutputPass().Show();
        }

        private void BUT_outputnmea_Click(object sender, EventArgs e)
        {
            new SerialOutputNMEA().Show();
        }

        private void BUT_followleader_Click(object sender, EventArgs e)
        {
            new FollowPathControl().Show();
        }

        private void BUT_sorttlogs_Click(object sender, EventArgs e)
        {
            var fbd = new FolderBrowserDialog();
            fbd.SelectedPath = Settings.Instance.LogDir;

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    LogSort.SortLogs(Directory.GetFiles(fbd.SelectedPath, "*.tlog"));
                }
                catch
                {
                }
            }
        }

        private void BUT_movingbase_Click(object sender, EventArgs e)
        {
            var si = new MovingBase();
            ThemeManager.ApplyThemeTo(si);
            si.Show();
        }

        private void but_getfw_Click(object sender, EventArgs e)
        {
            var basedir = Settings.GetDataDirectory() + "History";

            Directory.CreateDirectory(basedir);

            var fw = new Firmware();

            var list = fw.getFWList();

            var options = new optionsObject();
            options.softwares = list;

            var members = typeof(software).GetFields();

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.IndentChars = "    ";
            settings.Indent = true;
            settings.Encoding = Encoding.ASCII;

            using (var xmlwriter = XmlWriter.Create(basedir + Path.DirectorySeparatorChar + @"firmware2.xml", settings))
            {
                for (int a=0; a < options.softwares.Count; a++)
                {
                    Loading.ShowLoading(((a - 1) / (float)list.Count) * 100.0 + "% " + options.softwares[a].name, this);

                    List<Task<bool>> tasklist = new List<Task<bool>>();

                    foreach (var field in members)
                    {
                        if (field.Name.ToLower().Contains("url"))
                        {
                            var url = field.GetValue(options.softwares[a]).ToString();

                            if (String.IsNullOrEmpty(url))
                                continue;

                            field.SetValue(options.softwares[a], new Uri(url).LocalPath.TrimStart('/', '\\'));

                            var task = Download.getFilefromNetAsync(url, basedir + new Uri(url).LocalPath);
                            tasklist.Add(task);
                        }
                    }

                    //Task.WaitAll(tasklist.ToArray());
                }

                XmlSerializer xms = new XmlSerializer(typeof(optionsObject), new Type[] { typeof(software) });

                xms.Serialize(xmlwriter, options);
            }
            Loading.Close();
        }


        private void button3_Click(object sender, EventArgs e)
        {
            var frm = new WarningsManager();

            frm.Show();
        }

        private void but_mavserialport_Click(object sender, EventArgs e)
        {
            if (comport != null)
            {
                comport.Close();
                if (client != null && client.Connected)
                {
                    client.Close();
                }

                if (listener != null)
                {
                    listener.Stop();
                }
                return;
            }

            try
            {
                comport = new MAVLinkSerialPort(MainV2.comPort, MAVLink.SERIAL_CONTROL_DEV.GPS1);

                if (listener != null)
                {
                    listener.Stop();
                    listener = null;
                }

                listener = new TcpListener(IPAddress.Any, 500);

                listener.Start();

                listener.BeginAcceptTcpClient(DoAcceptTcpClientCallback, listener);
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(ex.ToString(), Strings.ERROR);
            }
        }

        private void DoAcceptTcpClientCallback(IAsyncResult ar)
        {
            try
            {
                // Get the listener that handles the client request.
                var listener = (TcpListener)ar.AsyncState;

                listener.BeginAcceptTcpClient(DoAcceptTcpClientCallback, listener);

                if (client != null && client.Connected)
                    return;

                // End the operation and display the received data on  
                // the console.
                using (
                    client = listener.EndAcceptTcpClient(ar))
                {
                    var stream = client.GetStream();

                    if (!comport.IsOpen)
                        comport.Open();

                    while (client.Connected && comport.IsOpen)
                    {
                        while (stream.DataAvailable)
                        {
                            var data = new byte[4096];
                            try
                            {
                                var len = stream.Read(data, 0, data.Length);

                                comport.Write(data, 0, len);
                            }
                            catch
                            {
                            }
                        }

                        while (comport.BytesToRead > 0)
                        {
                            var data = new byte[4096];
                            try
                            {
                                var len = comport.Read(data, 0, data.Length);

                                stream.Write(data, 0, len);
                            }
                            catch
                            {
                            }
                        }

                        Thread.Sleep(1);
                    }
                }
            }
            catch
            {
            }
        }

        private void BUT_magfit2_Click(object sender, EventArgs e)
        {
            MagCalib.ProcessLog(0);
        }

        private void BUT_shptopoly_Click(object sender, EventArgs e)
        {
            using (var fd = new OpenFileDialog())
            {
                fd.Filter = "Shape file|*.shp";
                var result = fd.ShowDialog();
                var file = fd.FileName;

                var pStart = new ProjectionInfo();
                var pESRIEnd = KnownCoordinateSystems.Geographic.World.WGS1984;
                var reproject = false;

                if (File.Exists(file))
                {
                    var prjfile = Path.GetDirectoryName(file) + Path.DirectorySeparatorChar +
                                  Path.GetFileNameWithoutExtension(file) + ".prj";
                    if (File.Exists(prjfile))
                    {
                        using (
                            var re =
                                File.OpenText(Path.GetDirectoryName(file) + Path.DirectorySeparatorChar +
                                              Path.GetFileNameWithoutExtension(file) + ".prj"))
                        {
                            pStart.ParseEsriString(re.ReadLine());

                            reproject = true;
                        }
                    }

                    var fs = FeatureSet.Open(file);

                    fs.FillAttributes();

                    var rows = fs.NumRows();

                    var dtOriginal = fs.DataTable;
                    for (var row = 0; row < dtOriginal.Rows.Count; row++)
                    {
                        var original = dtOriginal.Rows[row].ItemArray;
                    }

                    foreach (DataColumn col in dtOriginal.Columns)
                    {
                        Console.WriteLine(col.ColumnName + " " + col.DataType);
                    }

                    var a = 1;

                    var path = Path.GetDirectoryName(file);

                    foreach (var feature in fs.Features)
                    {
                        var sb = new StringBuilder();

                        sb.Append("#Shap to Poly - Mission Planner\r\n");
                        foreach (var point in feature.Coordinates)
                        {
                            if (reproject)
                            {
                                double[] xyarray = { point.X, point.Y };
                                double[] zarray = { point.Z };

                                Reproject.ReprojectPoints(xyarray, zarray, pStart, pESRIEnd, 0, 1);

                                point.X = xyarray[0];
                                point.Y = xyarray[1];
                                point.Z = zarray[0];
                            }

                            sb.Append(point.Y.ToString(CultureInfo.InvariantCulture) + "\t" +
                                      point.X.ToString(CultureInfo.InvariantCulture) + "\r\n");
                        }

                        log.Info("writting poly to " + path + Path.DirectorySeparatorChar + "poly-" + a + ".poly");
                        File.WriteAllText(path + Path.DirectorySeparatorChar + "poly-" + a + ".poly", sb.ToString());

                        a++;
                    }
                }
            }
        }

        private void but_gimbaltest_Click(object sender, EventArgs e)
        {
            if (MainV2.comPort.BaseStream.IsOpen)
                GimbalPoint.ProjectPoint();
            else
                CustomMessageBox.Show(Strings.PleaseConnect, Strings.ERROR);
        }

        private void but_maplogs_Click(object sender, EventArgs e)
        {
            var fbd = new FolderBrowserDialog();
            fbd.SelectedPath = Settings.Instance.LogDir;

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                LogMap.MapLogs(Directory.GetFiles(fbd.SelectedPath, "*.tlog", SearchOption.AllDirectories));
                LogMap.MapLogs(Directory.GetFiles(fbd.SelectedPath, "*.bin", SearchOption.AllDirectories));
                LogMap.MapLogs(Directory.GetFiles(fbd.SelectedPath, "*.log", SearchOption.AllDirectories));
            }
        }

        private void butlogindex_Click(object sender, EventArgs e)
        {
            var form = new LogIndex();

            form.Show();
        }

        private void but_structtest_Click(object sender, EventArgs e)
        {
            var array = new byte[100];

            for (var l = 0; l < array.Length; l++)
            {
                array[l] = (byte)l;
            }

            var a = 0;
            var start = DateTime.MinValue;
            var end = DateTime.MinValue;


            start = DateTime.Now;
            for (a = 0; a < 1000000; a++)
            {
                var obj = (object)new MAVLink.mavlink_heartbeat_t();
                MavlinkUtil.ByteArrayToStructure(array, ref obj, 6);
            }
            end = DateTime.Now;
            Console.WriteLine("ByteArrayToStructure " + (end - start).TotalMilliseconds);
            start = DateTime.Now;
            for (a = 0; a < 1000000; a++)
            {
                var ans1 = MavlinkUtil.ByteArrayToStructureT<MAVLink.mavlink_heartbeat_t>(array, 6);
            }
            end = DateTime.Now;
            Console.WriteLine("ByteArrayToStructureT<> " + (end - start).TotalMilliseconds);
            start = DateTime.Now;
            for (a = 0; a < 1000000; a++)
            {
                var ans2 = MavlinkUtil.ReadUsingPointer<MAVLink.mavlink_heartbeat_t>(array, 6);
            }
            end = DateTime.Now;
            Console.WriteLine("ReadUsingPointer " + (end - start).TotalMilliseconds);
            start = DateTime.Now;

            for (a = 0; a < 1000000; a++)
            {
                var ans3 = MavlinkUtil.ByteArrayToStructureGC<MAVLink.mavlink_heartbeat_t>(array, 6);
            }
            end = DateTime.Now;
            Console.WriteLine("ByteArrayToStructureGC " + (end - start).TotalMilliseconds);
        }

        private void but_armandtakeoff_Click(object sender, EventArgs e)
        {
            try
            {

                MainV2.comPort.setMode("Stabilize");

                if (MainV2.comPort.doARM(true))
                {
                    MainV2.comPort.setMode("GUIDED");

                    Thread.Sleep(300);

                    MainV2.comPort.doCommand(MAVLink.MAV_CMD.TAKEOFF, 0, 0, 0, 0, 0, 0, 10);
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(ex.ToString());
            }
        }

        private void but_sitl_comb_Click(object sender, EventArgs e)
        {
            StreamCombiner.Start();
        }

        private void but_injectgps_Click(object sender, EventArgs e)
        {
            new SerialInjectGPS().Show();
        }

        private void BUT_fft_Click(object sender, EventArgs e)
        {
            var fft = new fftui();

            fft.Show();
        }


        private void but_reboot_Click(object sender, EventArgs e)
        {
            if (CustomMessageBox.Show("Are you sure?","",MessageBoxButtons.YesNo) == (int)DialogResult.Yes)
                MainV2.comPort.doReboot(false, true);
        }

        private void BUT_QNH_Click(object sender, EventArgs e)
        {
            var currentQNH = MainV2.comPort.GetParam("GND_ABS_PRESS").ToString();

            if (InputBox.Show("QNH", "Enter the QNH in pascals (103040 = 1030.4 hPa)", ref currentQNH) ==
                DialogResult.OK)
            {
                var newQNH = double.Parse(currentQNH);

                MainV2.comPort.setParam("GND_ABS_PRESS", newQNH);
            }
        }

        private void but_trimble_Click(object sender, EventArgs e)
        {
            new Swarm.Sequence.LayoutEditor().Show();
        }

        private void myButton_vlc_Click(object sender, EventArgs e)
        {
            var render = new vlcrender();

            var url = render.playurl;
            if (InputBox.Show("enter url", "enter url", ref url) == DialogResult.OK)
            {
                render.playurl = url;
                try
                {
                    render.Start();
                }
                catch (Exception ex)
                {
                    CustomMessageBox.Show(ex.ToString(), Strings.ERROR);
                }
            }
        }

        private void but_gstream_Click(object sender, EventArgs e)
        {
            try
            {
                GStreamer.gstlaunch = GStreamer.LookForGstreamer();
                if (File.Exists(GStreamer.gstlaunch))
                {
                    GStreamer.Start();
                }
                else
                {
                    UDPVideoShim.DownloadGStreamer();
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(ex.ToString(), Strings.ERROR);
            }
        }

        private void but_agemapdata_Click(object sender, EventArgs e)
        {
            var removed = ((PureImageCache)MyImageCache.Instance).DeleteOlderThan(DateTime.Now.AddDays(-30),
                FlightData.instance.gMapControl1.MapProvider.DbId);

            CustomMessageBox.Show("Removed " + removed + " images");

            log.InfoFormat("Removed {0} images", removed);
        }

        private void myButton1_Click_2(object sender, EventArgs e)
        {
            ParameterMetaDataParser.GetParameterInformation(
                "https://raw.githubusercontent.com/ArduPilot/ardupilot/ArduCopter-2.8.1/ArduCopter/Parameters.pde"
                , "ArduCopter2.8.1.xml");

            ParameterMetaDataParser.GetParameterInformation(
                "https://raw.githubusercontent.com/ArduPilot/ardupilot/ArduCopter-2.9.1/ArduCopter/Parameters.pde"
                , "ArduCopter2.9.1.xml");

            ParameterMetaDataParser.GetParameterInformation(
                "https://raw.githubusercontent.com/ArduPilot/ardupilot/ArduCopter-3.0/ArduCopter/Parameters.pde"
                , "ArduCopter3.0.xml");

            ParameterMetaDataParser.GetParameterInformation(
                "https://raw.githubusercontent.com/ArduPilot/ardupilot/ArduCopter-3.1.5/ArduCopter/Parameters.pde"
                , "ArduCopter3.1.5.xml");

            ParameterMetaDataParser.GetParameterInformation(
                "https://raw.githubusercontent.com/ArduPilot/ardupilot/ArduCopter-3.2.1/ArduCopter/Parameters.pde"
                , "ArduCopter3.2.1.xml");

            ParameterMetaDataParser.GetParameterInformation(
                "https://raw.githubusercontent.com/ArduPilot/ardupilot/Copter-3.3.2/ArduCopter/Parameters.cpp"
                , "ArduCopter3.3.2.xml");

            ParameterMetaDataParser.GetParameterInformation(
                "https://raw.githubusercontent.com/ArduPilot/ardupilot/Copter-3.3/ArduCopter/Parameters.cpp"
                , "ArduCopter3.3.xml");

            ParameterMetaDataParser.GetParameterInformation(
         "https://raw.githubusercontent.com/ArduPilot/ardupilot/Copter-3.4/ArduCopter/Parameters.cpp"
         , "ArduCopter3.4.xml");

            ParameterMetaDataParser.GetParameterInformation(
                "https://raw.githubusercontent.com/ArduPilot/ardupilot/Copter-3.4.6/ArduCopter/Parameters.cpp"
                , "ArduCopter3.4.6.xml");

            ParameterMetaDataParser.GetParameterInformation(
                "https://raw.githubusercontent.com/ArduPilot/ardupilot/Copter-3.5.0/ArduCopter/Parameters.cpp"
                , "ArduCopter3.5.0.xml");

            ParameterMetaDataParser.GetParameterInformation(
                "https://raw.githubusercontent.com/ArduPilot/ardupilot/Copter-3.5.2/ArduCopter/Parameters.cpp"
                , "ArduCopter3.5.2.xml");

            ParameterMetaDataParser.GetParameterInformation(
                "https://raw.githubusercontent.com/ArduPilot/ardupilot/Copter-3.5.4/ArduCopter/Parameters.cpp"
                , "ArduCopter3.5.4.xml");



            // plane

            ParameterMetaDataParser.GetParameterInformation(
                "https://raw.githubusercontent.com/ArduPilot/ardupilot/ArduPlane-3.8.3/ArduPlane/Parameters.cpp"
                , "ArduPlane3.8.3.xml");

            ParameterMetaDataParser.GetParameterInformation(
          "https://raw.githubusercontent.com/ArduPilot/ardupilot/ArduPlane-3.7.1/ArduPlane/Parameters.cpp"
          , "ArduPlane3.7.1.xml");

            ParameterMetaDataParser.GetParameterInformation(
    "https://raw.githubusercontent.com/ArduPilot/ardupilot/ArduPlane-3.7.0/ArduPlane/Parameters.cpp"
    , "ArduPlane3.7.0.xml");

            ParameterMetaDataParser.GetParameterInformation(
           "https://raw.githubusercontent.com/ArduPilot/ardupilot/ArduPlane-3.6.0/ArduPlane/Parameters.cpp"
           , "ArduPlane3.6.0.xml");

            ParameterMetaDataParser.GetParameterInformation(
                "https://raw.githubusercontent.com/ArduPilot/ardupilot/ArduPlane-3.5.2/ArduPlane/Parameters.cpp"
                , "ArduPlane3.5.2.xml");

            ParameterMetaDataParser.GetParameterInformation(
                "https://raw.githubusercontent.com/ArduPilot/ardupilot/ArduPlane-3.3.0/ArduPlane/Parameters.pde"
                , "ArduPlane3.3.0.xml");

            ParameterMetaDataParser.GetParameterInformation(
                "https://raw.githubusercontent.com/ArduPilot/ardupilot/ArduPlane-3.2.2/ArduPlane/Parameters.pde"
                , "ArduPlane3.2.2.xml");

            ParameterMetaDataParser.GetParameterInformation(
                "https://raw.githubusercontent.com/ArduPilot/ardupilot/ArduPlane-3.1.0/ArduPlane/Parameters.pde"
                , "ArduPlane3.1.0.xml");

            ParameterMetaDataParser.GetParameterInformation(
                "https://raw.githubusercontent.com/ArduPilot/ardupilot/ArduPlane-3.0.3/ArduPlane/Parameters.pde"
                , "ArduPlane3.0.3.xml");

            ParameterMetaDataParser.GetParameterInformation(
                "https://raw.githubusercontent.com/ArduPilot/ardupilot/ArduPlane-2.78b/ArduPlane/Parameters.pde"
                , "ArduPlane2.78b.xml");

            ParameterMetaDataParser.GetParameterInformation(
                "https://raw.githubusercontent.com/ArduPilot/ardupilot/ArduPlane-2.75/ArduPlane/Parameters.pde"
                , "ArduPlane2.75.xml");
        }

        private void but_signkey_Click(object sender, EventArgs e)
        {
            var auth = new AuthKeys();

            auth.Show();
        }

        private void but_optflowcalib_Click(object sender, EventArgs e)
        {
            var test = new Form();
            var imagebox = new PictureBox();
            imagebox.Dock = DockStyle.Fill;
            imagebox.SizeMode = PictureBoxSizeMode.Zoom;
            test.Controls.Add(imagebox);

            test.Show();

            var flow = new OpticalFlow(MainV2.comPort);

            // disable on close form
            test.Closed += (o, args) =>
            {
                flow.CalibrationMode(false);
                flow.Close();
            };

            // enable calibration mode
            flow.CalibrationMode(true);

            // setup bitmap to screen
            flow.newImage += (s, eh) => imagebox.Image = (Image)eh.Image.Clone();
        }

        private void myButton2_Click(object sender, EventArgs e)
        {
            var sp = new Sphere();

            sp.Dock = DockStyle.Fill;

            var frm = new Form();

            frm.Controls.Add(sp);

            frm.Show();
        }

        private void but_gpsinj_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "tlog|*.tlog";
            ofd.ShowDialog();

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = "output.dat";
            var result = sfd.ShowDialog();

            if (ofd.CheckFileExists && result == DialogResult.OK)
            {
                using (var st = sfd.OpenFile())
                {
                    using (MAVLinkInterface mine = new MAVLinkInterface(ofd.OpenFile()))
                    {
                        mine.logreadmode = true;

                        while (mine.logplaybackfile.BaseStream.Position < mine.logplaybackfile.BaseStream.Length)
                        {
                            MAVLink.MAVLinkMessage packet = mine.readPacket();

                            if (packet.msgid == (uint)MAVLink.MAVLINK_MSG_ID.GPS_INJECT_DATA)
                            {
                                var item = packet.ToStructure<MAVLink.mavlink_gps_inject_data_t>();
                                st.Write(item.data, 0, item.len);
                            }
                            else if (packet.msgid == (uint)MAVLink.MAVLINK_MSG_ID.GPS_RTCM_DATA)
                            {
                                var item = packet.ToStructure<MAVLink.mavlink_gps_rtcm_data_t>();
                                st.Write(item.data, 0, item.len);
                            }
                        }
                    }
                }
            }
        }

        private void but_followswarm_Click(object sender, EventArgs e)
        {
            new Swarm.WaypointLeader.WPControl().Show();
        }

        private void myButton3_Click(object sender, EventArgs e)
        {
            but_GDAL_Click(sender, e);
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case 0x86: // WM_NCACTIVATE
                    var child = Control.FromHandle(m.LParam);

                    if (child is Form)
                    {
                        ThemeManager.ApplyThemeTo(child);
                    }
                    break;
            }
            base.WndProc(ref m);
        }

        private void but_GDAL_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                if (Directory.Exists(fbd.SelectedPath))
                {
                    GDAL.GDAL.OnProgress += GDAL_OnProgress;
                    GDAL.GDAL.ScanDirectory(fbd.SelectedPath);
                    DTED.OnProgress += GDAL_OnProgress;
                    DTED.AddCustomDirectory(fbd.SelectedPath);

                    Loading.Close();
                }
            }
        }

        private void GDAL_OnProgress(double percent, string message)
        {
            Loading.ShowLoading((percent).ToString("0.0%") + " " +message, this);
        }

        private void but_sortlogs_Click(object sender, EventArgs e)
        {
            MissionPlanner.Log.LogSort.SortLogs(Directory.GetFiles(Settings.Instance.LogDir, "*.tlog",
                SearchOption.AllDirectories), Settings.Instance.LogDir);
        }

        private void but_logdlscp_Click(object sender, EventArgs e)
        {
            LogDownloadscp form = new LogDownloadscp();
            form.Show();
        }

        private void but_td_Click(object sender, EventArgs e)
        {
           
        }

        private void but_dem_Click(object sender, EventArgs e)
        {
            UserControl ctl = new UserControl() {Width = 1100, AutoSize = true};

            string line = "";

            foreach (var item in GeoTiff.index)
            {
                //log.InfoFormat("Start Point ({0},{1},{2}) --> ({3},{4},{5})", item.i, item.j, item.k, item.x, item.y, item.z);

                line += String.Format("{0} = {1} = {2}*{3} {4}\n", item.FileName, item.Area, item.width, item.height, item.bits,
                    item.xscale, item.yscale, item.zscale);
            }

            ctl.Controls.Add(new Label() {Text = line, AutoSize = true, Location = new Point(0, 30)});
            var butt = new MyButton() {Text = "Open DEM Dir"};
            butt.Click += (a, ev) =>
            {
                System.Diagnostics.Process.Start(@"C:\ProgramData\Mission Planner\srtm\");
            };
            ctl.Controls.Add(butt);

            ctl.ShowUserControl();
        }

        private void but_gsttest_Click(object sender, EventArgs e)
        {
            System.Threading.ThreadPool.QueueUserWorkItem((a) =>
            {
                //GStreamer.test();
            });
        }

        private void but_proximity_Click(object sender, EventArgs e)
        {
            new ProximityControl(MainV2.comPort.MAV).Show();
        }

        private void but_dashware_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "bin|*.bin";
            ofd.ShowDialog();

            if (ofd.CheckFileExists)
            {
                string options = "GPS;ATT;NTUN;CTUN;MODE;CURR";
                InputBox.Show("", "Enter Messages you want eg PARM;NTUN;CTUN", ref options);

                var split = options.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);

                DashWare.Create(ofd.FileName, ofd.FileName + ".csv", split.Length > 0 ? split.ToList() : null);
            }
        }

        private void but_mavinspector_Click(object sender, EventArgs e)
        {
            new MAVLinkInspector(MainV2.comPort).Show();
        }

        private void BUT_driverclean_Click(object sender, EventArgs e)
        {
            CleanDrivers.Clean();
        }

        private void but_blupdate_Click(object sender, EventArgs e)
        {
            if (CustomMessageBox.Show("Are you sure you want to upgrade the bootloader? This can brick your board",
                    "BL Update", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == (int)DialogResult.Yes)
                if (CustomMessageBox.Show("Are you sure you want to upgrade the bootloader? This can brick your board",
                        "BL Update", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == (int) DialogResult.Yes)
                    if (MainV2.comPort.doCommand(MAVLink.MAV_CMD.FLASH_BOOTLOADER, 0, 0, 0, 0, 290876, 0, 0))
                    {
                        CustomMessageBox.Show("Upgraded bootloader");
                    }
                    else
                    {
                        CustomMessageBox.Show("Failed to upgrade bootloader");
                    }
        }

        private void but_3dmap_Click(object sender, EventArgs e)
        {
            var ogl = new OpenGLtest2();

            ogl.ShowUserControl();
        }

        private void but_anonlog_Click(object sender, EventArgs e)
        {
            CustomMessageBox.Show("This is beta, please confirm the output file");
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "tlog or bin/log|*.tlog;*.bin;*.log";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    var ext = Path.GetExtension(ofd.FileName).ToLower();
                    if (ext == ".bin")
                        ext = ".log";
                    using (SaveFileDialog sfd = new SaveFileDialog())
                    {
                        sfd.DefaultExt = ext;
                        sfd.FileName = Path.GetFileNameWithoutExtension(ofd.FileName) + "-anon" + ext;
                        if (sfd.ShowDialog() == DialogResult.OK)
                        {
                            Privacy.anonymise(ofd.FileName, sfd.FileName);
                        }
                    }
                }
            }
        }

        private void but_messageinterval_Click(object sender, EventArgs e)
        {
            var form = new Form();
            FlowLayoutPanel flp = new FlowLayoutPanel();
            flp.Dock = DockStyle.Fill;
            flp.AutoSize = true;
            ComboBox cmb = new ComboBox();
            cmb.DataSource = Enum.GetNames(typeof(MAVLink.MAVLINK_MSG_ID)).ToSortedList((s, s1) => s.CompareTo(s1));
            cmb.Width += 50;
            Button but = new Button();
            but.Text = "Set";
            ComboBox cmbrate = new ComboBox();
            cmbrate.DataSource = Enumerable.Range(1, 200).ToList();

            but.Click += (o, args) =>
            {
                var rate = int.Parse(cmbrate.Text.ToString());
                var value = Enum.Parse(typeof(MAVLink.MAVLINK_MSG_ID), cmb.Text.ToString());
                MainV2.comPort.doCommand(MAVLink.MAV_CMD.SET_MESSAGE_INTERVAL, (float) (int) value,
                    1 / (float) rate * 1000000.0f, 0, 0, 0, 0, 0);
            };

            Button but2 = new Button();
            but2.Text = "Set All";
            but2.Click += (o, args) =>
            {
                var rate = int.Parse(cmbrate.Text.ToString());
                ((IList) cmb.DataSource).ForEach(a =>
                {
                    var value = Enum.Parse(typeof(MAVLink.MAVLINK_MSG_ID), a.ToString());
                    MainV2.comPort.doCommand(MAVLink.MAV_CMD.SET_MESSAGE_INTERVAL, (float) (int) value,
                        1 / (float) rate * 1000000.0f, 0, 0, 0, 0, 0, false);
                });
            };

            //cmb.SelectedIndexChanged += (o, args) => { MAVLink.MAVLINK_MSG_ID.GET_MESSAGE_INTERVAL };

            form.Controls.Add(flp);

            flp.Controls.Add(cmb);
            flp.Controls.Add(cmbrate);
            flp.Controls.Add(but);

            flp.Controls.Add(but2);

            form.Show(this);
        }

        private void BUT_xplane_Click(object sender, EventArgs e)
        {

        }

        private void but_disablearmswitch_Click(object sender, EventArgs e)
        {
            if (CustomMessageBox.Show("Are you sure?", "", MessageBoxButtons.YesNo) == (int) DialogResult.Yes)
                MainV2.comPort.setMode(
                    new MAVLink.mavlink_set_mode_t() {custom_mode = MainV2.comPort.MAV.cs.armed ? 0u : 1u},
                    MAVLink.MAV_MODE_FLAG.SAFETY_ARMED);
        }
    }
}