// <copyright file="EnvironmentVariablesReceiver.cs" company="The Android Open Source Project, Ryan Conrad, Quamotion">
// Copyright (c) The Android Open Source Project, Ryan Conrad, Quamotion. All rights reserved.
// </copyright>

namespace SharpAdbClient
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Processes the output of the <c>printenv</c> command, which dumps all environment variables of
    /// an Android device.
    /// </summary>
    public sealed class EnvironmentVariablesReceiver : MultiLineReceiver
    {
        /// <summary>
        /// The path to the <c>printenv</c> command.
        /// </summary>
        public const string PrintEnvCommand = "/system/bin/printenv";

        /// <summary>
        /// A regular expression that can be used to parse the output of the <c>printenv</c> command.
        /// </summary>
        private const string EnvPattern = @"^([^=\s]+)\s*=\s*(.*)$";

        /// <summary>
        /// Initializes a new instance of the <see cref="EnvironmentVariablesReceiver"/> class.
        /// </summary>
        public EnvironmentVariablesReceiver()
        {
            this.EnvironmentVariables = new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets the environment variables that are currently defined on the device.
        /// </summary>
        public Dictionary<string, string> EnvironmentVariables
        {
            get;
            private set;
        }

        /// <summary>
        /// Processes the new lines.
        /// </summary>
        /// <param name="lines">The lines.</param>
        protected override void ProcessNewLines(IEnumerable<string> lines)
        {
            foreach (string line in lines)
            {
                if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
                {
                    continue;
                }

                Match m = Regex.Match(line, EnvPattern);
                if (m.Success)
                {
                    string label = m.Groups[1].Value.Trim();
                    string value = m.Groups[2].Value.Trim();

                    if (label.Length > 0)
                    {
                        if (this.EnvironmentVariables.ContainsKey(label))
                        {
                            this.EnvironmentVariables[label] = value;
                        }
                        else
                        {
                            this.EnvironmentVariables.Add(label, value);
                        }
                    }
                }
            }
        }
    }
}
