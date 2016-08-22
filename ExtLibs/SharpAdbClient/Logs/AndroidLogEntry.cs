// <copyright file="AndroidLogEntry.cs" company="The Android Open Source Project, Ryan Conrad, Quamotion">
// Copyright (c) The Android Open Source Project, Ryan Conrad, Quamotion. All rights reserved.
// </copyright>

namespace SharpAdbClient.Logs
{
    /// <summary>
    /// Represents a standard Android log entry (an entry in any Android log buffer
    /// except the Event buffer).
    /// </summary>
    /// <seealso href="https://android.googlesource.com/platform/system/core/+/master/liblog/logprint.c#442"/>
    public class AndroidLogEntry : LogEntry
    {
        /// <summary>
        /// Gets or sets the priority of the log message.
        /// </summary>
        public byte Priority
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the log tag of the message. Used to identify the source of a log message.
        /// It usually identifies the class or activity where the log call occured.
        /// </summary>
        public string Tag
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the message that has been logged.
        /// </summary>
        public string Message
        {
            get;
            set;
        }
    }
}
