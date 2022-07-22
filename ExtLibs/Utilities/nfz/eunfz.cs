namespace MissionPlanner.Utilities.nfz
{
    using System;
    using Flurl;
    using Flurl.Http;
    using Newtonsoft.Json;
    using GeoJSON.Net.Feature;
    using System.IO;
    using log4net;
    using System.Linq;

    public class EU
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static string filecache = Path.Combine(Settings.GetDataDirectory(), "eunfz.json");

        // always show it no matter what
        public static bool forceshow
        {
            get
            {
                return Settings.Instance.GetBoolean("eunfzforceshow");
            }
            set 
            { 
                Settings.Instance["eunfzforceshow"] = value.ToString();
            }
        }

        // user has chosen to show/hide it
        public static bool show
        {
            get
            {
                return Settings.Instance.GetBoolean("eunfzshow", true);
            }
            set
            {
                Settings.Instance["eunfzshow"] = value.ToString();
            }
        }

        public static bool asked
        {
            get
            {
                return Settings.Instance.GetBoolean("eunfzshowask", false);
            }
            set
            {
                Settings.Instance["eunfzshowask"] = value.ToString();
            }
        }

        public delegate bool cfnofly();
        public static event cfnofly ConfirmNoFly;

        static string[] countrys =
        {
"BE",
"BG",
"CZ",
"DK",
"DE",
"EE",
"IE",
"EL",
"ES",
"FR",
"HR",
"IT",
"CY",
"LV",
"LT",
"LU",
"HU",
"MT",
"NL",
"AT",
"PL",
"PT",
"RO",
"SI",
"SK",
"FI",
"SE"
        };

        public static async System.Threading.Tasks.Task<Nfz> LoadNFZ()
        {
            var result = await "https://www.cloudflare.com/cdn-cgi/trace".GetStringAsync();

            log.Debug(result);

            if (countrys.Any(a=>result.Contains("loc="+a)) && show || forceshow)
            {
                string url = "";

                if (ConfirmNoFly != null && asked == false)
                {
                    asked = true;
                    if (ConfirmNoFly.Invoke())
                    {
                        // user has chosen to show it
                        show = true;
                        forceshow = true;
                    }
                    else 
                    {
                        show = false;
                        return null;
                    }
                }

                Nfz nfzinfo;

                if (new FileInfo(filecache).LastWriteTimeUtc.AddHours(12) < DateTime.UtcNow || !File.Exists(filecache))
                {
                    nfzinfo = await url.GetJsonAsync<Nfz>();
                    try
                    {
                        File.WriteAllText(filecache, nfzinfo.ToJSON());
                    }
                    catch
                    {
                    }
                }
                else
                    nfzinfo = JsonConvert.DeserializeObject<Nfz>(File.ReadAllText(filecache));


                return nfzinfo;
            }
            return null;
        }
    }


        public partial class Nfz
        {
            [JsonProperty("title")]
            public string Title { get; set; }

            [JsonProperty("description")]
            public string Description { get; set; }

            [JsonProperty("features")]
            public Feature[] Features { get; set; }
        }

        public partial class Feature
        {
            [JsonProperty("identifier")]
            public long Identifier { get; set; }

            [JsonProperty("country")]
            public string Country { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("restriction")]
            public string Restriction { get; set; }

            [JsonProperty("restrictionConditions")]
            public string RestrictionConditions { get; set; }

            [JsonProperty("region")]
            public long Region { get; set; }

            [JsonProperty("reason")]
            public string[] Reason { get; set; }

            [JsonProperty("otherReasonInfo")]
            public string OtherReasonInfo { get; set; }

            [JsonProperty("regulationExemption")]
            public string RegulationExemption { get; set; }

            [JsonProperty("uSpaceClass")]
            public string USpaceClass { get; set; }

            [JsonProperty("message")]
            public string Message { get; set; }

            [JsonProperty("applicability")]
            public Applicability[] Applicability { get; set; }

            [JsonProperty("zoneAuthority")]
            public ZoneAuthority[] ZoneAuthority { get; set; }

            [JsonProperty("geometry")]
            public Geometry[] Geometry { get; set; }
        }

        public partial class Applicability
        {
            [JsonProperty("permanent")]
            public string Permanent { get; set; }
        }

        public partial class Geometry
        {
            [JsonProperty("uomDimensions")]
            public string UomDimensions { get; set; }

            [JsonProperty("lowerLimit")]
            public double LowerLimit { get; set; }

            [JsonProperty("lowerVerticalReference")]
            public string LowerVerticalReference { get; set; }

            [JsonProperty("upperLimit")]
            public double UpperLimit { get; set; }

            [JsonProperty("upperVerticalReference")]
            public string UpperVerticalReference { get; set; }

            [JsonProperty("horizontalProjection")]
            public HorizontalProjection HorizontalProjection { get; set; }
        }

        public partial class HorizontalProjection
        {
            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("coordinates", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public double[][][] Coordinates { get; set; }

            [JsonProperty("center", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public double[] Center { get; set; }

            [JsonProperty("radius", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public long? Radius { get; set; }
        }

        public partial class ZoneAuthority
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("service")]
            public string Service { get; set; }

            [JsonProperty("email")]
            public string Email { get; set; }

            [JsonProperty("contactName")]
            public string ContactName { get; set; }

            [JsonProperty("siteURL")]
            public string SiteUrl { get; set; }

            [JsonProperty("phone")]
            public string Phone { get; set; }

            [JsonProperty("purpose")]
            public string Purpose { get; set; }

            [JsonProperty("intervalBefore")]
            public string IntervalBefore { get; set; }
        }
    

}