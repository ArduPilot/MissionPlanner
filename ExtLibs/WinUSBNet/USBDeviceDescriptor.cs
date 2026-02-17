/*  WinUSBNet library
 *  (C) 2010 Thomas Bleeker (www.madwizard.org)
 *
 *  Licensed under the MIT license, see license.txt or:
 *  http://www.opensource.org/licenses/mit-license.php
 */

using System;
using System.Diagnostics.CodeAnalysis;

using Windows.Win32.Devices.Usb;

namespace Nefarius.Drivers.WinUSB;

/// <summary>
///     USB device details
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public sealed class USBDeviceDescriptor
{
    internal USBDeviceDescriptor(string path, USB_DEVICE_DESCRIPTOR deviceDesc, string manufacturer, string product,
        string serialNumber)
    {
        PathName = path;
        VID = deviceDesc.idVendor;
        PID = deviceDesc.idProduct;
        Manufacturer = manufacturer;
        Product = product;
        SerialNumber = serialNumber;


        ClassValue = deviceDesc.bDeviceClass;
        SubClass = deviceDesc.bDeviceSubClass;
        Protocol = deviceDesc.bDeviceProtocol;

        // If interface class is of a known type (USBBaseeClass enum), use this
        // for the InterfaceClass property.
        BaseClass = USBBaseClass.Unknown;
        if (Enum.IsDefined(typeof(USBBaseClass), (int)deviceDesc.bDeviceClass))
            BaseClass = (USBBaseClass)deviceDesc.bDeviceClass;
    }

    /// <summary>
    ///     Windows path name for the USB device
    /// </summary>
    public string PathName { get; }

    /// <summary>
    ///     USB vendor ID (VID) of the device
    /// </summary>
    public int VID { get; }

    /// <summary>
    ///     USB product ID (PID) of the device
    /// </summary>
    public int PID { get; }

    /// <summary>
    ///     Manufacturer name, or null if not available
    /// </summary>
    public string Manufacturer { get; }

    /// <summary>
    ///     Product name, or null if not available
    /// </summary>
    public string Product { get; }

    /// <summary>
    ///     Device serial number, or null if not available
    /// </summary>
    public string SerialNumber { get; }


    /// <summary>
    ///     Friendly device name, or path name when no
    ///     further device information is available
    /// </summary>
    public string FullName
    {
        get
        {
            if (Manufacturer != null && Product != null)
                return Product + " - " + Manufacturer;
            if (Product != null)
                return Product;
            if (SerialNumber != null)
                return SerialNumber;
            return PathName;
        }
    }

    /// <summary>
    ///     Device class code as defined in the interface descriptor
    ///     This property can be used if the class type is not defined
    ///     int the USBBaseClass enumeration
    /// </summary>
    public byte ClassValue { get; }

    /// <summary>
    ///     Device subclass code
    /// </summary>
    public byte SubClass { get; }

    /// <summary>
    ///     Device protocol code
    /// </summary>
    public byte Protocol { get; }

    /// <summary>
    ///     Device class code. If the device class does
    ///     not match any of the USBBaseClass enumeration values
    ///     the value will be USBBaseClass.Unknown
    /// </summary>
    public USBBaseClass BaseClass { get; }

    /// <inheritdoc />
    public override string ToString()
    {
        return FullName;
    }
}
