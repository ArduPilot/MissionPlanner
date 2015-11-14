using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using log4net;

namespace MissionPlanner.Plugin
{
    public class PluginLoader
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static List<Plugin> Plugins = new List<Plugin>();

        public static void Load(String file)
        {
            if (!File.Exists(file) || !file.EndsWith(".dll", true, null))
                return;

            Assembly asm = null;

            try
            {
                asm = Assembly.LoadFile(file);
            }
            catch (Exception)
            {
                // unable to load
                return;
            }

            Type pluginInfo = null;
            try
            {
                Type[] types = asm.GetTypes();
                Type type = typeof (MissionPlanner.Plugin.Plugin);
                foreach (var t in types)
                    if (type.IsAssignableFrom((Type) t))
                    {
                        pluginInfo = t;
                        break;
                    }

                if (pluginInfo != null)
                {
                    log.Info("Plugin Load " + file);

                    Object o = Activator.CreateInstance(pluginInfo);
                    Plugin plugin = (Plugin) o;

                    plugin.Assembly = asm;

                    plugin.Host = new PluginHost();

                    if (plugin.Init())
                    {
                        log.InfoFormat("Plugin Init {0} {1} by {2}", plugin.Name, plugin.Version, plugin.Author);
                        lock (Plugins)
                        {
                            Plugins.Add(plugin);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public static void LoadAll()
        {
            string path = Application.StartupPath + Path.DirectorySeparatorChar + "plugins" +
                          Path.DirectorySeparatorChar;

            if (!Directory.Exists(path))
                return;

            String[] files = Directory.GetFiles(path, "*.dll");
            foreach (var s in files)
                Load(Path.Combine(Environment.CurrentDirectory, s));

            for (Int32 i = 0; i < Plugins.Count; ++i)
            {
                lock (Plugins)
                {
                    Plugin p = Plugins.ElementAt(i);
                    try
                    {
                        if (!p.Loaded())
                        {
                            Plugins.RemoveAt(i);
                            --i;
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                        Plugins.RemoveAt(i);
                        --i;
                    }
                }
            }
        }
    }
}