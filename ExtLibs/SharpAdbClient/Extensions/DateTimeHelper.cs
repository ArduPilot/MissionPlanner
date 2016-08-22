// <copyright file="DateTimeHelper.cs" company="The Android Open Source Project, Ryan Conrad, Quamotion">
// Copyright (c) The Android Open Source Project, Ryan Conrad, Quamotion. All rights reserved.
// </copyright>

namespace SharpAdbClient
{
    using System;

    /// <summary>
    /// Provides helper methods for working with Unix-based date formats.
    /// </summary>
    public static class DateTimeHelper
    {
        /// <summary>
        /// Gets EPOCH time
        /// </summary>
        public static DateTime Epoch
        { get; } = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Converts a <see cref="DateTime"/> to the Unix equivalent.
        /// </summary>
        /// <param name="date">
        /// The date to convert to the Unix format.
        /// </param>
        /// <returns>
        /// A <see cref="long"/> that represents the date, in Unix format.
        /// </returns>
        public static long ToUnixEpoch(this DateTime date)
        {
            TimeSpan t = date.ToUniversalTime() - Epoch;
            long epoch = (long)t.TotalSeconds;
            return epoch;
        }
    }
}
