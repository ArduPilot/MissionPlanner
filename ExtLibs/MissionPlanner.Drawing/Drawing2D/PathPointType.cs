namespace MissionPlanner.Drawing.Drawing2D
{
    public enum PathPointType
    {
        Start = 0,
        Line = 1,
        Bezier = 3,
        Bezier3 = 3,
        PathTypeMask = 7,
        DashMode = 16,
        PathMarker = 32,
        CloseSubpath = 128,
    }
}