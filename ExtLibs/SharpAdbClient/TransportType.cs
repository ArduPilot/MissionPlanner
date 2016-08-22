// <copyright file="TransportType.cs" company="The Android Open Source Project, Ryan Conrad, Quamotion">
// Copyright (c) The Android Open Source Project, Ryan Conrad, Quamotion. All rights reserved.
// </copyright>

namespace SharpAdbClient
{
    /// <summary>
    /// Specifies the transport type used between the device and the Android Debug Bridge server.
    /// </summary>
    public enum TransportType
    {
        /// <summary>
        /// The device is connected through USB
        /// </summary>
        Usb,

        /// <summary>
        /// The device is connected through a local TCP connection.
        /// </summary>
        Local,

        /// <summary>
        /// The device is connected through any transport type.
        /// </summary>
        Any,

        /// <summary>
        /// The device is connected through the host transport type.
        /// </summary>
        Host
    }
}
