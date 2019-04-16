namespace MissionPlanner.Utilities.Drawing
{
    public enum GraphicsUnit
    {
        /// <summary>Specifies the world coordinate system unit as the unit of measure.</summary>
        World,
        /// <summary>Specifies the unit of measure of the display device. Typically pixels for video displays, and 1/100 inch for printers.</summary>
        Display,
        /// <summary>Specifies a device pixel as the unit of measure.</summary>
        Pixel,
        /// <summary>Specifies a printer's point (1/72 inch) as the unit of measure.</summary>
        Point,
        /// <summary>Specifies the inch as the unit of measure.</summary>
        Inch,
        /// <summary>Specifies the document unit (1/300 inch) as the unit of measure.</summary>
        Document,
        /// <summary>Specifies the millimeter as the unit of measure.</summary>
        Millimeter
    }
}