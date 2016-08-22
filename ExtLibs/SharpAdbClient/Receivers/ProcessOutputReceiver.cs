// <copyright file="ProcessOutputReceiver.cs" company="The Android Open Source Project, Ryan Conrad, Quamotion">
// Copyright (c) The Android Open Source Project, Ryan Conrad, Quamotion. All rights reserved.
// </copyright>

namespace SharpAdbClient.Receivers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using SharpAdbClient.DeviceCommands;

    /// <summary>
    /// Parses the output of a <c>cat /proc/[pid]/stat</c> command.
    /// </summary>
    internal class ProcessOutputReceiver : MultiLineReceiver
    {
        /// <summary>
        /// Gets a list of all processes that have been received.
        /// </summary>
        public Collection<AndroidProcess> Processes
        { get; private set; } = new Collection<AndroidProcess>();

        /// <inheritdoc/>
        protected override void ProcessNewLines(IEnumerable<string> lines)
        {
            foreach (var line in lines)
            {
                // Process has already died (e.g. the cat process itself)
                if (line.Contains("No such file or directory"))
                {
                    continue;
                }

                try
                {
                    this.Processes.Add(AndroidProcess.Parse(line));
                }
                catch (Exception)
                {
                    // Swallow
                }
            }
        }
    }
}
