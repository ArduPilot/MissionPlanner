using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MissionPlanner.Utilities.Drawing;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using Brushes = MissionPlanner.Utilities.Drawing.Brushes;
using Color = System.Drawing.Color;
using Graphics = MissionPlanner.Utilities.Drawing.Graphics;
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

            e.Surface.Canvas.DrawText(touchpoint.ToString(), 20, 20, new SKPaint() {TextSize = 16, StrokeWidth = 2});

            var g = new Graphics(e.Surface);

       

            g.DrawRectangle(new Pen(Color.Red), touchpoint.X, touchpoint.Y, 12,12);

        }

        SKPoint touchpoint = new SKPoint();

        private void SkglView_OnTouch(object sender, SKTouchEventArgs e)
        {
            touchpoint = e.Location;
            SkglView.InvalidateSurface();
        }
    }
}