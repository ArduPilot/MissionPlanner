// <copyright file="FramebufferHeader.cs" company="The Android Open Source Project, Ryan Conrad, Quamotion">
// Copyright (c) The Android Open Source Project, Ryan Conrad, Quamotion. All rights reserved.
// </copyright>

namespace SharpAdbClient
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>
    /// Whenever the <c>framebuffer:</c> service is invoked, the adb server responds with the contents
    /// of the framebuffer, prefixed with a <see cref="FramebufferHeader"/> object that contains more
    /// information about the framebuffer.
    /// </summary>
    public struct FramebufferHeader
    {
        /// <summary>
        /// Gets or sets the version of the framebuffer sturcture.
        /// </summary>
        public uint Version { get; set; }

        /// <summary>
        /// Gets or sets the number of bytes per pixel. Usual values include 32 or 24.
        /// </summary>
        public uint Bpp { get; set; }

        /// <summary>
        /// Gets or sets the total size, in bits, of the framebuffer.
        /// </summary>
        public uint Size { get; set; }

        /// <summary>
        /// Gets or sets the width, in pixels, of the framebuffer.
        /// </summary>
        public uint Width { get; set; }

        /// <summary>
        /// Gets or sets the height, in pixels, of the framebuffer.
        /// </summary>
        public uint Height { get; set; }

        /// <summary>
        /// Gets or sets information about the red color channel.
        /// </summary>
        public ColorData Red { get; set; }

        /// <summary>
        /// Gets or sets information about the blue color channel.
        /// </summary>
        public ColorData Blue { get; set; }

        /// <summary>
        /// Gets or sets information about the green color channel.
        /// </summary>
        public ColorData Green { get; set; }

        /// <summary>
        /// Gets or sets information about the alpha channel.
        /// </summary>
        public ColorData Alpha { get; set; }

        /// <summary>
        /// Creates a new <see cref="FramebufferHeader"/> object based on a byte arra which contains the data.
        /// </summary>
        /// <param name="data">
        /// The data that feeds the <see cref="FramebufferHeader"/> structure.
        /// </param>
        /// <returns>
        /// A new <see cref="FramebufferHeader"/> object.
        /// </returns>
        public static FramebufferHeader Read(byte[] data)
        {
            // as defined in https://android.googlesource.com/platform/system/core/+/master/adb/framebuffer_service.cpp
            FramebufferHeader header = default(FramebufferHeader);

            // Read the data from a MemoryStream so we can use the BinaryReader to process the data.
            using (MemoryStream stream = new MemoryStream(data))
            using (BinaryReader reader = new BinaryReader(stream, Encoding.ASCII, leaveOpen: true))
            {
                header.Version = reader.ReadUInt32();
                header.Bpp = reader.ReadUInt32();
                header.Size = reader.ReadUInt32();
                header.Width = reader.ReadUInt32();
                header.Height = reader.ReadUInt32();
                header.Red = new ColorData()
                {
                    Offset = reader.ReadUInt32(),
                    Length = reader.ReadUInt32()
                };

                header.Blue = new ColorData()
                {
                    Offset = reader.ReadUInt32(),
                    Length = reader.ReadUInt32()
                };

                header.Green = new ColorData()
                {
                    Offset = reader.ReadUInt32(),
                    Length = reader.ReadUInt32()
                };

                header.Alpha = new ColorData()
                {
                    Offset = reader.ReadUInt32(),
                    Length = reader.ReadUInt32()
                };
            }

            return header;
        }

        /// <summary>
        /// Converts a <see cref="byte"/> array containing the raw frame buffer data to a <see cref="Image"/>.
        /// </summary>
        /// <param name="buffer">
        /// The buffer containing the image data.
        /// </param>
        /// <returns>
        /// A <see cref="Image"/> that represents the image contained in the frame buffer.
        /// </returns>
        public Image ToImage(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            // The pixel format of the framebuffer may not be one that .NET recognizes, so we need to fix that
            var pixelFormat = this.StandardizePixelFormat(buffer);

            Bitmap bitmap = new Bitmap((int)this.Width, (int)this.Height, pixelFormat);
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, pixelFormat);
            Marshal.Copy(buffer, 0, bitmapData.Scan0, buffer.Length);
            bitmap.UnlockBits(bitmapData);

            return bitmap;
        }

        /// <summary>
        /// Returns the <see cref="PixelFormat"/> that describes pixel format of an image that is stored according to the information
        /// present in this <see cref="FramebufferHeader"/>. Because the <see cref="PixelFormat"/> enumeration does not allow for all
        /// formats supported by Android, this method also takes a <paramref name="buffer"/> and reorganizes the bytes in the buffer to
        /// match the return value of this function.
        /// </summary>
        /// <param name="buffer">
        /// A byte array in which the images are stored according to this <see cref="FramebufferHeader"/>.
        /// </param>
        /// <returns>
        /// A <see cref="PixelFormat"/> that describes how the image data is represented in this <paramref name="buffer"/>.
        /// </returns>
        private PixelFormat StandardizePixelFormat(byte[] buffer)
        {
            // Initial parameter validation.
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (buffer.Length != this.Width * this.Height * (this.Bpp / 8))
            {
                throw new ArgumentOutOfRangeException(nameof(buffer));
            }

            // By far, the most common format is a 32-bit pixel format, which is either
            // RGB or RGBA, where each color has 1 byte.
            if (this.Bpp == 32)
            {
                // Require at leat RGB to be present; and require them to be exactly one byte (8 bits) long.
                if (this.Red.Length != 8
                    || this.Blue.Length != 8
                    || this.Green.Length != 8)
                {
                    throw new ArgumentOutOfRangeException();
                }

                // Alpha can be present or absent, but must be 8 bytes long
                if (this.Alpha.Length != 0 && this.Alpha.Length != 8)
                {
                    throw new ArgumentOutOfRangeException();
                }

                // Get the index at which the red, bue, green and alpha values are stored.
                uint redIndex = this.Red.Offset / 8;
                uint blueIndex = this.Blue.Offset / 8;
                uint greenIndex = this.Green.Offset / 8;
                uint alphaIndex = this.Alpha.Offset / 8;

                // Loop over the array and re-order as required
                for (int i = 0; i < buffer.Length; i += 4)
                {
                    byte red = buffer[i + redIndex];
                    byte blue = buffer[i + blueIndex];
                    byte green = buffer[i + greenIndex];
                    byte alpha = buffer[i + alphaIndex];

                    // Convert to ARGB. Note, we're on a little endian system,
                    // so it's really BGRA. Confusing!
                    if (this.Alpha.Length == 8)
                    {
                        buffer[i + 3] = alpha;
                        buffer[i + 2] = red;
                        buffer[i + 1] = green;
                        buffer[i + 0] = blue;
                    }
                    else
                    {
                        buffer[i + 3] = red;
                        buffer[i + 2] = green;
                        buffer[i + 1] = blue;
                        buffer[i + 0] = 0;
                    }
                }

                // Return RGB or RGBA, function of the presence of an alpha channel.
                if (this.Alpha.Length == 0)
                {
                    return PixelFormat.Format32bppRgb;
                }
                else
                {
                    return PixelFormat.Format32bppArgb;
                }
            }
            else if (this.Bpp == 24)
            {
                // For 24-bit image depths, we only support RGB.
                if (this.Red.Offset == 0
                    && this.Red.Length == 8
                    && this.Green.Offset == 8
                    && this.Green.Length == 8
                    && this.Blue.Offset == 16
                    && this.Blue.Length == 8
                    && this.Alpha.Offset == 24
                    && this.Alpha.Length == 0)
                {
                    return PixelFormat.Format24bppRgb;
                }
            }
            else if (this.Bpp == 16)
            {
                // For 16-bit image depths, we only support Rgb565\.
                if (this.Red.Offset == 11
                    && this.Red.Length == 5
                    && this.Green.Offset == 5
                    && this.Green.Length == 6
                    && this.Blue.Offset == 0
                    && this.Blue.Length == 5
                    && this.Alpha.Offset == 0
                    && this.Alpha.Length == 0)
                {
                    return PixelFormat.Format16bppRgb565;
                }
            }

            // If not caught by any of the statements before, the format is not supported.
            throw new NotSupportedException();
        }
    }
}
