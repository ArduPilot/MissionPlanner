namespace tlogThumbnailHandler
{
    /// <summary>
    /// Defines the format of a bitmap returned by an <see cref="IThumbnailProvider"/>.
    /// </summary>
    public enum WTS_ALPHATYPE: int
    {
        /// <summary>
        /// The bitmap is an unknown format. The Shell tries nonetheless to detect whether the image has an alpha channel.
        /// </summary>
        WTSAT_UNKNOWN = 0,
        /// <summary>
        /// The bitmap is an RGB image without alpha. The alpha channel is invalid and the Shell ignores it.
        /// </summary>
        WTSAT_RGB = 1,
        /// <summary>
        /// The bitmap is an ARGB image with a valid alpha channel.
        /// </summary>
        WTSAT_ARGB = 2,
    }
}