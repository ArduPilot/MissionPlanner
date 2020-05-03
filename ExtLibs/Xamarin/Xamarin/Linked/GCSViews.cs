using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace MissionPlanner.Utilities
{
    internal class GCSViews
    {
        GCSViews()
        {
            instance = this;
            myhud = this;
            FlightPlanner = this;
        }

        internal GCSViews myhud;
        internal GCSViews instance;
        internal List<PointLatLngAlt> pointlist = new List<PointLatLngAlt>();
        internal bool streamjpgenable = false;
        internal MemoryStream streamjpg = new MemoryStream();
        internal Control mymap = new Control();

        public static GCSViews FlightData { get; internal set; } = new GCSViews();
        public static GCSViews FlightPlanner { get; internal set; }
    }
}