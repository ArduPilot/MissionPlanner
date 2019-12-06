// <copyright file="EventLogType.cs" company="The Android Open Source Project, Ryan Conrad, Quamotion">
// Copyright (c) The Android Open Source Project, Ryan Conrad, Quamotion. All rights reserved.
// </copyright>

namespace SharpAdbClient.Logs
{
    /// <summary>
    /// Represents the different types of values that can be stored in an event log entry.
    /// </summary>
    public enum EventLogType
    {
        /// <summary>
        /// The value is a four-byte signed integer.
        /// </summary>
        Integer = 0,

        /// <summary>
        /// The value is an eight-byte signed integer.
        /// </summary>
        Long = 1,

        /// <summary>
        /// The value is a string.
        /// </summary>
        String = 2,

        /// <summary>
        /// The value is a list of values.
        /// </summary>
        List = 3,

        /// <summary>
        /// The value is a four-byte signed floating number.
        /// </summary>
        Float = 4,
    }
}
