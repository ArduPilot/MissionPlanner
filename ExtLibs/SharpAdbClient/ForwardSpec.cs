// <copyright file="ForwardSpec.cs" company="The Android Open Source Project, Ryan Conrad, Quamotion">
// Copyright (c) The Android Open Source Project, Ryan Conrad, Quamotion. All rights reserved.
// </copyright>

namespace SharpAdbClient
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents an adb forward specification as used by the various adb port forwarding
    /// functions.
    /// </summary>
    public class ForwardSpec
    {
        /// <summary>
        /// Provides a mapping between a <see cref="string"/> and a <see cref="ForwardProtocol"/>
        /// value, used for the <see cref="string"/> representation of the <see cref="ForwardSpec"/>
        /// class.
        /// </summary>
        private static readonly Dictionary<string, ForwardProtocol> Mappings
            = new Dictionary<string, ForwardProtocol>(StringComparer.OrdinalIgnoreCase)
                {
                    { "tcp", ForwardProtocol.Tcp },
                    { "localabstract", ForwardProtocol.LocalAbstract },
                    { "localreserved", ForwardProtocol.LocalReserved },
                    { "localfilesystem", ForwardProtocol.LocalFilesystem },
                    { "dev", ForwardProtocol.Device },
                    { "jdwp", ForwardProtocol.JavaDebugWireProtocol }
                };

        /// <summary>
        /// Gets or sets the protocol which is being forwarded.
        /// </summary>
        public ForwardProtocol Protocol
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets, when the <see cref="Protocol"/> is <see cref="ForwardProtocol.Tcp"/>, the port
        /// number of the port being forwarded.
        /// </summary>
        public int Port
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets, when the <see cref="Protocol"/> is <see cref="ForwardProtocol.LocalAbstract"/>,
        /// <see cref="ForwardProtocol.LocalReserved"/> or <see cref="ForwardProtocol.LocalFilesystem"/>,
        /// the Unix domain socket name of the socket being forwarded.
        /// </summary>
        public string SocketName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets, when the <see cref="Protocol"/> is <see cref="ForwardProtocol.JavaDebugWireProtocol"/>,
        /// the process id of the process to which the debugger is attached.
        /// </summary>
        public int ProcessId
        {
            get;
            set;
        }

        /// <summary>
        /// Creates a <see cref="ForwardSpec"/> from its <see cref="string"/> representation.
        /// </summary>
        /// <param name="spec">
        /// A <see cref="string"/> which represents a <see cref="ForwardSpec"/>.
        /// </param>
        /// <returns>
        /// A <see cref="ForwardSpec"/> which represents <paramref name="spec"/>.
        /// </returns>
        public static ForwardSpec Parse(string spec)
        {
            if (spec == null)
            {
                throw new ArgumentNullException(nameof(spec));
            }

            var parts = spec.Split(new char[] { ':' }, 2, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2)
            {
                throw new ArgumentOutOfRangeException(nameof(spec));
            }

            if (!Mappings.ContainsKey(parts[0]))
            {
                throw new ArgumentOutOfRangeException(nameof(spec));
            }

            var protocol = Mappings[parts[0]];

            ForwardSpec value = new ForwardSpec();
            value.Protocol = protocol;

            int intValue;

            bool isInt = int.TryParse(parts[1], out intValue);

            switch (protocol)
            {
                case ForwardProtocol.JavaDebugWireProtocol:
                    if (!isInt)
                    {
                        throw new ArgumentOutOfRangeException(nameof(spec));
                    }

                    value.ProcessId = intValue;
                    break;

                case ForwardProtocol.Tcp:
                    if (!isInt)
                    {
                        throw new ArgumentOutOfRangeException(nameof(spec));
                    }

                    value.Port = intValue;
                    break;

                case ForwardProtocol.LocalAbstract:
                case ForwardProtocol.LocalFilesystem:
                case ForwardProtocol.LocalReserved:
                case ForwardProtocol.Device:
                    value.SocketName = parts[1];
                    break;
            }

            return value;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            var protocolString = Mappings.FirstOrDefault(v => v.Value == this.Protocol).Key;

            switch (this.Protocol)
            {
                case ForwardProtocol.JavaDebugWireProtocol:
                    return $"{protocolString}:{this.ProcessId}";

                case ForwardProtocol.Tcp:
                    return $"{protocolString}:{this.Port}";

                case ForwardProtocol.LocalAbstract:
                case ForwardProtocol.LocalFilesystem:
                case ForwardProtocol.LocalReserved:
                case ForwardProtocol.Device:
                    return $"{protocolString}:{this.SocketName}";

                default:
                    return string.Empty;
            }
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return (int)this.Protocol
                ^ this.Port
                ^ this.ProcessId
                ^ (this.SocketName == null ? 1 : this.SocketName.GetHashCode());
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            var other = obj as ForwardSpec;

            if (other == null)
            {
                return false;
            }

            if (other.Protocol != this.Protocol)
            {
                return false;
            }

            switch (this.Protocol)
            {
                case ForwardProtocol.JavaDebugWireProtocol:
                    return this.ProcessId == other.ProcessId;

                case ForwardProtocol.Tcp:
                    return this.Port == other.Port;

                case ForwardProtocol.LocalAbstract:
                case ForwardProtocol.LocalFilesystem:
                case ForwardProtocol.LocalReserved:
                case ForwardProtocol.Device:
                    return string.Equals(this.SocketName, other.SocketName);

                default:
                    return false;
            }
        }
    }
}
