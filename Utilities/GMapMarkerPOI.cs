﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using MissionPlanner.Properties;

namespace MissionPlanner.Utilities
{
    [Serializable]
    public class GMapMarkerPOI : GMarkerGoogle
    {
        public GMapMarkerPOI(PointLatLng p)
            : base(p, GMarkerGoogleType.red_dot)
        {
        }
    }
}