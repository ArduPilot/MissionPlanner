// <copyright file="SocketExtensions.cs" company="The Android Open Source Project, Ryan Conrad, Quamotion">
// Copyright (c) The Android Open Source Project, Ryan Conrad, Quamotion. All rights reserved.
// </copyright>

namespace SharpAdbClient
{
    using System;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides extension methods for the <see cref="Socket"/> class.
    /// </summary>
    internal static class SocketExtensions
    {
        /// <summary>
        /// Asynchronously receives data from a connected socket.
        /// </summary>
        /// <param name="socket">
        /// The socket from which to read data.
        /// </param>
        /// <param name="buffer">
        /// An array of type <see cref="byte"/> that is the storage location for
        /// the received data.
        /// </param>
        /// <param name="offset">
        /// The zero-based position in the <paramref name="buffer"/> parameter at which to
        /// start storing data.
        /// </param>
        /// <param name="size">
        /// The number of bytes to receive.
        /// </param>
        /// <param name="socketFlags">
        /// A bitwise combination of the <see cref="SocketFlags"/> values.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous task.
        /// </param>
        /// <remarks>
        /// Cancelling the task will also close the socket.
        /// </remarks>
        /// <returns>
        /// The number of bytes received.
        /// </returns>
        public static Task<int> ReceiveAsync(
            this Socket socket,
            byte[] buffer,
            int offset,
            int size,
            SocketFlags socketFlags,
            CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<int>(socket);

            // Register a callback so that when a cancellation is requested, the socket is closed.
            // This will cause an ObjectDisposedException to bubble up via TrySetResult, which we can catch
            // and convert to a TaskCancelledException - which is the exception we expect.
            cancellationToken.Register(() => socket.Close());

            socket.BeginReceive(
                buffer,
                offset,
                size,
                socketFlags,
                iar =>
                {
                    var t = (TaskCompletionSource<int>)iar.AsyncState;
                    var s = (Socket)t.Task.AsyncState;
                    try
                    {
                        t.TrySetResult(s.EndReceive(iar));
                    }
                    catch (Exception ex)
                    {
                        // Did the cancellationToken's request for cancellation cause the socket to be closed
                        // and an ObjectDisposedException to be thrown? If so, indicate the caller that we were
                        // cancelled. If not, bubble up the original exception.
                        if (ex is ObjectDisposedException && cancellationToken.IsCancellationRequested)
                        {
                            t.TrySetCanceled();
                        }
                        else
                        {
                            t.TrySetException(ex);
                        }
                    }
                },
                tcs);

            return tcs.Task;
        }
    }
}
