using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using log4net;
using Microsoft.Scripting.Utils;
using MissionPlanner.Properties;
using OpenTK.Graphics.ES20;

namespace MissionPlanner.Plugin
{
    public class PluginLoader
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static List<Plugin> Plugins = new List<Plugin>();

        static Assembly LoadFromSameFolder(object sender, ResolveEventArgs args)
        {
            if (args.RequestingAssembly == null)
                return null;
            string folderPath = Path.GetDirectoryName(args.RequestingAssembly.Location);
            string[] search = Directory.GetFiles(folderPath, new AssemblyName(args.Name).Name + ".dll",
                SearchOption.AllDirectories);

            foreach (var file in search)
            {
                Assembly assembly = Assembly.LoadFrom(file);
                if (assembly.FullName == args.Name) 
                    return assembly;
            }

            return null;
        }

        public static void Load(String file)
        {
            if (!File.Exists(file) || !file.EndsWith(".dll", true, null))
                return;

            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += new ResolveEventHandler(LoadFromSameFolder);

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

                    Object o = Activator.CreateInstance(pluginInfo, BindingFlags.Default, null, null, CultureInfo.CurrentUICulture);
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