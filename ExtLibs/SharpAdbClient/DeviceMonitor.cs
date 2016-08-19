// <copyright file="DeviceMonitor.cs" company="The Android Open Source Project, Ryan Conrad, Quamotion">
// Copyright (c) The Android Open Source Project, Ryan Conrad, Quamotion. All rights reserved.
// </copyright>

namespace SharpAdbClient
{
    using Exceptions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// <para>
    ///     A Device monitor. This connects to the Android Debug Bridge and get device and
    ///     debuggable process information from it.
    /// </para>
    /// </summary>
    /// <example>
    /// <para>
    ///     To receive notifications when devices connect to or disconnect from your PC, you can use the following code:
    /// </para>
    /// <code>
    /// void Test()
    /// {
    ///     var monitor = new DeviceMonitor(new AdbSocket());
    ///     monitor.DeviceConnected += this.OnDeviceConnected;
    ///     monitor.Start();
    /// }
    ///
    /// void OnDeviceConnected(object sender, DeviceDataEventArgs e)
    /// {
    ///     Console.WriteLine($"The device {e.Device.Name} has connected to this PC");
    /// }
    /// </code>
    /// </example>
    public class DeviceMonitor : IDeviceMonitor, IDisposable
    {
        /// <summary>
        /// Logging tag
        /// </summary>
        private const string Tag = nameof(DeviceMonitor);

        /// <summary>
        /// The list of devices currently connected to the Android Debug Bridge.
        /// </summary>
        private readonly List<DeviceData> devices;

        /// <summary>
        /// When the <see cref="Start"/> method is called, this <see cref="ManualResetEvent"/>
        /// is used to block the <see cref="Start"/> method until the <see cref="DeviceMonitorLoopAsync"/>
        /// has processed the first list of devices.
        /// </summary>
        private readonly ManualResetEvent firstDeviceListParsed = new ManualResetEvent(false);

        /// <summary>
        /// A <see cref="CancellationToken"/> that can be used to cancel the <see cref="monitorTask"/>.
        /// </summary>
        private readonly CancellationTokenSource monitorTaskCancellationTokenSource = new CancellationTokenSource();

        /// <summary>
        /// The <see cref="Task"/> that monitors the <see cref="Socket"/> and waits for device notifications.
        /// </summary>
        private Task monitorTask;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceMonitor"/> class.
        /// </summary>
        /// <param name="socket">
        /// The <see cref="IAdbSocket"/> that manages the connection with the adb server.
        /// </param>
        public DeviceMonitor(IAdbSocket socket)
        {
            if (socket == null)
            {
                throw new ArgumentNullException(nameof(socket));
            }

            this.Socket = socket;
            this.devices = new List<DeviceData>();
            this.Devices = this.devices.AsReadOnly();
        }

        /// <include file='IDeviceMonitor.xml' path='/IDeviceMonitor/DeviceChanged/*'/>
        public event EventHandler<DeviceDataEventArgs> DeviceChanged;

        /// <include file='IDeviceMonitor.xml' path='/IDeviceMonitor/DeviceConnected/*'/>
        public event EventHandler<DeviceDataEventArgs> DeviceConnected;

        /// <include file='IDeviceMonitor.xml' path='/IDeviceMonitor/DeviceDisconnected/*'/>
        public event EventHandler<DeviceDataEventArgs> DeviceDisconnected;

        /// <include file='IDeviceMonitor.xml' path='/IDeviceMonitor/Devices/*'/>
        public IReadOnlyCollection<DeviceData> Devices { get; private set; }

        /// <summary>
        /// Gets the <see cref="IAdbSocket"/> that represents the connection to the
        /// Android Debug Bridge.
        /// </summary>
        public IAdbSocket Socket { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is running.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if this instance is running; otherwise, <see langword="false"/>.
        /// </value>
        public bool IsRunning { get; private set; }

        /// <include file='IDeviceMonitor.xml' path='/IDeviceMonitor/Start/*'/>
        public void Start()
        {
            if (this.monitorTask == null)
            {
                this.firstDeviceListParsed.Reset();

                this.monitorTask = Task.Run(() => this.DeviceMonitorLoopAsync(this.monitorTaskCancellationTokenSource.Token));

                // Wait for the worker thread to have read the first list
                // of devices.
                this.firstDeviceListParsed.WaitOne();
            }
        }

        /// <summary>
        /// Stops the monitoring
        /// </summary>
        public void Dispose()
        {
            // First kill the monitor task, which has a dependency on the socket,
            // then close the socket.
            if (this.monitorTask != null)
            {
                this.IsRunning = false;

                // Stop the thread. The tread will keep waiting for updated information from adb
                // eternally, so we need to forcefully abort it here.
                this.monitorTaskCancellationTokenSource.Cancel();
                this.monitorTask.Wait();

                this.monitorTask = null;
            }

            // Close the connection to adb. To be done after the monitor task exited.
            if (this.Socket != null)
            {
                this.Socket.Dispose();
                this.Socket = null;
            }
        }

        /// <summary>
        /// Raises the <see cref="DeviceChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="DeviceDataEventArgs"/> instance containing the event data.</param>
        protected void OnDeviceChanged(DeviceDataEventArgs e)
        {
            if (this.DeviceChanged != null)
            {
                this.DeviceChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="DeviceConnected"/> event.
        /// </summary>
        /// <param name="e">The <see cref="DeviceDataEventArgs"/> instance containing the event data.</param>
        protected void OnDeviceConnected(DeviceDataEventArgs e)
        {
            if (this.DeviceConnected != null)
            {
                this.DeviceConnected(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="DeviceDisconnected"/> event.
        /// </summary>
        /// <param name="e">The <see cref="DeviceDataEventArgs"/> instance containing the event data.</param>
        protected void OnDeviceDisconnected(DeviceDataEventArgs e)
        {
            if (this.DeviceDisconnected != null)
            {
                this.DeviceDisconnected(this, e);
            }
        }

        /// <summary>
        /// Monitors the devices. This connects to the Debug Bridge
        /// </summary>
        private async Task DeviceMonitorLoopAsync(CancellationToken cancellationToken)
        {
            this.IsRunning = true;

            // Set up the connection to track the list of devices.
            this.InitializeSocket();

            do
            {
                try
                {
                    var value = await this.Socket.ReadStringAsync(cancellationToken).ConfigureAwait(false);
                    this.ProcessIncomingDeviceData(value);

                    this.firstDeviceListParsed.Set();
                }
                catch (TaskCanceledException ex)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        // The DeviceMonitor is shutting down (disposing) and Dispose()
                        // has called cancellationToken.Cancel(). This exception is expected,
                        // so we can safely swallow it.
                    }
                    else
                    {
                        // The exception was unexpected, so log it.
                        Log.Error(Tag, ex);
                    }
                }
                catch (AdbException adbException)
                {
                    if (adbException.ConnectionReset)
                    {
                        // The adb server was killed, for whatever reason. Try to restart it and recover from this.
                        AdbServer.Instance.RestartServer();
                        this.Socket.Reconnect();
                        this.InitializeSocket();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(Tag, ex);
                    throw;
                }
            }
            while (!cancellationToken.IsCancellationRequested);
        }

        private void InitializeSocket()
        {
            // Set up the connection to track the list of devices.
            this.Socket.SendAdbRequest("host:track-devices");
            this.Socket.ReadAdbResponse();
        }

        /// <summary>
        /// Processes the incoming device data.
        /// </summary>
        private void ProcessIncomingDeviceData(string result)
        {
            List<DeviceData> list = new List<DeviceData>();

            string[] deviceValues = result.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            List<DeviceData> devices = deviceValues.Select(d => DeviceData.CreateFromAdbData(d)).ToList();
            this.UpdateDevices(devices);
        }

        private void UpdateDevices(List<DeviceData> devices)
        {
            lock (this.devices)
            {
                // For each device in the current list, we look for a matching the new list.
                // * if we find it, we update the current object with whatever new information
                //   there is
                //   (mostly state change, if the device becomes ready, we query for build info).
                //   We also remove the device from the new list to mark it as "processed"
                // * if we do not find it, we remove it from the current list.
                // Once this is done, the new list contains device we aren't monitoring yet, so we
                // add them to the list, and start monitoring them.

                // Add or update existing devices
                foreach (var device in devices)
                {
                    var existingDevice = this.Devices.SingleOrDefault(d => d.Serial == device.Serial);

                    if (existingDevice == null)
                    {
                        this.devices.Add(device);
                        this.OnDeviceConnected(new DeviceDataEventArgs(device));
                    }
                    else
                    {
                        existingDevice.State = device.State;
                        this.OnDeviceChanged(new DeviceDataEventArgs(existingDevice));
                    }
                }

                // Remove devices
                foreach (var device in this.Devices.Where(d => !devices.Any(e => e.Serial == d.Serial)).ToArray())
                {
                    this.devices.Remove(device);
                    this.OnDeviceDisconnected(new DeviceDataEventArgs(device));
                }
            }
        }
    }
}
