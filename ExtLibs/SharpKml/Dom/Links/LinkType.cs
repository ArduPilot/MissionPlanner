using System.ComponentModel;
using SharpKml.Base;

namespace SharpKml.Dom
{
    /// <summary>
    /// Provides common properties for <see cref="Icon"/> and <see cref="Link"/>.
    /// </summary>
    /// <remarks>
    /// This is not part of the KML specification but is declared in the XSD.
    /// Because it is not part of the specification, this class should not be
    /// used; an instance of <see cref="Icon"/> or <see cref="Link"/> should
    /// be used instead.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class LinkType : BasicLink
    {
        /// <summary>The default value that should be used for <see cref="RefreshInterval"/>.</summary>
        public const double DefaultRefreshInterval = 4.0;

        /// <summary>The default value that should be used for <see cref="ViewBoundScale"/>.</summary>
        public const double DefaultViewBoundScale = 1.0;

        /// <summary>The default value that should be used for <see cref="ViewRefreshTime"/>.</summary>
        public const double DefaultViewRefreshTime = 4.0;

        /// <summary>Initializes a new instance of the LinkType class.</summary>
        internal LinkType()
        {
        }

        /// <summary>
        /// Gets or sets a value used to specify any additional query parameters
        /// not related to the geographic view.
        /// </summary>
        /// <remarks>
        /// The following query parameters may be used:
        /// <list type="bullet">
        /// <item>
        /// <term>[clientVersion]</term>
        /// <description>
        /// Version of earth browser client.
        /// </description>
        /// </item><item>
        /// <term>[kmlVersion]</term>
        /// <description>
        /// Version of requested kml.
        /// </description>
        /// </item><item>
        /// <term>[clientName]</term>
        /// <description>
        /// Name of earth browser client.
        /// </description>
        /// </item><item>
        /// <term>[language]</term>
        /// <description>
        /// Language preference of the earth browser client.
        /// </description>
        /// </item></list>
        /// </remarks>
        [KmlElement("httpQuery", 7)]
        public string HttpQuery { get; set; }

        /// <summary>
        /// Gets or sets the interval, in seconds, to wait before refreshing the resource.
        /// </summary>
        /// <remarks>The value shall be positive.</remarks>
        [KmlElement("refreshInterval", 2)]
        public double? RefreshInterval { get; set; }

        /// <summary>Gets or sets a time-based refresh mode.</summary>
        /// <remarks>
        /// If <see cref="RefreshInterval"/> is specified then RefreshMode should
        /// be set to <see cref="Dom.RefreshMode.OnInterval"/>.
        /// </remarks>
        [KmlElement("refreshMode", 1)]
        public RefreshMode? RefreshMode { get; set; }

        /// <summary>
        /// Gets or sets the value to scale all bounding box parameters.
        /// </summary>
        /// <remarks>
        /// A value less than 1 specifies to use a geographic area less than the
        /// current geographic view. A value greater than 1 specifies to use a
        /// geographic area greater than the current geographic view. The value
        /// shall be positive.
        /// </remarks>
        [KmlElement("viewBoundScale", 5)]
        public double? ViewBoundScale { get; set; }

        /// <summary>
        /// Gets or sets the format of a query string related to view parameters
        /// that is appended to the <see cref="BasicLink.Href"/> before the
        /// resource is fetched.
        /// </summary>
        /// <remarks>
        /// The following query parameters may be used:
        /// <list type="bullet">
        /// <item>
        /// <term>[lookatLon], [lookatLat]</term>
        /// <description>
        /// Longitude and latitude of the point that LookAt is viewing.
        /// </description>
        /// </item><item>
        /// <term>[lookatRange], [lookatTilt], [lookatHeading]</term>
        /// <description>
        /// Values used by the LookAt element (the Range, Tilt, and Heading properties).
        /// </description>
        /// </item><item>
        /// <term>[lookatTerrainLon], [lookatTerrainLat], [lookatTerrainAlt]</term>
        /// <description>
        /// Point on the terrain in decimal degrees/meters that LookAt is viewing.
        /// </description>
        /// </item><item>
        /// <term>[cameraLon], [cameraLat], [cameraAlt]</term>
        /// <description>
        /// Decimal degrees/meters of the eye point for the camera.
        /// </description>
        /// </item><item>
        /// <term>[horizFov], [vertFov]</term>
        /// <description>
        /// Horizontal, vertical field of view for the camera.
        /// </description>
        /// </item><item>
        /// <term>[horizPixels], [vertPixels]</term>
        /// <description>
        /// Size in pixels of the geographic view.
        /// </description>
        /// </item><item>
        /// <term>[terrainEnabled]</term>
        /// <description>
        /// Indicates whether the geographic view is showing terrain.
        /// </description>
        /// </item><item>
        /// <term>[bboxWest], [bboxSouth], [bboxEast], [bboxNorth]</term>
        /// <description>
        /// Bounding box limits matching the OGC Web Map Service (WMS) bounding box specification.
        /// </description>
        /// </item></list>
        /// </remarks>
        [KmlElement("viewFormat", 6)]
        public string ViewFormat { get; set; }

        /// <summary>
        /// Gets or sets how the link is refreshed when the geographic view changes.
        /// </summary>
        /// <remarks>
        /// If <see cref="ViewRefreshTime"/> is specified then ViewRefreshMode
        /// should be set to <see cref="Dom.ViewRefreshMode.OnStop"/>.
        /// </remarks>
        [KmlElement("viewRefreshMode", 3)]
        public ViewRefreshMode? ViewRefreshMode { get; set; }

        /// <summary>
        /// Gets or sets the number of seconds to wait before refreshing the
        /// geographic view after camera movement stops.
        /// </summary>
        /// <remarks>
        /// This applies when <see cref="ViewRefreshMode"/> is set to
        /// <see cref="Dom.ViewRefreshMode.OnStop"/>. The value shall be positive.
        /// </remarks>
        [KmlElement("viewRefreshTime", 4)]
        public double? ViewRefreshTime { get; set; }
    }
}
