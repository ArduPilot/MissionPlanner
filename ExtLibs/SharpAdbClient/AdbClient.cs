// <copyright file="AdbClient.cs" company="The Android Open Source Project, Ryan Conrad, Quamotion">
// Copyright (c) The Android Open Source Project, Ryan Conrad, Quamotion. All rights reserved.
// </copyright>

namespace SharpAdbClient
{
    using SharpAdbClient.Exceptions;
    using SharpAdbClient.Logs;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// <para>
    ///     Implements the <see cref="IAdbClient"/> interface, and allows you to interact with the
    ///     adb server and devices that are connected to that adb server.
    /// </para>
    /// <para>
    ///     For example, to fetch a list of all devices that are currently connected to this PC, you can
    ///     call the <see cref="GetDevices"/> method.
    /// </para>
    /// <para>
    ///     To run a command on a device, you can use the <see cref="ExecuteRemoteCommand(string, DeviceData, IShellOutputReceiver, CancellationToken, int)"/>
    ///     method.
    /// </para>
    /// </summary>
    /// <seealso href="https://github.com/android/platform_system_core/blob/master/adb/SERVICES.TXT">SERVICES.TXT</seealso>
    /// <seealso href="https://github.com/android/platform_system_core/blob/master/adb/adb_client.c">adb_client.c</seealso>
    /// <seealso href="https://github.com/android/platform_system_core/blob/master/adb/adb.c">adb.c</seealso>
    public class AdbClient : IAdbClient
    {
        /// <summary>
        /// The default encoding
        /// </summary>
        public const string DefaultEncoding = "ISO-8859-1";

        /// <summary>
        /// The default port to use when connecting to a device over TCP/IP.
        /// </summary>
        public const int DefaultPort = 5555;

        /// <summary>
        /// Logging tag
        /// </summary>
        private const string Tag = nameof(AdbClient);

        /// <summary>
        /// The default time to wait in the milliseconds.
        /// </summary>
        private const int WaitTime = 5;

        /// <summary>
        /// The singleton instance of the <see cref="AdbClient"/> class.
        /// </summary>
        private static IAdbClient instance = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdbClient"/> class.
        /// </summary>
        /// <param name="endPoint">
        /// The <see cref="EndPoint"/> at which the adb server is listening.
        /// </param>
        public AdbClient(EndPoint endPoint)
        {
            if (endPoint == null)
            {
                throw new ArgumentNullException();
            }

            this.EndPoint = endPoint;
        }

        /// <summary>
        /// Gets the encoding used when communicating with adb.
        /// </summary>
        public static Encoding Encoding
        { get; } = Encoding.GetEncoding(DefaultEncoding);

        /// <summary>
        /// Gets or sets the current global instance of the <see cref="IAdbClient"/> interface.
        /// </summary>
        public static IAdbClient Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AdbClient(AdbServer.Instance.EndPoint);
                }

                return instance;
            }

            set
            {
                instance = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="EndPoint"/> at which the adb server is listening.
        /// </summary>
        public EndPoint EndPoint
        {
            get;
            private set;
        }

        /// <summary>
        /// Create an ASCII string preceded by four hex digits. The opening "####"
        /// is the length of the rest of the string, encoded as ASCII hex(case
        /// doesn't matter).
        /// </summary>
        /// <param name="req">The request to form.
        /// </param>
        /// <returns>
        /// An array containing <c>####req</c>.
        /// </returns>
        public static byte[] FormAdbRequest(string req)
        {
            string resultStr = string.Format("{0}{1}\n", req.Length.ToString("X4"), req);
            byte[] result = Encoding.GetBytes(resultStr);
            return result;
        }

        /// <summary>
        /// Creates the adb forward request.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="port">The port.</param>
        /// <returns>
        /// This returns an array containing <c>"####tcp:{port}:{addStr}"</c>.
        /// </returns>
        public static byte[] CreateAdbForwardRequest(string address, int port)
        {
            string request;

            if (address == null)
            {
                request = "tcp:" + port;
            }
            else
            {
                request = "tcp:" + port + ":" + address;
            }

            return FormAdbRequest(request);
        }

        /// <inheritdoc/>
        public int GetAdbVersion()
        {
            using (var socket = Factories.AdbSocketFactory(this.EndPoint))
            {
                socket.SendAdbRequest("host:version");
                var response = socket.ReadAdbResponse();
                var version = socket.ReadString();

                return int.Parse(version, NumberStyles.HexNumber);
            }
        }

        /// <inheritdoc/>
        public void KillAdb()
        {
            using (IAdbSocket socket = Factories.AdbSocketFactory(this.EndPoint))
            {
                socket.SendAdbRequest("host:kill");

                // The host will immediately close the connection after the kill
                // command has been sent; no need to read the response.
            }
        }

        /// <inheritdoc/>
        public List<DeviceData> GetDevices()
        {
            using (IAdbSocket socket = Factories.AdbSocketFactory(this.EndPoint))
            {
                socket.SendAdbRequest("host:devices-l");
                socket.ReadAdbResponse();
                var reply = socket.ReadString();

                string[] data = reply.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

                return data.Select(d => DeviceData.CreateFromAdbData(d)).ToList();
            }
        }

        /// <inheritdoc/>
        public void SetDevice(IAdbSocket socket, DeviceData device)
        {
            // if the device is not null, then we first tell adb we're looking to talk
            // to a specific device
            if (device != null)
            {
                socket.SendAdbRequest($"host:transport:{device.Serial}");

                try
                {
                    var response = socket.ReadAdbResponse();
                }
                catch (AdbException e)
                {
                    if (string.Equals("device not found", e.AdbError, StringComparison.OrdinalIgnoreCase))
                    {
                        throw new DeviceNotFoundException(device.Serial);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void CreateForward(DeviceData device, string local, string remote, bool allowRebind)
        {
            this.EnsureDevice(device);

            using (IAdbSocket socket = Factories.AdbSocketFactory(this.EndPoint))
            {
                string rebind = allowRebind ? string.Empty : "norebind:";

                socket.SendAdbRequest($"host-serial:{device.Serial}:forward:{rebind}{local};{remote}");
                var response = socket.ReadAdbResponse();
            }
        }

        /// <inheritdoc/>
        public void CreateForward(DeviceData device, ForwardSpec local, ForwardSpec remote, bool allowRebind)
        {
            this.CreateForward(device, local?.ToString(), remote?.ToString(), allowRebind);
        }

        /// <inheritdoc/>
        public void RemoveForward(DeviceData device, int localPort)
        {
            this.EnsureDevice(device);

            using (IAdbSocket socket = Factories.AdbSocketFactory(this.EndPoint))
            {
                socket.SendAdbRequest($"host-serial:{device.Serial}:killforward:tcp:{localPort}");
                var response = socket.ReadAdbResponse();
            }
        }

        /// <inheritdoc/>
        public void RemoveAllForwards(DeviceData device)
        {
            this.EnsureDevice(device);

            using (IAdbSocket socket = Factories.AdbSocketFactory(this.EndPoint))
            {
                socket.SendAdbRequest($"host-serial:{device.Serial}:killforward-all");
                var response = socket.ReadAdbResponse();
            }
        }

        /// <inheritdoc/>
        public IEnumerable<ForwardData> ListForward(DeviceData device)
        {
            this.EnsureDevice(device);

            using (IAdbSocket socket = Factories.AdbSocketFactory(this.EndPoint))
            {
                socket.SendAdbRequest($"host-serial:{device.Serial}:list-forward");
                var response = socket.ReadAdbResponse();

                var data = socket.ReadString();

                var parts = data.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                return parts.Select(p => ForwardData.FromString(p));
            }
        }

        /// <inheritdoc/>
        public void ExecuteRemoteCommand(string command, DeviceData device, IShellOutputReceiver receiver, CancellationToken cancellationToken, int maxTimeToOutputResponse)
        {
            this.EnsureDevice(device);

            using (IAdbSocket socket = Factories.AdbSocketFactory(this.EndPoint))
            {
                cancellationToken.Register(() => socket.Close());

                this.SetDevice(socket, device);
                socket.SendAdbRequest($"shell:{command}");
                var response = socket.ReadAdbResponse();

                try
                {
                    using (StreamReader reader = new StreamReader(socket.GetShellStream(), Encoding))
                    {
                        // Previously, we would loop while reader.Peek() >= 0. Turns out that this would
                        // break too soon in certain cases (about every 10 loops, so it appears to be a timing
                        // issue). Checking for reader.ReadLine() to return null appears to be much more robust
                        // -- one of the integration test fetches output 1000 times and found no truncations.
                        while (!cancellationToken.IsCancellationRequested)
                        {
                            var line = reader.ReadLine();

                            if (line == null)
                            {
                                break;
                            }

                            if (receiver != null)
                            {
                                receiver.AddOutput(line);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    // If a cancellation was requested, this main loop is interrupted with an exception
                    // because the socket is closed. In that case, we don't need to throw a ShellCommandUnresponsiveException.
                    // In all other cases, something went wrong, and we want to report it to the user.
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        throw new ShellCommandUnresponsiveException(e);
                    }
                }
                finally
                {
                    if (receiver != null)
                    {
                        receiver.Flush();
                    }
                }
            }
        }

        /// <inheritdoc/>
        public IEnumerable<LogEntry> RunLogService(DeviceData device, params LogId[] logNames)
        {
            this.EnsureDevice(device);

            // The 'log' service has been deprecated, see
            // https://android.googlesource.com/platform/system/core/+/7aa39a7b199bb9803d3fd47246ee9530b4a96177
            using (IAdbSocket socket = Factories.AdbSocketFactory(this.EndPoint))
            {
                this.SetDevice(socket, device);

                StringBuilder request = new StringBuilder();
                request.Append("shell:logcat -B");

                foreach (var logName in logNames)
                {
                    request.Append($" -b {logName.ToString().ToLower()}");
                }

                socket.SendAdbRequest(request.ToString());
                var response = socket.ReadAdbResponse();

                using (Stream stream = socket.GetShellStream())
                using (LogReader reader = new LogReader(stream))
                {
                    while (true)
                    {
                        LogEntry entry = null;

                        try
                        {
                            entry = reader.ReadEntry();
                        }
                        catch (EndOfStreamException)
                        {
                            // This indicates the end of the stream; the entry will remain null.
                        }

                        if (entry != null)
                        {
                            yield return entry;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void Reboot(string into, DeviceData device)
        {
            this.EnsureDevice(device);

            var request = $"reboot:{into}";

            using (IAdbSocket socket = Factories.AdbSocketFactory(this.EndPoint))
            {
                this.SetDevice(socket, device);
                socket.SendAdbRequest(request);
                var response = socket.ReadAdbResponse();
            }
        }

        /// <inheritdoc/>
        public string Connect(DnsEndPoint endpoint)
        {
            if (endpoint == null)
            {
                throw new ArgumentNullException(nameof(endpoint));
            }

            using (IAdbSocket socket = Factories.AdbSocketFactory(this.EndPoint))
            {
                socket.SendAdbRequest($"host:connect:{endpoint.Host}:{endpoint.Port}");
                var response = socket.ReadAdbResponse();

                return socket.ReadString();
            }
        }

        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> if the <paramref name="device"/>
        /// parameter is <see langword="null"/>, and a <see cref="ArgumentOutOfRangeException"/>
        /// if <paramref name="device"/> does not have a valid serial number.
        /// </summary>
        /// <param name="device">
        /// A <see cref="DeviceData"/> object to validate.
        /// </param>
        protected void EnsureDevice(DeviceData device)
        {
            if (device == null)
            {
                throw new ArgumentNullException(nameof(device));
            }

            if (string.IsNullOrEmpty(device.Serial))
            {
                throw new ArgumentOutOfRangeException(nameof(device), "You must specific a serial number for the device");
            }
        }
    }
}
