using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Ionic.Zip;

namespace SharpKml.Engine
{
    /// <summary>
    /// Represents a Kmz archive, containing Kml data and associated files.
    /// </summary>
    /// <remarks>
    /// The entire Kmz archive (in its compressed state) will be held in memory
    /// until a call to <see cref="KmzFile.Dispose"/> is made.
    /// </remarks>
    public sealed class KmzFile : IDisposable
    {
        // This is the default name for writing a KML file to a new archive,
        // however, the default file for reading from an archive is the first
        // file in the table of contents that ends with ".kml".
        private const string DefaultKmlFilename = "doc.kml";
        private ZipFile _zip;

        // The whole ZipFile will be saved to our stream so that we can check
        // the zip when it's loaded so we're no creating random exceptions when
        // accessing the ZipFile.
        // Also, ZipExntry.Extract will throw an exception if we try to call it
        // and the ZipFile has been modified (e.g. ZipFile.UpdateEntry)
        private MemoryStream _zipStream;

        private KmzFile(MemoryStream stream)
        {
            _zipStream = stream;
        }

        /// <summary>
        /// Gets the filenames for the entries contained in the archive.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// <see cref="Dispose"/> has been called on this instance or the
        /// stream was closed.
        /// </exception>
        public IEnumerable<string> Files
        {
            get
            {
                this.ThrowIfDisposed();
                return _zip.EntryFileNames;
            }
        }

        /// <summary>
        /// Creates a new KmzFile using the data specified in the KmlFile.
        /// </summary>
        /// <param name="kml">The Kml data to add to the archive.</param>
        /// <returns>
        /// A new KmzFile populated with the data specified in the KmlFile.
        /// </returns>
        /// <remarks>
        /// This overloaded version does not resolve any links in the Kml data
        /// and, therefore, will not add any local references to the archive.
        /// </remarks>
        /// <exception cref="ArgumentNullException">kml is null.</exception>
        public static KmzFile Create(KmlFile kml)
        {
            if (kml == null)
            {
                throw new ArgumentNullException("kml");
            }

            var instance = new KmzFile(new MemoryStream());
            instance._zip = new ZipFile();

            // Add the Kml data
            using (var stream = new MemoryStream())
            {
                kml.Save(stream);
                instance.AddFile(DefaultKmlFilename, stream.ToArray());
            }
            return instance;
        }

        /// <summary>
        /// Creates a new KmzFile using the data specified in the Kml file.
        /// </summary>
        /// <param name="path">The path of the Kml file.</param>
        /// <returns>
        /// A new KmzFile populated with the data specified in the Kml file.
        /// </returns>
        /// <remarks>
        /// Any local references in the file are written to the Kmz as archived
        /// resources if the resource URI is relative to and below the location
        /// of the Kml file. This means all absolute paths, such as
        /// &lt;href&gt;/etc/passwd&lt;/href&gt;, are ignored, as well as
        /// relative paths that point to point to an object that is not in
        /// the same folder or subfolder of the Kml file, e.g.
        /// &lt;href&gt;../../etc/passwd&lt;/href&gt; will be ignored for the
        /// file "/home/libkml/kmlfile.kml".
        /// </remarks>
        /// <exception cref="ArgumentNullException">path is null.</exception>
        /// <exception cref="IOException">An I/O error occurred.</exception>
        /// <exception cref="System.Xml.XmlException">
        /// An error occurred while parsing the KML.
        /// </exception>
        public static KmzFile Create(string path)
        {
            // We'll need the base url for relative Url's later
            string basePath = Path.GetDirectoryName(path);
            byte[] data = FileHandler.ReadBytes(new Uri(path, UriKind.RelativeOrAbsolute));

            // Find all the links in the Kml
            LinkResolver links;
            using (var stream = new MemoryStream(data, false))
            using (var reader = new StreamReader(stream))
            {
                links = new LinkResolver(reader, false);
            }

            // Now, if nothing threw, create the actual Kmz file
            var instance = new KmzFile(new MemoryStream());
            instance._zip = new ZipFile();

            // The first file in the archive with a .kml extension is the default
            // file, so make sure we add the original Kml file first.
            instance.AddFile(DefaultKmlFilename, data);

            try
            {
                // Next gather the local references and add them.
                foreach (var link in links.RelativePaths)
                {
                    // Make sure it doesn't point to a directory below the base path
                    if (!link.StartsWith("..", StringComparison.Ordinal))
                    {
                        Uri uri = new Uri(Path.Combine(basePath, link), UriKind.RelativeOrAbsolute);
                        byte[] file = FileHandler.ReadBytes(uri);
                        instance.AddFile(link, file);
                    }
                }
            }
            catch // Make sure we don't leak anything
            {
                instance.Dispose();
                throw;
            }

            return instance;
        }

        /// <summary>Opens a KmzFile from the specified stream.</summary>
        /// <param name="stream">The stream to read the data from.</param>
        /// <returns>A KmzFile representing the specified stream.</returns>
        /// <exception cref="ArgumentNullException">stream is null.</exception>
        /// <exception cref="InvalidDataException">
        /// The Kmz archive is not in the expected format.
        /// </exception>
        /// <exception cref="IOException">An I/O error occurred.</exception>
        /// <exception cref="NotSupportedException">
        /// The stream does not support reading.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// The stream was closed.
        /// </exception>
        public static KmzFile Open(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            var memory = new MemoryStream();
            try
            {
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, (int)stream.Length);
                memory.Write(buffer, 0, buffer.Length);
            }
            catch
            {
                memory.Dispose();
                throw;
            }

            // Check if it's a valid Zip file
            memory.Position = 0;
            if (!ZipFile.IsZipFile(memory, true))
            {
                memory.Dispose();
                throw new InvalidDataException("The Kmz archive is not in the expected format.");
            }

            // Everything's ok
            var instance = new KmzFile(memory);
            instance.ResetZip(false);
            return instance;
        }

        /// <summary>Creates a KmzFile from the specified file path.</summary>
        /// <param name="path">
        /// The URI for the file containing the KMZ data.
        /// </param>
        /// <returns>A KmzFile representing the specified file path.</returns>
        /// <exception cref="ArgumentNullException">path is null.</exception>
        /// <exception cref="InvalidDataException">
        /// The Kmz archive is not in the expected format.
        /// </exception>
        /// <exception cref="IOException">An I/O error occurred.</exception>
        public static KmzFile Open(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            using (var stream = FileHandler.OpenRead(new Uri(path, UriKind.RelativeOrAbsolute)))
            {
                return Open(stream);
            }
        }

        /// <summary>
        /// Adds the specified data to the Kmz archive, using the specified
        /// filename and directory path within the archive.
        /// </summary>
        /// <param name="path">
        /// The name, including any path, to use within the archive.
        /// </param>
        /// <param name="data">The data to add to the archive.</param>
        /// <exception cref="ArgumentException">
        /// path is a zero-length string, contains only white space, or contains
        /// one or more invalid characters as defined by
        /// <see cref="Path.GetInvalidPathChars"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">path/data is null.</exception>
        /// <exception cref="ObjectDisposedException">
        /// <see cref="Dispose"/> has been called on this instance.
        /// </exception>
        public void AddFile(string path, byte[] data)
        {
            this.ThrowIfDisposed();
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            // GetPathRoot will validate the path for us. If an absolute path
            // is passed in then GetPathRoot will return the root directory
            // (i.e. not return an empty string). However, a relative path of
            // "../directory/file.name" will pass the test, so we manually check
            // for that case, as AddEntry is quite lax compared to the C++ version.
            if (!string.IsNullOrEmpty(Path.GetPathRoot(path)) ||
                path.StartsWith(".", StringComparison.Ordinal))
            {
                throw new ArgumentException("path is invalid.", "path");
            }

            try
            {
                _zip.AddEntry(path, data);
            }
            catch (ZipException)
            {
                throw new ArgumentException("path is invalid.", "path");
            }
        }

        /// <summary>Releases all resources used by this instance.</summary>
        public void Dispose()
        {
            if (_zip != null)
            {
                _zip.Dispose();
                _zip = null;
            }

            if (_zipStream != null)
            {
                _zipStream.Dispose();
                _zipStream = null;
            }
        }

        /// <summary>Extracts the specified file from the Kmz archive.</summary>
        /// <param name="path">
        /// The file, including directory information, to locate in the archive.
        /// </param>
        /// <returns>
        /// A byte array if the specified value parameter was found in the
        /// archive; otherwise, null.
        /// </returns>
        /// <exception cref="ObjectDisposedException">
        /// <see cref="Dispose"/> has been called on this instance.
        /// </exception>
        public byte[] ReadFile(string path)
        {
            this.ThrowIfDisposed();

            if (!string.IsNullOrEmpty(path))
            {
                // This will return null if the path is not found
                ZipEntry file = _zip[path];
                if (file != null)
                {
                    return this.ExtractResource(file);
                }
            }
            return null;
        }

        /// <summary>Extracts the default Kml file from the archive.</summary>
        /// <returns>
        /// A string containing the Kml content if a suitable file was found in
        /// the Kmz archive; otherwise, null.
        /// </returns>
        /// <remarks>
        /// This returns the first file in the Kmz archive table of contents
        /// which has a ".kml" extension. Note that the file found may not
        /// necessarily be in the root directory.
        /// </remarks>
        /// <exception cref="ObjectDisposedException">
        /// <see cref="Dispose"/> has been called on this instance.
        /// </exception>
        public string ReadKml()
        {
            this.ThrowIfDisposed();

            ZipEntry kml = _zip.FirstOrDefault(
                e => string.Equals(".kml", Path.GetExtension(e.FileName), StringComparison.OrdinalIgnoreCase));

            if (kml != null)
            {
                return ASCIIEncoding.Default.GetString(this.ExtractResource(kml));
            }
            return null;
        }

        /// <summary>Removes the specified file from the Kmz archive.</summary>
        /// <param name="path">
        /// The file, including directory information, to locate in the archive.
        /// </param>
        /// <returns>
        /// true if the specified file was found in the archive and successfully
        /// removed; otherwise, false.
        /// </returns>
        /// <exception cref="ObjectDisposedException">
        /// <see cref="Dispose"/> has been called on this instance.
        /// </exception>
        public bool RemoveFile(string path)
        {
            this.ThrowIfDisposed();

            if (!string.IsNullOrEmpty(path))
            {
                // By default RemoveEntry throws an ArgumentException if the file's
                // not found, so check first to avoid the exception
                if (_zip.ContainsEntry(path))
                {
                    _zip.RemoveEntry(path);
                    return true;
                }
            }
            return false;
        }

        /// <summary>Saves this instance to the specified stream.</summary>
        /// <param name="stream">The stream to write to.</param>
        /// <exception cref="ArgumentException">stream is not writable.</exception>
        /// <exception cref="ArgumentNullException">stream is null.</exception>
        /// <exception cref="IOException">An I/O error occurred.</exception>
        /// <exception cref="NotSupportedException">
        /// The stream does not support writing.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// <see cref="Dispose"/> has been called on this instance or the
        /// stream was closed.
        /// </exception>
        public void Save(Stream stream)
        {
            this.ThrowIfDisposed();
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            // ZipFile will hold on to the passed in stream, so we need to save
            // to our private stream and then copy that to the stream.
            this.ResetZip(true);
            _zipStream.Position = 0;

            byte[] buffer = new byte[_zipStream.Length];
            _zipStream.Read(buffer, 0, (int)_zipStream.Length);
            stream.Write(buffer, 0, buffer.Length);

            //_zipStream.CopyTo(stream);
        }

        /// <summary>Saves this instance to the specified path.</summary>
        /// <param name="path">The complete file path to write to.</param>
        /// <remarks>
        /// If the specified file exists in the specified path then it will be
        /// overwritten; otherwise, a new file will be created.
        /// </remarks>
        /// <exception cref="ArgumentException">
        /// path is a zero-length string, contains only white space, or contains
        /// one or more invalid characters as defined by
        /// <see cref="Path.GetInvalidPathChars"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">path is null.</exception>
        /// <exception cref="DirectoryNotFoundException">
        /// The specified path is invalid.
        /// </exception>
        /// <exception cref="IOException">An I/O error occurred.</exception>
        /// <exception cref="NotSupportedException">
        /// path is in an invalid format.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// <see cref="Dispose"/> has been called on this instance.
        /// </exception>
        /// <exception cref="PathTooLongException">
        /// The specified path, file name, or both exceed the system-defined
        /// maximum length.
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        /// The caller does not have the required permission or path specified a
        /// file that is read-only.
        /// </exception>
        public void Save(string path)
        {
            this.ThrowIfDisposed();
            using (var file = File.Create(path))
            {
                this.Save(file);
            }
        }

        /// <summary>
        /// Replaces the specified file's contents in the Kmz archive with the
        /// specified data.
        /// </summary>
        /// <param name="path">
        /// The name, including any path, of the file within the archive.
        /// </param>
        /// <param name="data">The data to add to the archive.</param>
        /// <exception cref="ArgumentNullException">path/data is null.</exception>
        /// <exception cref="ObjectDisposedException">
        /// <see cref="Dispose"/> has been called on this instance.
        /// </exception>
        public void UpdateFile(string path, byte[] data)
        {
            this.ThrowIfDisposed();
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            // ZipFile.UpdateEntry will create the entry if does not exist
            if (_zip.ContainsEntry(path))
            {
                _zip.UpdateEntry(path, data);
            }
        }

        private byte[] ExtractResource(ZipEntry entry)
        {
            // ZipEntry.Extract will throw an exception if the ZipFile has been
            // modified. Check the VersionNeeded - if it's zero then the entry
            // needs to be saved before we can extract it.
            if (entry.VersionNeeded == 0)
            {
                this.ResetZip(true);
                entry = _zip[entry.FileName];
            }

            using (var stream = new MemoryStream())
            {
                entry.Extract(stream);
                return stream.ToArray();
            }
        }

        private void ResetZip(bool save)
        {
            if (save)
            {
                var copy = new MemoryStream();
                _zip.Save(copy);
                _zip.Dispose();

                _zipStream.Dispose();
                _zipStream = copy;
            }

            _zipStream.Position = 0;
            _zip = ZipFile.Read(_zipStream);
        }

        private void ThrowIfDisposed()
        {
            if (_zipStream == null) // _zipStream is set to null only in Dispose
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }
    }
}
