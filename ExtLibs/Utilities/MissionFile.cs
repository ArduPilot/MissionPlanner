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
        public static RootObject ReadFile(string filename)
        {
            var file = File.ReadAllText(filename);

            var output = JsonConvert.DeserializeObject<RootObject>(file);

            return output;
        }

        public static void WriteFile(string filename, RootObject format)
        {
            var fileout = JsonConvert.SerializeObject(format, Formatting.Indented);

            File.WriteAllText(filename, fileout);
        }

        public static List<Locationwp> ConvertToLocationwps(RootObject format)
        {
            List<Locationwp> cmds = new List<Locationwp>();

            cmds.Add(ConvertFromMissionItem(format.mission.plannedHomePosition));

            foreach (var missionItem in format.mission.items)
            {
                if (missionItem.type != "SimpleItem")
                {
                    if (missionItem.type == "ComplexItem")
                    {
                        
                    }
                    continue;
                }
                cmds.Add(ConvertFromMissionItem(missionItem));
            }

            return cmds;
        }

        public static Locationwp ConvertFromMissionItem(List<double> missionItem)
        {
            return new Locationwp() {alt = (float)missionItem[2], lat = missionItem[0], lng = missionItem[1]};
        }

        public static Locationwp ConvertFromMissionItem(Item missionItem)
        {
            return missionItem;
        }

        public static Item ConvertFromLocationwp(Locationwp locationwp)
        {
            return locationwp;
        }

        //http://json2csharp.com/#
        public class GeoFence
        {
            public List<double> breachReturn { get; set; }
            public List<List<double>> polygon { get; set; }
            public int version { get; set; }
        }

        public class Camera
        {
            public int focalLength { get; set; }
            public int groundResolution { get; set; }
            public int imageFrontalOverlap { get; set; }
            public int imageSideOverlap { get; set; }
            public string name { get; set; }
            public bool orientationLandscape { get; set; }
            public int resolutionHeight { get; set; }
            public int resolutionWidth { get; set; }
            public double sensorHeight { get; set; }
            public double sensorWidth { get; set; }
        }

        public class Grid
        {
            public double altitude { get; set; }
            public int angle { get; set; }
            public bool relativeAltitude { get; set; }
            public double spacing { get; set; }
            public int turnAroundDistance { get; set; }
        }

        public class Item
        {
            public bool autoContinue { get; set; }
            public int command { get; set; }
            public List<double> coordinate { get; set; }
            public int doJumpId { get; set; }
            public int frame { get; set; }
            public List<double> @params { get; set; }
            public string type { get; set; }
            public Camera camera { get; set; }
            public int? cameraTriggerDistance { get; set; }
            public string complexItemType { get; set; }
            public bool? fixedValueIsAltitude { get; set; }
            public Grid grid { get; set; }
            public bool? hoverAndCapture { get; set; }
            public bool? manualGrid { get; set; }
            public List<List<double?>> polygon { get; set; }
            public bool? refly90Degrees { get; set; }
            public int? version { get; set; }
        }

        public class Mission
        {
            public int cruiseSpeed { get; set; }
            public int firmwareType { get; set; }
            public int hoverSpeed { get; set; }
            public List<Item> items { get; set; }
            public List<double> plannedHomePosition { get; set; }
            public int vehicleType { get; set; }
            public int version { get; set; }
        }

        public class RallyPoints
        {
            public List<List<double>> points { get; set; }
            public int version { get; set; }
        }

        public class RootObject
        {
            public string fileType { get; set; }
            public GeoFence geoFence { get; set; }
            public string groundStation { get; set; }
            public Mission mission { get; set; }
            public RallyPoints rallyPoints { get; set; }
            public int version { get; set; }
        }

        public static RootObject ConvertFromLocationwps(List<Locationwp> list, byte frame = (byte)MAVLink.MAV_FRAME.GLOBAL_RELATIVE_ALT)
        {
            RootObject temp = new RootObject()
            {
                groundStation = "MissionPlanner",
                version = 1
            };

            if (list.Count>0)
                temp.mission.plannedHomePosition = ConvertFromLocationwp(list[0]).coordinate.ToList();

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

                    // set frame type
                    temploc.frame = frame;

                    temp.mission.items.Add(temploc);

                    if (item.Tag != null)
                    {
                        //if (!temp.mission.complexItems.Contains(item.Tag))
                            //temp.complexItems.Add(item.Tag);
                    }

                    a++;
                }
            }

            return temp;
        }
    }
}