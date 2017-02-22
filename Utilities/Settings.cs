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
            get
            {
                try
                {
                    return this[ComPort + "_BAUD"];
                }
                catch
                {
                    return "";
                }
            }
            set { this[ComPort + "_BAUD"] = value; }
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

        public static string GetDefaultLogDir()
        {
            string directory = GetUserDataDirectory() + @"logs";
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            return directory;
        }

        public int GetInt32(string key)
        {
            int result = 0;
            string value = null;
            if (config.TryGetValue(key, out value))
            {
                int.TryParse(value, out result);
            }
            return result;
        }

        public DisplayView GetDisplayView(string key)
        {
            DisplayView result = new DisplayView();
            string value = null;
            if (config.TryGetValue(key, out value))
            {
                DisplayViewExtensions.TryParse(value, out result);
            }
            return result;
        }

        public bool GetBoolean(string key)
        {
            bool result = false;
            string value = null;
            if (config.TryGetValue(key, out value))
            {
                bool.TryParse(value, out result);
            }
            return result;
        }

        public float GetFloat(string key)
        {
            float result = 0f;
            string value = null;
            if (config.TryGetValue(key, out value))
            {
                float.TryParse(value, out result);
            }
            return result;
        }

        public double GetDouble(string key)
        {
            double result = 0D;
            string value = null;
            if (config.TryGetValue(key, out value))
            {
                double.TryParse(value, out result);
            }
            return result;
        }

        public byte GetByte(string key)
        {
            byte result = 0;
            string value = null;
            if (config.TryGetValue(key, out value))
            {
                byte.TryParse(value, out result);
            }
            return result;
        }

        /// <summary>
        /// Install directory path
        /// </summary>
        /// <returns></returns>
        public static string GetRunningDirectory()
        {
            return Application.StartupPath + Path.DirectorySeparatorChar;
        }

        /// <summary>
        /// Shared data directory
        /// </summary>
        /// <returns></returns>
        public static string GetDataDirectory()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + Path.DirectorySeparatorChar + "Mission Planner" +
                          Path.DirectorySeparatorChar;

            return path;
        }

        /// <summary>
        /// User specific data
        /// </summary>
        /// <returns></returns>
        public static string GetUserDataDirectory()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + Path.DirectorySeparatorChar + "Mission Planner" +
                          Path.DirectorySeparatorChar;

            return path;
        }

        /// <summary>
        /// full path to the config file
        /// </summary>
        /// <returns></returns>
        static string GetConfigFullPath()
        {
            // old path details
            string directory = GetRunningDirectory();
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var path = Path.Combine(directory, FileName);

            // get new path details
            var newdir = GetUserDataDirectory();

            if (!Directory.Exists(newdir))
            {
                Directory.CreateDirectory(newdir);
            }

            var newpath = Path.Combine(newdir, FileName);

            // check if oldpath config exists
            if (File.Exists(path))
            {
                // is new path exists already, then dont do anything
                if (!File.Exists(newpath))
                {
                    // move to new path
                    File.Move(path, newpath);

                    // copy other xmls as this will be first run
                    var files = Directory.GetFiles(directory, "*.xml", SearchOption.TopDirectoryOnly);

                    foreach (var file in files)
                    {
                        File.Copy(file, newdir + Path.GetFileName(file));
                    }
                }
            }

            return newpath;
        }
        
        public void Load()
        {
            using (XmlTextReader xmlreader = new XmlTextReader(GetConfigFullPath()))
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
            string filename = GetConfigFullPath();

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
