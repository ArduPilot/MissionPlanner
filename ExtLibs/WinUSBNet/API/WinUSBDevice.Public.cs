using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Windows.Win32;
using Windows.Win32.Devices.Usb;
using Windows.Win32.Storage.FileSystem;

using Nefarius.Drivers.WinUSB.Util;

namespace Nefarius.Drivers.WinUSB.API;

internal partial class WinUSBDevice
{
    public unsafe USB_DEVICE_DESCRIPTOR GetDeviceDescriptor()
    {
        USB_DEVICE_DESCRIPTOR deviceDesc;
        var size = (uint)Marshal.SizeOf(typeof(USB_DEVICE_DESCRIPTOR));

        uint transferred = 0;
        var success = PInvoke.WinUsb_GetDescriptor(
            _winUsbHandle.AsInterfaceHandle(),
            (byte)PInvoke.USB_DEVICE_DESCRIPTOR_TYPE,
            0,
            0,
            (byte*)&deviceDesc,
            size,
            &transferred
        );

        if (!success)
            throw APIException.Win32("Failed to get USB device descriptor.");

        if (transferred != size)
            throw APIException.Win32("Incomplete USB device descriptor.");

        return deviceDesc;
    }

    public ushort[] GetSupportedLanguageIDs()
    {
        var buffer = new byte[256];
        var length = ReadStringDescriptor(0, 0, buffer);
        length -= 2; // Skip length byte and descriptor type
        if (length < 0 || length % 2 != 0)
            throw new APIException("Unexpected length when reading supported languages.");

        var langIDs = new ushort[length / 2];
        Buffer.BlockCopy(buffer, 2, langIDs, 0, length);
        return langIDs;
    }

    // CLS-compliant version of the above
    public unsafe int[] GetSupportedLanguageIDs_CLS()
    {
        var buffer = new byte[256];
        var length = ReadStringDescriptor(0, 0, buffer);
        length -= 2; // Skip length byte and descriptor type
        if (length < 0 || length % 2 != 0)
            throw new APIException("Unexpected length when reading supported languages.");

        var langIDs = new int[length / 2];
        fixed (byte* ptr = buffer)
        {
            var ids = (ushort*)(ptr + 2);
            for (int i = 0; i < langIDs.Length; i++)
            {
                langIDs[i] = ids[i];
            }
        }

        Buffer.BlockCopy(buffer, 2, langIDs, 0, length);
        return langIDs;
    }

    public string GetStringDescriptor(byte index, ushort languageId)
    {
        var buffer = new byte[256];
        var length = ReadStringDescriptor(index, languageId, buffer);
        length -= 2; // Skip length byte and descriptor type
        if (length < 0)
            return null;
        var chars = Encoding.Unicode.GetChars(buffer, 2, length);
        return new string(chars);
    }

    public unsafe int ControlTransfer(byte requestType, byte request, ushort value, ushort index, ushort length,
        Span<byte> data)
    {
        uint bytesReturned = 0;
        WINUSB_SETUP_PACKET setupPacket;

        setupPacket.RequestType = requestType;
        setupPacket.Request = request;
        setupPacket.Value = value;
        setupPacket.Index = index;
        setupPacket.Length = length;

        fixed (byte* dataPtr = data)
        {
            var success = PInvoke.WinUsb_ControlTransfer(_winUsbHandle.AsInterfaceHandle(), setupPacket, dataPtr, length,
                &bytesReturned);
            if (!success) // todo check bytes returned?
                throw APIException.Win32("Control transfer on WinUSB device failed.");
            return (int)bytesReturned;
        }
    }

    // ReSharper disable once RedundantUnsafeContext
    public unsafe void OpenDevice(string devicePathName)
    {
        try
        {
            _deviceHandle = PInvoke.CreateFile(
                devicePathName,
                (uint)(FILE_ACCESS_RIGHTS.FILE_GENERIC_READ | FILE_ACCESS_RIGHTS.FILE_GENERIC_WRITE),
                FILE_SHARE_MODE.FILE_SHARE_READ | FILE_SHARE_MODE.FILE_SHARE_WRITE,
                null,
                FILE_CREATION_DISPOSITION.OPEN_EXISTING,
                FILE_FLAGS_AND_ATTRIBUTES.FILE_ATTRIBUTE_NORMAL
                | FILE_FLAGS_AND_ATTRIBUTES.FILE_FLAG_OVERLAPPED,
                null
            );

            if (_deviceHandle.IsInvalid)
                throw APIException.Win32("Failed to open WinUSB device handle.");
            InitializeDevice();
        }
        catch (Exception)
        {
            if (_deviceHandle != null)
            {
                _deviceHandle.Dispose();
                _deviceHandle = null;
            }

            FreeWinUSB();
            throw;
        }
    }

    public unsafe void ReadPipe(int interfaceIndex, byte pipeId, Span<byte> buffer, int offset, int bytesToRead,
        out uint bytesRead)
    {
        bool success;
        uint lengthTransferred = 0;
        fixed (byte* pBuffer = buffer)
        {
            success = PInvoke.WinUsb_ReadPipe(InterfaceHandle(interfaceIndex), pipeId, pBuffer + offset,
                (uint)bytesToRead,
                &lengthTransferred);

            bytesRead = lengthTransferred;
        }

        if (!success)
            throw APIException.Win32("Failed to read pipe on WinUSB device.");
    }

    public unsafe void ReadPipeOverlapped(int interfaceIndex, byte pipeId, byte[] buffer, int offset, int bytesToRead,
        USBAsyncResult result)
    {
        // TODO: heap allocation every call?
        var overlapped = new Overlapped
        {
            AsyncResult = result
        };

        NativeOverlapped* pOverlapped = null;
        uint bytesRead = 0;

        pOverlapped = overlapped.Pack(PipeIoCallback, buffer);
        bool success;
        // Buffer is pinned already by overlapped.Pack
        fixed (byte* pBuffer = buffer)
        {
            success = PInvoke.WinUsb_ReadPipe(InterfaceHandle(interfaceIndex), pipeId, pBuffer + offset,
                (uint)bytesToRead,
                &bytesRead, pOverlapped);
        }

        HandleOverlappedApi(success, "Failed to asynchronously read pipe on WinUSB device.", pOverlapped, result,
            (int)bytesRead);
    }

    public unsafe void WriteOverlapped(int interfaceIndex, byte pipeId, byte[] buffer, int offset, int bytesToWrite,
        USBAsyncResult result)
    {
        // TODO: heap allocation every call?
        var overlapped = new Overlapped
        {
            AsyncResult = result
        };

        NativeOverlapped* pOverlapped = null;

        uint bytesWritten;
        pOverlapped = overlapped.Pack(PipeIoCallback, buffer);

        bool success;
        // Buffer is pinned already by overlapped.Pack
        fixed (byte* pBuffer = buffer)
        {
            success = PInvoke.WinUsb_WritePipe(InterfaceHandle(interfaceIndex), pipeId, pBuffer + offset,
                (uint)bytesToWrite,
                &bytesWritten, pOverlapped);
        }

        HandleOverlappedApi(success, "Failed to asynchronously write pipe on WinUSB device.", pOverlapped, result,
            (int)bytesWritten);
    }

    public unsafe void ControlTransferOverlapped(byte requestType, byte request, ushort value, ushort index,
        ushort length,
        byte[] data, USBAsyncResult result)
    {
        uint bytesReturned = 0;
        WINUSB_SETUP_PACKET setupPacket;

        setupPacket.RequestType = requestType;
        setupPacket.Request = request;
        setupPacket.Value = value;
        setupPacket.Index = index;
        setupPacket.Length = length;

        // TODO: heap allocation every call?
        var overlapped = new Overlapped
        {
            AsyncResult = result
        };

        fixed (byte* pBuffer = data)
        {
            NativeOverlapped* pOverlapped = null;
            pOverlapped = overlapped.Pack(PipeIoCallback, data);
            var success =
                PInvoke.WinUsb_ControlTransfer(_winUsbHandle.AsInterfaceHandle(), setupPacket, pBuffer, length, &bytesReturned,
                    pOverlapped);
            HandleOverlappedApi(success, "Asynchronous control transfer on WinUSB device failed.", pOverlapped, result,
                (int)bytesReturned);
        }
    }

    public unsafe void AbortPipe(int interfaceIndex, byte pipeId)
    {
        var success = PInvoke.WinUsb_AbortPipe(InterfaceHandle(interfaceIndex), pipeId);
        if (!success)
            throw APIException.Win32("Failed to abort pipe on WinUSB device.");
    }

    public unsafe void WritePipe(int interfaceIndex, byte pipeId, ReadOnlySpan<byte> buffer, int offset, int length)
    {
        uint bytesWritten;
        bool success;

        fixed (byte* pBuffer = buffer)
        {
            success = PInvoke.WinUsb_WritePipe(InterfaceHandle(interfaceIndex), pipeId,
                pBuffer + offset, (uint)length,
                &bytesWritten);
        }

        if (!success || bytesWritten != length)
            throw APIException.Win32("Failed to write pipe on WinUSB device.");
    }

    public unsafe void FlushPipe(int interfaceIndex, byte pipeId)
    {
        var success = PInvoke.WinUsb_FlushPipe(InterfaceHandle(interfaceIndex), pipeId);
        if (!success)
            throw APIException.Win32("Failed to flush pipe on WinUSB device.");
    }

    public unsafe void ResetPipe(int interfaceIndex, byte pipeId)
    {
        var success = PInvoke.WinUsb_ResetPipe(InterfaceHandle(interfaceIndex), pipeId);
        if (!success)
            throw APIException.Win32("Failed to reset pipe on WinUSB device.");
    }

    public unsafe void SetPipePolicy(int interfaceIndex, byte pipeId, WINUSB_PIPE_POLICY policyType, bool value)
    {
        var byteVal = (byte)(value ? 1 : 0);
        var success = PInvoke.WinUsb_SetPipePolicy(InterfaceHandle(interfaceIndex), pipeId,
            policyType, 1, &byteVal);
        if (!success)
            throw APIException.Win32("Failed to set WinUSB pipe policy.");
    }

    public unsafe void SetPipePolicy(int interfaceIndex, byte pipeId, WINUSB_PIPE_POLICY policyType, uint value)
    {
        var success = PInvoke.WinUsb_SetPipePolicy(InterfaceHandle(interfaceIndex), pipeId,
            policyType, 4, &value);

        if (!success)
            throw APIException.Win32("Failed to set WinUSB pipe policy.");
    }

    public unsafe bool GetPipePolicyBool(int interfaceIndex, byte pipeId, WINUSB_PIPE_POLICY policyType)
    {
        byte result;
        uint length = 1;

        var success =
            PInvoke.WinUsb_GetPipePolicy(InterfaceHandle(interfaceIndex), pipeId, policyType,
                &length, &result);
        if (!success || length != 1)
            throw APIException.Win32("Failed to get WinUSB pipe policy.");
        return result != 0;
    }

    public unsafe uint GetPipePolicyUInt(int interfaceIndex, byte pipeId, WINUSB_PIPE_POLICY policyType)
    {
        uint result;
        uint length = 4;
        var success =
            PInvoke.WinUsb_GetPipePolicy(InterfaceHandle(interfaceIndex), pipeId, policyType,
                &length, &result);

        if (!success || length != 4)
            throw APIException.Win32("Failed to get WinUSB pipe policy.");
        return result;
    }

    public unsafe void SetPowerPolicy(WINUSB_POWER_POLICY policyType, bool value)
    {
        var byteVal = (byte)(value ? 1 : 0);
        var success = PInvoke.WinUsb_SetPowerPolicy(_winUsbHandle.AsInterfaceHandle(), policyType, 1, &byteVal);
        if (!success)
            throw APIException.Win32("Failed to set WinUSB power policy.");
    }

    public unsafe void SetPowerPolicy(WINUSB_POWER_POLICY policyType, uint value)
    {
        var success = PInvoke.WinUsb_SetPowerPolicy(_winUsbHandle.AsInterfaceHandle(), policyType, 4, &value);
        if (!success)
            throw APIException.Win32("Failed to set WinUSB power policy.");
    }

    public unsafe bool GetPowerPolicyBool(WINUSB_POWER_POLICY policyType)
    {
        byte result;
        uint length = 1;

        var success = PInvoke.WinUsb_GetPowerPolicy(_winUsbHandle.AsInterfaceHandle(), policyType, &length, &result);
        if (!success || length != 1)
            throw APIException.Win32("Failed to get WinUSB power policy.");
        return result != 0;
    }

    public unsafe uint GetPowerPolicyUInt(WINUSB_POWER_POLICY policyType)
    {
        uint result;
        uint length = 4;

        var success = PInvoke.WinUsb_GetPowerPolicy(_winUsbHandle.AsInterfaceHandle(), policyType, &length, &result);
        if (!success || length != 4)
            throw APIException.Win32("Failed to get WinUSB power policy.");
        return result;
    }

    public unsafe byte GetCurrentAlternateSetting(int interfaceIndex)
    {
        byte result;
        var success = PInvoke.WinUsb_GetCurrentAlternateSetting(InterfaceHandle(interfaceIndex), &result);
        if (!success)
            throw APIException.Win32("Failed to get WinUSB alternate setting.");
        return result;
    }

    public unsafe void SetCurrentAlternateSetting(int interfaceIndex, byte setting)
    {
        var success = PInvoke.WinUsb_SetCurrentAlternateSetting(InterfaceHandle(interfaceIndex), setting);
        if (!success)
            throw APIException.Win32("Failed to set WinUSB alternate setting.");
    }
}
