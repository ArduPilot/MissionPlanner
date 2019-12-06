// <copyright file="SyncCommand.cs" company="The Android Open Source Project, Ryan Conrad, Quamotion">
// Copyright (c) The Android Open Source Project, Ryan Conrad, Quamotion. All rights reserved.
// </copyright>

namespace SharpAdbClient
{
    /// <summary>
    /// Defines a command that can be sent to, or a response that can be received from,
    /// the sync service.
    /// </summary>
    public enum SyncCommand
    {
        /// <summary>
        /// List the files in a folder.
        /// </summary>
        LIST,

        /// <summary>
        /// Retrieve a file from device
        /// </summary>
        RECV,

        /// <summary>
        /// Send a file to device
        /// </summary>
        SEND,

        /// <summary>
        /// Stat a file
        /// </summary>
        STAT,

        /// <summary>
        /// A directory entry.
        /// </summary>
        DENT,

        /// <summary>
        /// The operation has failed.
        /// </summary>
        FAIL,

        /// <summary>
        /// Marks the start of a data packet.
        /// </summary>
        DATA,

        /// <summary>
        /// The server has acknowledged the request.
        /// </summary>
        OKAY,

        /// <summary>
        /// The operation has completed.
        /// </summary>
        DONE
    }
}
