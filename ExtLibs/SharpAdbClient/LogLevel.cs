// <copyright file="LogLevel.cs" company="The Android Open Source Project, Ryan Conrad, Quamotion">
// Copyright (c) The Android Open Source Project, Ryan Conrad, Quamotion. All rights reserved.
// </copyright>

namespace SharpAdbClient
{
    /// <summary>
    ///
    /// </summary>
    public static class LogLevel
    {
        /// <summary>
        /// Initializes static members of the <see cref="LogLevel"/> class.
        /// </summary>
        static LogLevel()
        {
            Verbose = new LogLevelInfo { Priority = 2, Value = "verbose", Letter = 'V' };
            Debug = new LogLevelInfo { Priority = 3, Value = "debug", Letter = 'D' };
            Info = new LogLevelInfo { Priority = 4, Value = "info", Letter = 'I' };
            Warn = new LogLevelInfo { Priority = 5, Value = "warn", Letter = 'W' };
            Error = new LogLevelInfo { Priority = 6, Value = "error", Letter = 'E' };
            Assert = new LogLevelInfo { Priority = 7, Value = "assert", Letter = 'A' };
        }

        /// <summary>
        /// Gets the verbose log level.
        /// </summary>
        /// <value>The verbose log level.</value>
        public static LogLevelInfo Verbose { get; private set; }

        /// <summary>
        /// Gets the debug log level.
        /// </summary>
        /// <value>The debug log level.</value>
        public static LogLevelInfo Debug { get; private set; }

        /// <summary>
        /// Gets the info log level.
        /// </summary>
        /// <value>The info log level.</value>
        public static LogLevelInfo Info { get; private set; }

        /// <summary>
        /// Gets the warn log level.
        /// </summary>
        /// <value>The warn log level.</value>
        public static LogLevelInfo Warn { get; private set; }

        /// <summary>
        /// Gets the error log level.
        /// </summary>
        /// <value>The error log level.</value>
        public static LogLevelInfo Error { get; private set; }

        /// <summary>
        /// Gets the assert log level.
        /// </summary>
        /// <value>The assert log level.</value>
        public static LogLevelInfo Assert { get; private set; }

        /// <summary>
        ///
        /// </summary>
        public class LogLevelInfo
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="LogLevelInfo"/> class.
            /// </summary>
            public LogLevelInfo()
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="LogLevelInfo"/> class.
            /// </summary>
            /// <param name="priority">The priority.</param>
            /// <param name="value">The value.</param>
            /// <param name="letter">The letter.</param>
            public LogLevelInfo(int priority, string value, char letter)
            {
                this.Priority = priority;
                this.Value = value;
                this.Letter = letter;
            }

            /// <summary>
            /// Gets or sets the priority.
            /// </summary>
            /// <value>The priority.</value>
            public int Priority { get; set; }

            /// <summary>
            /// Gets or sets the value.
            /// </summary>
            /// <value>The value.</value>
            public string Value { get; set; }

            /// <summary>
            /// Gets or sets the letter.
            /// </summary>
            /// <value>The letter.</value>
            public char Letter { get; set; }
        }
    }
}
