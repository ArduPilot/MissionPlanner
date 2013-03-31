using System;
using System.IO;
using System.Net;

namespace SharpKml.Engine
{
    /// <summary>
    /// Handles downloading of files from a URI, such as local files or network files.
    /// </summary>
    internal static class FileHandler
    {
        /// <summary>
        /// Reads a file from either http, ftp or local and returns a stream to
        /// its contents.
        /// </summary>
        /// <param name="uri">The uri to obtain the data from.</param>
        /// <returns>The file contents as a read-only stream.</returns>
        /// <exception cref="IOException">
        /// An error occurred reading the file. See the
        /// <see cref="Exception.InnerException"/> for more details.
        /// </exception>
        public static Stream OpenRead(Uri uri)
        {
            return new MemoryStream(ReadBytes(uri), false);
        }

        /// <summary>
        /// Reads a file from either http, ftp or local and returns its entire contents.
        /// </summary>
        /// <param name="uri">The uri to obtain the data from.</param>
        /// <returns>The file contents.</returns>
        /// <exception cref="IOException">
        /// An error occurred reading the file. See the
        /// <see cref="Exception.InnerException"/> for more details.
        /// </exception>
        public static byte[] ReadBytes(Uri uri)
        {
            // Try to convert from relative to absolute
            if (!uri.IsAbsoluteUri)
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), uri.OriginalString);
                if (!Uri.TryCreate(path, UriKind.Absolute, out uri))
                {
                    throw new IOException("Path is invalid.");
                }
            }

            var client = new WebClient();
            try
            {
                return client.DownloadData(uri);
            }
            catch (WebException ex)
            {
                throw new IOException("Unable to load file.", ex);
            }
            finally
            {
                client.Dispose();
            }
        }

        /// <summary>
        /// Reads a Kml file or the default Kml file from a Kmz archive.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        /// <returns>A KmlFile on success, or null on an error.</returns>
        public static KmlFile ReadFile(string path)
        {
            try
            {
                byte[] data = ReadBytes(new Uri(path, UriKind.RelativeOrAbsolute));
                using (var stream = new MemoryStream(data, false))
                {
                    string extension = Path.GetExtension(path);
                    if (string.Equals(extension, ".kml", StringComparison.OrdinalIgnoreCase))
                    {
                        return KmlFile.Load(stream);
                    }

                    if (string.Equals(extension, ".kmz", StringComparison.OrdinalIgnoreCase))
                    {
                        using (var kmz = KmzFile.Open(stream))
                        {
                            return KmlFile.LoadFromKmz(kmz);
                        }
                    }
                }
            }
            catch (IOException)
            {
                // Silently fail
            }
            return null;
        }
    }
}
