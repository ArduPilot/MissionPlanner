using System;
using System.Runtime.InteropServices;

namespace MissionPlanner.Joystick
{
    public static class Native
    {
        [DllImport("libc", EntryPoint = "ioctl", SetLastError = true)]
        public extern static int ioctl(IntPtr filedes, int command, [In, Out] ref byte value);

        [DllImport("libc", EntryPoint = "ioctl", SetLastError = true)]
        public extern static int ioctl(IntPtr filedes, int command, [In, Out] ref int value);

        [DllImport("libc", EntryPoint = "ioctl", SetLastError = true)]
        public extern static int ioctl(IntPtr filedes, int command, [In, Out] ref string value);


        /// <summary>
        /// https://github.com/jasper22/UbuntuUsbMonitor/blob/master/linux/UbuntuUsbMonitor.Network/Native/PInvoke.cs
        /// </summary>

    }
}