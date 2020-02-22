using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using log4net;
using Newtonsoft.Json;

namespace MissionPlanner.Utilities
{
    public class WaypointFile
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static List<Locationwp> ReadWaypointFile(string file)
        {
            int wp_count = 0;
            bool error = false;
            List<Locationwp> cmds = new List<Locationwp>();
            StreamReader sr = new StreamReader(File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
            string header = sr.ReadLine();
            if (header == null || !header.Contains("QGC WPL"))
            {
                CustomMessageBox.Show("Invalid Waypoint file");
                return cmds;
            }

            while (!error && !sr.EndOfStream)
            {
                string line = sr.ReadLine();
                // waypoints

                if (line.StartsWith("#"))
                    continue;

                //seq/cur/frame/mode
                string[] items = line.Split(new[] { '\t', ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);

                if (items.Length <= 9)
                    continue;

                try
                {
                    // check to see if the first wp is index 0/home.
                    // if it is not index 0, add a blank home point
                    if (wp_count == 0 && items[0] != "0")
                    {
                        cmds.Add(new Locationwp());
                    }

                    Locationwp temp = new Locationwp();
                    temp.frame = (byte)int.Parse(items[2], CultureInfo.InvariantCulture);

                    temp.id = (ushort)Enum.Parse(typeof(MAVLink.MAV_CMD), items[3], false);
                    temp.p1 = float.Parse(items[4], CultureInfo.InvariantCulture);

                    if (temp.id == 99)
                        temp.id = 0;

                    temp.alt = (float)(double.Parse(items[10], CultureInfo.InvariantCulture));
                    temp.lat = (double.Parse(items[8], CultureInfo.InvariantCulture));
                    temp.lng = (double.Parse(items[9], CultureInfo.InvariantCulture));

                    temp.p2 = (float)(double.Parse(items[5], CultureInfo.InvariantCulture));
                    temp.p3 = (float)(double.Parse(items[6], CultureInfo.InvariantCulture));
                    temp.p4 = (float)(double.Parse(items[7], CultureInfo.InvariantCulture));

                    cmds.Add(temp);

                    wp_count++;
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                    CustomMessageBox.Show("Line invalid\n" + line);
                }
            }

            sr.Close();
            return cmds;
        }
    }
    public class MissionFile
    {
        public static RootObject ReadFile(string filename)
        {
            using (var file =
                new StreamReader(File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
            {
                var output = JsonConvert.DeserializeObject<RootObject>(file.ReadToEnd());

                return output;
            }
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
                        foreach (var item in missionItem.TransectStyleComplexItem.Items)
                        {
                            cmds.Add(ConvertFromMissionItem(item));
                        }
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
      
        public class Circle2
        {
            public List<double> center { get; set; }
            public double radius { get; set; }
        }

        public class Circle
        {
            public Circle2 circle { get; set; }
            public bool inclusion { get; set; }
            public int version { get; set; }
        }

        public class Polygon
        {
            public bool inclusion { get; set; }
            public List<List<double>> polygon { get; set; }
            public int version { get; set; }
        }

        public class GeoFence
        {
            public List<Circle> circles { get; set; }
            public List<Polygon> polygons { get; set; }
            public int version { get; set; }
        }
        public class CameraCalc
        {
            public int AdjustedFootprintFrontal { get; set; }
            public int AdjustedFootprintSide { get; set; }
            public string CameraName { get; set; }
            public int DistanceToSurface { get; set; }
            public bool DistanceToSurfaceRelative { get; set; }
            public int version { get; set; }
        }
        public class TransectStyleComplexItem
        {
            public CameraCalc CameraCalc { get; set; }
            public int CameraShots { get; set; }
            public bool CameraTriggerInTurnAround { get; set; }
            public bool FollowTerrain { get; set; }
            public bool HoverAndCapture { get; set; }
            public List<Item> Items { get; set; }
            public bool Refly90Degrees { get; set; }
            public int TurnAroundDistance { get; set; }
            public List<List<double>> VisualTransectPoints { get; set; }
            public int version { get; set; }
        }
        public class Item
        {
            public bool autoContinue { get; set; }
            public int command { get; set; }
            public int doJumpId { get; set; }
            public int frame { get; set; }
            public List<double?> @params { get; set; }
            public string type { get; set; }
            public TransectStyleComplexItem TransectStyleComplexItem { get; set; }
            public int? angle { get; set; }
            public string complexItemType { get; set; }
            public int? entryLocation { get; set; }
            public bool? flyAlternateTransects { get; set; }
            public List<List<double?>> polygon { get; set; }
            public bool? splitConcavePolygons { get; set; }
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

            if (list.Count > 0)
                temp.mission.plannedHomePosition =
                    ConvertFromLocationwp(list[0]).@params.Skip(4).Select(a => a ?? 0.0).ToList();

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