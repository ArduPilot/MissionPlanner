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
            var fileout = JsonConvert.SerializeObject(format);

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
            Locationwp temp = new Locationwp();
            if (missionItem.frame == 3)
            {
                temp.options = 1;
            }
            else
            {
                temp.options = 0;
            }
            temp.id = missionItem.command;
            temp.p1 = missionItem.param1;

            if (temp.id == 99)
                temp.id = 0;

            temp.alt = (float)missionItem.coordinate[2];
            temp.lat = missionItem.coordinate[0];
            temp.lng = missionItem.coordinate[1];

            temp.p2 = missionItem.param2;
            temp.p3 = missionItem.param3;
            temp.p4 = missionItem.param4;

            return temp;
        }

        [DataContract]
        public class Format
        {
            [DataMember] public int MAV_AUTOPILOT;
            [DataMember] public List<object> complexItems;
            [DataMember] public string groundStation;
            [DataMember] public List<MissionItem> items;
            [DataMember] public MissionItem plannedHomePosition;
            [DataMember] public string version;
        }

        [DataContract]
        public class MissionItem
        {
            [DataMember] public bool autoContinue;
            [DataMember] public UInt16 command;
            [DataMember] public double[] coordinate;
            [DataMember] public byte frame;
            [DataMember] public int id;
            [DataMember] public Single param1;
            [DataMember] public Single param2;
            [DataMember] public Single param3;
            [DataMember] public Single param4;
            [DataMember] public string type;
        }
    }
}