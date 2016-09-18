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
            Solo.getFirmwareUrl();

            Solo.flash_px4(@"C:\temp\ArduCopter-v2.px4");

            //while (true)
            {
                System.Threading.Thread.Sleep(1000);
            }
        }
    }
}
