using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using SharpKml.Base;

namespace SharpKml.Dom.GX
{
    /// <summary>Represents an array of values.</summary>
    /// <remarks>This is not part of the OGC KML 2.2 standard.</remarks>
    [KmlElement("SimpleArrayData", KmlNamespaces.GX22Namespace)]
    public sealed class SimpleArrayData : Element
    {
        private static readonly XmlComponent ValueComponent = new XmlComponent(null, "value", KmlNamespaces.GX22Namespace);

        /// <summary>Initializes a new instance of the SimpleArrayData class.</summary>
        public SimpleArrayData()
        {
            this.RegisterValidChild<ValueElement>();
        }

        /// <summary>Gets or sets the name of the array.</summary>
        [KmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets the collection of values stored by this instance.
        /// </summary>
        public IEnumerable<string> Values
        {
            get
            {
                return from e in this.Children.OfType<ValueElement>()
                       select e.InnerText;
            }
        }

        /// <summary>
        /// Adds the specified value to <see cref="Values"/>.</summary>
        /// <param name="value">The value to add.</param>
        /// <exception cref="ArgumentNullException">value is null.</exception>
        public void AddValue(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            this.AddChild(new ValueElement(value));
        }

        /// <summary>Adds the gx:value to <see cref="Values"/>.</summary>
        /// <param name="orphan">The <see cref="Element"/> to add.</param>
        protected internal override void AddOrphan(Element orphan)
        {
            UnknownElement unknown = orphan as UnknownElement;
            if (unknown != null)
            {
                if (ValueComponent.Equals(unknown.UnknownData))
                {
                    this.AddValue(unknown.InnerText);
                    return;
                }
            }
            base.AddOrphan(orphan);
        }

        /// <summary>Used to correctly serialize the strings in Values.</summary>
        private class ValueElement : Element, ICustomElement
        {
            private readonly string _value;

            /// <summary>
            /// Initializes a new instance of the ValueElement class.
            /// </summary>
            /// <param name="value">
            /// The value to set the <see cref="Element.InnerText"/> to.
            /// </param>
            public ValueElement(string value)
            {
                _value = value;
            }

            /// <summary>
            /// Gets a value indicating whether to process the children of the Element.
            /// </summary>
            public bool ProcessChildren
            {
                get { return false; }
            }

            /// <summary>Writes the start of an XML element.</summary>
            /// <param name="writer">An <see cref="XmlWriter"/> to write to.</param>
            public void CreateStartElement(XmlWriter writer)
            {
                writer.WriteElementString(KmlNamespaces.GX22Prefix, "value", KmlNamespaces.Kml22Namespace, _value);
            }
        }
    }
}
