// <copyright file="IAdbServer.cs" company="The Android Open Source Project, Ryan Conrad, Quamotion">
// Copyright (c) The Android Open Source Project, Ryan Conrad, Quamotion. All rights reserved.
// </copyright>

namespace SharpAdbClient
{
    using Exceptions;
    using System.Net;

    /// <summary>
    /// Represents a common interface for any class that allows starting or stopping the Android Debug Bridge (adb)
    /// server/deamon.
    /// </summary>
    public interface IAdbServer
    {
        /// <summary>
        /// Gets the <see cref="EndPoint"/> at which the Android Debug Bridge server is listening.
        /// </summary>
        EndPoint EndPoint { get; }

        /// <summary>
        /// Starts the adb server if it was not previously running.
        /// </summary>
        /// <param name="adbPath">
        /// The path to the <c>adb.exe</c> executable that can be used to start the adb server.
        /// If this path is not provided, this method will throw an exception if the server
        /// is not running or is not up to date.
        /// </param>
        /// <param name="restartServerIfNewer">
        /// <see langword="true"/> to restart the adb server if the version of the <c>adb.exe</c>
        /// executable at <paramref name="adbPath"/> is newer than the version that is currently
        /// running; <see langword="false"/> to keep a previous version of the server running.
        /// </param>
        /// <returns>
        /// <list type="ordered">
        /// <item>
        ///     <see cref="StartServerResult.AlreadyRunning"/> if the adb server was already
        ///     running and the version of the adb server was at least <see cref="AdbServer.RequiredAdbVersion"/>.
        /// </item>
        /// <item>
        ///     <see cref="StartServerResult.RestartedOutdatedDaemon"/> if the adb server
        ///     was already running, but the version was less than <see cref="AdbServer.RequiredAdbVersion"/>
        ///     or less than the version of the adb client at <paramref name="adbPath"/> and the
        ///     <paramref name="restartServerIfNewer"/> flag was set.
        /// </item>
        /// <item>
        /// </item>
        ///     <see cref="StartServerResult.Started"/> if the adb server was not running,
        ///     and the server was started.
        /// </list>
        /// </returns>
        /// <exception cref="AdbException">
        /// The server was not running, or an outdated version of the server was running,
        /// and the <paramref name="adbPath"/> parameter was not specified.
        /// </exception>
        StartServerResult StartServer(string adbPath, bool restartServerIfNewer);

        /// <summary>
        /// Restarts the adb server if it suddenly became unavailable. Call this class if, for example,
        /// you receive an <see cref="AdbException"/> with the <see cref="AdbException.ConnectionReset"/> flag
        /// set to <see langword="true"/> - a clear indicating the ADB server died.
        /// </summary>
        /// <remarks>
        /// You can only call this method if you have previously started the adb server via
        /// <see cref="AdbServer.StartServer(string, bool)"/> and passed the full path to the adb server.
        /// </remarks>
        void RestartServer();

        /// <summary>
        /// Gets the status of the adb server.
        /// </summary>
        /// <returns>
        /// A <see cref="AdbServerStatus"/> object that describes the status of the
        /// adb server.
        /// </returns>
        AdbServerStatus GetStatus();
    }
}
