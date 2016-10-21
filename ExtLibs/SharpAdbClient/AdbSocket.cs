// <copyright file="AdbSocket.cs" company="The Android Open Source Project, Ryan Conrad, Quamotion">
// Copyright (c) The Android Open Source Project, Ryan Conrad, Quamotion. All rights reserved.
// </copyright>

namespace SharpAdbClient
{
    using Exceptions;
    using Logs;
    using System;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// <para>
    /// Implements a client for the Android Debug Bridge client-server protocol. Using the client, you
    /// can send messages to and receive messages from the Android Debug Bridge.
    /// </para>
    /// <para>
    /// The <see cref="AdbSocket"/> class implements the raw messaging protocol; that is,
    /// sending and receiving messages. For interacting with the services the Android Debug
    /// Bridge exposes, use the <see cref="AdbClient"/>.
    /// </para>
    /// <para>
    /// For more information about the protocol that is implemented here, see chapter
    /// II Protocol Details, section 1. Client &lt;-&gt;Server protocol at
    /// <see href="https://android.googlesource.com/platform/system/core/+/master/adb/OVERVIEW.TXT"/>.
    /// </para>
    /// </summary>
    public class AdbSocket : IAdbSocket, IDisposable
    {
        /// <summary>
        /// The default time to wait in the milliseconds.
        /// </summary>
        private const int WaitTime = 5;

        private const int Timeout = 5000;

        /// <summary>
        /// Logging tag
        /// </summary>
        private const string TAG = nameof(AdbSocket);

        /// <summary>
        /// The underlying TCP socket that manages the connection with the ADB server.
        /// </summary>
        private ITcpSocket socket;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdbSocket"/> class.
        /// </summary>
        /// <param name="endPoint">
        /// The <see cref="EndPoint"/> at which the Android Debug Bridge is listening
        /// for clients.
        /// </param>
        public AdbSocket(EndPoint endPoint)
        {
            this.socket = new TcpSocket();
            this.socket.Connect(endPoint);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdbSocket"/> class.
        /// </summary>
        /// <param name="socket">
        /// The <see cref="ITcpSocket"/> at which the Android Debug Bridge is listening
        /// for clients.
        /// </param>
        public AdbSocket(ITcpSocket socket)
        {
            this.socket = socket;
        }

        /// <inheritdoc/>
        public bool Connected
        {
            get { return this.socket.Connected; }
        }

        /// <summary>
        /// Determines whether the specified reply is okay.
        /// </summary>
        /// <param name="reply">The reply.</param>
        /// <returns>
        ///   <see langword="true"/> if the specified reply is okay; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsOkay(byte[] reply)
        {
            return AdbClient.Encoding.GetString(reply).Equals("OKAY");
        }

        /// <inheritdoc/>
        public virtual void Reconnect()
        {
            this.socket.Reconnect();
        }

        /// <inheritdoc/>
        public void Close()
        {
            this.socket.Close();
        }

        /// <summary>
        /// Releases all resources used by the current instance of the <see cref="AdbSocket"/>
        /// class.
        /// </summary>
        public virtual void Dispose()
        {
            this.socket.Dispose();
        }

        /// <inheritdoc/>
        public virtual void Read(byte[] data)
        {
            this.Read(data, -1);
        }

        /// <inheritdoc/>
        public virtual void SendSyncRequest(SyncCommand command, string path, int permissions)
        {
            this.SendSyncRequest(command, $"{path},0{permissions}");
        }

        /// <inheritdoc/>
        public virtual void SendSyncRequest(SyncCommand command, string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            this.SendSyncRequest(command, path.Length);

            byte[] pathBytes = AdbClient.Encoding.GetBytes(path);
            this.Write(pathBytes);
        }

        /// <inheritdoc/>
        public virtual void SendSyncRequest(SyncCommand command, int length)
        {
            // The message structure is:
            // First four bytes: command
            // Next four bytes: length of the path
            // Final bytes: path
            byte[] commandBytes = SyncCommandConverter.GetBytes(command);

            byte[] lengthBytes = BitConverter.GetBytes(length);

            if (!BitConverter.IsLittleEndian)
            {
                // Convert from big endian to little endian
                Array.Reverse(lengthBytes);
            }

            this.Write(commandBytes);
            this.Write(lengthBytes);
        }

        /// <inheritdoc/>
        public virtual SyncCommand ReadSyncResponse()
        {
            byte[] data = new byte[4];
            this.Read(data);

            return SyncCommandConverter.GetCommand(data);
        }

        /// <inheritdoc/>
        public virtual string ReadString()
        {
            // The first 4 bytes contain the length of the string
            var reply = new byte[4];
            this.Read(reply);

            // Convert the bytes to a hex string
            string lenHex = AdbClient.Encoding.GetString(reply);
            int len = int.Parse(lenHex, NumberStyles.HexNumber);

            // And get the string
            reply = new byte[len];
            this.Read(reply);

            string value = AdbClient.Encoding.GetString(reply);
            return value;
        }

        /// <inheritdoc/>
        public virtual string ReadSyncString(int length)
        {
            var reply = new byte[length];
            this.Read(reply);

            string value = AdbClient.Encoding.GetString(reply);
            return value;
        }

        /// <inheritdoc/>
        public virtual int ReadSyncLength()
        {
            var reply = new byte[4];
            this.Read(reply);

            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(reply);
            }

            return BitConverter.ToInt32(reply, 0);
        }

        /// <inheritdoc/>
        public virtual AdbResponse ReadAdbResponse()
        {
            var response = this.ReadAdbResponseInner();

            if (!response.IOSuccess || !response.Okay)
            {
                this.socket.Close();
                throw new AdbException($"An error occurred while reading a response from ADB: {response.Message}", response);
            }

            return response;
        }

        /// <inheritdoc/>
        public virtual void SendAdbRequest(string request)
        {
            byte[] data = AdbClient.FormAdbRequest(request);

            if (!this.Write(data))
            {
                throw new IOException($"Failed sending the request '{request}' to ADB");
            }
        }

        /// <inheritdoc/>
        public virtual void Send(byte[] data, int length)
        {
            int numWaits = 0;
            int count = -1;

            try
            {
                count = this.socket.Send(data, 0, length != -1 ? length : data.Length, SocketFlags.None);
                if (count < 0)
                {
                    throw new AdbException("channel EOF");
                }
                else if (count == 0)
                {
                    // TODO: need more accurate timeout?
                    if (Timeout != 0 && numWaits * WaitTime > Timeout)
                    {
                        throw new AdbException("A timeout has occurred while sending the data");
                    }

                    // non-blocking spin
                    Thread.Sleep(WaitTime);
                    numWaits++;
                }
                else
                {
                    numWaits = 0;
                }
            }
            catch (SocketException sex)
            {
                Log.Error(TAG, sex);
                throw;
            }
        }

        /// <inheritdoc/>
        public virtual void Read(byte[] data, int length)
        {
            int expLen = length != -1 ? length : data.Length;
            int count = -1;
            int totalRead = 0;

            while (count != 0 && totalRead < expLen)
            {
                try
                {
                    int left = expLen - totalRead;
                    int buflen = left < this.socket.ReceiveBufferSize ? left : this.socket.ReceiveBufferSize;

                    byte[] buffer = new byte[buflen];
                    this.socket.ReceiveBufferSize = expLen;
                    count = this.socket.Receive(buffer, buflen, SocketFlags.None);
                    if (count < 0)
                    {
                        Log.Error(TAG, "read: channel EOF");
                        throw new AdbException("EOF");
                    }
                    else if (count == 0)
                    {
                        Log.Info(TAG, "DONE with Read");
                    }
                    else
                    {
                        Array.Copy(buffer, 0, data, totalRead, count);
                        totalRead += count;
                    }
                }
                catch (SocketException sex)
                {
                    throw new AdbException(string.Format("No Data to read: {0}", sex.Message));
                }
            }
        }

        /// <inheritdoc/>
        public Stream GetShellStream()
        {
            var stream = this.socket.GetStream();
            return new ShellStream(stream, closeStream: true);
        }

        /// <summary>
        /// Write until all data in "data" is written or the connection fails or times out.
        /// </summary>
        /// <param name="data">The data to send.</param>
        /// <returns>
        /// Returns <see langword="true"/> if all data was written; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// This uses the default time out value.
        /// </remarks>
        protected bool Write(byte[] data)
        {
            try
            {
                this.Send(data, -1);
            }
            catch (IOException e)
            {
                Log.Error(TAG, e);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Reads the response from ADB after a command.
        /// </summary>
        /// <returns>
        /// A <see cref="AdbResponse"/> that represents the response received from ADB.
        /// </returns>
        protected AdbResponse ReadAdbResponseInner()
        {
            AdbResponse resp = new AdbResponse();

            byte[] reply = new byte[4];
            this.Read(reply);

            resp.IOSuccess = true;

            if (IsOkay(reply))
            {
                resp.Okay = true;
            }
            else
            {
                resp.Okay = false;
            }

            if (!resp.Okay)
            {
                var message = this.ReadString();
                resp.Message = message;
                Log.Error(TAG, "Got reply '{0}', diag='{1}'", this.ReplyToString(reply), resp.Message);
            }

            return resp;
        }

        /// <summary>
        /// Converts an ADB reply to a string.
        /// </summary>
        /// <param name="reply">
        /// A <see cref="byte"/> array that represents the ADB reply.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that represents the ADB reply.
        /// </returns>
        protected string ReplyToString(byte[] reply)
        {
            string result;
            try
            {
                result = Encoding.Default.GetString(reply);
            }
            catch (DecoderFallbackException uee)
            {
                Log.Error(TAG, uee);
                result = string.Empty;
            }

            return result;
        }
    }
}
