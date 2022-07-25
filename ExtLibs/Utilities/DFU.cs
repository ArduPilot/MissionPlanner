using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using DeviceProgramming.Dfu;
using DeviceProgramming.FileFormat;
using DeviceProgramming.Memory;
using LibUsbDotNet;
using LibUsbDotNet.Info;

namespace MissionPlanner.Utilities
{
    public class DFU
    {
        private static readonly Regex UsbIdRegex = new Regex
            (@"^(?<vid>[a-fA-F0-9]{1,4}):(?<pid>[a-fA-F0-9]{1,4})$", RegexOptions.Compiled);

        private static readonly Regex VersionRegex = new Regex
            (@"^(?<major>[0-9]{1,2})\.(?<minor>[0-9]{1,2})$", RegexOptions.Compiled);

        public static Action<int, string> Progress { get; set; } = (i, s) => { };

        static EventHandler<ProgressChangedEventArgs> printDownloadProgress = (obj, e) =>
        {
            Progress(e.ProgressPercentage, "Download progress");
            Console.WriteLine("Download progress: {0}%", e.ProgressPercentage);
        };
        static EventHandler<ErrorEventArgs> printDevError = (obj, e) =>
        {
            Progress(-1,
                String.Format("The DFU device reported the following error: {0}", e.GetException().Message));
            Console.Error.WriteLine("The DFU device reported the following error: {0}", e.GetException().Message);
        };

        public static void Flash(string filePath, int BinMemOffset = 0, int vid = 0x0483, int pid = 0xDF11)
        {
            // version is optional, FF means forced update
            int vmajor = 0xFF, vminor = 0xFF;

            var fileExt = Path.GetExtension(filePath);
            var isDfuFile = Dfu.IsExtensionSupported(fileExt);

            LibUsbDfu.Device device = null;
            try
            {
                Version fileVer = new Version(vmajor, vminor);
                Dfu.FileContent dfuFileData = null;
                RawMemory memory = null;

                // find the matching file parser by extension
                if (isDfuFile)
                {
                    dfuFileData = Dfu.ParseFile(filePath);
                    Console.WriteLine("DFU image parsed successfully.");

                    // DFU file specifies VID, PID and version, so override any arguments
                    vid = dfuFileData.DeviceInfo.VendorId;
                    pid = dfuFileData.DeviceInfo.ProductId;
                    fileVer = dfuFileData.DeviceInfo.ProductVersion;
                }
                else if (IntelHex.IsExtensionSupported(fileExt))
                {
                    memory = IntelHex.ParseFile(filePath);
                    Console.WriteLine("Intel HEX image parsed successfully.");
                }
                else if (SRecord.IsExtensionSupported(fileExt))
                {
                    memory = SRecord.ParseFile(filePath);
                    Console.WriteLine("SRecord image parsed successfully.");
                }
                else if (BinMemOffset > 0)
                {
                    memory = new RawMemory();
                    memory.TryAddSegment(new Segment((ulong)BinMemOffset, File.ReadAllBytes(filePath)));
                }
                else
                {
                    throw new ArgumentException("Image file format not recognized.");
                }

                // find the DFU device
                if (vid == 0 && pid == 0)
                {
                    LibUsbDfu.Device.TryOpen(UsbDevice.AllDevices.First(), out device);
                    vid = device.Info.VendorId;
                    pid = device.Info.ProductId;
                }
                else
                    device = LibUsbDfu.Device.OpenFirst(UsbDevice.AllDevices, vid, pid);

                device.DeviceError += printDevError;

                if (isDfuFile)
                {
                    // verify protocol version
                    if (dfuFileData.DeviceInfo.DfuVersion != device.DfuDescriptor.DfuVersion)
                    {
                        throw new InvalidOperationException(String.Format("DFU file version {0} doesn't match device DFU version {1}",
                            dfuFileData.DeviceInfo.DfuVersion,
                            device.DfuDescriptor.DfuVersion));
                    }
                }

                // if the device is in normal application mode, reconfigure it
                if (device.InAppMode())
                {
                    bool skipUpdate = fileVer <= device.Info.ProductVersion;

                    // skip update when it's deemed unnecessary
                    if (skipUpdate)
                    {
                        Console.WriteLine("The device is already up-to-date (version {0}), skipping update (version {1}).",
                            device.Info.ProductVersion,
                            fileVer);
                        return;
                    }

                    Console.WriteLine("Device found in application mode, reconfiguring device to DFU mode...");
                    device.Reconfigure();

                    // in case the device detached, we must find the DFU mode device
                    if (!device.IsOpen())
                    {
                        // clean up old device first
                        device.DeviceError -= printDevError;
                        device.Dispose();
                        device = null;

                        device = LibUsbDfu.Device.OpenFirst(UsbDevice.AllDevices, vid, pid);
                        device.DeviceError += printDevError;
                    }
                }
                else
                {
                    Console.WriteLine("Device found in DFU mode.");
                }

                // perform upgrade
                device.DownloadProgressChanged += printDownloadProgress;
                if (isDfuFile)
                {
                    device.DownloadFirmware(dfuFileData);
                }
                else
                {
                    device.DownloadFirmware(memory);
                }
                device.DownloadProgressChanged -= printDownloadProgress;

                Console.WriteLine("Download successful, manifesting update...");
                Progress(100, "Download successful, manifesting update...");
                device.Manifest();

                // if the device detached, clean up
                if (!device.IsOpen())
                {
                    device.DeviceError -= printDevError;
                    device.Dispose();
                    device = null;
                }

                // TODO find device again to verify new version
                Console.WriteLine("The device has been successfully upgraded.");
                Progress(100, "The device has been successfully upgraded.");
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Device Firmware Upgrade failed with exception: {0}.", e.ToString());
                //Environment.Exit(-1);
                Progress(-1, String.Format("Device Firmware Upgrade failed with exception: {0}.", e.ToString()));
            }
            finally
            {
                if (device != null)
                {
                    device.Dispose();
                }
            }
        }

        /// <summary>
        /// Returned in flash order, needs int reversal
        /// </summary>
        /// <param name="vid"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static byte[] GetSN(int vid = 0x0483, int pid = 0xDF11)
        {
            var UDID_START = 0x1FF1E800;

            LibUsbDfu.Device device = null;

            // find the DFU device
            if (vid == 0 && pid == 0)
                LibUsbDfu.Device.TryOpen(UsbDevice.AllDevices.First(), out device);
            else
                device = LibUsbDfu.Device.OpenFirst(UsbDevice.AllDevices, vid, pid);

            device.DeviceError += printDevError;
            ((UsbConfigInfo)device.GetPropertyOrFieldPrivate("ConfigInfo")).InterfaceInfoList.ForEach(a => Console.WriteLine(a.InterfaceString));

            device.Abort();
            device.ResetToIdle();
            device.Dnload((ushort)0, new byte[5]
            {
                (byte) 33,
                (byte) UDID_START,
                (byte) (UDID_START >> 8),
                (byte) (UDID_START >> 16),
                (byte) (UDID_START >> 24)
            });
            var status = device.GetStatus();
            for (status = device.GetStatus(); status.State == State.DnloadBusy; status = device.GetStatus())
                Thread.Sleep(status.PollTimeout);
            device.Abort();
            //Address = ((wBlockNum – 2) × wTransferSize) +Address_Pointer, where:
            //– wTransferSize is the length of the requested data buffer.
            var sn = device.Upload(2, (uint)1024);

            device.Close();

            Array.Resize(ref sn, 12);

            return sn;
        }
    }
}
