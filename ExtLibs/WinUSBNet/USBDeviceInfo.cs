/*  WinUSBNet library
 *  (C) 2010 Thomas Bleeker (www.madwizard.org)
 *
 *  Licensed under the MIT license, see license.txt or:
 *  http://www.opensource.org/licenses/mit-license.php
 */

using System.Diagnostics.CodeAnalysis;

using Nefarius.Drivers.WinUSB.API;

namespace Nefarius.Drivers.WinUSB;

/// <summary>
///     Gives information about a device. This information is retrieved using the setup API, not the
///     actual device descriptor. Device description and manufacturer will be the strings specified
///     in the .inf file. After a device is opened the actual device descriptor can be read as well.
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public sealed class USBDeviceInfo
{
    private readonly DeviceDetails _details;

    internal USBDeviceInfo(DeviceDetails details)
    {
        _details = details;
    }

    /// <summary>
    ///     Vendor ID (VID) of the USB device
    /// </summary>
    public int VID => _details.VID;

    /// <summary>
    ///     Product ID (PID) of the USB device
    /// </summary>
    public int PID => _details.PID;

    /// <summary>
    ///     Manufacturer of the device, as specified in the INF file (not the device descriptor)
    /// </summary>
    public string Manufacturer => _details.Manufacturer;

    /// <summary>
    ///     Description of the device, as specified in the INF file (not the device descriptor)
    /// </summary>
    public string DeviceDescription => _details.DeviceDescription;

    /// <summary>
    ///     Device pathname
    /// </summary>
    public string DevicePath => _details.DevicePath;

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{Manufacturer} - {DeviceDescription} (VID: {VID:X4}, PID: {PID:X4})";
    }
}
