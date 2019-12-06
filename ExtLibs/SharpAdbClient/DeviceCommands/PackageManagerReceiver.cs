// <copyright file="PackageManagerReceiver.cs" company="The Android Open Source Project, Ryan Conrad, Quamotion">
// Copyright (c) The Android Open Source Project, Ryan Conrad, Quamotion. All rights reserved.
// </copyright>

namespace SharpAdbClient.DeviceCommands
{
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// Parses the output of the various <c>pm</c> commands.
    /// </summary>
    public class PackageManagerReceiver : MultiLineReceiver
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PackageManagerReceiver"/> class.
        /// </summary>
        /// <param name="device">
        /// The device for which the package information is being received.
        /// </param>
        /// <param name="packageManager">
        /// The parent package manager.
        /// </param>
        public PackageManagerReceiver(DeviceData device, PackageManager packageManager)
        {
            this.Device = device;
            this.PackageManager = packageManager;
        }

        /// <summary>
        /// Gets the device.
        /// </summary>
        public DeviceData Device { get; private set; }

        /// <summary>
        /// Gets the package manager.
        /// </summary>
        public PackageManager PackageManager { get; private set; }

        /// <summary>
        /// Processes the new lines.
        /// </summary>
        /// <param name="lines">The lines.</param>
        protected override void ProcessNewLines(IEnumerable<string> lines)
        {
            this.PackageManager.Packages.Clear();

            foreach (var line in lines)
            {
                if (line != null && line.StartsWith("package:"))
                {
                    // Samples include:
                    // package:/system/app/LegacyCamera.apk=com.android.camera
                    // package:mwc2015.be
                    string[] parts = line.Split(':', '=');

                    if (parts.Length == 3)
                    {
                        this.PackageManager.Packages.Add(parts[2], parts[1]);
                    }
                    else if (parts.Length == 2)
                    {
                        this.PackageManager.Packages.Add(parts[1], null);
                    }
                    else
                    {
                        Debug.WriteLine($"Received invalid input {line}");
                    }
                }
            }
        }
    }
}
