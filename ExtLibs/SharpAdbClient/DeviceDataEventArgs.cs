// <copyright file="DeviceDataEventArgs.cs" company="The Android Open Source Project, Ryan Conrad, Quamotion">
// Copyright (c) The Android Open Source Project, Ryan Conrad, Quamotion. All rights reserved.
// </copyright>

namespace SharpAdbClient
{
    using System;

    /// <summary>
    /// The event arguments that are passed when a device event occurs.
    /// </summary>
    public class DeviceDataEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceDataEventArgs"/> class.
        /// </summary>
        /// <param name="device">The device.</param>
        public DeviceDataEventArgs(DeviceData device)
        {
            this.Device = device;
        }

        /// <summary>
        /// Gets the device.
        /// </summary>
        /// <value>The device.</value>
        public DeviceData Device { get; private set; }
    }
}
