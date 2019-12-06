// <copyright file="CommandAbortingException.cs" company="The Android Open Source Project, Ryan Conrad, Quamotion">
// Copyright (c) The Android Open Source Project, Ryan Conrad, Quamotion. All rights reserved.
// </copyright>

namespace SharpAdbClient.Exceptions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;

    /// <summary>
    /// Thrown when an executed command identifies that it is being aborted.
    /// </summary>
    public class CommandAbortingException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandAbortingException"/> class.
        /// </summary>
        public CommandAbortingException()
            : base("Permission to access the specified resource was denied.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandAbortingException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public CommandAbortingException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandAbortingException"/> class.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        /// <param name="context">The context.</param>
        public CommandAbortingException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandAbortingException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public CommandAbortingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
