using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MissionPlanner.Utilities
{
    class CleanDrivers
    {

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
                        if (line.ToUpper().Contains(@"USB\VID_26AC") || line.ToUpper().Contains(@"USB\VID_2341"))
                        {
                            try
                            {
                                Console.WriteLine(file);

                                sr.Close();

                                File.Delete(file);
                            }
                            catch { }
                            //System.Diagnostics.Process.Start("PnPutil.exe");//, "-f -d " + Path.GetFileName(file));
                        }
                    }

                    sr.Close();
                }
            }
        }
    }
}
