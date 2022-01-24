using System;
using System.IO;
using System.Runtime.InteropServices;

namespace AltitudeAngelWings.Service.AltitudeAngelTelemetry.Encryption
{
    public static class BinaryIoExtensions
    {
        /// <summary>
        ///     Read a structure from a <see cref="BinaryReader" />
        /// </summary>
        /// <typeparam name="T">Type of struct to read</typeparam>
        /// <param name="reader">Reader instance</param>
        /// <returns>Struct of type {T}</returns>
        public static T ReadStruct<T>(this BinaryReader reader) where T : struct
        {
            int size = Marshal.SizeOf(typeof(T));

            return ReadStruct<T>(reader, size);
        }

        /// <summary>
        ///     Attempt to read a structure from a <see cref="BinaryReader" />
        /// </summary>
        /// <typeparam name="T">Type of struct to read</typeparam>
        /// <param name="reader">Reader instance</param>
        /// <param name="s">Return structure if successful</param>
        /// <returns>True if read succeeds, false otherwise</returns>
        public static bool TryReadStruct<T>(this BinaryReader reader, out T s) where T : struct
        {
            s = default(T);

            int size = Marshal.SizeOf(typeof(T));

            if (reader.BaseStream.Length - reader.BaseStream.Position < size)
            {
                return false;
            }

            s = ReadStruct<T>(reader, size);
            return true;
        }

        /// <summary>
        ///     Write a structure to a <see cref="BinaryWriter" />
        /// </summary>
        /// <typeparam name="T">Type of struct to write</typeparam>
        /// <param name="writer">Writer instance</param>
        /// <param name="item">structure instance</param>
        public static void WriteStruct<T>(this BinaryWriter writer, T item) where T : struct
        {
            int size = Marshal.SizeOf(typeof(T));
            byte[] array = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(item, ptr, true);
                Marshal.Copy(ptr, array, 0, size);
                writer.Write(array);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        public static long BytesRemaining(this BinaryReader reader)
            => reader.BaseStream.Length - reader.BaseStream.Position;

        private static T ReadStruct<T>(this BinaryReader reader, int size) where T : struct
        {
            IntPtr ptr = Marshal.AllocHGlobal(size);
            try
            {
                byte[] array = reader.ReadBytes(size);
                Marshal.Copy(array, 0, ptr, size);
                var s = (T)Marshal.PtrToStructure(ptr, typeof(T));
                return s;
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }
    }
}
