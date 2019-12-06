using System;
using System.Collections.Generic;

namespace MissionPlanner.Drawing
{
    public class FontFamily
    {
        public static FontFamily GenericSansSerif { get; set; }
        public static FontFamily GenericMonospace { get; set; }

        public static IEnumerable<FontFamily> Families
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public string Name
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public FontFamily()
        {

        }

        public int GetCellAscent(FontStyle fontStyle)
        {
            return 0;
        }

        public int GetCellDescent(FontStyle fontStyle)
        {
            return 0;
        }

        public bool IsStyleAvailable(FontStyle bold)
        {
            return false;
        }
    }
}