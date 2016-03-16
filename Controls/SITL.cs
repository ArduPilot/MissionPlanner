using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace MissionPlanner.Controls
{
    public partial class SITL : Form
    {
        Uri sitlurl = new Uri("http://firmware.ardupilot.org/Tools/MissionPlanner/sitl/");

        string sitldirectory = Application.StartupPath + Path.DirectorySeparatorChar + "sitl" +
                               Path.DirectorySeparatorChar;

        GMapOverlay markeroverlay;

        GMapMarkerWP homemarker = new GMapMarkerWP(new PointLatLng(-34.98106, 117.85201), "H");
        bool onmarker = false;
        bool mousedown = false;
        private PointLatLng MouseDownStart;

        internal static UdpClient SITLSEND;

        internal static System.Diagnostics.Process simulator;

        /*
        { "+",         MultiCopter::create },
    { "quad",      MultiCopter::create },
    { "copter",    MultiCopter::create },
    { "x",         MultiCopter::create },
    { "hexa",      MultiCopter::create },
    { "octa",      MultiCopter::create },
    { "heli",      Helicopter::create },
    { "rover",     Rover::create },
    { "crrcsim",   CRRCSim::create },
    { "jsbsim",    JSBSim::create },
    { "last_letter", last_letter::create }
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

            try
            {
                if (simulator != null)
                    simulator.Kill();
            }
            catch
            {
            }

            Utilities.ThemeManager.ApplyThemeTo(this);

            MissionPlanner.Utilities.Tracking.AddPage(this.GetType().ToString(), this.Text);
        }

        private void pictureBoxplane_Click(object sender, EventArgs e)
        {
            Common.MessageShowAgain("MS Visual C++ Runtime 2013",
                "Please note that the plane sim requires\n'Visual C++ Redistributable Packages for Visual Studio 2013' to run.\n https://www.microsoft.com/en-us/download/details.aspx?id=40784");

            var exepath = CheckandGetSITLImage("ArduPlane.elf");

            string simdir = sitldirectory + "jsbsim" + Path.DirectorySeparatorChar;
            string destfile = simdir + Path.DirectorySeparatorChar + "JSBSim.exe";

            if (!File.Exists(destfile))
            {
                Directory.CreateDirectory(simdir);
                File.Copy(Application.StartupPath + Path.DirectorySeparatorChar + "JSBSim.exe", destfile);
            }

            if (markeroverlay.Markers.Count == 0)
            {
                CustomMessageBox.Show(Strings.Invalid_home_location);
                return;
            }

            StartSITL(exepath, "jsbsim", BuildHomeLocation(markeroverlay.Markers[0].Position, (int) NUM_heading.Value),
                @" --autotest-dir """ + Application.StartupPath.Replace('\\', '/') + @"""", 1);
        }

        private void pictureBoxrover_Click(object sender, EventArgs e)
        {
            if (markeroverlay.Markers.Count == 0)
            {
                CustomMessageBox.Show(Strings.Invalid_home_location);
                return;
            }
            var exepath = CheckandGetSITLImage("APMrover2.elf");

            StartSITL(exepath, "rover", BuildHomeLocation(markeroverlay.Markers[0].Position, (int) NUM_heading.Value));
        }

        private void pictureBoxquad_Click(object sender, EventArgs e)
        {
            if (markeroverlay.Markers.Count == 0)
            {
                CustomMessageBox.Show(Strings.Invalid_home_location);
                return;
            }
            var exepath = CheckandGetSITLImage("ArduCopter.elf");

            StartSITL(exepath, "+", BuildHomeLocation(markeroverlay.Markers[0].Position, (int) NUM_heading.Value));
        }

        private void pictureBoxheli_Click(object sender, EventArgs e)
        {
            if (markeroverlay.Markers.Count == 0)
            {
                CustomMessageBox.Show(Strings.Invalid_home_location);
                return;
            }
            var exepath = CheckandGetSITLImage("ArduHeli.elf");

            StartSITL(exepath, "heli", BuildHomeLocation(markeroverlay.Markers[0].Position, (int) NUM_heading.Value));
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

            Common.getFilefromNet(fullurl.ToString(),
                sitldirectory + Path.GetFileNameWithoutExtension(filename) + ".exe");

            load.Refresh();

            // dependancys
            var depurl = new Uri(sitlurl, "cyggcc_s-1.dll");
            Common.getFilefromNet(depurl.ToString(), sitldirectory + depurl.Segments[depurl.Segments.Length - 1]);

            load.Refresh();
            depurl = new Uri(sitlurl, "cygstdc++-6.dll");
            Common.getFilefromNet(depurl.ToString(), sitldirectory + depurl.Segments[depurl.Segments.Length - 1]);

            load.Refresh();
            depurl = new Uri(sitlurl, "cygwin1.dll");
            Common.getFilefromNet(depurl.ToString(), sitldirectory + depurl.Segments[depurl.Segments.Length - 1]);

            load.Close();

            return sitldirectory + Path.GetFileNameWithoutExtension(filename) + ".exe";
        }

        private void StartSITL(string exepath, string model, string homelocation, string extraargs = "", int speedup = 1)
        {
            if (String.IsNullOrEmpty(homelocation))
            {
                CustomMessageBox.Show(Strings.Invalid_home_location, Strings.ERROR);
                return;
            }

            string simdir = sitldirectory + model + Path.DirectorySeparatorChar;

            Directory.CreateDirectory(simdir);

            string path = Environment.GetEnvironmentVariable("PATH");

            Environment.SetEnvironmentVariable("PATH", simdir + ";" + path, EnvironmentVariableTarget.Process);

            Environment.SetEnvironmentVariable("HOME", simdir, EnvironmentVariableTarget.Process);

            ProcessStartInfo exestart = new ProcessStartInfo();
            exestart.FileName = exepath;
            exestart.Arguments = String.Format("-M{0} -O{1} -s{2} {3}", model, homelocation, speedup, extraargs);
            exestart.WorkingDirectory = simdir;
            exestart.WindowStyle = ProcessWindowStyle.Minimized;
            exestart.UseShellExecute = true;

            simulator = System.Diagnostics.Process.Start(exestart);

            System.Threading.Thread.Sleep(2000);

            MainV2.View.ShowScreen(MainV2.View.screens[0].Name);

            var client = new Comms.TcpSerial();

            try
            {
                client.client = new TcpClient("127.0.0.1", 5760);

                MainV2.comPort.BaseStream = client;

                SITLSEND = new UdpClient("127.0.0.1", 5501);

                MainV2.instance.doConnect(MainV2.comPort, "preset", "5760");
            }
            catch
            {
                CustomMessageBox.Show(Strings.Failed_to_connect_to_SITL_instance, Strings.ERROR);
            }

            this.Close();
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
    }
}