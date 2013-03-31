using System;

namespace SharpKml.Engine
{
    /// <summary>
    /// Helper extensions for handling a Uri how the C++ version does.
    /// </summary>
    public static class UriExtensions
    {
        private const UriComponents NormalizeComponents = UriComponents.Fragment | UriComponents.PathAndQuery | UriComponents.UserInfo;
        private static readonly Uri AbsoluteUri = new Uri("http://example.com/", UriKind.Absolute);

        /// <summary>
        /// Returns the fragment of the Uri, even on Relative Uri's.
        /// </summary>
        /// <param name="uri">The Uri instance.</param>
        /// <returns>A string containing Uri fragment or null on a failure.</returns>
        /// <exception cref="ArgumentNullException">uri is null.</exception>
        public static string GetFragment(this Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException("uri");
            }

            if (uri.IsAbsoluteUri)
            {
                return uri.GetComponents(UriComponents.Fragment, UriFormat.Unescaped);
            }

            Uri absolute = Relative(AbsoluteUri, uri);
            if ((absolute != null) && absolute.IsAbsoluteUri) // Prevent infinite loop
            {
                return GetFragment(absolute); // Try again
            }
            return null; // Give up!
        }

        /// <summary>
        /// Returns the local path of the Uri, even on Relative Uri's.
        /// </summary>
        /// <param name="uri">The Uri instance.</param>
        /// <returns>A string containing Uri path or null on a failure.</returns>
        /// <exception cref="ArgumentNullException">uri is null.</exception>
        public static string GetPath(this Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException("uri");
            }

            if (uri.IsAbsoluteUri)
            {
                return uri.GetComponents(UriComponents.Path, UriFormat.Unescaped);
            }

            return GetComponents(uri, UriComponents.Path);
        }

        /// <summary>Returns the Uri of the Kmz file, if one is found.</summary>
        /// <param name="uri">The Uri instance.</param>
        /// <returns>The Uri of the Kmz file if found; otherwise, null.</returns>
        /// <exception cref="ArgumentNullException">uri is null.</exception>
        public static Uri KmzUrl(this Uri uri)
        {
            var split = SplitKmz(uri);
            return (split == null) ? null : split.Item1;
        }

        /// <summary>Normalizes both absolute relative Uri's.</summary>
        /// <param name="uri">The Uri instance.</param>
        /// <returns>A Uri with that is normalized.</returns>
        /// <exception cref="ArgumentNullException">uri is null.</exception>
        public static Uri Normalize(this Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException("uri");
            }

            // Absolute paths are already normalized
            if (!uri.IsAbsoluteUri)
            {
                string components = GetComponents(uri, NormalizeComponents);
                if (components != null)
                {
                    // Create a new Uri from the normalized components
                    return new Uri(components, UriKind.Relative);
                }
            }

            // If absolute or the GetComponents method failed, return a copy
            return Clone(uri);
        }

        /// <summary>Combines the instance and the specified target.</summary>
        /// <param name="uri">The Uri instance.</param>
        /// <param name="target">The relative Uri to add to the instance.</param>
        /// <returns>
        /// A new Uri containing both this instance and the target or null if
        /// this instane is not an absolute Uri.
        /// </returns>
        /// <exception cref="ArgumentNullException">uri is null.</exception>
        public static Uri Relative(this Uri uri, Uri target)
        {
            if (uri == null)
            {
                throw new ArgumentNullException("uri");
            }

            if (target == null)
            {
                return Clone(uri);
            }

            Uri output;
            Uri.TryCreate(uri, target, out output);
            return output;
        }

        /// <summary>Combines the Href's of a Model.</summary>
        /// <param name="uri">The Uri instance.</param>
        /// <param name="geometry">The Href of the geometry.</param>
        /// <param name="target">The Href of the target.</param>
        /// <returns>
        /// The full Uri of the combined info or null if this instance is not
        /// an absolute Uri.
        /// </returns>
        /// <exception cref="ArgumentNullException">uri is null.</exception>
        public static Uri Resolve(this Uri uri, Uri geometry, Uri target)
        {
            if (uri == null)
            {
                throw new ArgumentNullException("uri");
            }

            Uri geo = Relative(uri, geometry);
            return geo == null ? null : Relative(geo, target);
        }

        /// <summary>
        /// Separates a Uri specifying a Kmz file into the file's Uri and the
        /// relative Uri of a reference inside the Kmz file.
        /// </summary>
        /// <param name="uri">The Uri instance.</param>
        /// <returns>
        /// The Uri of the Kmz file and the Uri of the resource or null if
        /// this instance does not reference a Kmz file.
        /// </returns>
        /// <exception cref="ArgumentNullException">uri is null.</exception>
        public static Tuple<Uri, Uri> SplitKmz(this Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException("uri");
            }

            string path = uri.AbsolutePath;
            int index = path.IndexOf(".kmz", StringComparison.OrdinalIgnoreCase);
            if (index > 0)
            {
                index += 4; // Add ".kmz"
                UriBuilder builder = new UriBuilder(uri);
                builder.Path = path.Substring(0, index);

                if (index < path.Length - 1) // Allow for '/'
                {
                    Uri file = new Uri(path.Substring(index + 1), UriKind.Relative); // Skip '/'
                    return Tuple.Create(builder.Uri, file);
                }
                return new Tuple<Uri, Uri>(builder.Uri, null);
            }
            return null;
        }

        private static Uri Clone(Uri source)
        {
            return new Uri(source.OriginalString, source.IsAbsoluteUri ? UriKind.Absolute : UriKind.Relative);
        }

        private static string GetComponents(Uri relativeUri, UriComponents components)
        {
            // Uri.GetComponents can only be called on an absolute Uri.
            Uri absolute = Relative(AbsoluteUri, relativeUri);
            if (absolute != null)
            {
                string output = absolute.GetComponents(components, UriFormat.Unescaped);
                output = output.TrimStart('/');

                // We need to add any "../" at the start of the original string,
                // as they are lost by the conversion to an absolute.
                string original = relativeUri.OriginalString;
                while (original.StartsWith("../", StringComparison.Ordinal))
                {
                    output = "../" + output;
                    original = original.Substring(3); // Remove the "../" and test again
                }

                return output;
            }
            return null;
        }
    }
}
