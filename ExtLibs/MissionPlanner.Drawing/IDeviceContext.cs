using System;

namespace System.Drawing
{
    public interface IDeviceContext : IDisposable
    {
        void ReleaseHdc();
        IntPtr GetHdc();
    }
}