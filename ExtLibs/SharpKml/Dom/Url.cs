using System;
using SharpKml.Base;

namespace SharpKml.Dom
{
    /// <summary>Specifies the location and handling of a resource.</summary>
    /// <remarks>Legacy API - not part of OGC KML 2.2</remarks>
    [Obsolete("Url deprecated in 2.2")]
    [KmlElement("Url")]
    public sealed class Url : LinkType
    {
        /// <summary>Converts the specified Url into a <see cref="Link"/></summary>
        /// <param name="url">The value to convert.</param>
        /// <returns>
        /// A copy of the specified value parameter as a <c>Link</c>.
        /// </returns>
        public static explicit operator Link(Url url)
        {
            Link output = new Link();
            output.Href = url.Href;
            output.HttpQuery = url.HttpQuery;
            output.Id = url.Id;
            output.RefreshInterval = url.RefreshInterval;
            output.RefreshMode = url.RefreshMode;
            output.TargetId = url.TargetId;
            output.ViewBoundScale = url.ViewBoundScale;
            output.ViewFormat = url.ViewFormat;
            output.ViewRefreshMode = url.ViewRefreshMode;
            output.ViewRefreshTime = url.ViewRefreshTime;
            return output;
        }
    }
}
