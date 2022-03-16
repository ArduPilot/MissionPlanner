using System;

namespace MissionPlanner.Utilities
{
    public interface IBrowserOpen
    {
        bool OpenURL(Uri uri);
    }
}