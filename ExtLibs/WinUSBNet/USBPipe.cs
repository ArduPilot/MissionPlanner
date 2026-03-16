/*  WinUSBNet library
 *  (C) 2010 Thomas Bleeker (www.madwizard.org)
 *
 *  Licensed under the MIT license, see license.txt or:
 *  http://www.opensource.org/licenses/mit-license.php
 */

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Windows.Win32.Devices.Usb;
using Nefarius.Drivers.WinUSB.API;

namespace Nefarius.Drivers.WinUSB;

/// <summary>
///     UsbPipe represents a single pipe on a WinUSB device. A pipe is connected
///     to a certain endpoint on the device and has a fixed direction (IN or OUT)
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global")]
public sealed class USBPipe
{
    private readonly WINUSB_PIPE_INFORMATION _pipeInfo;

    internal USBPipe(USBDevice device, WINUSB_PIPE_INFORMATION pipeInfo)
    {
        _pipeInfo = pipeInfo;
        Device = device;

        // Policy is not set until interface is attached
        Policy = null;
    }

    /// <summary>
    ///     Endpoint address including the direction in the most significant bit
    /// </summary>
    public byte Address => _pipeInfo.PipeId;

    /// <summary>
    ///     The USBDevice this pipe is associated with
    /// </summary>
    public USBDevice Device { get; }

    /// <summary>
    ///     Maximum packet size for transfers on this endpoint
    /// </summary>
    public int MaximumPacketSize => _pipeInfo.MaximumPacketSize;

    /// <summary>
    ///     The interface associated with this pipe
    /// </summary>
    public USBInterface Interface { get; private set; }

    /// <summary>
    ///     The pipe policy settings for this pipe
    /// </summary>
    public USBPipePolicy Policy { get; private set; }

    /// <summary>
    ///     The transfer method used for this pipe
    /// </summary>
    public USBTransferType TransferType => (USBTransferType)_pipeInfo.PipeType;

    /// <summary>
    ///     True if the pipe has direction OUT (host to device), false otherwise.
    /// </summary>
    public bool IsOut => (_pipeInfo.PipeId & 0x80) == 0;

    /// <summary>
    ///     True if the pipe has direction IN (device to host), false otherwise.
    /// </summary>
    public bool IsIn => (_pipeInfo.PipeId & 0x80) != 0;

    /// <summary>
    ///     Reads data from the pipe into a buffer.
    /// </summary>
    /// <param name="buffer">
    ///     The buffer to read data into. The maximum number of bytes that will be read is specified by the
    ///     length of the buffer.
    /// </param>
    /// <returns>The number of bytes read from the pipe.</returns>
    public int Read(Span<byte> buffer)
    {
        return Read(buffer, 0, buffer.Length);
    }

    /// <summary>
    ///     Reads data from the pipe into a buffer.
    /// </summary>
    /// <param name="buffer">The buffer to read data into.</param>
    /// <param name="offset">The byte offset in <paramref name="buffer" /> from which to begin writing data read from the pipe.</param>
    /// <param name="length">The maximum number of bytes to read, starting at offset</param>
    /// <returns>The number of bytes read from the pipe.</returns>
    public int Read(Span<byte> buffer, int offset, int length)
    {
        CheckReadParams(buffer, offset, length);

        try
        {
            Device.InternalDevice.ReadPipe(Interface.InterfaceIndex, _pipeInfo.PipeId, buffer, offset, length,
                out uint bytesRead);

            return (int)bytesRead;
        }
        catch (APIException e)
        {
            throw new USBException("Failed to read from pipe.", e);
        }
    }

    private void CheckReadParams(Span<byte> buffer, int offset, int length)
    {
        if (!IsIn)
            throw new NotSupportedException("Cannot read from a pipe with OUT direction.");

        var bufferLength = buffer.Length;
        if (offset < 0 || offset >= bufferLength)
            throw new ArgumentOutOfRangeException(nameof(offset),
                "Offset of data to read is outside the buffer boundaries.");
        if (length < 0 || offset + length > bufferLength)
            throw new ArgumentOutOfRangeException(nameof(length),
                "Length of data to read is outside the buffer boundaries.");
    }

    private void CheckWriteParams(ReadOnlySpan<byte> buffer, int offset, int length)
    {
        if (!IsOut)
            throw new NotSupportedException("Cannot write to a pipe with IN direction.");

        var bufferLength = buffer.Length;
        if (offset < 0 || ((bufferLength > 0 && offset >= bufferLength)))
            throw new ArgumentOutOfRangeException(nameof(offset),
                "Offset of data to write is outside the buffer boundaries.");
        if (length < 0 || offset + length > bufferLength)
            throw new ArgumentOutOfRangeException(nameof(length),
                "Length of data to write is outside the buffer boundaries.");
    }

    /// <summary>Initiates an asynchronous read operation on the pipe. </summary>
    /// <param name="buffer">Buffer that will receive the data read from the pipe.</param>
    /// <param name="offset">Byte offset within the buffer at which to begin writing the data received.</param>
    /// <param name="length">Length of the data to transfer.</param>
    /// <param name="userCallback">
    ///     An optional asynchronous callback, to be called when the operation is complete. Can be null
    ///     if no callback is required.
    /// </param>
    /// <param name="stateObject">
    ///     A user-provided object that distinguishes this particular asynchronous operation. Can be null
    ///     if not required.
    /// </param>
    /// <returns>An <see cref="IAsyncResult" /> object representing the asynchronous operation, which could still be pending.</returns>
    /// <remarks>
    ///     This method always completes immediately even if the operation is still pending. The <see cref="IAsyncResult" />
    ///     object returned represents the operation
    ///     and must be passed to <see cref="EndRead" /> to retrieve the result of the operation. For every call to this method
    ///     a matching call to
    ///     <see cref="EndRead" /> must be made. When <paramref name="userCallback" /> specifies a callback function, this
    ///     function will be called when the operation is completed. The optional
    ///     <paramref name="stateObject" /> parameter can be used to pass user-defined information to this callback or the
    ///     <see cref="IAsyncResult" />. The <see cref="IAsyncResult" />
    ///     also provides an event handle (<see cref="IAsyncResult.AsyncWaitHandle" />) that will be triggered when the
    ///     operation is complete as well.
    /// </remarks>
    public IAsyncResult BeginRead(byte[] buffer, int offset, int length, AsyncCallback userCallback, object stateObject)
    {
        CheckReadParams(buffer, offset, length);

        var result = new USBAsyncResult(userCallback, stateObject);
        try
        {
            Device.InternalDevice.ReadPipeOverlapped(Interface.InterfaceIndex, _pipeInfo.PipeId, buffer, offset, length,
                result);
        }
        catch (APIException e)
        {
            result.Dispose();
            throw new USBException("Failed to read from pipe.", e);
        }
        catch (Exception)
        {
            result.Dispose();
            throw;
        }

        return result;
    }

    /// <summary>
    ///     Waits for a pending asynchronous read operation to complete.
    /// </summary>
    /// <param name="asyncResult">
    ///     The <see cref="IAsyncResult" /> object representing the asynchronous operation,
    ///     as returned by <see cref="BeginRead" />.
    /// </param>
    /// <returns>The number of bytes transferred during the operation.</returns>
    /// <remarks>
    ///     Every call to <see cref="BeginRead" /> must have a matching call to <see cref="EndRead" /> to dispose
    ///     of any resources used and to retrieve the result of the operation. When the operation was successful the method
    ///     returns the number
    ///     of bytes that were transferred. If an error occurred during the operation this method will throw the exceptions
    ///     that would
    ///     otherwise have occurred during the operation. If the operation is not yet finished EndWrite will wait for the
    ///     operation to finish before returning.
    /// </remarks>
    public int EndRead(IAsyncResult asyncResult)
    {
        if (asyncResult == null)
            throw new NullReferenceException("asyncResult cannot be null");
        if (!(asyncResult is USBAsyncResult))
            throw new ArgumentException("AsyncResult object was not created by calling BeginRead on this class.");

        // todo: check duplicate end reads?
        var result = (USBAsyncResult)asyncResult;
        try
        {
            if (!result.IsCompleted)
                result.AsyncWaitHandle.WaitOne();

            if (result.Error != null)
                throw new USBException("Asynchronous read from pipe has failed.", result.Error);

            return result.BytesTransferred;
        }
        finally
        {
            result.Dispose();
        }
    }

    /// <summary>
    ///     Writes data from a buffer to the pipe.
    /// </summary>
    /// <param name="buffer">The buffer to write data from. The complete buffer will be written to the device.</param>
    public void Write(ReadOnlySpan<byte> buffer)
    {
        Write(buffer, 0, buffer.Length);
    }

    /// <summary>
    ///     Writes data from a buffer to the pipe.
    /// </summary>
    /// <param name="buffer">The buffer to write data from.</param>
    /// <param name="offset">The byte offset in <paramref name="buffer" /> from which to begin writing.</param>
    /// <param name="length">The number of bytes to write, starting at offset</param>
    public void Write(ReadOnlySpan<byte> buffer, int offset, int length)
    {
        CheckWriteParams(buffer, offset, length);

        try
        {
            Device.InternalDevice.WritePipe(Interface.InterfaceIndex, _pipeInfo.PipeId, buffer, offset, length);
        }
        catch (APIException e)
        {
            throw new USBException("Failed to write to pipe.", e);
        }
    }

    /// <summary>Initiates an asynchronous write operation on the pipe. </summary>
    /// <param name="buffer">Buffer that contains the data to write to the pipe.</param>
    /// <param name="offset">Byte offset within the buffer from which to begin writing.</param>
    /// <param name="length">Length of the data to transfer.</param>
    /// <param name="userCallback">
    ///     An optional asynchronous callback, to be called when the operation is complete. Can be null
    ///     if no callback is required.
    /// </param>
    /// <param name="stateObject">
    ///     A user-provided object that distinguishes this particular asynchronous operation. Can be null
    ///     if not required.
    /// </param>
    /// <returns>An <see cref="IAsyncResult" /> object representing the asynchronous operation, which could still be pending.</returns>
    /// <remarks>
    ///     This method always completes immediately even if the operation is still pending. The <see cref="IAsyncResult" />
    ///     object returned represents the operation
    ///     and must be passed to <see cref="EndWrite" /> to retrieve the result of the operation. For every call to this
    ///     method a matching call to
    ///     <see cref="EndWrite" /> must be made. When <paramref name="userCallback" /> specifies a callback function, this
    ///     function will be called when the operation is completed. The optional
    ///     <paramref name="stateObject" /> parameter can be used to pass user-defined information to this callback or the
    ///     <see cref="IAsyncResult" />. The <see cref="IAsyncResult" />
    ///     also provides an event handle (<see cref="IAsyncResult.AsyncWaitHandle" />) that will be triggered when the
    ///     operation is complete as well.
    /// </remarks>
    public IAsyncResult BeginWrite(byte[] buffer, int offset, int length, AsyncCallback userCallback,
        object stateObject)
    {
        CheckWriteParams(buffer, offset, length);

        var result = new USBAsyncResult(userCallback, stateObject);
        try
        {
            Device.InternalDevice.WriteOverlapped(Interface.InterfaceIndex, _pipeInfo.PipeId, buffer, offset, length,
                result);
        }
        catch (APIException e)
        {
            result.Dispose();
            throw new USBException("Failed to write to pipe.", e);
        }
        catch (Exception)
        {
            result.Dispose();
            throw;
        }

        return result;
    }

    /// <summary>
    ///     Waits for a pending asynchronous write operation to complete.
    /// </summary>
    /// <param name="asyncResult">
    ///     The <see cref="IAsyncResult" /> object representing the asynchronous operation,
    ///     as returned by <see cref="BeginWrite" />.
    /// </param>
    /// <returns>The number of bytes transferred during the operation.</returns>
    /// <remarks>
    ///     Every call to <see cref="BeginWrite" /> must have a matching call to <see cref="EndWrite" /> to dispose
    ///     of any resources used and to retrieve the result of the operation. When the operation was successful the method
    ///     returns the number
    ///     of bytes that were transferred. If an error occurred during the operation this method will throw the exceptions
    ///     that would
    ///     otherwise have occurred during the operation. If the operation is not yet finished EndWrite will wait for the
    ///     operation to finish before returning.
    /// </remarks>
    public int EndWrite(IAsyncResult asyncResult)
    {
        if (asyncResult == null)
            throw new NullReferenceException("asyncResult cannot be null");
        if (!(asyncResult is USBAsyncResult))
            throw new ArgumentException("AsyncResult object was not created by calling BeginWrite on this class.");

        var result = (USBAsyncResult)asyncResult;
        try
        {
            // todo: check duplicate end writes?

            if (!result.IsCompleted)
                result.AsyncWaitHandle.WaitOne();

            if (result.Error != null)
                throw new USBException("Asynchronous write to pipe has failed.", result.Error);

            return result.BytesTransferred;
        }
        finally
        {
            result.Dispose();
        }
    }

    /// <summary>
    ///     Aborts all pending transfers for this pipe.
    /// </summary>
    public void Abort()
    {
        try
        {
            Device.InternalDevice.AbortPipe(Interface.InterfaceIndex, _pipeInfo.PipeId);
        }
        catch (APIException e)
        {
            throw new USBException("Failed to abort pipe.", e);
        }
    }

    /// <summary>
    ///     Flushes the pipe, discarding any data that is cached. Only available on IN direction pipes.
    /// </summary>
    public void Flush()
    {
        if (!IsIn)
            throw new NotSupportedException("Flush is only supported on IN direction pipes");
        try
        {
            Device.InternalDevice.FlushPipe(Interface.InterfaceIndex, _pipeInfo.PipeId);
        }
        catch (APIException e)
        {
            throw new USBException("Failed to flush pipe.", e);
        }
    }

    /// <summary>
    ///     Resets the pipe to clear a stall condition.
    /// </summary>
    public void Reset()
    {
        try
        {
            Device.InternalDevice.ResetPipe(Interface.InterfaceIndex, _pipeInfo.PipeId);
        }
        catch (APIException e)
        {
            throw new USBException("Failed to reset pipe.", e);
        }
    }

    internal void AttachInterface(USBInterface usbInterface)
    {
        Interface = usbInterface;

        // Initialize policy now that interface is set (policy requires interface)
        Policy = new USBPipePolicy(Device, Interface.InterfaceIndex, _pipeInfo.PipeId);
    }

    /// <summary>Asynchronously reads a sequence of bytes from the USB pipe.</summary>
    /// <param name="buffer">Buffer that will receive the data read from the pipe.</param>
    /// <param name="offset">Byte offset within the buffer at which to begin writing the data received.</param>
    /// <param name="length">Length of the data to transfer.</param>
    /// <returns>
    ///     A task that represents the asynchronous read operation.
    ///     The value of the TResult parameter contains the total number of bytes that has been transferred.
    ///     The result value can be less than the number of bytes requested if the number of bytes currently available is less
    ///     than the requested number,
    ///     or it can be 0 (zero) if the end of the stream has been reached.
    /// </returns>
    public Task<int> ReadAsync(byte[] buffer, int offset, int length)
    {
        var tcs = new TaskCompletionSource<int>();

        BeginRead(buffer, offset, length, iar =>
        {
            try
            {
                tcs.TrySetResult(EndRead(iar));
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }
        }, null);

        return tcs.Task;
    }

    /// <summary>Asynchronously write a sequence of bytes from the USB pipe.</summary>
    /// <param name="buffer">Buffer that will receive the data read from the pipe.</param>
    /// <param name="offset">Byte offset within the buffer at which to begin writing the data received.</param>
    /// <param name="length">Length of the data to transfer.</param>
    /// <returns>
    ///     A task that represents the asynchronous read operation.
    ///     The value of the TResult parameter contains the total number of bytes that has been transferred.
    ///     The result value can be less than the number of bytes requested if the number of bytes currently available is less
    ///     than the requested number,
    ///     or it can be 0 (zero) if the end of the stream has been reached.
    /// </returns>
    public Task<int> WriteAsync(byte[] buffer, int offset, int length)
    {
        var tcs = new TaskCompletionSource<int>();

        BeginWrite(buffer, offset, length, iar =>
        {
            try
            {
                tcs.TrySetResult(EndWrite(iar));
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }
        }, null);

        return tcs.Task;
    }
}
