#region netDxf library, Copyright (C) 2009-2016 Daniel Carvajal (haplokuon@gmail.com)

//                        netDxf library
// Copyright (C) 2009-2016 Daniel Carvajal (haplokuon@gmail.com)
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

using System;
using System.Collections;
using System.Reflection;

namespace netDxf
{
    #region Class StringEnum

    /// <summary>
    /// Helper class for working with 'extended' enums using <see cref="StringValueAttribute"/> attributes.
    /// </summary>
    public class StringEnum
    {
        #region Instance implementation

        private readonly Type enumType;
        private static readonly Hashtable stringValues = new Hashtable();

        /// <summary>
        /// Creates a new <see cref="StringEnum"/> instance.
        /// </summary>
        /// <param name="enumType">Enum type.</param>
        public StringEnum(Type enumType)
        {
            if (enumType == null)
                throw new ArgumentNullException(nameof(enumType));

            if (!enumType.IsEnum)
                throw new ArgumentException(string.Format("The supplied type \"{0}\" must be an Enum.", enumType));

            this.enumType = enumType;
        }

        /// <summary>
        /// Gets the string value associated with the given enum value.
        /// </summary>
        /// <param name="valueName">Name of the enum value.</param>
        /// <returns>String Value</returns>
        public string GetStringValue(string valueName)
        {
            string stringValue;

            try
            {
                Enum type = (Enum)Enum.Parse(this.enumType, valueName);
                stringValue = GetStringValue(type);
            }
            catch
            {
                return null;
            }

            return stringValue;
        }

        /// <summary>
        /// Gets the string values associated with the enum.
        /// </summary>
        /// <returns>String value array</returns>
        public Array GetStringValues()
        {
            ArrayList values = new ArrayList();
            //Look for our string value associated with fields in this enum
            foreach (FieldInfo fi in this.enumType.GetFields())
            {
                //Check for our custom attribute
                StringValueAttribute[] attrs = fi.GetCustomAttributes(typeof (StringValueAttribute), false) as StringValueAttribute[];
                if (attrs != null)
                    if (attrs.Length > 0)
                        values.Add(attrs[0].Value);
            }

            return values.ToArray();
        }

        /// <summary>
        /// Gets the values as a 'bindable' list data source.
        /// </summary>
        /// <returns>IList for data binding</returns>
        public IList GetListValues()
        {
            Type underlyingType = Enum.GetUnderlyingType(this.enumType);

            ArrayList values = new ArrayList();
            //Look for our string value associated with fields in this enum
            foreach (FieldInfo fi in this.enumType.GetFields())
            {
                //Check for our custom attribute
                StringValueAttribute[] attrs = fi.GetCustomAttributes(typeof (StringValueAttribute), false) as StringValueAttribute[];
                if (attrs != null)
                    if (attrs.Length > 0)
                    {
                        object str = Convert.ChangeType(Enum.Parse(this.enumType, fi.Name), underlyingType);
                        if (str == null)
                            throw new Exception();
                        values.Add(new DictionaryEntry(str, attrs[0].Value));
                    }
            }

            return values;
        }

        /// <summary>
        /// Return the existence of the given string value within the enum.
        /// </summary>
        /// <param name="value">String value.</param>
        /// <returns>Existence of the string value</returns>
        public bool IsStringDefined(string value)
        {
            return Parse(this.enumType, value) != null;
        }

        /// <summary>
        /// Return the existence of the given string value within the enum.
        /// </summary>
        /// <param name="value">String value.</param>
        /// <param name="comparisonType">Specifies how to conduct a case-insensitive match on the supplied string value</param>
        /// <returns>Existence of the string value</returns>
        public bool IsStringDefined(string value, StringComparison comparisonType)
        {
            return Parse(this.enumType, value, comparisonType) != null;
        }

        /// <summary>
        /// Gets the underlying enum type for this instance.
        /// </summary>
        /// <value></value>
        public Type EnumType
        {
            get { return this.enumType; }
        }

        #endregion

        #region Static implementation

        /// <summary>
        /// Gets a string value for a particular enum value.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <returns>String Value associated via a <see cref="StringValueAttribute"/> attribute, or null if not found.</returns>
        public static string GetStringValue(Enum value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            string output = null;
            Type type = value.GetType();

            if (stringValues.ContainsKey(value))
                output = ((StringValueAttribute) stringValues[value]).Value;
            else
            {
                //Look for our 'StringValueAttribute' in the field's custom attributes
                FieldInfo fi = type.GetField(value.ToString());
                StringValueAttribute[] attrs = fi.GetCustomAttributes(typeof (StringValueAttribute), false) as StringValueAttribute[];
                if (attrs != null)
                    if (attrs.Length > 0)
                    {
                        stringValues.Add(value, attrs[0]);
                        output = attrs[0].Value;
                    }
            }
            return output;
        }

        /// <summary>
        /// Parses the supplied enum and string value to find an associated enum value (case sensitive).
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="value">String value.</param>
        /// <returns>Enum value associated with the string value, or null if not found.</returns>
        public static object Parse(Type type, string value)
        {
            return Parse(type, value, StringComparison.Ordinal);
        }

        /// <summary>
        /// Parses the supplied enum and string value to find an associated enum value.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="value">String value.</param>
        /// <param name="comparisonType">Specifies how to conduct a case-insensitive match on the supplied string value.</param>
        /// <returns>Enum value associated with the string value, or null if not found.</returns>
        public static object Parse(Type type, string value, StringComparison comparisonType)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            object output = null;
            string enumStringValue = null;

            if (!type.IsEnum)
                throw new ArgumentException(string.Format("The supplied type \"{0}\" must be an Enum.", type));

            //Look for our string value associated with fields in this enum
            foreach (FieldInfo fi in type.GetFields())
            {
                //Check for our custom attribute
                StringValueAttribute[] attrs = fi.GetCustomAttributes(typeof (StringValueAttribute), false) as StringValueAttribute[];
                if (attrs != null)
                    if (attrs.Length > 0)
                        enumStringValue = attrs[0].Value;

                //Check for equality then select actual enum value.
                if (string.Compare(enumStringValue, value, comparisonType) == 0)
                {
                    if (Enum.IsDefined(type, fi.Name))
                        output = Enum.Parse(type, fi.Name);
                    break;
                }
            }

            return output;
        }

        /// <summary>
        /// Return the existence of the given string value within the enum.
        /// </summary>
        /// <param name="value">String value.</param>
        /// <param name="enumType">Type of enum</param>
        /// <returns>Existence of the string value</returns>
        public static bool IsStringDefined(Type enumType, string value)
        {
            return Parse(enumType, value) != null;
        }

        /// <summary>
        /// Return the existence of the given string value within the enum.
        /// </summary>
        /// <param name="value">String value.</param>
        /// <param name="enumType">Type of enum</param>
        /// <param name="comparisonType">Specifies to conduct a case-insensitive match on the supplied string value</param>
        /// <returns>Existence of the string value</returns>
        public static bool IsStringDefined(Type enumType, string value, StringComparison comparisonType)
        {
            return Parse(enumType, value, comparisonType) != null;
        }

        #endregion
    }

    #endregion

    #region Class StringValueAttribute

    /// <summary>
    /// Simple attribute class for storing String Values
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class StringValueAttribute : Attribute
    {
        private readonly string value;

        /// <summary>
        /// Creates a new <see cref="StringValueAttribute"/> instance.
        /// </summary>
        /// <param name="value">Value.</param>
        public StringValueAttribute(string value)
        {
            this.value = value;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value></value>
        public string Value
        {
            get { return this.value; }
        }
    }

    #endregion
}