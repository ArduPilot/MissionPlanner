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
    public class UserAlert : Dictionary<string, UserAlertItem>
    {
        public static UserAlert Instance;

        public static string URL = "https://firmware.ardupilot.org/useralerts/manifest.json";

        public static async Task<UserAlert> GetAlerts()
        {
            var content = await new HttpClient().GetStringAsync(URL);

            File.WriteAllText(Settings.GetDataDirectory() + "UserAlerts.json", content);

            Instance = JsonConvert.DeserializeObject<UserAlert>(content);

            return Instance;
        }
    }

    /// <summary>
    /// A single User Alert for ArduPilot
    /// </summary>
    public partial class UserAlertItem
    {
        /// <summary>
        /// Which ArduPilot firmware is affected. Use comma separated value to specify multiple
        /// vehicles if 'all' does not work.
        /// </summary>
        [JsonProperty("affectedFirmware", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> AffectedFirmware { get; set; }

        /// <summary>
        /// An assessment of the likelihood of the issue occurring. 1 = CRITICAL - Likely to be
        /// encountered by all vehicle configurations. 2 = CRITICAL - Likely to be encountered by
        /// specific vehicle configurations, 3 = MAJOR - possible to be encountered. 4 = MINOR -
        /// unlikely to be encountered.
        /// </summary>
        [JsonProperty("criticality", NullValueHandling = NullValueHandling.Ignore)]
        public long? Criticality { get; set; }

        /// <summary>
        /// Date that this User Alert was raised in YYYYMMDD format.
        /// </summary>
        [JsonProperty("dateRaised", NullValueHandling = NullValueHandling.Ignore)]
        public long? DateRaised { get; set; }

        /// <summary>
        /// Date in YYYYMMDD format that this User Alert was resolved. 'Resolved' in this case means
        /// a patched ArduPilot version has been released for ALL affected vehicle and board types,
        /// and no further edits to this User Alert are expected. null is not resolved
        /// </summary>
        [JsonProperty("dateResolved")]
        public long? DateResolved { get; set; }

        /// <summary>
        /// Textual description of the User Alert. Should be understandable by an average user.
        /// </summary>
        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        /// <summary>
        /// Commit ID of the fix (on master branch) for this Issue. Can be multiple commits if the
        /// fix commits are spread among different libraries. Empty array if no fix yet. If this
        /// field is not [], it is considered that the issue has been fixed in master.
        /// </summary>
        [JsonProperty("fixCommit", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> FixCommit { get; set; }

        /// <summary>
        /// If the User Alert is only applicable to certain boards, list the board names. An empty
        /// array indicates that all boards are affected.
        /// </summary>
        [JsonProperty("hardwareLimited", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> HardwareLimited { get; set; }

        /// <summary>
        /// URLs to any supporting information about the issue, such as forum posts.
        /// </summary>
        [JsonProperty("linkedInfo", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> LinkedInfo { get; set; }

        /// <summary>
        /// URL to Issue in ArduPilot GitHub repository. null if no issue.
        /// </summary>
        [JsonProperty("linkedIssue")]
        public Uri LinkedIssue { get; set; }

        /// <summary>
        /// URL to the fix PR in ArduPilot GitHub repo. null if there is not PR yet.
        /// </summary>
        [JsonProperty("linkedPR")]
        public Uri LinkedPr { get; set; }

        /// <summary>
        /// Textual description of any mitigations that a user can take to prevent the issue from
        /// occurring BEFORE a patched ArduPilot is released. Should be understandable by an average
        /// user.
        /// </summary>
        [JsonProperty("mitigation", NullValueHandling = NullValueHandling.Ignore)]
        public string Mitigation { get; set; }

        /// <summary>
        /// ArduPilot release which contains fix. List must cover all firmwares listed in “Affected
        /// firmware”. It is assumed that all versions between VersionFrom and this are affected by
        /// the User Alert. This field is an empty if there is no fixed version yet.
        /// </summary>
        [JsonProperty("versionFixed", NullValueHandling = NullValueHandling.Ignore)]
        public VersionFixed VersionFixed { get; set; }

        /// <summary>
        /// ArduPilot release which introduced the issue, if known. Empty assumes all previous
        /// versions. It must cover all firmwares listed in 'Affected firmware'.
        /// </summary>
        [JsonProperty("versionFrom", NullValueHandling = NullValueHandling.Ignore)]
        public VersionFrom VersionFrom { get; set; }
    }

    /// <summary>
    /// ArduPilot release which contains fix. List must cover all firmwares listed in “Affected
    /// firmware”. It is assumed that all versions between VersionFrom and this are affected by
    /// the User Alert. This field is an empty if there is no fixed version yet.
    /// </summary>
    public partial class VersionFixed
    {
        [JsonProperty("antenna", NullValueHandling = NullValueHandling.Ignore)]
        public string Antenna { get; set; }

        [JsonProperty("copter", NullValueHandling = NullValueHandling.Ignore)]
        public string Copter { get; set; }

        [JsonProperty("plane", NullValueHandling = NullValueHandling.Ignore)]
        public string Plane { get; set; }

        [JsonProperty("rover", NullValueHandling = NullValueHandling.Ignore)]
        public string Rover { get; set; }

        [JsonProperty("sub", NullValueHandling = NullValueHandling.Ignore)]
        public string Sub { get; set; }
    }

    /// <summary>
    /// ArduPilot release which introduced the issue, if known. Empty assumes all previous
    /// versions. It must cover all firmwares listed in 'Affected firmware'.
    /// </summary>
    public partial class VersionFrom
    {
        [JsonProperty("antenna", NullValueHandling = NullValueHandling.Ignore)]
        public string Antenna { get; set; }

        [JsonProperty("copter", NullValueHandling = NullValueHandling.Ignore)]
        public string Copter { get; set; }

        [JsonProperty("plane", NullValueHandling = NullValueHandling.Ignore)]
        public string Plane { get; set; }

        [JsonProperty("rover", NullValueHandling = NullValueHandling.Ignore)]
        public string Rover { get; set; }

        [JsonProperty("sub", NullValueHandling = NullValueHandling.Ignore)]
        public string Sub { get; set; }
    }
}