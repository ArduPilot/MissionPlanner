using System;
using SharpKml.Base;

namespace SharpKml.Dom
{
    /// <summary>
    /// Controls the behavior of a <see cref="NetworkLink"/> that references
    /// the KML resource.
    /// </summary>
    /// <remarks>OGC KML 2.2 Section 13.2</remarks>
    [KmlElement("NetworkLinkControl")]
    public sealed class NetworkLinkControl : Element
    {
        /// <summary>The default value that should be used for <see cref="SessionLength"/>.</summary>
        public const double DefaultSessionLength = -1.0;

        private Update _update;
        private AbstractView _view;

        /// <summary>
        /// Gets or sets a string to append to the <see cref="NetworkLink"/>
        /// URL query.
        /// </summary>
        [KmlElement("cookie", 3)]
        public string Cookie { get; set; }

        /// <summary>
        /// Gets or sets a point in time at which the <see cref="NetworkLink"/>
        /// shall be refreshed.
        /// </summary>
        /// <remarks>
        /// Applies only if <see cref="NetworkLink.Link"/> has its
        /// <see cref="LinkType.RefreshMode"/> set to <see cref="RefreshMode.OnExpire"/>.
        /// </remarks>
        [KmlElement("expires", 8)]
        public DateTime? Expires { get; set; }

        /// <summary>
        /// Gets or sets the text for the <see cref="Feature.Description"/> of
        /// <see cref="NetworkLink"/>.
        /// </summary>
        /// <remarks>
        /// The text may include well formed HTML. This value shall take
        /// precedence over the value of <c>NetworkLink.Description</c>.
        /// </remarks>
        [KmlElement("linkDescription", 6)]
        public string LinkDescription { get; set; }

        /// <summary>
        /// Gets or sets a valid content for the <see cref="Feature.Name"/> of
        /// <see cref="NetworkLink"/>.
        /// </summary>
        /// <remarks>
        /// This value shall take precedence over the value of
        /// <c>NetworkLink.Name</c>.
        /// </remarks>
        [KmlElement("linkName", 5)]
        public string LinkName { get; set; }

        /// <summary>
        /// Gets or sets the value for <see cref="Feature.Snippet"/> of
        /// <see cref="NetworkLink"/>.
        /// </summary>
        /// <remarks>
        /// This value shall take precedence over the value of
        /// <c>NetworkLink.Snippet</c>.
        /// </remarks>
        [KmlElement(null, 7)]
        public LinkSnippet LinkSnippet { get; set; }

        /// <summary>
        /// Gets or sets the text that should be displayed when a
        /// <see cref="NetworkLink"/> is first activated or the Message
        /// value is updated.
        /// </summary>
        [KmlElement("message", 4)]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the minimum allowed time, in seconds, between refreshes
        /// of the referenced KML resource.
        /// </summary>
        /// <remarks>
        /// The value shall take precedence over <see cref="LinkType.RefreshInterval"/>.
        /// </remarks>
        [KmlElement("minRefreshPeriod", 1)]
        public double? RefreshPeriod { get; set; }

        /// <summary>
        /// Gets or sets the maximum time, in seconds, that an earth browser
        /// shall remain connected to the referenced KML resource.
        /// </summary>
        /// <remarks>
        /// A value of -1 indicates not to terminate the session explicitly.
        /// </remarks>
        [KmlElement("maxSessionLength", 2)]
        public double? SessionLength { get; set; }

        /// <summary>
        /// Gets or sets the associated <see cref="Update"/> of this instance.
        /// </summary>
        [KmlElement(null, 9)]
        public Update Update
        {
            get { return _update; }
            set { this.UpdatePropertyChild(value, ref _update); }
        }

        /// <summary>
        /// Gets or sets the associated <see cref="AbstractView"/> of this instance.
        /// </summary>
        [KmlElement(null, 10)]
        public AbstractView View
        {
            get { return _view; }
            set { this.UpdatePropertyChild(value, ref _view); }
        }
    }
}
