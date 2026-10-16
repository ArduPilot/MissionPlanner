using log4net;
using MissionPlanner.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MissionPlanner.Controls;
using DroneCAN;
using System.Text.RegularExpressions;
using System.Linq.Expressions;
using System.Threading;
using System.Runtime.InteropServices;
using System.Text;

namespace MissionPlanner.Plugin
{
    public class PluginLoader
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static PluginLoader()
        {

        }

        //List of disabled plugins (as dll file names)
        public static List<String> DisabledPluginNames = new List<String>();
        // Plugin enable/disable settings changed not loaded but enabled plugins will not shown
        public static bool bRestartRequired = false;

        public static List<Plugin> LoadingPlugins = new List<Plugin>();
        public static List<Plugin> Plugins = new List<Plugin>();

        public static Dictionary<string, string[]> filecache = new Dictionary<string, string[]>();

        public static Dictionary<string, string> ErrorInfo = new Dictionary<string, string>();



        //net 4.7.x cannot read alternate data streams directly, so we need to use win32 API
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr CreateFile(
            string lpFileName,
            uint dwDesiredAccess,
            uint dwShareMode,
            IntPtr lpSecurityAttributes,
            uint dwCreationDisposition,
            uint dwFlagsAndAttributes,
            IntPtr hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadFile(
            IntPtr hFile,
            byte[] lpBuffer,
            uint nNumberOfBytesToRead,
            out uint lpNumberOfBytesRead,
            IntPtr lpOverlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);


        private const uint GENERIC_READ = 0x80000000;
        private const uint GENERIC_WRITE = 0x40000000;
        private const uint DELETE = 0x00010000;
        private const uint FILE_SHARE_READ = 0x00000001;
        private const uint FILE_SHARE_DELETE = 0x00000004;
        private const uint OPEN_EXISTING = 3;
        private static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        private static int ParseZoneId(string content)
        {
            foreach (string line in content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (line.StartsWith("ZoneId=", StringComparison.OrdinalIgnoreCase))
                {
                    if (int.TryParse(line.Substring(7), out int zoneId))
                    {
                        return zoneId;
                    }
                }
            }
            return -1;
        }

        public static int getZoneIdentifier(string filePath)
        {
            IntPtr handle = IntPtr.Zero;
            string zoneIdPath = filePath + ":Zone.Identifier";
            try
            {
                handle = CreateFile(zoneIdPath, GENERIC_READ, FILE_SHARE_READ,
                    IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);

                if (handle == INVALID_HANDLE_VALUE)
                {
                    int error = Marshal.GetLastWin32Error();
                    if (error == 2) // ERROR_FILE_NOT_FOUND
                    {
                        log.Info("No Zone.Identifier found. File may be local or stream was removed.");
                        return 0;
                    }
                    else
                    {
                        Console.WriteLine($"Error opening file. Win32 error code: {error}");
                        return -1;
                    }
                }

                // Read the Zone.Identifier content using ReadFile
                byte[] buffer = new byte[1024];
                if (ReadFile(handle, buffer, (uint)buffer.Length, out uint bytesRead, IntPtr.Zero))
                {
                    string content = Encoding.UTF8.GetString(buffer, 0, (int)bytesRead);
                    log.Info("Zone.Identifier content:");
                    log.Info(content);

                    int zoneId = ParseZoneId(content);
                    return zoneId;

                }
                else
                {
                    log.Error($"Error reading file. Win32 error code: {Marshal.GetLastWin32Error()}");
                    return -1;
                }
            }
            finally
            {
                if (handle != IntPtr.Zero && handle != INVALID_HANDLE_VALUE)
                {
                    CloseHandle(handle);
                }
            }
        }



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

            InitPlugin(asm, file);

            log.InfoFormat("Plugin Load {0} time {1} s", file, (DateTime.Now - startDateTime).TotalSeconds);
        }

        public static void InitPlugin(Assembly asm, string pluginfilename)
        {
            if (asm == null)
                return;

            try
            {
                Type[] types = asm.GetTypes();
                Type type = typeof(MissionPlanner.Plugin.Plugin);
                foreach (var t in types)
                {
                    if (type == t)
                        continue;

                    if (type.IsAssignableFrom((Type)t))
                    {
                        Type pluginInfo = t;
                        if (pluginInfo != null)
                        {
                            try
                            {
                                //pluginInfo.GetConstructor(Type.EmptyTypes);
                                Object o = Expression.Lambda<Func<object>>(Expression.New(pluginInfo)).Compile()();
                                //Object o = Activator.CreateInstance(pluginInfo, BindingFlags.Default, null, null, CultureInfo.CurrentUICulture);
                                Plugin plugin = (Plugin)o;

                                plugin.Assembly = asm;

                                plugin.Host = new PluginHost();
                                plugin.FileName = Path.GetFileName(pluginfilename);

                                if (plugin.Init())
                                {
                                    log.InfoFormat("Plugin Init {0} {1} by {2}", plugin.Name, plugin.Version, plugin.Author);
                                    lock (LoadingPlugins)
                                    {
                                        LoadingPlugins.Add(plugin);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                log.Error("Failed to load plugin " + asm.FullName, ex);
                            }
                        }
                    }
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                log.Error("Failed to load plugin " + asm.FullName, ex);
                log.Error("Failed to load plugin " + asm.FullName, ex.LoaderExceptions.FirstOrDefault());
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

            log.Info("Plugin path: "+path);

            if (!Directory.Exists(path))
                return;

            // cs plugins are background compiled, and loaded in the ui thread
            Task.Run(() =>
            {
                String[] csFiles = Directory.GetFiles(path, "*.cs");

                foreach (var csFile in csFiles)
                {
                    log.Info("Plugin: " + csFile);
                    //Check if it is disabled (moved out from the previous IF, to make it loggable)
                    if (DisabledPluginNames.Contains(Path.GetFileName(csFile).ToLower()))
                    {
                        log.InfoFormat("Plugin {0} is disabled in config.xml", Path.GetFileName(csFile));
                        continue;
                    }

                    //loadassembly: MissionPlanner.WebAPIs
                    var content = File.ReadAllText(csFile);

                    var matches = Regex.Matches(content, @"^\/\/loadassembly: (.*)$", RegexOptions.Multiline);
                    foreach (Match m in matches)
                    {
                        try
                        {
                            log.Info("Try load " + m.Groups[1].Value.Trim());
                            Assembly.Load(m.Groups[1].Value.Trim());
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex);
                        }
                    }

                    try
                    {
                        // csharp 8
                        var ans = CodeGenRoslyn.BuildCode(csFile);

                        if (CodeGenRoslyn.lasterror != "")
                            lock(ErrorInfo)
                                ErrorInfo[csFile] = CodeGenRoslyn.lasterror;

                        InitPlugin(ans, Path.GetFileName(csFile));

                        log.Info("CodeGenRoslyn: " + csFile);
                        if (Program.MONO)
                            Thread.Sleep(2000);
                        continue;
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }


                    try
                    {
                        //csharp 5 max

                        // create a compiler
                        var compiler = CodeGen.CreateCompiler();
                        // get all the compiler parameters
                        var parms = CodeGen.CreateCompilerParameters();
                        // compile the code into an assembly
                        var results = CodeGen.CompileCodeFile(compiler, parms, csFile);

                        if (CodeGenRoslyn.lasterror != "")
                            lock (ErrorInfo)
                                ErrorInfo[csFile] = CodeGen.lasterror;

                        InitPlugin(results?.CompiledAssembly, Path.GetFileName(csFile));

                        if (results?.CompiledAssembly != null)
                        {
                            log.Info("CodeGen: " + csFile);
                            if (Program.MONO)
                                Thread.Sleep(2000);
                            continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }
                }

                MainV2.instance.BeginInvokeIfRequired(() =>
                {
                    PluginInit();
                });
            });

            String[] files = Directory.GetFiles(path, "*.dll");
            foreach (var s in files)
            {

                if (!Program.MONO) //Only on Windows
                {
                    int zoneID = getZoneIdentifier(s);
                    if (zoneID == 3) // Internet zone
                    {
                        CustomMessageBox.Show(String.Format("Plugin {0} is blocked by Windows (Zone.Identifier = Internet). Please unblock the file in its properties.", s));
                        continue;
                    }
                }

                Load(Path.Combine(Environment.CurrentDirectory, s));
            }

            InitPlugin(Assembly.GetAssembly(typeof(PluginLoader)), "self");

            PluginInit();
        }

        private static void PluginInit()
        {
            List<Plugin> LoadingSnapshot;

            lock (LoadingPlugins)
            {
                LoadingSnapshot = LoadingPlugins.ToList();
                LoadingPlugins.Clear();
            }

            foreach (var p in LoadingSnapshot)
            {
                try
                {
                    if (p.Loaded())
                    {
                        lock (Plugins)
                        {
                            Plugins.Add(p);
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }
        }
    }
}