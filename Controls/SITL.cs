using GMap.NET;
using GMap.NET.WindowsForms;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using MissionPlanner.Maps;
using MissionPlanner.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MissionPlanner.Controls
{
    public partial class SITL : MyUserControl, IActivate
    {
        //https://regex101.com/r/cH3kV3/2
        //https://regex101.com/r/cH3kV3/3
        Regex default_params_regex = new Regex(@"""([^""]+)""\s*:\s*\{\s*[^\{}]+""default_params_filename""\s*:\s*\[*""([^""]+)""\s*[^\}]*\}");

        Uri sitlurl = new Uri("http://firmware.ardupilot.org/Tools/MissionPlanner/sitl/");

        string sitldirectory = Settings.GetUserDataDirectory() + "sitl" +
                               Path.DirectorySeparatorChar;

        GMapOverlay markeroverlay;

        GMapMarkerWP homemarker = new GMapMarkerWP(new PointLatLng(-34.98106, 117.85201), "H");
        bool onmarker = false;
        bool mousedown = false;
        private PointLatLng MouseDownStart;

        internal static UdpClient SITLSEND;

        internal static System.Diagnostics.Process simulator;

        /*
    { "quadplane",          QuadPlane::create },
    { "xplane",             XPlane::create },
    { "firefly",            QuadPlane::create },
    { "+",                  MultiCopter::create },
    { "quad",               MultiCopter::create },
    { "copter",             MultiCopter::create },
    { "x",                  MultiCopter::create },
    { "hexa",               MultiCopter::create },
    { "octa",               MultiCopter::create },
    { "tri",                MultiCopter::create },
    { "y6",                 MultiCopter::create },
    { "heli",               Helicopter::create },
    { "heli-dual",          Helicopter::create },
    { "heli-compound",      Helicopter::create },
    { "singlecopter",       SingleCopter::create },
    { "coaxcopter",         SingleCopter::create },
    { "rover",              SimRover::create },
    { "crrcsim",            CRRCSim::create },
    { "jsbsim",             JSBSim::create },
    { "flightaxis",         FlightAxis::create },
    { "gazebo",             Gazebo::create },
    { "last_letter",        last_letter::create },
    { "tracker",            Tracker::create },
    { "balloon",            Balloon::create },
    { "plane",              Plane::create },
    { "calibration",        Calibration::create },
             */

        ///tmp/.build/ArduCopter.elf -M+ -O-34.98106,117.85201,40,0 
        ///tmp/.build/APMrover2.elf -Mrover -O-34.98106,117.85201,40,0 
        ///tmp/.build/ArduPlane.elf -Mjsbsim -O-34.98106,117.85201,40,0 --autotest-dir ./
        ///tmp/.build/ArduCopter.elf -Mheli -O-34.98106,117.85201,40,0 
        ~SITL()
        {
            try
            {
                if (simulator != null)
                    simulator.Kill();
            }
            catch
            {
            }
        }

        public SITL()
        {
            InitializeComponent();

            if (!Directory.Exists(sitldirectory))
                Directory.CreateDirectory(sitldirectory);

        }

        public void Activate()
        {
            homemarker.Position = MainV2.comPort.MAV.cs.HomeLocation;

            myGMAP1.Position = homemarker.Position;

            myGMAP1.MapProvider = GCSViews.FlightData.mymap.MapProvider;
            myGMAP1.MaxZoom = 22;
            myGMAP1.Zoom = 16;
            myGMAP1.DisableFocusOnMouseEnter = true;

            markeroverlay = new GMapOverlay("markers");
            myGMAP1.Overlays.Add(markeroverlay);

            markeroverlay.Markers.Add(homemarker);

            myGMAP1.Invalidate();

            Utilities.ThemeManager.ApplyThemeTo(this);

            MissionPlanner.Utilities.Tracking.AddPage(this.GetType().ToString(), this.Text);
        }

        private void pictureBoxplane_Click(object sender, EventArgs e)
        {
            //Common.MessageShowAgain("MS Visual C++ Runtime 2013", "Please note that the plane sim requires\n'Visual C++ Redistributable Packages for Visual Studio 2013' to run.\n https://www.microsoft.com/en-us/download/details.aspx?id=40784");

            var exepath = CheckandGetSITLImage("ArduPlane.elf");

            string simdir = sitldirectory + "jsbsim" + Path.DirectorySeparatorChar;
            string destfile = simdir + Path.DirectorySeparatorChar + "JSBSim.exe";

            if (!File.Exists(destfile))
            {
                //Directory.CreateDirectory(simdir);
                //File.Copy(Application.StartupPath + Path.DirectorySeparatorChar + "JSBSim.exe", destfile);
            }

            if (markeroverlay.Markers.Count == 0)
            {
                CustomMessageBox.Show(Strings.Invalid_home_location);
                return;
            }

            //StartSITL(exepath, "jsbsim", BuildHomeLocation(markeroverlay.Markers[0].Position, (int) NUM_heading.Value),@" --autotest-dir """ + Application.StartupPath.Replace('\\', '/') + @"""", 1);

            StartSITL(exepath, "plane", BuildHomeLocation(markeroverlay.Markers[0].Position, (int)NUM_heading.Value), "", (int)num_simspeed.Value);
        }

        private void pictureBoxrover_Click(object sender, EventArgs e)
        {
            if (markeroverlay.Markers.Count == 0)
            {
                CustomMessageBox.Show(Strings.Invalid_home_location);
                return;
            }
            var exepath = CheckandGetSITLImage("APMrover2.elf");

            StartSITL(exepath, "rover", BuildHomeLocation(markeroverlay.Markers[0].Position, (int)NUM_heading.Value), "", (int)num_simspeed.Value);
        }

        private void pictureBoxquad_Click(object sender, EventArgs e)
        {
            if (markeroverlay.Markers.Count == 0)
            {
                CustomMessageBox.Show(Strings.Invalid_home_location);
                return;
            }
            var exepath = CheckandGetSITLImage("ArduCopter.elf");

            StartSITL(exepath, "+", BuildHomeLocation(markeroverlay.Markers[0].Position, (int)NUM_heading.Value), "", (int)num_simspeed.Value);
        }

        private void pictureBoxheli_Click(object sender, EventArgs e)
        {
            if (markeroverlay.Markers.Count == 0)
            {
                CustomMessageBox.Show(Strings.Invalid_home_location);
                return;
            }
            var exepath = CheckandGetSITLImage("ArduHeli.elf");

            StartSITL(exepath, "heli", BuildHomeLocation(markeroverlay.Markers[0].Position, (int)NUM_heading.Value), "", (int)num_simspeed.Value);
        }

        string BuildHomeLocation(PointLatLng homelocation, int heading = 0)
        {
            return String.Format("{0},{1},{2},{3}", homelocation.Lat.ToString(CultureInfo.InvariantCulture), homelocation.Lng.ToString(CultureInfo.InvariantCulture),
                srtm.getAltitude(homelocation.Lat, homelocation.Lng).alt.ToString(CultureInfo.InvariantCulture), heading.ToString(CultureInfo.InvariantCulture));
        }

        private string CheckandGetSITLImage(string filename)
        {
            Uri fullurl = new Uri(sitlurl, filename);

            var load = Common.LoadingBox("Downloading", "Downloading sitl software");

            Download.getFilefromNet(fullurl.ToString(),
                sitldirectory + Path.GetFileNameWithoutExtension(filename) + ".exe");

            load.Refresh();

            // dependancys
            var depurl = new Uri(sitlurl, "cyggcc_s-1.dll");
            Download.getFilefromNet(depurl.ToString(), sitldirectory + depurl.Segments[depurl.Segments.Length - 1]);

            load.Refresh();
            depurl = new Uri(sitlurl, "cygstdc++-6.dll");
            Download.getFilefromNet(depurl.ToString(), sitldirectory + depurl.Segments[depurl.Segments.Length - 1]);

            load.Refresh();
            depurl = new Uri(sitlurl, "cygwin1.dll");
            Download.getFilefromNet(depurl.ToString(), sitldirectory + depurl.Segments[depurl.Segments.Length - 1]);

            load.Close();

            return sitldirectory + Path.GetFileNameWithoutExtension(filename) + ".exe";
        }

        private string GetDefaultConfig(string model)
        {
            if (Download.getFilefromNet(
                "https://raw.githubusercontent.com/ArduPilot/ardupilot/master/Tools/autotest/sim_vehicle.py",
                sitldirectory + "sim_vehicle.py") || File.Exists(sitldirectory + "sim_vehicle.py"))
            {
                var matches = default_params_regex.Matches(File.ReadAllText(sitldirectory + "sim_vehicle.py"));

                foreach (Match match in matches)
                {
                    if (match.Groups[1].Value.ToLower().Equals(model))
                    {
                        if (Download.getFilefromNet(
                            "https://raw.githubusercontent.com/ArduPilot/ardupilot/master/Tools/autotest/" +
                            match.Groups[2].Value.ToString(),
                            sitldirectory + match.Groups[2].Value.ToString()) || File.Exists(sitldirectory + match.Groups[2].Value.ToString()))
                            return sitldirectory + match.Groups[2].Value.ToString();
                    }
                }
            }

            if (Download.getFilefromNet(
                "https://raw.githubusercontent.com/ArduPilot/ardupilot/master/Tools/autotest/pysim/vehicleinfo.py",
                sitldirectory + "vehicleinfo.py") || File.Exists(sitldirectory + "vehicleinfo.py"))
            {
                cleanupJson(sitldirectory + "vehicleinfo.py");

                using (Newtonsoft.Json.JsonTextReader reader =
                    new JsonTextReader(File.OpenText(sitldirectory + "vehicleinfo.py")))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    var obj = (JObject) serializer.Deserialize(reader);

                    if (obj == null)
                        return "";

                    foreach (var fwtype in obj)
                    {
                        var frames = fwtype.Value["frames"];

                        if (frames == null)
                            continue;

                        var config = frames[model];

                        if (config == null)
                            continue;

                        var configs = config["default_params_filename"];

                        if (configs is JValue)
                        {
                            if (Download.getFilefromNet(
                                "https://raw.githubusercontent.com/ArduPilot/ardupilot/master/Tools/autotest/" +
                                configs.ToString(),
                                sitldirectory + configs.ToString()) || File.Exists(sitldirectory + configs.ToString()))
                            {
                                return sitldirectory + configs.ToString();
                            }
                        }

                        string data = "";

                        foreach (var config1 in configs)
                        {
                            if (Download.getFilefromNet(
                                "https://raw.githubusercontent.com/ArduPilot/ardupilot/master/Tools/autotest/" +
                                config1.ToString(),
                                sitldirectory + config1.ToString()) || File.Exists(sitldirectory + config1.ToString()))
                            {
                                data += "\r\n" + File.ReadAllText(sitldirectory + config1.ToString());
                            }
                        }

                        var temp = Path.GetTempFileName();
                        File.WriteAllText(temp, data);
                        return temp;
                    }
                }
            }
            return "";
        }

        void cleanupJson(string filename)
        {
            var content = File.ReadAllText(filename);

            var match = BraceMatch(content, '{', '}');

            match = Regex.Replace(match, @"#.*", "");

            File.WriteAllText(filename, match);
        }

        static string BraceMatch(string text, char braces, char bracee)
        {
            int level = 0;
            int start = 0;
            int end = 0;

            int index = -1;

            foreach (char c in text)
            {
                index++;
                if (c == braces)
                {
                    if (level == 0)
                        start = index;
                    // opening brace detected
                    level++;
                }

                if (c == bracee)
                {
                    level--;
                    end = index;

                    if (level == 0)
                        return text.Substring(start, end - start + 1);
                }
            }

            return "";
        }

        private void StartSITL(string exepath, string model, string homelocation, string extraargs = "", int speedup = 1)
        {
            if (String.IsNullOrEmpty(homelocation))
            {
                CustomMessageBox.Show(Strings.Invalid_home_location, Strings.ERROR);
                return;
            }

            if (!File.Exists(exepath))
            {
                CustomMessageBox.Show(Strings.Failed_to_download_the_SITL_image, Strings.ERROR);
                return;
            }

            // kill old session
            try
            {
                if (simulator != null)
                    simulator.Kill();
            }
            catch
            {
            }

            // override default model
            if (cmb_model.Text != "")
                model = cmb_model.Text;

            var config = GetDefaultConfig(model);

            if (!string.IsNullOrEmpty(config))
                extraargs += @" --defaults """ + config+@"""";

            extraargs += " " + txt_cmdline.Text + " ";

            if (chk_wipe.Checked)
                extraargs += " --wipe ";

            string simdir = sitldirectory + model + Path.DirectorySeparatorChar;

            Directory.CreateDirectory(simdir);

            string path = Environment.GetEnvironmentVariable("PATH");

            Environment.SetEnvironmentVariable("PATH", sitldirectory + ";" + simdir + ";" + path, EnvironmentVariableTarget.Process);

            Environment.SetEnvironmentVariable("HOME", simdir, EnvironmentVariableTarget.Process);

            ProcessStartInfo exestart = new ProcessStartInfo();
            exestart.FileName = exepath;
            exestart.Arguments = String.Format("-M{0} -O{1} -s{2} --uartA tcp:0 {3}", model, homelocation, speedup, extraargs);
            exestart.WorkingDirectory = simdir;
            exestart.WindowStyle = ProcessWindowStyle.Minimized;
            exestart.UseShellExecute = true;

            try
            {
                simulator = System.Diagnostics.Process.Start(exestart);
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Failed to start the simulator\n"+ ex.ToString(), Strings.ERROR);
                return;
            }

            System.Threading.Thread.Sleep(2000);

            MainV2.View.ShowScreen(MainV2.View.screens[0].Name);

            var client = new Comms.TcpSerial();

            try
            {
                client.client = new TcpClient("127.0.0.1", 5760);

                MainV2.comPort.BaseStream = client;

                SITLSEND = new UdpClient("127.0.0.1", 5501);

                Thread.Sleep(200);

                MainV2.instance.doConnect(MainV2.comPort, "preset", "5760");
            }
            catch
            {
                CustomMessageBox.Show(Strings.Failed_to_connect_to_SITL_instance, Strings.ERROR);
                return;
            }
        }

        static internal void rcinput()
        {
            try
            {
                byte[] rcreceiver = new byte[2*8];
                Array.ConstrainedCopy(BitConverter.GetBytes((ushort) MainV2.comPort.MAV.cs.rcoverridech1), 0, rcreceiver,0, 2);
                Array.ConstrainedCopy(BitConverter.GetBytes((ushort) MainV2.comPort.MAV.cs.rcoverridech2), 0, rcreceiver,2, 2);
                Array.ConstrainedCopy(BitConverter.GetBytes((ushort) MainV2.comPort.MAV.cs.rcoverridech3), 0, rcreceiver,4, 2);
                Array.ConstrainedCopy(BitConverter.GetBytes((ushort) MainV2.comPort.MAV.cs.rcoverridech4), 0, rcreceiver,6, 2);
                Array.ConstrainedCopy(BitConverter.GetBytes((ushort) MainV2.comPort.MAV.cs.rcoverridech5), 0, rcreceiver,8, 2);
                Array.ConstrainedCopy(BitConverter.GetBytes((ushort) MainV2.comPort.MAV.cs.rcoverridech6), 0, rcreceiver,10, 2);
                Array.ConstrainedCopy(BitConverter.GetBytes((ushort) MainV2.comPort.MAV.cs.rcoverridech7), 0, rcreceiver,12, 2);
                Array.ConstrainedCopy(BitConverter.GetBytes((ushort) MainV2.comPort.MAV.cs.rcoverridech8), 0, rcreceiver,14, 2);

                SITLSEND.Send(rcreceiver, rcreceiver.Length);
            }
            catch
            {
            }
        }

        private void myGMAP1_OnMarkerEnter(GMapMarker item)
        {
            if (!mousedown)
                onmarker = true;
        }

        private void myGMAP1_OnMarkerLeave(GMapMarker item)
        {
            if (!mousedown)
                onmarker = false;
        }

        private void myGMAP1_MouseMove(object sender, MouseEventArgs e)
        {
            if (onmarker)
            {
                if (e.Button == MouseButtons.Left)
                {
                    homemarker.Position = myGMAP1.FromLocalToLatLng(e.X, e.Y);
                }
            }
            else if (mousedown)
            {
                PointLatLng point = myGMAP1.FromLocalToLatLng(e.X, e.Y);

                double latdif = MouseDownStart.Lat - point.Lat;
                double lngdif = MouseDownStart.Lng - point.Lng;

                try
                {
                    myGMAP1.Position = new PointLatLng(myGMAP1.Position.Lat + latdif, myGMAP1.Position.Lng + lngdif);
                }
                catch
                {
                }
            }
        }

        private void myGMAP1_MouseUp(object sender, MouseEventArgs e)
        {
            mousedown = false;
            onmarker = false;
        }

        private void myGMAP1_MouseDown(object sender, MouseEventArgs e)
        {
            mousedown = true;
            MouseDownStart = myGMAP1.FromLocalToLatLng(e.X, e.Y);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.S))
            {
                var exepath = CheckandGetSITLImage("ArduCopter.elf");
                var model = "+";

                var config = GetDefaultConfig(model);
                var max = 10.0;

                if (InputBox.Show("how many?", "how many?", ref max) != DialogResult.OK)
                    return true;

                max--;

                for (int a = (int)max; a >= 0 ; a--)
                {
                    var extra = " --disable-fgview -r50";

                    if (!string.IsNullOrEmpty(config))
                        extra += @" --defaults """ + config + @""" -P SERIAL0_PROTOCOL=2 -P SERIAL1_PROTOCOL=2 ";

                    var home = new PointLatLngAlt(markeroverlay.Markers[0].Position).newpos((double)NUM_heading.Value, a * 4);

                    if (max == a)
                    {
                        extra += String.Format(
                            " -M{4} -s1 --home {3} --instance {0} --uartA tcp:0 {1} -P SYSID_THISMAV={2} ",
                            a, "", a + 1, BuildHomeLocation(home, (int) NUM_heading.Value), model);
                    }
                    else
                    {
                        extra += String.Format(
                            " -M{4} -s1 --home {3} --instance {0} --uartA tcp:0 {1} -P SYSID_THISMAV={2} ",
                            a, "--uartD tcpclient:127.0.0.1:" + (5772 + 10 * a), a + 1, BuildHomeLocation(home, (int) NUM_heading.Value), model);
                    }

                    string simdir = sitldirectory + model + (a+1) + Path.DirectorySeparatorChar;

                    Directory.CreateDirectory(simdir);

                    string path = Environment.GetEnvironmentVariable("PATH");

                    Environment.SetEnvironmentVariable("PATH", sitldirectory + ";" + simdir + ";" + path, EnvironmentVariableTarget.Process);

                    Environment.SetEnvironmentVariable("HOME", simdir, EnvironmentVariableTarget.Process);

                    ProcessStartInfo exestart = new ProcessStartInfo();
                    exestart.FileName = exepath;
                    exestart.Arguments = extra;
                    exestart.WorkingDirectory = simdir;
                    exestart.WindowStyle = ProcessWindowStyle.Minimized;
                    exestart.UseShellExecute = true;

                    File.AppendAllText(Settings.GetUserDataDirectory() + "sitl.bat", "mkdir " + (a + 1) + "\ncd " + (a + 1) + "\n" + @"""" + exepath + @"""" + " " + extra + " &\n");

                    File.AppendAllText(Settings.GetUserDataDirectory() + "sitl1.sh", "mkdir " + (a + 1) + "\ncd " + (a + 1) + "\n" + @"""../" + Path.GetFileName(exepath).Replace("C:", "/mnt/c").Replace("\\", "/").Replace(".exe",".elf") + @"""" + " " + extra.Replace("C:", "/mnt/c").Replace("\\", "/") + " &\nsleep .3\ncd ..\n");

                    Process.Start(exestart);
                }

                try
                {
                    var client = new Comms.TcpSerial();

                    client.client = new TcpClient("127.0.0.1", 5760);

                    MainV2.comPort.BaseStream = client;

                    SITLSEND = new UdpClient("127.0.0.1", 5501);

                    Thread.Sleep(200);

                    MainV2.instance.doConnect(MainV2.comPort, "preset", "5760");

                    return true;
                }
                catch
                {
                    CustomMessageBox.Show(Strings.Failed_to_connect_to_SITL_instance, Strings.ERROR);
                    return true;
                }
            }

            if (keyData == (Keys.Control | Keys.D))
            {
                var exepath = CheckandGetSITLImage("ArduCopter.elf");
                var model = "+";

                var config = GetDefaultConfig(model);
                var max = 10.0;

                if (InputBox.Show("how many?", "how many?", ref max) != DialogResult.OK)
                    return true;

                max--;

                for (int a = (int)max; a >= 0; a--)
                {
                    var extra = " --disable-fgview -r50 ";

                    if (!string.IsNullOrEmpty(config))
                        extra += @" --defaults """ + config + @""" -P SERIAL0_PROTOCOL=2 -P SERIAL1_PROTOCOL=2 ";

                    var home = new PointLatLngAlt(markeroverlay.Markers[0].Position).newpos((double)NUM_heading.Value, a * 4);

                    if (max == a)
                    {
                        extra += String.Format(
                            " -M{4} -s1 --home {3} --instance {0} --uartA tcp:0 {1} -P SYSID_THISMAV={2} ",
                            a, "", a + 1, BuildHomeLocation(home, (int)NUM_heading.Value), model);
                    }
                    else
                    {
                        extra += String.Format(
                            " -M{4} -s1 --home {3} --instance {0} --uartA tcp:0 {1} -P SYSID_THISMAV={2} ",
                            a, ""/*"--uartD tcpclient:127.0.0.1:" + (5770 + 10 * a)*/, a + 1, BuildHomeLocation(home, (int)NUM_heading.Value), model);
                    }

                    string simdir = sitldirectory + model + (a + 1) + Path.DirectorySeparatorChar;

                    Directory.CreateDirectory(simdir);

                    string path = Environment.GetEnvironmentVariable("PATH");

                    Environment.SetEnvironmentVariable("PATH", sitldirectory + ";" + simdir + ";" + path, EnvironmentVariableTarget.Process);

                    Environment.SetEnvironmentVariable("HOME", simdir, EnvironmentVariableTarget.Process);

                    ProcessStartInfo exestart = new ProcessStartInfo();
                    exestart.FileName = exepath;
                    exestart.Arguments = extra;
                    exestart.WorkingDirectory = simdir;
                    exestart.WindowStyle = ProcessWindowStyle.Minimized;
                    exestart.UseShellExecute = true;

                    Process.Start(exestart);
                }

                try
                {
                    for (int a = (int)max; a >= 0; a--)
                    {
                        var mav = new MAVLinkInterface();

                        var client = new Comms.TcpSerial();

                        client.client = new TcpClient("127.0.0.1", 5760 + (10*(a)));

                        mav.BaseStream = client;

                        //SITLSEND = new UdpClient("127.0.0.1", 5501);

                        Thread.Sleep(200);

                        MainV2.instance.doConnect(mav, "preset", "5760");

                        try
                        {
                            mav.GetParam("SYSID_THISMAV");
                            mav.setParam("SYSID_THISMAV", a + 1, true);

                            mav.GetParam("FRAME_CLASS");
                            mav.setParam("FRAME_CLASS", 1, true);
                        } catch { }

                        MainV2.Comports.Add(mav);
                    }

                    return true;
                }
                catch
                {
                    CustomMessageBox.Show(Strings.Failed_to_connect_to_SITL_instance, Strings.ERROR);
                    return true;
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}