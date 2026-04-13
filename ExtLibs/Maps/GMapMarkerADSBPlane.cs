using System;
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
        //private static readonly Bitmap adsb_glider = new Bitmap(global::MissionPlanner.Maps.Resources.adsb_glider, size);
        private static readonly Bitmap adsb_lighter_air = new Bitmap(global::MissionPlanner.Maps.Resources.adsb_balloon, size);
        private static readonly Bitmap adsb_parachute = new Bitmap(global::MissionPlanner.Maps.Resources.adsb_parachute, size);
        //private static readonly Bitmap adsb_ultralight = new Bitmap(global::MissionPlanner.Maps.Resources.adsb_ultralight, size);
        private static readonly Bitmap adsb_uav = new Bitmap(global::MissionPlanner.Maps.Resources.adsb_uav, size);
        private static readonly Bitmap adsb_emergency_surface = new Bitmap(global::MissionPlanner.Maps.Resources.adsb_emergency_surface, size);
        private static readonly Bitmap adsb_service_surface = new Bitmap(global::MissionPlanner.Maps.Resources.adsb_service_surface, size);
        private static readonly Bitmap adsb_point_obstacle = new Bitmap(global::MissionPlanner.Maps.Resources.adsb_balloon, size); // point obstacles are used for balloons


        public float heading = 0;
        public AlertLevelOptions AlertLevel = AlertLevelOptions.Green;
        public float DrawScale = 0.5f;

        // Cache the last drawn data to avoid re-coloring every frame
        private Bitmap lastDrawn = null;
        // Store the last alert level to decide whether to redraw
        private AlertLevelOptions lastAlertLevel = AlertLevelOptions.Green;
        private MAVLink.ADSB_EMITTER_TYPE lastEmitterCategory = MAVLink.ADSB_EMITTER_TYPE.NO_INFO;
        private bool lastIsOnGround = true;

        public MAVLink.ADSB_EMITTER_TYPE EmitterCategory { get; set; }
        public bool IsOnGround { get; set; }

        public enum AlertLevelOptions
        {
            Green,
            Orange,
            Red
        }

        public GMapMarkerADSBPlane(PointLatLng p, float heading, AlertLevelOptions alert = AlertLevelOptions.Green)
            : base(p)
        {
            this.AlertLevel = alert;
            this.heading = heading;
            Size = size;
            Offset = new Point(Size.Width / -2, Size.Height / -2);
        }

        private static void ColorSprite(Bitmap bitmap, Color fillColor)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;

            bool IsWhite(Color pixel) => pixel.R == 255 && pixel.G == 255 && pixel.B == 255 && pixel.A != 0;

            // Iterate through every pixel in the image
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

            bool needsRedraw = lastDrawn == null || lastAlertLevel != AlertLevel || lastEmitterCategory != EmitterCategory || lastIsOnGround != IsOnGround;
            if (!needsRedraw)
            {
                g.ScaleTransform(DrawScale, DrawScale);
                g.DrawImageUnscaled(lastDrawn, lastDrawn.Width / -2, lastDrawn.Height / -2);
                g.Transform = temp;
                return;
            }

            // Set the icon based on emitter category
            Bitmap bitmap = adsb_unknown;
            DrawScale = 0.5f;

            switch (this.EmitterCategory)
            {
                case MAVLink.ADSB_EMITTER_TYPE.NO_INFO:
                    bitmap = adsb_unknown;
                    break;
                case MAVLink.ADSB_EMITTER_TYPE.LIGHT:
                    bitmap = adsb_light;
                    break;
                case MAVLink.ADSB_EMITTER_TYPE.SMALL:
                    bitmap = adsb_small;
                    break;
                case MAVLink.ADSB_EMITTER_TYPE.LARGE:
                    bitmap = adsb_large;
                    DrawScale *= 1.25f;
                    break;
                case MAVLink.ADSB_EMITTER_TYPE.HEAVY:
                    bitmap = adsb_heavy;
                    DrawScale *= 1.5f;
                    break;
                case MAVLink.ADSB_EMITTER_TYPE.HIGH_VORTEX_LARGE:
                    bitmap = adsb_heavy;
                    DrawScale *= 1.5f;
                    break;
                case MAVLink.ADSB_EMITTER_TYPE.HIGHLY_MANUV:
                    bitmap = adsb_highly_manuv;
                    break;
                case MAVLink.ADSB_EMITTER_TYPE.ROTOCRAFT:
                    bitmap = adsb_rotocraft;
                    break;
                case MAVLink.ADSB_EMITTER_TYPE.GLIDER:
                    bitmap = adsb_lighter_air;
                    break;
                case MAVLink.ADSB_EMITTER_TYPE.PARACHUTE:
                    bitmap = adsb_parachute;
                    break;
                case MAVLink.ADSB_EMITTER_TYPE.ULTRA_LIGHT:
                    bitmap = adsb_lighter_air;
                    break;
                case MAVLink.ADSB_EMITTER_TYPE.UAV:
                    bitmap = adsb_uav;
                    break;
                case MAVLink.ADSB_EMITTER_TYPE.SPACE:
                    //???
                    break;
                case MAVLink.ADSB_EMITTER_TYPE.EMERGENCY_SURFACE:
                    bitmap = adsb_emergency_surface;
                    break;
                case MAVLink.ADSB_EMITTER_TYPE.SERVICE_SURFACE:
                    bitmap = adsb_service_surface;
                    break;
                case MAVLink.ADSB_EMITTER_TYPE.POINT_OBSTACLE:
                    bitmap = adsb_point_obstacle;
                    break;
            }
            lastEmitterCategory = EmitterCategory;
            bitmap = (Bitmap)bitmap.Clone();
            g.ScaleTransform(DrawScale, DrawScale);
            // Set the color based on alert level
            var fillColor = Color.Green;
            switch (AlertLevel)
            {
                case AlertLevelOptions.Green:
                    fillColor = Color.Green;
                    break;
                case AlertLevelOptions.Orange:
                    fillColor = Color.Orange;
                    break;
                case AlertLevelOptions.Red:
                    fillColor = Color.Red;
                    break;
            }
            lastAlertLevel = AlertLevel;

            if (IsOnGround)
            {
                fillColor = Color.DarkGray;
                fillColor = Color.FromArgb(128, fillColor);
            }
            lastIsOnGround = IsOnGround;

            ColorSprite(bitmap, fillColor);

            g.DrawImageUnscaled(bitmap, bitmap.Width / -2, bitmap.Height / -2);

            lastDrawn = (Bitmap)bitmap.Clone();

            g.Transform = temp;
        }
    }
}