using System;
using SharpKml.Dom;

namespace SharpKml.Base
{
    /// <summary>
    /// Provides data for the <see cref="Parser.ElementAdded"/> event.
    /// </summary>
    public sealed class ElementEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the ElementEventArgs class.
        /// </summary>
        /// <param name="element">
        /// The value to assign to the Element property.
        /// </param>
        internal ElementEventArgs(Element element)
        {
            this.Element = element;
        }

        /// <summary>
        /// Gets the <see cref="Dom.Element"/> found during parsing.
        /// </summary>
        public Element Element { get; private set; }
    }
}
