namespace GMap.NET.WindowsForms
{
    //using System.Windows.Forms;
    using GMap.NET;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Runtime.Serialization;

    /// <summary>
    /// GMap.NET route
    /// </summary>
    [Serializable]
#if !PocketPC
    public class GMapRoute : MapRoute, ISerializable, IDeserializationCallback, IDisposable
#else
    public class GMapRoute : MapRoute, IDisposable
#endif
    {
        GMapOverlay overlay;
        public GMapOverlay Overlay
        {
            get
            {
                return overlay;
            }
            internal set
            {
                overlay = value;
            }
        }

        private bool visible = true;

        /// <summary>
        /// is marker visible
        /// </summary>
        public bool IsVisible
        {
            get
            {
                return visible;
            }
            set
            {
                if (value != visible)
                {
                    visible = value;

                    if (Overlay != null && Overlay.Control != null)
                    {
                        if (visible)
                        {
                            Overlay.Control.UpdateRouteLocalPosition(this);
                        }
                        else
                        {
                            if (Overlay.Control.IsMouseOverRoute)
                            {
                                Overlay.Control.IsMouseOverRoute = false;
#if !PocketPC
                                Overlay.Control.RestoreCursorOnLeave();
#endif
                            }
                        }
                        if (!Overlay.Control.HoldInvalidation)
                        {
                            Overlay.Control.Core.Refresh.Set();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// can receive input
        /// </summary>
        public bool IsHitTestVisible = false;

        private bool isMouseOver = false;

        /// <summary>
        /// is mouse over
        /// </summary>
        public bool IsMouseOver
        {
            get
            {
                return isMouseOver;
            }
            set
            {
                isMouseOver = value;
            }
        }

        /// <summary>
        /// Controls how directional arrows are drawn on the route.
        /// </summary>
        public enum ArrowDrawMode
        {
            /// <summary>No arrows.</summary>
            None,
            /// <summary>Arrow at the midpoint of every line segment (legacy behaviour).</summary>
            PerSegment,
            /// <summary>Single arrow at the midpoint of the entire route.</summary>
            SinglePerRoute
        }
        public ArrowDrawMode ArrowMode { get; set; } = ArrowDrawMode.None;
        /// <summary>Arrow arm length in pixels.</summary>
        public float ArrowLength = 15;
        /// <summary>Minimum total route length in pixels before an arrow is drawn (SinglePerRoute mode).</summary>
        public double MinSegmentPixels = 20;

#if !PocketPC
        /// <summary>
        /// Indicates whether the specified point is contained within this System.Drawing.Drawing2D.GraphicsPath
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool IsInside(int x, int y)
        {
            if (graphicsPath != null)
            {
                return graphicsPath.IsOutlineVisible(x, y, Stroke);
            }

            return false;
        }

        GraphicsPath graphicsPath;
        public void UpdateGraphicsPath()
        {
            if (graphicsPath == null)
            {
                graphicsPath = new GraphicsPath();
            }
            else
            {
                graphicsPath.Reset();
            }

            {
                for (int i = 0; i < LocalPoints.Count; i++)
                {
                    GPoint p2 = LocalPoints[i];

                    if (Math.Abs(p2.X) > 99000 || Math.Abs(p2.Y) > 99000)
                    {
                        if (Stroke.DashStyle != DashStyle.Solid)
                            Stroke.DashStyle = DashStyle.Solid;
                    }

                    if (i == 0)
                    {
                        graphicsPath.AddLine(p2.X, p2.Y, p2.X, p2.Y);
                    }
                    else
                    {
                        System.Drawing.PointF p = graphicsPath.GetLastPoint();
                        graphicsPath.AddLine(p.X, p.Y, p2.X, p2.Y);
                    }
                }
            }
        }
#endif

        public virtual void OnRender(IGraphics g)
        {
#if !PocketPC
            if (IsVisible && graphicsPath != null)
            {
                Pen Stroke = (Pen)this.Stroke.Clone();

                if (Stroke.DashStyle == DashStyle.Custom)
                {
                    ArrowMode = ArrowDrawMode.PerSegment;
                    Stroke.DashStyle = DashStyle.Solid;
                }

                if (graphicsPath.PointCount > 400)
                {
                    Console.WriteLine("route OnRender Large Graphics Path " + graphicsPath.PointCount);
                }

                g.DrawPath(Stroke, graphicsPath);

                if (ArrowMode != ArrowDrawMode.None && graphicsPath.PointCount > 1)
                {
                    double deg2rad = Math.PI / 180.0;
                    double rad2deg = 1 / deg2rad;

                    if (ArrowMode == ArrowDrawMode.PerSegment)
                    {
                        PointF last = PointF.Empty;
                        foreach (PointF item in graphicsPath.PathPoints)
                        {
                            if (last == PointF.Empty)
                            {
                                last = item;
                                continue;
                            }

                            float polx = item.X - last.X;
                            float poly = item.Y - last.Y;

                            // distance
                            double r = Math.Sqrt(Math.Pow(polx, 2) + Math.Pow(poly, 2));

                            if (r <= 20)
                                continue;

                            // angle
                            double angle = Math.Atan2(poly, polx);

                            if (double.IsNaN(angle))
                                continue;

                            float midx = polx / 2;
                            float midy = poly / 2;

                            float midxstart = last.X + midx;
                            float midystart = last.Y + midy;

                            double leftangle = angle + 210 * deg2rad;
                            double rightangle = angle - 210 * deg2rad;

                            float length = 15;

                            g.DrawLine(Stroke, midxstart, midystart, midxstart + length * (float)Math.Cos(leftangle), midystart + length * (float)Math.Sin(leftangle));
                            g.DrawLine(Stroke, midxstart, midystart, midxstart + length * (float)Math.Cos(rightangle), midystart + length * (float)Math.Sin(rightangle));

                            last = item;
                        }
                    }
                    else if (ArrowMode == ArrowDrawMode.SinglePerRoute)
                    {
                        var pts = graphicsPath.PathPoints;
                        double totalLen = 0;
                        for (int i = 1; i < pts.Length; i++)
                        {
                            float dx = pts[i].X - pts[i - 1].X;
                            float dy = pts[i].Y - pts[i - 1].Y;
                            totalLen += Math.Sqrt(dx * dx + dy * dy);
                        }

                        if (totalLen > MinSegmentPixels) // reuse as min route length
                        {
                            double target = totalLen * 0.5; // middle of the route
                            double acc = 0;

                            for (int i = 1; i < pts.Length; i++)
                            {
                                PointF p0 = pts[i - 1];
                                PointF p1 = pts[i];

                                float dx = p1.X - p0.X;
                                float dy = p1.Y - p0.Y;
                                double segLen = Math.Sqrt(dx * dx + dy * dy);

                                if (segLen <= 0)
                                    continue;

                                if (acc + segLen >= target)
                                {
                                    double t = (target - acc) / segLen;
                                    float midx = (float)(p0.X + t * dx);
                                    float midy = (float)(p0.Y + t * dy);

                                    double angle = Math.Atan2(dy, dx);
                                    if (!double.IsNaN(angle))
                                    {
                                        double leftAngle = angle + 210 * deg2rad;
                                        double rightAngle = angle - 210 * deg2rad;
                                        float len = ArrowLength;

                                        g.DrawLine(Stroke, midx, midy,
                                            midx + len * (float)Math.Cos(leftAngle),
                                            midy + len * (float)Math.Sin(leftAngle));
                                        g.DrawLine(Stroke, midx, midy,
                                            midx + len * (float)Math.Cos(rightAngle),
                                            midy + len * (float)Math.Sin(rightAngle));
                                    }
                                    break;
                                }

                                acc += segLen;
                            }
                        }
                    }
                }
            }
#else
            if (IsVisible)
            {
                Point[] pnts = new Point[LocalPoints.Count];
                for (int i = 0; i < LocalPoints.Count; i++)
                {
                    Point p2 = new Point((int)LocalPoints[i].X, (int)LocalPoints[i].Y);
                    pnts[pnts.Length - 1 - i] = p2;
                }

                if (pnts.Length > 1)
                {
                    g.DrawLines(Stroke, pnts);
                }
            }
#endif
        }

#if !PocketPC
        public static readonly Pen DefaultStroke = new Pen(Color.FromArgb(144, Color.MidnightBlue));
#else
        public static readonly Pen DefaultStroke = new Pen(Color.MidnightBlue);
#endif

        /// <summary>
        /// specifies how the outline is painted
        /// </summary>
        [NonSerialized]
        public Pen Stroke = DefaultStroke;

        public readonly List<GPoint> LocalPoints = new List<GPoint>();

        static GMapRoute()
        {
#if !PocketPC
            DefaultStroke.LineJoin = LineJoin.Round;
#endif
            DefaultStroke.Width = 5;
        }

        public GMapRoute(string name)
            : base(name)
        {

        }

        public GMapRoute(IEnumerable<PointLatLng> points, string name)
            : base(points, name)
        {

        }

#if !PocketPC
        #region ISerializable Members

        // Temp store for de-serialization.
        private GPoint[] deserializedLocalPoints;

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization.</param>
        /// <exception cref="T:System.Security.SecurityException">
        /// The caller does not have the required permission.
        /// </exception>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("Visible", this.IsVisible);
            info.AddValue("LocalPoints", this.LocalPoints.ToArray());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GMapRoute"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected GMapRoute(SerializationInfo info, StreamingContext context)
           : base(info, context)
        {
            //this.Stroke = Extensions.GetValue<Pen>(info, "Stroke", new Pen(Color.FromArgb(144, Color.MidnightBlue)));
            this.IsVisible = Extensions.GetStruct<bool>(info, "Visible", true);
            this.deserializedLocalPoints = Extensions.GetValue<GPoint[]>(info, "LocalPoints");
        }

        #endregion

        #region IDeserializationCallback Members

        /// <summary>
        /// Runs when the entire object graph has been de-serialized.
        /// </summary>
        /// <param name="sender">The object that initiated the callback. The functionality for this parameter is not currently implemented.</param>
        public override void OnDeserialization(object sender)
        {
            base.OnDeserialization(sender);

            // Accounts for the de-serialization being breadth first rather than depth first.
            LocalPoints.AddRange(deserializedLocalPoints);
            LocalPoints.Capacity = Points.Count;
        }

        #endregion
#endif

        #region IDisposable Members

        bool disposed = false;

        public virtual void Dispose()
        {
            if (!disposed)
            {
                disposed = true;

                LocalPoints.Clear();

#if !PocketPC
                if (graphicsPath != null)
                {
                    graphicsPath.Dispose();
                    graphicsPath = null;
                }
#endif
                base.Clear();
            }
        }

        #endregion
    }

    public delegate void RouteClick(GMapRoute item, /*MouseEventArgs*/ object mouseEventArgs);
    public delegate void RouteEnter(GMapRoute item);
    public delegate void RouteLeave(GMapRoute item);
}
