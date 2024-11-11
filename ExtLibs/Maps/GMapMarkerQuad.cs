using System;
using System.Drawing;
using GMap.NET;
using GMap.NET.WindowsForms;
using MissionPlanner.Utilities;
using Org.BouncyCastle.Crypto.Signers;

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

        private static Pen _greenpen;
        private static Pen _transparent_greenpen;
        private static Pen _bluepen;
        private static Pen _transparent_bluepen;
        private static SolidBrush _greenbrush;
        private static SolidBrush _transparent_greenbrush;
        private static SolidBrush _textbrush;
        private static SolidBrush _transparent_textbrush;

        static Pen greenpen
        {
            set
            {
                _greenpen = value;
                _transparent_greenpen = new Pen(Color.FromArgb(100, _greenpen.Color), _greenpen.Width);
            }
        }
        static Pen bluepen
        {
            set
            {
                _bluepen = value;
                _transparent_bluepen = new Pen(Color.FromArgb(100, _bluepen.Color), _bluepen.Width);
            }
        }
        static SolidBrush greenbrush
        {
            set
            {
                _greenbrush = value;
                _transparent_greenbrush = new SolidBrush(Color.FromArgb(100, _greenbrush.Color));
            }
        }

        static SolidBrush textbrush
        {
            set
            {
                _textbrush = value;
                _transparent_textbrush = new SolidBrush(Color.FromArgb(100, _textbrush.Color));
            }
        }

        private Pen thisgreenpen
        {
            get
            {
                return IsTransparent ? _transparent_greenpen : _greenpen;
            }
        }

        private Pen thisbluepen
        {
            get
            {
                return IsTransparent ? _transparent_bluepen : _bluepen;
            }
        }

        private SolidBrush thisgreenbrush
        {
            get
            {
                return IsTransparent ? _transparent_greenbrush : _greenbrush;
            }
        }

        private SolidBrush thistextbrush
        {
            get
            {
                return IsTransparent ? _transparent_textbrush : _textbrush;
            }
        }

        public float Heading { get => heading; set => heading = value; }
        public float Cog { get => cog; set => cog = value; }
        public float Target { get => target; set => target = value; }
        public int Sysid { get => sysid; set => sysid = value; }
        public float framerotation { get; set; } = 0;

        static GMapMarkerQuad()
        {
            greenpen = new Pen(ExtensionsMaps.ColorFromHex("8dc63f"), 3);
            bluepen = new Pen(ExtensionsMaps.ColorFromHex("00aeef"), _greenpen.Width);
            greenbrush = new SolidBrush(_greenpen.Color);
            textbrush = new SolidBrush(Color.Red);
        }

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
            if (IsHidden)
            {
                return;
            }

            var temp = g.Transform;
            g.TranslateTransform(LocalPosition.X, LocalPosition.Y);
            g.TranslateTransform(-Offset.X, -Offset.Y);
            g.RotateTransform(-Overlay.Control.Bearing);

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
                g.DrawArc(thisgreenpen, 35f - 10 + Offset.X, 12f - 10 + Offset.Y, 20, 20, 0, 360);
                g.DrawArc(thisgreenpen, 35f - 10 + Offset.X, 57f - 10 + Offset.Y, 20, 20, 0, 360);
                g.DrawArc(thisgreenpen, 57f - 10 + Offset.X, 35f - 10 + Offset.Y, 20, 20, 0, 360);
                g.DrawArc(thisgreenpen, 12f - 10 + Offset.X, 35f - 10 + Offset.Y, 20, 20, 0, 360);

                g.DrawArc(thisgreenpen, 35f - 2.5f + Offset.X, 12f - 2.5f + Offset.Y, 5, 5, 0, 360);
                g.DrawArc(thisgreenpen, 35f - 2.5f + Offset.X, 57f - 2.5f + Offset.Y, 5, 5, 0, 360);
                g.DrawArc(thisgreenpen, 57f - 2.5f + Offset.X, 35f - 2.5f + Offset.Y, 5, 5, 0, 360);
                g.DrawArc(thisgreenpen, 12f - 2.5f + Offset.X, 35f - 2.5f + Offset.Y, 5, 5, 0, 360);
                                
                g.DrawLine(thisbluepen, 35 + Offset.X, 12 + Offset.Y, 35 + Offset.X, 35 + Offset.Y);
                g.DrawLine(thisgreenpen, 35 + Offset.X, 36 + Offset.Y, 35 + Offset.X, 57 + Offset.Y);
                g.DrawLine(thisgreenpen, 57 + Offset.X, 35 + Offset.Y, 12 + Offset.X, 35 + Offset.Y);

                g.FillRectangle(thisgreenbrush, 32 + Offset.X, 30 + Offset.Y, 5, 8);

                g.RotateTransform(-framerotation);
            }

            g.DrawString(Sysid.ToString(), new Font(FontFamily.GenericMonospace, 15, FontStyle.Bold), thistextbrush, -8, -8);

            g.Transform = temp;

            {
                double width =
                    (Overlay.Control.MapProvider.Projection.GetDistance(Overlay.Control.FromLocalToLatLng(0, 0),
                         Overlay.Control.FromLocalToLatLng(Overlay.Control.Width, 0)) * 1000.0);
                // size of a square meter in pixels on our map
                double m2pixelwidth = Overlay.Control.Width / width;

                GPoint loc = new GPoint((int) (LocalPosition.X - (m2pixelwidth * warn * 2)), LocalPosition.Y);

                var markerDimension = (int)Math.Abs(loc.X - LocalPosition.X);
                if (markerDimension == 0)
                {
                    return;
                }

                if (m2pixelwidth > 0.001 && warn > 0)
                    g.DrawArc(Pens.Orange,
                        new System.Drawing.Rectangle(
                            LocalPosition.X - Offset.X - (int) (Math.Abs(loc.X - LocalPosition.X) / 2),
                            LocalPosition.Y - Offset.Y - (int) Math.Abs(loc.X - LocalPosition.X) / 2,
                            markerDimension, markerDimension), 0,
                        360);

                loc = new GPoint((int) (LocalPosition.X - (m2pixelwidth * danger * 2)), LocalPosition.Y);
                markerDimension = (int)Math.Abs(loc.X - LocalPosition.X);
                if (markerDimension == 0)
                {
                    return;
                }

                if (m2pixelwidth > 0.001 && danger > 0)
                    g.DrawArc(Pens.Red,
                        new System.Drawing.Rectangle(
                            LocalPosition.X - Offset.X - (int) (Math.Abs(loc.X - LocalPosition.X) / 2),
                            LocalPosition.Y - Offset.Y - (int) Math.Abs(loc.X - LocalPosition.X) / 2,
                            markerDimension, markerDimension), 0,
                        360);
            }
        }
    }
}