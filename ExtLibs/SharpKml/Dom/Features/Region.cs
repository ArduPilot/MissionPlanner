using SharpKml.Base;

namespace SharpKml.Dom
{
    /// <summary>Affects the visibility of a <see cref="Feature"/>.</summary>
    /// <remarks>
    /// <para>OGC KML 2.2 Section 9.13</para>
    /// <para>Regions define both culling and level-of-detail control over the
    /// display of the <see cref="Feature"/>, including elements that are defined
    /// lower in the hierarchy.</para>
    /// <para>A Region is said to be "active" when the bounding box is within the
    /// user's view and the LOD requirements are met. Feature elements associated
    /// with a Region are drawn only when the Region is active.</para>
    /// <para>When <see cref="LinkType.ViewRefreshMode"/> is
    /// <see cref="ViewRefreshMode.OnRegion"/>, the <see cref="Link"/> or
    /// <see cref="Icon"/> is loaded only when the Region is active. In a
    /// <see cref="Container"/> or <see cref="NetworkLinkControl"/> hierarchy, this
    /// calculation uses the Region that is the closest ancestor in the hierarchy.</para>
    /// </remarks>
    [KmlElement("Region")]
    public sealed class Region : KmlObject
    {
        private LatLonAltBox _box;
        private Lod _lod;

        /// <summary>
        /// Gets or sets an area of interest defined by geographic coordinates
        /// and altitudes.
        /// </summary>
        [KmlElement(null, 1)]
        public LatLonAltBox LatLonAltBox
        {
            get { return _box; }
            set { this.UpdatePropertyChild(value, ref _box); }
        }

        /// <summary>
        /// Gets or sets the validity range in terms of projected screen size.
        /// </summary>
        [KmlElement(null, 2)]
        public Lod LevelOfDetail
        {
            get { return _lod; }
            set { this.UpdatePropertyChild(value, ref _lod); }
        }
    }
}
