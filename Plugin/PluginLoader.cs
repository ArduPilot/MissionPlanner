﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using log4net;
using MissionPlanner.Utilities;

namespace MissionPlanner.Plugin
{
    public class PluginLoader
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static List<Plugin> Plugins = new List<Plugin>();

        public static Dictionary<string,string[]> filecache = new Dictionary<string, string[]>();

        static Assembly LoadFromSameFolder(object sender, ResolveEventArgs args)
        {
            if (args.RequestingAssembly == null)
                return null;

            // check install folder
            string folderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (filecache.ContainsKey(folderPath))
            {

            }
            else
            {
                string[] search1 = Directory.GetFiles(folderPath, "*.dll",
                    SearchOption.AllDirectories);

                filecache[folderPath] = search1;
            }

            foreach (var file in filecache[folderPath].Where(a=>a.ToLower().Contains(new AssemblyName(args.Name).Name.ToLower() + ".dll")))
            {
                try
                {
                    Assembly assembly = Assembly.LoadFrom(file);
                    if (assembly.FullName == args.Name)
                        return assembly;
                } catch { }
            }

            // check local directory
            folderPath = Path.GetDirectoryName(args.RequestingAssembly.Location);
            if (filecache.ContainsKey(folderPath))
            {

            }
            else
            {
                string[] search1 = Directory.GetFiles(folderPath, "*.dll",
                    SearchOption.AllDirectories);

                filecache[folderPath] = search1;
            }

            foreach (var file in filecache[folderPath].Where(a => a.ToLower().Contains(new AssemblyName(args.Name).Name.ToLower() + ".dll")))
            {
                try
                {
                    Assembly assembly = Assembly.LoadFrom(file);
                    if (assembly.FullName == args.Name)
                        return assembly;
                } catch { }
            }

            log.Info("LoadFromSameFolder " + args.RequestingAssembly + "-> "+ args.Name);

            return null;
        }

        public static void Load(String file)
        {
            if (!File.Exists(file) || !file.EndsWith(".dll", true, null) ||
                file.ToLower().Contains("microsoft.") ||
                file.ToLower().Contains("system.") ||
                file.ToLower().Contains("missionplanner.grid.dll") ||
                file.ToLower().Contains("usbserialforandroid") 
                )
                return;

            // file exists in the install directory, so skip trying to load it as a plugin
            if (File.Exists(file) && File.Exists(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) +
                                                 Path.DirectorySeparatorChar + Path.GetFileName(file)))
                return;
            
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += new ResolveEventHandler(LoadFromSameFolder);

            Assembly asm = null;

            DateTime startDateTime = DateTime.Now;
            
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
            catch (Exception ex)
            {
                log.Error("Failed to load plugin " + file, ex);
            }

            log.InfoFormat("Plugin Load {0} time {1} s", file, (DateTime.Now - startDateTime).TotalSeconds);
        }

        public static void LoadAll()
        {
            string path = Settings.GetRunningDirectory() + "plugins" +
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