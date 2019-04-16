namespace MissionPlanner.Utilities.Drawing
{
    public enum FlushIntention
    {
        /// <summary>Specifies that the stack of all graphics operations is flushed immediately.</summary>
        Flush,
        /// <summary>Specifies that all graphics operations on the stack are executed as soon as possible. This synchronizes the graphics state.</summary>
        Sync
    }
}