/*  WinUSBNet library
 *  (C) 2010 Thomas Bleeker (www.madwizard.org)
 *
 *  Licensed under the MIT license, see license.txt or:
 *  http://www.opensource.org/licenses/mit-license.php
 */

using System;
using System.Diagnostics.CodeAnalysis;

using Windows.Win32.Devices.Usb;

using Nefarius.Drivers.WinUSB.API;

namespace Nefarius.Drivers.WinUSB;

/// <summary>
///     Describes the power policy for a USB device
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public sealed class USBPowerPolicy
{
    private readonly USBDevice _device;

    internal USBPowerPolicy(USBDevice device)
    {
        _device = device;
    }

    /// <summary>
    ///     When true, the device is auto-suspended when either no transfers are pending, or only In transfers on an
    ///     interrupt or bulk endpoint are pending.
    ///     Default value is determined by the DefaultIdleState registry value.
    /// </summary>
    /// <seealso href="https://docs.microsoft.com/windows/win32/api/winusb/nf-winusb-winusb_setpowerpolicy">
    ///     WinUSB_GetPowerPolicy for a more detailed description
    /// </seealso>
    public bool AutoSuspend
    {
        get => _device.InternalDevice.GetPowerPolicyBool(WINUSB_POWER_POLICY.AUTO_SUSPEND);
        set => _device.InternalDevice.SetPowerPolicy(WINUSB_POWER_POLICY.AUTO_SUSPEND, value);
    }

    /// <summary>
    ///     The minimum amount of milliseconds that must pass before the device can be suspended.
    /// </summary>
    /// <seealso href="https://docs.microsoft.com/windows/win32/api/winusb/nf-winusb-winusb_setpowerpolicy">
    ///     WinUSB_GetPowerPolicy for a more detailed description
    /// </seealso>
    public int SuspendDelay
    {
        get => (int)_device.InternalDevice.GetPowerPolicyUInt(WINUSB_POWER_POLICY.SUSPEND_DELAY);
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Suspend delay cannot be negative.");
            _device.InternalDevice.SetPowerPolicy(WINUSB_POWER_POLICY.SUSPEND_DELAY, (uint)value);
        }
    }
}
