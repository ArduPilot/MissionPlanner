using System;
using System.Collections.Generic;
using System.Text;

namespace solo
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            if (args.Length > 0 && args[0] == "fw")
            {
                Solo.getFirmwareUrl().Wait();
                return;
            }

            var alive = Solo.is_solo_alive;

            if (!alive)
            {
                Console.WriteLine("Solo is not responding to pings");
                return;
            }

            var logs = Solo.GetLogNames();

            foreach (var log in logs)
            {
                Console.WriteLine(log);

                //Solo.DownloadDFLog(log, log);
            }

            var file = Solo.getFirmwareUrl();

            //Solo.flash_px4(@"C:\temp\ArduCopter-v2.px4");

            while (!file.IsCompleted)
            {
                System.Threading.Thread.Sleep(1000);
            }


        }
    }
}
