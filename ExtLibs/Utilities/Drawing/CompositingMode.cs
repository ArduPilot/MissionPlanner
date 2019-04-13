namespace MissionPlanner.Utilities.Drawing
{
    public enum CompositingMode
    {
        /// <summary>Specifies that when a color is rendered, it is blended with the background color. The blend is determined by the alpha component of the color being rendered.</summary>
        SourceOver,
        /// <summary>Specifies that when a color is rendered, it overwrites the background color.</summary>
        SourceCopy
    }
}