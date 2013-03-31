using System;
using System.Globalization;
using System.Reflection;

namespace SharpKml.Base
{
    /// <summary>
    /// Converts a string to an object according to the Kml specification.
    /// </summary>
    internal static class ValueConverter
    {
        // These are the only valid DateTime formats
        private static readonly string[] dateTimeFormats =
        {
            "yyyy", // xsd:gYear
            "yyyy-MM", // xsd:gYearMonth
            "yyyy-MM-dd", // xsd:date
            "yyyy-MM-ddTHH:mm:ssZ", // xsd:dateTime
            "yyyy-MM-ddTHH:mm:sszzzzzz" // xsd:dateTime
        };

        /// <summary>Tries to convert the specified string to an object.</summary>
        /// <param name="type">The type the value should be converted to.</param>
        /// <param name="text">The string to convert.</param>
        /// <param name="value">The output, if successful; otherwise, null.</param>
        /// <returns>
        /// true if the specified string was converted to the specified type;
        /// otherwise, false.
        /// </returns>
        public static bool TryGetValue(Type type, string text, out object value)
        {
            if (type.IsEnum)
            {
                value = GetEnum(type, text);
            }
            else if (type == typeof(bool)) // bool.TryParse doesn't work in our situation
            {
                value = GetBool(text);
            }
            else if (type.IsPrimitive)
            {
                value = GetPrimitive(type, text);
            }
            else if (type == typeof(string))
            {
                value = text;
            }
            else if (type == typeof(Color32))
            {
                value = Color32.Parse(text);
            }
            else if (type == typeof(DateTime))
            {
                value = GetDateTime(text);
            }
            else if (type == typeof(Uri))
            {
                Uri uri;
                Uri.TryCreate(text, UriKind.RelativeOrAbsolute, out uri);
                value = uri; // Will be null if TryCreate failed
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Unknown type:" + type);
                value = null;
                return false;
            }
            return true;
        }

        // A bool can be either true/1 or false/0
        private static object GetBool(string value)
        {
            if (value != null)
            {
                value = value.Trim();
                if (value.Equals("true", StringComparison.Ordinal) ||
                    value.Equals("1", StringComparison.Ordinal))
                {
                    return true;
                }
                else if (value.Equals("false", StringComparison.Ordinal) ||
                         value.Equals("0", StringComparison.Ordinal))
                {
                    return false;
                }
            }
            return null;
        }

        private static object GetDateTime(string value)
        {
            const DateTimeStyles Style =
                DateTimeStyles.AdjustToUniversal |
                DateTimeStyles.AllowWhiteSpaces |
                DateTimeStyles.AssumeUniversal;

            DateTime date;
            if (DateTime.TryParseExact(value, dateTimeFormats, CultureInfo.InvariantCulture, Style, out date))
            {
                return date;
            }
            return null;
        }

        private static object GetEnum(Type type, string value)
        {
            if (value != null)
            {
                value = value.Trim();
                foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Static))
                {
                    KmlElementAttribute element = TypeBrowser.GetElement(field);
                    if (element != null && string.Equals(element.ElementName, value, StringComparison.Ordinal))
                    {
                        return field.GetValue(null);
                    }
                }
            }
            return null;
        }

        // Only called on Primitive types, as these all have a TryParse method
        private static object GetPrimitive(Type type, string value)
        {
            // Get the TryParse method
            MethodInfo tryParse = type.GetMethod("TryParse", new Type[] { typeof(string), type.MakeByRefType() });
            System.Diagnostics.Debug.Assert(tryParse != null, "TryParse method not found.");

            object[] parameters = { value, null }; // null will be filled by TryParse
            if ((bool)tryParse.Invoke(null, parameters))
            {
                return parameters[1];
            }
            return null; // Failed to convert
        }
    }
}
