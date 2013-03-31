using System;
using System.Collections.Generic;
using System.Reflection;
using SharpKml.Base;
using SharpKml.Dom;

namespace SharpKml.Engine
{
    /// <summary>
    /// Provides extension methods for <see cref="Element"/> objects.
    /// </summary>
    public static class ElementExtensions
    {
        /// <summary>
        /// Creates a deep copy of the <see cref="Element"/>.
        /// </summary>
        /// <typeparam name="T">A class deriving from <c>Element</c>.</typeparam>
        /// <param name="element">The class instance.</param>
        /// <returns>A new <c>Element</c> representing the same data.</returns>
        /// <exception cref="ArgumentNullException">element is null.</exception>
        public static T Clone<T>(this T element) where T : Element
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            // We're simply going to serialize it then parse it again
            // This could be rewritten to use reflection but this should
            // be fast enough.
            var serializer = new Serializer();
            var parser = new Parser();

            Element output;
            if (element.GetType() == typeof(IconStyle.IconLink)) // Special case as IconStyle has the same Kml name as Icon
            {
                if (element.Parent == null)
                {
                    var parent = new IconStyle();
                    parent.Icon = (IconStyle.IconLink)(Element)element; // Have to cast to Element first
                    serializer.Serialize(parent);
                    parent.Icon = null; // Sets the Icon's Parent property back to null
                }
                else
                {
                    serializer.Serialize(element.Parent);
                }
                parser.ParseString(serializer.Xml, true);
                IconStyle root = (IconStyle)parser.Root;
                output = root.Icon;
                root.Icon = null; // Clear the output's parent
            }
            else
            {
                serializer.Serialize(element);
                parser.ParseString(serializer.Xml, true);
                output = parser.Root;
            }
            return (T)output;
        }

        /// <summary>
        /// Provides a way to iterate over all children <see cref="Element"/>s
        /// contained by this instance.
        /// </summary>
        /// <param name="element">The class instance.</param>
        /// <returns>An IEnumerable&lt;Element&gt; for specified element.</returns>
        /// <exception cref="ArgumentNullException">element is null.</exception>
        public static IEnumerable<Element> Flatten(this Element element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return ElementWalker.Walk(element);
        }

        /// <summary>
        /// Finds the <see cref="Element.Parent"/> of the element which is
        /// of the specified type.
        /// </summary>
        /// <typeparam name="T">A type deriving from <c>Element</c>.</typeparam>
        /// <param name="element">The class instance.</param>
        /// <returns>
        /// The closest element in the hierarchy of the specified type or null
        /// if no elements in the hierarchy are of the specified type.
        /// </returns>
        /// <exception cref="ArgumentNullException">element is null.</exception>
        public static T GetParent<T>(this Element element) where T : Element
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            var parent = element.Parent;
            while (parent != null)
            {
                T typed = parent as T;
                if (typed != null) // parent is not null so the conversion must have worked
                {
                    return typed;
                }
                parent = parent.Parent;
            }
            return null;
        }

        /// <summary>
        /// Determines if the <see cref="Element"/> has a parent of the
        /// specified type.
        /// </summary>
        /// <typeparam name="T">A type deriving from <c>Element</c>.</typeparam>
        /// <param name="element">The class instance.</param>
        /// <returns>
        /// True if an element further up the hierarchy is of the specified type;
        /// otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException">element is null.</exception>
        public static bool IsChildOf<T>(this Element element) where T : Element
        {
            return GetParent<T>(element) != null;
        }

        /// <summary>
        /// Copies the <see cref="Element"/>s properties from the specified source.
        /// </summary>
        /// <typeparam name="T">A class deriving from <c>Element</c>.</typeparam>
        /// <param name="element">The class instance.</param>
        /// <param name="source">The element to copy from.</param>
        /// <exception cref="ArgumentException">
        /// element and source do not have the same type.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// element or source is null.
        /// </exception>
        public static void Merge<T>(this T element, T source) where T : Element
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (element.GetType() != source.GetType()) // Can happen if one is a more derived class
            {
                throw new ArgumentException("source type must match that of the target instance.");
            }

            if (!object.ReferenceEquals(element, source)) // Make sure we're not playing with ourselves... so to speak
            {
                foreach (var child in source.Children)
                {
                    element.AddChild(child.Clone());
                }
                Merge(source, element, source.GetType());
            }
        }

        private static object CreateClone(object value)
        {
            Type type = value.GetType();

            // First check if we can simple return the passed in value as value
            // types and strings are immutable. This also includes the Color32.
            if (type.IsValueType || type == typeof(string))
            {
                return value;
            }
            else if (type == typeof(Uri))
            {
                return new Uri(((Uri)value).OriginalString, UriKind.RelativeOrAbsolute);
            }
            return null;
        }

        private static void Merge(object source, object target, Type type)
        {
            if (type == null || type == typeof(Element))
            {
                return; // Can't go any higher.
            }

            const BindingFlags PropertyFlags = BindingFlags.DeclaredOnly |
                                               BindingFlags.Instance |
                                               BindingFlags.NonPublic |
                                               BindingFlags.Public;
            foreach (var property in type.GetProperties(PropertyFlags))
            {
                if (!property.CanWrite)
                {
                    continue;
                }

                object newValue = null;
                object value = property.GetValue(source, null);

                // First check if it's an element and merge any existing info.
                Element sourceElement = value as Element;
                if (sourceElement != null)
                {
                    newValue = MergeElements(sourceElement, (Element)property.GetValue(target, null));
                }
                else if (value != null)
                {
                    newValue = CreateClone(value);
                }

                if (newValue != null)
                {
                    property.SetValue(target, newValue, null);
                }
            }
            Merge(source, target, type.BaseType);
        }

        private static Element MergeElements(Element source, Element target)
        {
            // If the target is null there's nothing to merge with, so create a copy.
            // Also create a copy if the source is an ICustomElement and overwrite
            // the value in target.
            if (target == null || (source is ICustomElement))
            {
                return Clone(source);
            }

            // Else merge and return the updated target.
            Merge(target, source);
            return target;
        }
    }
}
