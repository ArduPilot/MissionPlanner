using System;
using System.Collections.Generic;
using System.Reflection;
using SharpKml.Dom;

namespace SharpKml.Base
{
    /// <summary>
    /// Creates a derived class of <see cref="Element"/> from an XML element.
    /// </summary>
    public static class KmlFactory
    {
        // We need two so we can do reverse lookups
        private static Dictionary<XmlComponent, Type> _types = new Dictionary<XmlComponent, Type>();
        private static Dictionary<Type, XmlComponent> _names = new Dictionary<Type, XmlComponent>();

        /// <summary>Initializes static members of the KmlFactory class.</summary>
        static KmlFactory()
        {
            // Register all the sub-classes of Element that are
            // in the same assembly as Element.
            RegisterAssembly(Assembly.GetAssembly(typeof(Element)));
        }

        /// <summary>
        /// Creates a derived class of <see cref="Element"/> based on the XML
        /// information.
        /// </summary>
        /// <param name="xml">The XML information of the element.</param>
        /// <returns>
        /// A derived class of <c>Element</c> if the specified information was
        /// found; otherwise, null.
        /// </returns>
        /// <exception cref="System.MemberAccessException">
        /// Cannot create an instance of an abstract class, or this member was
        /// invoked with a late-binding mechanism.
        /// </exception>
        /// <exception cref="System.MethodAccessException">
        /// The caller does not have permission to call this constructor.
        /// </exception>
        /// <exception cref="System.MissingMethodException">
        /// No matching public constructor was found.
        /// </exception>
        /// <exception cref="System.Reflection.TargetInvocationException">
        /// The constructor being called throws an exception.
        /// </exception>
        public static Element CreateElement(XmlComponent xml)
        {
            Type type;
            if (_types.TryGetValue(xml, out type))
            {
                return (Element)Activator.CreateInstance(type);
            }
            return null;
        }

        /// <summary>
        /// Gets the XML information associated with the specified type.
        /// </summary>
        /// <param name="type">The type to search for.</param>
        /// <returns>
        /// An XmlComponent containing the XML information associated with the
        /// specified type if the type was found; otherwise, null.
        /// </returns>
        /// <exception cref="ArgumentNullException">type is null.</exception>
        public static XmlComponent FindType(Type type)
        {
            XmlComponent value;
            if (_names.TryGetValue(type, out value)) // Will throw is type is null.
            {
                return value;
            }
            return null;
        }

        /// <summary>
        /// Registers the specified type with the specified XML name and
        /// namespace URI.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the class to be registered, deriving from
        /// <see cref="Element"/>.
        /// </typeparam>
        /// <param name="xml">The XML information of the element.</param>
        /// <exception cref="ArgumentNullException">xml is null.</exception>
        /// <exception cref="ArgumentException">
        /// The type has already been registered or another type with the
        /// same XML name and namespace URI has been already registered.
        /// </exception>
        public static void Register<T>(XmlComponent xml) where T : Element
        {
            if (xml == null)
            {
                throw new ArgumentNullException("xml");
            }

            RegisterType(xml.Clone(), typeof(T)); // Don't store what the user passed us
        }

        // Private helper function to ensure both dicionaries are updated.
        private static void RegisterType(XmlComponent xml, Type type)
        {
            if (_names.ContainsKey(type))
            {
                throw new ArgumentException("Class type has already been registered.");
            }

            if (_types.ContainsKey(xml))
            {
                throw new ArgumentException("Another type has been registered with the specified XML qualified name.");
            }

            _names.Add(type, xml);
            _types.Add(xml, type);
        }

        private static void RegisterAssembly(Assembly assembly)
        {
            foreach (var type in assembly.GetExportedTypes())
            {
                if (type.IsSubclassOf(typeof(Element)))
                {
                    KmlElementAttribute element = TypeBrowser.GetElement(type);
                    if (element != null)
                    {
                        var xml = new XmlComponent(null, element.ElementName, element.Namespace);
                        RegisterType(xml, type);
                    }
                }
            }
        }
    }
}
