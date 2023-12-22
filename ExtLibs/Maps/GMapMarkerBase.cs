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
        public static bool DisplayCOGSetting = true;
        public static bool DisplayHeadingSetting = true;
        public static bool DisplayNavBearingSetting = true;
        public static bool DisplayRadiusSetting = true;
        public static bool DisplayTargetSetting = true;
        public static int length = 500;
        public static InactiveDisplayStyleEnum InactiveDisplayStyle = InactiveDisplayStyleEnum.Normal;
        
        // Instance variables
        public bool IsActive = true;
        protected bool IsHidden => InactiveDisplayStyle == InactiveDisplayStyleEnum.Hidden && !IsActive;
        protected bool IsTransparent => InactiveDisplayStyle == InactiveDisplayStyleEnum.Transparent && !IsActive;
        protected bool DisplayCOG => DisplayCOGSetting && !IsTransparent;
        protected bool DisplayHeading => DisplayHeadingSetting && !IsTransparent;
        protected bool DisplayNavBearing => DisplayNavBearingSetting && !IsTransparent;
        protected bool DisplayRadius => DisplayRadiusSetting && !IsTransparent;
        protected bool DisplayTarget => DisplayTargetSetting && !IsTransparent;
        
        public GMapMarkerBase(PointLatLng pos):base(pos)
        {
        }

        public enum InactiveDisplayStyleEnum
        {
            Normal,
            Transparent,
            Hidden
        }
    }
}
