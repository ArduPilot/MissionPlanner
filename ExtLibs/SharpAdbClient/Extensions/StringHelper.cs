// <copyright file="StringHelper.cs" company="The Android Open Source Project, Ryan Conrad, Quamotion">
// Copyright (c) The Android Open Source Project, Ryan Conrad, Quamotion. All rights reserved.
// </copyright>

namespace SharpAdbClient
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// Provides extension methods for working with <see cref="string"/> objects.
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        /// Matches the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public static Match Match(this string source, string pattern, RegexOptions options)
        {
            return Regex.Match(source, pattern, options);
        }

        /// <summary>
        /// Determines whether the specified source is match.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="pattern">The pattern.</param>
        /// <returns>
        ///   <see langword="true"/> if the specified source is match; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsMatch(this string source, string pattern)
        {
            return IsMatch(source, pattern, RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Determines whether the specified source is match.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="options">The options.</param>
        /// <returns>
        ///   <see langword="true"/> if the specified source is match; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsMatch(this string source, string pattern, RegexOptions options)
        {
            return Regex.IsMatch(source, pattern, options);
        }
    }
}
