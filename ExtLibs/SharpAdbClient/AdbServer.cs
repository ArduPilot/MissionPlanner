// <copyright file="AdbServer.cs" company="The Android Open Source Project, Ryan Conrad, Quamotion">
// Copyright (c) The Android Open Source Project, Ryan Conrad, Quamotion. All rights reserved.
// </copyright>

namespace SharpAdbClient
{
    using Mono.Unix;
    using SharpAdbClient.Exceptions;
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;

    /// <summary>
    /// <para>
    /// Provides methods for interacting with the adb server. The adb server must be running for
    /// the rest of the <c>Managed.Adb</c> library to work.
    /// </para>
    /// <para>
    /// The adb server is a background process
    /// that runs on the host machine. Its purpose if to sense the USB ports to know when devices are
    /// attached/removed, as well as when emulator instances start/stop. The ADB server is really one
    /// giant multiplexing loop whose purpose is to orchestrate the exchange of data
    /// between clients and devices.
    /// </para>
    /// </summary>
    public class AdbServer : IAdbServer
    {
        /// <summary>
        /// The port at which the Android Debug Bridge server listens by default.
        /// </summary>
        public const int AdbServerPort = 5037;

        /// <summary>
        /// The minum version of <c>adb.exe</c> that is supported by this library.
        /// </summary>
        public static readonly Version RequiredAdbVersion = new Version(1, 0, 20);

        /// <summary>
        /// The error code that is returned by the <see cref="SocketException"/> when the connection is refused.
        /// </summary>
        /// <remarks>
        /// No connection could be made because the target computer actively refused it.This usually
        /// results from trying to connect to a service that is inactive on the foreign host—that is,
        ///  one with no server application running.
        /// </remarks>
        /// <seealso href="https://msdn.microsoft.com/en-us/library/ms740668.aspx"/>
        internal const int ConnectionRefused = 10061;

        /// <summary>
        /// The error code that is returned by the <see cref="SocketException"/> when the connection was reset by the peer.
        /// </summary>
        /// <remarks>
        /// An existing connection was forcibly closed by the remote host. This normally results if the peer application on the
        /// remote host is suddenly stopped, the host is rebooted, the host or remote network interface is disabled, or the remote
        /// host uses a hard close. This error may also result if a connection was broken due to keep-alive activity detecting
        /// a failure while one or more operations are in progress.
        /// </remarks>
        /// <seealso href="https://msdn.microsoft.com/en-us/library/ms740668.aspx"/>
        internal const int ConnectionReset = 10054;

        /// <summary>
        /// The tag to use when logging.
        /// </summary>
        private const string Tag = nameof(AdbServer);

        /// <summary>
        /// A lock used to ensure only one caller at a time can attempt to restart adb.
        /// </summary>
        private static readonly object RestartLock = new object();

        /// <summary>
        /// The path to the adb server. Cached from calls to <see cref="StartServer(string, bool)"/>. Used when restarting
        /// the server to figure out where adb is located.
        /// </summary>
        private static string cachedAdbPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdbServer"/> class.
        /// </summary>
        public AdbServer()
            : this(new IPEndPoint(IPAddress.Loopback, AdbServerPort))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdbServer"/> class, and the specific <see cref="EndPoint"/> at which
        /// the server should be listening.
        /// </summary>
        /// <param name="endPoint">
        /// The <see cref="EndPoint"/> at which the server should be listening.
        /// </param>
        public AdbServer(EndPoint endPoint)
        {
            if (endPoint == null)
            {
                throw new ArgumentNullException(nameof(endPoint));
            }

            if (!(endPoint is IPEndPoint || endPoint is DnsEndPoint || endPoint is UnixEndPoint))
            {
                throw new NotSupportedException("Only TCP and Unix endpoints are supported");
            }

            this.EndPoint = endPoint;
        }

        /// <summary>
        /// Gets or sets the default instance of the <see cref="IAdbServer"/> interface.
        /// </summary>
        public static IAdbServer Instance
        { get; set; } = new AdbServer();

        /// <inheritdoc/>
        public EndPoint EndPoint { get; private set; }

        /// <inheritdoc/>
        public StartServerResult StartServer(string adbPath, bool restartServerIfNewer)
        {
            var serverStatus = this.GetStatus();
            Version commandLineVersion = null;

            var commandLineClient = Factories.AdbCommandLineClientFactory(adbPath);

            if (commandLineClient.IsValidAdbFile(adbPath))
            {
                cachedAdbPath = adbPath;
                commandLineVersion = commandLineClient.GetVersion();
            }

            // If the server is running, and no adb path is provided, check if we have the minimum
            // version
            if (adbPath == null)
            {
                if (!serverStatus.IsRunning)
                {
                    throw new AdbException("The adb server is not running, but no valid path to the adb.exe executable was provided. The adb server cannot be started.");
                }

                if (serverStatus.Version >= RequiredAdbVersion)
                {
                    return StartServerResult.AlreadyRunning;
                }
                else
                {
                    throw new AdbException($"The adb deamon is running an outdated version ${commandLineVersion}, but not valid path to the adb.exe executable was provided. A more recent version of the adb server cannot be started.");
                }
            }

            if (serverStatus.IsRunning
                && ((serverStatus.Version < RequiredAdbVersion)
                     || ((serverStatus.Version < commandLineVersion) && restartServerIfNewer)))
            {
                if (adbPath == null)
                {
                    throw new ArgumentNullException(nameof(adbPath));
                }

                AdbClient.Instance.KillAdb();
                serverStatus.IsRunning = false;
                serverStatus.Version = null;

                commandLineClient.StartServer();
                return StartServerResult.RestartedOutdatedDaemon;
            }
            else if (!serverStatus.IsRunning)
            {
                if (adbPath == null)
                {
                    throw new ArgumentNullException(nameof(adbPath));
                }

                commandLineClient.StartServer();
                return StartServerResult.Started;
            }
            else
            {
                return StartServerResult.AlreadyRunning;
            }
        }

        /// <inheritdoc/>
        public void RestartServer()
        {
            if (!File.Exists(cachedAdbPath))
            {
                throw new InvalidOperationException($"The adb server was not started via {nameof(AdbServer)}.{nameof(this.StartServer)} or no path to adb was specified. The adb server cannot be restarted.");
            }

            lock (RestartLock)
            {
                this.StartServer(cachedAdbPath, false);
            }
        }

        /// <inheritdoc/>
        public AdbServerStatus GetStatus()
        {
            // Try to connect to a running instance of the adb server
            try
            {
                int versionCode = AdbClient.Instance.GetAdbVersion();

                return new AdbServerStatus()
                {
                    IsRunning = true,
                    Version = new Version(1, 0, versionCode)
                };
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode == SocketError.ConnectionRefused)
                {
                    return new AdbServerStatus()
                    {
                        IsRunning = false,
                        Version = null
                    };
                }
                else
                {
                    // An unexpected exception occurred; re-throw the exception
                    throw;
                }
            }
        }
    }
}
