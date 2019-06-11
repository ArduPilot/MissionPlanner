namespace MissionPlanner.Utilities.Drawing
{
    public enum ContentAlignment
    {
        /// <summary>Content is vertically aligned at the top, and horizontally aligned on the left.</summary>
        TopLeft = 1,
        /// <summary>Content is vertically aligned at the top, and horizontally aligned at the center.</summary>
        TopCenter = 2,
        /// <summary>Content is vertically aligned at the top, and horizontally aligned on the right.</summary>
        TopRight = 4,
        /// <summary>Content is vertically aligned in the middle, and horizontally aligned on the left.</summary>
        MiddleLeft = 0x10,
        /// <summary>Content is vertically aligned in the middle, and horizontally aligned at the center.</summary>
        MiddleCenter = 0x20,
        /// <summary>Content is vertically aligned in the middle, and horizontally aligned on the right.</summary>
        MiddleRight = 0x40,
        /// <summary>Content is vertically aligned at the bottom, and horizontally aligned on the left.</summary>
        BottomLeft = 0x100,
        /// <summary>Content is vertically aligned at the bottom, and horizontally aligned at the center.</summary>
        BottomCenter = 0x200,
        /// <summary>Content is vertically aligned at the bottom, and horizontally aligned on the right.</summary>
        BottomRight = 0x400
    }
}