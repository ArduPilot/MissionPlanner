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
        public static Task<IList<IUsbSerialDriver>> GetPorts(UsbManager usbManager)
        {
            var cdcacmTuples = new[]
            {
                (0x2dae, 0x1001),
                (0x2dae, 0x1002),
                (0x2dae, 0x1003),
                (0x2dae, 0x1004),
                (0x2dae, 0x1005),
                (0x2dae, 0x1006),
                (0x2dae, 0x1007),
                (0x2dae, 0x1008),
                (0x2dae, 0x1009),
                (0x2dae, 0x1010),
                (0x2dae, 0x1011),
                (0x2dae, 0x1015),
                (0x2dae, 0x1016),

                (0x2dae, 0x0001),
                (0x2dae, 0x0002),
                (0x2dae, 0x0003),
                (0x2dae, 0x0004),
                (0x2dae, 0x0005),
                (0x2dae, 0x0006),
                (0x2dae, 0x0007),
                (0x2dae, 0x0008),
                (0x2dae, 0x0009),
                (0x2dae, 0x0010),
                (0x2dae, 0x0011),
                (0x2dae, 0x0015),
                (0x2dae, 0x0016),

                (0x0483, 0x5740),
                (0x1209, 0x5740),
                (0x26AC, 0x11),
                (0x20A0, 0x415E),
                (0x20A0, 0x415C),
                (0x20A0, 0x41D0),
                (0x20A0, 0x415D),
            };

            var table = new ProbeTable();

            foreach (var cdcacmTuple in cdcacmTuples)
            {
                table.AddProduct(cdcacmTuple.Item1, cdcacmTuple.Item2,
                    Java.Lang.Class.FromType(typeof(CdcAcmSerialDriver)));
            }

            var prober = new UsbSerialProber(table);

            var drivers = prober.FindAllDriversAsync(usbManager);

            return drivers;
        }
    }
}