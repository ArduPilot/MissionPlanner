namespace MissionPlanner
{
    /// <summary>
    /// Broad vehicle classes for type-specific handling of MAVLink commands
    /// (e.g. which mavcmd set to load, whether loiter arcs apply).
    /// </summary>
    public enum VehicleClass
    {
        Unknown = 0,
        Copter,
        Plane,
        Rover,
        Tracker
    }
}
