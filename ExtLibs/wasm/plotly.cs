using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace wasm
{
    public class plotly
    {
        public RootObject root = new RootObject();

        public plotly()
        {
            
        }

        public plotly(string name)
        {
            root.name = name;
        }

        public string getJSON()
        {
            return JsonConvert.SerializeObject(root);
        }

        public void fromJSON(string input)
        {
            root = JsonConvert.DeserializeObject<RootObject>(input);
        }

        public void AddXY(object x, object y)
        {
            if (root.x == null)
            {
                root.x = new List<object>();
                root.y = new List<object>();
            }

            root.x.Add(x);
            root.y.Add(y);
        }

        public void AddXYZ(object x, object y, object z)
        {
            if (root.z == null)
            {
                root.x = new List<object>();
                root.y = new List<object>();
                root.z = new List<object>();
            }

            root.x.Add(x);
            root.y.Add(y);
            root.z.Add(z);
        }

        public void AddLatLng(double lat, double lng)
        {
            if (root.lat == null)
            {
                root.lat = new List<double>();
                root.lon = new List<double>();
            }

            root.lat.Add(lat);
            root.lon.Add(lng);
        }
    }

    [Serializable]
    public class RootObject
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<object> x;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<object> y;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<object> z;

        public string mode = "lines";
        [DefaultValue("")]
        public string name = "";
        public string type = "scatter";
        [DefaultValue("")] // geo3
        public string geo = "";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<double> lat;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<double> lon;
    }
}
