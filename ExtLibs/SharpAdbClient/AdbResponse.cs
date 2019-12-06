// <copyright file="AdbResponse.cs" company="The Android Open Source Project, Ryan Conrad, Quamotion">
// Copyright (c) The Android Open Source Project, Ryan Conrad, Quamotion. All rights reserved.
// </copyright>

namespace SharpAdbClient
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// An Adb Communication Response
    /// </summary>
    public class AdbResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AdbResponse"/> class.
        /// </summary>
        public AdbResponse()
        {
            this.Message = string.Empty;
        }

        /// <summary>
        /// Gets a <see cref="AdbResponse"/> that represents the OK response sent by ADB.
        /// </summary>
        public static AdbResponse OK
        { get; } = new AdbResponse()
        {
            IOSuccess = true,
            Okay = true,
            Message = string.Empty,
            Timeout = false
        };

        /// <summary>
        /// Gets or sets a value indicating whether the IO communication was a success.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if successful; otherwise, <see langword="false"/>.
        /// </value>
        public bool IOSuccess { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="AdbResponse"/> is okay.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if okay; otherwise, <see langword="false"/>.
        /// </value>
        public bool Okay { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="AdbResponse"/> is timeout.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if timeout; otherwise, <see langword="false"/>.
        /// </value>
        public bool Timeout { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }

        /// <summary>
        /// Creates a new instance of the <see cref="AdbResponse"/> class, based on an
        /// error message returned by adb.
        /// </summary>
        /// <param name="message">
        /// The error message returned by adb.
        /// </param>
        /// <returns>
        /// A new <see cref="AdbResponse"/> object that represents the error.
        /// </returns>
        public static AdbResponse FromError(string message)
        {
            return new AdbResponse()
            {
                IOSuccess = true,
                Message = message,
                Okay = false,
                Timeout = false
            };
        }

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to the current
        /// <see cref="AdbResponse"/> object.
        /// </summary>
        /// <param name="obj">
        /// The <see cref="object"/> to compare with the current <see cref="AdbResponse"/> object.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the specified object is equal to the current object;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var other = obj as AdbResponse;

            if (other == null)
            {
                return false;
            }

            return other.IOSuccess == this.IOSuccess
                && string.Equals(other.Message, this.Message, StringComparison.OrdinalIgnoreCase)
                && other.Okay == this.Okay
                && other.Timeout == this.Timeout;
        }

        /// <summary>
        /// Gets the hash code for the current <see cref="AdbResponse"/>.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="AdbResponse"/>.
        /// </returns>
        public override int GetHashCode()
        {
            int hash = 17;
            hash = (hash * 23) + this.IOSuccess.GetHashCode();
            hash = (hash * 23) + this.Message == null ? 0 : this.Message.GetHashCode();
            hash = (hash * 23) + this.Okay.GetHashCode();
            hash = (hash * 23) + this.Timeout.GetHashCode();

            return hash;
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents the current <see cref="AdbResponse"/>.
        /// </summary>
        /// <returns>
        /// <c>OK</c> if the response is an OK response, or <c>Error: {Message}</c> if the
        /// response indicates an error.
        /// </returns>
        public override string ToString()
        {
            if (this.Equals(AdbResponse.OK))
            {
                return "OK";
            }
            else
            {
                return $"Error: {this.Message}";
            }
        }
    }
}
