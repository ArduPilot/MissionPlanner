namespace MissionPlanner.Utilities.nfz
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using System.Linq;
    using System.Net.Http;
    using Flurl;
    using Flurl.Http;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Linq;
    using GeoJSON.Net;
    using GeoJSON.Net.Feature;
    using GeoJSON.Net.Geometry;
    using System.IO;
    using log4net;

    public class HK
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static string filecache = Path.Combine(Settings.GetDataDirectory(), "hknfz.json");

        // always show it no matter what
        public static bool forceshow
        {
            get
            {
                return Settings.Instance.GetBoolean("hknfzforceshow");
            }
            set 
            { 
                Settings.Instance["hknfzforceshow"] = value.ToString();
            }
        }

        // user has chosen to show/hide it
        public static bool show
        {
            get
            {
                return Settings.Instance.GetBoolean("hknfzshow", true);
            }
            set
            {
                Settings.Instance["hknfzshow"] = value.ToString();
            }
        }

        public static bool asked
        {
            get
            {
                return Settings.Instance.GetBoolean("hknfzshowask", false);
            }
            set
            {
                Settings.Instance["hknfzshowask"] = value.ToString();
            }
        }

        public delegate bool cfnofly();
        public static event cfnofly ConfirmNoFly;

        public static async System.Threading.Tasks.Task<FeatureCollection> LoadNFZ()
        {
            var result = await "https://www.cloudflare.com/cdn-cgi/trace".GetStringAsync();

            log.Debug(result);

            if (result.Contains("loc=HK") && show || forceshow)
            {
                string url = "https://esua.cad.gov.hk/web/droneMap/api/nfz?apiKey=a04e6ffec803f6c08126423c32316712";

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

                FeatureCollection nfzinfo;

                if (new FileInfo(filecache).LastWriteTimeUtc.AddHours(12) < DateTime.UtcNow || !File.Exists(filecache))
                {
                    nfzinfo = await url.GetJsonAsync<FeatureCollection>();
                    try
                    {
                        File.WriteAllText(filecache, nfzinfo.ToJSON());
                    }
                    catch
                    {
                    }
                }
                else
                    nfzinfo = JsonConvert.DeserializeObject<FeatureCollection>(File.ReadAllText(filecache));


                return nfzinfo;
            }
            return null;
        }
    }
}