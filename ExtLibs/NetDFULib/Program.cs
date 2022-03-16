using System;
using System.IO;

namespace NetDFULib
{
    class Program
    {
        const uint STDFUFILES_NOERROR = 0x12340000;
        const UInt16 defaultSTMVid = 0x0483;
        const UInt16 defafultSTMPid = 0xDF11;
        const UInt16 defaultFwVersion = 0x2200;

        static void Main(string[] args)
        {
            var fw = new NetDFULib.FirmwareUpdate();

            fw.OnFirmwareUpdateProgress += (s, a) => { Console.WriteLine(a.Message + " " + a.Percentage); };	

            var hexFile = args[0];
            var dfufile = Path.GetTempFileName();
            UInt16 vid = defaultSTMVid;
            UInt16 pid = defafultSTMPid;
            UInt16 fwVersion = defaultFwVersion;

            if (new HEX2DFU().ConvertHexToDFU(hexFile, dfufile, vid, pid, fwVersion))
            {
                fw.UpdateFirmware(dfufile, false, true);
            } else
            {
                Console.WriteLine("Error converting hex to dfu file");
            }

            return;
        }
    }
}