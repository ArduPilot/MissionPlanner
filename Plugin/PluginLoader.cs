using log4net;
using MissionPlanner.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MissionPlanner.Plugin
{
    public class PluginLoader
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        //List of disabled plugins (as dll file names)
        public static List<String> DisabledPluginNames = new List<String>();
        // Plugin enable/disable settings changed not loaded but enabled plugins will not shown
        public static bool bRestartRequired = false;

        public static List<Plugin> Plugins = new List<Plugin>();

        public static Dictionary<string, string[]> filecache = new Dictionary<string, string[]>();

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

            foreach (var file in filecache[folderPath].Where(a => a.ToLower().Contains(new AssemblyName(args.Name).Name.ToLower() + ".dll")))
            {
                try
                {
                    Assembly assembly = Assembly.LoadFrom(file);
                    if (assembly.FullName == args.Name)
                        return assembly;
                }
                catch { }
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
                }
                catch { }
            }

            log.Info("LoadFromSameFolder " + args.RequestingAssembly + "-> " + args.Name);

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

            //Check if it is disabled (moved out from the previous IF, to make it loggable)
            if (DisabledPluginNames.Contains(Path.GetFileName(file).ToLower()))
            {
                log.InfoFormat("Plugin {0} is disabled in config.xml", Path.GetFileName(file));
                return;
            }

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
                log.Info("Plugin Load " + file);
            }
            catch (Exception)
            {
                // unable to load
                return;
            }

            InitPlugin(asm);

            log.InfoFormat("Plugin Load {0} time {1} s", file, (DateTime.Now - startDateTime).TotalSeconds);
        }

        public static void InitPlugin(Assembly asm)
        {
            if (asm == null)
                return;

            Type pluginInfo = null;
            try
            {
                Type[] types = asm.GetTypes();
                Type type = typeof(MissionPlanner.Plugin.Plugin);
                foreach (var t in types)
                    if (type.IsAssignableFrom((Type) t))
                    {
                        pluginInfo = t;
                        break;
                    }

                if (pluginInfo != null)
                {
                    Object o = Activator.CreateInstance(pluginInfo, BindingFlags.Default, null, null,
                        CultureInfo.CurrentUICulture);
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
                log.Error("Failed to load plugin " + asm.FullName, ex);
            }
        }

        public static void LoadAll()
        {
            string path = Settings.GetRunningDirectory() + "plugins" +
                          Path.DirectorySeparatorChar;

            if (!Directory.Exists(path))
                return;

            String[] csFiles = Directory.GetFiles(path, "*.cs");

            foreach (var csFile in csFiles)
            {
                // create a compiler
                var compiler = CodeGen.CreateCompiler();
                // get all the compiler parameters
                var parms = CodeGen.CreateCompilerParameters();
                // compile the code into an assembly
                var results = CodeGen.CompileCode(compiler, parms, File.ReadAllText(csFile));

                InitPlugin(results?.CompiledAssembly);
            }

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