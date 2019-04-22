namespace MissionPlanner.Utilities.Drawing
{
    public enum TextRenderingHint
    {
        /// <summary>Each character is drawn using its glyph bitmap, with the system default rendering hint. The text will be drawn using whatever font-smoothing settings the user has selected for the system.</summary>
        SystemDefault,
        /// <summary>Each character is drawn using its glyph bitmap. Hinting is used to improve character appearance on stems and curvature.</summary>
        SingleBitPerPixelGridFit,
        /// <summary>Each character is drawn using its glyph bitmap. Hinting is not used.</summary>
        SingleBitPerPixel,
        /// <summary>Each character is drawn using its antialiased glyph bitmap with hinting. Much better quality due to antialiasing, but at a higher performance cost.</summary>
        AntiAliasGridFit,
        /// <summary>Each character is drawn using its antialiased glyph bitmap without hinting. Better quality due to antialiasing. Stem width differences may be noticeable because hinting is turned off.</summary>
        AntiAlias,
        /// <summary>Each character is drawn using its glyph ClearType bitmap with hinting. The highest quality setting. Used to take advantage of ClearType font features.</summary>
        ClearTypeGridFit
    }
}