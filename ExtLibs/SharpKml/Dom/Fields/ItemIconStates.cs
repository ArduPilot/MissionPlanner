using System;
using SharpKml.Base;

namespace SharpKml.Dom
{
    /// <summary>
    /// Specifies the current state of a <see cref="NetworkLink"/> or
    /// <see cref="Folder"/>.
    /// </summary>
    /// <remarks>
    /// <para>OGC KML 2.2 Section 16.13</para>
    /// <para>This enumeration has a <see cref="FlagsAttribute"/> attribute
    /// that allows a bitwise combination of its member values.</para>
    /// </remarks>
    [Flags]
    public enum ItemIconStates
    {
        /// <summary>Indicates no value has been specified.</summary>
        None,

        /// <summary>Represents an open folder.</summary>
        [KmlElement("open")]
        Open = 0x01,

        /// <summary>Represents a closed folder.</summary>
        [KmlElement("closed")]
        Closed = 0x02,

        /// <summary>Represents an error in fetch.</summary>
        [KmlElement("error")]
        Error = 0x04,

        /// <summary>Represents a fetch state of 0.</summary>
        [KmlElement("fetching0")]
        Fetching0 = 0x08,

        /// <summary>Represents a fetch state of 1.</summary>
        [KmlElement("fetching1")]
        Fetching1 = 0x10,

        /// <summary>Represents a fetch state of 2.</summary>
        [KmlElement("fetching2")]
        Fetching2 = 0x20
    }


}
