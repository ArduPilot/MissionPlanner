// <copyright file="UnknownOptionException.cs" company="The Android Open Source Project, Ryan Conrad, Quamotion">
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
    /// Thrown when a command has an unknown option passed
    /// </summary>
    public class UnknownOptionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownOptionException"/> class.
        /// </summary>
        public UnknownOptionException()
            : base("Unknown option.")
            {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownOptionException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public UnknownOptionException(string message)
            : base(message)
            {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownOptionException"/> class.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        /// <param name="context">The context.</param>
        public UnknownOptionException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
            {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownOptionException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public UnknownOptionException(string message, Exception innerException)
            : base(message, innerException)
            {
        }
    }
}
