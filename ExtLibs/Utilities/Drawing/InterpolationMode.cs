namespace MissionPlanner.Utilities.Drawing
{
    public enum InterpolationMode
    {
        /// <summary>Equivalent to the <see cref="F:System.Drawing.Drawing2D.QualityMode.Invalid" /> element of the <see cref="T:System.Drawing.Drawing2D.QualityMode" /> enumeration.</summary>
        Invalid = -1,
        /// <summary>Specifies default mode.</summary>
        Default,
        /// <summary>Specifies low quality interpolation.</summary>
        Low,
        /// <summary>Specifies high quality interpolation.</summary>
        High,
        /// <summary>Specifies bilinear interpolation. No prefiltering is done. This mode is not suitable for shrinking an image below 50 percent of its original size. </summary>
        Bilinear,
        /// <summary>Specifies bicubic interpolation. No prefiltering is done. This mode is not suitable for shrinking an image below 25 percent of its original size.</summary>
        Bicubic,
        /// <summary>Specifies nearest-neighbor interpolation.</summary>
        NearestNeighbor,
        /// <summary>Specifies high-quality, bilinear interpolation. Prefiltering is performed to ensure high-quality shrinking. </summary>
        HighQualityBilinear,
        /// <summary>Specifies high-quality, bicubic interpolation. Prefiltering is performed to ensure high-quality shrinking. This mode produces the highest quality transformed images.</summary>
        HighQualityBicubic
    }
}