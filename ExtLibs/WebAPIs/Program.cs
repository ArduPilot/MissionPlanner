using Dowding.Model;
using System;
using System.Collections.Generic;
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
            var dowd =new Dowding();
            dowd.Auth("", "").Wait();

            dowd.StartWS<VehicleTick>().Wait();

            while (true)
            {
                Thread.Sleep(1000);
            }
        }
    }
}