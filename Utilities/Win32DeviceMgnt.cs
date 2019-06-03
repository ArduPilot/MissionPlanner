using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using log4net;
using System.Reflection;
using MissionPlanner.ArduPilot;

//http://www.nakov.com/blog/2009/05/10/enumerate-all-com-ports-and-find-their-name-and-description-in-c/
public class Win32DeviceMgmt
{
    private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private const int BUFFER_SIZE=255;
    private const UInt32 DIGCF_PRESENT = 0x00000002;
    private const UInt32 DIGCF_DEVICEINTERFACE = 0x00000010;
    private const UInt32 SPDRP_DEVICEDESC = 0x00000000;
    private const UInt32 DICS_FLAG_GLOBAL = 0x00000001;
    private const UInt32 DIREG_DEV = 0x00000001;
    private const UInt32 KEY_QUERY_VALUE = 0x0001;
    /// <summary>
    /// https://docs.microsoft.com/en-us/windows-hardware/drivers/install/guid-devinterface-comport
    /// </summary>
    private const string GUID_DEVINTERFACE_COMPORT = "86E0D1E0-8089-11D0-9CE4-08003E301F73";
    /// <summary>
    /// https://docs.microsoft.com/en-us/windows-hardware/drivers/install/guid-devinterface-usb-device
    /// </summary>
    private const string GUID_DEVINTERFACE_USB_DEVICE = "A5DCBF10-6530-11D2-901F-00C04FB951ED";

    /// <summary>
    /// Device registry property codes
    /// </summary>
    public enum SPDRP : int
    {
        /// <summary>
        /// DeviceDesc (R/W)
        /// </summary>
        SPDRP_DEVICEDESC = 0x00000000,

        /// <summary>
        /// HardwareID (R/W)
        /// </summary>
        SPDRP_HARDWAREID = 0x00000001,

        /// <summary>
        /// CompatibleIDs (R/W)
        /// </summary>
        SPDRP_COMPATIBLEIDS = 0x00000002,

        /// <summary>
        /// unused
        /// </summary>
        SPDRP_UNUSED0 = 0x00000003,

        /// <summary>
        /// Service (R/W)
        /// </summary>
        SPDRP_SERVICE = 0x00000004,

        /// <summary>
        /// unused
        /// </summary>
        SPDRP_UNUSED1 = 0x00000005,

        /// <summary>
        /// unused
        /// </summary>
        SPDRP_UNUSED2 = 0x00000006,

        /// <summary>
        /// Class (R--tied to ClassGUID)
        /// </summary>
        SPDRP_CLASS = 0x00000007,

        /// <summary>
        /// ClassGUID (R/W)
        /// </summary>
        SPDRP_CLASSGUID = 0x00000008,

        /// <summary>
        /// Driver (R/W)
        /// </summary>
        SPDRP_DRIVER = 0x00000009,

        /// <summary>
        /// ConfigFlags (R/W)
        /// </summary>
        SPDRP_CONFIGFLAGS = 0x0000000A,

        /// <summary>
        /// Mfg (R/W)
        /// </summary>
        SPDRP_MFG = 0x0000000B,

        /// <summary>
        /// FriendlyName (R/W)
        /// </summary>
        SPDRP_FRIENDLYNAME = 0x0000000C,

        /// <summary>
        /// LocationInformation (R/W)
        /// </summary>
        SPDRP_LOCATION_INFORMATION = 0x0000000D,

        /// <summary>
        /// PhysicalDeviceObjectName (R)
        /// </summary>
        SPDRP_PHYSICAL_DEVICE_OBJECT_NAME = 0x0000000E,

        /// <summary>
        /// Capabilities (R)
        /// </summary>
        SPDRP_CAPABILITIES = 0x0000000F,

        /// <summary>
        /// UiNumber (R)
        /// </summary>
        SPDRP_UI_NUMBER = 0x00000010,

        /// <summary>
        /// UpperFilters (R/W)
        /// </summary>
        SPDRP_UPPERFILTERS = 0x00000011,

        /// <summary>
        /// LowerFilters (R/W)
        /// </summary>
        SPDRP_LOWERFILTERS = 0x00000012,

        /// <summary>
        /// BusTypeGUID (R)
        /// </summary>
        SPDRP_BUSTYPEGUID = 0x00000013,

        /// <summary>
        /// LegacyBusType (R)
        /// </summary>
        SPDRP_LEGACYBUSTYPE = 0x00000014,

        /// <summary>
        /// BusNumber (R)
        /// </summary>
        SPDRP_BUSNUMBER = 0x00000015,

        /// <summary>
        /// Enumerator Name (R)
        /// </summary>
        SPDRP_ENUMERATOR_NAME = 0x00000016,

        /// <summary>
        /// Security (R/W, binary form)
        /// </summary>
        SPDRP_SECURITY = 0x00000017,

        /// <summary>
        /// Security (W, SDS form)
        /// </summary>
        SPDRP_SECURITY_SDS = 0x00000018,

        /// <summary>
        /// Device Type (R/W)
        /// </summary>
        SPDRP_DEVTYPE = 0x00000019,

        /// <summary>
        /// Device is exclusive-access (R/W)
        /// </summary>
        SPDRP_EXCLUSIVE = 0x0000001A,

        /// <summary>
        /// Device Characteristics (R/W)
        /// </summary>
        SPDRP_CHARACTERISTICS = 0x0000001B,

        /// <summary>
        /// Device Address (R)
        /// </summary>
        SPDRP_ADDRESS = 0x0000001C,

        /// <summary>
        /// UiNumberDescFormat (R/W)
        /// </summary>
        SPDRP_UI_NUMBER_DESC_FORMAT = 0X0000001D,

        /// <summary>
        /// Device Power Data (R)
        /// </summary>
        SPDRP_DEVICE_POWER_DATA = 0x0000001E,

        /// <summary>
        /// Removal Policy (R)
        /// </summary>
        SPDRP_REMOVAL_POLICY = 0x0000001F,

        /// <summary>
        /// Hardware Removal Policy (R)
        /// </summary>
        SPDRP_REMOVAL_POLICY_HW_DEFAULT = 0x00000020,

        /// <summary>
        /// Removal Policy Override (RW)
        /// </summary>
        SPDRP_REMOVAL_POLICY_OVERRIDE = 0x00000021,

        /// <summary>
        /// Device Install State (R)
        /// </summary>
        SPDRP_INSTALL_STATE = 0x00000022,

        /// <summary>
        /// Device Location Paths (R)
        /// </summary>
        SPDRP_LOCATION_PATHS = 0x00000023,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SP_DEVINFO_DATA
    {
        public Int32 cbSize;
        public Guid ClassGuid;
        public Int32 DevInst;
        public UIntPtr Reserved;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SP_DEVICE_INTERFACE_DATA
    {
        public Int32 cbSize;
        public Guid interfaceClassGuid;
        public Int32 flags;
        private UIntPtr reserved;
    }

    [DllImport(@"setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern Boolean SetupDiEnumDeviceInterfaces(
        IntPtr hDevInfo,
        ref SP_DEVINFO_DATA devInfo,
        ref Guid interfaceClassGuid,
        UInt32 memberIndex,
        ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData
    );

    [DllImport("setupapi.dll")]
    private static extern Int32 SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);

    [DllImport(@"setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool SetupDiEnumDeviceInfo(IntPtr DeviceInfoSet, Int32 MemberIndex, ref SP_DEVINFO_DATA DeviceInterfaceData);

    [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool SetupDiGetDeviceRegistryProperty(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData,
        SPDRP property, out UInt32 propertyRegDataType, StringBuilder propertyBuffer, uint propertyBufferSize, out UInt32 requiredSize);

    [DllImport("setupapi.dll", SetLastError = true)]
    private static extern IntPtr SetupDiGetClassDevs(ref Guid gClass, UInt32 iEnumerator, IntPtr hParent, UInt32 nFlags);

    [DllImport("Setupapi", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetupDiOpenDevRegKey(IntPtr hDeviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, uint scope,
        uint hwProfile, uint parameterRegistryValueKind, uint samDesired);

    [DllImport("setupapi.dll", SetLastError = true)]
    static extern bool SetupDiGetDevicePropertyW(
        IntPtr deviceInfoSet,
        ref SP_DEVINFO_DATA DeviceInfoData,
        ref DEVPROPKEY propertyKey,
        out UInt64 propertyType, // or Uint32 ?
        IntPtr propertyBuffer, // or byte[] 
        Int32 propertyBufferSize,
        out int requiredSize, // <---- 
        UInt32 flags);

    // Device Property 
    [StructLayout(LayoutKind.Sequential)]
    internal struct DEVPROPKEY
    {
        public Guid fmtid;
        public UInt32 pid;
        public string name;
    }

    /// <summary>
    /// The values for iManufacturer, iProduct, and iSerialNumber are just indexs that are used by the USB_STRING_DESCRIPTOR request
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct USB_DEVICE_DESCRIPTOR
    {
        public byte bLength;
        public byte bDescriptorType;
        public ushort bcdUSB;
        public byte bDeviceClass;
        public byte bDeviceSubClass;
        public byte bDeviceProtocol;
        public byte bMaxPacketSize0;
        public ushort idVendor;
        public ushort idProduct;
        public ushort bcdDevice;
        public byte iManufacturer;
        public byte iProduct;
        public byte iSerialNumber;
        public byte bNumConfigurations;
    }
    const int MAXIMUM_USB_STRING_LENGTH = 255;
    const int USB_STRING_DESCRIPTOR_TYPE = 3;
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct USB_STRING_DESCRIPTOR
    {
        public byte bLength;
        public byte bDescriptorType;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAXIMUM_USB_STRING_LENGTH)]
        public string bString;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct USB_SETUP_PACKET
    {
        public byte bmRequest;
        public byte bRequest;
        public short wValue;
        public short wIndex;
        public short wLength;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct USB_DESCRIPTOR_REQUEST
    {
        public int ConnectionIndex;
        public USB_SETUP_PACKET SetupPacket;
        //public byte[] Data;
    }
    [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Auto)]
    static extern bool DeviceIoControl(IntPtr hDevice, uint dwIoControlCode,
        IntPtr lpInBuffer, int nInBufferSize,
        IntPtr lpOutBuffer, int nOutBufferSize,
        out int lpBytesReturned, IntPtr lpOverlapped);

    const int IOCTL_USB_GET_DESCRIPTOR_FROM_NODE_CONNECTION = 0x220410;
    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern IntPtr CreateFile(
        [MarshalAs(UnmanagedType.LPTStr)] string filename,
        [MarshalAs(UnmanagedType.U4)] FileAccess access,
        [MarshalAs(UnmanagedType.U4)] FileShare share,
        IntPtr securityAttributes, // optional SECURITY_ATTRIBUTES struct or IntPtr.Zero
        [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
        [MarshalAs(UnmanagedType.U4)] FileAttributes flagsAndAttributes,
        IntPtr templateFile);

    public static string GetManufact(IntPtr h , USB_DEVICE_DESCRIPTOR PortDeviceDescriptor, int PortPortNumber)
    {
        if (PortDeviceDescriptor.iManufacturer > 0)
        {
            int nBytesReturned;
            int nBytes = BUFFER_SIZE;

            // build a request for string descriptor
            USB_DESCRIPTOR_REQUEST Request = new USB_DESCRIPTOR_REQUEST();
            Request.ConnectionIndex = PortPortNumber;
            Request.SetupPacket.wValue = (short)((USB_STRING_DESCRIPTOR_TYPE << 8) + PortDeviceDescriptor.iManufacturer);
            Request.SetupPacket.wLength = (short)(nBytes - Marshal.SizeOf(Request));
            Request.SetupPacket.wIndex = 0x409; // Language Code

            // Geez, I wish C# had a Marshal.MemSet() method
            string NullString = new string((char)0, nBytes / Marshal.SystemDefaultCharSize);
            IntPtr ptrRequest = Marshal.StringToHGlobalAuto(NullString);
            Marshal.StructureToPtr(Request, ptrRequest, true);

            // Use an IOCTL call to request the String Descriptor
            if (DeviceIoControl(h, IOCTL_USB_GET_DESCRIPTOR_FROM_NODE_CONNECTION, ptrRequest, nBytes, ptrRequest, nBytes, out nBytesReturned, IntPtr.Zero))
            {
                // The location of the string descriptor is immediately after
                // the Request structure.  Because this location is not "covered"
                // by the structure allocation, we're forced to zero out this
                // chunk of memory by using the StringToHGlobalAuto() hack above
                IntPtr ptrStringDesc = new IntPtr(ptrRequest.ToInt32() + Marshal.SizeOf(Request));
                USB_STRING_DESCRIPTOR StringDesc = (USB_STRING_DESCRIPTOR)Marshal.PtrToStructure(ptrStringDesc, typeof(USB_STRING_DESCRIPTOR));
                return StringDesc.bString;
            }
            Marshal.FreeHGlobal(ptrRequest);
        }

        return "";
    }

    [DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "RegQueryValueExW", SetLastError = true)]
    private static extern int RegQueryValueEx(IntPtr hKey, string lpValueName, int lpReserved, out uint lpType,
        StringBuilder lpData, ref uint lpcbData);

    [DllImport("advapi32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    private static extern int RegCloseKey(IntPtr hKey);

    [DllImport("kernel32.dll")]
    private static extern Int32 GetLastError();



    public static List<DeviceInfo> GetAllCOMPorts()
    {
        // windows 7 virtualcoms
        var devices = GetClassDevs(GUID_DEVINTERFACE_USB_DEVICE).ToList();

        // window 10
        devices.AddRange(GetClassDevs(GUID_DEVINTERFACE_COMPORT));

        return devices;
    }

    private static List<DeviceInfo> GetClassDevs(string guid)
    {
        Guid guidComPorts = new Guid(guid);
        IntPtr hDeviceInfoSet = SetupDiGetClassDevs(
            ref guidComPorts, 0, IntPtr.Zero, DIGCF_PRESENT | DIGCF_DEVICEINTERFACE);
        if (hDeviceInfoSet == IntPtr.Zero)
        {
            throw new Exception("Failed to get device information set for the COM ports");
        }

        try
        {
            List<DeviceInfo> devices = new List<DeviceInfo>();
            Int32 iMemberIndex = 0;
            while (true)
            {
                SP_DEVINFO_DATA deviceInfoData = new SP_DEVINFO_DATA();
                deviceInfoData.cbSize = Marshal.SizeOf(typeof(SP_DEVINFO_DATA));
                SP_DEVICE_INTERFACE_DATA deviceInterfaceData = new SP_DEVICE_INTERFACE_DATA();
                deviceInterfaceData.cbSize = Marshal.SizeOf(typeof(SP_DEVICE_INTERFACE_DATA));
                //bool success = SetupDiEnumDeviceInterfaces(hDeviceInfoSet,ref deviceInfoData, ref guidComPorts, iMemberIndex, ref deviceInterfaceData);
                bool success = SetupDiEnumDeviceInfo(hDeviceInfoSet, iMemberIndex, ref deviceInfoData);
                if (!success)
                {
                    log.Info("no more devices " + GetLastError());
                    // No more devices in the device information set
                    break;
                }

                // hDeviceInfoSet - needs to be the hub
                // var manu = GetManufact(hDeviceInfoSet, new USB_DEVICE_DESCRIPTOR() {iManufacturer = 1}, pp);

                DeviceInfo deviceInfo = new DeviceInfo();
                try
                {
                    deviceInfo.name = GetDeviceName(hDeviceInfoSet, deviceInfoData);
                }
                catch (Exception e)
                {
                    log.Error(e);
                    iMemberIndex++;
                    continue;
                }

                try
                {
                    deviceInfo.description =
                        GetDeviceDescription(hDeviceInfoSet, deviceInfoData, SPDRP.SPDRP_DEVICEDESC);
                }
                catch (Exception e)
                {
                    log.Error(e);
                    iMemberIndex++;
                    continue;
                }

                try
                {
                    deviceInfo.hardwareid =
                        GetDeviceDescription(hDeviceInfoSet, deviceInfoData, SPDRP.SPDRP_HARDWAREID);
                }
                catch (Exception e)
                {
                    log.Error(e);
                    iMemberIndex++;
                    continue;
                }

                foreach (SPDRP prop in Enum.GetValues(typeof(SPDRP)))
                {
                    try
                    {
                        log.Info((SPDRP) prop + ": " +
                                 GetDeviceDescription(hDeviceInfoSet, deviceInfoData, (SPDRP) prop));
                    }
                    catch
                    {
                    }
                }

                //https://github.com/tpn/winsdk-10/blob/master/Include/10.0.10240.0/shared/devpkey.h

                var list = new[]
                {
                    DEFINE_DEVPROPKEY("DEVPKEY_Device_DeviceDesc", 0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1,
                        0x46, 0xa8, 0x50, 0xe0, 2), // DEVPROP_TYPE_STRING
                    DEFINE_DEVPROPKEY("DEVPKEY_Device_Manufacturer", 0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1,
                        0x46, 0xa8, 0x50, 0xe0, 13), // DEVPROP_TYPE_STRING
                    DEFINE_DEVPROPKEY("DEVPKEY_Device_FriendlyName", 0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1,
                        0x46, 0xa8, 0x50, 0xe0, 14), // DEVPROP_TYPE_STRING
                    DEFINE_DEVPROPKEY("DEVPKEY_Device_EnumeratorName", 0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67,
                        0xd1, 0x46, 0xa8, 0x50, 0xe0, 24), // DEVPROP_TYPE_STRING

                    DEFINE_DEVPROPKEY("DEVPKEY_Device_Model", 0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52,
                        0x99, 0x6e, 0x57, 39), // DEVPROP_TYPE_STRING

                    DEFINE_DEVPROPKEY("DEVPKEY_Device_BusReportedDeviceDesc", 0x540b947e, 0x8b40, 0x45bc, 0xa8, 0xa2,
                        0x6a, 0x0b, 0x89, 0x4c, 0xbd, 0xa2, 4), // DEVPROP_TYPE_STRING

                    DEFINE_DEVPROPKEY("DEVPKEY_DeviceContainer_FriendlyName", 0x656A3BB3, 0xECC0, 0x43FD, 0x84, 0x77,
                        0x4A, 0xE0, 0x40, 0x4A, 0x96, 0xCD, 12288), // DEVPROP_TYPE_STRING

                    DEFINE_DEVPROPKEY("DEVPKEY_DeviceContainer_Manufacturer", 0x656A3BB3, 0xECC0, 0x43FD, 0x84, 0x77,
                        0x4A, 0xE0, 0x40, 0x4A, 0x96, 0xCD, 8192), // DEVPROP_TYPE_STRING

                    DEFINE_DEVPROPKEY("DEVPKEY_DeviceContainer_ModelName", 0x656A3BB3, 0xECC0, 0x43FD, 0x84, 0x77, 0x4A,
                        0xE0, 0x40, 0x4A, 0x96, 0xCD, 8194), // DEVPROP_TYPE_STRING (localizable)

                    DEFINE_DEVPROPKEY("DEVPKEY_DeviceContainer_ModelNumber", 0x656A3BB3, 0xECC0, 0x43FD, 0x84, 0x77,
                        0x4A, 0xE0, 0x40, 0x4A, 0x96, 0xCD, 8195), // DEVPROP_TYPE_STRING
                    DEFINE_DEVPROPKEY("DEVPKEY_DeviceContainer_DeviceDescription1", 0x78c34fc8, 0x104a, 0x4aca, 0x9e,
                        0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57, 81), // DEVPROP_TYPE_STRING

                    DEFINE_DEVPROPKEY("DEVPKEY_DeviceContainer_DeviceDescription2", 0x78c34fc8, 0x104a, 0x4aca, 0x9e,
                        0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57, 82), // DEVPROP_TYPE_STRING
                };

                ulong propertyType = 0;
                int requiredSize = 0;
                for (int i = 0; i < list.Length; i++)
                {
                    IntPtr buffer = Marshal.AllocHGlobal(1024);
                    try
                    {
                        if (SetupDiGetDevicePropertyW(hDeviceInfoSet, ref deviceInfoData, ref list[i], out propertyType,
                            buffer, 1024, out requiredSize, 0))
                        {
                            var out11 = Marshal.PtrToStringAuto(buffer);
                            log.Info(list[i].name + " " + out11);

                            if (list[i].name == "DEVPKEY_Device_BusReportedDeviceDesc")
                                deviceInfo.board = out11;
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }

                    Marshal.FreeHGlobal(buffer);
                }

                devices.Add(deviceInfo);

                iMemberIndex++;
            }

            return devices;
        }
        finally
        {
            SetupDiDestroyDeviceInfoList(hDeviceInfoSet);
        }
    }

    private static DEVPROPKEY DEFINE_DEVPROPKEY(string dEVPKEY_Device_Manufacturer, uint v1, int v2, int v3, byte v4, byte v5, byte v6, byte v7, byte v8, byte v9, byte v10, byte v11, uint v12)
    {
        DEVPROPKEY key = new DEVPROPKEY();
        key.pid = v12;
        key.fmtid = new Guid(v1, (ushort) v2, (ushort) v3, v4, v5, v6, v7, v8, v9, v10, v11);
        key.name = dEVPKEY_Device_Manufacturer;
        return key;
    }

    private static string GetDeviceName(IntPtr pDevInfoSet, SP_DEVINFO_DATA deviceInfoData)
    {
        IntPtr hDeviceRegistryKey = SetupDiOpenDevRegKey(pDevInfoSet, ref deviceInfoData,
            DICS_FLAG_GLOBAL, 0, DIREG_DEV, KEY_QUERY_VALUE);
        if (hDeviceRegistryKey == IntPtr.Zero)
        {
            throw new Exception("Failed to open a registry key for device-specific configuration information");
        }

        StringBuilder deviceNameBuf = new StringBuilder(256);
        try
        {
            uint lpRegKeyType;
            uint length = (uint)deviceNameBuf.Capacity;
            int result = RegQueryValueEx(hDeviceRegistryKey, "PortName", 0, out lpRegKeyType, deviceNameBuf, ref length);
            if (result != 0)
            {
                throw new Exception("Can not read registry value PortName for device " + deviceInfoData.ClassGuid);
            }
        }
        finally
        {
            RegCloseKey(hDeviceRegistryKey);
        }

        string deviceName = deviceNameBuf.ToString();
        return deviceName;
    }

    private static string GetDeviceDescription(IntPtr hDeviceInfoSet, SP_DEVINFO_DATA deviceInfoData, SPDRP property)
    {
        StringBuilder descriptionBuf = new StringBuilder(256);
        uint propRegDataType;
        uint length = (uint)descriptionBuf.Capacity;
        bool success = SetupDiGetDeviceRegistryProperty(hDeviceInfoSet, ref deviceInfoData, property,
            out propRegDataType, descriptionBuf, length, out length);
        if (!success)
        {
            throw new Exception("Can not read registry value PortName for device " + deviceInfoData.ClassGuid);
        }
        string deviceDescription = descriptionBuf.ToString();
        return deviceDescription;
    }

}