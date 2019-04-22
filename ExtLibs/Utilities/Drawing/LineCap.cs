namespace MissionPlanner.Utilities.Drawing
{
    public enum LineCap
    {
        /// <summary>Specifies a flat line cap.</summary>
        Flat = 0,
        /// <summary>Specifies a square line cap.</summary>
        Square = 1,
        /// <summary>Specifies a round line cap.</summary>
        Round = 2,
        /// <summary>Specifies a triangular line cap.</summary>
        Triangle = 3,
        /// <summary>Specifies no anchor.</summary>
        NoAnchor = 0x10,
        /// <summary>Specifies a square anchor line cap.</summary>
        SquareAnchor = 17,
        /// <summary>Specifies a round anchor cap.</summary>
        RoundAnchor = 18,
        /// <summary>Specifies a diamond anchor cap.</summary>
        DiamondAnchor = 19,
        /// <summary>Specifies an arrow-shaped anchor cap.</summary>
        ArrowAnchor = 20,
        /// <summary>Specifies a custom line cap.</summary>
        Custom = 0xFF,
        /// <summary>Specifies a mask used to check whether a line cap is an anchor cap.</summary>
        AnchorMask = 240
    }
}