namespace MissionPlanner.Utilities.Drawing
{
    public enum SmoothingMode
    {
        /// <summary>Specifies an invalid mode.</summary>
        Invalid = -1,
        /// <summary>Specifies no antialiasing.</summary>
        Default,
        /// <summary>Specifies no antialiasing.</summary>
        HighSpeed,
        /// <summary>Specifies antialiased rendering.</summary>
        HighQuality,
        /// <summary>Specifies no antialiasing.</summary>
        None,
        /// <summary>Specifies antialiased rendering.</summary>
        AntiAlias
    }
}