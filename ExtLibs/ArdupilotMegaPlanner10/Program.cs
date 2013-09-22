using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArdupilotMega
{
    class Program
    {
        static void Main(string[] args)
        {
            var t = Type.GetType("Mono.Runtime");
            bool MONO = (t != null);

            if (!MONO)
                System.Diagnostics.Process.Start("MissionPlanner.exe");
            else
                System.Diagnostics.Process.Start("mono MissionPlanner.exe");    
        }
    }
}
