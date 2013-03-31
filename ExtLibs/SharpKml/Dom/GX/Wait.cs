using SharpKml.Base;

namespace SharpKml.Dom.GX
{
    /// <summary>
    /// The camera remains still for the specified <see cref="Duration"/> before
    /// playing the next <see cref="TourPrimitive"/>.
    /// </summary>
    /// <remarks>This is not part of the OGC KML 2.2 standard.</remarks>
    [KmlElement("Wait", KmlNamespaces.GX22Namespace)]
    public sealed class Wait : TourPrimitive
    {
        /// <summary>Gets or sets the amount of time, in seconds.</summary>
        [KmlElement("duration", KmlNamespaces.GX22Namespace)]
        public double? Duration { get; set; }
    }
}
