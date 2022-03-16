using System.Collections.Generic;

namespace MissionPlanner.Utilities
{
    public class LineOffset
    {
        public static List<PointLatLngAlt> GetPolygon(List<PointLatLngAlt> polyline, int distm)
        {
            if (polyline.Count <= 3)
                return new List<PointLatLngAlt>();

            List<PointLatLngAlt> leftoffsetpoints = new List<PointLatLngAlt>();

            List<PointLatLngAlt> rightoffsetpoints = new List<PointLatLngAlt>();

            PointLatLngAlt prevpoint = polyline[0];

            // generate a point list for all points
            foreach (var point in polyline)
            {
                if (point == prevpoint)
                    continue;

                double dist = prevpoint.GetDistance(point);
                if (dist < (distm*1.1))
                    continue;

                double bearing = prevpoint.GetBearing(point);

                leftoffsetpoints.Add(point.newpos(bearing - 90, distm));
                rightoffsetpoints.Add(point.newpos(bearing + 90, distm));

                prevpoint = point;
            }

            if (leftoffsetpoints.Count <= 1)
                return new List<PointLatLngAlt>();

            //
            List<PointLatLngAlt> polygonPoints = new List<PointLatLngAlt>();

            polygonPoints.AddRange(leftoffsetpoints);

            rightoffsetpoints.Reverse();

            polygonPoints.AddRange(rightoffsetpoints);

            return polygonPoints;
        }
    }
}