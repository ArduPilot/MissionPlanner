#if net4
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;

namespace fastJSON
{
    internal class DynamicJson : DynamicObject
    {
        private IDictionary<string, object> Dictionary { get; set; }

        public DynamicJson(string json)
        {
            var dictionary = fastJSON.JSON.Instance.Parse(json);
            if (dictionary is IDictionary<string, object>)
                this.Dictionary = (IDictionary<string, object>)dictionary;
        }

        private DynamicJson(object dictionary)
        {
            if (dictionary is IDictionary<string, object>)
                this.Dictionary = (IDictionary<string, object>)dictionary;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (this.Dictionary.TryGetValue(binder.Name, out result) == false)
                if (this.Dictionary.TryGetValue(binder.Name.ToLower(), out result) == false)
                    return false;// throw new Exception("property not found " + binder.Name);

            if (result is IDictionary<string, object>)
                result = new DynamicJson(result as IDictionary<string, object>);

            else if (result is List<object> && (result as List<object>).First() is IDictionary<string, object>)
                result = new List<DynamicJson>((result as List<object>).Select(x => new DynamicJson(x as IDictionary<string, object>)));

            else if (result is List<object>)
                result = result as List<object>;

            return this.Dictionary.ContainsKey(binder.Name);
        }
    }
}
#endif