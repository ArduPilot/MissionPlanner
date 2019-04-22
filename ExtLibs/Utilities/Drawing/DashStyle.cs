namespace MissionPlanner.Utilities.Drawing
{
    public enum DashStyle
    {
        /// <summary>Specifies a solid line.</summary>
        Solid,
        /// <summary>Specifies a line consisting of dashes.</summary>
        Dash,
        /// <summary>Specifies a line consisting of dots.</summary>
        Dot,
        /// <summary>Specifies a line consisting of a repeating pattern of dash-dot.</summary>
        DashDot,
        /// <summary>Specifies a line consisting of a repeating pattern of dash-dot-dot.</summary>
        DashDotDot,
        /// <summary>Specifies a user-defined custom dash style.</summary>
        Custom
    }
}