using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace ArdupilotMega.Plugin
{
    public class PluginLoader
    {
        public static List<IPlugin> Plugins = new List<IPlugin>();

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
                Type type = typeof(ArdupilotMega.Plugin.IPlugin);
                foreach (var t in types)
                    if (type.IsAssignableFrom((Type)t))
                    {
                        pluginInfo = t;
                        break;
                    }

                if (pluginInfo != null)
                {
                    Object o = Activator.CreateInstance(pluginInfo);
                    IPlugin plugin = (IPlugin)o;
                    if (plugin.Init())
                    {
                        Plugins.Add(plugin);
                    }
                }
            }
            catch (Exception)
            {
            }

        }

        public static void LoadAll()
        {
            String[] files = Directory.GetFiles(Application.StartupPath +  Path.DirectorySeparatorChar+ "Plugins" +  Path.DirectorySeparatorChar, "*.dll");
            foreach (var s in files)
                Load(Path.Combine(Environment.CurrentDirectory, s));

            for (Int32 i = 0; i < Plugins.Count; ++i)
            {
                IPlugin p = Plugins.ElementAt(i);
                try
                {
                    if (!p.Loaded())
                    {
                        Plugins.RemoveAt(i);
                        --i;
                    }

                    p.Host = MainV2.instance;

                    p.Init();
                }
                catch (Exception)
                {
                    Plugins.RemoveAt(i);
                    --i;
                }
            }
        }
    }
}
