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
            DoIt();

            while (true)
            {
                Thread.Sleep(1000);
                Console.Write(".");
            }
        }

        private static async void DoIt()
        {
            var dowd = new Dowding();
            dowd.Auth(File.ReadAllLines(@"C:\Users\mich1\dowding.txt")[0],
                File.ReadAllLines(@"C:\Users\mich1\dowding.txt")[1]).Wait();

            //dowd.SetToken("");
/*
            var ws = await dowd.StartWS<VehicleTick>();

            var zones = await dowd.GetZone();

            var contacts = await dowd.GetContact(minTs: DateTime.UtcNow.AddSeconds(-120).toUnixTime().ToString(), thin: true, limit: 100);

            var list = contacts.Select(a =>
                    (a.VehicleId, a.VehicleLastLat, a.VehicleLastLon, a.VehicleLastHae, a.VehicleLastTs))
                .ToList();

            //var agents = await dowd.GetAgents();

            //var vehc = await dowd.GetVehicle(contacts.First().Id);
*/
            dowd.Start();

           // Debugger.Break();
        }

        public static int toUnixTime(this DateTime dateTime)
        {
            return (int)dateTime.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public static DateTime fromUnixTime(this int time)
        {
            return new DateTime(1970, 1, 1).AddSeconds(time);
        }
    }
}