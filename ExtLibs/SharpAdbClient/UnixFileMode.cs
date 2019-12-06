// <copyright file="UnixFileMode.cs" company="The Android Open Source Project, Ryan Conrad, Quamotion">
// Copyright (c) The Android Open Source Project, Ryan Conrad, Quamotion. All rights reserved.
// </copyright>

namespace SharpAdbClient
{
    using System;

    /// <summary>
    /// Describes the properties of a file on an Android device.
    /// </summary>
    [Flags]
    public enum UnixFileMode
    {
        /// <summary>
        /// The mask that can be used to retrieve the file type from
        /// a <see cref="UnixFileMode"/>.
        /// </summary>
        TypeMask = 0x8000,

        /// <summary>
        /// The file is a Unix socket.
        /// </summary>
        Socket = 0xc000,

        /// <summary>
        /// The file is a symbolic link.
        /// </summary>
        SymbolicLink = 0xa000,

        /// <summary>
        /// The file is a regular file.
        /// </summary>
        Regular = 0x8000,

        /// <summary>
        /// The file is a block device.
        /// </summary>
        Block = 0x6000,

        /// <summary>
        /// The file is a directory.
        /// </summary>
        Directory = 0x4000,

        /// <summary>
        /// The file is a character device.
        /// </summary>
        Character = 0x2000,

        /// <summary>
        /// The file is a first-in first-out queue.
        /// </summary>
        FIFO = 0x1000
    }
}
