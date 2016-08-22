//-----------------------------------------------------------------------
// <copyright file="VersionInfoReceiver.cs" company="The Android Open Source Project, Ryan Conrad, Quamotion">
// Copyright (c) The Android Open Source Project, Ryan Conrad, Quamotion. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace SharpAdbClient.DeviceCommands
{
    using SharpAdbClient;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Processes command line output of the <c>dumpsys package</c> command.
    /// </summary>
    internal class VersionInfoReceiver : InfoReceiver
    {
        /// <summary>
        /// The name of the version code property.
        /// </summary>
        private static string versionCode = "VersionCode";

        /// <summary>
        /// The name of the version name property.
        /// </summary>
        private static string versionName = "VersionName";

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionInfoReceiver"/> class.
        /// </summary>
        public VersionInfoReceiver()
        {
            this.AddPropertyParser(versionCode, this.GetVersionCode);
            this.AddPropertyParser(versionName, this.GetVersionName);
        }

        /// <summary>
        /// Gets the version code of the specified package
        /// </summary>
        public VersionInfo VersionInfo
        {
            get
            {
                if (this.GetPropertyValue(versionCode) != null && this.GetPropertyValue(versionName) != null)
                {
                    return new VersionInfo((int)this.GetPropertyValue(versionCode), (string)this.GetPropertyValue(versionName));
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Parses the given line and extracts the version name if possible.
        /// </summary>
        /// <param name="line">The line to be parsed.</param>
        /// <returns>The extracted version name.</returns>
        internal object GetVersionName(string line)
        {
            if (line != null && line.Trim().StartsWith("versionName="))
            {
                return line.Trim().Substring(12).Trim();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Parses the given line and extracts the version code if possible.
        /// </summary>
        /// <param name="line">The line to be parsed.</param>
        /// <returns>The extracted version code.</returns>
        internal object GetVersionCode(string line)
        {
            if (line == null)
            {
                return null;
            }

            string versionCodeRegex = @"versionCode=(\d*) targetSdk=";
            Match match = Regex.Match(line, versionCodeRegex);
            if (match.Success)
            {
                return int.Parse(match.Groups[1].Value.Trim());
            }
            else
            {
                return null;
            }
        }
    }
}
