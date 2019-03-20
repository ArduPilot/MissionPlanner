using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using log4net;
using System.Reflection;
//http://www.nakov.com/blog/2009/05/10/enumerate-all-com-ports-and-find-their-name-and-description-in-c/
public class Win32DeviceMgmt
{
    private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


    private const UInt32 DIGCF_PRESENT = 0x00000002;
    private const UInt32 DIGCF_DEVICEINTERFACE = 0x00000010;
    private const UInt32 SPDRP_DEVICEDESC = 0x00000000;
    private const UInt32 DICS_FLAG_GLOBAL = 0x00000001;
    private const UInt32 DIREG_DEV = 0x00000001;
    private const UInt32 KEY_QUERY_VALUE = 0x0001;
    private const string GUID_DEVINTERFACE_COMPORT = "86E0D1E0-8089-11D0-9CE4-08003E301F73";

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
    private struct SP_DEVINFO_DATA
    {
        public Int32 cbSize;
        public Guid ClassGuid;
        public Int32 DevInst;
        public UIntPtr Reserved;
    };

    [DllImport("setupapi.dll")]
    private static extern Int32 SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);

    [DllImport("setupapi.dll")]
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

    [DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "RegQueryValueExW", SetLastError = true)]
    private static extern int RegQueryValueEx(IntPtr hKey, string lpValueName, int lpReserved, out uint lpType,
        StringBuilder lpData, ref uint lpcbData);

    [DllImport("advapi32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    private static extern int RegCloseKey(IntPtr hKey);

    [DllImport("kernel32.dll")]
    private static extern Int32 GetLastError();

    public struct DeviceInfo
    {
        public string name;
        public string description;
        public string board;
        public string hardwareid;
    }

    public static List<DeviceInfo> GetAllCOMPorts()
    {
        Guid guidComPorts = new Guid(GUID_DEVINTERFACE_COMPORT);
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
                bool success = SetupDiEnumDeviceInfo(hDeviceInfoSet, iMemberIndex, ref deviceInfoData);
                if (!success)
                {
                    // No more devices in the device information set
                    break;
                }

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
                    SetupDiGetDevicePropertyW(hDeviceInfoSet, ref deviceInfoData, ref list[i], out propertyType,
                        buffer, 1024, out requiredSize, 0);

                    var out11 = Marshal.PtrToStringAuto(buffer);
                    log.Info(list[i].name + " " + out11);

                    if (list[i].name == "DEVPKEY_Device_BusReportedDeviceDesc")
                        deviceInfo.board = out11;

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