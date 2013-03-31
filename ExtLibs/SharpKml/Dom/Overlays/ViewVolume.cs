using SharpKml.Base;

namespace SharpKml.Dom
{
    /// <summary>
    /// Defines how much of the current scene in a <see cref="PhotoOverlay"/>
    /// is visible.
    /// </summary>
    /// <remarks>
    /// <para>OGC KML 2.2 Section 11.5</para>
    /// <para>Specifying the field of view is analogous to specifying the lens
    /// opening in a physical camera. A small field of view, like a telephoto
    /// lens, focuses on a small part of the scene. A large field of view, like
    /// a wide-angle lens, focuses on a large part of the scene.</para>
    /// </remarks>
    [KmlElement("ViewVolume")]
    public sealed class ViewVolume : KmlObject
    {
        /// <summary>
        /// Gets or sets the angle, in decimal degrees, from the bottom side
        /// of the view volume to camera's view vector.
        /// </summary>
        [KmlElement("bottomFov", 3)]
        public double? Bottom { get; set; }

        /// <summary>
        /// Gets or sets the angle, in decimal degrees, from the left side of
        /// the view volume to the camera's view vector.
        /// </summary>
        /// <remarks>
        /// A negative value of the angle corresponds to a field of view that
        /// is 'left' of the view vector.
        /// </remarks>
        [KmlElement("leftFov", 1)]
        public double? Left { get; set; }

        /// <summary>
        /// Gets or sets the length in meters of the view vector, which starts
        /// from the camera viewpoint and ends at the <see cref="PhotoOverlay"/>
        /// shape.
        /// </summary>
        /// <remarks>The value shall be positive.</remarks>
        [KmlElement("near", 5)]
        public double? Near { get; set; }

        /// <summary>
        /// Gets or sets the angle, in decimal degrees, from the camera's view
        /// vector to the right side of the view volume.
        /// </summary>
        /// <remarks>
        /// A positive value of the angle corresponds to a field of view that
        /// is 'right' of the view vector.
        /// </remarks>
        [KmlElement("rightFov", 2)]
        public double? Right { get; set; }

        /// <summary>
        /// Gets or sets the angle, in decimal degrees, from the camera's view
        /// vector to the top side of the view volume.
        /// </summary>
        [KmlElement("topFov", 4)]
        public double? Top { get; set; }
    }
}
