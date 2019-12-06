// <copyright file="ColorData.cs" company="The Android Open Source Project, Ryan Conrad, Quamotion">
// Copyright (c) The Android Open Source Project, Ryan Conrad, Quamotion. All rights reserved.
// </copyright>

namespace SharpAdbClient
{
    /// <summary>
    /// <para>
    /// Contains information about a color element. Information about each pixel on the screen
    /// is contained in a byte array (for example, a 4-byte array for 32-bit color depths), and
    /// a certain number of bits are reserved for each color.
    /// </para>
    /// <para>
    /// For example, in a 24-bit RGB structure, the first byte contains the red color,
    /// the next byte the green color and the last byte the blue color.
    /// </para>
    /// </summary>
    public struct ColorData
    {
        /// <summary>
        /// Gets or sets the number of bits that contain information for this color.
        /// </summary>
        public uint Length { get; set; }

        /// <summary>
        /// Gets or sets the offset, in bits, within the byte array for a pixel, at which the
        /// bytes that contain information for this color are stored.
        /// </summary>
        public uint Offset { get; set; }
    }
}
