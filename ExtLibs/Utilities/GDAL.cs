using System;
using System.Collections.Generic;
using System.Text;
using GMap.NET.MapProviders;

namespace MissionPlanner.Utilities
{
    public class GDAL
    {
        public static IGDAL GDALBase;

        public static void ScanDirectory(string s)
        {
            if(GDALBase != null)
                GDALBase.ScanDirectory(s);
        }

        public static event Action<double, string> OnProgress;

        public static GMapProvider GetProvider()
        {
            if(GDALBase != null)
                return GDALBase.GetProvider();
            return null;
        }
    }

    public interface IGDAL
    {
        void ScanDirectory(string s);
        GMapProvider GetProvider();
    }
}
