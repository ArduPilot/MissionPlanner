﻿using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace MissionPlanner.Utilities
{
    /// <summary>
    /// This class loads and saves some handy app level settings so UI state is preserved across sessions.
    /// </summary>
    public class Settings
    {
        static Settings _instance;

        public static string AppConfigName { get; set; } = "Mission Planner";

        public static Settings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Settings();
                    try
                    {
                        _instance.Load();
                    } catch { }
                }
                return _instance;
            }
        }

        public Settings()
        {
        }

        /// <summary>
        /// use to store all internal config - use Instance
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

        public string this[string key, string defaultvalue]
        {
            get
            {
                string value = this[key];
                if (value == null)
                    value = defaultvalue;
                return value;
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

        public string UserAgent { get; set; } = "";
        
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

        public IEnumerable<string> GetList(string key)
        {
            if(config.ContainsKey(key))
                return config[key].Split(';');
            return new string[0];
        }

        public void SetList(string key, IEnumerable<string> list)
        {
            config[key] = list.Aggregate((s, s1) => s + ';' + s1);
        }

        public void AppendList(string key, string item)
        {
            var list = GetList(key).ToList();
            list.Add(item);
            SetList(key, list);
        }

        public int GetInt32(string key, int defaulti = 0)
        {
            int result = defaulti;
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

        public bool GetBoolean(string key, bool defaultb = false)
        {
            bool result = defaultb;
            string value = null;
            if (config.TryGetValue(key, out value))
            {
                bool.TryParse(value, out result);
            }
            return result;
        }

        public float GetFloat(string key, float defaultv = 0)
        {
            float result = defaultv;
            string value = null;
            if (config.TryGetValue(key, out value))
            {
                float.TryParse(value, out result);
            }
            return result;
        }

        public double GetDouble(string key, double defaultd = 0)
        {
            double result = defaultd;
            string value = null;
            if (config.TryGetValue(key, out value))
            {
                double.TryParse(value, out result);
            }
            return result;
        }

        public byte GetByte(string key, byte defaultb = 0)
        {
            byte result = defaultb;
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
            
            var ass = Assembly.GetEntryAssembly();

            if (ass == null)
                return "." + Path.DirectorySeparatorChar;

            var location = ass.Location;

            var path = Path.GetDirectoryName(location);

            return path + Path.DirectorySeparatorChar;
        }

        static bool isMono()
        {
            var t = Type.GetType("Mono.Runtime");
            return (t != null);
        }

        /// <summary>
        /// Shared data directory
        /// </summary>
        /// <returns></returns>
        public static string GetDataDirectory()
        {
            if (isMono())
            {
                return GetUserDataDirectory();
            }

            var path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + Path.DirectorySeparatorChar + AppConfigName +
                          Path.DirectorySeparatorChar;

            return path;
        }

        /// <summary>
        /// User specific data
        /// </summary>
        /// <returns></returns>
        public static string GetUserDataDirectory()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + Path.DirectorySeparatorChar + AppConfigName +
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
            if (!File.Exists(GetConfigFullPath()))
                return;

            try
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
            catch (Exception ex)
            {
                File.Copy(GetConfigFullPath(), GetConfigFullPath() + DateTime.Now.toUnixTime() + ".failed", true);
                throw;
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
