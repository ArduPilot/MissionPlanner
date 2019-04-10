using System.Collections.Generic;
using System.IO;
using MissionPlanner.Utilities;
using Newtonsoft.Json;

namespace MissionPlanner.Swarm.Sequence
{
    public class Sequence
    {
        public List<Layout> Layouts { get; set; } = new List<Layout>();
        public List<string> Steps { get; set; } = new List<string>();

        public static Sequence Load(string filename)
        {
            var seq = JsonConvert.DeserializeObject<Sequence>(File.ReadAllText(filename));

            return seq;
        }

        public void Save(string filename)
        {
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);

            File.WriteAllText(filename, json);
        }
    }

    public class Layout
    {
        public string Id { get; set; }

        public int DelayStart { get; set; }

        public int DelayEnd { get; set; }

        public Dictionary<int, Vector3> Offset = new Dictionary<int, Vector3>();

        public void AddOffset(int sysid, Vector3 offset)
        {
            Offset[sysid] = offset;
        }

        public override string ToString()
        {
            return Id;
        }
    }
}
