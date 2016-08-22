// <copyright file="DeviceExtensions.cs" company="The Android Open Source Project, Ryan Conrad, Quamotion">
// Copyright (c) The Android Open Source Project, Ryan Conrad, Quamotion. All rights reserved.
// </copyright>

namespace SharpAdbClient.DeviceCommands
{
    using Receivers;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// Provides extension methods for the <see cref="DeviceData"/> class, allowing you to run
    /// commands directory against a <see cref="DeviceData"/> object.
    /// </summary>
    public static class DeviceExtensions
    {
        /// <summary>
        /// Executes a shell command on the device.
        /// </summary>
        /// <param name="device">
        /// The device on which to run the command.
        /// </param>
        /// <param name="command">
        /// The command to execute.
        /// </param>
        /// <param name="receiver">
        /// Optionally, a <see cref="IShellOutputReceiver"/> that processes the command output.
        /// </param>
        public static void ExecuteShellCommand(this DeviceData device, string command, IShellOutputReceiver receiver)
        {
            AdbClient.Instance.ExecuteRemoteCommand(command, device, receiver);
        }

        /// <summary>
        /// Gets the file statistics of a given file.
        /// </summary>
        /// <param name="device">
        /// The device on which to look for the file.
        /// </param>
        /// <param name="path">
        /// The path to the file.
        /// </param>
        /// <returns>
        /// A <see cref="FileStatistics"/> object that represents the file.
        /// </returns>
        public static FileStatistics Stat(this DeviceData device, string path)
        {
            using (ISyncService service = Factories.SyncServiceFactory(device))
            {
                return service.Stat(path);
            }
        }

        /// <summary>
        /// Gets the properties of a device.
        /// </summary>
        /// <param name="device">
        /// The device for which to list the properties.
        /// </param>
        /// <returns>
        /// A dictionary containing the properties of the device, and their values.
        /// </returns>
        public static Dictionary<string, string> GetProperties(this DeviceData device)
        {
            var receiver = new GetPropReceiver();
            AdbClient.Instance.ExecuteRemoteCommand(GetPropReceiver.GetpropCommand, device, receiver);
            return receiver.Properties;
        }

        /// <summary>
        /// Gets the environment variables currently defined on a device.
        /// </summary>
        /// <param name="device">
        /// The device for which to list the environment variables.
        /// </param>
        /// <returns>
        /// A dictionary containing the environment variables of the device, and their values.
        /// </returns>
        public static Dictionary<string, string> GetEnvironmentVariables(this DeviceData device)
        {
            var receiver = new EnvironmentVariablesReceiver();
            AdbClient.Instance.ExecuteRemoteCommand(EnvironmentVariablesReceiver.PrintEnvCommand, device, receiver);
            return receiver.EnvironmentVariables;
        }

        /// <summary>
        /// Uninstalls a package from the device.
        /// </summary>
        /// <param name="device">
        /// The device on which to uninstall the package.
        /// </param>
        /// <param name="packageName">
        /// The name of the package to uninstall.
        /// </param>
        public static void UninstallPackage(this DeviceData device, string packageName)
        {
            PackageManager manager = new PackageManager(device);
            manager.UninstallPackage(packageName);
        }

        /// <summary>
        /// Requests the version information from the device.
        /// </summary>
        /// <param name="device">
        /// The device on which to uninstall the package.
        /// </param>
        /// <param name="packageName">
        /// The name of the package from which to get the application version.
        /// </param>
        public static VersionInfo GetPackageVersion(this DeviceData device, string packageName)
        {
            PackageManager manager = new PackageManager(device);
            return manager.GetVersionInfo(packageName);
        }

        /// <summary>
        /// Lists all processes running on the device.
        /// </summary>
        /// <param name="device">
        /// The device on which to list the processes that are running.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{AndroidProcess}"/> that will iterate over all
        /// processes that are currently running on the device.
        /// </returns>
        public static IEnumerable<AndroidProcess> ListProcesses(this DeviceData device)
        {
            // There are a couple of gotcha's when listing processes on an Android device.
            // One way would be to run ps and parse the output. However, the output of
            // ps differents from Android version to Android version, is not delimited, nor
            // entirely fixed length, and some of the fields can be empty, so it's almost impossible
            // to parse correctly.
            //
            // The alternative is to directly read the values in /proc/[pid], pretty much like ps
            // does (see https://android.googlesource.com/platform/system/core/+/master/toolbox/ps.c).
            //
            // The easiest way to do the directory listings would be to use the SyncService; unfortunately,
            // the sync service doesn't work very well with /proc/ so we're back to using ls and taking it
            // from there.
            List<AndroidProcess> processes = new List<AndroidProcess>();

            // List all processes by doing ls /proc/.
            // All subfolders which are completely numeric are PIDs
            ConsoleOutputReceiver receiver = new ConsoleOutputReceiver();
            device.ExecuteShellCommand("/system/bin/ls /proc/", receiver);

            Collection<int> pids = new Collection<int>();

            using (StringReader reader = new StringReader(receiver.ToString()))
            {
                while (reader.Peek() > 0)
                {
                    string line = reader.ReadLine();

                    if (!line.All(c => char.IsDigit(c)))
                    {
                        continue;
                    }

                    var pid = int.Parse(line);

                    pids.Add(pid);
                }
            }

            // For each pid, we can get /proc/[pid]/stat, which contains the process information in a well-defined
            // format - see http://man7.org/linux/man-pages/man5/proc.5.html.
            // Doing cat on each file one by one takes too much time. Doing cat on all of them at the same time doesn't work
            // either, because the command line would be too long.
            // So we do it 50 processes at at time.
            StringBuilder catBuilder = new StringBuilder();
            ProcessOutputReceiver processOutputReceiver = new ProcessOutputReceiver();

            for (int i = 0; i < pids.Count; i++)
            {
                if (i % 50 == 0)
                {
                    catBuilder.Clear();
                    catBuilder.Append("cat ");
                }

                catBuilder.Append($"/proc/{pids[i]}/stat ");

                if (i > 0 && (i % 50 == 0 || i == pids.Count - 1))
                {
                        device.ExecuteShellCommand(catBuilder.ToString(), processOutputReceiver);
                }
            }

            processOutputReceiver.Flush();

            return processOutputReceiver.Processes;
        }
    }
}
