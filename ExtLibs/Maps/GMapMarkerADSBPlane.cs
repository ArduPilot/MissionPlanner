using System;
using System.Collections.Generic;
using System.Drawing;
using GMap.NET;
using GMap.NET.Drawing.Properties;
using GMap.NET.WindowsForms;

namespace MissionPlanner.Maps
{
    [Serializable]
    public class GMapMarkerADSBPlane : GMapMarker
    {
        // The images we're using are 72x72, so we'll use that as the size
        private static readonly Size size = new Size(72, 72);

        // Images retrieved from tar1090 sprites: https://github.com/wiedehopf/tar1090/blob/master/html/images/sprites.png
        private static readonly Bitmap adsb_unknown = new Bitmap(global::MissionPlanner.Maps.Resources.adsb_unknown, size);
        private static readonly Bitmap adsb_light = new Bitmap(global::MissionPlanner.Maps.Resources.adsb_light, size);
        private static readonly Bitmap adsb_small = new Bitmap(global::MissionPlanner.Maps.Resources.adsb_small, size);
        private static readonly Bitmap adsb_large = new Bitmap(global::MissionPlanner.Maps.Resources.adsb_large, size);
        private static readonly Bitmap adsb_heavy = new Bitmap(global::MissionPlanner.Maps.Resources.adsb_heavy, size);
        private static readonly Bitmap adsb_highly_manuv = new Bitmap(global::MissionPlanner.Maps.Resources.adsb_highly_manuv, size);
        private static readonly Bitmap adsb_rotocraft = new Bitmap(global::MissionPlanner.Maps.Resources.adsb_rotocraft, size);
        private static readonly Bitmap adsb_glider = new Bitmap(global::MissionPlanner.Maps.Resources.adsb_glider, size);
        private static readonly Bitmap adsb_lighter_air = new Bitmap(global::MissionPlanner.Maps.Resources.adsb_balloon, size);
        private static readonly Bitmap adsb_parachute = new Bitmap(global::MissionPlanner.Maps.Resources.adsb_parachute, size);
        private static readonly Bitmap adsb_uav = new Bitmap(global::MissionPlanner.Maps.Resources.adsb_uav, size);
        private static readonly Bitmap adsb_emergency_surface = new Bitmap(global::MissionPlanner.Maps.Resources.adsb_emergency_surface, size);
        private static readonly Bitmap adsb_service_surface = new Bitmap(global::MissionPlanner.Maps.Resources.adsb_service_surface, size);
        private static readonly Bitmap adsb_point_obstacle = new Bitmap(global::MissionPlanner.Maps.Resources.adsb_balloon, size); // point obstacles are used for balloons


        // Recoloured sprites shared by every marker, keyed by sprite and fill colour, so hundreds of
        // aircraft share a handful of bitmaps instead of each holding its own copy
        private static readonly Dictionary<Tuple<Bitmap, int>, Bitmap> coloredSprites =
            new Dictionary<Tuple<Bitmap, int>, Bitmap>();
        private static readonly object coloredSpritesLock = new object();

        public float heading = 0;
        public AlertLevelOptions AlertLevel = AlertLevelOptions.Green;
        public float DrawScale { get; private set; } = 0.5f;

        private MAVLink.ADSB_EMITTER_TYPE emitterCategory = MAVLink.ADSB_EMITTER_TYPE.NO_INFO;

        public MAVLink.ADSB_EMITTER_TYPE EmitterCategory
        {
            get => emitterCategory;
            set
            {
                emitterCategory = value;
                GetSprite(value, out float scale);
                if (scale != DrawScale)
                {
                    DrawScale = scale;
                    UpdateSize();
                }
            }
        }

        public bool IsOnGround { get; set; }

        public enum AlertLevelOptions
        {
            Green,
            /// <summary>A warning: amber, with a ring around it</summary>
            Orange,
            /// <summary>A collision threat: red, with a turning ring of quarter arcs around it</summary>
            Red,
            /// <summary>No threat: the sprite as drawn, white with a dark outline, which shows on any map</summary>
            None
        }

        // Amber rather than Color.Orange, so a warning doesn't look like the old no-threat colour
        private static readonly Color Amber = Color.FromArgb(255, 191, 0);

        public GMapMarkerADSBPlane(PointLatLng p, float heading, AlertLevelOptions alert = AlertLevelOptions.Green)
            : base(p)
        {
            this.AlertLevel = alert;
            this.heading = heading;
            UpdateSize();
        }

        /// <summary>
        /// Match the marker's hover and click area to the icon as drawn, not the full-size sprite
        /// </summary>
        private void UpdateSize()
        {
            int side = (int)Math.Round(size.Width * DrawScale);
            Size = new Size(side, side);
            Offset = new Point(side / -2, side / -2);
        }

        /// <summary>
        /// The sprite for an emitter category and the scale to draw it at
        /// </summary>
        private static Bitmap GetSprite(MAVLink.ADSB_EMITTER_TYPE category, out float scale)
        {
            scale = 0.5f;
            switch (category)
            {
                case MAVLink.ADSB_EMITTER_TYPE.LIGHT:
                    return adsb_light;
                case MAVLink.ADSB_EMITTER_TYPE.SMALL:
                    return adsb_small;
                case MAVLink.ADSB_EMITTER_TYPE.LARGE:
                    scale *= 1.25f;
                    return adsb_large;
                case MAVLink.ADSB_EMITTER_TYPE.HEAVY:
                case MAVLink.ADSB_EMITTER_TYPE.HIGH_VORTEX_LARGE:
                    scale *= 1.5f;
                    return adsb_heavy;
                case MAVLink.ADSB_EMITTER_TYPE.HIGHLY_MANUV:
                    return adsb_highly_manuv;
                case MAVLink.ADSB_EMITTER_TYPE.ROTOCRAFT:
                    return adsb_rotocraft;
                case MAVLink.ADSB_EMITTER_TYPE.GLIDER:
                    return adsb_glider;
                case MAVLink.ADSB_EMITTER_TYPE.LIGHTER_AIR:
                    return adsb_lighter_air;
                case MAVLink.ADSB_EMITTER_TYPE.PARACHUTE:
                    return adsb_parachute;
                case MAVLink.ADSB_EMITTER_TYPE.ULTRA_LIGHT:
                    // No dedicated ultralight sprite; the glider is the closest match
                    return adsb_glider;
                case MAVLink.ADSB_EMITTER_TYPE.UAV:
                    return adsb_uav;
                case MAVLink.ADSB_EMITTER_TYPE.EMERGENCY_SURFACE:
                    return adsb_emergency_surface;
                case MAVLink.ADSB_EMITTER_TYPE.SERVICE_SURFACE:
                    return adsb_service_surface;
                case MAVLink.ADSB_EMITTER_TYPE.POINT_OBSTACLE:
                    return adsb_point_obstacle;
                case MAVLink.ADSB_EMITTER_TYPE.NO_INFO:
                case MAVLink.ADSB_EMITTER_TYPE.SPACE:
                default:
                    return adsb_unknown;
            }
        }

        private static Bitmap GetColoredSprite(Bitmap sprite, Color fillColor)
        {
            var key = Tuple.Create(sprite, fillColor.ToArgb());
            lock (coloredSpritesLock)
            {
                if (!coloredSprites.TryGetValue(key, out var colored))
                {
                    // Clone so we never touch the shared static sprite
                    colored = (Bitmap)sprite.Clone();
                    ColorSprite(colored, fillColor);
                    coloredSprites[key] = colored;
                }
                return colored;
            }
        }

        private static void ColorSprite(Bitmap bitmap, Color fillColor)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;

            bool IsWhite(Color pixel) => pixel.R == 255 && pixel.G == 255 && pixel.B == 255 && pixel.A != 0;

            // Iterate through every pixel in the image; this runs once per sprite and colour
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color pixel = bitmap.GetPixel(x, y);

                    // If the pixel is white, color it with the fill color
                    if (IsWhite(pixel))
                    {
                        bitmap.SetPixel(x, y, fillColor);
                    }
                }
            }
        }

        private void DrawThreatRing(IGraphics g, Color color, bool high)
        {
            float r = size.Width * DrawScale * 0.62f;
            var smoothing = g.SmoothingMode;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using (var fill = new SolidBrush(Color.FromArgb(40, color)))
            using (var edge = new Pen(Color.FromArgb(110, Color.Black), high ? 5 : 3))
            using (var ring = new Pen(color, high ? 3 : 2))
            {
                g.FillEllipse(fill, -r, -r, 2 * r, 2 * r);
                if (high)
                {
                    // Two quarter arcs that turn, a quarter turn a second: a solid red ring disappears
                    // next to the red ownship marker, and the motion catches the eye. The map repaints
                    // about every 0.3 s while connected, so this steps ~27 degrees a frame.
                    float start = (float)(DateTime.Now.TimeOfDay.TotalSeconds * 90 % 360);
                    for (int i = 0; i < 2; i++)
                    {
                        // A dark edge under the ring keeps it visible on light road maps
                        g.DrawArc(edge, -r, -r, 2 * r, 2 * r, start + i * 180, 90);
                        g.DrawArc(ring, -r, -r, 2 * r, 2 * r, start + i * 180, 90);
                    }
                }
                else
                {
                    // A dark edge under the ring keeps it visible on light road maps
                    g.DrawEllipse(edge, -r - 0.5f, -r - 0.5f, 2 * r + 1, 2 * r + 1);
                    g.DrawEllipse(ring, -r, -r, 2 * r, 2 * r);
                }
            }
            g.SmoothingMode = smoothing;
        }

        public override void OnRender(IGraphics g)
        {
            var temp = g.Transform;
            g.TranslateTransform(LocalPosition.X - Offset.X, LocalPosition.Y - Offset.Y);

            g.RotateTransform(-Overlay.Control.Bearing);

            try
            {
                g.RotateTransform(heading);
            }
            catch
            {
            }

            Bitmap sprite = GetSprite(EmitterCategory, out _);

            // Set the color based on alert level
            Color? fillColor = null;
            switch (AlertLevel)
            {
                case AlertLevelOptions.Green:
                    fillColor = Color.Green;
                    break;
                case AlertLevelOptions.Orange:
                    fillColor = Amber;
                    break;
                case AlertLevelOptions.Red:
                    fillColor = Color.Red;
                    break;
            }

            if (IsOnGround)
            {
                fillColor = Color.FromArgb(128, Color.DarkGray);
            }

            // A ring around threats, so they stand out at any icon size and on any map background
            if (AlertLevel == AlertLevelOptions.Orange || AlertLevel == AlertLevelOptions.Red)
            {
                DrawThreatRing(g, AlertLevel == AlertLevelOptions.Red ? Color.Red : Amber,
                    AlertLevel == AlertLevelOptions.Red);
            }

            Bitmap colored = fillColor.HasValue ? GetColoredSprite(sprite, fillColor.Value) : sprite;

            g.ScaleTransform(DrawScale, DrawScale);
            g.DrawImageUnscaled(colored, colored.Width / -2, colored.Height / -2);

            g.Transform = temp;
        }
    }
}