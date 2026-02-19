namespace MissionPlanner.Utilities
{
    /// <summary>
    /// Stored on plus-marker Tags to identify the two adjacent waypoints.
    /// Clicking the plus-marker inserts a new waypoint between
    /// <see cref="now"/> and <see cref="next"/>.
    /// </summary>
    public struct midline
    {
        public PointLatLngAlt now { get; set; }
        public PointLatLngAlt next { get; set; }
    }
}
