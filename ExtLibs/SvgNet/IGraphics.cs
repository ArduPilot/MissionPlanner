using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;

namespace System
{
    public interface IGraphics : IGraphics<Region, CompositingMode, CompositingQuality, InterpolationMode,
        GraphicsUnit, PixelOffsetMode,
        SmoothingMode, TextRenderingHint, Matrix, GraphicsContainer, Pen, FillMode, Icon, Image, ImageAttributes,
        GraphicsPath, Font, Brush,
        StringFormat, FlushIntention, MatrixOrder, GraphicsState, CombineMode, CoordinateSpace>
    {

    }
    
}