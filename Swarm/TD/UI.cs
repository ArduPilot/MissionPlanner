using GMap.NET;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace MissionPlanner.Swarm.TD
{
    public partial class UI: Form
    {
        static TD.Controller controller;

        public UI()
        {
            InitializeComponent();

            typeof(Panel).InvokeMember("DoubleBuffered", BindingFlags.SetProperty
            | BindingFlags.Instance | BindingFlags.NonPublic, null,
            panel1, new object[] { true });
        }

        private Controls.MyButton but_Start;
        private Panel panel1;
        private Label lbl_mode;
        private Timer timer1;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.but_Start = new MissionPlanner.Controls.MyButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lbl_mode = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // but_Start
            // 
            this.but_Start.Location = new System.Drawing.Point(12, 12);
            this.but_Start.Name = "but_Start";
            this.but_Start.Size = new System.Drawing.Size(75, 23);
            this.but_Start.TabIndex = 0;
            this.but_Start.Text = "Start";
            this.but_Start.UseVisualStyleBackColor = true;
            this.but_Start.Click += new System.EventHandler(this.myButton1_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Location = new System.Drawing.Point(12, 103);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(870, 412);
            this.panel1.TabIndex = 1;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // lbl_mode
            // 
            this.lbl_mode.AutoSize = true;
            this.lbl_mode.Location = new System.Drawing.Point(93, 17);
            this.lbl_mode.Name = "lbl_mode";
            this.lbl_mode.Size = new System.Drawing.Size(35, 13);
            this.lbl_mode.TabIndex = 2;
            this.lbl_mode.Text = "label1";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // UI
            // 
            this.ClientSize = new System.Drawing.Size(894, 527);
            this.Controls.Add(this.lbl_mode);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.but_Start);
            this.Name = "UI";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (controller == null)
                return;

            var minlat = controller.DG.Fence.Min((a) => a.Lat);
            var minlng = controller.DG.Fence.Min((a) => a.Lng);

            var maxlat = controller.DG.Fence.Max((a) => a.Lat);
            var maxlng = controller.DG.Fence.Max((a) => a.Lng);

            RectLatLng rect = new RectLatLng(maxlat, minlng, maxlng - minlng, maxlat - minlat);

            Size drawin = panel1.Size;

            var ratio = drawin.Width / (double)drawin.Height;
            var ratiorect = rect.WidthLng / rect.HeightLat;

            if(ratio > 1)
                rect.Inflate(0, rect.HeightLat * ratio - rect.WidthLng);
            else
                rect.Inflate(rect.WidthLng / ratio - rect.HeightLat, 0);

            rect.Inflate(0.00001,0.00001);

            minlat = rect.Bottom;
            minlng = rect.Left;

            maxlat = rect.Top;
            maxlng = rect.Right;

            var dist = ((PointLatLngAlt)rect.LocationTopLeft).GetDistance(new PointLatLngAlt(rect.Top, rect.Right));

            var distperpixel = (float)(drawin.Width / dist);

            foreach (var line in controller.DG.Fence.PrevNowNext(null, null))
            {
                if (line.Item1 == null)
                    continue;

                var posx = (float)map(line.Item1.Lng, minlng, maxlng, 0, drawin.Width);
                var posy = (float)map(line.Item1.Lat, minlat, maxlat, drawin.Height, 0);

                var posx2 = (float)map(line.Item2.Lng, minlng, maxlng, 0, drawin.Width);
                var posy2 = (float)map(line.Item2.Lat, minlat, maxlat, drawin.Height, 0);

                e.Graphics.DrawLine(Pens.Pink, posx, posy, posx2, posy2);
            }
                        
            foreach (var drone in controller.DG.Drones)
            {
                if (drone.Location != null)
                {
                    var posx = (float)map(drone.Location.Lng, minlng, maxlng, 0, drawin.Width);
                    var posy = (float)map(drone.Location.Lat, minlat, maxlat, drawin.Height, 0);

                    // location
                    e.Graphics.FillPie(Brushes.Red, posx- distperpixel/2, posy- distperpixel/2, distperpixel, distperpixel, 0, 360);
                    e.Graphics.DrawString(drone.MavState.sysid.ToString(), SystemFonts.DefaultFont, Brushes.White, posx, posy);

                    e.Graphics.DrawArc(Pens.Red, posx - (float)drone.bubblerad * distperpixel, posy - (float)drone.bubblerad * distperpixel, distperpixel * (float)drone.bubblerad * 2, distperpixel * (float)drone.bubblerad * 2, 0, 360);

                    if (drone.TargetLocation != null)
                    {
                        var postargetx = (float)map(drone.TargetLocation.Lng, minlng, maxlng, 0, drawin.Width);
                        var postargety = (float)map(drone.TargetLocation.Lat, minlat, maxlat, drawin.Height, 0);
                        // targetlocation
                        e.Graphics.FillPie(Brushes.Green, postargetx-5, postargety-5, 10, 10, 0, 360);

                        // line between
                        e.Graphics.DrawLine(Pens.Blue, posx, posy, postargetx, postargety);
                    }

                    //alt
                    var heightpx = (float)map(drone.Location.Alt, 0, controller.DG.FenceMaxAlt, 0, drawin.Height);
                    e.Graphics.FillRectangle(Brushes.Yellow, 10 + drone.MavState.sysid * 10, drawin.Height - heightpx, 10, heightpx);

                    e.Graphics.DrawString(drone.MavState.sysid.ToString() + "\n" + drone.Location.Alt.ToString("0.0"), SystemFonts.DefaultFont, Brushes.White, 10 + drone.MavState.sysid * 10, drawin.Height - heightpx - 30);
                }
            }
        }

        static double map(double x, double in_min, double in_max, double out_min, double out_max)
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }

        private void myButton1_Click(object sender, EventArgs e)
        {
            if (controller == null)
            {
                if (MainV2.instance.FlightPlanner.drawnpolygon.Points.Count == 0)
                {
                    CustomMessageBox.Show("Please draw a polygon for the fence in flightplanner");
                    return;
                }

                controller = new Controller();

                var fencepolygon = new List<PointLatLng>(MainV2.instance.FlightPlanner.drawnpolygon.Points);

                fencepolygon.ForEach(a => { controller.DG.Fence.Add((PointLatLngAlt)a); });

                double minalt = 2;
                double maxalt = 30;

                InputBox.Show("", "Fence Min Alt", ref minalt);
                InputBox.Show("", "Fence Max Alt", ref maxalt);

                controller.DG.FenceMinAlt = minalt;
                controller.DG.FenceMaxAlt = maxalt;

                controller.Start();
                but_Start.Text = "Stop";
            }
            else
            {
                controller?.Stop();
                controller = null;
                but_Start.Text = "Start";
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lbl_mode.Text = controller?.DG?.CurrentMode.ToString();

            panel1.Invalidate();
        }
    }
}
