// <copyright file="PackageManager.cs" company="The Android Open Source Project, Ryan Conrad, Quamotion">
// Copyright (c) The Android Open Source Project, Ryan Conrad, Quamotion. All rights reserved.
// </copyright>

namespace SharpAdbClient.DeviceCommands
{
    using Exceptions;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;

    /// <summary>
    /// Allows you to get information about packages that are installed on a device.
    /// </summary>
    public class PackageManager
    {
        /// <summary>
        /// The path to a temporary directory to use when pushing files to the device.
        /// </summary>
        public const string TempInstallationDirectory = "/storage/sdcard0/tmp/";

        /// <summary>
        /// The command that list all packages installed on the device.
        /// </summary>
        private const string ListFull = "pm list packages -f";

        /// <summary>
        /// The command that list all third party packages installed on the device.
        /// </summary>
        private const string ListThirdPartyOnly = "pm list packages -f -3";

        /// <summary>
        /// The tag to use when logging messages.
        /// </summary>
        private const string Tag = nameof(PackageManager);

        /// <summary>
        /// Initializes a new instance of the <see cref="PackageManager"/> class.
        /// </summary>
        /// <param name="device">
        /// The device on which to look for packages.
        /// </param>
        /// <param name="thirdPartyOnly">
        /// <see langword="true"/> to only indicate third party applications;
        /// <see langword="false"/> to also include built-in applications.
        /// </param>
        public PackageManager(DeviceData device, bool thirdPartyOnly = false)
        {
            if (device == null)
            {
                throw new ArgumentNullException(nameof(device));
            }

            this.Device = device;
            this.Packages = new Dictionary<string, string>();
            this.ThirdPartyOnly = thirdPartyOnly;
            this.RefreshPackages();
        }

        /// <summary>
        /// Gets a value indicating whether this package manager only lists third party
        /// applications, or also includes built-in applications.
        /// </summary>
        public bool ThirdPartyOnly
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the list of packages currently installed on the device. They key is the name of the
        /// package; the value the package path.
        /// </summary>
        public Dictionary<string, string> Packages { get; private set; }

        /// <summary>
        /// Gets the device.
        /// </summary>
        public DeviceData Device { get; private set; }

        /// <summary>
        /// Refreshes the packages.
        /// </summary>
        public void RefreshPackages()
        {
            this.ValidateDevice();

            PackageManagerReceiver pmr = new PackageManagerReceiver(this.Device, this);

            if (this.ThirdPartyOnly)
            {
                this.Device.ExecuteShellCommand(ListThirdPartyOnly, pmr);
            }
            else
            {
                this.Device.ExecuteShellCommand(ListFull, pmr);
            }
        }

        /// <summary>
        /// Installs an Android application on device.
        /// </summary>
        /// <param name="packageFilePath">
        /// The absolute file system path to file on local host to install.
        /// </param>
        /// <param name="reinstall">
        /// <see langword="true"/>if re-install of app should be performed; otherwise,
        /// <see langword="false"/>.
        /// </param>
        public void InstallPackage(string packageFilePath, bool reinstall)
        {
            this.ValidateDevice();

            string remoteFilePath = this.SyncPackageToDevice(packageFilePath);
            this.InstallRemotePackage(remoteFilePath, reinstall);
            this.RemoveRemotePackage(remoteFilePath);
        }

        /// <summary>
        /// Installs the application package that was pushed to a temporary location on the device.
        /// </summary>
        /// <param name="remoteFilePath">absolute file path to package file on device</param>
        /// <param name="reinstall">set to <see langword="true"/> if re-install of app should be performed</param>
        public void InstallRemotePackage(string remoteFilePath, bool reinstall)
        {
            this.ValidateDevice();

            InstallReceiver receiver = new InstallReceiver();
            var reinstallSwitch = reinstall ? "-r " : string.Empty;

            string cmd = $"pm install {reinstallSwitch}{remoteFilePath}";
            this.Device.ExecuteShellCommand(cmd, receiver);

            if (!string.IsNullOrEmpty(receiver.ErrorMessage))
            {
                throw new PackageInstallationException(receiver.ErrorMessage);
            }
        }

        /// <summary>
        /// Uninstalls a package from the device.
        /// </summary>
        /// <param name="packageName">
        /// The name of the package to uninstall.
        /// </param>
        public void UninstallPackage(string packageName)
        {
            this.ValidateDevice();

            InstallReceiver receiver = new InstallReceiver();
            this.Device.ExecuteShellCommand($"pm uninstall {packageName}", receiver);
            if (!string.IsNullOrEmpty(receiver.ErrorMessage))
            {
                throw new PackageInstallationException(receiver.ErrorMessage);
            }
        }

        /// <summary>
        /// Requests the version information from the device.
        /// </summary>
        /// <param name="packageName">
        /// The name of the package from which to get the application version.
        /// </param>
        public VersionInfo GetVersionInfo(string packageName)
        {
            this.ValidateDevice();

            VersionInfoReceiver receiver = new VersionInfoReceiver();
            this.Device.ExecuteShellCommand($"dumpsys package {packageName}", receiver);
            return receiver.VersionInfo;
        }

        private void ValidateDevice()
        {
            if (this.Device.State != DeviceState.Online)
            {
                throw new AdbException("Device is offline");
            }
        }

        /// <summary>
        /// Pushes a file to device
        /// </summary>
        /// <param name="localFilePath">the absolute path to file on local host</param>
        /// <returns>destination path on device for file</returns>
        /// <exception cref="IOException">if fatal error occurred when pushing file</exception>
        private string SyncPackageToDevice(string localFilePath)
        {
            this.ValidateDevice();

            try
            {
                string packageFileName = Path.GetFileName(localFilePath);

                // only root has access to /data/local/tmp/... not sure how adb does it then...
                // workitem: 16823
                // workitem: 19711
                string remoteFilePath = LinuxPath.Combine(TempInstallationDirectory, packageFileName);

                Log.Debug(packageFileName, $"Uploading {packageFileName} onto device '{this.Device.Serial}'");

                using (ISyncService sync = Factories.SyncServiceFactory(this.Device))
                using (Stream stream = File.OpenRead(localFilePath))
                {
                    string message = $"Uploading file onto device '{this.Device.Serial}'";
                    Log.Debug(Tag, message);

                    sync.Push(stream, remoteFilePath, 644, File.GetLastWriteTime(localFilePath), CancellationToken.None);
                }

                return remoteFilePath;
            }
            catch (IOException e)
            {
                Log.Error(Tag, $"Unable to open sync connection! reason: {e.Message}");
                throw;
            }
        }

        /// <summary>
        /// Remove a file from device
        /// </summary>
        /// <param name="remoteFilePath">path on device of file to remove</param>
        /// <exception cref="IOException">if file removal failed</exception>
        private void RemoveRemotePackage(string remoteFilePath)
        {
            // now we delete the app we sync'ed
            try
            {
                this.Device.ExecuteShellCommand("rm " + remoteFilePath, null);
            }
            catch (IOException e)
            {
                Log.Error(Tag, $"Failed to delete temporary package: {e.Message}");
                throw e;
            }
        }
    }
}
