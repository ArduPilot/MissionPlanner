namespace MissionPlanner.Utilities
{
    public class FenceCircle
    {
        /// <summary>
        /// Circle centre. Not used (null) for HomeInclusive, which is always centred on the vehicle's home.
        /// </summary>
        public PointLatLngAlt Center { get; set; }
        public float Radius { get; set; }

        public PolyType Mode { get; set; }

        public enum PolyType
        {
            Inclusive = MAVLink.MAV_CMD.FENCE_CIRCLE_INCLUSION,
            Exclusive = MAVLink.MAV_CMD.FENCE_CIRCLE_EXCLUSION,
            HomeInclusive = MAVLink.MAV_CMD.FENCE_HOME_CIRCLE_INCLUSION
        }
    }
}