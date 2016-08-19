// <copyright file="IDeviceMonitor.cs" company="The Android Open Source Project, Ryan Conrad, Quamotion">
// Copyright (c) The Android Open Source Project, Ryan Conrad, Quamotion. All rights reserved.
// </copyright>

namespace SharpAdbClient
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides a common interface for any class that allows you to monitor the list of
    /// devices that are currently connected to the adb server.
    /// </summary>
    public interface IDeviceMonitor : IDisposable
    {
        /// <include file='IDeviceMonitor.xml' path='/IDeviceMonitor/DeviceChanged/*'/>
        event EventHandler<DeviceDataEventArgs> DeviceChanged;

        /// <include file='IDeviceMonitor.xml' path='/IDeviceMonitor/DeviceConnected/*'/>
        event EventHandler<DeviceDataEventArgs> DeviceConnected;

        /// <include file='IDeviceMonitor.xml' path='/IDeviceMonitor/DeviceDisconnected/*'/>
        event EventHandler<DeviceDataEventArgs> DeviceDisconnected;

        /// <include file='IDeviceMonitor.xml' path='/IDeviceMonitor/Devices/*'/>
        IReadOnlyCollection<DeviceData> Devices { get; }

        /// <include file='IDeviceMonitor.xml' path='/IDeviceMonitor/Start/*'/>
        void Start();
    }
}
