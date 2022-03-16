using System;
using System.Drawing;
using GMap.NET;
using GMap.NET.WindowsForms;
using MissionPlanner.Utilities;

namespace MissionPlanner.Maps
{
    [Serializable]
    public class GMapMarkerQuad : GMapMarkerBase
    {
        private readonly Bitmap icon = global::MissionPlanner.Maps.Resources.quadicon;

        float heading = 0;
        float cog = -1;
        float target = -1;
        private int sysid = -1;

        public float warn = -1;
        public float danger = -1;

        static Color green = ExtensionsMaps.ColorFromHex("8dc63f");
        static Color blue = ExtensionsMaps.ColorFromHex("00aeef");
        static Pen greenpen = new Pen(green, 3);
        static Pen bluepen = new Pen(blue, 3);
        static SolidBrush greenbrush = new SolidBrush(green);

        public float Heading { get => heading; set => heading = value; }
        public float Cog { get => cog; set => cog = value; }
        public float Target { get => target; set => target = value; }
        public int Sysid { get => sysid; set => sysid = value; }
        public float framerotation { get; set; } = 0;

        public GMapMarkerQuad(PointLatLng p, float heading, float cog, float target, int sysid)
            : base(p)
        {
            this.Heading = heading;
            this.Cog = cog;
            this.Target = target;
            this.Sysid = sysid;
            Size = icon.Size;
            // for hitzone
            Offset = new Point(-icon.Width / 2, -icon.Width / 2);
        }

        public override void OnRender(IGraphics g)
        {
            var temp = g.Transform;
            g.TranslateTransform(LocalPosition.X, LocalPosition.Y);
            // set centerpoint as 0,0
            g.TranslateTransform(-Offset.X, -Offset.Y);

            // anti NaN
            try
            {
                if (DisplayHeading)
                    g.DrawLine(new Pen(Color.Red, 2), 0.0f, 0.0f,
                        (float) Math.Cos((Heading - 90) * MathHelper.deg2rad) * length,
                        (float) Math.Sin((Heading - 90) * MathHelper.deg2rad) * length);
            }
            catch
            {
            }

            //g.DrawLine(new Pen(Color.Green, 2), 0.0f, 0.0f, (float)Math.Cos((nav_bearing - 90) * MathHelper.deg2rad) * length, (float)Math.Sin((nav_bearing - 90) * MathHelper.deg2rad) * length);
            if (DisplayCOG)
                g.DrawLine(new Pen(Color.Black, 2), 0.0f, 0.0f,
                    (float) Math.Cos((Cog - 90) * MathHelper.deg2rad) * length,
                    (float) Math.Sin((Cog - 90) * MathHelper.deg2rad) * length);
            if (DisplayTarget)
                g.DrawLine(new Pen(Color.Orange, 2), 0.0f, 0.0f,
                    (float) Math.Cos((Target - 90) * MathHelper.deg2rad) * length,
                    (float) Math.Sin((Target - 90) * MathHelper.deg2rad) * length);
            // anti NaN
            try
            {
                g.RotateTransform(Heading);
            }
            catch
            {
            }

            //g.DrawImageUnscaled(icon, icon.Width / -2 + 2, icon.Height / -2);

            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                g.RotateTransform(framerotation);

                //motors
                g.DrawArc(greenpen, 35f - 10 + Offset.X, 12f - 10 + Offset.Y, 20, 20, 0, 360);
                g.DrawArc(greenpen, 35f - 10 + Offset.X, 57f - 10 + Offset.Y, 20, 20, 0, 360);
                g.DrawArc(greenpen, 57f - 10 + Offset.X, 35f - 10 + Offset.Y, 20, 20, 0, 360);
                g.DrawArc(greenpen, 12f - 10 + Offset.X, 35f - 10 + Offset.Y, 20, 20, 0, 360);

                g.DrawArc(greenpen, 35f - 2.5f + Offset.X, 12f - 2.5f + Offset.Y, 5, 5, 0, 360);
                g.DrawArc(greenpen, 35f - 2.5f + Offset.X, 57f - 2.5f + Offset.Y, 5, 5, 0, 360);
                g.DrawArc(greenpen, 57f - 2.5f + Offset.X, 35f - 2.5f + Offset.Y, 5, 5, 0, 360);
                g.DrawArc(greenpen, 12f - 2.5f + Offset.X, 35f - 2.5f + Offset.Y, 5, 5, 0, 360);
                                
                g.DrawLine(bluepen, 35 + Offset.X, 12 + Offset.Y, 35 + Offset.X, 35 + Offset.Y);
                g.DrawLine(greenpen, 35 + Offset.X, 36 + Offset.Y, 35 + Offset.X, 57 + Offset.Y);
                g.DrawLine(greenpen, 57 + Offset.X, 35 + Offset.Y, 12 + Offset.X, 35 + Offset.Y);

                g.FillRectangle(greenbrush, 32 + Offset.X, 30 + Offset.Y, 5, 8);

                g.RotateTransform(-framerotation);
            }

            g.DrawString(Sysid.ToString(), new Font(FontFamily.GenericMonospace, 15, FontStyle.Bold), Brushes.Red, -8,
              -8);

            g.Transform = temp;

            {
                double width =
                    (Overlay.Control.MapProvider.Projection.GetDistance(Overlay.Control.FromLocalToLatLng(0, 0),
                         Overlay.Control.FromLocalToLatLng(Overlay.Control.Width, 0)) * 1000.0);
                double m2pixelwidth = Overlay.Control.Width / width;

                GPoint loc = new GPoint((int) (LocalPosition.X - (m2pixelwidth * warn * 2)), LocalPosition.Y);

                if (m2pixelwidth > 0.001 && warn > 0)
                    g.DrawArc(Pens.Orange,
                        new System.Drawing.Rectangle(
                            LocalPosition.X - Offset.X - (int) (Math.Abs(loc.X - LocalPosition.X) / 2),
                            LocalPosition.Y - Offset.Y - (int) Math.Abs(loc.X - LocalPosition.X) / 2,
                            (int) Math.Abs(loc.X - LocalPosition.X), (int) Math.Abs(loc.X - LocalPosition.X)), 0,
                        360);

                loc = new GPoint((int) (LocalPosition.X - (m2pixelwidth * danger * 2)), LocalPosition.Y);

                if (m2pixelwidth > 0.001 && danger > 0)
                    g.DrawArc(Pens.Red,
                        new System.Drawing.Rectangle(
                            LocalPosition.X - Offset.X - (int) (Math.Abs(loc.X - LocalPosition.X) / 2),
                            LocalPosition.Y - Offset.Y - (int) Math.Abs(loc.X - LocalPosition.X) / 2,
                            (int) Math.Abs(loc.X - LocalPosition.X), (int) Math.Abs(loc.X - LocalPosition.X)), 0,
                        360);
            }
        }
    }
}