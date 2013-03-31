using System;
using System.Globalization;
using System.Text;

namespace SharpKml.Base
{
    /// <summary>Formats the value of an object to KML specification.</summary>
    internal class KmlFormatter : ICustomFormatter, IFormatProvider
    {
        private static KmlFormatter _instance = new KmlFormatter();
        private StringBuilder _formatter = new StringBuilder();

        private KmlFormatter()
        {
        }

        /// <summary>Gets the default instance of the KmlFormatter class.</summary>
        public static KmlFormatter Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// Converts the value of a specified object to an equivalent string
        /// representation using specified format and the invariant-culture
        /// formatting information.
        /// </summary>
        /// <param name="format">
        /// A format string containing formatting specifications.
        /// </param>
        /// <param name="arg">An object to format.</param>
        /// <param name="formatProvider">
        /// An IFormatProvider object that supplies format information about
        /// the current instance.
        /// </param>
        /// <returns>
        /// The string representation of the value of arg, formatted as specified
        /// by format and the invariant-culture.
        /// </returns>
        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            _formatter.Length = 0; // Clear the StringBuilder
            if (format == null)
            {
                if ((arg is double) || (arg is float) || (arg is decimal))
                {
                    // Return a maximum 15 meaningful digits
                    _formatter.AppendFormat(CultureInfo.InvariantCulture, "{0:#0.##############}", arg);
                    return _formatter.ToString();
                }

                DateTime? date = arg as DateTime?;
                if (date != null)
                {
                    return date.Value.ToString("yyyy-MM-ddTHH:mm:sszzzzzz", CultureInfo.InvariantCulture);
                }

                bool? boolean = arg as bool?;
                if (boolean != null)
                {
                    return boolean.Value ? "true" : "false"; // bool.ToString returns True or False, we need all lower case
                }
            }
            _formatter.AppendFormat(CultureInfo.InvariantCulture, "{0:" + format + "}", arg);
            return _formatter.ToString();
        }

        /// <summary>
        /// Returns an object that provides formatting services for the specified type.
        /// </summary>
        /// <param name="formatType">
        /// An object that specifies the type of format object to return.
        /// </param>
        /// <returns>
        /// This instance if the value parameter is ICustomFormatter; otherwise, null.
        /// </returns>
        /// <remarks>Part of the IFormatProvider interface.</remarks>
        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter))
            {
                return this;
            }
            return null;
        }
    }
}
