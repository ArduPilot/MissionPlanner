using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Xml;

namespace Core.Xml
{
    public static class XmlTypeAssociator<T>
    {
        private static Dictionary<string, Type> namedTypes = new Dictionary<string, Type>();        

        /// <summary>
        /// This allows us to keep a list with a serializer for each type because XmlSerializer objects reference assemblies and are expensive...
        /// </summary>
        public static XmlSerializer GetSerializer(string str)
        {
            return GetSerializer(FindType(str));
        }
        public static XmlSerializer GetSerializer(Type t)
        {
            return XMLSerializeManager.GetSerializer(t);
        }
        public static XmlSerializer GetSerializer()
        {
            return GetSerializer(typeof(T));
        }
        public static XmlSerializer GetSerializer(XmlReader r)
        {
            if (typeof(T).Name.ToLower() == r.Name.ToLower()) {
                return GetSerializer();
            }
            return GetSerializer(r.Name);
        }

        public static string GetName(Type t)
        {
            if (!t.IsGenericType) {
                return t.Name;
            }
            string name = t.Name.Substring(0, t.Name.Length - 2);
            foreach (Type genT in t.GetGenericArguments()) {
                name += "Of" + GetName(genT);
            }
            return name;
        }

        public static void AddType(Type t)
        {
            AddType(GetName(t), t);
        }
        public static void AddType(string nam, Type t)
        {
            namedTypes.Add(nam, t);
        }

        private static Type FindType(string value)
        {
            if (!namedTypes.ContainsKey(value)) {                
                throw new Exception("Could not find a Type for this value: " + value + ". Use XmlList<T>.AddType to include the appropriate type.");
            }
            return namedTypes[value];
        }

        public static List<Type> GetAllTypes()
        {
            List<Type> ans = new List<Type>();
            foreach (Type t in namedTypes.Values) {
                ans.Add(t);
            }
            return ans;
        }

        public static bool Exists()
        {
            return namedTypes.Count > 0;
        }


    }
}
