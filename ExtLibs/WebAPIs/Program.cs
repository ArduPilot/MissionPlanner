using Dowding.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MissionPlanner.WebAPIs
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {

        }

        internal static int toUnixTime(this DateTime dateTime)
        {
            return (int)dateTime.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }

        internal static DateTime fromUnixTime(this int time)
        {
            return new DateTime(1970, 1, 1).AddSeconds(time);
        }
    }
}