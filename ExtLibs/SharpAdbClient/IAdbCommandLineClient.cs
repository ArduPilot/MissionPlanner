// <copyright file="IAdbCommandLineClient.cs" company="The Android Open Source Project, Ryan Conrad, Quamotion">
// Copyright (c) The Android Open Source Project, Ryan Conrad, Quamotion. All rights reserved.
// </copyright>

namespace SharpAdbClient
{
    using System;

    /// <summary>
    /// Provides a common interface for any class that provides access to the
    /// <c>adb.exe</c> executable.
    /// </summary>
    public interface IAdbCommandLineClient
    {
        /// <summary>
        /// Queries adb for its version number and checks it against <see cref="AdbServer.RequiredAdbVersion"/>.
        /// </summary>
        Version GetVersion();

        /// <summary>
        /// Starts the adb server by running the <c>adb start-server</c> command.
        /// </summary>
        void StartServer();

        /// <summary>
        /// Throws an error if the path does not point to a valid instance of <c>adb.exe</c>.
        /// </summary>
        /// <param name="adbPath">
        /// The path to validate.
        /// </param>
        bool IsValidAdbFile(string adbPath);
    }
}
