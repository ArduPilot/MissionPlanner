using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ArdupilotMega
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var t = Type.GetType("Mono.Runtime");
                bool MONO = (t != null);

                if (!MONO)
                    System.Diagnostics.Process.Start(Application.StartupPath + Path.DirectorySeparatorChar + "MissionPlanner.exe");
                else
                    System.Diagnostics.Process.Start("mono MissionPlanner.exe");
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); Console.ReadLine(); }
        }
    }
}
