using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace MissionPlanner.Utilities.Drawing
{
    public sealed class ColorPalette
    {
        private int flags;

        private Color[] entries;

        /// <summary>Gets a value that specifies how to interpret the color information in the array of colors.</summary>
        /// <returns>The following flag values are valid: 0x00000001The color values in the array contain alpha information. 0x00000002The colors in the array are grayscale values. 0x00000004The colors in the array are halftone values. </returns>
        public int Flags => flags;

        /// <summary>Gets an array of <see cref="T:System.Drawing.Color" /> structures.</summary>
        /// <returns>The array of <see cref="T:System.Drawing.Color" /> structure that make up this <see cref="T:System.Drawing.Imaging.ColorPalette" />.</returns>
        public Color[] Entries => entries;

        internal ColorPalette(int count)
        {
            entries = new Color[count];
        }

        internal ColorPalette()
        {
            entries = new Color[1];
        }

        internal void ConvertFromMemory(IntPtr memory)
        {
            flags = Marshal.ReadInt32(memory);
            int num = Marshal.ReadInt32((IntPtr)((long)memory + 4));
            entries = new Color[num];
            for (int i = 0; i < num; i++)
            {
                int argb = Marshal.ReadInt32((IntPtr)((long)memory + 8 + i * 4));
                entries[i] = Color.FromArgb(argb);
            }
        }

        internal IntPtr ConvertToMemory()
        {
            int num = entries.Length;
            IntPtr intPtr;
            checked
            {
                intPtr = Marshal.AllocHGlobal(4 * (2 + num));
                Marshal.WriteInt32(intPtr, 0, flags);
                Marshal.WriteInt32((IntPtr)((long)intPtr + 4), 0, num);
            }
            for (int i = 0; i < num; i++)
            {
                Marshal.WriteInt32((IntPtr)((long)intPtr + 4 * (i + 2)), 0, entries[i].ToArgb());
            }
            return intPtr;
        }
    }
}