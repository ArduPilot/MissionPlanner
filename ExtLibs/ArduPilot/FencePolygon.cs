using System.Collections.Generic;

namespace MissionPlanner.Utilities
{
    public class FencePolygon
    {
        public List<PointLatLngAlt> Points { get; set; } = new List<PointLatLngAlt>();

        public PolyType Mode { get; set; }

        public enum PolyType
        {
            Inclusive = MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_INCLUSION,
            Exclusive = MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_EXCLUSION
        }
    }
}