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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Threading;

using Windows.Win32;
using Windows.Win32.Devices.Usb;
using Windows.Win32.Foundation;

using Microsoft.Win32.SafeHandles;

using Nefarius.Drivers.WinUSB.Util;

namespace Nefarius.Drivers.WinUSB.API;

/// <summary>
///     Wrapper for a WinUSB device dealing with the WinUSB and additional interface handles
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
internal partial class WinUSBDevice : IDisposable
{
    private WINUSB_INTERFACE_HANDLE[] _addInterfaces;
    private SafeFileHandle _deviceHandle;
    private bool _disposed;
    private SafeHandle _winUsbHandle;

    public int InterfaceCount => 1 + (_addInterfaces?.Length ?? 0);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~WinUSBDevice()
    {
        Dispose(false);
    }

    private void CheckNotDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException("USB device object has been disposed.");
        }
    }

    // TODO: check if not disposed on methods (although this is already checked by USBDevice)

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            // Dispose managed resources
            if (_deviceHandle is { IsInvalid: false })
            {
                _deviceHandle.Dispose();
            }

            _deviceHandle = null;
        }

        // Dispose unmanaged resources
        FreeWinUSB();
        _disposed = true;
    }

    private void FreeWinUSB()
    {
        if (_addInterfaces != null)
        {
            foreach (WINUSB_INTERFACE_HANDLE entry in _addInterfaces)
            {
                PInvoke.WinUsb_Free(entry);
            }

            _addInterfaces = null;
        }

        if (_winUsbHandle is { IsInvalid: false })
        {
            PInvoke.WinUsb_Free(_winUsbHandle.AsInterfaceHandle());
        }

        _winUsbHandle = null;
    }

    private unsafe int ReadStringDescriptor(byte index, ushort languageId, byte[] buffer)
    {
        fixed (byte* bufferPtr = buffer)
        {
            uint transferred = 0;
            BOOL success = PInvoke.WinUsb_GetDescriptor(
                _winUsbHandle.AsInterfaceHandle(),
                (byte)PInvoke.USB_STRING_DESCRIPTOR_TYPE,
                index,
                languageId,
                bufferPtr,
                (uint)buffer.Length,
                &transferred
            );

            if (!success)
            {
                throw APIException.Win32("Failed to get USB string descriptor (" + index + ").");
            }

            if (transferred == 0)
            {
                throw new APIException("No data returned when reading USB descriptor.");
            }

            int length = buffer[0];

            if (length != transferred)
            {
                throw new APIException("Unexpected length when reading USB descriptor.");
            }

            return length;
        }
    }

    private WINUSB_INTERFACE_HANDLE InterfaceHandle(int index)
    {
        return index == 0 ? _winUsbHandle.AsInterfaceHandle() : _addInterfaces[index - 1];
    }

    public unsafe void GetInterfaceInfo(int interfaceIndex, out USB_INTERFACE_DESCRIPTOR descriptor,
        out WINUSB_PIPE_INFORMATION[] pipes)
    {
        USB_INTERFACE_DESCRIPTOR desc;
        List<WINUSB_PIPE_INFORMATION> pipeList = new();
        BOOL success = PInvoke.WinUsb_QueryInterfaceSettings(InterfaceHandle(interfaceIndex), 0, &desc);
        if (!success)
        {
            throw APIException.Win32("Failed to get WinUSB device interface descriptor.");
        }

        descriptor = desc;

        WINUSB_INTERFACE_HANDLE interfaceHandle = InterfaceHandle(interfaceIndex);
        for (byte pipeIdx = 0; pipeIdx < descriptor.bNumEndpoints; pipeIdx++)
        {
            WINUSB_PIPE_INFORMATION pipeInfo;
            success = PInvoke.WinUsb_QueryPipe(interfaceHandle, 0, pipeIdx, &pipeInfo);

            pipeList.Add(pipeInfo);
            if (!success)
            {
                throw APIException.Win32("Failed to get WinUSB device pipe information.");
            }
        }

        pipes = pipeList.ToArray();
    }

    private unsafe void InitializeDevice()
    {
        WINUSB_INTERFACE_HANDLE handle;
        BOOL success = PInvoke.WinUsb_Initialize(new HANDLE(_deviceHandle.DangerousGetHandle()), &handle);

        if (!success)
        {
            throw APIException.Win32("Failed to initialize WinUSB handle. Device might not be connected.");
        }

        _winUsbHandle = new SafeFileHandle(new IntPtr(handle.Value), false);

        List<WINUSB_INTERFACE_HANDLE> interfaces = new();
        byte idx = 0;

        try
        {
            while (true)
            {
                WINUSB_INTERFACE_HANDLE ifaceHandle;
                success = PInvoke.WinUsb_GetAssociatedInterface(_winUsbHandle.AsInterfaceHandle(), idx, &ifaceHandle);
                if (!success)
                {
                    if (Marshal.GetLastWin32Error() == (int)WIN32_ERROR.ERROR_NO_MORE_ITEMS)
                    {
                        break;
                    }

                    throw APIException.Win32("Failed to enumerate interfaces for WinUSB device.");
                }

                interfaces.Add(ifaceHandle);
                idx++;
            }
        }
        finally
        {
            // Save interface handles (will be cleaned by Dispose)
            // also in case of exception (which is why it is in finally block),
            // because some handles might have already been opened and need
            // to be disposed.
            _addInterfaces = interfaces.ToArray();
        }

        // TODO: IDK why but this causes a hang when debugging, deadlock?
        if (!Debugger.IsAttached)
        {
            // Bind handle (needed for overlapped I/O thread pool)
            ThreadPool.BindHandle(_deviceHandle);
        }
        // TODO: bind interface handles as well? doesn't seem to be necessary
    }

    private static unsafe void HandleOverlappedApi(bool success, string errorMessage, NativeOverlapped* pOverlapped,
        USBAsyncResult result, int bytesTransferred)
    {
        if (!success)
        {
            if (Marshal.GetLastWin32Error() != (int)WIN32_ERROR.ERROR_IO_PENDING)
            {
                Overlapped.Unpack(pOverlapped);
                Overlapped.Free(pOverlapped);
                throw APIException.Win32(errorMessage);
            }
        }
        else
        {
            // Immediate success!
            Overlapped.Unpack(pOverlapped);
            Overlapped.Free(pOverlapped);

            result.OnCompletion(true, null, bytesTransferred, false);
            // is the callback still called in this case?? todo
        }
    }

    private static unsafe void PipeIoCallback(uint errorCode, uint numBytes, NativeOverlapped* pOverlapped)
    {
        try
        {
            Exception error = null;
            if (errorCode != 0)
            {
                error = APIException.Win32("Asynchronous operation on WinUSB device failed.", (int)errorCode);
            }

            Overlapped overlapped = Overlapped.Unpack(pOverlapped);
            USBAsyncResult result = (USBAsyncResult)overlapped.AsyncResult;
            Overlapped.Free(pOverlapped);
            pOverlapped = null;

            result.OnCompletion(false, error, (int)numBytes, true);
        }
        finally
        {
            if (pOverlapped != null)
            {
                Overlapped.Unpack(pOverlapped);
                Overlapped.Free(pOverlapped);
            }
        }
    }
}