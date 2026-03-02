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
///     Represents a single USB interface from a USB device
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public sealed class USBInterface
{
    internal USBInterface(USBDevice device, int interfaceIndex, USB_INTERFACE_DESCRIPTOR rawDesc,
        USBPipeCollection pipes)
    {
        // Set raw class identifiers
        ClassValue = rawDesc.bInterfaceClass;
        SubClass = rawDesc.bInterfaceSubClass;
        Protocol = rawDesc.bInterfaceProtocol;

        Number = rawDesc.bInterfaceNumber;
        InterfaceIndex = interfaceIndex;

        // If interface class is of a known type (USBBaseClass enum), use this
        // for the InterfaceClass property.
        BaseClass = USBBaseClass.Unknown;
        if (Enum.IsDefined(typeof(USBBaseClass), (int)rawDesc.bInterfaceClass))
            BaseClass = (USBBaseClass)rawDesc.bInterfaceClass;


        Device = device;
        Pipes = pipes;

        // Handle pipes
        foreach (var pipe in pipes)
        {
            // Attach pipe to this interface
            pipe.AttachInterface(this);

            // If first in or out pipe, set InPipe and OutPipe
            if (pipe.IsIn && InPipe == null)
                InPipe = pipe;
            if (pipe.IsOut && OutPipe == null)
                OutPipe = pipe;
        }
    }

    /// <summary>
    ///     Collection of pipes associated with this interface
    /// </summary>
    public USBPipeCollection Pipes { get; }

    /// <summary>
    ///     Interface number from the interface descriptor
    /// </summary>
    public int Number { get; }

    /// <summary>
    ///     USB device associated with this interface
    /// </summary>
    public USBDevice Device { get; }

    /// <summary>
    ///     First IN direction pipe on this interface
    /// </summary>
    public USBPipe InPipe { get; }

    /// <summary>
    ///     First OUT direction pipe on this interface
    /// </summary>
    public USBPipe OutPipe { get; }

    /// <summary>
    ///     Interface class code. If the interface class does
    ///     not match any of the USBBaseClass enumeration values
    ///     the value will be USBBaseClass.Unknown
    /// </summary>
    public USBBaseClass BaseClass { get; }

    /// <summary>
    ///     Interface class code as defined in the interface descriptor
    ///     This property can be used if the class type is not defined
    ///     int the USBBaseClass enumeration
    /// </summary>
    public byte ClassValue { get; }

    /// <summary>
    ///     Interface subclass code
    /// </summary>
    public byte SubClass { get; }

    /// <summary>
    ///     Interface protocol code
    /// </summary>
    public byte Protocol { get; }

    /// <summary>
    ///     Interface alternate setting
    /// </summary>
    public byte AlternateSetting
    {
        get => Device.InternalDevice.GetCurrentAlternateSetting(InterfaceIndex);
        set => Device.InternalDevice.SetCurrentAlternateSetting(InterfaceIndex, value);
    }

    /// Zero based interface index in WinUSB.
    /// Note that this is not necessarily the same as the interface *number*
    /// from the interface descriptor. There might be interfaces within the
    /// USB device that do not use WinUSB, these are not counted for index.
    internal int InterfaceIndex { get; }
}
