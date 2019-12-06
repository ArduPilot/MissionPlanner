// <copyright file="SyncService.cs" company="The Android Open Source Project, Ryan Conrad, Quamotion">
// Copyright (c) The Android Open Source Project, Ryan Conrad, Quamotion. All rights reserved.
// </copyright>

namespace SharpAdbClient
{
    using Exceptions;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Threading;

    /// <summary>
    /// <para>
    ///     Provides access to the sync service running on the Android device. Allows you to
    ///     list, download and upload files on the device.
    /// </para>
    /// </summary>
    /// <example>
    /// <para>
    ///     To send files to or receive files from your Android device, you can use the following code:
    /// </para>
    /// <code>
    /// void DownloadFile()
    /// {
    ///     var device = AdbClient.Instance.GetDevices().First();
    ///
    ///     using (SyncService service = new SyncService(new AdbSocket(), device))
    ///     using (Stream stream = File.OpenWrite(@"C:\MyFile.txt"))
    ///     {
    ///         service.Pull("/data/MyFile.txt", stream, null, CancellationToken.None);
    ///     }
    /// }
    ///
    ///     void UploadFile()
    /// {
    ///     var device = AdbClient.Instance.GetDevices().First();
    ///
    ///     using (SyncService service = new SyncService(new AdbSocket(), device))
    ///     using (Stream stream = File.OpenRead(@"C:\MyFile.txt"))
    ///     {
    ///         service.Push(stream, "/data/MyFile.txt", null, CancellationToken.None);
    ///     }
    /// }
    /// </code>
    /// </example>
    public class SyncService : ISyncService, IDisposable
    {
        /// <summary>
        /// Logging tag
        /// </summary>
        private const string Tag = nameof(SyncService);

        /// <summary>
        /// The maximum size of data to transfer between the device and the PC
        /// in one block.
        /// </summary>
        private const int MaxBufferSize = 64 * 1024;

        /// <summary>
        /// The maximum length of a path on the remote device.
        /// </summary>
        private const int MaxPathLength = 1024;

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncService"/> class.
        /// </summary>
        /// <param name="device">
        /// The device on which to interact with the files.
        /// </param>
        public SyncService(DeviceData device)
            : this(Factories.AdbSocketFactory(AdbServer.Instance.EndPoint), device)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncService"/> class.
        /// </summary>
        /// <param name="socket">
        /// A <see cref="IAdbSocket"/> that enables to connection with the
        /// adb server.
        /// </param>
        /// <param name="device">
        /// The device on which to interact with the files.
        /// </param>
        public SyncService(IAdbSocket socket, DeviceData device)
        {
            this.Socket = socket;
            this.Device = device;

            this.Open();
        }

        /// <summary>
        /// Gets the device on which the file operations are being executed.
        /// </summary>
        public DeviceData Device
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the <see cref="IAdbSocket"/> that enables the connection with the
        /// adb server.
        /// </summary>
        public IAdbSocket Socket { get; private set; }

        /// <include file='.\ISyncService.xml' path='/SyncService/IsOpen/*'/>
        public bool IsOpen
        {
            get
            {
                return this.Socket != null && this.Socket.Connected;
            }
        }

        /// <include file='.\ISyncService.xml' path='/SyncService/Open/*'/>
        public void Open()
        {
            // target a specific device
            AdbClient.Instance.SetDevice(this.Socket, this.Device);

            this.Socket.SendAdbRequest("sync:");
            var resp = this.Socket.ReadAdbResponse();
        }

        /// <include file='.\ISyncService.xml' path='/SyncService/Push/*'/>
        public void Push(Stream stream, string remotePath, int permissions, DateTime timestamp, CancellationToken cancellationToken)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (remotePath == null)
            {
                throw new ArgumentNullException(nameof(remotePath));
            }

            if (remotePath.Length > MaxPathLength)
            {
                throw new ArgumentOutOfRangeException(nameof(remotePath), $"The remote path {remotePath} exceeds the maximum path size {MaxPathLength}");
            }

            this.Socket.SendSyncRequest(SyncCommand.SEND, remotePath, permissions);

            // create the buffer used to read.
            // we read max SYNC_DATA_MAX.
            byte[] buffer = new byte[MaxBufferSize];

            // look while there is something to read
            while (true)
            {
                // check if we're canceled
                cancellationToken.ThrowIfCancellationRequested();

                // read up to SYNC_DATA_MAX
                int read = stream.Read(buffer, 0, MaxBufferSize);

                if (read == 0)
                {
                    // we reached the end of the file
                    break;
                }

                // now send the data to the device
                // first write the amount read
                this.Socket.SendSyncRequest(SyncCommand.DATA, read);
                this.Socket.Send(buffer, read);
            }

            // create the DONE message
            int time = (int)timestamp.ToUnixEpoch();
            this.Socket.SendSyncRequest(SyncCommand.DONE, time);

            // read the result, in a byte array containing 2 ints
            // (id, size)
            var result = this.Socket.ReadSyncResponse();
            var length = this.Socket.ReadSyncLength();

            if (result == SyncCommand.FAIL)
            {
                var message = this.Socket.ReadSyncString(length);

                throw new AdbException(message);
            }
            else if (result != SyncCommand.OKAY)
            {
                throw new AdbException($"The server sent an invali repsonse {result}");
            }
        }

        /// <include file='.\ISyncService.xml' path='/SyncService/PullFile2/*'/>
        public void Pull(string remoteFilepath, Stream stream, CancellationToken cancellationToken)
        {
            if (remoteFilepath == null)
            {
                throw new ArgumentNullException(nameof(remoteFilepath));
            }

            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            byte[] buffer = new byte[MaxBufferSize];

            this.Socket.SendSyncRequest(SyncCommand.RECV, remoteFilepath);

            while (true)
            {
                var response = this.Socket.ReadSyncResponse();
                var length = this.Socket.ReadSyncLength();
                cancellationToken.ThrowIfCancellationRequested();

                if (response == SyncCommand.DONE)
                {
                    break;
                }
                else if (response == SyncCommand.FAIL)
                {
                    var message = this.Socket.ReadSyncString(length);
                    throw new AdbException($"Failed to pull '{remoteFilepath}'. {message}");
                }
                else if (response != SyncCommand.DATA)
                {
                    throw new AdbException($"The server sent an invalid response {response}");
                }

                if (length > MaxBufferSize)
                {
                    throw new AdbException($"The adb server is sending {length} bytes of data, which exceeds the maximum chunk size {MaxBufferSize}");
                }

                // now read the length we received
                this.Socket.Read(buffer, length);
                stream.Write(buffer, 0, length);
            }
        }

        /// <include file='.\ISyncService.xml' path='/SyncService/Stat/*'/>
        public FileStatistics Stat(string remotePath)
        {
            // create the stat request message.
            this.Socket.SendSyncRequest(SyncCommand.STAT, remotePath);

            if (this.Socket.ReadSyncResponse() != SyncCommand.STAT)
            {
                throw new AdbException($"The server returned an invalid sync response.");
            }

            // read the result, in a byte array containing 3 ints
            // (mode, size, time)
            FileStatistics value = new FileStatistics();
            value.Path = remotePath;

            this.ReadStatistics(value);

            return value;
        }

        /// <include file='.\ISyncService.xml' path='/SyncService/GetDirectoryListing/*'/>
        public IEnumerable<FileStatistics> GetDirectoryListing(string remotePath)
        {
            Collection<FileStatistics> value = new Collection<FileStatistics>();

            // create the stat request message.
            this.Socket.SendSyncRequest(SyncCommand.LIST, remotePath);

            while (true)
            {
                var response = this.Socket.ReadSyncResponse();
                FileStatistics entry = new FileStatistics();
                this.ReadStatistics(entry);

                var length = this.Socket.ReadSyncLength();

                if (response == SyncCommand.DONE)
                {
                    break;
                }
                else if (response != SyncCommand.DENT)
                {
                    throw new AdbException($"The server returned an invalid sync response.");
                }

                entry.Path = this.Socket.ReadSyncString(length);

                value.Add(entry);
            }

            return value;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (this.Socket != null)
            {
                this.Socket.Dispose();
                this.Socket = null;
            }
        }

        private void ReadStatistics(FileStatistics value)
        {
            byte[] statResult = new byte[12];
            this.Socket.Read(statResult);

            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(statResult, 0, 4);
                Array.Reverse(statResult, 4, 4);
                Array.Reverse(statResult, 8, 4);
            }

            value.FileMode = (UnixFileMode)BitConverter.ToInt32(statResult, 0);
            value.Size = BitConverter.ToInt32(statResult, 4);
            value.Time = DateTimeHelper.Epoch.AddSeconds(BitConverter.ToInt32(statResult, 8)).ToLocalTime();
        }
    }
}
