// <copyright file="MultilineReceiver.cs" company="The Android Open Source Project, Ryan Conrad, Quamotion">
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
    public abstract class MultiLineReceiver : IShellOutputReceiver
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultiLineReceiver"/> class.
        /// </summary>
        public MultiLineReceiver()
        {
            this.Lines = new List<string>();
        }

        /// <summary>
        /// Gets or sets a value indicating whether [trim lines].
        /// </summary>
        /// <value><see langword="true"/> if [trim lines]; otherwise, <see langword="false"/>.</value>
        public bool TrimLines { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the receiver parses error messages.
        /// </summary>
        /// <value>
        ///     <see langword="true"/> if this receiver parsers error messages; otherwise <see langword="false"/>.
        /// </value>
        /// <remarks>
        /// The default value is <see langword="false"/>. If set to <see langword="false"/>, the <see cref="AdbClient"/>
        /// will detect common error messages and throw an exception.
        /// </remarks>
        public virtual bool ParsesErrors { get; protected set; }

        /// <summary>
        /// Gets or sets the lines.
        /// </summary>
        /// <value>The lines.</value>
        protected ICollection<string> Lines { get; set; }

        /// <summary>
        /// Adds a line to the output.
        /// </summary>
        /// <param name="line">
        /// The line to add to the output.
        /// </param>
        public void AddOutput(string line)
        {
            this.Lines.Add(line);
        }

        /// <summary>
        /// Flushes the output.
        /// </summary>
        public void Flush()
        {
            if (this.Lines.Count > 0)
            {
                // send it for final processing
                this.ProcessNewLines(this.Lines);
                this.Lines.Clear();
            }

            this.Done();
        }

        /// <summary>
        /// Finishes the receiver
        /// </summary>
        protected virtual void Done()
        {
            // Do nothing
        }

        /// <summary>
        /// Processes the new lines.
        /// </summary>
        /// <param name="lines">The lines.</param>
        protected abstract void ProcessNewLines(IEnumerable<string> lines);
    }
}
