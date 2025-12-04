using System;
using System.Drawing;
using System.Windows.Forms;
using GMap.NET.WindowsForms;

namespace MissionPlanner.Controls
{
    /// <summary>
    /// A scale bar that draws directly on the map for true transparency.
    /// </summary>
    public class MapScaleBar
    {
        private GMapControl _mapControl;
        private double _currentZoom = -1;
        private int _barWidth = 100;
        private string _distanceText = "";
        private int _drawX = 70;
        private int _drawY = 8; // offset from bottom

        // Scale values to display (in meters)
        private static readonly double[] ScaleValues = new double[]
        {
            1, 2, 5, 10, 20, 50, 100, 200, 500,
            1000, 2000, 5000, 10000, 20000, 50000, 100000,
            200000, 500000, 1000000, 2000000, 5000000
        };

        public int DrawX
        {
            get => _drawX;
            set => _drawX = value;
        }

        public int DrawY
        {
            get => _drawY;
            set => _drawY = value;
        }

        /// <summary>
        /// Sets the map control to track for zoom changes.
        /// </summary>
        public void SetMapControl(GMapControl mapControl)
        {
            _mapControl = mapControl;
            if (_mapControl != null)
            {
                _mapControl.OnMapZoomChanged += MapControl_OnMapZoomChanged;
                UpdateScale();
            }
        }

        private void MapControl_OnMapZoomChanged()
        {
            UpdateScale();
            _mapControl?.Invalidate();
        }

        /// <summary>
        /// Call this to manually update the scale bar.
        /// </summary>
        public void UpdateScale()
        {
            if (_mapControl == null)
                return;

            double zoom = _mapControl.Zoom;
            if (Math.Abs(zoom - _currentZoom) < 0.01)
                return;

            _currentZoom = zoom;

            // Calculate meters per pixel at current zoom and latitude
            double lat = _mapControl.Position.Lat;
            double metersPerPixel = 156543.03392 * Math.Cos(lat * Math.PI / 180) / Math.Pow(2, zoom);

            // Find the best scale value that fits in approximately 200 pixels
            double targetDistance = metersPerPixel * 200;

            // Find the closest scale value
            double bestScale = ScaleValues[0];
            foreach (var scale in ScaleValues)
            {
                if (scale <= targetDistance * 1.5)
                    bestScale = scale;
                else
                    break;
            }

            _barWidth = (int)(bestScale / metersPerPixel);
            _barWidth = Math.Max(80, Math.Min(500, _barWidth));

            // Format the distance text
            if (bestScale >= 1000)
                _distanceText = (bestScale / 1000).ToString("0.##") + " km";
            else
                _distanceText = bestScale.ToString("0") + " m";
        }

        /// <summary>
        /// Draw the scale bar directly on the map. Call this from the map's Paint event.
        /// </summary>
        public void DoPaintRemote(PaintEventArgs e, Control parent)
        {
            if (_mapControl == null)
                return;

            UpdateScale();

            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            int barHeight = 4;
            int x = _drawX;
            int y = parent.Height - _drawY - barHeight;

            // Draw bar background (dark)
            using (var bgBrush = new SolidBrush(Color.FromArgb(180, 0, 0, 0)))
            {
                g.FillRectangle(bgBrush, x - 1, y - 1, _barWidth + 2, barHeight + 2);
            }

            // Draw scale bar (white)
            using (var barBrush = new SolidBrush(Color.White))
            {
                g.FillRectangle(barBrush, x, y, _barWidth, barHeight);
            }

            // Draw end caps
            using (var capPen = new Pen(Color.White, 1))
            {
                g.DrawLine(capPen, x, y - 3, x, y + barHeight + 2);
                g.DrawLine(capPen, x + _barWidth - 1, y - 3, x + _barWidth - 1, y + barHeight + 2);
            }

            // Draw distance text with shadow
            using (var font = new Font("Segoe UI", 8f, FontStyle.Bold))
            {
                var textSize = g.MeasureString(_distanceText, font);
                float textX = x + (_barWidth - textSize.Width) / 2;
                float textY = y - textSize.Height - 2;

                // Shadow
                using (var shadowBrush = new SolidBrush(Color.FromArgb(180, 0, 0, 0)))
                {
                    g.DrawString(_distanceText, font, shadowBrush, textX + 1, textY + 1);
                }

                // Text
                using (var textBrush = new SolidBrush(Color.White))
                {
                    g.DrawString(_distanceText, font, textBrush, textX, textY);
                }
            }
        }

        public void Dispose()
        {
            if (_mapControl != null)
            {
                _mapControl.OnMapZoomChanged -= MapControl_OnMapZoomChanged;
            }
        }
    }
}
