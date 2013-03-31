using SharpKml.Base;

namespace SharpKml.Dom.GX
{
    /// <summary>
    /// Can be used instead of the OGC KML standard <see cref="SharpKml.Dom.AltitudeMode"/>.
    /// </summary>
    public enum AltitudeMode
    {
        /// <summary>
        /// Interprets the altitude as a value in meters above the sea floor.
        /// </summary>
        /// <remarks>
        /// If the KML feature is above land rather than sea, the altitude will
        /// be interpreted as being above the ground.
        /// </remarks>
        [KmlElement("clampToSeaFloor")]
        ClampToSeafloor = 0,

        /// <summary>
        /// The altitude specification is ignored, and the KML feature will be
        /// positioned on the sea floor.
        /// </summary>
        /// <remarks>
        /// If the KML feature is on land rather than at sea, ClampToSeaFloor
        /// will instead clamp to ground.
        /// </remarks>
        [KmlElement("relativeToSeaFloor")]
        RelativeToSeafloor
    }

    /// <summary>Specifies the type of flight mode.</summary>
    public enum FlyToMode
    {
        /// <summary>
        /// FlyTos each begin and end at zero velocity.
        /// </summary>
        [KmlElement("bounce")]
        Bounce = 0,

        /// <summary>
        /// FlyTos allow for an unbroken flight from point to point to point (and on).
        /// </summary>
        [KmlElement("smooth")]
        Smooth
    }

    /// <summary>
    /// Allows the tour to be paused until a user takes action to continue the tour.
    /// </summary>
    public enum PlayMode
    {
        /// <summary>
        /// Waits for user action to continue the tour.
        /// </summary>
        [KmlElement("pause")]
        Pause = 0
    }
}
