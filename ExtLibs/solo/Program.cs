using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace solo
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var alive = Solo.is_solo_alive;

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
