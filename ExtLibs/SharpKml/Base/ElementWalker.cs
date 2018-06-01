using System;
using System.Collections.Generic;
using SharpKml.Dom;

namespace SharpKml.Base
{
    /// <summary>Navigates all the children of an element.</summary>
    internal static class ElementWalker
    {
        /// <summary>
        /// Navigates the specified <see cref="Element"/> and it's children.
        /// </summary>
        /// <param name="root">The <c>Element</c> to navigate.</param>
        /// <exception cref="ArgumentNullException">root is null.</exception>
        /// <returns>An IEnumerable collection of the Elements.</returns>
        public static IEnumerable<Element> Walk(Element root)
        {
            if (root == null)
            {
                throw new ArgumentNullException("root");
            }

            return WalkElement(root);
        }

        // Because the Element's won't be nested too deep (max 100), a Stack based
        // iterative approach is not necessary so recursion is used for clarity.
        private static IEnumerable<Element> WalkElement(Element element)
        {
            ICustomElement customElement = element as ICustomElement;
            if ((customElement == null) || customElement.ProcessChildren)
            {
                yield return element; // Is a valid Element

                // Explore the children
                foreach (var child in element.Children)
                {
                    foreach (var e in WalkElement(child))
                    {
                        yield return e;
                    }
                }

                // Explore the properties
                TypeBrowser browser = TypeBrowser.Create(element.GetType());
                foreach (var property in browser.Elements)
                {
                    // All properties with their ElementName set to null will be Elements
                    // Check here to avoid the GetValue the property is not an Element.
                    if (property.Item2.ElementName == null)
                    {
                        object value = property.Item1.GetValue(element, null);
                        if (value != null)
                        {
                            foreach (var e in WalkElement((Element)value))
                            {
                                yield return e;
                            }
                        }
                    }
                }
            }
        }
    }
}
