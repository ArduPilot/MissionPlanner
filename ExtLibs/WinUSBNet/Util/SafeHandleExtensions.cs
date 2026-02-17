using System.Runtime.InteropServices;

using Windows.Win32.Devices.Usb;

namespace Nefarius.Drivers.WinUSB.Util;

internal static class SafeHandleExtensions
{
    public static WINUSB_INTERFACE_HANDLE AsInterfaceHandle(this SafeHandle handle)
    {
        return new WINUSB_INTERFACE_HANDLE(handle.DangerousGetHandle());
    }
}