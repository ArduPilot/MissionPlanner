using System.Xml;

namespace SharpKml.Dom
{
    /// <summary>
    /// Allows customized serialization of an <see cref="Element"/>.
    /// </summary>
    public interface ICustomElement
    {
        /// <summary>
        /// Gets a value indicating whether to process the children of the Element.
        /// </summary>
        bool ProcessChildren { get; }

        /// <summary>Writes the start of an XML element.</summary>
        /// <param name="writer">An <see cref="XmlWriter"/> to write to.</param>
        void CreateStartElement(XmlWriter writer);
    }
}
