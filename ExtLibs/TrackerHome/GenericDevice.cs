using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace TrackerHomeGPS
{
    public abstract partial class Device
    {
        protected static Guid DeviceGuid;

        public static bool DevicePresent()
        {
            if (DeviceGuid != null) return DevicePresent(DeviceGuid);
            else return false;
        }

        protected static bool DevicePresent(Guid g)
        {
            // Used to capture how many bytes are returned by system calls
            UInt32 theBytesReturned = 0;

            // SetupAPI32.DLL Data Structures
            SP_DEVINFO_DATA theDevInfoData = new SP_DEVINFO_DATA();
            theDevInfoData.cbSize = Marshal.SizeOf(theDevInfoData);
            IntPtr theDevInfo = SetupDiGetClassDevs(ref g, IntPtr.Zero, IntPtr.Zero, (int)(DiGetClassFlags.DIGCF_PRESENT | DiGetClassFlags.DIGCF_DEVICEINTERFACE));
            SP_DEVICE_INTERFACE_DATA theInterfaceData = new SP_DEVICE_INTERFACE_DATA();
            theInterfaceData.cbSize = Marshal.SizeOf(theInterfaceData);

            // Check for the device
            if (!SetupDiEnumDeviceInterfaces(theDevInfo, IntPtr.Zero, ref g, 0, ref theInterfaceData) && GetLastError() == ERROR_NO_MORE_ITEMS)
                return false;

            // Get the device's file path
            SetupDiGetDeviceInterfaceDetail(theDevInfo, ref theInterfaceData, IntPtr.Zero, 0, ref theBytesReturned, IntPtr.Zero);

            return !(theBytesReturned <= 0);
        }
    }
}
