using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace MissionPlanner.Utilities
{
    /// <summary>
    /// This class loads and saves some handy app level settings so UI state is preserved across sessions.
    /// </summary>
    public class Settings
    {
        static Settings _instance;

        public static Settings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Settings();
                }
                return _instance;
            }
        }

        public Settings()
        {
        }

        /// <summary>
        /// use to store all internal config
        /// </summary>
        public static Dictionary<string, string> config = new Dictionary<string, string>();

        const string FileName = "config.xml";

        public string this[string key]
        {
            get
            {
                string value = null;
                config.TryGetValue(key, out value);
                return value;
            }

            set
            {
                config[key] = value;
            }
        }

        public IEnumerable<string> Keys
        {
            // the "ToArray" makes this safe for someone to add items while enumerating.
            get { return config.Keys.ToArray(); }
        }
        public bool ContainsKey(string key)
        {
            return config.ContainsKey(key);
        }

        public string ComPort
        {
            get { return this["comport"]; }
            set { this["comport"] = value; }
        }

        public string APMFirmware
        {
            get { return this["APMFirmware"]; }
            set { this["APMFirmware"] = value; }
        }

        public string BaudRate
        {
            get { return this["baudrate"]; }
            set { this["baudrate"] = value; }
        }

        public string LogDir
        {
            get
            {
                string dir = this["logdirectory"];
                if (string.IsNullOrEmpty(dir))
                {
                    dir = GetDefaultLogDir();
                }
                return dir;
            }
            set
            {
                this["logdirectory"] = value;
            }
        }

        public int Count { get { return config.Count; } }

        static string GetDefaultLogDir()
        {
            string directory = Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + @"logs";
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            return Path.Combine(directory, @"logs");
        }

        internal int GetInt32(string key)
        {
            int result = 0;
            string value = null;
            if (config.TryGetValue(key, out value))
            {
                int.TryParse(value, out result);
            }
            return result;
        }

        internal bool GetBoolean(string key)
        {
            bool result = false;
            string value = null;
            if (config.TryGetValue(key, out value))
            {
                bool.TryParse(value, out result);
            }
            return result;
        }

        internal float GetFloat(string key)
        {
            float result = 0f;
            string value = null;
            if (config.TryGetValue(key, out value))
            {
                float.TryParse(value, out result);
            }
            return result;
        }

        internal double GetDouble(string key)
        {
            double result = 0D;
            string value = null;
            if (config.TryGetValue(key, out value))
            {
                double.TryParse(value, out result);
            }
            return result;
        }

        internal byte GetByte(string key)
        {
            byte result = 0;
            string value = null;
            if (config.TryGetValue(key, out value))
            {
                byte.TryParse(value, out result);
            }
            return result;
        }

        public static string GetFullPath()
        {
            string directory = Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar;
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            return Path.Combine(directory, FileName);
        }
        
        public void Load()
        {
            using (XmlTextReader xmlreader = new XmlTextReader(GetFullPath()))
            {
                while (xmlreader.Read())
                {
                    if (xmlreader.NodeType == XmlNodeType.Element)
                    {
                        try
                        {
                            switch (xmlreader.Name)
                            {
                                case "Config":
                                    break;
                                case "xml":
                                    break;
                                default:
                                    config[xmlreader.Name] = xmlreader.ReadString();
                                    break;
                            }
                        }
                        // silent fail on bad entry
                        catch (Exception)
                        {
                        }
                    }
                }
            }
        }

        public void Save()
        {
            string filename = GetFullPath();

            using (XmlTextWriter xmlwriter = new XmlTextWriter(filename, Encoding.UTF8))
            {
                xmlwriter.Formatting = Formatting.Indented;

                xmlwriter.WriteStartDocument();

                xmlwriter.WriteStartElement("Config");

                foreach (string key in config.Keys)
                {
                    try
                    {
                        if (key == "" || key.Contains("/")) // "/dev/blah"
                            continue;

                        xmlwriter.WriteElementString(key, ""+config[key]);
                    }
                    catch
                    {
                    }
                }

                xmlwriter.WriteEndElement();

                xmlwriter.WriteEndDocument();
                xmlwriter.Close();
            }
        }

        public void Remove(string key)
        {
            if (config.ContainsKey(key))
            {
                config.Remove(key);
            }
        }

    }
}
