using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MissionPlanner.GeoRef
{
    public class PictureInformation : SingleLocation
    {
        string path;

        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        DateTime shotTimeReportedByCamera;

        public DateTime ShotTimeReportedByCamera
        {
            get { return shotTimeReportedByCamera; }
            set { shotTimeReportedByCamera = value; }
        }

        int width;

        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        int height;

        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        public PictureInformation()
        {
            width = 3200;
            height = 2400;
        }
    }
}