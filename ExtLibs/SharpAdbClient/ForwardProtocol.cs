// <copyright file="ForwardProtocol.cs" company="The Android Open Source Project, Ryan Conrad, Quamotion">
// Copyright (c) The Android Open Source Project, Ryan Conrad, Quamotion. All rights reserved.
// </copyright>

namespace SharpAdbClient
{
    /// <summary>
    /// Represents a protocol which is being forwarded over adb.
    /// </summary>
    public enum ForwardProtocol
    {
        /// <summary>
        /// Enables the forwarding of a TCP port.
        /// </summary>
        Tcp,

        /// <summary>
        /// Enables the forwarding of a Unix domain socket.
        /// </summary>
        LocalAbstract,

        /// <summary>
        /// Enables the forwarding of a Unix domain socket.
        /// </summary>
        LocalReserved,

        /// <summary>
        /// Enables the forwarding of a Unix domain socket.
        /// </summary>
        LocalFilesystem,

        /// <summary>
        /// Enables the forwarding of a character device.
        /// </summary>
        Device,

        /// <summary>
        /// Enables port forwarding of the java debugger for a specific process.
        /// </summary>
        JavaDebugWireProtocol
    }
}
