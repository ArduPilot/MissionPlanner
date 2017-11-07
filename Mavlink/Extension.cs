using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MissionPlanner.Mavlink
{
    static class Extension
    {
        public static string ToJSON(this MAVLink.MAVLinkMessage msg)
        {
            return JsonConvert.SerializeObject(msg);
        }

        public static MAVLink.MAVLinkMessage FromJSON(this string msg)
        {
            return JsonConvert.DeserializeObject<MAVLink.MAVLinkMessage>(msg);
        }
    }
}
