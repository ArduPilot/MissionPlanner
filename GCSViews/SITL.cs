using GMap.NET;
using GMap.NET.WindowsForms;
using MissionPlanner.Controls;
using MissionPlanner.Maps;
using MissionPlanner.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using log4net;
using MissionPlanner.ArduPilot;

namespace MissionPlanner.GCSViews
{
    public partial class SITL : MyUserControl, IActivate
    {
        internal static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        //https://regex101.com/r/cH3kV3/2
        //https://regex101.com/r/cH3kV3/3
        Regex default_params_regex = new Regex(@"""([^""]+)""\s*:\s*\{\s*[^\{}]+""default_params_filename""\s*:\s*\[*""([^""]+)""\s*[^\}]*\}");

        Uri sitlmasterurl = new Uri("https://firmware.ardupilot.org/Tools/MissionPlanner/sitl/");

        Uri sitlcopterstableurl = new Uri("https://firmware.ardupilot.org/Tools/MissionPlanner/sitl/CopterStable/");
        Uri sitlplanestableurl = new Uri("https://firmware.ardupilot.org/Tools/MissionPlanner/sitl/PlaneStable/");
        Uri sitlroverstableurl = new Uri("https://firmware.ardupilot.org/Tools/MissionPlanner/sitl/RoverStable/");

        string sitldirectory = Settings.GetUserDataDirectory() + "sitl" +
                               Path.DirectorySeparatorChar;

        public static string BundledPath = "";

        GMapOverlay markeroverlay;

        GMapMarkerWP homemarker = new GMapMarkerWP(new PointLatLng(-34.98106, 117.85201), "H");
        bool onmarker = false;
        bool mousedown = false;
        private PointLatLng MouseDownStart;

        internal static UdpClient SITLSEND;

        internal static List<System.Diagnostics.Process> simulator = new List<Process>();

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
                simulator.ForEach(a=>
                {
                    try
                    {
                        a.Kill();
                    }catch { }
                });
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
            if(MainV2.comPort.MAV.cs.PlannedHomeLocation.Lat == 0 && MainV2.comPort.MAV.cs.PlannedHomeLocation.Lng == 0)
                homemarker.Position = new PointLatLng(-35.3633515, 149.1652412);
            else
                homemarker.Position = MainV2.comPort.MAV.cs.PlannedHomeLocation;

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

        private async void pictureBoxplane_Click(object sender, EventArgs e)
        {

            var exepath = CheckandGetSITLImage("ArduPlane.elf");

            if (markeroverlay.Markers.Count == 0)
            {
                CustomMessageBox.Show(Strings.Invalid_home_location);
                return;
            }

            try
            {
                StartSITL(await exepath, "plane",
                    BuildHomeLocation(markeroverlay.Markers[0].Position, (int) NUM_heading.Value), "",
                    (int) num_simspeed.Value);
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Failed to download and start sitl\n" + ex.ToString());
            }
        }

        private async void pictureBoxrover_Click(object sender, EventArgs e)
        {
            if (markeroverlay.Markers.Count == 0)
            {
                CustomMessageBox.Show(Strings.Invalid_home_location);
                return;
            }

            var exepath = CheckandGetSITLImage("ArduRover.elf");
            try
            {
                StartSITL(await exepath, "rover",
                    BuildHomeLocation(markeroverlay.Markers[0].Position, (int) NUM_heading.Value), "",
                    (int) num_simspeed.Value);
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Failed to download and start sitl\n" + ex.ToString());
            }
        }

        private async void pictureBoxquad_Click(object sender, EventArgs e)
        {
            if (markeroverlay.Markers.Count == 0)
            {
                CustomMessageBox.Show(Strings.Invalid_home_location);
                return;
            }

            var exepath = CheckandGetSITLImage("ArduCopter.elf");
            try
            {
                StartSITL(await exepath, "+",
                    BuildHomeLocation(markeroverlay.Markers[0].Position, (int) NUM_heading.Value), "",
                    (int) num_simspeed.Value);
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Failed to download and start sitl\n" + ex.ToString());
            }
        }

        private async void pictureBoxheli_Click(object sender, EventArgs e)
        {
            if (markeroverlay.Markers.Count == 0)
            {
                CustomMessageBox.Show(Strings.Invalid_home_location);
                return;
            }

            var exepath = CheckandGetSITLImage("ArduHeli.elf");
            try
            {
                StartSITL(await exepath, "heli",
                    BuildHomeLocation(markeroverlay.Markers[0].Position, (int) NUM_heading.Value), "",
                    (int) num_simspeed.Value);
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Failed to download and start sitl\n" + ex.ToString());
            }
        }

        string BuildHomeLocation(PointLatLng homelocation, int heading = 0)
        {
            return String.Format("{0},{1},{2},{3}", homelocation.Lat.ToString(CultureInfo.InvariantCulture), homelocation.Lng.ToString(CultureInfo.InvariantCulture),
                srtm.getAltitude(homelocation.Lat, homelocation.Lng).alt.ToString(CultureInfo.InvariantCulture), heading.ToString(CultureInfo.InvariantCulture));
        }

        [DllImport("libc", SetLastError = true)]
        private static extern int chmod(string pathname, int mode);

        // user permissions
        const int S_IRUSR = 0x100;
        const int S_IWUSR = 0x80;
        const int S_IXUSR = 0x40;

        // group permission
        const int S_IRGRP = 0x20;
        const int S_IWGRP = 0x10;
        const int S_IXGRP = 0x8;

        // other permissions
        const int S_IROTH = 0x4;
        const int S_IWOTH = 0x2;
        const int S_IXOTH = 0x1;

        /// <summary>
        /// Try BundlePath first, then arm manifest, then cygwin on server
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private async Task<string> CheckandGetSITLImage(string filename)
        {
            if (BundledPath != "")
            {
                filename = filename.Replace(".elf", "");
                var file = filename;
                if (!File.Exists(BundledPath + System.IO.Path.DirectorySeparatorChar + file))
                {
                    string[] checks = new string[] { "{0}", "{0}.exe", "lib{0}.so", "{0}.so", "{0}.elf" };

                    foreach (var template in checks)
                    {
                        file = String.Format(template, filename);
                        log.Info("try path " + BundledPath + System.IO.Path.DirectorySeparatorChar + file);
                        if (File.Exists(BundledPath + System.IO.Path.DirectorySeparatorChar + file))
                        {
                            return BundledPath + System.IO.Path.DirectorySeparatorChar + file;
                        }
                        file = file.ToLower();
                        log.Info("try path " + BundledPath + System.IO.Path.DirectorySeparatorChar + file);
                        if (File.Exists(BundledPath + System.IO.Path.DirectorySeparatorChar + file))
                        {
                            return BundledPath + System.IO.Path.DirectorySeparatorChar + file;
                        }
                    }
                }

                return "";
            }

            if ((RuntimeInformation.OSArchitecture == Architecture.X64 ||
              RuntimeInformation.OSArchitecture == Architecture.X86) && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var type = APFirmware.MAV_TYPE.Copter;
                if (filename.ToLower().Contains("copter"))
                    type = APFirmware.MAV_TYPE.Copter;
                if (filename.ToLower().Contains("plane"))
                    type = APFirmware.MAV_TYPE.FIXED_WING;
                if (filename.ToLower().Contains("rover"))
                    type = APFirmware.MAV_TYPE.GROUND_ROVER;
                if (filename.ToLower().Contains("heli"))
                    type = APFirmware.MAV_TYPE.HELICOPTER;

                var fw = APFirmware.GetOptions(new DeviceInfo() { board = "", hardwareid = "" }, APFirmware.RELEASE_TYPES.OFFICIAL, type);
                fw = fw.Where(a => a.Platform == "SITL_x86_64_linux_gnu").ToList();
                if (fw.Count > 0)
                {
                    var path = sitldirectory + Path.GetFileNameWithoutExtension(filename);
                    if (!chk_skipdownload.Checked)
                    {
                        Download.getFilefromNet(fw.First().Url.AbsoluteUri, path);
                        try
                        {
                            int _0755 = S_IRUSR | S_IXUSR | S_IWUSR
                                | S_IRGRP | S_IXGRP
                                | S_IROTH | S_IXOTH;

                            chmod(path, _0755);
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex);
                        }
                    }
                    return path;
                }
            }

            if (RuntimeInformation.OSArchitecture == Architecture.Arm ||
               RuntimeInformation.OSArchitecture == Architecture.Arm64)
            {
                var type = APFirmware.MAV_TYPE.Copter;
                if (filename.ToLower().Contains("copter"))
                    type = APFirmware.MAV_TYPE.Copter;
                if (filename.ToLower().Contains("plane"))
                    type = APFirmware.MAV_TYPE.FIXED_WING;
                if (filename.ToLower().Contains("rover"))
                    type = APFirmware.MAV_TYPE.GROUND_ROVER;
                if (filename.ToLower().Contains("heli"))
                    type = APFirmware.MAV_TYPE.HELICOPTER;

                var fw = APFirmware.GetOptions(new DeviceInfo() { board = "", hardwareid="" }, APFirmware.RELEASE_TYPES.OFFICIAL, type);
                fw = fw.Where(a => a.Platform == "SITL_arm_linux_gnueabihf").ToList();
                if (fw.Count > 0)
                {
                    var path = sitldirectory + Path.GetFileNameWithoutExtension(filename);
                    if (!chk_skipdownload.Checked)
                    {
                        Download.getFilefromNet(fw.First().Url.AbsoluteUri, path);
                        try {
                            int _0755 =            S_IRUSR | S_IXUSR | S_IWUSR
                                | S_IRGRP | S_IXGRP
                                | S_IROTH | S_IXOTH;

                            chmod(path, _0755);
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex);
                        }
                    }
                    return path;
                }
            }

            if (!chk_skipdownload.Checked)
            {
                // kill old session - so we can overwrite if needed
                try
                {
                    simulator.ForEach(a =>
                    {
                        try
                        {
                            a.Kill();
                        }
                        catch { }
                    });
                }
                catch
                {
                }

                var url = sitlmasterurl;
                var result = CustomMessageBox.Show("Select the version you want to use?", "Select your version", CustomMessageBox.MessageBoxButtons.YesNo, CustomMessageBox.MessageBoxIcon.Question, "Latest(Dev)", "Stable");

                if(result == CustomMessageBox.DialogResult.Yes)
                {
                    // master by default
                }
                else if (result == CustomMessageBox.DialogResult.No)
                {
                    if (filename.ToLower().Contains("copter"))
                        url = sitlcopterstableurl;
                    if (filename.ToLower().Contains("rover"))
                        url = sitlroverstableurl;
                    if (filename.ToLower().Contains("plane"))
                        url = sitlplanestableurl;
                    if (filename.ToLower().Contains("heli"))
                        url = sitlcopterstableurl;
                } else
                {
                    return null;
                }

                Uri fullurl = new Uri(url, filename);

                var load = Common.LoadingBox("Downloading", "Downloading sitl software");

                var t1 = Download.getFilefromNetAsync(fullurl.ToString(),
                    sitldirectory + Path.GetFileNameWithoutExtension(filename) + ".exe");

                load.Refresh();

                var files = new string[] { "cygatomic-1.dll",
                    "cyggcc_s-1.dll",
                    "cyggomp-1.dll",
                    "cygquadmath-0.dll",
                    "cygssp-0.dll",
                    "cygstdc++-6.dll",
                    "cygwin1.dll"
                };

                // dependancys

                Parallel.ForEach(files, new ParallelOptions() { MaxDegreeOfParallelism = 2 }, (a, b) =>
                {
                    var depurl = new Uri(url, a);
                    var t2 = Download.getFilefromNet(depurl.ToString(), sitldirectory + depurl.Segments[depurl.Segments.Length - 1]);
                });

                await t1;

                load.Close();
            }

            return sitldirectory + Path.GetFileNameWithoutExtension(filename) + ".exe";
        }

        private async Task<string> GetDefaultConfig(string model)
        {
            if (await Download.getFilefromNetAsync(
                    "https://raw.githubusercontent.com/ArduPilot/ardupilot/master/Tools/autotest/sim_vehicle.py",
                    sitldirectory + "sim_vehicle.py").ConfigureAwait(false) || File.Exists(sitldirectory + "sim_vehicle.py"))
            {
                var matches = default_params_regex.Matches(File.ReadAllText(sitldirectory + "sim_vehicle.py"));

                foreach (Match match in matches)
                {
                    if (match.Groups[1].Value.ToLower().Equals(model))
                    {
                        if (await Download.getFilefromNetAsync(
                                "https://raw.githubusercontent.com/ArduPilot/ardupilot/master/Tools/autotest/" +
                                match.Groups[2].Value.ToString(),
                                sitldirectory + match.Groups[2].Value.ToString()).ConfigureAwait(false) || File.Exists(sitldirectory + match.Groups[2].Value.ToString()))
                            return sitldirectory + match.Groups[2].Value.ToString();
                    }
                }
            }

            if (await Download.getFilefromNetAsync(
                    "https://firmware.ardupilot.org/Tools/MissionPlanner/vehicleinfo.py",
                    sitldirectory + "vehicleinfo.py").ConfigureAwait(false) || File.Exists(sitldirectory + "vehicleinfo.py"))
            {
                try
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
                                Directory.CreateDirectory(Path.GetDirectoryName(sitldirectory + configs.ToString()));

                                if (await Download.getFilefromNetAsync(
                                        "https://raw.githubusercontent.com/ArduPilot/ardupilot/master/Tools/autotest/" +
                                        configs.ToString(),
                                        sitldirectory + configs.ToString()).ConfigureAwait(false) ||
                                    File.Exists(sitldirectory + configs.ToString()))
                                {
                                    return sitldirectory + configs.ToString();
                                }
                            }

                            string data = "";

                            foreach (var config1 in configs)
                            {
                                Directory.CreateDirectory(Path.GetDirectoryName(sitldirectory + config1.ToString()));

                                if (await Download.getFilefromNetAsync(
                                        "https://raw.githubusercontent.com/ArduPilot/ardupilot/master/Tools/autotest/" +
                                        config1.ToString(),
                                        sitldirectory + config1.ToString()).ConfigureAwait(false) ||
                                    File.Exists(sitldirectory + config1.ToString()))
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
                catch (Exception ex)
                {
                    log.Error(ex);
                    Console.WriteLine(ex.ToString());
                }
            }
            return "";
        }

        void cleanupJson(string filename)
        {
            var content = File.ReadAllText(filename);

            var match = BraceMatch(content, '{', '}');

            match = Regex.Replace(match, @"#.*", "");

            match = Regex.Replace(match, @"True", "\"True\"");
            match = Regex.Replace(match, @"False", "\"False\"");

            // ensure any handles are closed
            GC.Collect();

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

        private async void StartSITL(string exepath, string model, string homelocation, string extraargs = "", int speedup = 1)
        {

            //If we got null, it means that the verison selection box was canceled.
            if (exepath == null) return;

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
                simulator.ForEach(a =>
                {
                    try
                    {
                        a.Kill();
                    }
                    catch { }
                });
            }
            catch
            {
            }

            // override default model
            if (cmb_model.Text != "")
                model = cmb_model.Text;

            var config = await GetDefaultConfig(model);

            if (!string.IsNullOrEmpty(config))
                extraargs += @" --defaults """ + config + @"""";

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
            Console.WriteLine("sitl: {0} {1} {2}", exestart.WorkingDirectory, exestart.FileName,
                exestart.Arguments);
            if (RuntimeInformation.OSArchitecture == Architecture.X64 ||
                RuntimeInformation.OSArchitecture == Architecture.X86)
            {
                exestart.UseShellExecute = true;

                try
                {
                    simulator.Add(System.Diagnostics.Process.Start(exestart));
                }
                catch (Exception ex)
                {
                    CustomMessageBox.Show("Failed to start the simulator\n" + ex.ToString(), Strings.ERROR);
                    return;
                }
            }
            else
            {

                exestart.UseShellExecute = false;
                exestart.RedirectStandardOutput = true;
                exestart.RedirectStandardError = true;

                try
                {
                    var proc = System.Diagnostics.Process.Start(exestart);
                    simulator.Add(proc);

                    proc.ErrorDataReceived += (sender, args) => { Console.WriteLine("SITL ERR: " + args.Data); };

                    proc.OutputDataReceived += (sender, args) => { Console.WriteLine("SITL: " + args.Data); };

                    proc.Exited += (sender, args) => { Console.WriteLine("SITL EXIT!"); };

                    proc.BeginOutputReadLine();
                    proc.BeginErrorReadLine();

                }
                catch (Exception ex)
                {
                    CustomMessageBox.Show("Failed to start the simulator\n" + ex.ToString(), Strings.ERROR);
                    return;
                }
            }

            await Task.Delay(2000);

            MainV2.instance.InvokeIfRequired(() =>
            {
                MainV2.View.ShowScreen(MainV2.View.screens[0].Name);
            });

            var client = new Comms.TcpSerial();

            try
            {
                client.client = new TcpClient("127.0.0.1", 5760);

                MainV2.comPort.BaseStream = client;

                SITLSEND = new UdpClient("127.0.0.1", 5501);

                await Task.Delay(200);

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
                byte[] rcreceiver = new byte[2 * 8];
                Array.ConstrainedCopy(BitConverter.GetBytes((ushort)MainV2.comPort.MAV.cs.rcoverridech1), 0, rcreceiver, 0, 2);
                Array.ConstrainedCopy(BitConverter.GetBytes((ushort)MainV2.comPort.MAV.cs.rcoverridech2), 0, rcreceiver, 2, 2);
                Array.ConstrainedCopy(BitConverter.GetBytes((ushort)MainV2.comPort.MAV.cs.rcoverridech3), 0, rcreceiver, 4, 2);
                Array.ConstrainedCopy(BitConverter.GetBytes((ushort)MainV2.comPort.MAV.cs.rcoverridech4), 0, rcreceiver, 6, 2);
                Array.ConstrainedCopy(BitConverter.GetBytes((ushort)MainV2.comPort.MAV.cs.rcoverridech5), 0, rcreceiver, 8, 2);
                Array.ConstrainedCopy(BitConverter.GetBytes((ushort)MainV2.comPort.MAV.cs.rcoverridech6), 0, rcreceiver, 10, 2);
                Array.ConstrainedCopy(BitConverter.GetBytes((ushort)MainV2.comPort.MAV.cs.rcoverridech7), 0, rcreceiver, 12, 2);
                Array.ConstrainedCopy(BitConverter.GetBytes((ushort)MainV2.comPort.MAV.cs.rcoverridech8), 0, rcreceiver, 14, 2);

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
                StartSwarmChain();
                return true;
            }

            if (keyData == (Keys.Control | Keys.D))
            {
                _ = StartSwarmSeperate(Firmwares.ArduCopter2);
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        public async Task StartSwarmSeperate(Firmwares firmware)
        {
            var max = 10;

            if (InputBox.Show("how many?", "how many?", ref max) != DialogResult.OK)
                return;

            // kill old session
            try
            {
                simulator.ForEach(a =>
                {
                    try
                    {
                        a.Kill();
                    }
                    catch { }
                });
            }
            catch
            {
            }
            Task<string> exepath;
            string model = "";
            if (firmware == Firmwares.ArduPlane)
            {
                exepath = CheckandGetSITLImage("ArduPlane.elf");
                model = "plane";
            } else
            if (firmware == Firmwares.ArduRover)
            {
                exepath = CheckandGetSITLImage("ArduRover.elf");
                model = "rover";
            }
            else // (firmware == Firmwares.ArduCopter2)
            {
                exepath = CheckandGetSITLImage("ArduCopter.elf");
                model = "+";
            }

            var config = await GetDefaultConfig(model);

            max--;

            for (int a = (int)max; a >= 0; a--)
            {
                var extra = " --disable-fgview ";

                if (!string.IsNullOrEmpty(config))
                    extra += @" --defaults """ + config + @",identity.parm"" -P SERIAL0_PROTOCOL=2 -P SERIAL1_PROTOCOL=2 ";

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
                        a, "" /*"--uartD tcpclient:127.0.0.1:" + (5770 + 10 * a)*/, a + 1,
                        BuildHomeLocation(home, (int)NUM_heading.Value), model);
                }

                string simdir = sitldirectory + model + (a + 1) + Path.DirectorySeparatorChar;

                Directory.CreateDirectory(simdir);

                File.WriteAllText(simdir + "identity.parm", String.Format(@"SERIAL0_PROTOCOL=2
SERIAL1_PROTOCOL=2
SYSID_THISMAV={0}
SIM_TERRAIN=0
TERRAIN_ENABLE=0
SCHED_LOOP_RATE=50
SIM_RATE_HZ=400
SIM_DRIFT_SPEED=0
SIM_DRIFT_TIME=0
", a + 1));

                string path = Environment.GetEnvironmentVariable("PATH");

                Environment.SetEnvironmentVariable("PATH", sitldirectory + ";" + simdir + ";" + path,
                    EnvironmentVariableTarget.Process);

                Environment.SetEnvironmentVariable("HOME", simdir, EnvironmentVariableTarget.Process);

                ProcessStartInfo exestart = new ProcessStartInfo();
                exestart.FileName = await exepath;
                exestart.Arguments = extra;
                exestart.WorkingDirectory = simdir;
                exestart.WindowStyle = ProcessWindowStyle.Minimized;
                exestart.UseShellExecute = true;

                simulator.Add(System.Diagnostics.Process.Start(exestart));

                await Task.Delay(100);
            }

            await Task.Delay(2000);

            MainV2.View.ShowScreen(MainV2.View.screens[0].Name);

            try
            {
                Parallel.For(0, max + 1, (a) =>
                //for (int a = (int)max; a >= 0; a--)
                {
                    var mav = new MAVLinkInterface();

                    var client = new Comms.TcpSerial();
                    try
                    {

                        client.client = new TcpClient("127.0.0.1", 5760 + (10 * (a)));
                    }
                    catch (Exception ex)
                    {
                        return;
                    }

                    mav.BaseStream = client;

                    //SITLSEND = new UdpClient("127.0.0.1", 5501);

                    Thread.Sleep(200);

                    this.BeginInvokeIfRequired(() =>
                    {
                        MainV2.instance.doConnect(mav, "preset", "5760", false);

                        lock (this)
                            MainV2.Comports.Add(mav);

                        try
                        {
                            _ = mav.getParamListMavftpAsync((byte)mav.sysidcurrent, (byte)mav.compidcurrent);
                        }
                        catch
                        {
                        }
                    });
                }
                );

                return;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                CustomMessageBox.Show(Strings.Failed_to_connect_to_SITL_instance +
                                      ex.InnerException?.Message, Strings.ERROR);
                return;
            }
        }

        public async void StartSwarmChain()
        {
            var max = 10;

            if (InputBox.Show("how many?", "how many?", ref max) != DialogResult.OK)
                return;

            // kill old session
            try
            {
                simulator.ForEach(a =>
                {
                    try
                    {
                        a.Kill();
                    }
                    catch { }
                });
            }
            catch
            {
            }

            var exepath = CheckandGetSITLImage("ArduCopter.elf");
            var model = "+";

            var config= await GetDefaultConfig(model);
            max--;

            for (int a = (int)max; a >= 0; a--)
            {
                var extra = " --disable-fgview ";

                if (!string.IsNullOrEmpty(config))
                    extra += @" --defaults """ + config + @",identity.parm"" -P SERIAL0_PROTOCOL=2 -P SERIAL1_PROTOCOL=2 ";

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
                        a, "--uartD tcpclient:127.0.0.1:" + (5772 + 10 * a), a + 1,
                        BuildHomeLocation(home, (int)NUM_heading.Value), model);
                }

                string simdir = sitldirectory + model + (a + 1) + Path.DirectorySeparatorChar;

                Directory.CreateDirectory(simdir);

                File.WriteAllText(simdir + "identity.parm", String.Format(@"SERIAL0_PROTOCOL=2
SERIAL1_PROTOCOL=2
SYSID_THISMAV={0}
SIM_TERRAIN=0
TERRAIN_ENABLE=0
SCHED_LOOP_RATE=50
SIM_RATE_HZ=400
SIM_DRIFT_SPEED=0
SIM_DRIFT_TIME=0
", a + 1));

                string path = Environment.GetEnvironmentVariable("PATH");

                Environment.SetEnvironmentVariable("PATH", sitldirectory + ";" + simdir + ";" + path,
                    EnvironmentVariableTarget.Process);

                Environment.SetEnvironmentVariable("HOME", simdir, EnvironmentVariableTarget.Process);

                ProcessStartInfo exestart = new ProcessStartInfo();
                exestart.FileName = await exepath;
                exestart.Arguments = extra;
                exestart.WorkingDirectory = simdir;
                exestart.WindowStyle = ProcessWindowStyle.Minimized;
                exestart.UseShellExecute = true;

                File.AppendAllText(Settings.GetUserDataDirectory() + "sitl.bat",
                    "mkdir " + (a + 1) + "\ncd " + (a + 1) + "\n" + @"""" + await exepath + @"""" + " " + extra + " &\n");

                File.AppendAllText(Settings.GetUserDataDirectory() + "sitl1.sh",
                    "mkdir " + (a + 1) + "\ncd " + (a + 1) + "\n" + @"""../" +
                    Path.GetFileName(await exepath).Replace("C:", "/mnt/c").Replace("\\", "/").Replace(".exe", ".elf") + @"""" + " " +
                    extra.Replace("C:", "/mnt/c").Replace("\\", "/") + " &\nsleep .3\ncd ..\n");

                simulator.Add(System.Diagnostics.Process.Start(exestart));
            }

            System.Threading.Thread.Sleep(2000);

            MainV2.View.ShowScreen(MainV2.View.screens[0].Name);

            try
            {
                var client = new Comms.TcpSerial();

                client.client = new TcpClient("127.0.0.1", 5760);

                MainV2.comPort.BaseStream = client;

                SITLSEND = new UdpClient("127.0.0.1", 5501);

                Thread.Sleep(200);

                this.BeginInvokeIfRequired(() =>
                {
                    MainV2.instance.doConnect(MainV2.comPort, "preset", "5760", false);
                    try
                    {
                        _ = MainV2.comPort.getParamListMavftpAsync((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent);
                    }
                    catch
                    {
                    }
                });

                return;
            }
            catch
            {
                CustomMessageBox.Show(Strings.Failed_to_connect_to_SITL_instance, Strings.ERROR);
                return;
            }
        }

        private void but_swarmseq_Click(object sender, EventArgs e)
        {
             StartSwarmChain();
        }

        private void but_swarmlink_Click(object sender, EventArgs e)
        {
            _ = StartSwarmSeperate(Firmwares.ArduCopter2);
        }

        private void but_swarmplane_Click(object sender, EventArgs e)
        {
            _ = StartSwarmSeperate(Firmwares.ArduPlane);
        }

        private void but_swarmrover_Click(object sender, EventArgs e)
        {
            _ = StartSwarmSeperate(Firmwares.ArduRover);
        }
    }
}