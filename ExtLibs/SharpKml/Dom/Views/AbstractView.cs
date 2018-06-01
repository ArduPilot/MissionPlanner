using SharpKml.Base;

namespace SharpKml.Dom
{
    /// <summary>Represents an AbstractViewGroup.</summary>
    /// <remarks>OGC KML 2.2 Section 14.1</remarks>
    public abstract class AbstractView : KmlObject
    {
        private TimePrimitive _primitive;

        /// <summary>Gets or sets the associated time primitive.</summary>
        /// <remarks>
        /// The time primitive must be in the Google Extension namespace.
        /// </remarks>
        [KmlElement(null, 1)]
        public TimePrimitive GXTimePrimitive
        {
            get { return _primitive; }
            set { this.UpdatePropertyChild(value, ref _primitive); }
        }
    }
}
