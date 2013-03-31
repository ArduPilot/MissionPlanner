using System.Collections.Generic;
using SharpKml.Base;

namespace SharpKml.Dom
{
    /// <summary>
    /// Used to position a photograph relative to the camera viewpoint and also
    /// to define field-of-view parameters.
    /// </summary>
    /// <remarks>
    /// <para>OGC KML 2.2 Section 11.4</para>
    /// <para>The PhotoOverlay may be rendered on one of three shapes
    /// (as determined by <see cref="Shape"/> hape):
    /// <list type="bullet">
    /// <item><description>A 2D planar rectangle.</description></item>
    /// <item><description>A cylinder (for a panoramic photo).</description></item>
    /// <item><description>A sphere (for a spherical panorama).</description></item>
    /// </list></para>
    /// <para>The photo overlay faces toward the viewpoint and its center is
    /// placed at the head of the view vector. The view vector is defined as the
    /// vector based at the viewpoint and in the direction specified by the
    /// corresponding <see cref="AbstractView"/> element. The length of the view
    /// vector is determined by the value of <see cref="ViewVolume.Near"/>. The
    /// photo overlay is positioned such that the view vector points toward the
    /// photo and is orthogonal to the center of the image (see Section 11.4.2).
    /// </para><para>The URL for the PhotoOverlay image is specified in
    /// <see cref="Overlay.Icon"/> in the <see cref="BasicLink.Href"/>
    /// property.</para>
    /// </remarks>
    [KmlElement("PhotoOverlay")]
    public sealed class PhotoOverlay : Overlay, IBoundsInformation
    {
        private Point _location;
        private ImagePyramid _pyramid;
        private ViewVolume _view;

        /// <summary>
        /// Gets or sets the associated <see cref="ImagePyramid"/> of this instance.
        /// </summary>
        [KmlElement(null, 3)]
        public ImagePyramid Image
        {
            get { return _pyramid; }
            set { this.UpdatePropertyChild(value, ref _pyramid); }
        }

        /// <summary>Gets or sets the location of an associated icon.</summary>
        /// <remarks>
        /// The <see cref="Point"/> is styled using associated or default styles.
        /// </remarks>
        [KmlElement(null, 4)]
        public Point Location
        {
            get { return _location; }
            set { this.UpdatePropertyChild(value, ref _location); }
        }

        /// <summary>
        /// Gets or sets a rotation of the overlay about its center, in decimal
        /// degrees.
        /// </summary>
        /// <remarks>
        /// Values can be ±180, with 0 being North. Rotations are specified in
        /// a counterclockwise direction.
        /// </remarks>
        [KmlElement("rotation", 1)]
        public double? Rotation { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Dom.Shape"/> the PhotoOverlay is
        /// projected onto.
        /// </summary>
        [KmlElement("shape", 5)]
        public Shape Shape { get; set; }

        /// <summary>
        /// Gets or sets how much of the current scene is visible.
        /// </summary>
        [KmlElement(null, 2)]
        public ViewVolume View
        {
            get { return _view; }
            set { this.UpdatePropertyChild(value, ref _view); }
        }

        /// <summary>
        /// Gets the coordinates of the bounds of this instance.
        /// </summary>
        IEnumerable<Vector> IBoundsInformation.Coordinates
        {
            get
            {
                if ((this.Location != null) && (this.Location.Coordinate != null))
                {
                    yield return this.Location.Coordinate;
                }
            }
        }
    }
}
