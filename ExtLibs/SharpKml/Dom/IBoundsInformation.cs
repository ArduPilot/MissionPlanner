using System.Collections.Generic;
using SharpKml.Base;

namespace SharpKml.Dom
{
    /// <summary>
    /// This is a helper interface for the Feature/Geometry extensions of
    /// CalculateBounds.
    /// </summary>
    internal interface IBoundsInformation
    {
        /// <summary>
        /// Gets the coordinates of the bounds of this instance.
        /// </summary>
        IEnumerable<Vector> Coordinates { get; }
    }
}
