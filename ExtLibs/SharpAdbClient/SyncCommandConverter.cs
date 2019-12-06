// <copyright file="SyncCommandConverter.cs" company="The Android Open Source Project, Ryan Conrad, Quamotion">
// Copyright (c) The Android Open Source Project, Ryan Conrad, Quamotion. All rights reserved.
// </copyright>

namespace SharpAdbClient
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Converts <see cref="SyncCommand"/> values to their binary representation
    /// and vice versa.
    /// </summary>
    public static class SyncCommandConverter
    {
        /// <summary>
        /// Maps the <see cref="SyncCommand"/> values to their string representations.
        /// </summary>
        private static readonly Dictionary<SyncCommand, string> Values = new Dictionary<SyncCommand, string>();

        /// <summary>
        /// Initializes static members of the <see cref="SyncCommandConverter"/> class.
        /// </summary>
        static SyncCommandConverter()
        {
            Values.Add(SyncCommand.DATA, "DATA");
            Values.Add(SyncCommand.DENT, "DENT");
            Values.Add(SyncCommand.DONE, "DONE");
            Values.Add(SyncCommand.FAIL, "FAIL");
            Values.Add(SyncCommand.LIST, "LIST");
            Values.Add(SyncCommand.OKAY, "OKAY");
            Values.Add(SyncCommand.RECV, "RECV");
            Values.Add(SyncCommand.SEND, "SEND");
            Values.Add(SyncCommand.STAT, "STAT");
        }

        /// <summary>
        /// Gets the byte array that represents the <see cref="SyncCommand"/>.
        /// </summary>
        /// <param name="command">
        /// The <see cref="SyncCommand"/> to convert.
        /// </param>
        /// <returns>
        /// A byte array that represents the <see cref="SyncCommand"/>.
        /// </returns>
        public static byte[] GetBytes(SyncCommand command)
        {
            if (!Values.ContainsKey(command))
            {
                throw new ArgumentOutOfRangeException(nameof(command), $"{command} is not a valid sync command");
            }

            string commandText = Values[command];
            byte[] commandBytes = AdbClient.Encoding.GetBytes(commandText);

            return commandBytes;
        }

        /// <summary>
        /// Determines which <see cref="SyncCommand"/> is represented by this byte array.
        /// </summary>
        /// <param name="value">
        /// A byte array that represents a <see cref="SyncCommand"/>.
        /// </param>
        /// <returns>
        /// The corresponding <see cref="SyncCommand"/>.
        /// </returns>
        public static SyncCommand GetCommand(byte[] value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (value.Length != 4)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            string commandText = AdbClient.Encoding.GetString(value);

            var key = Values.Where(d => string.Equals(d.Value, commandText, StringComparison.OrdinalIgnoreCase)).Select(d => new SyncCommand?(d.Key)).SingleOrDefault();

            if (key == null)
            {
                throw new ArgumentOutOfRangeException(nameof(value), $"{commandText} is not a valid sync command");
            }

            return key.Value;
        }
    }
}
