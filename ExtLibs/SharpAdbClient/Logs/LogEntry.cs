// <copyright file="LogEntry.cs" company="The Android Open Source Project, Ryan Conrad, Quamotion">
// Copyright (c) The Android Open Source Project, Ryan Conrad, Quamotion. All rights reserved.
// </copyright>

namespace SharpAdbClient.Logs
{
    using System;

    /// <summary>
    /// The userspace structure for version 1 of the logger_entry ABI.
    /// This structure is returned to userspace by the kernel logger
    /// driver unless an upgrade to a newer ABI version is requested.
    /// </summary>
    /// <seealso href="https://android.googlesource.com/platform/system/core/+/master/include/log/logger.h"/>
    public class LogEntry
    {
        /// <summary>
        /// Gets or sets the process ID of the code that generated the log message.
        /// </summary>
        public int ProcessId { get; set; }

        /// <summary>
        /// Gets or sets the thread ID of the code that generated the log message.
        /// </summary>
        public int ThreadId { get; set; }

        /// <summary>
        /// Gets or sets the date and time at which the message was logged.
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the log id (v3) of the payload effective UID of logger (v2);
        /// this value is not available for v1 entries.
        /// </summary>
        public uint Id { get; set; }

        /// <summary>
        /// Gets or sets the entry's payload
        /// </summary>
        public byte[] Data { get; set; }
    }
}
