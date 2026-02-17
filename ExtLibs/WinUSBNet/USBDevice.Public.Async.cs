using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Nefarius.Drivers.WinUSB;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
public partial class USBDevice
{
    /// <summary>
    ///     Asynchronously issue a sequence of bytes IO over the default control endpoint.
    ///     This method allows both IN and OUT direction transfers, depending on the highest bit of the
    ///     <paramref name="requestType" /> parameter.
    ///     Alternatively, <see cref="ControlInAsync(byte,byte,int,int,byte[],int)" />
    ///     and <see cref="ControlOutAsync(byte,byte,int,int,byte[],int)" /> can be used for asynchronous control transfers in
    ///     a specific direction,
    ///     which is the recommended way because it prevents using the wrong direction accidentally.
    ///     Use the BeginControlTransfer method when the direction is not known at compile time.
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
    /// <returns>
    ///     A task that represents the asynchronous read operation.
    ///     The value of the TResult parameter contains the total number of bytes that has been transferred.
    ///     The result value can be less than the number of bytes requested if the number of bytes currently available is less
    ///     than the requested number,
    ///     or it can be 0 (zero) if the end of the stream has been reached.
    /// </returns>
    public Task<int> ControlTransferAsync(byte requestType, byte request, int value, int index, byte[] buffer,
        int length)
    {
        TaskCompletionSource<int> tcs = new();

        BeginControlTransfer(requestType, request, value, index, buffer, length, iar =>
        {
            try
            {
                tcs.TrySetResult(EndControlTransfer(iar));
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }
        }, null);

        return tcs.Task;
    }

    /// <summary>
    ///     Asynchronously issue a sequence of bytes IO over the default control endpoint.
    ///     This method allows both IN and OUT direction transfers, depending on the highest bit of the
    ///     <paramref name="requestType" /> parameter.
    ///     Alternatively, <see cref="ControlInAsync(byte,byte,int,int,byte[])" />
    ///     and <see cref="ControlOutAsync(byte,byte,int,int,byte[])" /> can be used for asynchronous control transfers in a
    ///     specific direction,
    ///     which is the recommended way because it prevents using the wrong direction accidentally.
    ///     Use the BeginControlTransfer method when the direction is not known at compile time.
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
    /// <returns>
    ///     A task that represents the asynchronous read operation.
    ///     The value of the TResult parameter contains the total number of bytes that has been transferred.
    ///     The result value can be less than the number of bytes requested if the number of bytes currently available is less
    ///     than the requested number,
    ///     or it can be 0 (zero) if the end of the stream has been reached.
    /// </returns>
    public Task<int> ControlTransferAsync(byte requestType, byte request, int value, int index, byte[] buffer)
    {
        TaskCompletionSource<int> tcs = new();

        BeginControlTransfer(requestType, request, value, index, buffer, iar =>
        {
            try
            {
                tcs.TrySetResult(EndControlTransfer(iar));
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }
        }, null);

        return tcs.Task;
    }

    /// <summary>
    ///     Asynchronously issue a sequence of bytes IO without a data stage over the default control endpoint.
    ///     This method allows both IN and OUT direction transfers, depending on the highest bit of the
    ///     <paramref name="requestType" /> parameter.
    ///     Alternatively, <see cref="ControlInAsync(byte,byte,int,int)" />
    ///     and <see cref="ControlOutAsync(byte,byte,int,int)" /> can be used for asynchronous control transfers in a specific
    ///     direction,
    ///     which is the recommended way because it prevents using the wrong direction accidentally.
    ///     Use the BeginControlTransfer method when the direction is not known at compile time.
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
    /// <returns>
    ///     A task that represents the asynchronous read operation.
    ///     The value of the TResult parameter contains the total number of bytes that has been transferred.
    ///     The result value can be less than the number of bytes requested if the number of bytes currently available is less
    ///     than the requested number,
    ///     or it can be 0 (zero) if the end of the stream has been reached.
    /// </returns>
    public Task<int> ControlTransferAsync(byte requestType, byte request, int value, int index)
    {
        TaskCompletionSource<int> tcs = new();

        BeginControlTransfer(requestType, request, value, index, iar =>
        {
            try
            {
                tcs.TrySetResult(EndControlTransfer(iar));
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }
        }, null);

        return tcs.Task;
    }

    /// <summary>
    ///     Asynchronously issue a sequence of bytes input operation over the default control endpoint.
    ///     The request should have an IN direction (specified by the highest bit of the <paramref name="requestType" />
    ///     parameter).
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
    /// <param name="buffer">The buffer that will receive the data transferred.</param>
    /// <param name="length">
    ///     Length of the data to transfer. Must be equal to or less than the length of
    ///     <paramref name="buffer" />. The setup packet's length member will be set to this length.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous input operation.
    ///     The value of the TResult parameter contains the total number of bytes that has been transferred.
    ///     The result value can be less than the number of bytes requested if the number of bytes currently available is less
    ///     than the requested number,
    ///     or it can be 0 (zero) if the end of the stream has been reached.
    /// </returns>
    public Task<int> ControlInAsync(byte requestType, byte request, int value, int index, byte[] buffer, int length)
    {
        TaskCompletionSource<int> tcs = new();

        BeginControlIn(requestType, request, value, index, buffer, length, iar =>
        {
            try
            {
                tcs.TrySetResult(EndControlTransfer(iar));
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }
        }, null);

        return tcs.Task;
    }

    /// <summary>
    ///     Asynchronously issue a sequence of bytes input operation over the default control endpoint.
    ///     The request should have an IN direction (specified by the highest bit of the <paramref name="requestType" />
    ///     parameter).
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
    /// <param name="buffer">The buffer that will receive the data transferred.</param>
    /// <returns>
    ///     A task that represents the asynchronous input operation.
    ///     The value of the TResult parameter contains the total number of bytes that has been transferred.
    ///     The result value can be less than the number of bytes requested if the number of bytes currently available is less
    ///     than the requested number,
    ///     or it can be 0 (zero) if the end of the stream has been reached.
    /// </returns>
    public Task<int> ControlInAsync(byte requestType, byte request, int value, int index, byte[] buffer)
    {
        TaskCompletionSource<int> tcs = new();

        BeginControlIn(requestType, request, value, index, buffer, iar =>
        {
            try
            {
                tcs.TrySetResult(EndControlTransfer(iar));
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }
        }, null);

        return tcs.Task;
    }

    /// <summary>
    ///     Asynchronously issue a sequence of bytes input operation without a data stage over the default control endpoint.
    ///     The request should have an IN direction (specified by the highest bit of the <paramref name="requestType" />
    ///     parameter).
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
    /// <returns>
    ///     A task that represents the asynchronous input operation.
    ///     The value of the TResult parameter contains the total number of bytes that has been transferred.
    ///     The result value can be less than the number of bytes requested if the number of bytes currently available is less
    ///     than the requested number,
    ///     or it can be 0 (zero) if the end of the stream has been reached.
    /// </returns>
    public Task<int> ControlInAsync(byte requestType, byte request, int value, int index)
    {
        TaskCompletionSource<int> tcs = new();

        BeginControlIn(requestType, request, value, index, iar =>
        {
            try
            {
                tcs.TrySetResult(EndControlTransfer(iar));
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }
        }, null);

        return tcs.Task;
    }

    /// <summary>
    ///     Asynchronously issue a sequence of bytes output operation over the default control endpoint.
    ///     The request should have an OUT direction (specified by the highest bit of the <paramref name="requestType" />
    ///     parameter).
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
    /// <param name="buffer">The buffer that contains the data to be transferred.</param>
    /// <param name="length">
    ///     Length of the data to transfer. Must be equal to or less than the length of
    ///     <paramref name="buffer" />. The setup packet's length member will be set to this length.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous output operation.
    ///     The value of the TResult parameter contains the total number of bytes that has been transferred.
    ///     The result value can be less than the number of bytes requested if the number of bytes currently available is less
    ///     than the requested number,
    ///     or it can be 0 (zero) if the end of the stream has been reached.
    /// </returns>
    public Task<int> ControlOutAsync(byte requestType, byte request, int value, int index, byte[] buffer, int length)
    {
        TaskCompletionSource<int> tcs = new();

        BeginControlOut(requestType, request, value, index, buffer, length, iar =>
        {
            try
            {
                tcs.TrySetResult(EndControlTransfer(iar));
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }
        }, null);

        return tcs.Task;
    }

    /// <summary>
    ///     Asynchronously issue a sequence of bytes output operation over the default control endpoint.
    ///     The request should have an OUT direction (specified by the highest bit of the <paramref name="requestType" />
    ///     parameter).
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
    /// <param name="buffer">The buffer that contains the data to be transferred.</param>
    /// <returns>
    ///     A task that represents the asynchronous output operation.
    ///     The value of the TResult parameter contains the total number of bytes that has been transferred.
    ///     The result value can be less than the number of bytes requested if the number of bytes currently available is less
    ///     than the requested number,
    ///     or it can be 0 (zero) if the end of the stream has been reached.
    /// </returns>
    public Task<int> ControlOutAsync(byte requestType, byte request, int value, int index, byte[] buffer)
    {
        TaskCompletionSource<int> tcs = new();

        BeginControlOut(requestType, request, value, index, buffer, iar =>
        {
            try
            {
                tcs.TrySetResult(EndControlTransfer(iar));
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }
        }, null);

        return tcs.Task;
    }

    /// <summary>
    ///     Asynchronously issue a sequence of bytes output operation without a data stage over the default control endpoint.
    ///     The request should have an OUT direction (specified by the highest bit of the <paramref name="requestType" />
    ///     parameter).
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
    /// <returns>
    ///     A task that represents the asynchronous output operation.
    ///     The value of the TResult parameter contains the total number of bytes that has been transferred.
    ///     The result value can be less than the number of bytes requested if the number of bytes currently available is less
    ///     than the requested number,
    ///     or it can be 0 (zero) if the end of the stream has been reached.
    /// </returns>
    public Task<int> ControlOutAsync(byte requestType, byte request, int value, int index)
    {
        TaskCompletionSource<int> tcs = new();

        BeginControlOut(requestType, request, value, index, iar =>
        {
            try
            {
                tcs.TrySetResult(EndControlTransfer(iar));
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }
        }, null);

        return tcs.Task;
    }
}