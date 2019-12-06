using System;

namespace MissionPlanner.Drawing
{
    public interface IDeviceContext : IDisposable
    {
        void ReleaseHdc();
        IntPtr GetHdc();
    }
}