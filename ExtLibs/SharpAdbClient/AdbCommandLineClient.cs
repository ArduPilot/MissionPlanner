// <copyright file="AdbCommandLineClient.cs" company="The Android Open Source Project, Ryan Conrad, Quamotion">
// Copyright (c) The Android Open Source Project, Ryan Conrad, Quamotion. All rights reserved.
// </copyright>

namespace SharpAdbClient
{
    using SharpAdbClient.Exceptions;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Provides methods for interacting with the <c>adb.exe</c> command line client.
    /// </summary>
    public class AdbCommandLineClient : IAdbCommandLineClient
    {
        /// <summary>
        /// The tag to use when logging.
        /// </summary>
        private const string Tag = nameof(AdbCommandLineClient);

        /// <summary>
        /// The regex pattern for getting the adb version from the <c>adb version</c> command.
        /// </summary>
        private const string AdbVersionPattern = "^.*(\\d+)\\.(\\d+)\\.(\\d+)$";

        /// <summary>
        /// Initializes a new instance of the <see cref="AdbCommandLineClient"/> class.
        /// </summary>
        /// <param name="adbPath">
        /// The path to the <c>adb.exe</c> executable.
        /// </param>
        public AdbCommandLineClient(string adbPath)
        {
            if (string.IsNullOrWhiteSpace(adbPath))
            {
                throw new ArgumentNullException(nameof(adbPath));
            }

            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                    if (!string.Equals(Path.GetFileName(adbPath), "adb.exe", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new ArgumentOutOfRangeException(nameof(adbPath), $"{adbPath} does not seem to be a valid adb.exe executable. The path must end with `adb.exe`");
                    }

                    break;

                case PlatformID.Unix:
                case PlatformID.MacOSX:
                    if (!string.Equals(Path.GetFileName(adbPath), "adb", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new ArgumentOutOfRangeException(nameof(adbPath), $"{adbPath} does not seem to be a valid adb executable. The path must end with `adb`");
                    }

                    break;

                default:
                    throw new NotSupportedException("SharpAdbClient only supports launching adb.exe on Windows, Mac OS and Linux");
            }

            this.EnsureIsValidAdbFile(adbPath);

            this.AdbPath = adbPath;
        }

        /// <summary>
        /// Gets the path to the <c>adb.exe</c> executable.
        /// </summary>
        public string AdbPath
        {
            get;
            private set;
        }

        /// <summary>
        /// Queries adb for its version number and checks it against <see cref="AdbServer.RequiredAdbVersion"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="Version"/> object that contains the version number of the Android Command Line client.
        /// </returns>
        public Version GetVersion()
        {
            // Run the adb.exe version command and capture the output.
            List<string> standardOutput = new List<string>();

            this.RunAdbProcess("version", null, standardOutput);

            // Parse the output to get the version.
            var version = GetVersionFromOutput(standardOutput);

            if (version == null)
            {
                throw new AdbException($"The version of the adb executable at {this.AdbPath} could not be determined.");
            }

            if (version < AdbServer.RequiredAdbVersion)
            {
                string message = $"Required minimum version of adb: {AdbServer.RequiredAdbVersion}. Current version is {version}";
                Log.LogAndDisplay(LogLevel.Error, Tag, message);
                throw new AdbException(message);
            }

            return version;
        }

        /// <summary>
        /// Starts the adb server by running the <c>adb start-server</c> command.
        /// </summary>
        public void StartServer()
        {
            int status = this.RunAdbProcessInner("start-server", null, null);

            if (status == 0)
            {
                return;
            }

            // Starting the adb server failed for whatever reason. This can happen if adb.exe
            // is running but is not accepting requests. In that case, try to kill it & start again.
            // It kills all processes named "adb", so let's hope nobody else named their process that way.
            foreach (var adbProcess in Process.GetProcessesByName("adb"))
            {
                try
                {
                    adbProcess.Kill();
                }
                catch (Win32Exception)
                {
                    // The associated process could not be terminated
                    // or
                    // The process is terminating.
                }
                catch (InvalidOperationException)
                {
                    // The process has already exited.
                    // There is no process associated with this Process object.
                }
            }

            // Try again. This time, we don't call "Inner", and an exception will be thrown if the start operation fails
            // again. We'll let that exception bubble up the stack.
            this.RunAdbProcess("start-server", null, null);
        }

        /// <inheritdoc/>
        public virtual bool IsValidAdbFile(string adbPath)
        {
            return File.Exists(adbPath);
        }

        /// <summary>
        /// Parses the output of the <c>adb.exe version</c> command and determines the
        /// adb version.
        /// </summary>
        /// <param name="output">
        /// The output of the <c>adb.exe version</c> command.
        /// </param>
        /// <returns>
        /// A <see cref="Version"/> object that represents the version of the adb command
        /// line client.
        /// </returns>
        internal static Version GetVersionFromOutput(List<string> output)
        {
            foreach (var line in output)
            {
                // Skip empty lines
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                Match matcher = Regex.Match(line, AdbVersionPattern);
                if (matcher.Success)
                {
                    int majorVersion = int.Parse(matcher.Groups[1].Value);
                    int minorVersion = int.Parse(matcher.Groups[2].Value);
                    int microVersion = int.Parse(matcher.Groups[3].Value);

                    return new Version(majorVersion, minorVersion, microVersion);
                }
            }

            return null;
        }

        /// <summary>
        /// Runs the <c>adb.exe</c> process, invoking a specific <paramref name="command"/>,
        /// and reads the standard output and standard error output.
        /// </summary>
        /// <param name="command">
        /// The <c>adb.exe</c> command to invoke, such as <c>version</c> or <c>start-server</c>.
        /// </param>
        /// <param name="errorOutput">
        /// A list in which to store the standard error output. Each line is added as a new entry.
        /// This value can be <see langword="null"/> if you are not interested in the standard
        /// error.
        /// </param>
        /// <param name="standardOutput">
        /// A list in which to store the standard output. Each line is added as a new entry.
        /// This value can be <see langword="null"/> if you are not interested in the standard
        /// output.
        /// </param>
        /// <remarks>
        /// <para>
        /// Use this command only for <c>adb</c> commands that return immediately, such as
        /// <c>adb version</c>. This operation times out after 5 seconds.
        /// </para>
        /// </remarks>
        /// <exception cref="AdbException">
        /// The process exited with an exit code other than <c>0</c>.
        /// </exception>
        protected virtual void RunAdbProcess(string command, List<string> errorOutput, List<string> standardOutput)
        {
            int status = this.RunAdbProcessInner(command, errorOutput, standardOutput);

            if (status != 0)
            {
                throw new AdbException($"The adb process returned error code {status} when running command {command}");
            }
        }

        /// <summary>
        /// Runs the <c>adb.exe</c> process, invoking a specific <paramref name="command"/>,
        /// and reads the standard output and standard error output.
        /// </summary>
        /// <param name="command">
        /// The <c>adb.exe</c> command to invoke, such as <c>version</c> or <c>start-server</c>.
        /// </param>
        /// <param name="errorOutput">
        /// A list in which to store the standard error output. Each line is added as a new entry.
        /// This value can be <see langword="null"/> if you are not interested in the standard
        /// error.
        /// </param>
        /// <param name="standardOutput">
        /// A list in which to store the standard output. Each line is added as a new entry.
        /// This value can be <see langword="null"/> if you are not interested in the standard
        /// output.
        /// </param>
        /// <returns>
        /// The return code of the <c>adb</c> process.
        /// </returns>
        /// <remarks>
        /// <para>
        /// Use this command only for <c>adb</c> commands that return immediately, such as
        /// <c>adb version</c>. This operation times out after 5 seconds.
        /// </para>
        /// </remarks>
        protected virtual int RunAdbProcessInner(string command, List<string> errorOutput, List<string> standardOutput)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            int status;

            ProcessStartInfo psi = new ProcessStartInfo(this.AdbPath, command)
            {
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };

            using (Process process = Process.Start(psi))
            {
                var standardErrorString = process.StandardError.ReadToEnd();
                var standardOutputString = process.StandardOutput.ReadToEnd();

                if (errorOutput != null)
                {
                    errorOutput.AddRange(standardErrorString.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));
                }

                if (standardOutput != null)
                {
                    standardOutput.AddRange(standardOutputString.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));
                }

                // get the return code from the process
                if (!process.WaitForExit(5000))
                {
                    process.Kill();
                }

                status = process.ExitCode;
            }

            return status;
        }
    }
}
