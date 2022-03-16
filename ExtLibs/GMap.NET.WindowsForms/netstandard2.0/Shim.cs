using SkiaSharp;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;


namespace SvgNet.SvgGdi
{
}
namespace OpenTK.Graphics{}
namespace OpenTK.Platform{}
public class GdiGraphics : Graphics
{
    public GdiGraphics(SKSurface surface) : base(surface)
    {
    }

    public GdiGraphics(IntPtr handle, int width, int height) : base(handle, width, height)
    {
    }

    public GdiGraphics(Graphics fromImage) : base(fromImage.Surface)
    {
    }
}

public class SkiaGraphics : Graphics
{
    public SkiaGraphics(SKSurface surface) : base(surface)
    {
    }

    public SkiaGraphics(IntPtr handle, int width, int height) : base(handle, width, height)
    {
    }
}
