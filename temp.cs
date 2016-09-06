using System;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using DotSpatial.Data;
using DotSpatial.Projections;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using log4net;
using MissionPlanner.Arduino;
using MissionPlanner.Comms;
using MissionPlanner.Controls;
using MissionPlanner.GCSViews;
using MissionPlanner.GeoRef;
using MissionPlanner.HIL;
using MissionPlanner.Log;
using MissionPlanner.Maps;
using MissionPlanner.Swarm;
using MissionPlanner.Utilities;
using MissionPlanner.Warnings;
using resedit;
using ILog = log4net.ILog;
using LogAnalyzer = MissionPlanner.Utilities.LogAnalyzer;

namespace MissionPlanner
{
    public partial class temp : Form
    {
        private static readonly ILog log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static XPlane xp;

        private static MAVLinkSerialPort comport;

        private static TcpListener listener;

        private static TcpClient client;

        public temp()
        {
            InitializeComponent();

            //if (System.Diagnostics.Debugger.IsAttached) 
            {
                try
                {
                    var ogl = new OpenGLtest2();

                    Controls.Add(ogl);

                    ogl.Dock = DockStyle.Fill;
                }
                catch
                {
                }
            }

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
        }

        private void BUT_clearcustommaps_Click(object sender, EventArgs e)
        {
            var MainMap = new GMapControl();
            MainMap.MapProvider = GoogleSatelliteMapProvider.Instance;

            var removed = MainMap.Manager.PrimaryCache.DeleteOlderThan(DateTime.Now, Custom.Instance.DbId);

            CustomMessageBox.Show("Removed " + removed + " images");

            log.InfoFormat("Removed {0} images", removed);
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
            ParameterMetaDataParser.GetParameterInformation();

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

        private void BUT_xplane_Click(object sender, EventArgs e)
        {
            if (xp == null)
            {
                xp = new XPlane();

                xp.SetupSockets(49005, 49000, "127.0.0.1");
            }


            ThreadPool.QueueUserWorkItem(runxplanemove);

            //xp.Shutdown();
        }

        private void runxplanemove(object o)
        {
            while (xp != null)
            {
                Thread.Sleep(500);
                xp.MoveToPos(MainV2.comPort.MAV.cs.lat, MainV2.comPort.MAV.cs.lng, MainV2.comPort.MAV.cs.alt,
                    MainV2.comPort.MAV.cs.roll, MainV2.comPort.MAV.cs.pitch, MainV2.comPort.MAV.cs.yaw);
            }
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

        private void BUT_driverclean_Click(object sender, EventArgs e)
        {
            CleanDrivers.Clean();
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

            using (
                var xmlwriter = new XmlTextWriter(basedir + Path.DirectorySeparatorChar + @"firmware2.xml",
                    Encoding.ASCII))
            {
                xmlwriter.Formatting = Formatting.Indented;

                xmlwriter.WriteStartDocument();

                xmlwriter.WriteStartElement("options");

                foreach (var software in list)
                {
                    //if (!software.name.Contains("Copter"))
                    //  continue;

                    xmlwriter.WriteStartElement("Firmware");

                    if (software.url != "")
                        xmlwriter.WriteElementString("url", new Uri(software.url).LocalPath.TrimStart('/', '\\'));
                    if (software.url2560 != "")
                        xmlwriter.WriteElementString("url2560", new Uri(software.url2560).LocalPath.TrimStart('/', '\\'));
                    if (software.url2560_2 != "")
                        xmlwriter.WriteElementString("url2560-2",
                            new Uri(software.url2560_2).LocalPath.TrimStart('/', '\\'));
                    if (software.urlpx4v1 != "")
                        xmlwriter.WriteElementString("urlpx4", new Uri(software.urlpx4v1).LocalPath.TrimStart('/', '\\'));
                    if (software.urlpx4v2 != "")
                        xmlwriter.WriteElementString("urlpx4v2",
                            new Uri(software.urlpx4v2).LocalPath.TrimStart('/', '\\'));
                    if (software.urlpx4v4 != "")
                        xmlwriter.WriteElementString("urlpx4v4",
                            new Uri(software.urlpx4v4).LocalPath.TrimStart('/', '\\'));
                    if (software.urlvrbrainv40 != "")
                        xmlwriter.WriteElementString("urlvrbrainv40",
                            new Uri(software.urlvrbrainv40).LocalPath.TrimStart('/', '\\'));
                    if (software.urlvrbrainv45 != "")
                        xmlwriter.WriteElementString("urlvrbrainv45",
                            new Uri(software.urlvrbrainv45).LocalPath.TrimStart('/', '\\'));
                    if (software.urlvrbrainv50 != "")
                        xmlwriter.WriteElementString("urlvrbrainv50",
                            new Uri(software.urlvrbrainv50).LocalPath.TrimStart('/', '\\'));
                    if (software.urlvrbrainv51 != "")
                        xmlwriter.WriteElementString("urlvrbrainv51",
                            new Uri(software.urlvrbrainv51).LocalPath.TrimStart('/', '\\'));
                    if (software.urlvrbrainv52 != "")
                        xmlwriter.WriteElementString("urlvrbrainv52",
                            new Uri(software.urlvrbrainv52).LocalPath.TrimStart('/', '\\'));
                    if (software.urlvrcorev10 != "")
                        xmlwriter.WriteElementString("urlvrcorev10",
                            new Uri(software.urlvrcorev10).LocalPath.TrimStart('/', '\\'));
                    if (software.urlvrubrainv51 != "")
                        xmlwriter.WriteElementString("urlvrubrainv51",
                            new Uri(software.urlvrubrainv51).LocalPath.TrimStart('/', '\\'));
                    if (software.urlvrubrainv52 != "")
                        xmlwriter.WriteElementString("urlvrubrainv52",
                            new Uri(software.urlvrubrainv52).LocalPath.TrimStart('/', '\\'));
                    if (software.urlbebop2 != "")
                        xmlwriter.WriteElementString("urlbebop2",
                            new Uri(software.urlbebop2).LocalPath.TrimStart('/', '\\'));
                    xmlwriter.WriteElementString("name", software.name);
                    xmlwriter.WriteElementString("desc", software.desc);
                    xmlwriter.WriteElementString("format_version", software.k_format_version.ToString());

                    xmlwriter.WriteEndElement();

                    if (software.url != "")
                    {
                        Common.getFilefromNet(software.url, basedir + new Uri(software.url).LocalPath);
                    }
                    if (software.url2560 != "")
                    {
                        Common.getFilefromNet(software.url2560, basedir + new Uri(software.url2560).LocalPath);
                    }
                    if (software.url2560_2 != "")
                    {
                        Common.getFilefromNet(software.url2560_2, basedir + new Uri(software.url2560_2).LocalPath);
                    }
                    if (software.urlpx4v1 != "")
                    {
                        Common.getFilefromNet(software.urlpx4v1, basedir + new Uri(software.urlpx4v1).LocalPath);
                    }
                    if (software.urlpx4v2 != "")
                    {
                        Common.getFilefromNet(software.urlpx4v2, basedir + new Uri(software.urlpx4v2).LocalPath);
                    }
                    if (software.urlpx4v4 != "")
                    {
                        Common.getFilefromNet(software.urlpx4v4, basedir + new Uri(software.urlpx4v4).LocalPath);
                    }

                    if (software.urlvrbrainv40 != "")
                    {
                        Common.getFilefromNet(software.urlvrbrainv40,
                            basedir + new Uri(software.urlvrbrainv40).LocalPath);
                    }
                    if (software.urlvrbrainv45 != "")
                    {
                        Common.getFilefromNet(software.urlvrbrainv45,
                            basedir + new Uri(software.urlvrbrainv45).LocalPath);
                    }
                    if (software.urlvrbrainv50 != "")
                    {
                        Common.getFilefromNet(software.urlvrbrainv50,
                            basedir + new Uri(software.urlvrbrainv50).LocalPath);
                    }
                    if (software.urlvrbrainv51 != "")
                    {
                        Common.getFilefromNet(software.urlvrbrainv51,
                            basedir + new Uri(software.urlvrbrainv51).LocalPath);
                    }
                    if (software.urlvrbrainv52 != "")
                    {
                        Common.getFilefromNet(software.urlvrbrainv52,
                            basedir + new Uri(software.urlvrbrainv52).LocalPath);
                    }
                    if (software.urlvrcorev10 != "")
                    {
                        Common.getFilefromNet(software.urlvrcorev10, basedir + new Uri(software.urlvrcorev10).LocalPath);
                    }
                    if (software.urlvrubrainv51 != "")
                    {
                        Common.getFilefromNet(software.urlvrubrainv51,
                            basedir + new Uri(software.urlvrubrainv51).LocalPath);
                    }
                    if (software.urlvrubrainv52 != "")
                    {
                        Common.getFilefromNet(software.urlvrubrainv52,
                            basedir + new Uri(software.urlvrubrainv52).LocalPath);
                    }
                    if (software.urlbebop2 != "")
                    {
                        Common.getFilefromNet(software.urlbebop2,
                            basedir + new Uri(software.urlbebop2).LocalPath);
                    }
                }

                xmlwriter.WriteEndElement();
                xmlwriter.WriteEndDocument();
            }
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
                var listener = (TcpListener) ar.AsyncState;

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
                                double[] xyarray = {point.X, point.Y};
                                double[] zarray = {point.Z};

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
                array[l] = (byte) l;
            }

            var a = 0;
            var start = DateTime.MinValue;
            var end = DateTime.MinValue;


            start = DateTime.Now;
            for (a = 0; a < 1000000; a++)
            {
                var obj = (object) new MAVLink.mavlink_heartbeat_t();
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
            MainV2.comPort.setMode("Stabilize");

            if (MainV2.comPort.doARM(true))
            {
                MainV2.comPort.setMode("GUIDED");

                Thread.Sleep(300);

                MainV2.comPort.doCommand(MAVLink.MAV_CMD.TAKEOFF, 0, 0, 0, 0, 0, 0, 10);
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
            var port = "com1";
            if (InputBox.Show("enter comport", "enter comport", ref port) == DialogResult.OK)
            {
                new AP_GPS_GSOF(port);
            }
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
                if (GStreamer.checkGstLaunchExe())
                {
                    GStreamer.Start();
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(ex.ToString(), Strings.ERROR);
            }
        }

        private void but_agemapdata_Click(object sender, EventArgs e)
        {
            var removed = ((PureImageCache) MyImageCache.Instance).DeleteOlderThan(DateTime.Now.AddDays(-30),
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


// plane

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
            flow.newImage += (s, eh) => imagebox.Image = (Image) eh.Image.Clone();
        }

        private void myButton2_Click(object sender, EventArgs e)
        {
            var sp = new Sphere();

            sp.Dock = DockStyle.Fill;

            var frm = new Form();

            frm.Controls.Add(sp);

            frm.Show();
        }
    }
}