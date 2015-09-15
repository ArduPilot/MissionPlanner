using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SharpKml.Base
{
    /// <summary>
    /// Helper class for extracting properties with a KmlAttribute/KmlElement
    /// assigned to them, searching the entire inheritance hierarchy of the type.
    /// </summary>
    internal class TypeBrowser
    {
        // Used for a cache. This is very important for performance as it ruduces
        // the amount of work done in reflection, as the attributes associated
        // with a Type won't change during the lifetime of the program (ignoring
        // funky Emit etc. vodoo which this library doesn't use)
        private static Dictionary<Type, TypeBrowser> _types = new Dictionary<Type, TypeBrowser>();

        private Dictionary<XmlComponent, Tuple<PropertyInfo, KmlAttributeAttribute>> _attributes =
            new Dictionary<XmlComponent, Tuple<PropertyInfo, KmlAttributeAttribute>>();

        // Needs to be ordered
        private List<Tuple<XmlComponent, PropertyInfo, KmlElementAttribute>> _elements =
            new List<Tuple<XmlComponent, PropertyInfo, KmlElementAttribute>>();

        private TypeBrowser(Type type)
        {
            this.ExtractAttributes(type);
        }

        /// <summary>
        /// Gets the properties with a KmlAttribute attribute.
        /// </summary>
        public IEnumerable<Tuple<PropertyInfo, KmlAttributeAttribute>> Attributes
        {
            get { return _attributes.Values; }
        }

        /// <summary>Gets the properties with a KmlElement attribute.</summary>
        public IEnumerable<Tuple<PropertyInfo, KmlElementAttribute>> Elements
        {
            get
            {
                return from element in _elements
                       select Tuple.Create(element.Item2, element.Item3);
            }
        }

        /// <summary>Creates TypeBrowser representing the specified type.</summary>
        /// <param name="type">The type to extract properties from.</param>
        /// <returns>
        /// A TypeBroswer containing information about the specified type.
        /// </returns>
        public static TypeBrowser Create(Type type)
        {
            if (!_types.ContainsKey(type))
            {
                _types.Add(type, new TypeBrowser(type));
            }
            return _types[type];
        }

        /// <summary>
        /// Gets a KmlAttribute attribute associated with the specified
        /// ICustomAttributeProvider.
        /// </summary>
        /// <param name="provider">
        /// The ICustomAttributeProvider to retrieve the attribute from.
        /// </param>
        /// <returns>
        /// A KmlAttributeAttribute associated with the specified value parameter
        /// if one was found; otherwise, null.
        /// </returns>
        public static KmlAttributeAttribute GetAttribute(ICustomAttributeProvider provider)
        {
            return GetAttribute<KmlAttributeAttribute>(provider);
        }

        /// <summary>
        /// Gets a KmlElement attribute associated with the specified
        /// ICustomAttributeProvider.
        /// </summary>
        /// <param name="provider">
        /// The ICustomAttributeProvider to retrieve the attribute from.
        /// </param>
        /// <returns>
        /// A KmlElementAttribute associated with the specified value parameter
        /// if one was found; otherwise, null.
        /// </returns>
        public static KmlElementAttribute GetElement(ICustomAttributeProvider provider)
        {
            return GetAttribute<KmlElementAttribute>(provider);
        }


        /// <summary>
        /// Gets a KmlElement attribute associated with the specified Enum.
        /// </summary>
        /// <param name="value">
        /// The Enum value to retrieve the attribute from.
        /// </param>
        /// <returns>
        /// A KmlElementAttribute associated with the specified value parameter
        /// if one was found; otherwise, null.
        /// </returns>
        /// <exception cref="ArgumentNullException">value is null.</exception>
        public static KmlElementAttribute GetEnum(Enum value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            Type type = value.GetType();
            //string name = type.GetEnumName(value);

            var memInfo = type.GetMember(value.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(KmlElementAttribute), false);
            var description = ((KmlElementAttribute)attributes[0]).ElementName;

            string name = value.ToString();
            
            //string name = type.ToString();
            if (name != null)
            {
                return GetAttribute<KmlElementAttribute>(type.GetField(name));
            }
            return null;
        }

        /// <summary>
        /// Finds a property with the specified XML attribute information.
        /// </summary>
        /// <param name="xml">The XML information to find.</param>
        /// <returns>
        /// A PropertyInfo for the first property found matching the specified
        /// information or null if no matches were found.
        /// </returns>
        public PropertyInfo FindAttribute(XmlComponent xml)
        {
            Tuple<PropertyInfo, KmlAttributeAttribute> property;
            if (_attributes.TryGetValue(xml, out property))
            {
                return property.Item1;
            }
            return null;
        }

        /// <summary>
        /// Finds a property with the specified XML element information.
        /// </summary>
        /// <param name="xml">The XML information to find.</param>
        /// <returns>
        /// A PropertyInfo for the first property found matching the specified
        /// information or null if no matches were found.
        /// </returns>
        public PropertyInfo FindElement(XmlComponent xml)
        {
            var query = from element in _elements
                        where element.Item1.Equals(xml)
                        select element.Item2;

            return query.FirstOrDefault();
        }

        private static T GetAttribute<T>(ICustomAttributeProvider provider) where T : class
        {
            if (provider == null)
            {
                return null;
            }

            object[] attributes = provider.GetCustomAttributes(typeof(T), false);
            return (attributes.Length > 0) ? (T)attributes[0] : null;
        }

        private void ExtractAttributes(Type type)
        {
            if (type == null || type == typeof(object))
            {
                return; // We've reached the top, now we have to stop
            }

            // Look at the base type first as the KML schema specifies <sequence>
            // This will also find private fields in the base classes, which can't
            // be seen through a derived class.
            this.ExtractAttributes(type.BaseType);

            // Store the found elements here so we can add them in order later
            var elements = new List<Tuple<XmlComponent, PropertyInfo, KmlElementAttribute>>();
            const BindingFlags PropertyFlags = BindingFlags.DeclaredOnly |
                                               BindingFlags.Instance |
                                               BindingFlags.NonPublic |
                                               BindingFlags.Public;
            foreach (var property in type.GetProperties(PropertyFlags))
            {
                var attribute = GetAttribute(property);
                if (attribute != null)
                {
                    XmlComponent component = new XmlComponent(null, attribute.AttributeName, null);

                    // Check if a property has already been registered with the info.
                    // Ignore later properties - i.e. don't throw an exception.
                    if (!_attributes.ContainsKey(component))
                    {
                        _attributes.Add(component, Tuple.Create(property, attribute));
                    }
                }
                else
                {
                    var element = GetElement(property);
                    if (element != null)
                    {
                        XmlComponent component = new XmlComponent(null, element.ElementName, element.Namespace);
                        elements.Add(Tuple.Create(component, property, element));
                    }
                }
            }

            // Now add the elements in order
            _elements.AddRange(elements.OrderBy(e => e.Item3.Order));
        }
    }
}
