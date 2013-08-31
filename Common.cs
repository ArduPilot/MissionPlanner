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
using ArdupilotMega.Attributes;
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
using ArdupilotMega;
using System.Reflection;
using ArdupilotMega.Utilities;

using System.IO;

using System.Drawing.Drawing2D;
using ProjNet.CoordinateSystems.Transformations;
using ProjNet.CoordinateSystems;

namespace ArdupilotMega
{

    /// <summary>
    /// used to override the drawing of the waypoint box bounding
    /// </summary>
    public class GMapMarkerRect : GMapMarker
    {
        public Pen Pen = new Pen(Brushes.White, 2);

        public Color Color { get { return Pen.Color; } set { Pen.Color = value; } }

        public GMapMarker InnerMarker;

        public int wprad = 0;
        public GMapControl MainMap;

        public GMapMarkerRect(PointLatLng p)
            : base(p)
        {
            Pen.DashStyle = DashStyle.Dash;

            // do not forget set Size of the marker
            // if so, you shall have no event on it ;}
            Size = new System.Drawing.Size(50, 50);
            Offset = new System.Drawing.Point(-Size.Width / 2, -Size.Height / 2 - 20);
        }

        public override void OnRender(Graphics g)
        {
            base.OnRender(g);

            if (wprad == 0 || MainMap == null)
                return;

            // undo autochange in mouse over
            if (Pen.Color == Color.Blue)
                Pen.Color = Color.White;

                double width = (MainMap.Manager.GetDistance(MainMap.FromLocalToLatLng(0, 0), MainMap.FromLocalToLatLng(MainMap.Width, 0)) * 1000.0);
                double height = (MainMap.Manager.GetDistance(MainMap.FromLocalToLatLng(0, 0), MainMap.FromLocalToLatLng(MainMap.Height, 0)) * 1000.0);
                double m2pixelwidth = MainMap.Width / width;
                double m2pixelheight = MainMap.Height / height;

            GPoint loc = new GPoint((int)(LocalPosition.X - (m2pixelwidth * wprad * 2)), LocalPosition.Y);// MainMap.FromLatLngToLocal(wpradposition);

            g.DrawArc(Pen, new System.Drawing.Rectangle(LocalPosition.X - Offset.X - (Math.Abs(loc.X - LocalPosition.X) / 2), LocalPosition.Y - Offset.Y - Math.Abs(loc.X - LocalPosition.X) / 2, Math.Abs(loc.X - LocalPosition.X), Math.Abs(loc.X - LocalPosition.X)), 0, 360);
       
        }
    }

    public class GMapMarkerRover : GMapMarker
    {
        const float rad2deg = (float)(180 / Math.PI);
        const float deg2rad = (float)(1.0 / rad2deg);

        static readonly System.Drawing.Size SizeSt = new System.Drawing.Size(global::MissionPlanner.Properties.Resources.rover.Width, global::MissionPlanner.Properties.Resources.rover.Height);
        float heading = 0;
        float cog = -1;
        float target = -1;
        float nav_bearing = -1;
        public GMapControl MainMap;

        public GMapMarkerRover(PointLatLng p, float heading, float cog, float nav_bearing, float target, GMapControl map)
            : base(p)
        {
            this.heading = heading;
            this.cog = cog;
            this.target = target;
            this.nav_bearing = nav_bearing;
            Size = SizeSt;
            MainMap = map;
        }

        public override void OnRender(Graphics g)
        {
            Matrix temp = g.Transform;
            g.TranslateTransform(LocalPosition.X, LocalPosition.Y);

            g.RotateTransform(-MainMap.Bearing);

            int length = 500;
            // anti NaN
            try
            {
                g.DrawLine(new Pen(Color.Red, 2), 0.0f, 0.0f, (float)Math.Cos((heading - 90) * deg2rad) * length, (float)Math.Sin((heading - 90) * deg2rad) * length);
            }
            catch { }
            g.DrawLine(new Pen(Color.Green, 2), 0.0f, 0.0f, (float)Math.Cos((nav_bearing - 90) * deg2rad) * length, (float)Math.Sin((nav_bearing - 90) * deg2rad) * length);
            g.DrawLine(new Pen(Color.Black, 2), 0.0f, 0.0f, (float)Math.Cos((cog - 90) * deg2rad) * length, (float)Math.Sin((cog - 90) * deg2rad) * length);
            g.DrawLine(new Pen(Color.Orange, 2), 0.0f, 0.0f, (float)Math.Cos((target - 90) * deg2rad) * length, (float)Math.Sin((target - 90) * deg2rad) * length);
            // anti NaN

            try
            {
                g.RotateTransform(heading);
            }
            catch { }
            g.DrawImageUnscaled(global::MissionPlanner.Properties.Resources.rover, global::MissionPlanner.Properties.Resources.rover.Width / -2, global::MissionPlanner.Properties.Resources.rover.Height / -2);

            g.Transform = temp;
        }
    }


    public class GMapMarkerPlane : GMapMarker
    {
        const float rad2deg = (float)(180 / Math.PI);
        const float deg2rad = (float)(1.0 / rad2deg);

        static readonly System.Drawing.Size SizeSt = new System.Drawing.Size(global::MissionPlanner.Properties.Resources.planeicon.Width, global::MissionPlanner.Properties.Resources.planeicon.Height);
        float heading = 0;
        float cog = -1;
        float target = -1;
        float nav_bearing = -1;
        public GMapControl MainMap;

        public GMapMarkerPlane(PointLatLng p, float heading, float cog, float nav_bearing,float target, GMapControl map)
            : base(p)
        {
            this.heading = heading;
            this.cog = cog;
            this.target = target;
            this.nav_bearing = nav_bearing;
            Size = SizeSt;
            MainMap = map;
        }

        public override void OnRender(Graphics g)
        {
            Matrix temp = g.Transform;
            g.TranslateTransform(LocalPosition.X, LocalPosition.Y);

            g.RotateTransform(-MainMap.Bearing);

            int length = 500;
// anti NaN
            try
            {
                g.DrawLine(new Pen(Color.Red, 2), 0.0f, 0.0f, (float)Math.Cos((heading - 90) * deg2rad) * length, (float)Math.Sin((heading - 90) * deg2rad) * length);
            }
            catch { }
            g.DrawLine(new Pen(Color.Green, 2), 0.0f, 0.0f, (float)Math.Cos((nav_bearing - 90) * deg2rad) * length, (float)Math.Sin((nav_bearing - 90) * deg2rad) * length);
            g.DrawLine(new Pen(Color.Black, 2), 0.0f, 0.0f, (float)Math.Cos((cog - 90) * deg2rad) * length, (float)Math.Sin((cog - 90) * deg2rad) * length);
            g.DrawLine(new Pen(Color.Orange, 2), 0.0f, 0.0f, (float)Math.Cos((target - 90) * deg2rad) * length, (float)Math.Sin((target - 90) * deg2rad) * length);
// anti NaN
            try
            {

                float desired_lead_dist = 100;


                double width = (MainMap.Manager.GetDistance(MainMap.FromLocalToLatLng(0, 0), MainMap.FromLocalToLatLng(MainMap.Width, 0)) * 1000.0);
                double m2pixelwidth = MainMap.Width / width;

                float alpha = ((desired_lead_dist * (float)m2pixelwidth) / MainV2.comPort.MAV.cs.radius) * rad2deg;

                if (MainV2.comPort.MAV.cs.radius < -1)
                {
                    // fixme 

                    float p1 = (float)Math.Cos((cog) * deg2rad) * MainV2.comPort.MAV.cs.radius + MainV2.comPort.MAV.cs.radius;

                    float p2 = (float)Math.Sin((cog) * deg2rad) * MainV2.comPort.MAV.cs.radius + MainV2.comPort.MAV.cs.radius;

                    g.DrawArc(new Pen(Color.HotPink, 2), p1, p2, Math.Abs(MainV2.comPort.MAV.cs.radius) * 2, Math.Abs(MainV2.comPort.MAV.cs.radius) * 2, cog, alpha);

                }

                else if (MainV2.comPort.MAV.cs.radius > 1)
                {
                    // correct

                    float p1 = (float)Math.Cos((cog - 180) * deg2rad) * MainV2.comPort.MAV.cs.radius + MainV2.comPort.MAV.cs.radius;

                    float p2 = (float)Math.Sin((cog - 180) * deg2rad) * MainV2.comPort.MAV.cs.radius + MainV2.comPort.MAV.cs.radius;

                    g.DrawArc(new Pen(Color.HotPink, 2), -p1, -p2, MainV2.comPort.MAV.cs.radius * 2, MainV2.comPort.MAV.cs.radius * 2, cog - 180, alpha);
                }

            }

            catch { }


            try {
            g.RotateTransform(heading);
            } catch{}
            g.DrawImageUnscaled(global::MissionPlanner.Properties.Resources.planeicon, global::MissionPlanner.Properties.Resources.planeicon.Width / -2, global::MissionPlanner.Properties.Resources.planeicon.Height / -2);

            g.Transform = temp;
        }
    }


    public class GMapMarkerQuad : GMapMarker
    {
        const float rad2deg = (float)(180 / Math.PI);
        const float deg2rad = (float)(1.0 / rad2deg);

        static readonly System.Drawing.Size SizeSt = new System.Drawing.Size(global::MissionPlanner.Properties.Resources.quadicon.Width, global::MissionPlanner.Properties.Resources.quadicon.Height);
        float heading = 0;
        float cog = -1;
        float target = -1;

        public GMapMarkerQuad(PointLatLng p, float heading, float cog, float target)
            : base(p)
        {
            this.heading = heading;
            this.cog = cog;
            this.target = target;
            Size = SizeSt;
        }

        public override void OnRender(Graphics g)
        {
            Matrix temp = g.Transform;
            g.TranslateTransform(LocalPosition.X, LocalPosition.Y);

            int length = 500;
// anti NaN
            try
            {
                g.DrawLine(new Pen(Color.Red, 2), 0.0f, 0.0f, (float)Math.Cos((heading - 90) * deg2rad) * length, (float)Math.Sin((heading - 90) * deg2rad) * length);
            }
            catch { }
            //g.DrawLine(new Pen(Color.Green, 2), 0.0f, 0.0f, (float)Math.Cos((nav_bearing - 90) * deg2rad) * length, (float)Math.Sin((nav_bearing - 90) * deg2rad) * length);
            g.DrawLine(new Pen(Color.Black, 2), 0.0f, 0.0f, (float)Math.Cos((cog - 90) * deg2rad) * length, (float)Math.Sin((cog - 90) * deg2rad) * length);
            g.DrawLine(new Pen(Color.Orange, 2), 0.0f, 0.0f, (float)Math.Cos((target - 90) * deg2rad) * length, (float)Math.Sin((target - 90) * deg2rad) * length);
// anti NaN
            try
            {
                g.RotateTransform(heading);
            }
            catch { }
            g.DrawImageUnscaled(global::MissionPlanner.Properties.Resources.quadicon, global::MissionPlanner.Properties.Resources.quadicon.Width / -2 + 2, global::MissionPlanner.Properties.Resources.quadicon.Height / -2);

            g.Transform = temp;
        }
    }

    class NoCheckCertificatePolicy : ICertificatePolicy
    {
        public bool CheckValidationResult(ServicePoint srvPoint, X509Certificate certificate, WebRequest request, int certificateProblem)
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
            ms,
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
            [DisplayText("HIL")]
            AP_PRODUCT_ID_NONE = 0x00,	// Hardware in the loop
            [DisplayText("APM1 1280")]
            AP_PRODUCT_ID_APM1_1280 = 0x01,// APM1 with 1280 CPUs
            [DisplayText("APM1 2560")]
            AP_PRODUCT_ID_APM1_2560 = 0x02,// APM1 with 2560 CPUs
            [DisplayText("SITL")]
            AP_PRODUCT_ID_SITL = 0x03,// Software in the loop
            [DisplayText("APM2 ES C4")]
            AP_PRODUCT_ID_APM2ES_REV_C4 = 0x14,// APM2 with MPU6000ES_REV_C4
            [DisplayText("APM2 ES C5")]
            AP_PRODUCT_ID_APM2ES_REV_C5 = 0x15,	// APM2 with MPU6000ES_REV_C5
            [DisplayText("APM2 ES D6")]
            AP_PRODUCT_ID_APM2ES_REV_D6 = 0x16,	// APM2 with MPU6000ES_REV_D6
            [DisplayText("APM2 ES D7")]
            AP_PRODUCT_ID_APM2ES_REV_D7 = 0x17,	// APM2 with MPU6000ES_REV_D7
            [DisplayText("APM2 ES D8")]
            AP_PRODUCT_ID_APM2ES_REV_D8 = 0x18,	// APM2 with MPU6000ES_REV_D8	
            [DisplayText("APM2 C4")]
            AP_PRODUCT_ID_APM2_REV_C4 = 0x54,// APM2 with MPU6000_REV_C4 	
            [DisplayText("APM2 C5")]
            AP_PRODUCT_ID_APM2_REV_C5 = 0x55,	// APM2 with MPU6000_REV_C5 	
            [DisplayText("APM2 D6")]
            AP_PRODUCT_ID_APM2_REV_D6 = 0x56,	// APM2 with MPU6000_REV_D6 		
            [DisplayText("APM2 D7")]
            AP_PRODUCT_ID_APM2_REV_D7 = 0x57,	// APM2 with MPU6000_REV_D7 	
            [DisplayText("APM2 D8")]
            AP_PRODUCT_ID_APM2_REV_D8 = 0x58,	// APM2 with MPU6000_REV_D8 	
            [DisplayText("APM2 D9")]
            AP_PRODUCT_ID_APM2_REV_D9 = 0x59	// APM2 with MPU6000_REV_D9 
        }
        /*
        public enum apmmodes
        {
            [DisplayText("Manual")]
            MANUAL = 0,
            [DisplayText("Circle")]
            CIRCLE = 1,
            [DisplayText("Stabilize")]
            STABILIZE = 2,
            [DisplayText("Training")]
            TRAINING = 3,
            [DisplayText("FBW A")]
            FLY_BY_WIRE_A = 5,
            [DisplayText("FBW B")]
            FLY_BY_WIRE_B = 6,
            [DisplayText("Auto")]
            AUTO = 10,
            [DisplayText("RTL")]
            RTL = 11,
            [DisplayText("Loiter")]
            LOITER = 12,
            [DisplayText("Guided")]
            GUIDED = 15,

            TAKEOFF = 99
        }

        public enum aprovermodes
        {
            [DisplayText("Manual")]
            MANUAL = 0,
            [DisplayText("Learning")]
            LEARNING = 2,
            [DisplayText("Steering")]
            STEERING = 3,
            [DisplayText("Hold")]
            HOLD = 4,
            [DisplayText("Auto")]
            AUTO = 10,
            [DisplayText("RTL")]
            RTL = 11,
            [DisplayText("Guided")]
            GUIDED = 15,
            [DisplayText("Initialising")]
            INITIALISING = 16
        }

        public enum ac2modes
        {
            [DisplayText("Stabilize")]
            STABILIZE = 0,			// hold level position
            [DisplayText("Acro")]
            ACRO = 1,			// rate control
            [DisplayText("Alt Hold")]
            ALT_HOLD = 2,		// AUTO control
            [DisplayText("Auto")]
            AUTO = 3,			// AUTO control
            [DisplayText("Guided")]
            GUIDED = 4,		// AUTO control
            [DisplayText("Loiter")]
            LOITER = 5,		// Hold a single location
            [DisplayText("RTL")]
            RTL = 6,				// AUTO control
            [DisplayText("Circle")]
            CIRCLE = 7,
            [DisplayText("Pos Hold")]
            POSITION = 8,
            [DisplayText("Land")]
            LAND = 9,				// AUTO control
            OF_LOITER = 10,
            [DisplayText("Toy")]
			TOY = 11
        }
        */
  
        public static bool getFilefromNet(string url,string saveto) {
            try
            {
                // this is for mono to a ssl server
                //ServicePointManager.CertificatePolicy = new NoCheckCertificatePolicy(); 

                ServicePointManager.ServerCertificateValidationCallback =
    new System.Net.Security.RemoteCertificateValidationCallback((sender, certificate, chain, policyErrors) => { return true; });

                // Create a request using a URL that can receive a post. 
                WebRequest request = WebRequest.Create(url);
                request.Timeout = 10000;
                // Set the Method property of the request to POST.
                request.Method = "GET";
                // Get the response.
                WebResponse response = request.GetResponse();
                // Display the status.
                log.Info(((HttpWebResponse)response).StatusDescription);
                if (((HttpWebResponse)response).StatusCode != HttpStatusCode.OK)
                    return false;
                // Get the stream containing content returned by the server.
                Stream dataStream = response.GetResponseStream();

                long bytes = response.ContentLength;
                long contlen = bytes;

                byte[] buf1 = new byte[1024];

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
            catch (Exception ex) { log.Info("getFilefromNet(): " + ex.ToString()); return false; }
        }

        public static List<KeyValuePair<int,string>> getModesList(CurrentState cs)
        {
            log.Info("getModesList Called");

            // ensure we get the correct list
            MainV2.comPort.MAV.cs.firmware = cs.firmware;

            Utilities.ParameterMetaDataRepository parm = new ParameterMetaDataRepository();

            if (cs.firmware == MainV2.Firmwares.ArduPlane)
            {
                var flightModes = parm.GetParameterOptionsInt("FLTMODE1");
                flightModes.Add(new KeyValuePair<int,string>(16,"INITIALISING"));
                return flightModes;
            }
            else if (cs.firmware == MainV2.Firmwares.Ateryx)
            {
                var flightModes = parm.GetParameterOptionsInt("FLTMODE1"); //same as apm
                return flightModes;
            }
            else if (cs.firmware == MainV2.Firmwares.ArduCopter2 || MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduHeli)
            {
                var flightModes = parm.GetParameterOptionsInt("FLTMODE1");
                return flightModes;
            }
            else if (cs.firmware == MainV2.Firmwares.ArduRover)
            {
                var flightModes = parm.GetParameterOptionsInt("MODE1");
                return flightModes;
            }

            return null;
        }

        public static Form LoadingBox(string title, string promptText)
        {
            Form form = new Form();
            System.Windows.Forms.Label label = new System.Windows.Forms.Label();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainV2));
            form.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));

            form.Text = title;
            label.Text = promptText;

            label.SetBounds(9, 50, 372, 13);

            label.AutoSize = true;

            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] { label });
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
            ArdupilotMega.Controls.MyButton buttonOk = new ArdupilotMega.Controls.MyButton();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainV2));
            form.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));

            form.Text = title;
            label.Text = promptText;

            chk.Tag = ("SHOWAGAIN_" + title.Replace(" ","_"));
            chk.AutoSize = true;
            chk.Text = "Show me again?";
            chk.Checked = true;
            chk.Location = new Point(9,80);

            if (MainV2.config[(string)chk.Tag] != null && (string)MainV2.config[(string)chk.Tag] == "False") // skip it
            {
                form.Dispose();
                chk.Dispose();
                buttonOk.Dispose();
                label.Dispose();
                return DialogResult.OK;
            }

            chk.CheckStateChanged += new EventHandler(chk_CheckStateChanged);

            buttonOk.Text = "OK";
            buttonOk.DialogResult = DialogResult.OK;
            buttonOk.Location = new Point(form.Right - 100 ,80);

            label.SetBounds(9, 40, 372, 13);

            label.AutoSize = true;

            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] { label, chk, buttonOk });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;

            ThemeManager.ApplyThemeTo(form);

            DialogResult dialogResult =form.ShowDialog();

            form.Dispose();

            form = null;

            return dialogResult;
        }

        static void chk_CheckStateChanged(object sender, EventArgs e)
        {
            MainV2.config[(string)((CheckBox)(sender)).Tag] = ((CheckBox)(sender)).Checked.ToString();
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

            return input;
        }
    }





}
