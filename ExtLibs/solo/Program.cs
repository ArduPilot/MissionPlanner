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

            if (args.Length > 0 && args[0] == "flash")
            {
                if (args[1] == "solo")
                    Solo.flash(args[2], "", true);
                else if (args[1] == "controller")
                    Solo.flash(args[2], "", false);
                else
                    Console.WriteLine("invalid command  solo flash solo firmware.gz");
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
