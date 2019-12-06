// <copyright file="TcpSocket.cs" company="The Android Open Source Project, Ryan Conrad, Quamotion">
// Copyright (c) The Android Open Source Project, Ryan Conrad, Quamotion. All rights reserved.
// </copyright>

namespace SharpAdbClient
{
    using Mono.Unix;
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Implements the <see cref="ITcpSocket" /> interface using the standard <see cref="Socket"/>
    /// class.
    /// </summary>
    public class TcpSocket : ITcpSocket
    {
        private Socket socket;
        private EndPoint endPoint;

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpSocket"/> class.
        /// </summary>
        public TcpSocket()
        {
        }

        /// <inheritdoc/>
        public bool Connected
        {
            get
            {
                return this.socket.Connected;
            }
        }

        /// <inheritdoc/>
        public int ReceiveBufferSize
        {
            get
            {
                return this.socket.ReceiveBufferSize;
            }

            set
            {
                this.socket.ReceiveBufferSize = value;
            }
        }

        /// <inheritdoc/>
        public void Connect(EndPoint endPoint)
        {
            this.socket = CreateSocket(endPoint);
            this.socket.Connect(endPoint);
            this.socket.Blocking = true;
            this.endPoint = endPoint;
        }

        /// <inheritdoc/>
        public void Reconnect()
        {
            if (this.socket.Connected)
            {
                // Already connected - nothing to do.
                return;
            }

            this.socket = CreateSocket(this.endPoint);
            this.Connect(this.endPoint);
        }

        /// <inheritdoc/>
        public void Close()
        {
            this.socket.Close();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.socket.Dispose();
        }

        /// <inheritdoc/>
        public int Send(byte[] buffer, int offset, int size, SocketFlags socketFlags)
        {
            return this.socket.Send(buffer, offset, size, socketFlags);
        }

        /// <inheritdoc/>
        public Stream GetStream()
        {
            return new NetworkStream(this.socket);
        }

        /// <inheritdoc/>
        public int Receive(byte[] buffer, int offset, SocketFlags socketFlags)
        {
            return this.socket.Receive(buffer, offset, socketFlags);
        }

        /// <inheritdoc/>
        public Task<int> ReceiveAsync(byte[] buffer, int offset, int size, SocketFlags socketFlags, CancellationToken cancellationToken)
        {
            return this.socket.ReceiveAsync(buffer, offset, size, socketFlags, cancellationToken);
        }

        /// <summary>
        /// Creates a new, uninitialized socket which can connect to an ADB server.
        /// </summary>
        /// <param name="endPoint">
        /// The <see cref="EndPoint"/> to which to connect. Currently <see cref="IPEndPoint"/>, <see cref="DnsEndPoint"/>
        /// and <see cref="UnixEndPoint"/> endpoint types are supported.
        /// </param>
        /// <returns>
        /// A new <see cref="Socket"/> which can connect to an ADB server.
        /// </returns>
        internal static Socket CreateSocket(EndPoint endPoint)
        {
            if (endPoint is IPEndPoint || endPoint is DnsEndPoint)
            {
                return new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }
            else if (endPoint is UnixEndPoint)
            {
                return new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.IP);
            }
            else
            {
                throw new NotSupportedException("Only Windows, Linux and Mac OS are supported");
            }
        }
    }
}
