using System.Windows.Forms;
using MissionPlanner;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;
using Newtonsoft.Json;

public static class Extension
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
