// <copyright file="ForwardData.cs" company="The Android Open Source Project, Ryan Conrad, Quamotion">
// Copyright (c) The Android Open Source Project, Ryan Conrad, Quamotion. All rights reserved.
// </copyright>

namespace SharpAdbClient
{
    /// <summary>
    /// Contains information about port forwarding configured by the Android Debug Bridge.
    /// </summary>
    public class ForwardData
    {
        /// <summary>
        /// Gets or sets the serial number of the device for which the port forwarding is
        /// configured.
        /// </summary>
        public string SerialNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a <see cref="string"/> that represents the local (PC) endpoint.
        /// </summary>
        public string Local
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a <see cref="ForwardSpec"/> that represents the local (PC) endpoint.
        /// </summary>
        public ForwardSpec LocalSpec
        {
            get
            {
                return ForwardSpec.Parse(this.Local);
            }
        }

        /// <summary>
        /// Gets or sets a <see cref="string"/> that represents the remote (device) endpoint.
        /// </summary>
        public string Remote
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a <see cref="ForwardSpec"/> that represents the remote (device) endpoint.
        /// </summary>
        public ForwardSpec RemoteSpec
        {
            get
            {
                return ForwardSpec.Parse(this.Remote);
            }
        }

        /// <summary>
        /// Creates a new instance of the <seealso cref="ForwardData"/> class by parsing
        /// a <see cref="string"/>.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> value to parse.
        /// </param>
        /// <returns>
        /// A <see cref="ForwardData"/> object that represents the port forwarding information
        /// contained in <paramref name="value"/>.
        /// </returns>
        public static ForwardData FromString(string value)
        {
            if (value == null)
            {
                return null;
            }

            string[] parts = value.Split(' ');
            return new ForwardData()
            {
                SerialNumber = parts[0],
                Local = parts[1],
                Remote = parts[2]
            };
        }
    }
}
