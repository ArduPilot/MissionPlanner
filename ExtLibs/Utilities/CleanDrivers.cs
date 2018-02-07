using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Permissions;
using System.Text.RegularExpressions;

namespace MissionPlanner.Utilities
{
    public class CleanDrivers
    {
        private static int GetOSArchitecture()
        {
            string pa =
                Environment.GetEnvironmentVariable("ProgramW6432");
            return (String.IsNullOrEmpty(pa) ? 32 : 64);
        }

        public static void Clean()
        {
            Process pr = new Process();
            pr.StartInfo = new ProcessStartInfo("pnputil", "/enum-drivers")
            {
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            pr.Start();
            var so = pr.StandardOutput.ReadToEndAsync();
            var se = pr.StandardError.ReadToEndAsync();

            /*
Published Name:     oem62.inf
Original Name:      lpro564s.inf
Provider Name:      Logitech
Class Name:         Sound, video and game controllers
Class GUID:         {4d36e96c-e325-11ce-bfc1-08002be10318}
Driver Version:     10/22/2012 13.80.853.0
Signer Name:        Microsoft Windows Hardware Compatibility Publisher
             */

            var pun = Regex.Matches(so.Result, @"Published Name:\s+(.+)");
            var orn = Regex.Matches(so.Result, @"Original Name:\s+(.+)");
            var prn = Regex.Matches(so.Result, @"Provider Name:\s+(.+)");
            var cln = Regex.Matches(so.Result, @"Class Name:\s+(.+)");
            var clg = Regex.Matches(so.Result, @"Class GUID:\s+(.+)");
            var drv = Regex.Matches(so.Result, @"Driver Version:\s+(.+)");
            var sin = Regex.Matches(so.Result, @"Signer Name:\s+(.+)");

            for (int i = 0; i < pun.Count; i++)
            {
                Console.WriteLine("{0} {1} {2} {3} {4}", pun[i].Groups[1].Value.Trim(),
                    orn[i].Groups[1].Value.Trim(),
                    prn[i].Groups[1].Value.Trim(),
                    cln[i].Groups[1].Value.Trim(), 
                    sin[i].Groups[1].Value.Trim());

                ProcessStartInfo si =
                    new ProcessStartInfo("pnputil", "-f -d " + pun[i].Groups[1].Value.Trim()) {Verb = "runas"};

                if (sin[i].Groups[1].Value.Trim() == "Michael Oborne")
                {
                    var pr2 = Process.Start(si);
                    continue;
                }

                if(prn[i].Groups[1].Value.Trim() == "3D Robotics" ||
                   prn[i].Groups[1].Value.Trim() == "Laser Navigation" ||
                   prn[i].Groups[1].Value.Trim() == "Hex Technology Limited")
                {
                    Process.Start(si);
                    continue;
                }
        }
        }
    }
}
