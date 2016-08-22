// <copyright file="LogId.cs" company="The Android Open Source Project, Ryan Conrad, Quamotion">
// Copyright (c) The Android Open Source Project, Ryan Conrad, Quamotion. All rights reserved.
// </copyright>

namespace SharpAdbClient.Logs
{
    /// <summary>
    /// Identifies the various Android log buffers.
    /// </summary>
    /// <seealso href="https://android.googlesource.com/platform/system/core/+/master/include/log/log.h#596"/>
    public enum LogId : uint
    {
        /// <summary>
        /// The main log buffer
        /// </summary>
        Main = 0,

        /// <summary>
        /// The buffer that contains radio/telephony related messages.
        /// </summary>
        Radio = 1,

        /// <summary>
        /// The buffer containing events-related messages.
        /// </summary>
        Events = 2,

        /// <summary>
        /// The Android system log buffer.
        /// </summary>
        System = 3,

        /// <summary>
        /// The Android crash log buffer.
        /// </summary>
        Crash = 4,

        /// <summary>
        /// The Android kernel log buffer.
        /// </summary>
        Kernel = 5
    }
}
