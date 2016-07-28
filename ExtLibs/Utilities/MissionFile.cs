using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace MissionPlanner.Utilities
{
    public class MissionFile
    {
        public static void test()
        {
            var file = File.ReadAllText(@"C:\Users\michael\Desktop\logs\FileFormat.mission");

            var output = JsonConvert.DeserializeObject<Format>(file);

            var fileout = JsonConvert.SerializeObject(output);
        }

        public static Format ReadFile(string filename)
        {
            var file = File.ReadAllText(filename);

            var output = JsonConvert.DeserializeObject<Format>(file);

            return output;
        }

        public static void WriteFile(string filename, Format format)
        {
            var fileout = JsonConvert.SerializeObject(format, Formatting.Indented);

            File.WriteAllText(filename, fileout);
        }

        public static List<Locationwp> ConvertToLocationwps(Format format)
        {
            List<Locationwp> cmds = new List<Locationwp>();

            cmds.Add(ConvertFromMissionItem(format.plannedHomePosition));

            foreach (var missionItem in format.items)
            {
                cmds.Add(ConvertFromMissionItem(missionItem));
            }

            return cmds;
        }

        public static Locationwp ConvertFromMissionItem(MissionItem missionItem)
        {
            return missionItem;
        }

        public static MissionItem ConvertFromLocationwp(Locationwp locationwp)
        {
            return locationwp;
        }

        public class Format
        {
            public int MAV_AUTOPILOT;
            public List<object> complexItems = new List<object>();
            public string groundStation;
            public List<MissionItem> items= new List<MissionItem>();
            public MissionItem plannedHomePosition;
            public string version = "1.0";
        }

        public class MissionItem
        {
            public bool autoContinue = true;
            public UInt16 command;
            public double[] coordinate = new double[3];
            public byte frame;
            public UInt16 id;
            public Single param1;
            public Single param2;
            public Single param3;
            public Single param4;
            public string type;
        }

        public class SurveyItem
        {
            public bool cameraTrigger = false;
            public double cameraTriggerDistance;
            public double gridAltitude;
            public bool gridAltitudeRelative;
            public double gridAngle;
            public double gridSpacing;
            public int id;
            public List<double[]> polygon = new List<double[]>();
            public string type;
            public int version;
        }

        public static Format ConvertFromLocationwps(List<Locationwp> list, byte frame = (byte)MAVLink.MAV_FRAME.GLOBAL_RELATIVE_ALT)
        {
            Format temp = new Format()
            {
                MAV_AUTOPILOT = (int)MAVLink.MAV_AUTOPILOT.ARDUPILOTMEGA,
                groundStation = "MissionPlanner",
                version = "1.0"
            };

            if (list.Count>0)
                temp.plannedHomePosition = ConvertFromLocationwp(list[0]);

            if (list.Count > 1)
            {
                int a = -1;
                foreach (var item in list)
                {
                    // skip home
                    if (a == -1)
                    {
                        a++;
                        continue;
                    }

                    var temploc = ConvertFromLocationwp(item);

                    // set id count
                    temploc.id = (ushort)a;
                    // set frame type
                    temploc.frame = frame;

                    temp.items.Add(temploc);

                    if (item.Tag != null)
                    {
                        if (!temp.complexItems.Contains(item.Tag))
                            temp.complexItems.Add(item.Tag);
                    }

                    a++;
                }
            }

            return temp;
        }
    }
}