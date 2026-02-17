/*  WinUSBNet library
 *  (C) 2010 Thomas Bleeker (www.madwizard.org)
 *
 *  Licensed under the MIT license, see license.txt or:
 *  http://www.opensource.org/licenses/mit-license.php
 */

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Windows.Win32.Devices.Usb;

using Nefarius.Drivers.WinUSB.API;

[assembly: CLSCompliant(true)]

namespace Nefarius.Drivers.WinUSB;

/// <summary>
///     The UsbDevice class represents a single WinUSB device.
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public partial class USBDevice : IDisposable
{
    private bool _disposed;

    internal WinUSBDevice InternalDevice { get; }

    /// <summary>
    ///     Finalizer for the UsbDevice. Disposes all unmanaged handles.
    /// </summary>
    ~USBDevice()
    {
        Dispose(false);
    }

    /// <summary>
    ///     Disposes the object
    /// </summary>
    /// <param name="disposing">
    ///     Indicates whether Dispose was called manually (true) or by
    ///     the garbage collector (false) via the destructor.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            InternalDevice?.Dispose();
        }

        // Clean unmanaged resources here.
        // (none currently)

        _disposed = true;
    }

    private void InitializeInterfaces()
    {
        int numInterfaces = InternalDevice.InterfaceCount;

        List<USBPipe> allPipes = new();

        USBInterface[] interfaces = new USBInterface[numInterfaces];
        // UsbEndpoint
        for (int i = 0; i < numInterfaces; i++)
        {
            InternalDevice.GetInterfaceInfo(
                i,
                out USB_INTERFACE_DESCRIPTOR descriptor,
                out WINUSB_PIPE_INFORMATION[] pipesInfo
            );
            USBPipe[] interfacePipes = new USBPipe[pipesInfo.Length];
            for (int k = 0; k < pipesInfo.Length; k++)
            {
                USBPipe pipe = new(this, pipesInfo[k]);
                interfacePipes[k] = pipe;
                allPipes.Add(pipe);
            }

            // TODO:
            //if (descriptor.iInterface != 0)
            //    _wuDevice.GetStringDescriptor(descriptor.iInterface);
            USBPipeCollection pipeCollection = new(interfacePipes);
            interfaces[i] = new USBInterface(this, i, descriptor, pipeCollection);
        }

        Pipes = new USBPipeCollection(allPipes.ToArray());
        Interfaces = new USBInterfaceCollection(interfaces);
    }

    private static void CheckControlParams(int value, int index, Span<byte> buffer, int length)
    {
        if (value is < ushort.MinValue or > ushort.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Value parameter out of range.");
        }

        if (index is < ushort.MinValue or > ushort.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Index parameter out of range.");
        }

        if (length > buffer.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(length),
                "Length parameter is larger than the size of the buffer.");
        }

        if (length > ushort.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(length), "Length too large");
        }
    }

    private static void CheckIn(byte requestType)
    {
        if ((requestType & 0x80) == 0) // Host to device?
        {
            throw new ArgumentException("Request type is not IN.");
        }
    }

    private static void CheckOut(byte requestType)
    {
        if ((requestType & 0x80) == 0x80) // Device to host?
        {
            throw new ArgumentException("Request type is not OUT.");
        }
    }

    private void CheckNotDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException("USB device object has been disposed.");
        }
    }

    private static USBDeviceDescriptor GetDeviceDescriptor(string devicePath)
    {
        try
        {
            using WinUSBDevice wuDevice = new();
            wuDevice.OpenDevice(devicePath);
            USB_DEVICE_DESCRIPTOR deviceDesc = wuDevice.GetDeviceDescriptor();

            // Get first supported language ID
            ushort[] langIDs = wuDevice.GetSupportedLanguageIDs();
            ushort langID = 0;
            if (langIDs.Length > 0)
            {
                langID = langIDs[0];
            }

            string manufacturer = null, product = null, serialNumber = null;
            byte idx = 0;
            idx = deviceDesc.iManufacturer;
            if (idx > 0)
            {
                manufacturer = wuDevice.GetStringDescriptor(idx, langID);
            }

            idx = deviceDesc.iProduct;
            if (idx > 0)
            {
                product = wuDevice.GetStringDescriptor(idx, langID);
            }

            idx = deviceDesc.iSerialNumber;
            if (idx > 0)
            {
                serialNumber = wuDevice.GetStringDescriptor(idx, langID);
            }

            USBDeviceDescriptor descriptor = new(devicePath, deviceDesc, manufacturer, product, serialNumber);

            return descriptor;
        }
        catch (APIException e)
        {
            throw new USBException("Failed to retrieve device descriptor.", e);
        }
    }
}