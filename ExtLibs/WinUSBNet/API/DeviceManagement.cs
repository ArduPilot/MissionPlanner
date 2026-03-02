/*  WinUSBNet library
 *  (C) 2010 Thomas Bleeker (www.madwizard.org)
 *
 *  Licensed under the MIT license, see license.txt or:
 *  http://www.opensource.org/licenses/mit-license.php
 */

/* NOTE: Parts of the code in this file are based on the work of Jan Axelson
 * See http://www.lvr.com/winusb.htm for more information
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

using Windows.Win32.Devices.DeviceAndDriverInstallation;
using Windows.Win32.Foundation;

namespace Nefarius.Drivers.WinUSB.API;

/// <summary>
///     Routines for detecting devices and receiving device notifications.
/// </summary>
internal static partial class DeviceManagement
{
    private static readonly IntPtr INVALID_HANDLE_VALUE = new(-1);

    private static byte[] GetProperty(IntPtr deviceInfoSet, SP_DEVINFO_DATA deviceInfoData,
        SETUP_DI_REGISTRY_PROPERTY property,
        out int regType)
    {
        uint requiredSize;

        if (!SetupDiGetDeviceRegistryProperty(deviceInfoSet, ref deviceInfoData, property, IntPtr.Zero, IntPtr.Zero, 0,
                out requiredSize))
        {
            if (Marshal.GetLastWin32Error() != (int)WIN32_ERROR.ERROR_INSUFFICIENT_BUFFER)
            {
                throw APIException.Win32("Failed to get buffer size for device registry property.");
            }
        }

        byte[] buffer = new byte[requiredSize];

        if (!SetupDiGetDeviceRegistryProperty(deviceInfoSet, ref deviceInfoData, property, out regType, buffer,
                (uint)buffer.Length, out requiredSize))
        {
            throw APIException.Win32("Failed to get device registry property.");
        }

        return buffer;
    }

    // todo: is the queried data always available, or should we check ERROR_INVALID_DATA?
    private static string GetStringProperty(IntPtr deviceInfoSet, SP_DEVINFO_DATA deviceInfoData,
        SETUP_DI_REGISTRY_PROPERTY property)
    {
        int regType;
        byte[] buffer = GetProperty(deviceInfoSet, deviceInfoData, property, out regType);
        if (regType != (int)RegTypes.REG_SZ)
        {
            throw new APIException("Invalid registry type returned for device property.");
        }

        // sizof(char), 2 bytes, are removed to leave out the string terminator
        return Encoding.Unicode.GetString(buffer, 0, buffer.Length - sizeof(char));
    }

    private static string[] GetMultiStringProperty(IntPtr deviceInfoSet, SP_DEVINFO_DATA deviceInfoData,
        SETUP_DI_REGISTRY_PROPERTY property)
    {
        int regType;
        byte[] buffer = GetProperty(deviceInfoSet, deviceInfoData, property, out regType);
        if (regType != (int)RegTypes.REG_MULTI_SZ)
        {
            throw new APIException("Invalid registry type returned for device property.");
        }

        string fullString = Encoding.Unicode.GetString(buffer);

        return fullString.Split(new[] { '\0' }, StringSplitOptions.RemoveEmptyEntries);
    }

    private static DeviceDetails GetDeviceDetails(string devicePath, IntPtr deviceInfoSet,
        SP_DEVINFO_DATA deviceInfoData)
    {
        DeviceDetails details = new();
        details.DevicePath = devicePath;
        details.DeviceDescription =
            GetStringProperty(deviceInfoSet, deviceInfoData, SETUP_DI_REGISTRY_PROPERTY.SPDRP_DEVICEDESC);
        details.Manufacturer = GetStringProperty(deviceInfoSet, deviceInfoData, SETUP_DI_REGISTRY_PROPERTY.SPDRP_MFG);
        string[] hardwareIDs =
            GetMultiStringProperty(deviceInfoSet, deviceInfoData, SETUP_DI_REGISTRY_PROPERTY.SPDRP_HARDWAREID);

        bool foundVid = false;
        bool foundPid = false;
        foreach (string idString in hardwareIDs)
        {
            if (!idString.StartsWith("USB\\"))
            {
                continue;
            }

            string hardwareId = idString.Substring("USB\\".Length);

            // The expected format here is `USB\VID_1234&PID_5678&...`.
            // This is not guaranteed for all USB devices, however. There might be additional
            // leading info which breaks a more naive check, such as this case seen in the wild:
            // `USB\ASMEDIAUSBD_HubSS&VER01166101&VID_0424&PID_5807&...`
            // So we split the string and check each component separately.

            static void ParseHardwareId(string component, string prefix, ref ushort result, ref bool found)
            {
                if (found || !component.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                string digits = component.Substring(prefix.Length);
                if (ushort.TryParse(digits, NumberStyles.AllowHexSpecifier, null, out ushort vid))
                {
                    result = vid;
                    found = true;
                }
            }

            foreach (string component in hardwareId.Split('&'))
            {
                ParseHardwareId(component, "VID_", ref details.VID, ref foundVid);
                ParseHardwareId(component, "PID_", ref details.PID, ref foundPid);

                if (foundVid && foundPid)
                {
                    break;
                }
            }
        }

        if (!foundVid || !foundPid)
        {
            throw new APIException("Failed to find VID and PID for USB device. No hardware ID could be parsed.");
        }

        return details;
    }


    public static DeviceDetails[] FindDevicesFromGuid(Guid guid)
    {
        IntPtr deviceInfoSet = IntPtr.Zero;
        List<DeviceDetails> deviceList = new();
        try
        {
            deviceInfoSet = SetupDiGetClassDevs(ref guid, IntPtr.Zero, IntPtr.Zero,
                (int)(SETUP_DI_GET_CLASS_DEVS_FLAGS.DIGCF_PRESENT |
                      SETUP_DI_GET_CLASS_DEVS_FLAGS.DIGCF_DEVICEINTERFACE));
            if (deviceInfoSet == INVALID_HANDLE_VALUE)
            {
                throw APIException.Win32("Failed to enumerate devices.");
            }

            int memberIndex = 0;
            while (true)
            {
                // Begin with 0 and increment through the device information set until
                // no more devices are available.
                SP_DEVICE_INTERFACE_DATA deviceInterfaceData = new();

                // The cbSize element of the deviceInterfaceData structure must be set to
                // the structure's size in bytes.
                // The size is 28 bytes for 32-bit code and 32 bytes for 64-bit code.
                deviceInterfaceData.cbSize = Marshal.SizeOf(deviceInterfaceData);

                bool success;

                success = SetupDiEnumDeviceInterfaces(deviceInfoSet, IntPtr.Zero, ref guid, memberIndex,
                    ref deviceInterfaceData);

                // Find out if a device information set was retrieved.
                if (!success)
                {
                    int lastError = Marshal.GetLastWin32Error();
                    if (lastError == (int)WIN32_ERROR.ERROR_NO_MORE_ITEMS)
                    {
                        break;
                    }

                    throw APIException.Win32("Failed to get device interface.");
                }
                // A device is present.

                int bufferSize = 0;

                success = SetupDiGetDeviceInterfaceDetail
                (deviceInfoSet,
                    ref deviceInterfaceData,
                    IntPtr.Zero,
                    0,
                    ref bufferSize,
                    IntPtr.Zero);

                if (!success)
                {
                    if (Marshal.GetLastWin32Error() != (int)WIN32_ERROR.ERROR_INSUFFICIENT_BUFFER)
                    {
                        throw APIException.Win32("Failed to get interface details buffer size.");
                    }
                }

                IntPtr detailDataBuffer = IntPtr.Zero;
                try
                {
                    // Allocate memory for the SP_DEVICE_INTERFACE_DETAIL_DATA structure using the returned buffer size.
                    detailDataBuffer = Marshal.AllocHGlobal(bufferSize);

                    // Store cbSize in the first bytes of the array. The number of bytes varies with 32- and 64-bit systems.

                    Marshal.WriteInt32(detailDataBuffer, IntPtr.Size == 4 ? 4 + Marshal.SystemDefaultCharSize : 8);

                    // Call SetupDiGetDeviceInterfaceDetail again.
                    // This time, pass a pointer to DetailDataBuffer
                    // and the returned required buffer size.

                    // build a DevInfo Data structure
                    SP_DEVINFO_DATA da = new();
                    da.cbSize = Marshal.SizeOf(da);


                    success = SetupDiGetDeviceInterfaceDetail
                    (deviceInfoSet,
                        ref deviceInterfaceData,
                        detailDataBuffer,
                        bufferSize,
                        ref bufferSize,
                        ref da);

                    if (!success)
                    {
                        throw APIException.Win32("Failed to get device interface details.");
                    }


                    // Skip over cbsize (4 bytes) to get the address of the devicePathName.

                    IntPtr pDevicePathName = new(detailDataBuffer.ToInt64() + 4);
                    string pathName = Marshal.PtrToStringUni(pDevicePathName);

                    // Get the String containing the devicePathName.

                    DeviceDetails details = GetDeviceDetails(pathName, deviceInfoSet, da);


                    deviceList.Add(details);
                }
                finally
                {
                    if (detailDataBuffer != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(detailDataBuffer);
                        detailDataBuffer = IntPtr.Zero;
                    }
                }

                memberIndex++;
            }
        }
        finally
        {
            if (deviceInfoSet != IntPtr.Zero && deviceInfoSet != INVALID_HANDLE_VALUE)
            {
                SetupDiDestroyDeviceInfoList(deviceInfoSet);
            }
        }

        return deviceList.ToArray();
    }
}