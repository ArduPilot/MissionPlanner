using System;
using System.Diagnostics.CodeAnalysis;

using Windows.Win32.Devices.Usb;

using Nefarius.Drivers.WinUSB.API;

namespace Nefarius.Drivers.WinUSB;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global")]
public partial class USBDevice
{
    /// <summary>
    ///     Constructs a new USB device
    /// </summary>
    /// <param name="deviceInfo">USB device info of the device to create</param>
    public USBDevice(USBDeviceInfo deviceInfo)
        : this(deviceInfo.DevicePath)
    {
        // Handled in other constructor
    }

    /// <summary>
    ///     Constructs a new USB device
    /// </summary>
    /// <param name="devicePathName">Device path name of the USB device to create</param>
    public USBDevice(string devicePathName)
    {
        Descriptor = GetDeviceDescriptor(devicePathName);
        InternalDevice = new WinUSBDevice();
        try
        {
            InternalDevice.OpenDevice(devicePathName);
            InitializeInterfaces();
            PowerPolicy = new USBPowerPolicy(this);
        }
        catch (APIException e)
        {
            InternalDevice.Dispose();
            throw new USBException("Failed to open device.", e);
        }
    }

    /// <summary>
    ///     Collection of all pipes available on the USB device
    /// </summary>
    public USBPipeCollection Pipes { get; private set; }

    /// <summary>
    ///     Collection of all interfaces available on the USB device
    /// </summary>
    public USBInterfaceCollection Interfaces { get; private set; }

    /// <summary>
    ///     Device descriptor with information about the device
    /// </summary>
    public USBDeviceDescriptor Descriptor { get; }

    /// <summary>
    ///     The power policy settings for this device
    /// </summary>
    public USBPowerPolicy PowerPolicy { get; }

    /// <summary>
    ///     Specifies the timeout in milliseconds for control pipe operations. If a control transfer does not finish within the
    ///     specified time it will fail.
    ///     When set to zero, no timeout is used. Default value is 5000 milliseconds.
    /// </summary>
    /// <seealso href="http://msdn.microsoft.com/en-us/library/aa476439.aspx">
    ///     WinUSB_GetPipePolicy for a more detailed
    ///     description
    /// </seealso>
    public int ControlPipeTimeout
    {
        get => (int)InternalDevice.GetPipePolicyUInt(0, 0x00, WINUSB_PIPE_POLICY.PIPE_TRANSFER_TIMEOUT);
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Control pipe timeout cannot be negative.");
            }

            InternalDevice.SetPipePolicy(0, 0x00, WINUSB_PIPE_POLICY.PIPE_TRANSFER_TIMEOUT, (uint)value);
        }
    }

    /// <summary>
    ///     Disposes the UsbDevice including all unmanaged WinUSB handles. This function
    ///     should be called when the UsbDevice object is no longer in use, otherwise
    ///     unmanaged handles will remain open until the garbage collector finalizes the
    ///     object.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Initiates a control transfer over the default control endpoint. This method allows both IN and OUT direction
    ///     transfers, depending
    ///     on the highest bit of the <paramref name="requestType" /> parameter. Alternatively,
    ///     <see cref="ControlIn(byte,byte,int,int,Span{byte},int)" /> and
    ///     <see cref="ControlOut(byte,byte,int,int,Span{byte},int)" /> can be used for control transfers in a specific
    ///     direction,
    ///     which is the recommended way because
    ///     it prevents using the wrong direction accidentally. Use the ControlTransfer method when the direction is not known
    ///     at compile time.
    /// </summary>
    /// <param name="requestType">The setup packet request type.</param>
    /// <param name="request">The setup packet device request.</param>
    /// <param name="value">
    ///     The value member in the setup packet. Its meaning depends on the request. Value should be between
    ///     zero and 65535 (0xFFFF).
    /// </param>
    /// <param name="index">
    ///     The index member in the setup packet. Its meaning depends on the request. Index should be between
    ///     zero and 65535 (0xFFFF).
    /// </param>
    /// <param name="buffer">
    ///     The data to transfer in the data stage of the control. When the transfer is in the IN direction the data received
    ///     will be
    ///     written to this buffer. For an OUT direction transfer the contents of the buffer are written sent through the pipe.
    /// </param>
    /// <param name="length">
    ///     Length of the data to transfer. Must be equal to or less than the length of <paramref name="buffer" />.
    ///     The setup packet's length member will be set to this length.
    /// </param>
    /// <returns>The number of bytes received from the device.</returns>
    public int ControlTransfer(byte requestType, byte request, int value, int index, Span<byte> buffer, int length)
    {
        // Parameters are int and not ushort because ushort is not CLS compliant.
        CheckNotDisposed();
        CheckControlParams(value, index, buffer, length);

        try
        {
            return InternalDevice.ControlTransfer(requestType, request, (ushort)value, (ushort)index, (ushort)length,
                buffer);
        }
        catch (APIException e)
        {
            throw new USBException("Control transfer failed", e);
        }
    }

    /// <summary>
    ///     Initiates an asynchronous control transfer over the default control endpoint. This method allows both IN and OUT
    ///     direction transfers, depending
    ///     on the highest bit of the <paramref name="requestType" /> parameter. Alternatively,
    ///     <see cref="BeginControlIn(byte,byte,int,int,byte[],int,AsyncCallback,object)" /> and
    ///     <see cref="BeginControlIn(byte,byte,int,int,byte[],int,AsyncCallback,object)" /> can be used for asynchronous
    ///     control transfers in a specific direction, which is
    ///     the recommended way because it prevents using the wrong direction accidentally. Use the BeginControlTransfer method
    ///     when the direction is not
    ///     known at compile time.
    /// </summary>
    /// <param name="requestType">The setup packet request type.</param>
    /// <param name="request">The setup packet device request.</param>
    /// <param name="value">
    ///     The value member in the setup packet. Its meaning depends on the request. Value should be between
    ///     zero and 65535 (0xFFFF).
    /// </param>
    /// <param name="index">
    ///     The index member in the setup packet. Its meaning depends on the request. Index should be between
    ///     zero and 65535 (0xFFFF).
    /// </param>
    /// <param name="buffer">
    ///     The data to transfer in the data stage of the control. When the transfer is in the IN direction the data received
    ///     will be
    ///     written to this buffer. For an OUT direction transfer the contents of the buffer are written sent through the pipe.
    ///     Note: This buffer is not allowed
    ///     to change for the duration of the asynchronous operation.
    /// </param>
    /// <param name="length">
    ///     Length of the data to transfer. Must be equal to or less than the length of
    ///     <paramref name="buffer" />. The setup packet's length member will be set to this length.
    /// </param>
    /// <param name="userCallback">
    ///     An optional asynchronous callback, to be called when the control transfer is complete. Can
    ///     be null if no callback is required.
    /// </param>
    /// <param name="stateObject">
    ///     A user-provided object that distinguishes this particular asynchronous operation. Can be null
    ///     if not required.
    /// </param>
    /// <returns>
    ///     An <see cref="IAsyncResult" /> object representing the asynchronous control transfer, which could still be
    ///     pending.
    /// </returns>
    /// <remarks>
    ///     This method always completes immediately even if the operation is still pending. The <see cref="IAsyncResult" />
    ///     object returned represents the operation
    ///     and must be passed to <see cref="EndControlTransfer" /> to retrieve the result of the operation. For every call to
    ///     this method a matching call to
    ///     <see cref="EndControlTransfer" /> must be made. When <paramref name="userCallback" /> specifies a callback
    ///     function, this function will be called when the operation is completed. The optional
    ///     <paramref name="stateObject" /> parameter can be used to pass user-defined information to this callback or the
    ///     <see cref="IAsyncResult" />. The <see cref="IAsyncResult" />
    ///     also provides an event handle (<see cref="IAsyncResult.AsyncWaitHandle" />) that will be triggered when the
    ///     operation is complete as well.
    /// </remarks>
    public IAsyncResult BeginControlTransfer(byte requestType, byte request, int value, int index, byte[] buffer,
        int length, AsyncCallback userCallback, object stateObject)
    {
        // Parameters are int and not ushort because ushort is not CLS compliant.
        CheckNotDisposed();
        CheckControlParams(value, index, buffer, length);

        USBAsyncResult result = new(userCallback, stateObject);

        try
        {
            InternalDevice.ControlTransferOverlapped(requestType, request, (ushort)value, (ushort)index, (ushort)length,
                buffer, result);
        }
        catch (APIException e)
        {
            result.Dispose();
            throw new USBException("Asynchronous control transfer failed", e);
        }
        catch (Exception)
        {
            result.Dispose();
            throw;
        }

        return result;
    }

    /// <summary>
    ///     Initiates an asynchronous control transfer over the default control endpoint. This method allows both IN and OUT
    ///     direction transfers, depending
    ///     on the highest bit of the <paramref name="requestType" /> parameter. Alternatively,
    ///     <see cref="BeginControlIn(byte,byte,int,int,byte[],int,AsyncCallback,object)" /> and
    ///     <see cref="BeginControlIn(byte,byte,int,int,byte[],int,AsyncCallback,object)" /> can be used for asynchronous
    ///     control transfers in a specific direction, which is
    ///     the recommended way because it prevents using the wrong direction accidentally. Use the BeginControlTransfer method
    ///     when the direction is not
    ///     known at compile time.
    /// </summary>
    /// <param name="requestType">The setup packet request type.</param>
    /// <param name="request">The setup packet device request.</param>
    /// <param name="value">
    ///     The value member in the setup packet. Its meaning depends on the request. Value should be between
    ///     zero and 65535 (0xFFFF).
    /// </param>
    /// <param name="index">
    ///     The index member in the setup packet. Its meaning depends on the request. Index should be between
    ///     zero and 65535 (0xFFFF).
    /// </param>
    /// <param name="buffer">
    ///     The data to transfer in the data stage of the control. When the transfer is in the IN direction the data received
    ///     will be
    ///     written to this buffer. For an OUT direction transfer the contents of the buffer are written sent through the pipe.
    ///     The setup packet's length member will
    ///     be set to the length of this buffer. Note: This buffer is not allowed to change for the duration of the
    ///     asynchronous operation.
    /// </param>
    /// <param name="userCallback">
    ///     An optional asynchronous callback, to be called when the control transfer is complete. Can
    ///     be null if no callback is required.
    /// </param>
    /// <param name="stateObject">
    ///     A user-provided object that distinguishes this particular asynchronous operation. Can be null
    ///     if not required.
    /// </param>
    /// <returns>
    ///     An <see cref="IAsyncResult" /> object representing the asynchronous control transfer, which could still be
    ///     pending.
    /// </returns>
    /// <remarks>
    ///     This method always completes immediately even if the operation is still pending. The <see cref="IAsyncResult" />
    ///     object returned represents the operation
    ///     and must be passed to <see cref="EndControlTransfer" /> to retrieve the result of the operation. For every call to
    ///     this method a matching call to
    ///     <see cref="EndControlTransfer" /> must be made. When <paramref name="userCallback" /> specifies a callback
    ///     function, this function will be called when the operation is completed. The optional
    ///     <paramref name="stateObject" /> parameter can be used to pass user-defined information to this callback or the
    ///     <see cref="IAsyncResult" />. The <see cref="IAsyncResult" />
    ///     also provides an event handle (<see cref="IAsyncResult.AsyncWaitHandle" />) that will be triggered when the
    ///     operation is complete as well.
    /// </remarks>
    public IAsyncResult BeginControlTransfer(byte requestType, byte request, int value, int index, byte[] buffer,
        AsyncCallback userCallback, object stateObject)
    {
        return BeginControlTransfer(requestType, request, value, index, buffer, buffer.Length, userCallback,
            stateObject);
    }

    /// <summary>
    ///     Waits for a pending asynchronous control transfer to complete.
    /// </summary>
    /// <param name="asyncResult">
    ///     The <see cref="IAsyncResult" /> object representing the asynchronous operation,
    ///     as returned by one of the ControlIn, ControlOut or ControlTransfer methods.
    /// </param>
    /// <returns>The number of bytes transferred during the operation.</returns>
    /// <remarks>
    ///     Every asynchronous control transfer must have a matching call to <see cref="EndControlTransfer" /> to dispose
    ///     of any resources used and to retrieve the result of the operation. When the operation was successful the method
    ///     returns the number
    ///     of bytes that were transferred. If an error occurred during the operation this method will throw the exceptions
    ///     that
    ///     would
    ///     otherwise have occurred during the operation. If the operation is not yet finished EndControlTransfer will wait for
    ///     the
    ///     operation to finish before returning.
    /// </remarks>
    public int EndControlTransfer(IAsyncResult asyncResult)
    {
        if (asyncResult == null)
        {
            throw new NullReferenceException("asyncResult cannot be null");
        }

        if (!(asyncResult is USBAsyncResult))
        {
            throw new ArgumentException(
                "AsyncResult object was not created by calling one of the BeginControl* methods on this class.");
        }

        // todo: check duplicate end control
        USBAsyncResult result = (USBAsyncResult)asyncResult;
        try
        {
            if (!result.IsCompleted)
            {
                result.AsyncWaitHandle.WaitOne();
            }

            if (result.Error != null)
            {
                throw new USBException("Asynchronous control transfer from pipe has failed.", result.Error);
            }

            return result.BytesTransferred;
        }
        finally
        {
            result.Dispose();
        }
    }

    /// <summary>
    ///     Initiates a control transfer over the default control endpoint. This method allows both IN and OUT direction
    ///     transfers, depending
    ///     on the highest bit of the <paramref name="requestType" /> parameter). Alternatively,
    ///     <see cref="ControlIn(byte,byte,int,int,Span{byte})" /> and
    ///     <see cref="ControlOut(byte,byte,int,int,Span{byte})" /> can be used for control transfers in a specific direction,
    ///     which is the recommended way because
    ///     it prevents using the wrong direction accidentally. Use the ControlTransfer method when the direction is not known
    ///     at compile time.
    /// </summary>
    /// <param name="requestType">The setup packet request type.</param>
    /// <param name="request">The setup packet device request.</param>
    /// <param name="value">
    ///     The value member in the setup packet. Its meaning depends on the request. Value should be between
    ///     zero and 65535 (0xFFFF).
    /// </param>
    /// <param name="index">
    ///     The index member in the setup packet. Its meaning depends on the request. Index should be between
    ///     zero and 65535 (0xFFFF).
    /// </param>
    /// <param name="buffer">
    ///     The data to transfer in the data stage of the control. When the transfer is in the IN direction the data received
    ///     will be
    ///     written to this buffer. For an OUT direction transfer the contents of the buffer are written sent through the pipe.
    ///     The length of this
    ///     buffer is used as the number of bytes in the control transfer. The setup packet's length member will be set to this
    ///     length as well.
    /// </param>
    /// <returns>The number of bytes received from the device.</returns>
    public int ControlTransfer(byte requestType, byte request, int value, int index, Span<byte> buffer)
    {
        return ControlTransfer(requestType, request, value, index, buffer, buffer.Length);
    }

    /// <summary>
    ///     Initiates a control transfer without a data stage over the default control endpoint. This method allows both IN and
    ///     OUT direction transfers, depending
    ///     on the highest bit of the <paramref name="requestType" /> parameter). Alternatively,
    ///     <see cref="ControlIn(byte,byte,int,int)" /> and
    ///     <see cref="ControlOut(byte,byte,int,int)" /> can be used for control transfers in a specific direction, which is
    ///     the recommended way because
    ///     it prevents using the wrong direction accidentally. Use the ControlTransfer method when the direction is not known
    ///     at compile time.
    /// </summary>
    /// <param name="requestType">The setup packet request type.</param>
    /// <param name="request">The setup packet device request.</param>
    /// <param name="value">
    ///     The value member in the setup packet. Its meaning depends on the request. Value should be between
    ///     zero and 65535 (0xFFFF).
    /// </param>
    /// <param name="index">
    ///     The index member in the setup packet. Its meaning depends on the request. Index should be between
    ///     zero and 65535 (0xFFFF).
    /// </param>
    public void ControlTransfer(byte requestType, byte request, int value, int index)
    {
        // TODO: null instead of empty buffer. But overlapped code would have to be fixed for this (no buffer to pin)
        ControlTransfer(requestType, request, value, index, Array.Empty<byte>(), 0);
    }

    /// <summary>
    ///     Initiates a control transfer over the default control endpoint. The request should have an IN direction (specified
    ///     by the highest bit
    ///     of the <paramref name="requestType" /> parameter). A buffer to receive the data is automatically created by this
    ///     method.
    /// </summary>
    /// <param name="requestType">
    ///     The setup packet request type. The request type must specify the IN direction (highest bit
    ///     set).
    /// </param>
    /// <param name="request">The setup packet device request.</param>
    /// <param name="value">
    ///     The value member in the setup packet. Its meaning depends on the request. Value should be between
    ///     zero and 65535 (0xFFFF).
    /// </param>
    /// <param name="index">
    ///     The index member in the setup packet. Its meaning depends on the request. Index should be between
    ///     zero and 65535 (0xFFFF).
    /// </param>
    /// <param name="length">
    ///     Length of the data to transfer. A buffer will be created with this length and the length member of the setup packet
    ///     will be set to this length.
    /// </param>
    /// <returns>A buffer containing the data transferred.</returns>
    /// <remarks>
    ///     This routine initially allocates a buffer to hold the <paramref name="length" /> bytes of data expected from the
    ///     device.
    ///     If the device responds with less data than expected, this routine will allocate a smaller buffer to copy and return
    ///     only the bytes actually received.
    /// </remarks>
    public byte[] ControlIn(byte requestType, byte request, int value, int index, int length)
    {
        CheckIn(requestType);
        byte[] buffer = new byte[length];
        int actuallyReceived = ControlTransfer(requestType, request, value, index, buffer, buffer.Length);
        if (actuallyReceived < length)
        {
            byte[] outBuffer = new byte[actuallyReceived];
            Array.Copy(buffer, 0, outBuffer, 0, actuallyReceived);
            return outBuffer;
        }

        return buffer;
    }

    /// <summary>
    ///     Initiates a control transfer over the default control endpoint. The request should have an IN direction (specified
    ///     by the highest bit
    ///     of the <paramref name="requestType" /> parameter).
    /// </summary>
    /// <param name="requestType">
    ///     The setup packet request type. The request type must specify the IN direction (highest bit
    ///     set).
    /// </param>
    /// <param name="request">The setup packet device request.</param>
    /// <param name="value">
    ///     The value member in the setup packet. Its meaning depends on the request. Value should be between
    ///     zero and 65535 (0xFFFF).
    /// </param>
    /// <param name="index">
    ///     The index member in the setup packet. Its meaning depends on the request. Index should be between
    ///     zero and 65535 (0xFFFF).
    /// </param>
    /// <param name="buffer">The buffer that will receive the data transferred.</param>
    /// <param name="length">
    ///     Length of the data to transfer. The length member of the setup packet will be set to this length. The buffer
    ///     specified
    ///     by the <paramref name="buffer" /> parameter should have at least this length.
    /// </param>
    /// <returns>The number of bytes received from the device.</returns>
    public int ControlIn(byte requestType, byte request, int value, int index, Span<byte> buffer, int length)
    {
        CheckIn(requestType);
        return ControlTransfer(requestType, request, value, index, buffer, length);
    }

    /// <summary>
    ///     Initiates a control transfer over the default control endpoint. The request should have an IN direction (specified
    ///     by the highest bit
    ///     of the <paramref name="requestType" /> parameter). The length of buffer given by the <paramref name="buffer" />
    ///     parameter will dictate
    ///     the number of bytes that are transferred and the value of the setup packet's length member.
    /// </summary>
    /// <param name="requestType">
    ///     The setup packet request type. The request type must specify the IN direction (highest bit
    ///     set).
    /// </param>
    /// <param name="request">The setup packet device request.</param>
    /// <param name="value">
    ///     The value member in the setup packet. Its meaning depends on the request. Value should be between
    ///     zero and 65535 (0xFFFF).
    /// </param>
    /// <param name="index">
    ///     The index member in the setup packet. Its meaning depends on the request. Index should be between
    ///     zero and 65535 (0xFFFF).
    /// </param>
    /// <param name="buffer">
    ///     The buffer that will receive the data transferred. The length of this buffer will be the number of
    ///     bytes transferred.
    /// </param>
    /// <returns>The number of bytes received from the device.</returns>
    public int ControlIn(byte requestType, byte request, int value, int index, Span<byte> buffer)
    {
        CheckIn(requestType);
        return ControlTransfer(requestType, request, value, index, buffer);
    }

    /// <summary>
    ///     Initiates a control transfer without a data stage over the default control endpoint. The request should have an IN
    ///     direction (specified by the highest bit
    ///     of the <paramref name="requestType" /> parameter). The setup packets' length member will be set to zero.
    /// </summary>
    /// <param name="requestType">
    ///     The setup packet request type. The request type must specify the IN direction (highest bit
    ///     set).
    /// </param>
    /// <param name="request">The setup packet device request.</param>
    /// <param name="value">
    ///     The value member in the setup packet. Its meaning depends on the request. Value should be between
    ///     zero and 65535 (0xFFFF).
    /// </param>
    /// <param name="index">
    ///     The index member in the setup packet. Its meaning depends on the request. Index should be between
    ///     zero and 65535 (0xFFFF).
    /// </param>
    public void ControlIn(byte requestType, byte request, int value, int index)
    {
        CheckIn(requestType);
        // TODO: null instead of empty buffer. But overlapped code would have to be fixed for this (no buffer to pin)
        ControlTransfer(requestType, request, value, index, Array.Empty<byte>());
    }

    /// <summary>
    ///     Initiates a control transfer over the default control endpoint. The request should have an OUT direction (specified
    ///     by the highest bit
    ///     of the <paramref name="requestType" /> parameter).
    /// </summary>
    /// <param name="requestType">
    ///     The setup packet request type. The request type must specify the OUT direction (highest bit
    ///     cleared).
    /// </param>
    /// <param name="request">The setup packet device request.</param>
    /// <param name="value">
    ///     The value member in the setup packet. Its meaning depends on the request. Value should be between
    ///     zero and 65535 (0xFFFF).
    /// </param>
    /// <param name="index">
    ///     The index member in the setup packet. Its meaning depends on the request. Index should be between
    ///     zero and 65535 (0xFFFF).
    /// </param>
    /// <param name="buffer">A buffer containing the data to transfer in the data stage.</param>
    /// <param name="length">
    ///     Length of the data to transfer. Only the first <paramref name="length" /> bytes of <paramref name="buffer" /> will
    ///     be transferred.
    ///     The setup packet's length parameter is set to this length.
    /// </param>
    public void ControlOut(byte requestType, byte request, int value, int index, Span<byte> buffer, int length)
    {
        CheckOut(requestType);
        ControlTransfer(requestType, request, value, index, buffer, length);
    }

    /// <summary>
    ///     Initiates a control transfer over the default control endpoint. The request should have an OUT direction (specified
    ///     by the highest bit
    ///     of the <paramref name="requestType" /> parameter).
    /// </summary>
    /// <param name="requestType">
    ///     The setup packet request type. The request type must specify the OUT direction (highest bit
    ///     cleared).
    /// </param>
    /// <param name="request">The setup packet device request.</param>
    /// <param name="value">
    ///     The value member in the setup packet. Its meaning depends on the request. Value should be between
    ///     zero and 65535 (0xFFFF).
    /// </param>
    /// <param name="index">
    ///     The index member in the setup packet. Its meaning depends on the request. Index should be between
    ///     zero and 65535 (0xFFFF).
    /// </param>
    /// <param name="buffer">
    ///     A buffer containing the data to transfer in the data stage. The complete buffer is transferred. The setup packet's
    ///     length
    ///     parameter is set to the length of this buffer.
    /// </param>
    public void ControlOut(byte requestType, byte request, int value, int index, Span<byte> buffer)
    {
        CheckOut(requestType);
        ControlTransfer(requestType, request, value, index, buffer);
    }

    /// <summary>
    ///     Initiates a control transfer without a data stage over the default control endpoint. The request should have an OUT
    ///     direction (specified by the highest bit
    ///     of the <paramref name="requestType" /> parameter. The setup packets' length member will be set to zero.
    /// </summary>
    /// <param name="requestType">
    ///     The setup packet request type. The request type must specify the OUT direction (highest bit
    ///     cleared).
    /// </param>
    /// <param name="request">The setup packet device request.</param>
    /// <param name="value">
    ///     The value member in the setup packet. Its meaning depends on the request. Value should be between
    ///     zero and 65535 (0xFFFF).
    /// </param>
    /// <param name="index">
    ///     The index member in the setup packet. Its meaning depends on the request. Index should be between
    ///     zero and 65535 (0xFFFF).
    /// </param>
    public void ControlOut(byte requestType, byte request, int value, int index)
    {
        CheckOut(requestType);
        // TODO: null instead of empty buffer. But overlapped code would have to be fixed for this (no buffer to pin)
        ControlTransfer(requestType, request, value, index, Array.Empty<byte>());
    }

    /// <summary>
    ///     Initiates an asynchronous control transfer without a data stage over the default control endpoint. This method
    ///     allows both IN and OUT direction transfers, depending
    ///     on the highest bit of the <paramref name="requestType" /> parameter. Alternatively,
    ///     <see cref="BeginControlIn(byte,byte,int,int,byte[],int,AsyncCallback,object)" /> and
    ///     <see cref="BeginControlIn(byte,byte,int,int,byte[],int,AsyncCallback,object)" /> can be used for asynchronous
    ///     control transfers in a specific direction, which is
    ///     the recommended way because it prevents using the wrong direction accidentally. Use the BeginControlTransfer method
    ///     when the direction is not
    ///     known at compile time.
    /// </summary>
    /// <param name="requestType">The setup packet request type.</param>
    /// <param name="request">The setup packet device request.</param>
    /// <param name="value">
    ///     The value member in the setup packet. Its meaning depends on the request. Value should be between
    ///     zero and 65535 (0xFFFF).
    /// </param>
    /// <param name="index">
    ///     The index member in the setup packet. Its meaning depends on the request. Index should be between
    ///     zero and 65535 (0xFFFF).
    /// </param>
    /// <param name="userCallback">
    ///     An optional asynchronous callback, to be called when the control transfer is complete. Can
    ///     be null if no callback is required.
    /// </param>
    /// <param name="stateObject">
    ///     A user-provided object that distinguishes this particular asynchronous operation. Can be null
    ///     if not required.
    /// </param>
    /// <returns>
    ///     An <see cref="IAsyncResult" /> object representing the asynchronous control transfer, which could still be
    ///     pending.
    /// </returns>
    /// <remarks>
    ///     This method always completes immediately even if the operation is still pending. The <see cref="IAsyncResult" />
    ///     object returned represents the operation
    ///     and must be passed to <see cref="EndControlTransfer" /> to retrieve the result of the operation. For every call to
    ///     this method a matching call to
    ///     <see cref="EndControlTransfer" /> must be made. When <paramref name="userCallback" /> specifies a callback
    ///     function, this function will be called when the operation is completed. The optional
    ///     <paramref name="stateObject" /> parameter can be used to pass user-defined information to this callback or the
    ///     <see cref="IAsyncResult" />. The <see cref="IAsyncResult" />
    ///     also provides an event handle (<see cref="IAsyncResult.AsyncWaitHandle" />) that will be triggered when the
    ///     operation is complete as well.
    /// </remarks>
    public IAsyncResult BeginControlTransfer(byte requestType, byte request, int value, int index,
        AsyncCallback userCallback, object stateObject)
    {
        // TODO: null instead of empty buffer. But overlapped code would have to be fixed for this (no buffer to pin)
        return BeginControlTransfer(requestType, request, value, index, Array.Empty<byte>(), 0, userCallback,
            stateObject);
    }

    /// <summary>
    ///     Initiates an asynchronous control transfer over the default control endpoint.  The request should have an IN
    ///     direction (specified by the highest bit
    ///     of the <paramref name="requestType" /> parameter).
    /// </summary>
    /// <param name="requestType">
    ///     The setup packet request type. The request type must specify the IN direction (highest bit
    ///     set).
    /// </param>
    /// <param name="request">The setup packet device request.</param>
    /// <param name="value">
    ///     The value member in the setup packet. Its meaning depends on the request. Value should be between
    ///     zero and 65535 (0xFFFF).
    /// </param>
    /// <param name="index">
    ///     The index member in the setup packet. Its meaning depends on the request. Index should be between
    ///     zero and 65535 (0xFFFF).
    /// </param>
    /// <param name="buffer">The buffer that will receive the data transferred.</param>
    /// <param name="length">
    ///     Length of the data to transfer. Must be equal to or less than the length of
    ///     <paramref name="buffer" />. The setup packet's length member will be set to this length.
    /// </param>
    /// <param name="userCallback">
    ///     An optional asynchronous callback, to be called when the control transfer is complete. Can
    ///     be null if no callback is required.
    /// </param>
    /// <param name="stateObject">
    ///     A user-provided object that distinguishes this particular asynchronous operation. Can be null
    ///     if not required.
    /// </param>
    /// <returns>
    ///     An <see cref="IAsyncResult" /> object representing the asynchronous control transfer, which could still be
    ///     pending.
    /// </returns>
    /// <remarks>
    ///     This method always completes immediately even if the operation is still pending. The <see cref="IAsyncResult" />
    ///     object returned represents the operation
    ///     and must be passed to <see cref="EndControlTransfer" /> to retrieve the result of the operation. For every call to
    ///     this method a matching call to
    ///     <see cref="EndControlTransfer" /> must be made. When <paramref name="userCallback" /> specifies a callback
    ///     function, this function will be called when the operation is completed. The optional
    ///     <paramref name="stateObject" /> parameter can be used to pass user-defined information to this callback or the
    ///     <see cref="IAsyncResult" />. The <see cref="IAsyncResult" />
    ///     also provides an event handle (<see cref="IAsyncResult.AsyncWaitHandle" />) that will be triggered when the
    ///     operation is complete as well.
    /// </remarks>
    public IAsyncResult BeginControlIn(byte requestType, byte request, int value, int index, byte[] buffer, int length,
        AsyncCallback userCallback, object stateObject)
    {
        CheckIn(requestType);
        return BeginControlTransfer(requestType, request, value, index, buffer, length, userCallback, stateObject);
    }

    /// <summary>
    ///     Initiates an asynchronous control transfer over the default control endpoint. The request should have an IN
    ///     direction (specified by the highest bit
    ///     of the <paramref name="requestType" /> parameter).
    /// </summary>
    /// <param name="requestType">
    ///     The setup packet request type. The request type must specify the IN direction (highest bit
    ///     set).
    /// </param>
    /// <param name="request">The setup packet device request.</param>
    /// <param name="value">
    ///     The value member in the setup packet. Its meaning depends on the request. Value should be between
    ///     zero and 65535 (0xFFFF).
    /// </param>
    /// <param name="index">
    ///     The index member in the setup packet. Its meaning depends on the request. Index should be between
    ///     zero and 65535 (0xFFFF).
    /// </param>
    /// <param name="buffer">
    ///     The buffer that will receive the data transferred. The setup packet's length member will be set to
    ///     the length of this buffer.
    /// </param>
    /// <param name="userCallback">
    ///     An optional asynchronous callback, to be called when the control transfer is complete. Can
    ///     be null if no callback is required.
    /// </param>
    /// <param name="stateObject">
    ///     A user-provided object that distinguishes this particular asynchronous operation. Can be null
    ///     if not required.
    /// </param>
    /// <returns>
    ///     An <see cref="IAsyncResult" /> object representing the asynchronous control transfer, which could still be
    ///     pending.
    /// </returns>
    /// <remarks>
    ///     This method always completes immediately even if the operation is still pending. The <see cref="IAsyncResult" />
    ///     object returned represents the operation
    ///     and must be passed to <see cref="EndControlTransfer" /> to retrieve the result of the operation. For every call to
    ///     this method a matching call to
    ///     <see cref="EndControlTransfer" /> must be made. When <paramref name="userCallback" /> specifies a callback
    ///     function, this function will be called when the operation is completed. The optional
    ///     <paramref name="stateObject" /> parameter can be used to pass user-defined information to this callback or the
    ///     <see cref="IAsyncResult" />. The <see cref="IAsyncResult" />
    ///     also provides an event handle (<see cref="IAsyncResult.AsyncWaitHandle" />) that will be triggered when the
    ///     operation is complete as well.
    /// </remarks>
    public IAsyncResult BeginControlIn(byte requestType, byte request, int value, int index, byte[] buffer,
        AsyncCallback userCallback, object stateObject)
    {
        CheckIn(requestType);
        return BeginControlTransfer(requestType, request, value, index, buffer, userCallback, stateObject);
    }

    /// <summary>
    ///     Initiates an asynchronous control transfer without a data stage over the default control endpoint.
    ///     The request should have an IN direction (specified by the highest bit of the <paramref name="requestType" />
    ///     parameter).
    ///     The setup packets' length member will be set to zero.
    /// </summary>
    /// <param name="requestType">
    ///     The setup packet request type. The request type must specify the IN direction (highest bit
    ///     set).
    /// </param>
    /// <param name="request">The setup packet device request.</param>
    /// <param name="value">
    ///     The value member in the setup packet. Its meaning depends on the request. Value should be between
    ///     zero and 65535 (0xFFFF).
    /// </param>
    /// <param name="index">
    ///     The index member in the setup packet. Its meaning depends on the request. Index should be between
    ///     zero and 65535 (0xFFFF).
    /// </param>
    /// <param name="userCallback">
    ///     An optional asynchronous callback, to be called when the control transfer is complete. Can
    ///     be null if no callback is required.
    /// </param>
    /// <param name="stateObject">
    ///     A user-provided object that distinguishes this particular asynchronous operation. Can be null
    ///     if not required.
    /// </param>
    /// <returns>
    ///     An <see cref="IAsyncResult" /> object representing the asynchronous control transfer, which could still be
    ///     pending.
    /// </returns>
    /// <remarks>
    ///     This method always completes immediately even if the operation is still pending. The <see cref="IAsyncResult" />
    ///     object returned represents the operation
    ///     and must be passed to <see cref="EndControlTransfer" /> to retrieve the result of the operation. For every call to
    ///     this method a matching call to
    ///     <see cref="EndControlTransfer" /> must be made. When <paramref name="userCallback" /> specifies a callback
    ///     function, this function will be called when the operation is completed. The optional
    ///     <paramref name="stateObject" /> parameter can be used to pass user-defined information to this callback or the
    ///     <see cref="IAsyncResult" />. The <see cref="IAsyncResult" />
    ///     also provides an event handle (<see cref="IAsyncResult.AsyncWaitHandle" />) that will be triggered when the
    ///     operation is complete as well.
    /// </remarks>
    public IAsyncResult BeginControlIn(byte requestType, byte request, int value, int index, AsyncCallback userCallback,
        object stateObject)
    {
        CheckIn(requestType);
        return BeginControlTransfer(requestType, request, value, index, userCallback, stateObject);
    }

    /// <summary>
    ///     Initiates an asynchronous control transfer over the default control endpoint.  The request should have an OUT
    ///     direction (specified by the highest bit
    ///     of the <paramref name="requestType" /> parameter).
    /// </summary>
    /// <param name="requestType">
    ///     The setup packet request type. The request type must specify the OUT direction (highest bit
    ///     cleared).
    /// </param>
    /// <param name="request">The setup packet device request.</param>
    /// <param name="value">
    ///     The value member in the setup packet. Its meaning depends on the request. Value should be between
    ///     zero and 65535 (0xFFFF).
    /// </param>
    /// <param name="index">
    ///     The index member in the setup packet. Its meaning depends on the request. Index should be between
    ///     zero and 65535 (0xFFFF).
    /// </param>
    /// <param name="buffer">The buffer that contains the data to be transferred.</param>
    /// <param name="length">
    ///     Length of the data to transfer. Must be equal to or less than the length of
    ///     <paramref name="buffer" />. The setup packet's length member will be set to this length.
    /// </param>
    /// <param name="userCallback">
    ///     An optional asynchronous callback, to be called when the control transfer is complete. Can
    ///     be null if no callback is required.
    /// </param>
    /// <param name="stateObject">
    ///     A user-provided object that distinguishes this particular asynchronous operation. Can be null
    ///     if not required.
    /// </param>
    /// <returns>
    ///     An <see cref="IAsyncResult" /> object representing the asynchronous control transfer, which could still be
    ///     pending.
    /// </returns>
    /// <remarks>
    ///     This method always completes immediately even if the operation is still pending. The <see cref="IAsyncResult" />
    ///     object returned represents the operation
    ///     and must be passed to <see cref="EndControlTransfer" /> to retrieve the result of the operation. For every call to
    ///     this method a matching call to
    ///     <see cref="EndControlTransfer" /> must be made. When <paramref name="userCallback" /> specifies a callback
    ///     function, this function will be called when the operation is completed. The optional
    ///     <paramref name="stateObject" /> parameter can be used to pass user-defined information to this callback or the
    ///     <see cref="IAsyncResult" />. The <see cref="IAsyncResult" />
    ///     also provides an event handle (<see cref="IAsyncResult.AsyncWaitHandle" />) that will be triggered when the
    ///     operation is complete as well.
    /// </remarks>
    public IAsyncResult BeginControlOut(byte requestType, byte request, int value, int index, byte[] buffer, int length,
        AsyncCallback userCallback, object stateObject)
    {
        CheckOut(requestType);
        return BeginControlTransfer(requestType, request, value, index, buffer, length, userCallback, stateObject);
    }

    /// <summary>
    ///     Initiates an asynchronous control transfer over the default control endpoint. The request should have an OUT
    ///     direction (specified by the highest bit
    ///     of the <paramref name="requestType" /> parameter).
    /// </summary>
    /// <param name="requestType">
    ///     The setup packet request type. The request type must specify the OUT direction (highest bit
    ///     cleared).
    /// </param>
    /// <param name="request">The setup packet device request.</param>
    /// <param name="value">
    ///     The value member in the setup packet. Its meaning depends on the request. Value should be between
    ///     zero and 65535 (0xFFFF).
    /// </param>
    /// <param name="index">
    ///     The index member in the setup packet. Its meaning depends on the request. Index should be between
    ///     zero and 65535 (0xFFFF).
    /// </param>
    /// <param name="buffer">
    ///     The buffer that contains the data to be transferred. The setup packet's length member will be set
    ///     to the length of this buffer.
    /// </param>
    /// <param name="userCallback">
    ///     An optional asynchronous callback, to be called when the control transfer is complete. Can
    ///     be null if no callback is required.
    /// </param>
    /// <param name="stateObject">
    ///     A user-provided object that distinguishes this particular asynchronous operation. Can be null
    ///     if not required.
    /// </param>
    /// <returns>
    ///     An <see cref="IAsyncResult" /> object representing the asynchronous control transfer, which could still be
    ///     pending.
    /// </returns>
    /// <remarks>
    ///     This method always completes immediately even if the operation is still pending. The <see cref="IAsyncResult" />
    ///     object returned represents the operation
    ///     and must be passed to <see cref="EndControlTransfer" /> to retrieve the result of the operation. For every call to
    ///     this method a matching call to
    ///     <see cref="EndControlTransfer" /> must be made. When <paramref name="userCallback" /> specifies a callback
    ///     function, this function will be called when the operation is completed. The optional
    ///     <paramref name="stateObject" /> parameter can be used to pass user-defined information to this callback or the
    ///     <see cref="IAsyncResult" />. The <see cref="IAsyncResult" />
    ///     also provides an event handle (<see cref="IAsyncResult.AsyncWaitHandle" />) that will be triggered when the
    ///     operation is complete as well.
    /// </remarks>
    public IAsyncResult BeginControlOut(byte requestType, byte request, int value, int index, byte[] buffer,
        AsyncCallback userCallback, object stateObject)
    {
        CheckOut(requestType);
        return BeginControlTransfer(requestType, request, value, index, buffer, userCallback, stateObject);
    }

    /// <summary>
    ///     Initiates an asynchronous control transfer without a data stage over the default control endpoint.
    ///     The request should have an OUT direction (specified by the highest bit of the <paramref name="requestType" />
    ///     parameter).
    ///     The setup packets' length member will be set to zero.
    /// </summary>
    /// <param name="requestType">
    ///     The setup packet request type. The request type must specify the OUT direction (highest bit
    ///     cleared).
    /// </param>
    /// <param name="request">The setup packet device request.</param>
    /// <param name="value">
    ///     The value member in the setup packet. Its meaning depends on the request. Value should be between
    ///     zero and 65535 (0xFFFF).
    /// </param>
    /// <param name="index">
    ///     The index member in the setup packet. Its meaning depends on the request. Index should be between
    ///     zero and 65535 (0xFFFF).
    /// </param>
    /// <param name="userCallback">
    ///     An optional asynchronous callback, to be called when the control transfer is complete. Can
    ///     be null if no callback is required.
    /// </param>
    /// <param name="stateObject">
    ///     A user-provided object that distinguishes this particular asynchronous operation. Can be null
    ///     if not required.
    /// </param>
    /// <returns>
    ///     An <see cref="IAsyncResult" /> object representing the asynchronous control transfer, which could still be
    ///     pending.
    /// </returns>
    /// <remarks>
    ///     This method always completes immediately even if the operation is still pending. The <see cref="IAsyncResult" />
    ///     object returned represents the operation
    ///     and must be passed to <see cref="EndControlTransfer" /> to retrieve the result of the operation. For every call to
    ///     this method a matching call to
    ///     <see cref="EndControlTransfer" /> must be made. When <paramref name="userCallback" /> specifies a callback
    ///     function, this function will be called when the operation is completed. The optional
    ///     <paramref name="stateObject" /> parameter can be used to pass user-defined information to this callback or the
    ///     <see cref="IAsyncResult" />. The <see cref="IAsyncResult" />
    ///     also provides an event handle (<see cref="IAsyncResult.AsyncWaitHandle" />) that will be triggered when the
    ///     operation is complete as well.
    /// </remarks>
    public IAsyncResult BeginControlOut(byte requestType, byte request, int value, int index,
        AsyncCallback userCallback, object stateObject)
    {
        CheckOut(requestType);
        // TODO: null instead of empty buffer. But overlapped code would have to be fixed for this (no buffer to pin)
        return BeginControlTransfer(requestType, request, value, index, Array.Empty<byte>(), userCallback, stateObject);
    }

    /// <summary>
    ///     Finds WinUSB devices with a GUID matching the parameter guidString
    /// </summary>
    /// <param name="guidString">
    ///     The GUID string that the device should match.
    ///     The format of this string may be any format accepted by the constructor
    ///     of the System.Guid class
    /// </param>
    /// <returns>
    ///     An array of USBDeviceInfo objects representing the
    ///     devices found. When no devices are found an empty array is
    ///     returned.
    /// </returns>
    public static USBDeviceInfo[] GetDevices(string guidString)
    {
        return GetDevices(new Guid(guidString));
    }

    /// <summary>
    ///     Finds WinUSB devices with a GUID matching the parameter guid
    /// </summary>
    /// <param name="guid">The GUID that the device should match.</param>
    /// <returns>
    ///     An array of USBDeviceInfo objects representing the
    ///     devices found. When no devices are found an empty array is
    ///     returned.
    /// </returns>
    public static USBDeviceInfo[] GetDevices(Guid guid)
    {
        DeviceDetails[] detailList = DeviceManagement.FindDevicesFromGuid(guid);

        USBDeviceInfo[] devices = new USBDeviceInfo[detailList.Length];

        for (int i = 0; i < detailList.Length; i++)
        {
            devices[i] = new USBDeviceInfo(detailList[i]);
        }

        return devices;
    }

    /// <summary>
    ///     Finds the first WinUSB device with a GUID matching the parameter guid.
    ///     If multiple WinUSB devices match the GUID only the first one is returned.
    /// </summary>
    /// <param name="guid">The GUID that the device should match.</param>
    /// <returns>
    ///     An UsbDevice object representing the device if found. If
    ///     no device with the given GUID could be found null is returned.
    /// </returns>
    public static USBDevice GetSingleDevice(Guid guid)
    {
        DeviceDetails[] detailList = DeviceManagement.FindDevicesFromGuid(guid);
        if (detailList.Length == 0)
        {
            return null;
        }

        return new USBDevice(detailList[0].DevicePath);
    }

    /// <summary>
    ///     Finds the first WinUSB device with a GUID matching the parameter guidString.
    ///     If multiple WinUSB devices match the GUID only the first one is returned.
    /// </summary>
    /// <param name="guidString">The GUID string that the device should match.</param>
    /// <returns>
    ///     An UsbDevice object representing the device if found. If
    ///     no device with the given GUID could be found null is returned.
    /// </returns>
    public static USBDevice GetSingleDevice(string guidString)
    {
        return GetSingleDevice(new Guid(guidString));
    }

    /// <summary>
    ///     Opens a WinUSB device by provided path (symbolic link).
    /// </summary>
    /// <param name="path">The device path (symbolic link) to open.</param>
    /// <returns>a <see cref="USBDevice" /> object.</returns>
    public static USBDevice GetSingleDeviceByPath(string path)
    {
        return new USBDevice(path);
    }

    /// <summary>
    ///     Gets available language IDs from the device.
    /// </summary>
    public int[] GetSupportedLanguageIDs()
    {
        try
        {
            return InternalDevice.GetSupportedLanguageIDs_CLS();
        }
        catch (APIException e)
        {
            throw new USBException("Failed to retrieve language IDs.", e);
        }
    }

    /// <summary>
    ///     Synchronously reads the string descriptor.
    /// </summary>
    /// <param name="index">
    ///     The descriptor index. For an explanation of the descriptor index, see the Universal Serial Bus
    ///     specification (<see href="www.usb.org" />).
    /// </param>
    /// <param name="languageId">
    ///     A value that specifies the language identifier. languageID should be between zero and 65535
    ///     (0xFFFF).
    /// </param>
    /// <returns>The string descriptor content.</returns>
    public string GetStringDescriptor(byte index, int languageId)
    {
        try
        {
            return InternalDevice.GetStringDescriptor(index, (ushort)languageId);
        }
        catch (APIException e)
        {
            throw new USBException("Failed to retrieve string descriptor.", e);
        }
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return Descriptor.ToString();
    }
}