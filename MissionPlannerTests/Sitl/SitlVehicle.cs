using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MissionPlanner;
using MissionPlanner.Comms;

namespace MissionPlannerTests.Sitl
{
    /// <summary>
    /// An ArduPilot SITL vehicle for a test class, connected to a MAVLinkInterface over TCP.
    ///
    /// Set MP_SITL to the SITL binary to run these tests, they are inconclusive otherwise:
    ///   MP_SITL=C:\path\ArduCopter.exe                                     a native build
    ///   MP_SITL=wsl:/home/me/ardupilot/build/sitl/bin/arducopter          a Linux build in WSL
    /// MP_SITL_WSL_DISTRO picks the WSL distribution (the default one otherwise), MP_SITL_DEFAULTS
    /// replaces the default parameter files (as the binary sees them, comma separated). See
    /// MissionPlannerTests/Sitl/README.md.
    ///
    /// SITL exits when its SERIAL0 connection closes, so a vehicle serves one connection.
    /// </summary>
    public sealed class SitlVehicle : IDisposable
    {
        public const string EnvBinary = "MP_SITL";
        public const string EnvDistro = "MP_SITL_WSL_DISTRO";
        public const string EnvDefaults = "MP_SITL_DEFAULTS";

        public const uint SysId = 1;
        public const byte CompId = 1;

        /// <summary>
        /// The SITL's current directory on this machine: its eeprom, logs/ and MAVFTP root
        /// </summary>
        public string WorkDir { get; }

        public int TcpPort { get; }

        public MAVLinkInterface Mav { get; private set; }

        readonly Process process;
        readonly string wslDistro;
        readonly bool wsl;
        readonly CancellationTokenSource stop = new CancellationTokenSource();
        readonly System.Text.StringBuilder output = new System.Text.StringBuilder();
        Task reader;

        public static bool IsConfigured => !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(EnvBinary));

        public const string NotConfigured = "Set " + EnvBinary + " to an ArduPilot SITL binary to run the SITL tests";

        /// <summary>
        /// Start SITL and connect to it, or end the test as inconclusive when no SITL is configured
        /// </summary>
        /// <param name="parameters">set before the vehicle boots, through a defaults file</param>
        public static SitlVehicle Start(string model = "+", IDictionary<string, double> parameters = null)
        {
            if (!IsConfigured)
                Assert.Inconclusive(NotConfigured);

            var vehicle = new SitlVehicle(Environment.GetEnvironmentVariable(EnvBinary).Trim(), model,
                parameters ?? new Dictionary<string, double>());
            try
            {
                vehicle.Connect();
                return vehicle;
            }
            catch
            {
                vehicle.Dispose();
                throw;
            }
        }

        SitlVehicle(string binary, string model, IDictionary<string, double> parameters)
        {
            WorkDir = Path.Combine(Path.GetTempPath(), "mp-sitl-" + Guid.NewGuid().ToString("N").Substring(0, 8));
            Directory.CreateDirectory(WorkDir);
            File.WriteAllLines(Path.Combine(WorkDir, "test.parm"),
                parameters.Select(a => a.Key + " " + a.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)));

            // SERIAL0 is 5760 + 10 * instance, the other ports SITL opens move with it
            int instance = FreeInstance();
            TcpPort = 5760 + 10 * instance;

            wsl = binary.StartsWith("wsl:", StringComparison.OrdinalIgnoreCase);
            if (wsl)
                binary = binary.Substring(4);
            wslDistro = Environment.GetEnvironmentVariable(EnvDistro);

            string defaults = Environment.GetEnvironmentVariable(EnvDefaults);
            string arguments = "--model " + model + " --speedup 1 -I" + instance +
                               " --home -35.363261,149.165230,584,353";

            if (wsl)
            {
                string dir = ToWslPath(WorkDir);
                // the repository's default parameters for the vehicle, if this is a build tree
                string guess = "$(dirname \"$bin\")/../../../Tools/autotest/default_params/" + DefaultsFile(binary);
                string script = "#!/bin/bash\n" +
                                "bin='" + binary + "'\n" +
                                "cd '" + dir + "'\n" +
                                (string.IsNullOrEmpty(defaults)
                                    ? "d=" + guess + "; if [ -f \"$d\" ]; then defaults=\"$d,test.parm\"; else defaults=test.parm; fi\n"
                                    : "defaults='" + defaults + ",test.parm'\n") +
                                "\"$bin\" " + arguments + " --defaults \"$defaults\" </dev/null >sitl.out 2>&1 &\n" +
                                "echo $! > sitl.pid\n" +
                                "wait\n";
                File.WriteAllText(Path.Combine(WorkDir, "sitl.sh"), script.Replace("\r", ""));

                process = Process.Start(new ProcessStartInfo("wsl.exe",
                    (string.IsNullOrEmpty(wslDistro) ? "" : "-d " + wslDistro + " ") + "-e bash \"" + dir + "/sitl.sh\"")
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                });
            }
            else
            {
                if (string.IsNullOrEmpty(defaults))
                {
                    // MissionPlanner keeps the default parameters beside its SITL binaries
                    string guess = Path.Combine(Path.GetDirectoryName(binary), "default_params", DefaultsFile(binary));
                    defaults = File.Exists(guess) ? guess : null;
                }

                process = Process.Start(new ProcessStartInfo(binary,
                    arguments + " --defaults \"" + (defaults == null ? "" : defaults + ",") + "test.parm\"")
                {
                    WorkingDirectory = WorkDir,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                });
            }

            // keep its output, the pipe must not fill and block it
            process.OutputDataReceived += (s, e) => { lock (output) output.AppendLine(e.Data); };
            process.ErrorDataReceived += (s, e) => { lock (output) output.AppendLine(e.Data); };
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
        }

        static string DefaultsFile(string binary)
        {
            string name = Path.GetFileNameWithoutExtension(binary.Replace('/', Path.DirectorySeparatorChar)).ToLowerInvariant();
            if (name.Contains("rover")) return "rover.parm";
            if (name.Contains("plane")) return "plane.parm";
            if (name.Contains("sub")) return "sub.parm";
            return "copter.parm";
        }

        static string ToWslPath(string windowsPath)
        {
            string full = Path.GetFullPath(windowsPath);
            return "/mnt/" + char.ToLowerInvariant(full[0]) + full.Substring(2).Replace('\\', '/');
        }

        static int FreeInstance()
        {
            var random = new Random();
            for (int attempt = 0; attempt < 50; attempt++)
            {
                int instance = random.Next(10, 60);
                // SERIAL0 to SERIAL2, and the simulator's own ports a few above
                if (Enumerable.Range(0, 10).All(a => PortIsFree(5760 + 10 * instance + a)))
                    return instance;
            }

            throw new InvalidOperationException("no free SITL instance");
        }

        static bool PortIsFree(int port)
        {
            try
            {
                var listener = new TcpListener(IPAddress.Loopback, port);
                listener.Start();
                listener.Stop();
                return true;
            }
            catch (SocketException)
            {
                return false;
            }
        }

        void Connect()
        {
            // SITL waits for this connection before it starts, and the first one is the only one
            var deadline = DateTime.UtcNow.AddSeconds(60);
            TcpClient client = null;
            while (client == null)
            {
                if (process.HasExited)
                    throw new InvalidOperationException("SITL exited: " + ReadOutput());
                try
                {
                    client = new TcpClient();
                    client.Connect(IPAddress.Loopback, TcpPort);
                }
                catch (SocketException)
                {
                    client.Dispose();
                    client = null;
                    if (DateTime.UtcNow > deadline)
                        throw new TimeoutException("SITL did not listen on " + TcpPort + ": " + ReadOutput());
                    Thread.Sleep(250);
                }
            }

            client.NoDelay = true;
            Mav = new MAVLinkInterface
            {
                BaseStream = new TcpSerial { client = client, Host = "127.0.0.1", Port = TcpPort.ToString() },
                speechenabled = false,
            };
            Mav.Open(false, true, false);
            if (!Mav.BaseStream.IsOpen || Mav.sysidcurrent != SysId)
                throw new InvalidOperationException("no heartbeat from SITL: " + ReadOutput());

            // MainV2's serial reader: read whenever no operation has taken the link for itself
            reader = Task.Run(async () =>
            {
                while (!stop.IsCancellationRequested && Mav.BaseStream.IsOpen)
                {
                    try
                    {
                        if (!Mav.giveComport && Mav.BaseStream.BytesToRead > 0)
                            await Mav.readPacketAsync().ConfigureAwait(false);
                        else
                            await Task.Delay(2).ConfigureAwait(false);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("SITL reader: " + e.Message);
                    }
                }
            });

            // MainV2 connects with the param list, setParam only sets the params in it
            if (Mav.getParamList(SysId, CompId).Count == 0)
                throw new InvalidOperationException("no params from SITL");

            // ask for telemetry the way MainV2 does after it connects
            foreach (MAVLink.MAV_DATA_STREAM stream in new[]
            {
                MAVLink.MAV_DATA_STREAM.EXTENDED_STATUS, MAVLink.MAV_DATA_STREAM.POSITION, MAVLink.MAV_DATA_STREAM.EXTRA1,
                MAVLink.MAV_DATA_STREAM.EXTRA2, MAVLink.MAV_DATA_STREAM.RAW_SENSORS
            })
                Mav.requestDatastream(stream, 4, SysId, CompId);
        }

        /// <summary>
        /// The state MissionPlanner keeps of the vehicle
        /// </summary>
        public MAVState State => Mav.MAVlist[SysId, CompId];

        /// <summary>
        /// Wait for a message that satisfies the test, from the reader
        /// </summary>
        public MAVLink.MAVLinkMessage WaitFor(MAVLink.MAVLINK_MSG_ID id, Func<MAVLink.MAVLinkMessage, bool> test,
            TimeSpan timeout)
        {
            var found = new TaskCompletionSource<MAVLink.MAVLinkMessage>();
            EventHandler<MAVLink.MAVLinkMessage> handler = (s, m) =>
            {
                if (m.msgid == (uint)id && m.sysid == SysId && test(m))
                    found.TrySetResult(m);
            };
            Mav.OnPacketReceived += handler;
            try
            {
                if (!found.Task.Wait(timeout))
                    throw new TimeoutException("no " + id + " in " + timeout);
                return found.Task.Result;
            }
            finally
            {
                Mav.OnPacketReceived -= handler;
            }
        }

        public string ReadOutput()
        {
            try
            {
                string all;
                lock (output)
                    all = output.ToString();
                string file = Path.Combine(WorkDir, "sitl.out");
                if (File.Exists(file))
                {
                    using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (var text = new StreamReader(stream))
                        all += text.ReadToEnd();
                }
                return all.Length > 2000 ? all.Substring(all.Length - 2000) : all;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public void Dispose()
        {
            stop.Cancel();
            try { reader?.Wait(2000); } catch { }

            // closing SERIAL0 ends SITL
            try { Mav?.Close(); } catch { }
            try { Mav?.Dispose(); } catch { }

            try
            {
                if (!process.WaitForExit(5000))
                {
                    if (wsl)
                    {
                        string pid = File.ReadAllText(Path.Combine(WorkDir, "sitl.pid")).Trim();
                        using (var kill = Process.Start(new ProcessStartInfo("wsl.exe",
                                   (string.IsNullOrEmpty(wslDistro) ? "" : "-d " + wslDistro + " ") + "-e kill " + pid)
                               { UseShellExecute = false, CreateNoWindow = true }))
                            kill.WaitForExit(5000);
                    }

                    if (!process.WaitForExit(5000))
                        process.Kill();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("SITL stop: " + e.Message);
            }

            process.Dispose();

            try { Directory.Delete(WorkDir, true); } catch { }
        }
    }
}
