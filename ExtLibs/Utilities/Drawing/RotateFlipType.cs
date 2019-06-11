namespace MissionPlanner.Utilities.Drawing
{
    public enum RotateFlipType
    {
        /// <summary>Specifies no clockwise rotation and no flipping.</summary>
        RotateNoneFlipNone = 0,
        /// <summary>Specifies a 90-degree clockwise rotation without flipping.</summary>
        Rotate90FlipNone = 1,
        /// <summary>Specifies a 180-degree clockwise rotation without flipping.</summary>
        Rotate180FlipNone = 2,
        /// <summary>Specifies a 270-degree clockwise rotation without flipping.</summary>
        Rotate270FlipNone = 3,
        /// <summary>Specifies no clockwise rotation followed by a horizontal flip.</summary>
        RotateNoneFlipX = 4,
        /// <summary>Specifies a 90-degree clockwise rotation followed by a horizontal flip.</summary>
        Rotate90FlipX = 5,
        /// <summary>Specifies a 180-degree clockwise rotation followed by a horizontal flip.</summary>
        Rotate180FlipX = 6,
        /// <summary>Specifies a 270-degree clockwise rotation followed by a horizontal flip.</summary>
        Rotate270FlipX = 7,
        /// <summary>Specifies no clockwise rotation followed by a vertical flip.</summary>
        RotateNoneFlipY = 6,
        /// <summary>Specifies a 90-degree clockwise rotation followed by a vertical flip.</summary>
        Rotate90FlipY = 7,
        /// <summary>Specifies a 180-degree clockwise rotation followed by a vertical flip.</summary>
        Rotate180FlipY = 4,
        /// <summary>Specifies a 270-degree clockwise rotation followed by a vertical flip.</summary>
        Rotate270FlipY = 5,
        /// <summary>Specifies no clockwise rotation followed by a horizontal and vertical flip.</summary>
        RotateNoneFlipXY = 2,
        /// <summary>Specifies a 90-degree clockwise rotation followed by a horizontal and vertical flip.</summary>
        Rotate90FlipXY = 3,
        /// <summary>Specifies a 180-degree clockwise rotation followed by a horizontal and vertical flip.</summary>
        Rotate180FlipXY = 0,
        /// <summary>Specifies a 270-degree clockwise rotation followed by a horizontal and vertical flip.</summary>
        Rotate270FlipXY = 1
    }
}