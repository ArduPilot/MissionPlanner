namespace MissionPlanner.Utilities.Drawing
{
    public enum HatchStyle
    {
        /// <summary>A pattern of horizontal lines.</summary>
        Horizontal = 0,
        /// <summary>A pattern of vertical lines.</summary>
        Vertical = 1,
        /// <summary>A pattern of lines on a diagonal from upper left to lower right.</summary>
        ForwardDiagonal = 2,
        /// <summary>A pattern of lines on a diagonal from upper right to lower left.</summary>
        BackwardDiagonal = 3,
        /// <summary>Specifies horizontal and vertical lines that cross.</summary>
        Cross = 4,
        /// <summary>A pattern of crisscross diagonal lines.</summary>
        DiagonalCross = 5,
        /// <summary>Specifies a 5-percent hatch. The ratio of foreground color to background color is 5:95.</summary>
        Percent05 = 6,
        /// <summary>Specifies a 10-percent hatch. The ratio of foreground color to background color is 10:90.</summary>
        Percent10 = 7,
        /// <summary>Specifies a 20-percent hatch. The ratio of foreground color to background color is 20:80.</summary>
        Percent20 = 8,
        /// <summary>Specifies a 25-percent hatch. The ratio of foreground color to background color is 25:75.</summary>
        Percent25 = 9,
        /// <summary>Specifies a 30-percent hatch. The ratio of foreground color to background color is 30:70.</summary>
        Percent30 = 10,
        /// <summary>Specifies a 40-percent hatch. The ratio of foreground color to background color is 40:60.</summary>
        Percent40 = 11,
        /// <summary>Specifies a 50-percent hatch. The ratio of foreground color to background color is 50:50.</summary>
        Percent50 = 12,
        /// <summary>Specifies a 60-percent hatch. The ratio of foreground color to background color is 60:40.</summary>
        Percent60 = 13,
        /// <summary>Specifies a 70-percent hatch. The ratio of foreground color to background color is 70:30.</summary>
        Percent70 = 14,
        /// <summary>Specifies a 75-percent hatch. The ratio of foreground color to background color is 75:25.</summary>
        Percent75 = 0xF,
        /// <summary>Specifies a 80-percent hatch. The ratio of foreground color to background color is 80:100.</summary>
        Percent80 = 0x10,
        /// <summary>Specifies a 90-percent hatch. The ratio of foreground color to background color is 90:10.</summary>
        Percent90 = 17,
        /// <summary>Specifies diagonal lines that slant to the right from top points to bottom points and are spaced 50 percent closer together than <see cref="F:System.Drawing.Drawing2D.HatchStyle.ForwardDiagonal" />, but are not antialiased.</summary>
        LightDownwardDiagonal = 18,
        /// <summary>Specifies diagonal lines that slant to the left from top points to bottom points and are spaced 50 percent closer together than <see cref="F:System.Drawing.Drawing2D.HatchStyle.BackwardDiagonal" />, but they are not antialiased.</summary>
        LightUpwardDiagonal = 19,
        /// <summary>Specifies diagonal lines that slant to the right from top points to bottom points, are spaced 50 percent closer together than, and are twice the width of <see cref="F:System.Drawing.Drawing2D.HatchStyle.ForwardDiagonal" />. This hatch pattern is not antialiased.</summary>
        DarkDownwardDiagonal = 20,
        /// <summary>Specifies diagonal lines that slant to the left from top points to bottom points, are spaced 50 percent closer together than <see cref="F:System.Drawing.Drawing2D.HatchStyle.BackwardDiagonal" />, and are twice its width, but the lines are not antialiased.</summary>
        DarkUpwardDiagonal = 21,
        /// <summary>Specifies diagonal lines that slant to the right from top points to bottom points, have the same spacing as hatch style <see cref="F:System.Drawing.Drawing2D.HatchStyle.ForwardDiagonal" />, and are triple its width, but are not antialiased.</summary>
        WideDownwardDiagonal = 22,
        /// <summary>Specifies diagonal lines that slant to the left from top points to bottom points, have the same spacing as hatch style <see cref="F:System.Drawing.Drawing2D.HatchStyle.BackwardDiagonal" />, and are triple its width, but are not antialiased.</summary>
        WideUpwardDiagonal = 23,
        /// <summary>Specifies vertical lines that are spaced 50 percent closer together than <see cref="F:System.Drawing.Drawing2D.HatchStyle.Vertical" />.</summary>
        LightVertical = 24,
        /// <summary>Specifies horizontal lines that are spaced 50 percent closer together than <see cref="F:System.Drawing.Drawing2D.HatchStyle.Horizontal" />.</summary>
        LightHorizontal = 25,
        /// <summary>Specifies vertical lines that are spaced 75 percent closer together than hatch style <see cref="F:System.Drawing.Drawing2D.HatchStyle.Vertical" /> (or 25 percent closer together than <see cref="F:System.Drawing.Drawing2D.HatchStyle.LightVertical" />).</summary>
        NarrowVertical = 26,
        /// <summary>Specifies horizontal lines that are spaced 75 percent closer together than hatch style <see cref="F:System.Drawing.Drawing2D.HatchStyle.Horizontal" /> (or 25 percent closer together than <see cref="F:System.Drawing.Drawing2D.HatchStyle.LightHorizontal" />).</summary>
        NarrowHorizontal = 27,
        /// <summary>Specifies vertical lines that are spaced 50 percent closer together than <see cref="F:System.Drawing.Drawing2D.HatchStyle.Vertical" /> and are twice its width.</summary>
        DarkVertical = 28,
        /// <summary>Specifies horizontal lines that are spaced 50 percent closer together than <see cref="F:System.Drawing.Drawing2D.HatchStyle.Horizontal" /> and are twice the width of <see cref="F:System.Drawing.Drawing2D.HatchStyle.Horizontal" />.</summary>
        DarkHorizontal = 29,
        /// <summary>Specifies dashed diagonal lines, that slant to the right from top points to bottom points.</summary>
        DashedDownwardDiagonal = 30,
        /// <summary>Specifies dashed diagonal lines, that slant to the left from top points to bottom points.</summary>
        DashedUpwardDiagonal = 0x1F,
        /// <summary>Specifies dashed horizontal lines.</summary>
        DashedHorizontal = 0x20,
        /// <summary>Specifies dashed vertical lines.</summary>
        DashedVertical = 33,
        /// <summary>Specifies a hatch that has the appearance of confetti.</summary>
        SmallConfetti = 34,
        /// <summary>Specifies a hatch that has the appearance of confetti, and is composed of larger pieces than <see cref="F:System.Drawing.Drawing2D.HatchStyle.SmallConfetti" />.</summary>
        LargeConfetti = 35,
        /// <summary>Specifies horizontal lines that are composed of zigzags.</summary>
        ZigZag = 36,
        /// <summary>Specifies horizontal lines that are composed of tildes.</summary>
        Wave = 37,
        /// <summary>Specifies a hatch that has the appearance of layered bricks that slant to the left from top points to bottom points.</summary>
        DiagonalBrick = 38,
        /// <summary>Specifies a hatch that has the appearance of horizontally layered bricks.</summary>
        HorizontalBrick = 39,
        /// <summary>Specifies a hatch that has the appearance of a woven material.</summary>
        Weave = 40,
        /// <summary>Specifies a hatch that has the appearance of a plaid material.</summary>
        Plaid = 41,
        /// <summary>Specifies a hatch that has the appearance of divots.</summary>
        Divot = 42,
        /// <summary>Specifies horizontal and vertical lines, each of which is composed of dots, that cross.</summary>
        DottedGrid = 43,
        /// <summary>Specifies forward diagonal and backward diagonal lines, each of which is composed of dots, that cross.</summary>
        DottedDiamond = 44,
        /// <summary>Specifies a hatch that has the appearance of diagonally layered shingles that slant to the right from top points to bottom points.</summary>
        Shingle = 45,
        /// <summary>Specifies a hatch that has the appearance of a trellis.</summary>
        Trellis = 46,
        /// <summary>Specifies a hatch that has the appearance of spheres laid adjacent to one another.</summary>
        Sphere = 47,
        /// <summary>Specifies horizontal and vertical lines that cross and are spaced 50 percent closer together than hatch style <see cref="F:System.Drawing.Drawing2D.HatchStyle.Cross" />.</summary>
        SmallGrid = 48,
        /// <summary>Specifies a hatch that has the appearance of a checkerboard.</summary>
        SmallCheckerBoard = 49,
        /// <summary>Specifies a hatch that has the appearance of a checkerboard with squares that are twice the size of <see cref="F:System.Drawing.Drawing2D.HatchStyle.SmallCheckerBoard" />.</summary>
        LargeCheckerBoard = 50,
        /// <summary>Specifies forward diagonal and backward diagonal lines that cross but are not antialiased.</summary>
        OutlinedDiamond = 51,
        /// <summary>Specifies a hatch that has the appearance of a checkerboard placed diagonally.</summary>
        SolidDiamond = 52,
        /// <summary>Specifies the hatch style <see cref="F:System.Drawing.Drawing2D.HatchStyle.Cross" />.</summary>
        LargeGrid = 4,
        /// <summary>Specifies hatch style <see cref="F:System.Drawing.Drawing2D.HatchStyle.Horizontal" />.</summary>
        Min = 0,
        /// <summary>Specifies hatch style <see cref="F:System.Drawing.Drawing2D.HatchStyle.SolidDiamond" />.</summary>
        Max = 4
    }
}