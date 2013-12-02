using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MissionPlanner.Log
{
    public class DFLog
    {
        public struct Label
        {
            public int Id;
            public string Format;
            public string[] FieldNames;

            public int Length;
            public string Name;
        }

        public static Dictionary<string, Label> logformat = new Dictionary<string, Label>();

        public static void Clear()
        {
            logformat.Clear();
        }

        public static void FMTLine(string strLine)
        {
            try
            {
                strLine = strLine.Replace(", ", ",");
                strLine = strLine.Replace(": ", ":");

                string[] items = strLine.Split(',', ':');

                string[] names = new string[items.Length - 5];
                Array.ConstrainedCopy(items, 5, names, 0, names.Length);

                Label lbl = new Label() { Name = items[3], Id = int.Parse(items[1]), Format = items[4], Length = int.Parse(items[2]), FieldNames = names };

                logformat[lbl.Name] = lbl;
            }
            catch { }
        }
    }
}
