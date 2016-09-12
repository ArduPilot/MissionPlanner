// <copyright file="AdbException.cs" company="The Android Open Source Project, Ryan Conrad, Quamotion">
// Copyright (c) The Android Open Source Project, Ryan Conrad, Quamotion. All rights reserved.
// </copyright>

namespace SharpAdbClient.Exceptions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Sockets;
    using System.Runtime.Serialization;
    using System.Text;

    /// <summary>
    /// Represents an exception with communicating with ADB
    /// </summary>
    public class AdbException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AdbException"/> class.
        /// </summary>
        public AdbException()
            : base("An error occurred with ADB")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdbException"/> class with the
        /// specified error message.
        /// </summary>
        /// <param name="message">
        /// The message that describes the error.
        /// </param>
        public AdbException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdbException"/> class with the
        /// specified client error message and adb error message.
        /// </summary>
        /// <param name="message">
        /// The message that describes the error on the client side.
        /// </param>
        /// <param name="adbError">
        /// The raw error message that was sent by adb.
        /// </param>
        public AdbException(string message, string adbError)
            : base(message)
        {
            this.AdbError = adbError;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdbException"/> class with the
        /// specified client error message and <see cref="AdbResponse"/>
        /// </summary>
        /// <param name="message">
        /// The message that describes the error on the client side.
        /// </param>
        /// <param name="response">
        /// The <see cref="AdbResponse"/> that was sent by adb.
        /// </param>
        public AdbException(string message, AdbResponse response)
            : base(message)
        {
            this.AdbError = response.Message;
            this.Response = response;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdbException"/> class.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        /// <param name="context">The context.</param>
        public AdbException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdbException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public AdbException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Gets the error message that was sent by adb.
        /// </summary>
        public string AdbError
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the response that was sent by adb.
        /// </summary>
        public AdbResponse Response
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether the underlying error was a <see cref="SocketException"/> where the
        /// <see cref="SocketException.SocketErrorCode"/> is set to <see cref="SocketError.ConnectionReset"/>, indicating
        /// that the connection was reset by the remote server. This happens when the adb server was killed.
        /// </summary>
        public bool ConnectionReset
        {
            get
            {
                var socketException = this.InnerException as SocketException;

                if (socketException == null)
                {
                    return false;
                }

                return socketException.SocketErrorCode == SocketError.ConnectionReset;
            }
        }
    }
}
