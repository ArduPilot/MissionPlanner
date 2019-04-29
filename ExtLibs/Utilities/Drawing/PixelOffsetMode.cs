namespace MissionPlanner.Utilities.Drawing
{
    public enum PixelOffsetMode
    {
        /// <summary>Specifies an invalid mode.</summary>
        Invalid = -1,
        /// <summary>Specifies the default mode.</summary>
        Default,
        /// <summary>Specifies high speed, low quality rendering.</summary>
        HighSpeed,
        /// <summary>Specifies high quality, low speed rendering.</summary>
        HighQuality,
        /// <summary>Specifies no pixel offset.</summary>
        None,
        /// <summary>Specifies that pixels are offset by -.5 units, both horizontally and vertically, for high speed antialiasing.</summary>
        Half
    }
}