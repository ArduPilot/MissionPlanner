// <copyright file="ITcpSocket.cs" company="The Android Open Source Project, Ryan Conrad, Quamotion">
// Copyright (c) The Android Open Source Project, Ryan Conrad, Quamotion. All rights reserved.
// </copyright>

namespace SharpAdbClient
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides an interface that allows access to the standard .NET <see cref="Socket"/>
    /// class. The main purpose of this interface is to enable mocking of the <see cref="Socket"/>
    /// in unit test scenarios.
    /// </summary>
    public interface ITcpSocket : IDisposable
    {
        /// <summary>
        /// Gets a value indicating whether a <see cref="ITcpSocket"/> is connected to a remote host as of the last
        /// <see cref="Send"/> or <see cref="Receive"/> operation.
        /// </summary>
        bool Connected
        {
            get;
        }

        /// <summary>
        /// Gets or sets a value that specifies the size of the receive buffer of the <see cref="ITcpSocket"/>.
        /// </summary>
        int ReceiveBufferSize
        {
            get;
            set;
        }

        /// <summary>
        /// Establishes a connection to a remote host.
        /// </summary>
        /// <param name="endPoint">
        /// An <see cref="EndPoint"/> that represents the remote device.
        /// </param>
        void Connect(EndPoint endPoint);

        /// <summary>
        /// Re-establishes the connection to a remote host. Assumes you have resolved the reason that caused the
        /// socket to disconnect.
        /// </summary>
        void Reconnect();

        /// <summary>
        /// Closes the <see cref="ITcpSocket"/> connection and releases all associated resources.
        /// </summary>
        void Close();

        /// <summary>
        /// Sends the specified number of bytes of data to a connected
        /// <see cref="ITcpSocket"/>, starting at the specified <paramref name="offset"/>,
        /// and using the specified <paramref name="socketFlags"/>.
        /// </summary>
        /// <param name="buffer">
        /// An array of type Byte that contains the data to be sent.
        /// </param>
        /// <param name="offset">
        /// The position in the data buffer at which to begin sending data.
        /// </param>
        /// <param name="size">
        /// The number of bytes to send.
        /// </param>
        /// <param name="socketFlags">
        /// A bitwise combination of the SocketFlags values.
        /// </param>
        /// <returns>
        /// The number of bytes sent to the Socket.
        /// </returns>
        int Send(byte[] buffer, int offset, int size, SocketFlags socketFlags);

        /// <summary>
        /// Receives the specified number of bytes from a bound <see cref="ITcpSocket"/> into the specified offset position of the
        /// receive buffer, using the specified SocketFlags.
        /// </summary>
        /// <param name="buffer">
        /// An array of type Byte that is the storage location for received data.
        /// </param>
        /// <param name="size">
        /// The number of bytes to receive.
        /// </param>
        /// <param name="socketFlags">
        /// A bitwise combination of the SocketFlags values.
        /// </param>
        /// <returns>
        /// The number of bytes received.
        /// </returns>
        int Receive(byte[] buffer, int size, SocketFlags socketFlags);

        /// <summary>
        /// Receives the specified number of bytes from a bound <see cref="ITcpSocket"/> into the specified offset position of the
        /// receive buffer, using the specified SocketFlags.
        /// </summary>
        /// <param name="buffer">
        /// An array of type Byte that is the storage location for received data.
        /// </param>
        /// <param name="offset">
        /// The location in buffer to store the received data.
        /// </param>
        /// <param name="size">
        /// The number of bytes to receive.
        /// </param>
        /// <param name="socketFlags">
        /// A bitwise combination of the SocketFlags values.
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
        Task<int> ReceiveAsync(byte[] buffer, int offset, int size, SocketFlags socketFlags, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the underlying <see cref="Stream"/>.
        /// </summary>
        /// <returns>
        /// The underlying stream.
        /// </returns>
        Stream GetStream();
    }
}
