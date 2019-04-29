namespace MissionPlanner.Utilities.Drawing
{
    public enum CopyPixelOperation
    {
        /// <summary>The destination area is filled by using the color associated with index 0 in the physical palette. (This color is black for the default physical palette.)</summary>
        Blackness = 66,
        /// <summary>Windows that are layered on top of your window are included in the resulting image. By default, the image contains only your window. Note that this generally cannot be used for printing device contexts.</summary>
        CaptureBlt = 0x40000000,
        /// <summary>The destination area is inverted.</summary>
        DestinationInvert = 5570569,
        /// <summary>The colors of the source area are merged with the colors of the selected brush of the destination device context using the Boolean <see langword="AND" /> operator.</summary>
        MergeCopy = 12583114,
        /// <summary>The colors of the inverted source area are merged with the colors of the destination area by using the Boolean <see langword="OR" /> operator.</summary>
        MergePaint = 12255782,
        /// <summary>The bitmap is not mirrored.</summary>
        NoMirrorBitmap = int.MinValue,
        /// <summary>The inverted source area is copied to the destination.</summary>
        NotSourceCopy = 3342344,
        /// <summary>The source and destination colors are combined using the Boolean <see langword="OR" /> operator, and then resultant color is then inverted.</summary>
        NotSourceErase = 1114278,
        /// <summary>The brush currently selected in the destination device context is copied to the destination bitmap.</summary>
        PatCopy = 15728673,
        /// <summary>The colors of the brush currently selected in the destination device context are combined with the colors of the destination are using the Boolean <see langword="XOR" /> operator.</summary>
        PatInvert = 5898313,
        /// <summary>The colors of the brush currently selected in the destination device context are combined with the colors of the inverted source area using the Boolean <see langword="OR" /> operator. The result of this operation is combined with the colors of the destination area using the Boolean <see langword="OR" /> operator.</summary>
        PatPaint = 16452105,
        /// <summary>The colors of the source and destination areas are combined using the Boolean <see langword="AND" /> operator.</summary>
        SourceAnd = 8913094,
        /// <summary>The source area is copied directly to the destination area.</summary>
        SourceCopy = 13369376,
        /// <summary>The inverted colors of the destination area are combined with the colors of the source area using the Boolean <see langword="AND" /> operator.</summary>
        SourceErase = 4457256,
        /// <summary>The colors of the source and destination areas are combined using the Boolean <see langword="XOR" /> operator.</summary>
        SourceInvert = 6684742,
        /// <summary>The colors of the source and destination areas are combined using the Boolean <see langword="OR" /> operator.</summary>
        SourcePaint = 15597702,
        /// <summary>The destination area is filled by using the color associated with index 1 in the physical palette. (This color is white for the default physical palette.)</summary>
        Whiteness = 16711778
    }
}