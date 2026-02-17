/*  WinUSBNet library
 *  (C) 2010 Thomas Bleeker (www.madwizard.org)
 *
 *  Licensed under the MIT license, see license.txt or:
 *  http://www.opensource.org/licenses/mit-license.php
 */

using System;
using System.Diagnostics.CodeAnalysis;

namespace Nefarius.Drivers.WinUSB;

/// <summary>
///     Exception used by WinUSBNet to indicate errors. This is the
///     main exception to catch when using the library.
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public sealed class USBException : Exception
{
    /// <summary>
    ///     Constructs a new USBException with the given message
    /// </summary>
    /// <param name="message">The message describing the exception</param>
    internal USBException(string message)
        : base(message)
    {
    }

    /// <summary>
    ///     Constructs a new USBException with the given message and underlying exception
    ///     that caused the USBException.
    /// </summary>
    /// <param name="message">The message describing the exception</param>
    /// <param name="innerException">The underlying exception causing the USBException</param>
    internal USBException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
