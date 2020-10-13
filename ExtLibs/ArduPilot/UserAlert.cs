using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using LibVLC.NET;
using MissionPlanner.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MissionPlanner.ArduPilot
{
    public class UserAlertRoot: Dictionary<string,UserAlertItem>
    {
        public static UserAlertRoot Instance;

        public static string URL = "https://firmware.ardupilot.org/userAlerts/manifest.json";
        public static async Task<UserAlertRoot> GetAlerts()
        {
            var content = await new HttpClient().GetStringAsync(URL);

            File.WriteAllText(Settings.GetDataDirectory() + "UserAlerts.json", content);

            Instance = JsonConvert.DeserializeObject<UserAlertRoot>(content);

            return Instance;
        }
    }

    public partial class UserAlertItem
    {
        [JsonProperty("affectedFirmware", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> AffectedFirmware { get; set; }

        [JsonProperty("dateRaised", NullValueHandling = NullValueHandling.Ignore)]
        public long? DateRaised { get; set; }

        [JsonProperty("dateResolved", NullValueHandling = NullValueHandling.Ignore)]
        public long? DateResolved { get; set; }

        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        [JsonProperty("fixCommit", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> FixCommit { get; set; }

        [JsonProperty("hardwareLimited", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> HardwareLimited { get; set; }

        [JsonProperty("linkedInfo", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> LinkedInfo { get; set; }

        [JsonProperty("linkedIssue", NullValueHandling = NullValueHandling.Ignore)]
        public string LinkedIssue { get; set; }

        [JsonProperty("linkedPR", NullValueHandling = NullValueHandling.Ignore)]
        public string LinkedPr { get; set; }

        [JsonProperty("mitigation", NullValueHandling = NullValueHandling.Ignore)]
        public string Mitigation { get; set; }

        [JsonProperty("versionFixed", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> VersionFixed { get; set; }

        [JsonProperty("versionFrom", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> VersionFrom { get; set; }
    }
}
