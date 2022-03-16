using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using MissionPlanner.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Windows.Forms;

namespace MissionPlanner.Controls
{
    /// <summary>
    /// Mono handles calls from other thread difrently - this prevents those crashs
    /// </summary>
    public class myGMAP : GMap.NET.WindowsForms.GMapControl
    {
        public bool inOnPaint = false;
        string otherthread = "";
        int lastx = 0;
        int lasty = 0;
        public myGMAP()
            : base()
        {
            this.Text = "Map";
            IgnoreMarkerOnMouseWheel = true;
            GestureHappened += MyGMAP_GestureHappened;
        }

        GMapMarker p1 = new GMarkerGoogle(PointLatLng.Empty, GMarkerGoogleType.blue_small);
        GMapMarker p2 = new GMarkerGoogle(PointLatLng.Empty, GMarkerGoogleType.blue_small);

        private void MyGMAP_GestureHappened(object sender, GestureEventArgs e)
        {
            if(e.Operation == Gestures.Pan)
            {
                p1.Position = e.FirstPointLL;
                p2.Position = e.SecondPointLL;
                Overlays.First().Markers.Add(p1);
                Overlays.First().Markers.Add(p2);

                //Core.Position = e.FirstPointLL;

                PointLatLng point = FromLocalToLatLng(e.SecondPoint.X, e.SecondPoint.Y);

                double latdif = e.FirstPointLL.Lat - point.Lat;
                double lngdif = e.FirstPointLL.Lng - point.Lng;

                //Position = new PointLatLng(Position.Lat + latdif,
                  //  Position.Lng + lngdif);
            }
            if(e.Operation== Gestures.Zoom)
            {
                // aim is to get the ll listed under the users finger

                var startlocal = FromLatLngToLocal(e.FirstPointLL);

                p1.Position = e.FirstPointLL;
                p2.Position = e.SecondPointLL;
                Overlays.First().Markers.Add(p1);
                Overlays.First().Markers.Add(p2);


                var scale = e.DistanceBetweenNow / (float)e.DistanceBetweenStart;

                // use the start center as our center;
                //Position = FromLocalToLatLng((int)startlocal.X, (int)startlocal.Y);
                Core.Position = e.FirstPointLL;
                //Core.mouseLastZoom.X = e.SecondPoint.X;
                //Core.mouseLastZoom.Y = e.SecondPoint.Y;

                Core.MouseWheelZooming = true;
                Zoom += (scale - 1);
                Core.MouseWheelZooming = false;

                iArguments = e.DistanceBetweenNow;

                Console.WriteLine("zoom " + e.FirstPoint + " " + e.SecondPoint + " ") ;
            }
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            var start = DateTime.Now;

            if (inOnPaint)
            {
                Console.WriteLine("Was in onpaint Gmap th:" + System.Threading.Thread.CurrentThread.Name + " in " + otherthread);
                return;
            }

            otherthread = System.Threading.Thread.CurrentThread.Name;

            inOnPaint = true;

            try
            {
                base.OnPaint(e);
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }

            inOnPaint = false;

            var end = DateTime.Now;

            System.Diagnostics.Debug.WriteLine("map draw time " + (end-start).TotalMilliseconds);
        }

        protected override void OnInvalidated(InvalidateEventArgs e)
        {
            base.OnInvalidated(e);
        }

        public new void Invalidate()
        {
            base.Invalidate();
        }

        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                var buffer = 1;
                // try prevent alot of cpu usage
                if (e.X >= lastx - buffer && e.X <= lastx + buffer && e.Y >= lasty - buffer && e.Y <= lasty + buffer)
                    return;

                if (HoldInvalidation)
                    return;

                lastx = e.X;
                lasty = e.Y;

                base.OnMouseMove(e);
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }










        private Point first_point = new Point();
        private Point second_point = new Point();
        PointLatLng first_pointLL = new PointLatLng();
        PointLatLng second_pointLL = new PointLatLng();
        private int iArguments = 0;
        private const Int64 ULL_ARGUMENTS_BIT_MASK = 0xFFFFFFFFL;
        private const int WM_GESTURENOTIFY = 0x11A;
        private const int WM_GESTURE = 0x119;
        private const int GC_ALLGESTURES = 0x1;
        private const int GID_BEGIN = 1;
        private const int GID_END = 2;
        private const int GID_ZOOM = 3;
        private const int GID_PAN = 4;
        private const int GID_ROTATE = 5;
        private const int GID_TWOFINGERTAP = 6;
        private const int GID_PRESSANDTAP = 7;
        private const int GF_BEGIN = 0x1;
        private const int GF_INERTIA = 0x2;
        private const int GF_END = 0x4;
        private struct GESTURECONFIG
        {
            public int dwID;
            public int dwWant;
            public int dwBlock;
        }
        public struct POINTS
        {
            public short x;
            public short y;
        }
        private struct GESTUREINFO
        {
            public int cbSize;
            public int dwFlags;
            public int dwID;
            public IntPtr hwndTarget;
            [MarshalAs(UnmanagedType.Struct)]
            public POINTS ptsLocation;
            public int dwInstanceID;
            public int dwSequenceID;
            public Int64 ullArguments;
            public int cbExtraArgs;
        }
        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetGestureConfig(IntPtr hWnd, int dwReserved, int cIDs, ref GESTURECONFIG pGestureConfig, int cbSize);

        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetGestureInfo(IntPtr hGestureInfo, ref GESTUREINFO pGestureInfo);


        private int _gestureConfigSize;
        private int _gestureInfoSize;
        [SecurityPermission(SecurityAction.Demand)]
        private void SetupStructSizes()
        {
            _gestureConfigSize = Marshal.SizeOf(new GESTURECONFIG());
            _gestureInfoSize = Marshal.SizeOf(new GESTUREINFO());
        }
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        protected override void WndProc(ref Message m)
        {
            if (_gestureConfigSize == 0)
                SetupStructSizes();

            bool handled;
            switch (m.Msg)
            {
                case WM_GESTURENOTIFY:
                    {
                        GESTURECONFIG gc = new GESTURECONFIG();
                        gc.dwID = GID_ZOOM;
                        gc.dwWant = GC_ALLGESTURES;
                        gc.dwBlock = 0;
                        bool bResult = SetGestureConfig(Handle, 0, 1, ref gc, _gestureConfigSize);
                        if (!bResult)
                            throw new Exception("Error in execution of SetGestureConfig");
                        handled = true;
                        break;
                    }

                case WM_GESTURE:
                    {
                        handled = DecodeGesture(ref m);
                        break;
                    }

                default:
                    {
                        handled = false;
                        break;
                    }
            }
            base.WndProc(ref m);
            if (handled)
            {
                try
                {
                    m.Result = new IntPtr(1);
                }
                catch (Exception excep)
                {
                    Debug.Print("Could not allocate result ptr");
                    Debug.Print(excep.ToString());
                }
            }
        }
        private bool DecodeGesture(ref Message m)
        {
            GESTUREINFO gi;
            try
            {
                gi = new GESTUREINFO();
            }
            catch (Exception excep)
            {
                Debug.Print("Could not allocate resources to decode gesture");
                Debug.Print(excep.ToString());
                return false;
            }
            gi.cbSize = _gestureInfoSize;
            if (!GetGestureInfo(m.LParam, ref gi))
                return false;
            switch (gi.dwID)
            {
                case GID_BEGIN:
                case GID_END:
                    {
                        break;
                    }

                case GID_TWOFINGERTAP:
                    {
                        break;
                    }

                case GID_ZOOM:
                    {
                        switch (gi.dwFlags)
                        {
                            case GF_BEGIN:
                                {
                                    iArguments = System.Convert.ToInt32(gi.ullArguments & ULL_ARGUMENTS_BIT_MASK);
                                    first_point.X = gi.ptsLocation.x;
                                    first_point.Y = gi.ptsLocation.y;
                                    first_pointLL = FromLocalToLatLng(PointToClient(first_point));
                                    break;
                                }

                            default:
                                {
                                    second_point.X = gi.ptsLocation.x;
                                    second_point.Y = gi.ptsLocation.y;
                                    second_pointLL = FromLocalToLatLng(PointToClient(second_point));
                                    GestureHappened?.Invoke(this, new GestureEventArgs() { Operation = Gestures.Zoom, DistanceBetweenStart = iArguments, DistanceBetweenNow = System.Convert.ToInt32(gi.ullArguments & ULL_ARGUMENTS_BIT_MASK), FirstPoint = first_point, SecondPoint = second_point, FirstPointLL = first_pointLL, SecondPointLL = second_pointLL });
                                    break;
                                }
                        }

                        break;
                    }

                case GID_PAN:
                    {
                        switch (gi.dwFlags)
                        {
                            case GF_BEGIN:
                                {
                                    first_point.X = gi.ptsLocation.x;
                                    first_point.Y = gi.ptsLocation.y;
                                    first_pointLL = FromLocalToLatLng(PointToClient(first_point));
                                    break;
                                }

                            default:
                                {
                                    second_point.X = gi.ptsLocation.x;
                                    second_point.Y = gi.ptsLocation.y;
                                    second_pointLL = FromLocalToLatLng(PointToClient(second_point));
                                    GestureHappened?.Invoke(this, new GestureEventArgs() { Operation = Gestures.Pan, FirstPoint = first_point, SecondPoint = second_point, FirstPointLL = first_pointLL, SecondPointLL = second_pointLL });
                                    break;
                                }
                        }

                        break;
                    }

                case GID_PRESSANDTAP:
                    {
                        break;
                    }

                case GID_ROTATE:
                    {
                        break;
                    }
            }
            return true;
        }


        public enum Gestures
        {
            Pan,
            Zoom
        }

        public class GestureEventArgs : EventArgs
        {
            public Gestures Operation { get; set; }
            public Point FirstPoint { get; set; }
            public Point SecondPoint { get; set; }
            public PointLatLng FirstPointLL { get; set; }
            public PointLatLng SecondPointLL { get; set; }
            public int DistanceBetweenStart { get; set; }
            public int DistanceBetweenNow { get; set; }
        }

        public event GestureHappenedEventHandler GestureHappened;

        public delegate void GestureHappenedEventHandler(object sender, GestureEventArgs e);
    }
}
