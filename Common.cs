using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using AGaugeApp;
using System.IO.Ports;
using System.Threading;
using MissionPlanner.Attributes;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System.Security.Cryptography.X509Certificates;
using System.Net;
using System.Net.Sockets;
using System.Xml; // config file
using System.Runtime.InteropServices; // dll imports
using log4net;
using ZedGraph; // Graphs
using MissionPlanner;
using System.Reflection;
using MissionPlanner.Utilities;
using System.IO;
using System.Drawing.Drawing2D;
using ProjNet.CoordinateSystems.Transformations;
using ProjNet.CoordinateSystems;

namespace MissionPlanner
{
    /// <summary>
    /// used to override the drawing of the waypoint box bounding
    /// </summary>
    [Serializable]
    public class GMapMarkerRect : GMapMarker
    {
        public Pen Pen = new Pen(Brushes.White, 2);

        public Color Color
        {
            get { return Pen.Color; }
            set
            {
                if (!initcolor.HasValue) initcolor = value;
                Pen.Color = value;
            }
        }

        Color? initcolor = null;

        public GMapMarker InnerMarker;

        public int wprad = 0;

        public void ResetColor()
        {
            if (initcolor.HasValue)
                Color = initcolor.Value;
            else
                Color = Color.White;
        }

        public GMapMarkerRect(PointLatLng p)
            : base(p)
        {
            Pen.DashStyle = DashStyle.Dash;

            // do not forget set Size of the marker
            // if so, you shall have no event on it ;}
            Size = new System.Drawing.Size(50, 50);
            Offset = new System.Drawing.Point(-Size.Width/2, -Size.Height/2 - 20);
        }

        public override void OnRender(Graphics g)
        {
            base.OnRender(g);

            if (wprad == 0 || Overlay.Control == null)
                return;

            // if we have drawn it, then keep that color
            if (!initcolor.HasValue)
                Color = Color.White;

            // undo autochange in mouse over
            //if (Pen.Color == Color.Blue)
            //  Pen.Color = Color.White;

            double width =
                (Overlay.Control.MapProvider.Projection.GetDistance(Overlay.Control.FromLocalToLatLng(0, 0),
                    Overlay.Control.FromLocalToLatLng(Overlay.Control.Width, 0))*1000.0);
            double height =
                (Overlay.Control.MapProvider.Projection.GetDistance(Overlay.Control.FromLocalToLatLng(0, 0),
                    Overlay.Control.FromLocalToLatLng(Overlay.Control.Height, 0))*1000.0);
            double m2pixelwidth = Overlay.Control.Width/width;
            double m2pixelheight = Overlay.Control.Height/height;

            GPoint loc = new GPoint((int) (LocalPosition.X - (m2pixelwidth*wprad*2)), LocalPosition.Y);
                // MainMap.FromLatLngToLocal(wpradposition);

            if (m2pixelheight > 0.5)
                g.DrawArc(Pen,
                    new System.Drawing.Rectangle(
                        LocalPosition.X - Offset.X - (int) (Math.Abs(loc.X - LocalPosition.X)/2),
                        LocalPosition.Y - Offset.Y - (int) Math.Abs(loc.X - LocalPosition.X)/2,
                        (int) Math.Abs(loc.X - LocalPosition.X), (int) Math.Abs(loc.X - LocalPosition.X)), 0, 360);
        }
    }

    [Serializable]
    public class GMapMarkerADSBPlane : GMapMarker
    {
        const float rad2deg = (float) (180/Math.PI);
        const float deg2rad = (float) (1.0/rad2deg);

        private readonly Bitmap icon = global::MissionPlanner.Properties.Resources.FW_icons_2013_logos_01;

        float heading = 0;

        public GMapMarkerADSBPlane(PointLatLng p, float heading)
            : base(p)
        {
            icon = new Bitmap(icon, new Size(40, 40));
            this.heading = heading;
            Size = icon.Size;
        }

        public override void OnRender(Graphics g)
        {
            Matrix temp = g.Transform;
            g.TranslateTransform(LocalPosition.X, LocalPosition.Y);

            g.RotateTransform(-Overlay.Control.Bearing);

            try
            {
                g.RotateTransform(heading);
            }
            catch
            {
            }
            g.DrawImageUnscaled(icon, icon.Width/-2, icon.Height/-2);

            g.Transform = temp;
        }
    }

    [Serializable]
    public class GMapMarkerWP : GMarkerGoogle
    {
        string wpno = "";
        public bool selected = false;
        SizeF txtsize = SizeF.Empty;

        public GMapMarkerWP(PointLatLng p, string wpno)
            : base(p, GMarkerGoogleType.green)
        {
            this.wpno = wpno;
        }

        public override void OnRender(Graphics g)
        {
            if (selected)
            {
                g.FillEllipse(Brushes.Red, new Rectangle(this.LocalPosition, this.Size));
                g.DrawArc(Pens.Red, new Rectangle(this.LocalPosition, this.Size), 0, 360);
            }

            base.OnRender(g);

            var midw = LocalPosition.X + 10;
            var midh = LocalPosition.Y + 3;

            if (txtsize.Width > 15)
                midw -= 4;

            if (IsMouseOver)
            {
                if (txtsize == SizeF.Empty)
                    txtsize = g.MeasureString(wpno, SystemFonts.DefaultFont);

                g.DrawString(wpno, SystemFonts.DefaultFont, Brushes.Black, new PointF(midw, midh));
            }
        }
    }

    [Serializable]
    public class GMapMarkerRover : GMapMarker
    {
        const float rad2deg = (float) (180/Math.PI);
        const float deg2rad = (float) (1.0/rad2deg);

        static readonly System.Drawing.Size SizeSt =
            new System.Drawing.Size(global::MissionPlanner.Properties.Resources.rover.Width,
                global::MissionPlanner.Properties.Resources.rover.Height);

        float heading = 0;
        float cog = -1;
        float target = -1;
        float nav_bearing = -1;

        public GMapMarkerRover(PointLatLng p, float heading, float cog, float nav_bearing, float target)
            : base(p)
        {
            this.heading = heading;
            this.cog = cog;
            this.target = target;
            this.nav_bearing = nav_bearing;
            Size = SizeSt;
        }

        public override void OnRender(Graphics g)
        {
            Matrix temp = g.Transform;
            g.TranslateTransform(LocalPosition.X, LocalPosition.Y);

            g.RotateTransform(-Overlay.Control.Bearing);

            int length = 500;
            // anti NaN
            try
            {
                g.DrawLine(new Pen(Color.Red, 2), 0.0f, 0.0f, (float) Math.Cos((heading - 90)*deg2rad)*length,
                    (float) Math.Sin((heading - 90)*deg2rad)*length);
            }
            catch
            {
            }
            g.DrawLine(new Pen(Color.Green, 2), 0.0f, 0.0f, (float) Math.Cos((nav_bearing - 90)*deg2rad)*length,
                (float) Math.Sin((nav_bearing - 90)*deg2rad)*length);
            g.DrawLine(new Pen(Color.Black, 2), 0.0f, 0.0f, (float) Math.Cos((cog - 90)*deg2rad)*length,
                (float) Math.Sin((cog - 90)*deg2rad)*length);
            g.DrawLine(new Pen(Color.Orange, 2), 0.0f, 0.0f, (float) Math.Cos((target - 90)*deg2rad)*length,
                (float) Math.Sin((target - 90)*deg2rad)*length);
            // anti NaN

            try
            {
                g.RotateTransform(heading);
            }
            catch
            {
            }
            g.DrawImageUnscaled(global::MissionPlanner.Properties.Resources.rover,
                global::MissionPlanner.Properties.Resources.rover.Width/-2,
                global::MissionPlanner.Properties.Resources.rover.Height/-2);

            g.Transform = temp;
        }
    }

    [Serializable]
    public class GMapMarkerPlane : GMapMarker
    {
        const float rad2deg = (float) (180/Math.PI);
        const float deg2rad = (float) (1.0/rad2deg);

        private readonly Bitmap icon = global::MissionPlanner.Properties.Resources.planeicon;

        float heading = 0;
        float cog = -1;
        float target = -1;
        float nav_bearing = -1;

        public GMapMarkerPlane(PointLatLng p, float heading, float cog, float nav_bearing, float target)
            : base(p)
        {
            this.heading = heading;
            this.cog = cog;
            this.target = target;
            this.nav_bearing = nav_bearing;
            Size = icon.Size;
        }

        public override void OnRender(Graphics g)
        {
            Matrix temp = g.Transform;
            g.TranslateTransform(LocalPosition.X, LocalPosition.Y);

            g.RotateTransform(-Overlay.Control.Bearing);

            int length = 500;
            // anti NaN
            try
            {
                g.DrawLine(new Pen(Color.Red, 2), 0.0f, 0.0f, (float) Math.Cos((heading - 90)*deg2rad)*length,
                    (float) Math.Sin((heading - 90)*deg2rad)*length);
            }
            catch
            {
            }
            g.DrawLine(new Pen(Color.Green, 2), 0.0f, 0.0f, (float) Math.Cos((nav_bearing - 90)*deg2rad)*length,
                (float) Math.Sin((nav_bearing - 90)*deg2rad)*length);
            g.DrawLine(new Pen(Color.Black, 2), 0.0f, 0.0f, (float) Math.Cos((cog - 90)*deg2rad)*length,
                (float) Math.Sin((cog - 90)*deg2rad)*length);
            g.DrawLine(new Pen(Color.Orange, 2), 0.0f, 0.0f, (float) Math.Cos((target - 90)*deg2rad)*length,
                (float) Math.Sin((target - 90)*deg2rad)*length);
            // anti NaN
            try
            {
                float desired_lead_dist = 100;


                double width =
                    (Overlay.Control.MapProvider.Projection.GetDistance(Overlay.Control.FromLocalToLatLng(0, 0),
                        Overlay.Control.FromLocalToLatLng(Overlay.Control.Width, 0))*1000.0);
                double m2pixelwidth = Overlay.Control.Width/width;

                float alpha = ((desired_lead_dist*(float) m2pixelwidth)/MainV2.comPort.MAV.cs.radius)*rad2deg;

                if (MainV2.comPort.MAV.cs.radius < -1)
                {
                    // fixme 

                    float p1 = (float) Math.Cos((cog)*deg2rad)*MainV2.comPort.MAV.cs.radius +
                               MainV2.comPort.MAV.cs.radius;

                    float p2 = (float) Math.Sin((cog)*deg2rad)*MainV2.comPort.MAV.cs.radius +
                               MainV2.comPort.MAV.cs.radius;

                    g.DrawArc(new Pen(Color.HotPink, 2), p1, p2, Math.Abs(MainV2.comPort.MAV.cs.radius)*2,
                        Math.Abs(MainV2.comPort.MAV.cs.radius)*2, cog, alpha);
                }

                else if (MainV2.comPort.MAV.cs.radius > 1)
                {
                    // correct

                    float p1 = (float) Math.Cos((cog - 180)*deg2rad)*MainV2.comPort.MAV.cs.radius +
                               MainV2.comPort.MAV.cs.radius;

                    float p2 = (float) Math.Sin((cog - 180)*deg2rad)*MainV2.comPort.MAV.cs.radius +
                               MainV2.comPort.MAV.cs.radius;

                    g.DrawArc(new Pen(Color.HotPink, 2), -p1, -p2, MainV2.comPort.MAV.cs.radius*2,
                        MainV2.comPort.MAV.cs.radius*2, cog - 180, alpha);
                }
            }

            catch
            {
            }


            try
            {
                g.RotateTransform(heading);
            }
            catch
            {
            }
            g.DrawImageUnscaled(icon, icon.Width/-2, icon.Height/-2);

            g.Transform = temp;
        }
    }

    [Serializable]
    public class GMapMarkerQuad : GMapMarker
    {
        const float rad2deg = (float) (180/Math.PI);
        const float deg2rad = (float) (1.0/rad2deg);

        private readonly Bitmap icon = global::MissionPlanner.Properties.Resources.quadicon;

        float heading = 0;
        float cog = -1;
        float target = -1;
        private int sysid = -1;

        public GMapMarkerQuad(PointLatLng p, float heading, float cog, float target, int sysid)
            : base(p)
        {
            this.heading = heading;
            this.cog = cog;
            this.target = target;
            this.sysid = sysid;
            Size = icon.Size;
        }

        public override void OnRender(Graphics g)
        {
            Matrix temp = g.Transform;
            g.TranslateTransform(LocalPosition.X, LocalPosition.Y);

            int length = 500;
            // anti NaN
            try
            {
                g.DrawLine(new Pen(Color.Red, 2), 0.0f, 0.0f, (float) Math.Cos((heading - 90)*deg2rad)*length,
                    (float) Math.Sin((heading - 90)*deg2rad)*length);
            }
            catch
            {
            }
            //g.DrawLine(new Pen(Color.Green, 2), 0.0f, 0.0f, (float)Math.Cos((nav_bearing - 90) * deg2rad) * length, (float)Math.Sin((nav_bearing - 90) * deg2rad) * length);
            g.DrawLine(new Pen(Color.Black, 2), 0.0f, 0.0f, (float) Math.Cos((cog - 90)*deg2rad)*length,
                (float) Math.Sin((cog - 90)*deg2rad)*length);
            g.DrawLine(new Pen(Color.Orange, 2), 0.0f, 0.0f, (float) Math.Cos((target - 90)*deg2rad)*length,
                (float) Math.Sin((target - 90)*deg2rad)*length);
            // anti NaN
            try
            {
                g.RotateTransform(heading);
            }
            catch
            {
            }

            g.DrawImageUnscaled(icon, icon.Width/-2 + 2, icon.Height/-2);

            g.DrawString(sysid.ToString(), new Font(FontFamily.GenericMonospace, 15, FontStyle.Bold), Brushes.Red, -8,
                -8);

            g.Transform = temp;
        }
    }

    [Serializable]
    public class GMapMarkerHeli : GMapMarker
    {
        const float rad2deg = (float) (180/Math.PI);
        const float deg2rad = (float) (1.0/rad2deg);

        private readonly Bitmap icon = global::MissionPlanner.Properties.Resources.heli;

        float heading = 0;
        float cog = -1;
        float target = -1;

        public GMapMarkerHeli(PointLatLng p, float heading, float cog, float target)
            : base(p)
        {
            this.heading = heading;
            this.cog = cog;
            this.target = target;
            Size = icon.Size;
        }

        public override void OnRender(Graphics g)
        {
            Matrix temp = g.Transform;
            g.TranslateTransform(LocalPosition.X, LocalPosition.Y);

            int length = 500;
            // anti NaN
            try
            {
                g.DrawLine(new Pen(Color.Red, 2), 0.0f, 0.0f, (float) Math.Cos((heading - 90)*deg2rad)*length,
                    (float) Math.Sin((heading - 90)*deg2rad)*length);
            }
            catch
            {
            }
            //g.DrawLine(new Pen(Color.Green, 2), 0.0f, 0.0f, (float)Math.Cos((nav_bearing - 90) * deg2rad) * length, (float)Math.Sin((nav_bearing - 90) * deg2rad) * length);
            g.DrawLine(new Pen(Color.Black, 2), 0.0f, 0.0f, (float) Math.Cos((cog - 90)*deg2rad)*length,
                (float) Math.Sin((cog - 90)*deg2rad)*length);
            g.DrawLine(new Pen(Color.Orange, 2), 0.0f, 0.0f, (float) Math.Cos((target - 90)*deg2rad)*length,
                (float) Math.Sin((target - 90)*deg2rad)*length);
            // anti NaN
            try
            {
                g.RotateTransform(heading);
            }
            catch
            {
            }
            g.DrawImageUnscaled(icon, icon.Width/-2 + 2, icon.Height/-2);

            g.Transform = temp;
        }
    }

    [Serializable]
    public class GMapMarkerAntennaTracker : GMapMarker
    {
        const float rad2deg = (float) (180/Math.PI);
        const float deg2rad = (float) (1.0/rad2deg);

        private readonly Bitmap icon = global::MissionPlanner.Properties.Resources.Antenna_Tracker_01;

        float heading = 0;
        private float target = 0;

        public GMapMarkerAntennaTracker(PointLatLng p, float heading, float target)
            : base(p)
        {
            Size = icon.Size;
            this.heading = heading;
            this.target = target;
        }

        public override void OnRender(Graphics g)
        {
            Matrix temp = g.Transform;
            g.TranslateTransform(LocalPosition.X, LocalPosition.Y);

            int length = 500;

            try
            {
                // heading
                g.DrawLine(new Pen(Color.Red, 2), 0.0f, 0.0f, (float) Math.Cos((heading - 90)*deg2rad)*length,
                    (float) Math.Sin((heading - 90)*deg2rad)*length);

                // target
                g.DrawLine(new Pen(Color.Orange, 2), 0.0f, 0.0f, (float) Math.Cos((target - 90)*deg2rad)*length,
                    (float) Math.Sin((target - 90)*deg2rad)*length);
            }
            catch
            {
            }

            g.DrawImage(icon, -20, -20, 40, 40);

            g.Transform = temp;
        }
    }


    class NoCheckCertificatePolicy : ICertificatePolicy
    {
        public bool CheckValidationResult(ServicePoint srvPoint, X509Certificate certificate, WebRequest request,
            int certificateProblem)
        {
            return true;
        }
    }


    public class Common
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public enum distances
        {
            Meters,
            Feet
        }

        public enum speeds
        {
            meters_per_second,
            fps,
            kph,
            mph,
            knots
        }


        /// <summary>
        /// from libraries\AP_Math\rotations.h
        /// </summary>
        public enum Rotation
        {
            ROTATION_NONE = 0,
            ROTATION_YAW_45,
            ROTATION_YAW_90,
            ROTATION_YAW_135,
            ROTATION_YAW_180,
            ROTATION_YAW_225,
            ROTATION_YAW_270,
            ROTATION_YAW_315,
            ROTATION_ROLL_180,
            ROTATION_ROLL_180_YAW_45,
            ROTATION_ROLL_180_YAW_90,
            ROTATION_ROLL_180_YAW_135,
            ROTATION_PITCH_180,
            ROTATION_ROLL_180_YAW_225,
            ROTATION_ROLL_180_YAW_270,
            ROTATION_ROLL_180_YAW_315,
            ROTATION_ROLL_90,
            ROTATION_ROLL_90_YAW_45,
            ROTATION_ROLL_90_YAW_90,
            ROTATION_ROLL_90_YAW_135,
            ROTATION_ROLL_270,
            ROTATION_ROLL_270_YAW_45,
            ROTATION_ROLL_270_YAW_90,
            ROTATION_ROLL_270_YAW_135,
            ROTATION_PITCH_90,
            ROTATION_PITCH_270,
            ROTATION_MAX
        }


        public enum ap_product
        {
            [DisplayText("HIL")] AP_PRODUCT_ID_NONE = 0x00, // Hardware in the loop
            [DisplayText("APM1 1280")] AP_PRODUCT_ID_APM1_1280 = 0x01, // APM1 with 1280 CPUs
            [DisplayText("APM1 2560")] AP_PRODUCT_ID_APM1_2560 = 0x02, // APM1 with 2560 CPUs
            [DisplayText("SITL")] AP_PRODUCT_ID_SITL = 0x03, // Software in the loop
            [DisplayText("PX4")] AP_PRODUCT_ID_PX4 = 0x04, // PX4 on NuttX
            [DisplayText("PX4 FMU 2")] AP_PRODUCT_ID_PX4_V2 = 0x05, // PX4 FMU2 on NuttX
            [DisplayText("APM2 ES C4")] AP_PRODUCT_ID_APM2ES_REV_C4 = 0x14, // APM2 with MPU6000ES_REV_C4
            [DisplayText("APM2 ES C5")] AP_PRODUCT_ID_APM2ES_REV_C5 = 0x15, // APM2 with MPU6000ES_REV_C5
            [DisplayText("APM2 ES D6")] AP_PRODUCT_ID_APM2ES_REV_D6 = 0x16, // APM2 with MPU6000ES_REV_D6
            [DisplayText("APM2 ES D7")] AP_PRODUCT_ID_APM2ES_REV_D7 = 0x17, // APM2 with MPU6000ES_REV_D7
            [DisplayText("APM2 ES D8")] AP_PRODUCT_ID_APM2ES_REV_D8 = 0x18, // APM2 with MPU6000ES_REV_D8	
            [DisplayText("APM2 C4")] AP_PRODUCT_ID_APM2_REV_C4 = 0x54, // APM2 with MPU6000_REV_C4 	
            [DisplayText("APM2 C5")] AP_PRODUCT_ID_APM2_REV_C5 = 0x55, // APM2 with MPU6000_REV_C5 	
            [DisplayText("APM2 D6")] AP_PRODUCT_ID_APM2_REV_D6 = 0x56, // APM2 with MPU6000_REV_D6 		
            [DisplayText("APM2 D7")] AP_PRODUCT_ID_APM2_REV_D7 = 0x57, // APM2 with MPU6000_REV_D7 	
            [DisplayText("APM2 D8")] AP_PRODUCT_ID_APM2_REV_D8 = 0x58, // APM2 with MPU6000_REV_D8 	
            [DisplayText("APM2 D9")] AP_PRODUCT_ID_APM2_REV_D9 = 0x59, // APM2 with MPU6000_REV_D9 
            [DisplayText("FlyMaple")] AP_PRODUCT_ID_FLYMAPLE = 0x100, // Flymaple with ITG3205, ADXL345, HMC5883, BMP085
            [DisplayText("Linux")] AP_PRODUCT_ID_L3G4200D = 0x101, // Linux with L3G4200D and ADXL345
        }

        public static bool getFilefromNet(string url, string saveto)
        {
            try
            {
                // this is for mono to a ssl server
                //ServicePointManager.CertificatePolicy = new NoCheckCertificatePolicy(); 

                ServicePointManager.ServerCertificateValidationCallback =
                    new System.Net.Security.RemoteCertificateValidationCallback(
                        (sender, certificate, chain, policyErrors) => { return true; });

                log.Info(url);
                // Create a request using a URL that can receive a post. 
                WebRequest request = WebRequest.Create(url);
                request.Timeout = 10000;
                // Set the Method property of the request to POST.
                request.Method = "GET";
                // Get the response.
                WebResponse response = request.GetResponse();
                // Display the status.
                log.Info(((HttpWebResponse) response).StatusDescription);
                if (((HttpWebResponse) response).StatusCode != HttpStatusCode.OK)
                    return false;

                if (File.Exists(saveto))
                {
                    DateTime lastfilewrite = new FileInfo(saveto).LastWriteTime;
                    DateTime lasthttpmod = ((HttpWebResponse) response).LastModified;

                    if (lasthttpmod < lastfilewrite)
                    {
                        if (((HttpWebResponse) response).ContentLength == new FileInfo(saveto).Length)
                        {
                            log.Info("got LastModified " + saveto + " " + ((HttpWebResponse) response).LastModified +
                                     " vs " + new FileInfo(saveto).LastWriteTime);
                            response.Close();
                            return true;
                        }
                    }
                }

                // Get the stream containing content returned by the server.
                Stream dataStream = response.GetResponseStream();

                long bytes = response.ContentLength;
                long contlen = bytes;

                byte[] buf1 = new byte[1024];

                if (!Directory.Exists(Path.GetDirectoryName(saveto)))
                    Directory.CreateDirectory(Path.GetDirectoryName(saveto));

                FileStream fs = new FileStream(saveto + ".new", FileMode.Create);

                DateTime dt = DateTime.Now;

                while (dataStream.CanRead && bytes > 0)
                {
                    Application.DoEvents();
                    log.Debug(saveto + " " + bytes);
                    int len = dataStream.Read(buf1, 0, buf1.Length);
                    bytes -= len;
                    fs.Write(buf1, 0, len);
                }

                fs.Close();
                dataStream.Close();
                response.Close();

                File.Delete(saveto);
                File.Move(saveto + ".new", saveto);

                return true;
            }
            catch (Exception ex)
            {
                log.Info("getFilefromNet(): " + ex.ToString());
                return false;
            }
        }

        public static List<KeyValuePair<int, string>> getModesList(CurrentState cs)
        {
            log.Info("getModesList Called");

            if (cs.firmware == MainV2.Firmwares.ArduPlane)
            {
                var flightModes = Utilities.ParameterMetaDataRepository.GetParameterOptionsInt("FLTMODE1",
                    cs.firmware.ToString());
                flightModes.Add(new KeyValuePair<int, string>(16, "INITIALISING"));
                return flightModes;
            }
            else if (cs.firmware == MainV2.Firmwares.Ateryx)
            {
                var flightModes = Utilities.ParameterMetaDataRepository.GetParameterOptionsInt("FLTMODE1",
                    cs.firmware.ToString()); //same as apm
                return flightModes;
            }
            else if (cs.firmware == MainV2.Firmwares.ArduCopter2)
            {
                var flightModes = Utilities.ParameterMetaDataRepository.GetParameterOptionsInt("FLTMODE1",
                    cs.firmware.ToString());
                return flightModes;
            }
            else if (cs.firmware == MainV2.Firmwares.ArduRover)
            {
                var flightModes = Utilities.ParameterMetaDataRepository.GetParameterOptionsInt("MODE1",
                    cs.firmware.ToString());
                return flightModes;
            }
            else if (cs.firmware == MainV2.Firmwares.ArduTracker)
            {
                var temp = new List<KeyValuePair<int, string>>();
                temp.Add(new KeyValuePair<int, string>(0, "MANUAL"));
                temp.Add(new KeyValuePair<int, string>(1, "STOP"));
                temp.Add(new KeyValuePair<int, string>(2, "SCAN"));
                temp.Add(new KeyValuePair<int, string>(3, "SERVO_TEST"));
                temp.Add(new KeyValuePair<int, string>(10, "AUTO"));
                temp.Add(new KeyValuePair<int, string>(16, "INITIALISING"));

                return temp;
            }

            return null;
        }

        public static Form LoadingBox(string title, string promptText)
        {
            Form form = new Form();
            System.Windows.Forms.Label label = new System.Windows.Forms.Label();
            System.ComponentModel.ComponentResourceManager resources =
                new System.ComponentModel.ComponentResourceManager(typeof (MainV2));
            form.Icon = ((System.Drawing.Icon) (resources.GetObject("$this.Icon")));

            form.Text = title;
            label.Text = promptText;

            label.SetBounds(9, 50, 372, 13);

            label.AutoSize = true;

            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] {label});
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;

            ThemeManager.ApplyThemeTo(form);

            form.Show();
            form.Refresh();
            label.Refresh();
            Application.DoEvents();
            return form;
        }

        public static DialogResult MessageShowAgain(string title, string promptText)
        {
            Form form = new Form();
            System.Windows.Forms.Label label = new System.Windows.Forms.Label();
            CheckBox chk = new CheckBox();
            Controls.MyButton buttonOk = new Controls.MyButton();
            System.ComponentModel.ComponentResourceManager resources =
                new System.ComponentModel.ComponentResourceManager(typeof (MainV2));
            form.Icon = ((System.Drawing.Icon) (resources.GetObject("$this.Icon")));

            form.Text = title;
            label.Text = promptText;

            chk.Tag = ("SHOWAGAIN_" + title.Replace(" ", "_").Replace('+', '_'));
            chk.AutoSize = true;
            chk.Text = Strings.ShowMeAgain;
            chk.Checked = true;
            chk.Location = new Point(9, 80);

            if (MainV2.config[(string) chk.Tag] != null && (string) MainV2.config[(string) chk.Tag] == "False")
                // skip it
            {
                form.Dispose();
                chk.Dispose();
                buttonOk.Dispose();
                label.Dispose();
                return DialogResult.OK;
            }

            chk.CheckStateChanged += new EventHandler(chk_CheckStateChanged);

            buttonOk.Text = Strings.OK;
            buttonOk.DialogResult = DialogResult.OK;
            buttonOk.Location = new Point(form.Right - 100, 80);

            label.SetBounds(9, 40, 372, 13);

            label.AutoSize = true;

            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] {label, chk, buttonOk});
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;

            ThemeManager.ApplyThemeTo(form);

            DialogResult dialogResult = form.ShowDialog();

            form.Dispose();

            form = null;

            return dialogResult;
        }

        static void chk_CheckStateChanged(object sender, EventArgs e)
        {
            MainV2.config[(string) ((CheckBox) (sender)).Tag] = ((CheckBox) (sender)).Checked.ToString();
        }

        public static string speechConversion(string input)
        {
            if (MainV2.comPort.MAV.cs.wpno == 0)
            {
                input = input.Replace("{wpn}", "Home");
            }
            else
            {
                input = input.Replace("{wpn}", MainV2.comPort.MAV.cs.wpno.ToString());
            }

            input = input.Replace("{asp}", MainV2.comPort.MAV.cs.airspeed.ToString("0"));

            input = input.Replace("{alt}", MainV2.comPort.MAV.cs.alt.ToString("0"));

            input = input.Replace("{wpa}", MainV2.comPort.MAV.cs.targetalt.ToString("0"));

            input = input.Replace("{gsp}", MainV2.comPort.MAV.cs.groundspeed.ToString("0"));

            input = input.Replace("{mode}", MainV2.comPort.MAV.cs.mode.ToString());

            input = input.Replace("{batv}", MainV2.comPort.MAV.cs.battery_voltage.ToString("0.00"));

            input = input.Replace("{batp}", (MainV2.comPort.MAV.cs.battery_remaining).ToString("0"));

            input = input.Replace("{vsp}", (MainV2.comPort.MAV.cs.verticalspeed).ToString("0.0"));

            input = input.Replace("{curr}", (MainV2.comPort.MAV.cs.current).ToString("0.0"));

            input = input.Replace("{hdop}", (MainV2.comPort.MAV.cs.gpshdop).ToString("0.00"));

            input = input.Replace("{satcount}", (MainV2.comPort.MAV.cs.satcount).ToString("0"));

            input = input.Replace("{rssi}", (MainV2.comPort.MAV.cs.rssi).ToString("0"));

            input = input.Replace("{disthome}", (MainV2.comPort.MAV.cs.DistToHome).ToString("0"));

            input = input.Replace("{timeinair}",
                (new TimeSpan(0, 0, 0, (int) MainV2.comPort.MAV.cs.timeInAir)).ToString());

            return input;
        }
    }
}