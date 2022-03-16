using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DriverCleanup
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = Environment.GetEnvironmentVariable("PATH");
            path = @"c:\windows\system32\;" + path;
            Environment.SetEnvironmentVariable("PATH", path);

            Process pr = new Process();
            pr.StartInfo = new ProcessStartInfo(@"pnputil.exe", "-e")
            {
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            pr.StartInfo.Environment["PATH"] = path;

            try
            {
                pr.Start();
                var so = pr.StandardOutput.ReadToEnd();
                var se = pr.StandardError.ReadToEnd();

                var pun = Regex.Matches(so, @"Published Name\s*:\s+(.+)", RegexOptions.IgnoreCase);
                var orn = Regex.Matches(so, @"Original Name:\s+(.+)");
                var prn = Regex.Matches(so, @"(Provider Name|Driver package provider)\s*:\s+(.+)", RegexOptions.IgnoreCase);
                var cln = Regex.Matches(so, @"(Class Name|Class)\s*:\s+(.+)");
                var clg = Regex.Matches(so, @"Class GUID:\s+(.+)");
                var drv = Regex.Matches(so, @"Driver Version:\s+(.+)");
                var sin = Regex.Matches(so, @"Signer Name\s*:\s+(.+)");

                for (int i = 0; i < pun.Count; i++)
                {
                    Console.WriteLine("{0} {1} {2} {3}", pun.Count > i ? pun[i].Groups[1].Value.Trim() : "",
                        //orn[i].Groups[1].Value.Trim(),
                        prn.Count > i ? prn[i].Groups[2].Value.Trim() : "",
                        cln.Count > i ? cln[i].Groups[2].Value.Trim() : "",
                        sin.Count > i ? sin[i].Groups[1].Value.Trim() : "");

                    ProcessStartInfo si =
                        new ProcessStartInfo("pnputil", "-f -d " + pun[i].Groups[1].Value.Trim()) {Verb = "runas"};
                    try
                    {
                        if (sin.Count > i ? sin[i].Groups[1].Value.Trim() == "Michael Oborne" : false)
                        {
                            var pr2 = Process.Start(si);
                            continue;
                        }
                        
                        if (prn[i].Groups[2].Value.Trim() == "ArduPilot Project" ||
                            prn[i].Groups[2].Value.Trim() == "ChibiOS" || 
                            prn[i].Groups[2].Value.Trim() == "3D Robotics" ||
                            prn[i].Groups[2].Value.Trim() == "Laser Navigation" ||
                            prn[i].Groups[2].Value.Trim() == "Hex Technology Limited" ||
                            prn[i].Groups[2].Value.Trim() == "Holybro" ||
                            prn[i].Groups[2].Value.Trim() == "Hex Technology Limited")
                        {
                            Process.Start(si);
                            continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}