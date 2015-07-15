using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace TrackerHomeGPS
{
    public abstract partial class Device
    {
        #region SystemEnums
        public enum DeviceSetupClassGuids
        {
            Battery = 0,
            Biometric,
            Bluetooth,
            CDROM,
            DiskDrive,
            Display,
            FloppyDiskController,
            FloppyDisk,
            HardDiskController,
            HIDClass,
            IEEEDot4,
            IEEEDot4Print,
            IEEE61883,
            IEEEAVC,
            IEEESBP2,
            IEEE1394,
            Image,
            Infrared,
            Keyboard,
            MediumChanger,
            MTD,
            Modem,
            Monitor,
            Mouse,
            MultiFunction,
            Media,
            MultiportSerial,
            Net,
            NetClient,
            NetService,
            NetTrans,
            SecurityAccelerator,
            PCMCIA,
            Ports,
            Printer,
            PNPPrinters,
            Processor,
            SCSIAdaptor,
            Sensor,
            SmartCardReader,
            Volume,
            System,
            TapeDrive,
            USBDevice,
            WCEUSBS,
            WindowsPortableDevice,
            SideShow
        }

        public enum DeviceInterfaceClassGuids
        {
            USBDevice,
            USBController,
            USBHub,
            Bus1394,
            IEEE61883,
            AppLaunchButton,
            Battery,
            Lid,
            Memory,
            MessageIndicator,
            Processor,
            SysButton,
            ThermalZone,
            Bluetooth,
            DispBrightness,
            DispAdapter,
            I2C,
            Image,
            Monitor,
            OPM,
            VideoOutArrival,
            DispDeviceArrival,
            HID,
            Keyboard,
            Mouse,
            Modem,
        }


        [Flags]
        public enum DiGetClassFlags : uint
        {
            DIGCF_DEFAULT = 0x00000001,
            DIGCF_PRESENT = 0x00000002,
            DIGCF_ALLCLASSES = 0x00000004,
            DIGCF_PROFILE = 0x00000008,
            DIGCF_DEVICEINTERFACE = 0x00000010,
        }

        [Flags]
        public enum GenericAccessRights : uint
        {
            GenericRead = 0x80000000,
            GenericWrite = 0x40000000,
            GenericExecute = 0x20000000,
            GenericAll = 0x10000000
        }

        [Flags]
        public enum FileHandleShareMode : uint
        {
            FileShareNone = 0x00000000,
            FileShareDelete = 0x00000004,
            FileShareRead = 0x00000001,
            FileShareWrite = 0x00000002
        }

        public enum FileCreationDisposition : uint
        {
            CreateAlways = 2,
            CreateNew = 1,
            OpenAlways = 4,
            OpenExisting = 3,
            TruncateExisting = 5
        }

        [Flags]
        public enum FileFlagsAndAttributes : uint
        {
            FileAttributeArchive = 0x00000020,
            FileAttributeEncrypted = 0x00004000,
            FileAttributeHidden = 0x00000002,
            FileAttributeNormal = 0x00000080,
            FileAttributeOffline = 0x00001000,
            FileAttributeReadOnly = 0x00000001,
            FileAttributeSystem = 0x00000004,
            FileAttributeTemporary = 0x00000100,
            FileFlagBackupSemantics = 0x02000000,
            FileFlagDeleteOnClose = 0x04000000,
            FileFlagNoBuffering = 0x20000000,
            FileFlagOpenNoRecall = 0x00100000,
            FileFlagOpenReparsePoint = 0x00200000,
            FileFlagOverlapped = 0x40000000,
            FileFlagPosixSemantics = 0x01000000,
            FileFlagRandomAccess = 0x10000000,
            FileFlagSessionAware = 0x00800000,
            FileFlagSequentialScan = 0x08000000,
            FileFlagWriteThrough = 0x80000000,
        }
        #endregion

        #region SetupApi.Dll_Functions
        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SetupDiGetClassDevs(ref Guid ClassGuid, IntPtr Enumerator, IntPtr hwndParent, UInt32 Flags);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SetupDiGetClassDevs(IntPtr ClassGuid, String Enumerator, IntPtr hwndParent, UInt32 Flags);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern bool SetupDiEnumDeviceInfo(IntPtr DeviceInfoSet, UInt32 MemberIndex, ref SP_DEVINFO_DATA DeviceInfoData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern bool SetupDiEnumDeviceInterfaces(IntPtr DeviceInfoSet, ref SP_DEVINFO_DATA DeviceInfoData, ref Guid InterfaceClassGuid, UInt32 MemberIndex, ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern bool SetupDiEnumDeviceInterfaces(IntPtr DeviceInfoSet, IntPtr DeviceInfoData, ref Guid InterfaceClassGuid, UInt32 MemberIndex, ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr DeviceInfoSet, ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData, ref SP_DEVICE_INTERFACE_DETAIL_DATA DeviceInterfaceDetailData, UInt32 DeviceInterfaceDetailDataSize, ref UInt32 RequiredSize, ref SP_DEVINFO_DATA DeviceInfoData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr DeviceInfoSet, ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData, IntPtr DeviceInterfaceDetailData, UInt32 DeviceInterfaceDetailDataSize, IntPtr RequiredSize, ref SP_DEVINFO_DATA DeviceInfoData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr DeviceInfoSet, ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData, IntPtr DeviceInterfaceDetailData, UInt32 DeviceInterfaceDetailDataSize, ref UInt32 RequiredSize, IntPtr DeviceInfoData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern bool SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);
        #endregion

        #region Kernel32.Dll_Functions
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        public static extern SafeFileHandle CreateFile(String lpFileName, UInt32 dwDesiredAccess, UInt32 dwShareMode, IntPtr lpSecurityAttributes, UInt32 dwCreationDisposition, UInt32 dwFlagsAndAttributes, IntPtr hTemplateFile);

        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool DeviceIoControl(SafeFileHandle hDevice, UInt32 dwIoControlCode, ref byte[] lpInBuffer, UInt32 nInBufferSize, IntPtr lpOutBuffer, UInt32 nOutBufferSize, ref UInt32 lpBytesReturned, IntPtr Overlap);

        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool DeviceIoControl(SafeFileHandle hDevice, UInt32 dwIoControlCode, IntPtr lpInBuffer, UInt32 nInBufferSize, IntPtr lpOutBuffer, UInt32 nOutBufferSize, ref UInt32 lpBytesReturned, IntPtr Overlap);

        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool WriteFile(SafeFileHandle hFile, ref byte[] lpBuffer, UInt32 nNumberOfBytesToWrite, ref UInt32 lpNumberOfBytesWritten, IntPtr Overlap);

        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool WriteFile(SafeFileHandle hFile, IntPtr lpBuffer, UInt32 nNumberOfBytesToWrite, ref UInt32 lpNumberOfBytesWritten, IntPtr Overlap);

        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool ReadFile(SafeFileHandle hFile, ref byte[] lpBuffer, UInt32 nNumberOfBytesToRead, ref UInt32 lpNumberOfBytesRead, IntPtr Overlap);

        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool ReadFile(SafeFileHandle hFile, IntPtr lpBuffer, UInt32 nNumberOfBytesToRead, ref UInt32 lpNumberOfBytesRead, IntPtr Overlap);

        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        public static extern long GetLastError();
        #endregion

        #region SystemStructures
        [StructLayout(LayoutKind.Sequential)]
        public struct SP_DEVICE_INTERFACE_DATA
        {
            public Int32 cbSize;
            public Guid InterfaceClassGuid;
            public Int32 Flags;
            public IntPtr Reserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SP_DEVINFO_DATA
        {
            public Int32 cbSize;
            public Guid ClassGuid;
            public Int32 DevInst;
            public IntPtr Reserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SECURITY_ATTRIBUTES
        {
            public uint nLength;
            public IntPtr lpSecurityDescriptor;
            public bool bInheritHandle;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SP_DEVICE_INTERFACE_DETAIL_DATA
        {
            public Int32 cbSize;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public String DevicePath;
        }
        #endregion

        #region WINDOWS_ERRORS
        public const long ERROR_NO_MORE_ITEMS = 259L;
        #endregion

        #region DEVICE_TYPE
        public const UInt32 FILE_DEVICE_BEEP = 0x00000001;
        public const UInt32 FILE_DEVICE_CD_ROM = 0x00000002;
        public const UInt32 FILE_DEVICE_CD_ROM_FILE_SYSTEM = 0x00000003;
        public const UInt32 FILE_DEVICE_CONTROLLER = 0x00000004;
        public const UInt32 FILE_DEVICE_DATALINK = 0x00000005;
        public const UInt32 FILE_DEVICE_DFS = 0x00000006;
        public const UInt32 FILE_DEVICE_DISK = 0x00000007;
        public const UInt32 FILE_DEVICE_DISK_FILE_SYSTEM = 0x00000008;
        public const UInt32 FILE_DEVICE_FILE_SYSTEM = 0x00000009;
        public const UInt32 FILE_DEVICE_INPORT_PORT = 0x0000000a;
        public const UInt32 FILE_DEVICE_KEYBOARD = 0x0000000b;
        public const UInt32 FILE_DEVICE_MAILSLOT = 0x0000000c;
        public const UInt32 FILE_DEVICE_MIDI_IN = 0x0000000d;
        public const UInt32 FILE_DEVICE_MIDI_OUT = 0x0000000e;
        public const UInt32 FILE_DEVICE_MOUSE = 0x0000000f;
        public const UInt32 FILE_DEVICE_MULTI_UNC_PROVIDER = 0x00000010;
        public const UInt32 FILE_DEVICE_NAMED_PIPE = 0x00000011;
        public const UInt32 FILE_DEVICE_NETWORK = 0x00000012;
        public const UInt32 FILE_DEVICE_NETWORK_BROWSER = 0x00000013;
        public const UInt32 FILE_DEVICE_NETWORK_FILE_SYSTEM = 0x00000014;
        public const UInt32 FILE_DEVICE_NULL = 0x00000015;
        public const UInt32 FILE_DEVICE_PARALLEL_PORT = 0x00000016;
        public const UInt32 FILE_DEVICE_PHYSICAL_NETCARD = 0x00000017;
        public const UInt32 FILE_DEVICE_PRINTER = 0x00000018;
        public const UInt32 FILE_DEVICE_SCANNER = 0x00000019;
        public const UInt32 FILE_DEVICE_SERIAL_MOUSE_PORT = 0x0000001a;
        public const UInt32 FILE_DEVICE_SERIAL_PORT = 0x0000001b;
        public const UInt32 FILE_DEVICE_SCREEN = 0x0000001c;
        public const UInt32 FILE_DEVICE_SOUND = 0x0000001d;
        public const UInt32 FILE_DEVICE_STREAMS = 0x0000001e;
        public const UInt32 FILE_DEVICE_TAPE = 0x0000001f;
        public const UInt32 FILE_DEVICE_TAPE_FILE_SYSTEM = 0x00000020;
        public const UInt32 FILE_DEVICE_TRANSPORT = 0x00000021;
        public const UInt32 FILE_DEVICE_UNKNOWN = 0x00000022;
        public const UInt32 FILE_DEVICE_VIDEO = 0x00000023;
        public const UInt32 FILE_DEVICE_VIRTUAL_DISK = 0x00000024;
        public const UInt32 FILE_DEVICE_WAVE_IN = 0x00000025;
        public const UInt32 FILE_DEVICE_WAVE_OUT = 0x00000026;
        public const UInt32 FILE_DEVICE_8042_PORT = 0x00000027;
        public const UInt32 FILE_DEVICE_NETWORK_REDIRECTOR = 0x00000028;
        public const UInt32 FILE_DEVICE_BATTERY = 0x00000029;
        public const UInt32 FILE_DEVICE_BUS_EXTENDER = 0x0000002a;
        public const UInt32 FILE_DEVICE_MODEM = 0x0000002b;
        public const UInt32 FILE_DEVICE_VDM = 0x0000002c;
        public const UInt32 FILE_DEVICE_MASS_STORAGE = 0x0000002d;
        #endregion

        #region METHOD_CODES
        public const UInt32 METHOD_BUFFERED = 0x00000000;
        public const UInt32 METHOD_IN_DIRECT = 0x00000001;
        public const UInt32 METHOD_OUT_DIRECT = 0x00000002;
        public const UInt32 METHOD_NEITHER = 0x00000003;
        #endregion

        #region FILE_ACCESS_CODES
        public const UInt32 FILE_ANY_ACCESS = 0x00000000;
        public const UInt32 FILE_READ_ACCESS = 0x00000001;
        public const UInt32 FILE_WRITE_ACCESS = 0x00000002;
        #endregion

        public static UInt32 CTL_CODE(UInt32 DeviceType, UInt32 Function, UInt32 Method, UInt32 Access)
        {
            return (UInt32)(((DeviceType) << 16) | ((Access) << 14) | ((Function) << 2) | (Method));
        }

        #region SystemEnumGuidMappings
        #region DeviceClassGuidMap
        private static Dictionary<DeviceSetupClassGuids, string> _devclassguidmap = new Dictionary<DeviceSetupClassGuids, string>() {
			{ DeviceSetupClassGuids.Battery, "{72631e54-78a4-11d0-bcf7-00aa00b7b32a}" },
			{ DeviceSetupClassGuids.Biometric, "{53D29EF7-377C-4D14-864B-EB3A85769359}" },
			{ DeviceSetupClassGuids.Bluetooth, "{e0cbf06c-cd8b-4647-bb8a-263b43f0f974}" },
			{ DeviceSetupClassGuids.CDROM, "{4d36e965-e325-11ce-bfc1-08002be10318}" },
			{ DeviceSetupClassGuids.DiskDrive, "{4d36e967-e325-11ce-bfc1-08002be10318}" },
			{ DeviceSetupClassGuids.Display, "{4d36e968-e325-11ce-bfc1-08002be10318}" },
			{ DeviceSetupClassGuids.FloppyDiskController, "{4d36e969-e325-11ce-bfc1-08002be10318}" },
			{ DeviceSetupClassGuids.FloppyDisk, "{4d36e980-e325-11ce-bfc1-08002be10318}" },
			{ DeviceSetupClassGuids.HardDiskController, "{4d36e96a-e325-11ce-bfc1-08002be10318}" },
			{ DeviceSetupClassGuids.HIDClass, "{745a17a0-74d3-11d0-b6fe-00a0c90f57da}" },
			{ DeviceSetupClassGuids.Image, "{6bdd1fc6-810f-11d0-bec7-08002be2092f}" },
			{ DeviceSetupClassGuids.Keyboard, "{4d36e96b-e325-11ce-bfc1-08002be10318}" },

			/** GAP : http://msdn.microsoft.com/en-us/library/windows/hardware/ff553426(v=vs.85).aspx **/

			{ DeviceSetupClassGuids.USBDevice, "{88BAE032-5A81-49f0-BC3D-A4FF138216D6}" },
		};
        #endregion

        #region DeviceInterfaceGuidMap
        private static Dictionary<DeviceInterfaceClassGuids, string> _devifaceguidmap = new Dictionary<DeviceInterfaceClassGuids, string>()
		{
			{ DeviceInterfaceClassGuids.USBDevice, "{A5DCBF10-6530-11D2-901F-00C04FB951ED}" },
			{ DeviceInterfaceClassGuids.USBController, "{3ABF6F2D-71C4-462A-8A92-1E6861E6AF27}" },
			{ DeviceInterfaceClassGuids.USBHub, "{F18A0E88-C30C-11D0-8815-00A0C906BED8}" },
			{ DeviceInterfaceClassGuids.Bus1394, "{6BDD1FC1-810F-11d0-BEC7-08002BE2092F}" },
			{ DeviceInterfaceClassGuids.IEEE61883, "{7EBEFBC0-3200-11d2-B4C2-00A0C9697D07}" },
			{ DeviceInterfaceClassGuids.HID, "{4D1E55B2-F16F-11CF-88CB-001111000030}" },
			{ DeviceInterfaceClassGuids.Keyboard, "{884b96c3-56ef-11d1-bc8c-00a0c91405dd}" },
			{ DeviceInterfaceClassGuids.Mouse, "{378DE44C-56EF-11D1-BC8C-00A0C91405DD}" },
			{ DeviceInterfaceClassGuids.Modem, "{2C7089AA-2E0E-11D1-B114-00C04FC2AAE4}" },
		};
        #endregion
        #endregion

        public class DeviceNotReadyException : Exception
        {
            public DeviceNotReadyException(string message) : base(message) { }
        }
    }
}
