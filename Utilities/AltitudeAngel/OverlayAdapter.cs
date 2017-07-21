using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using AltitudeAngel.IsolatedPlugin.Common.Maps;
using GeoJSON.Net.Feature;
using GMap.NET;
using GMap.NET.WindowsForms;

namespace MissionPlanner.Utilities.AltitudeAngel
{
    class OverlayAdapter : IOverlay
    {
        private readonly GMapOverlay _overlay;
        private readonly SynchronizationContext _context;

        public OverlayAdapter(GMapOverlay overlay)
        {
            _context = new WindowsFormsSynchronizationContext();
            _overlay = overlay;
        }

        public bool IsVisible
        {
            get
            {
                bool value = false;
                try
                {
                    _context.Send(state =>
                    {
                        value = _overlay.IsVisibile;
                    }, null);
                }
                catch (InvalidAsynchronousStateException)
                {

                }
                return value;
            }
            set
            {
                try
                {
                    _context.Send(state =>
                    {
                        _overlay.IsVisibile = value;
                    }, null);
                }
                catch (InvalidAsynchronousStateException)
                {

                }
            }
        }

        public void AddPolygon(string name, List<PointLatLng> points, ColorInfo colorInfo, Feature featureInfo)
        {
            try
            {
                _context.Send(_ =>
                {
                    _overlay.Polygons.Add(new GMapPolygon(points, name)
                    {
                        Fill = new SolidBrush(Color.FromArgb((int) colorInfo.FillColor)),
                        Stroke = new Pen(Color.FromArgb((int) colorInfo.StrokeColor), colorInfo.StrokeWidth),
                        IsHitTestVisible = true,
                        Tag = featureInfo
                    });

                }, null);
            }
            catch (InvalidAsynchronousStateException)
            {

            }
        }

        public bool PolygonExists(string name)
        {
            bool polygonExists = false;

            try
            {
                _context.Send(_ => polygonExists = _overlay.Polygons.Any(i => i.Name == name), null);
            }
            catch (InvalidAsynchronousStateException)
            {

            }

            return polygonExists;
        }

        public void AddLine(string name, List<PointLatLng> points, ColorInfo colorInfo, Feature featureInfo)
        {
            try
            {
                _context.Send(_ =>
                {
                    _overlay.Routes.Add(new GMapRoute(points, name)
                    {
                        Stroke = new Pen(Color.FromArgb((int) colorInfo.StrokeColor), colorInfo.StrokeWidth + 2),
                        IsHitTestVisible = true,
                        Tag = featureInfo
                    });
                }, null);
            }
            catch (InvalidAsynchronousStateException)
            {

            }
        }

        public bool LineExists(string name)
        {
            bool exists = false;
            try
            {
                _context.Send(_ => exists = _overlay.Routes.Any(i => i.Name == name), null);
            }
            catch (InvalidAsynchronousStateException)
            {

            }
            return exists;
        }
    }
}