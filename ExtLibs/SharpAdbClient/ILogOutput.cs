// <copyright file="ILogOutput.cs" company="The Android Open Source Project, Ryan Conrad, Quamotion">
// Copyright (c) The Android Open Source Project, Ryan Conrad, Quamotion. All rights reserved.
// </copyright>

namespace SharpAdbClient
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    ///
    /// </summary>
    public interface ILogOutput
    {
        /// <summary>
        /// Writes the specified log level.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="message">The message.</param>
        void Write(LogLevel.LogLevelInfo logLevel, string tag, string message);

        /// <summary>
        /// Writes the and prompt log.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="message">The message.</param>
        void WriteAndPromptLog(LogLevel.LogLevelInfo logLevel, string tag, string message);
    }
}
