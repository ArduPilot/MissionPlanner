// <copyright file="AdbServerStatus.cs" company="The Android Open Source Project, Ryan Conrad, Quamotion">
// Copyright (c) The Android Open Source Project, Ryan Conrad, Quamotion. All rights reserved.
// </copyright>

namespace SharpAdbClient
{
    using System;

    /// <summary>
    /// Represents the status of the adb server.
    /// </summary>
    public struct AdbServerStatus
    {
        /// <summary>
        /// Gets or sets a value indicating whether the server is currently running.
        /// </summary>
        public bool IsRunning
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets, when the server is running, the version of the server that is running.
        /// </summary>
        public Version Version
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a <see cref="string"/> that represents the current <see cref="AdbServerStatus"/>
        /// object.
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> that represents the current <see cref="AdbServerStatus"/>
        /// object.
        /// </returns>
        public override string ToString()
        {
            if (this.IsRunning)
            {
                return $"Version {this.Version} of the adb daemon is running.";
            }
            else
            {
                return "The adb daemon is not running.";
            }
        }
    }
}
