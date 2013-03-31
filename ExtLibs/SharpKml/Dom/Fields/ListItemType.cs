using SharpKml.Base;

namespace SharpKml.Dom
{
    /// <summary>
    /// Specifies how a <see cref="Feature"/> and its contents shall be
    /// displayed as items in a list view.
    /// </summary>
    /// <remarks>OGC KML 2.2 Section 16.15</remarks>
    public enum ListItemType
    {
        /// <summary>
        /// The <see cref="Feature"/>'s visibility is tied to its item's checkbox.
        /// </summary>
        [KmlElement("check")]
        Check = 0,

        /// <summary>
        /// Only one of the <see cref="Container"/>'s items shall be visible
        /// at a time.
        /// </summary>
        [KmlElement("radioFolder")]
        RadioFolder,

        /// <summary>
        /// Use a normal checkbox for visibility but do not display the
        /// <see cref="Container"/>'s children in the list view.
        /// </summary>
        /// <remarks>
        /// A checkbox allows the user to toggle visibility of the child
        /// objects in the viewer.
        /// </remarks>
        [KmlElement("checkOffOnly")]
        CheckOffOnly,

        /// <summary>
        /// Prevents all items from being made visible at once.
        /// </summary>
        /// <remarks>
        /// The user can turn everything in the <see cref="Container"/> off but
        /// cannot turn everything on at the same time. This setting is useful
        /// for <c>Container</c>'s containing large amounts of data.
        /// </remarks>
        [KmlElement("checkHideChildren")]
        CheckHideChildren
    }
}
