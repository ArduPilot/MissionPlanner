using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Parse 'fmtp' attribute in SDP
// Extract H265 fields
// By Roger Hardiman, RJH Technical Consultancy Ltd

namespace Rtsp.Sdp
{
    public class H265Parameters : IDictionary<String, String>
    {
        private readonly Dictionary<String, String> parameters = new Dictionary<string, string>();

        public List<byte[]> SpropParameterSets
        {
            get
            {
                List<byte[]> result = new List<byte[]>();

                if (ContainsKey("sprop-vps")&& this["sprop-vps"] != null)
                {
                    result.AddRange(this["sprop-vps"].Split(',').Select(x => Convert.FromBase64String(x)));
                }

                if (ContainsKey("sprop-sps") && this["sprop-sps"] != null)
                {
                    result.AddRange(this["sprop-sps"].Split(',').Select(x => Convert.FromBase64String(x)));
                }

                if (ContainsKey("sprop-pps") && this["sprop-pps"] != null)
                {
                    result.AddRange(this["sprop-pps"].Split(',').Select(x => Convert.FromBase64String(x)));
                }
                return result;
            }
        }

        public static H265Parameters Parse(String parameterString)
        {
            var result = new H265Parameters();
            foreach (var pair in parameterString.Split(';').Select(x => x.Trim().Split(new char[] { '=' }, 2)))
            {
                if(!string.IsNullOrWhiteSpace(pair[0]))
                    result[pair[0]] = pair.Length > 1 ? pair[1] : null;
            }
            return result;
        }

        public override string ToString()
        {
            return parameters.Select(p => p.Key + (p.Value != null ? "=" + p.Value : string.Empty)).Aggregate((x, y) => x + ";" + y);
        }

        public String this[String index]
        {
            get { return parameters[index]; }
            set { parameters[index] = value; }
        }

        public int Count
        {
            get
            {
                return parameters.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return ((IDictionary<string, string>)parameters).IsReadOnly;
            }
        }

        public ICollection<string> Keys
        {
            get
            {
                return ((IDictionary<string, string>)parameters).Keys;
            }
        }

        public ICollection<string> Values
        {
            get
            {
                return ((IDictionary<string, string>)parameters).Values;
            }
        }

        public void Add(KeyValuePair<string, string> item)
        {
            ((IDictionary<string, string>)parameters).Add(item);
        }

        public void Add(string key, string value)
        {
            parameters.Add(key, value);
        }

        public void Clear()
        {
            parameters.Clear();
        }

        public bool Contains(KeyValuePair<string, string> item)
        {
            return ((IDictionary<string, string>)parameters).Contains(item);
        }

        public bool ContainsKey(string key)
        {
            return parameters.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            ((IDictionary<string, string>)parameters).CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return ((IDictionary<string, string>)parameters).GetEnumerator();
        }

        public bool Remove(KeyValuePair<string, string> item)
        {
            return ((IDictionary<string, string>)parameters).Remove(item);
        }

        public bool Remove(string key)
        {
            return parameters.Remove(key);
        }

        public bool TryGetValue(string key, out string value)
        {
            return parameters.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IDictionary<string, string>)parameters).GetEnumerator();
        }
    }
}
