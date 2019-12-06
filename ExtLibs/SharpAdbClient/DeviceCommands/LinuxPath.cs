// <copyright file="LinuxPath.cs" company="The Android Open Source Project, Ryan Conrad, Quamotion">
// Copyright (c) The Android Open Source Project, Ryan Conrad, Quamotion. All rights reserved.
// </copyright>

namespace SharpAdbClient.DeviceCommands
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Just like System.IO.Path, except it is geared for Linux
    /// </summary>
    public static class LinuxPath
    {
        /// <summary>
        /// The directory separator character.
        /// </summary>
        public const char DirectorySeparatorChar = '/';

        /// <summary>
        /// Pattern to escape filenames for shell command consumption.
        /// </summary>
        private const string EscapePattern = "([\\\\()*+?\"'#/\\s])";

        private static readonly char[] InvalidCharacters = new char[]
        {
            '|', '\\', '?', '*', '<', '\"', ':', '>', '+', '[', ']'
        };

        /// <summary>
        /// Combine the specified paths to form one path
        /// </summary>
        /// <param name="paths">The paths.</param>
        /// <returns></returns>
        public static string Combine(params string[] paths)
        {
            if (paths == null)
            {
                throw new ArgumentNullException(nameof(paths));
            }

            int capacity = 0;
            int num2 = 0;
            for (int i = 0; i < paths.Length; i++)
            {
                if (paths[i] == null)
                {
                    throw new ArgumentNullException(nameof(paths));
                }

                if (paths[i].Length != 0)
                {
                    CheckInvalidPathChars(paths[i]);
                    if (IsPathRooted(paths[i]))
                    {
                        num2 = i;
                        capacity = paths[i].Length;
                    }
                    else
                    {
                        capacity += paths[i].Length;
                    }

                    char ch = paths[i][paths[i].Length - 1];
                    if (ch != DirectorySeparatorChar)
                    {
                        capacity++;
                    }
                }
            }

            StringBuilder builder = new StringBuilder(capacity);
            for (int j = num2; j < paths.Length; j++)
            {
                if (paths[j].Length != 0)
                {
                    if (builder.Length == 0)
                    {
                        builder.Append(FixupPath(paths[j]));
                    }
                    else
                    {
                        char ch2 = builder[builder.Length - 1];
                        if (ch2 != DirectorySeparatorChar)
                        {
                            builder.Append(DirectorySeparatorChar);
                        }

                        builder.Append(paths[j]);
                    }
                }
            }

            return builder.ToString();
        }

        /// <summary>Returns the directory information for the specified path string.</summary>
        /// <returns>A <see cref="T:System.String"></see> containing directory information for path, or null if path denotes a root directory, is the empty string (""), or is null. Returns <see cref="F:System.String.Empty"></see> if path does not contain directory information.</returns>
        /// <param name="path">The path of a file or directory. </param>
        /// <exception cref="T:System.ArgumentException">The path parameter contains invalid characters, is empty, or contains only white spaces, or contains a wildcard character. </exception>
        /// <exception cref="T:System.IO.PathTooLongException">The path parameter is longer than the system-defined maximum length.</exception>
        /// <filterpriority>1</filterpriority>
        public static string GetDirectoryName(string path)
        {
            if (path != null)
            {
                CheckInvalidPathChars(path);

                string tpath = path;
                if (tpath.Length > 1)
                {
                    if (tpath.EndsWith(new string(new char[] { DirectorySeparatorChar })))
                    {
                        return tpath.Substring(0, tpath.Length);
                    }

                    tpath = tpath.Substring(0, tpath.LastIndexOf(DirectorySeparatorChar) + 1);

                    return FixupPath(tpath);
                }
                else if (tpath.Length == 1)
                {
                    return new string(new char[] { DirectorySeparatorChar });
                }
            }

            return null;
        }

        /// <summary>Returns the file name and extension of the specified path string.</summary>
        /// <returns>A <see cref="T:System.String"></see> consisting of the characters after the last directory character in path. If the last character of path is a directory or volume separator character, this method returns <see cref="F:System.String.Empty"></see>. If path is null, this method returns null.</returns>
        /// <param name="path">The path string from which to obtain the file name and extension. </param>
        /// <exception cref="T:System.ArgumentException">path contains one or more of the invalid characters defined in <see cref="F:System.IO.Path.InvalidPathChars"></see>, or contains a wildcard character. </exception>
        /// <filterpriority>1</filterpriority>
        public static string GetFileName(string path)
        {
            if (path != null)
            {
                CheckInvalidPathChars(path);
                int length = path.Length;
                int num2 = length;
                while (--num2 >= 0)
                {
                    char ch = path[num2];
                    if (ch == DirectorySeparatorChar)
                    {
                        return path.Substring(num2 + 1, (length - num2) - 1);
                    }
                }
            }

            return path;
        }

        /// <summary>Gets a value indicating whether the specified path string contains absolute or relative path information.</summary>
        /// <returns>true if path contains an absolute path; otherwise, false.</returns>
        /// <param name="path">The path to test. </param>
        /// <exception cref="T:System.ArgumentException">path contains one or more of the invalid characters defined in <see cref="F:System.IO.Path.InvalidPathChars"></see>, or contains a wildcard character. </exception>
        /// <filterpriority>1</filterpriority>
        public static bool IsPathRooted(string path)
        {
            if (path != null)
            {
                CheckInvalidPathChars(path);
                int length = path.Length;
                if ((length >= 1 && (path[0] == DirectorySeparatorChar)) ||
                    (length == 1))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns an escaped version of the entry name.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static string Escape(string path)
        {
            return new Regex(EscapePattern).Replace(path, new MatchEvaluator(m => m.Result("\\\\$1")));
        }

        /// <summary>
        /// Quotes the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static string Quote(string path)
        {
            if (path.Contains(" "))
            {
                return string.Format("\"{0}\"", path);
            }
            else
            {
                return path;
            }
        }

        /// <summary>
        /// Checks the invalid path chars.
        /// </summary>
        /// <param name="path">The path.</param>
        internal static void CheckInvalidPathChars(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (path.ToCharArray().Any(c => c < 0x20 || InvalidCharacters.Contains(c)))
            {
                throw new ArgumentException("Path contains invalid characters");
            }
        }

        /// <summary>
        /// Fixups the path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        private static string FixupPath(string path)
        {
            string sb = path;
            sb = sb.Replace(System.IO.Path.DirectorySeparatorChar, DirectorySeparatorChar);

            if (sb != "." && !sb.StartsWith(new string(new char[] { DirectorySeparatorChar })))
            {
                sb = string.Format(".{0}{1}", DirectorySeparatorChar, sb);
            }

            if (!sb.EndsWith(new string(new char[] { DirectorySeparatorChar })))
            {
                sb = string.Format("{0}{1}", sb, DirectorySeparatorChar);
            }

            sb = sb.Replace("//", new string(new char[] { DirectorySeparatorChar }));

            return sb;
        }
    }
}
