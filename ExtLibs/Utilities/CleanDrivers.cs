using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

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
            string[] files = Directory.GetFiles(@"c:\windows\inf\", "*.inf",SearchOption.AllDirectories);

            foreach (string file in files)
            {
                using (StreamReader sr = new StreamReader(File.OpenRead(file)))
                {

                    // USB\VID_26AC    3dr
                    // USB\VID_2341   arduino

                    while (sr.BaseStream != null && !sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        if (line.ToUpper().Contains(@"USB\VID_26AC"))// || line.ToUpper().Contains(@"USB\VID_2341"))
                        {
                            try
                            {
                                Console.WriteLine(file);

                              //  File.Delete(file);
                            }
                            catch { }

                            if (GetOSArchitecture() == 64)
                            {
                                System.Diagnostics.Process.Start(Application.StartupPath + Path.DirectorySeparatorChar + "driver/DPInstx64.exe", @"/u """ + file + @""" /d");
                            }
                            else
                            {
                                System.Diagnostics.Process.Start(Application.StartupPath + Path.DirectorySeparatorChar + "driver/DPInstx86.exe", @"/u """ + file + @""" /d");
                            }
                        }
                    }

                    sr.Close();
                }
            }
        }
    }
}
