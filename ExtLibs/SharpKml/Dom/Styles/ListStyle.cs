using System.Collections.Generic;
using System.Linq;
using SharpKml.Base;

namespace SharpKml.Dom
{
    /// <summary>
    /// Specifies how a <see cref="Feature"/> is displayed in the list view.
    /// </summary>
    /// <remarks>OGC KML 2.2 Section 12.13</remarks>
    [KmlElement("ListStyle")]
    public sealed class ListStyle : SubStyle
    {
        /// <summary>The default value that should be used for <see cref="BackgroundColor"/>.</summary>
        public static readonly Color32 DefaultBackgroundColor = new Color32(255, 255, 255, 255);

        /// <summary>The default value that should be used for <see cref="MaximumSnippetLines"/>.</summary>
        public const int DefaultMaximumSnippetLines = 2;

        /// <summary>Initializes a new instance of the ListStyle class.</summary>
        public ListStyle()
        {
            this.RegisterValidChild<ItemIcon>();
        }

        /// <summary>
        /// Gets or sets the background color of the graphic element.
        /// </summary>
        [KmlElement("bgColor", 2)]
        public Color32? BackgroundColor { get; set; }

        /// <summary>
        /// Gets the <see cref="ItemIcon"/>s contained by this instance.
        /// </summary>
        public IEnumerable<ItemIcon> ItemIcons
        {
            get { return this.Children.OfType<ItemIcon>(); }
        }

        /// <summary>
        /// Gets or sets how a <see cref="Folder"/> and its contents shall be
        /// displayed as items in the list view.
        /// </summary>
        [KmlElement("listItemType", 1)]
        public ListItemType? ItemType { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of lines to display for the
        /// <see cref="Feature.Snippet"/> value in the list view.
        /// </summary>
        [KmlElement("maxSnippetLines", 4)]
        public int? MaximumSnippetLines { get; set; }

        /// <summary>
        /// Adds the specified <see cref="ItemIcon"/> to this instance.
        /// </summary>
        /// <param name="icon">The <c>ItemIcon</c> to add to this instance.</param>
        /// <exception cref="System.ArgumentNullException">icon is null.</exception>
        /// <exception cref="System.InvalidOperationException">
        /// icon belongs to another <see cref="Element"/>.
        /// </exception>
        public void AddItemIcon(ItemIcon icon)
        {
            this.AddChild(icon);
        }
    }
}
