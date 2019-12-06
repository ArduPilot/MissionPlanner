using System;
using System.Collections.Generic;
using System.Text;
using GMap.NET;
using GMap.NET.WindowsForms;

namespace MissionPlanner.Maps
{
    [Serializable]
    public class GMapMarkerBase: GMapMarker
    {
        public static bool DisplayCOG = true;
        public static bool DisplayHeading = true;
        public static bool DisplayNavBearing = true;
        public static bool DisplayRadius = true;
        public static bool DisplayTarget = true;
        public static int length = 500;

        public GMapMarkerBase(PointLatLng pos):base(pos)
        {
        }
    }
}
