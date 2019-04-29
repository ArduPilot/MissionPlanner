using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GMap.NET;
using GMap.NET.MapProviders;
using MissionPlanner.Controls;
using MissionPlanner.Utilities.Drawing;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using Brushes = MissionPlanner.Utilities.Drawing.Brushes;
using Color = System.Drawing.Color;
using Graphics = MissionPlanner.Utilities.Drawing.Graphics;
using Image = MissionPlanner.Utilities.Drawing.Image;
using Pen = MissionPlanner.Utilities.Drawing.Pen;
using StringFormat = MissionPlanner.Utilities.Drawing.StringFormat;
using SystemFonts = MissionPlanner.Utilities.Drawing.SystemFonts;

namespace Xamarin
{
    public partial class MainPage_old : ContentPage
    {


        public MainPage_old()
        {
            InitializeComponent();

            AGauge.BackgroundImage = Image.FromStream(new MemoryStream( Properties.Resources.guagebg));

            GMap.NET.GMaps.Instance.PrimaryCache = new MissionPlanner.Maps.MyImageCache();

            GMapControl.MapProvider = GMapProviders.GoogleSatelliteMap;
            GMapControl.MaxZoom = 21;
            GMapControl.MapScaleInfoEnabled = true;
            GMapControl.ScalePen = new Pen(Color.Orange);
            GMapControl.Zoom = 10;
            GMapControl.Position = new PointLatLng(-35, 117.89);
        }


        private void Button1_Pressed(object sender, EventArgs e)
        {

        }

        private void Button2_Pressed(object sender, EventArgs e)
        {

        }

        private void Button3_Pressed(object sender, EventArgs e)
        {
        }

        private void SKGLView_OnPaintSurface(object sender, SKPaintGLSurfaceEventArgs e)
        {


            e.Surface.Canvas.Clear(SKColors.AliceBlue);

            e.Surface.Canvas.DrawRect(10, 10, 50, 50, new SKPaint() {StrokeWidth = 2, Color = SKColors.Red});

            e.Surface.Canvas.DrawText(touchpoint.ToString(), 80, 20, new SKPaint() {TextSize = 16, StrokeWidth = 2});

            var g = new Graphics(e.Surface);

       

            g.DrawRectangle(new Pen(Color.Red), touchpoint.X, touchpoint.Y, 12,12);



            e.Surface.Canvas.DrawText(base.Width + " " + base.Height, 80, 40, new SKPaint() { TextSize = 16, StrokeWidth = 2 });

            e.Surface.Canvas.DrawText(SkglView.CanvasSize.ToString(), 80, 60, new SKPaint() { TextSize = 16, StrokeWidth = 2 });

        }

        SKPoint touchpoint = new SKPoint();

        private void SkglView_OnTouch(object sender, SKTouchEventArgs e)
        {
            touchpoint = e.Location;
            SkglView.InvalidateSurface();

            e.Handled = true;
        }
    }
}