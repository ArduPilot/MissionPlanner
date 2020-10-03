using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Hardware.Usb;
using Hoho.Android.UsbSerial.Driver;
using Hoho.Android.UsbSerial.Util;

namespace Hoho.Android.UsbSerial
{
    public class AndroidSerialBase
    {
        public static List<(int Vid, int Pid)> cdcacmTuples = new List<(int Vid, int Pid)>()
        {
            (0x0483, 0x5740), // ST
            (0x1209, 0x5740), // Ardu comp
            (0x1209, 0x5741), // Ardu non-comp
            (0x26AC, 0x11), // 3dr
        };

        public static Task<IList<IUsbSerialDriver>> GetPorts(UsbManager usbManager)
        {
            var table = UsbSerialProber.DefaultProbeTable;

            foreach (var cdcacmTuple in cdcacmTuples)
            {
                if (table.FindDriver(cdcacmTuple.Item1, cdcacmTuple.Item2) == null)
                    table.AddProduct(cdcacmTuple.Item1, cdcacmTuple.Item2,
                        Java.Lang.Class.FromType(typeof(CdcAcmSerialDriver)));
            }

            var prober = new UsbSerialProber(table);

            var drivers = prober.FindAllDriversAsync(usbManager);

            return drivers;
        }
    }
}