namespace MissionPlanner.Utilities.Drawing
{
    public enum WrapMode
    {
        /// <summary>Tiles the gradient or texture.</summary>
        Tile,
        /// <summary>Reverses the texture or gradient horizontally and then tiles the texture or gradient.</summary>
        TileFlipX,
        /// <summary>Reverses the texture or gradient vertically and then tiles the texture or gradient.</summary>
        TileFlipY,
        /// <summary>Reverses the texture or gradient horizontally and vertically and then tiles the texture or gradient.</summary>
        TileFlipXY,
        /// <summary>The texture or gradient is not tiled.</summary>
        Clamp
    }
}