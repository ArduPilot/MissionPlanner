namespace MissionPlanner.Utilities.Drawing
{
    public enum ImageLockMode
    {
        /// <summary>Specifies that a portion of the image is locked for reading.</summary>
        ReadOnly = 1,
        /// <summary>Specifies that a portion of the image is locked for writing.</summary>
        WriteOnly,
        /// <summary>Specifies that a portion of the image is locked for reading or writing.</summary>
        ReadWrite,
        /// <summary>Specifies that the buffer used for reading or writing pixel data is allocated by the user. If this flag is set, the <paramref name="flags" /> parameter of the <see cref="Bitmap.LockBits" /> method serves as an input parameter (and possibly as an output parameter). If this flag is cleared, then the <paramref name="flags" /> parameter serves only as an output parameter.</summary>
        UserInputBuffer
    }
}