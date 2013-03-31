using SharpKml.Base;

namespace SharpKml.Dom
{
    /// <summary>
    /// Specifies a hierarchical set of images, each of which is an increasingly
    /// lower resolution (towards the top of the pyramid).
    /// </summary>
    /// <remarks>
    /// <para>OGC KML 2.2 Section 11.6</para>
    /// <para>Each image in the pyramid is subdivided into tiles so only the
    /// portions in view are loaded.</para>
    /// <para>The pixel size of the original image is specified by
    /// <see cref="Width"/> and <see cref="Height"/>. The width and height
    /// can be any size and do not need to be a power of 2. You can fill out the
    /// remaining pixels with blank pixels (as described in Section 11.6.3)</para>
    /// <para>Tiles must be square, and the <see cref="TileSize"/> must be a
    /// power of 2. A tile size of 256 (the default) or 512 is recommended.</para>
    /// <para>See Section 11.4.3 for handling large images and Section 11.6.3
    /// for creating an Image Pyramid.</para>
    /// </remarks>
    [KmlElement("ImagePyramid")]
    public sealed class ImagePyramid : KmlObject
    {
        /// <summary>The default value that should be used for <see cref="TileSize"/>.</summary>
        public const int DefaultTileSize = 256;

        /// <summary>
        /// Gets or sets where to begin numbering the tiles in each layer of
        /// the pyramid.
        /// </summary>
        [KmlElement("gridOrigin", 4)]
        public GridOrigin? GridOrigin { get; set; }

        /// <summary>
        /// Gets or sets the height, in pixels, of the original image.
        /// </summary>
        [KmlElement("maxHeight", 3)]
        public int? Height { get; set; }

        /// <summary>Gets or sets the size of the tiles, in pixels.</summary>
        /// <remarks>
        /// Tiles must be square, and TileSize must be a power of 2. A tile size
        /// of 256 (the default) or 512 is recommended. The original image is
        /// divided into tiles of this size, at varying resolutions.
        /// </remarks>
        [KmlElement("tileSize", 1)]
        public int? TileSize { get; set; }

        /// <summary>
        /// Gets or sets the width, in pixels, of the original image.
        /// </summary>
        [KmlElement("maxWidth", 2)]
        public int? Width { get; set; }
    }
}
