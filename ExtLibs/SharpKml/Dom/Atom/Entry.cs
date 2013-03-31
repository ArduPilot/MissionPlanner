using System.Collections.Generic;
using System.Linq;
using SharpKml.Base;

namespace SharpKml.Dom.Atom
{
    /// <summary>
    /// Acts as a container for metadata and data associated with an Atom entry.
    /// </summary>
    /// <remarks>
    /// RFC 4287 Section 4.1.2 (see http://atompub.org/rfc4287.html)
    /// This is not part of the OGC KML 2.2 standard.
    /// </remarks>
    [KmlElement("entry", KmlNamespaces.AtomNamespace)]
    public sealed class Entry : Element
    {
        private Content _content;

        /// <summary>Initializes a new instance of the Entry class.</summary>
        public Entry()
        {
            this.RegisterValidChild<Category>();
            this.RegisterValidChild<Link>();
        }

        /// <summary>Gets the categories associated with this instance.</summary>
        public IEnumerable<Category> Categories
        {
            get { return this.Children.OfType<Category>(); }
        }

        /// <summary>Gets or sets the content of the Entry.</summary>
        [KmlElement(null)]
        public Content Content
        {
            get { return _content; }
            set { this.UpdatePropertyChild(value, ref _content); }
        }

        /// <summary>
        /// Gets or sets a permanent, universally unique identifier for this instance.
        /// </summary>
        [KmlElement("id", KmlNamespaces.AtomNamespace)]
        public string Id { get; set; }

        /// <summary>Gets the links associated with this instance.</summary>
        public IEnumerable<Link> Links
        {
            get { return this.Children.OfType<Link>(); }
        }

        /// <summary>
        /// Gets or sets a short summary, abstract, or excerpt of an entry.
        /// </summary>
        /// <remarks>
        /// It is not advisable for the Summary to duplicate <see cref="Title"/>
        /// or <see cref="Content"/> because Atom Processors might assume there
        /// is a useful summary when there is none.
        /// </remarks>
        [KmlElement("summary", KmlNamespaces.AtomNamespace)]
        public string Summary { get; set; }

        /// <summary>Gets or sets a human-readable title for this instance.</summary>
        [KmlElement("title", KmlNamespaces.AtomNamespace)]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets a Date construct indicating the most recent instant in time
        /// when this instance was modified in a way the publisher considers significant.
        /// </summary>
        /// <remarks>
        /// Not all modifications necessarily result in a changed Updated value.
        /// </remarks>
        [KmlElement("updated", KmlNamespaces.AtomNamespace)]
        public string Updated { get; set; }

        /// <summary>
        /// Adds the specified <see cref="Category"/> to this instance.
        /// </summary>
        /// <param name="category">The <c>Category</c> to add to this instance.</param>
        /// <exception cref="System.ArgumentNullException">category is null.</exception>
        /// <exception cref="System.InvalidOperationException">
        /// category belongs to another <see cref="Element"/>.
        /// </exception>
        public void AddCategory(Category category)
        {
            this.AddChild(category);
        }

        /// <summary>
        /// Adds the specified <see cref="Link"/> to this instance.
        /// </summary>
        /// <param name="link">The <c>Link</c> to add to this instance.</param>
        /// <exception cref="System.ArgumentNullException">link is null.</exception>
        /// <exception cref="System.InvalidOperationException">
        /// link belongs to another <see cref="Element"/>.
        /// </exception>
        public void AddLink(Link link)
        {
            this.AddChild(link);
        }
    }
}
