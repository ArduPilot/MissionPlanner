using Org.Gdal.Gdal;
using System;
using System.ComponentModel;

namespace GDAL
{
    public static class Extensions
    {
        public static int GetCount(this ColorTable ct)
        {
            return ct.Count;
        }

        public static ColorTable GetRasterColorTable(this Band ct)
        {
            return ct.RasterColorTable;
        }   

        public static int GetColorInterpretation(this Band ct)
        {
            return ct.ColorInterpretation;
        }
        
    }

    public class ColorEntry
    {
        public static implicit operator ColorEntry(int d)
        {
            return new ColorEntry()
            {
                c1 = (d) & 0xff, //r 
                c2 = (d >> 8) & 0xff, //g 
                c3 = (d >> 16) & 0xff, //b
                c4 = (d >> 24) & 0xff //a
            };
        }

        public int c4;
        public int c1;
        public int c2;
        public int c3;
    }

    public enum DataType
    {
        GDT_Unknown,
        GDT_Byte,
        GDT_UInt16,
        GDT_Int16,
        GDT_UInt32,
        GDT_Int32,
        GDT_Float32,
        GDT_Float64,
        GDT_CInt16,
        GDT_CInt32,
        GDT_CFloat32,
        GDT_CFloat64,
        GDT_TypeCount
    }
    public enum ColorInterp
    {
        GCI_Undefined = 0,
        GCI_GrayIndex = 1,
        GCI_PaletteIndex = 2,
        GCI_RedBand = 3,
        GCI_GreenBand = 4,
        GCI_BlueBand = 5,
        GCI_AlphaBand = 6,
        GCI_HueBand = 7,
        GCI_SaturationBand = 8,
        GCI_LightnessBand = 9,
        GCI_CyanBand = 10,
        GCI_MagentaBand = 11,
        GCI_YellowBand = 12,
        GCI_BlackBand = 13,
        GCI_YCbCr_YBand = 14,
        GCI_YCbCr_CbBand = 0xF,
        GCI_YCbCr_CrBand = 0x10,
        GCI_Max = 0x10
    }

}
public struct Point : IEquatable<Point>
{
	public static readonly Point Empty;

	private int x;

	private int y;

	[Browsable(false)]
	public bool IsEmpty
	{
		get
		{
			if (x == 0)
			{
				return y == 0;
			}
			return false;
		}
	}

	public int X
	{
		get
		{
			return x;
		}
		set
		{
			x = value;
		}
	}

	public int Y
	{
		get
		{
			return y;
		}
		set
		{
			y = value;
		}
	}

	public Point(int x, int y)
	{
		this.x = x;
		this.y = y;
	}

	public Point(Size sz)
	{
		x = sz.Width;
		y = sz.Height;
	}

	public Point(int dw)
	{
		x = LowInt16(dw);
		y = HighInt16(dw);
	}

	public static implicit operator PointF(Point p)
	{
		return new PointF(p.X, p.Y);
	}

	public static explicit operator Size(Point p)
	{
		return new Size(p.X, p.Y);
	}

	public static Point operator +(Point pt, Size sz)
	{
		return Add(pt, sz);
	}

	public static Point operator -(Point pt, Size sz)
	{
		return Subtract(pt, sz);
	}

	public static bool operator ==(Point left, Point right)
	{
		if (left.X == right.X)
		{
			return left.Y == right.Y;
		}
		return false;
	}

	public static bool operator !=(Point left, Point right)
	{
		return !(left == right);
	}

	public static Point Add(Point pt, Size sz)
	{
		return new Point(pt.X + sz.Width, pt.Y + sz.Height);
	}

	public static Point Subtract(Point pt, Size sz)
	{
		return new Point(pt.X - sz.Width, pt.Y - sz.Height);
	}

	public static Point Ceiling(PointF value)
	{
		return new Point((int)Math.Ceiling(value.X), (int)Math.Ceiling(value.Y));
	}

	public static Point Truncate(PointF value)
	{
		return new Point((int)value.X, (int)value.Y);
	}

	public static Point Round(PointF value)
	{
		return new Point((int)Math.Round(value.X), (int)Math.Round(value.Y));
	}

	public override bool Equals(object obj)
	{
		if (obj is Point)
		{
			return Equals((Point)obj);
		}
		return false;
	}

	public bool Equals(Point other)
	{
		return this == other;
	}


	public void Offset(int dx, int dy)
	{
		X += dx;
		Y += dy;
	}

	public void Offset(Point p)
	{
		Offset(p.X, p.Y);
	}

	public override string ToString()
	{
		return "{X=" + X + ",Y=" + Y + "}";
	}

	private static short HighInt16(int n)
	{
		return (short)((n >> 16) & 0xFFFF);
	}

	private static short LowInt16(int n)
	{
		return (short)(n & 0xFFFF);
	}
}
public struct Size : IEquatable<Size>
{
	public static readonly Size Empty;

	private int width;

	private int height;

	[Browsable(false)]
	public bool IsEmpty
	{
		get
		{
			if (width == 0)
			{
				return height == 0;
			}
			return false;
		}
	}

	public int Width
	{
		get
		{
			return width;
		}
		set
		{
			width = value;
		}
	}

	public int Height
	{
		get
		{
			return height;
		}
		set
		{
			height = value;
		}
	}

	public Size(Point pt)
	{
		width = pt.X;
		height = pt.Y;
	}

	public Size(int width, int height)
	{
		this.width = width;
		this.height = height;
	}

	public static implicit operator SizeF(Size p)
	{
		return new SizeF(p.Width, p.Height);
	}

	public static Size operator +(Size sz1, Size sz2)
	{
		return Add(sz1, sz2);
	}

	public static Size operator -(Size sz1, Size sz2)
	{
		return Subtract(sz1, sz2);
	}

	public static bool operator ==(Size sz1, Size sz2)
	{
		if (sz1.Width == sz2.Width)
		{
			return sz1.Height == sz2.Height;
		}
		return false;
	}

	public static bool operator !=(Size sz1, Size sz2)
	{
		return !(sz1 == sz2);
	}

	public static explicit operator Point(Size size)
	{
		return new Point(size.Width, size.Height);
	}

	public static Size Add(Size sz1, Size sz2)
	{
		return new Size(sz1.Width + sz2.Width, sz1.Height + sz2.Height);
	}

	public static Size Ceiling(SizeF value)
	{
		return new Size((int)Math.Ceiling(value.Width), (int)Math.Ceiling(value.Height));
	}

	public static Size Subtract(Size sz1, Size sz2)
	{
		return new Size(sz1.Width - sz2.Width, sz1.Height - sz2.Height);
	}

	public static Size Truncate(SizeF value)
	{
		return new Size((int)value.Width, (int)value.Height);
	}

	public static Size Round(SizeF value)
	{
		return new Size((int)Math.Round(value.Width), (int)Math.Round(value.Height));
	}

	public override bool Equals(object obj)
	{
		if (obj is Size)
		{
			return Equals((Size)obj);
		}
		return false;
	}

	public bool Equals(Size other)
	{
		return this == other;
	}



	public override string ToString()
	{
		return "{Width=" + width + ", Height=" + height + "}";
	}
}
public struct PointF : IEquatable<PointF>
{
	public static readonly PointF Empty;

	private float x;

	private float y;

	[Browsable(false)]
	public bool IsEmpty
	{
		get
		{
			if (x == 0f)
			{
				return y == 0f;
			}
			return false;
		}
	}

	public float X
	{
		get
		{
			return x;
		}
		set
		{
			x = value;
		}
	}

	public float Y
	{
		get
		{
			return y;
		}
		set
		{
			y = value;
		}
	}

	public PointF(float x, float y)
	{
		this.x = x;
		this.y = y;
	}

	public static PointF operator +(PointF pt, Size sz)
	{
		return Add(pt, sz);
	}

	public static PointF operator -(PointF pt, Size sz)
	{
		return Subtract(pt, sz);
	}

	public static PointF operator +(PointF pt, SizeF sz)
	{
		return Add(pt, sz);
	}

	public static PointF operator -(PointF pt, SizeF sz)
	{
		return Subtract(pt, sz);
	}

	public static bool operator ==(PointF left, PointF right)
	{
		if (left.X == right.X)
		{
			return left.Y == right.Y;
		}
		return false;
	}

	public static bool operator !=(PointF left, PointF right)
	{
		return !(left == right);
	}

	public static PointF Add(PointF pt, Size sz)
	{
		return new PointF(pt.X + (float)sz.Width, pt.Y + (float)sz.Height);
	}

	public static PointF Subtract(PointF pt, Size sz)
	{
		return new PointF(pt.X - (float)sz.Width, pt.Y - (float)sz.Height);
	}

	public static PointF Add(PointF pt, SizeF sz)
	{
		return new PointF(pt.X + sz.Width, pt.Y + sz.Height);
	}

	public static PointF Subtract(PointF pt, SizeF sz)
	{
		return new PointF(pt.X - sz.Width, pt.Y - sz.Height);
	}

	public override bool Equals(object obj)
	{
		if (obj is PointF)
		{
			return Equals((PointF)obj);
		}
		return false;
	}

	public bool Equals(PointF other)
	{
		return this == other;
	}



	public override string ToString()
	{
		return "{X=" + x + ", Y=" + y + "}";
	}
}

public struct SizeF : IEquatable<SizeF>
{
	public static readonly SizeF Empty;

	private float width;

	private float height;

	[Browsable(false)]
	public bool IsEmpty
	{
		get
		{
			if (width == 0f)
			{
				return height == 0f;
			}
			return false;
		}
	}

	public float Width
	{
		get
		{
			return width;
		}
		set
		{
			width = value;
		}
	}

	public float Height
	{
		get
		{
			return height;
		}
		set
		{
			height = value;
		}
	}

	public SizeF(SizeF size)
	{
		width = size.width;
		height = size.height;
	}

	public SizeF(PointF pt)
	{
		width = pt.X;
		height = pt.Y;
	}

	public SizeF(float width, float height)
	{
		this.width = width;
		this.height = height;
	}

	public static SizeF operator +(SizeF sz1, SizeF sz2)
	{
		return Add(sz1, sz2);
	}

	public static SizeF operator -(SizeF sz1, SizeF sz2)
	{
		return Subtract(sz1, sz2);
	}

	public static bool operator ==(SizeF sz1, SizeF sz2)
	{
		if (sz1.Width == sz2.Width)
		{
			return sz1.Height == sz2.Height;
		}
		return false;
	}

	public static bool operator !=(SizeF sz1, SizeF sz2)
	{
		return !(sz1 == sz2);
	}

	public static explicit operator PointF(SizeF size)
	{
		return new PointF(size.Width, size.Height);
	}

	public static SizeF Add(SizeF sz1, SizeF sz2)
	{
		return new SizeF(sz1.Width + sz2.Width, sz1.Height + sz2.Height);
	}

	public static SizeF Subtract(SizeF sz1, SizeF sz2)
	{
		return new SizeF(sz1.Width - sz2.Width, sz1.Height - sz2.Height);
	}

	public override bool Equals(object obj)
	{
		if (obj is SizeF)
		{
			return Equals((SizeF)obj);
		}
		return false;
	}

	public bool Equals(SizeF other)
	{
		return this == other;
	}


	public PointF ToPointF()
	{
		return (PointF)this;
	}

	public Size ToSize()
	{
		return Size.Truncate(this);
	}

	public override string ToString()
	{
		return "{Width=" + width + ", Height=" + height + "}";
	}
}

[Serializable]
public struct RectangleF : IEquatable<RectangleF>
{
	public static readonly RectangleF Empty;

	private float x;

	private float y;

	private float width;

	private float height;

	[Browsable(false)]
	public PointF Location
	{
		get
		{
			return new PointF(X, Y);
		}
		set
		{
			X = value.X;
			Y = value.Y;
		}
	}

	[Browsable(false)]
	public SizeF Size
	{
		get
		{
			return new SizeF(Width, Height);
		}
		set
		{
			Width = value.Width;
			Height = value.Height;
		}
	}

	public float X
	{
		get
		{
			return x;
		}
		set
		{
			x = value;
		}
	}

	public float Y
	{
		get
		{
			return y;
		}
		set
		{
			y = value;
		}
	}

	public float Width
	{
		get
		{
			return width;
		}
		set
		{
			width = value;
		}
	}

	public float Height
	{
		get
		{
			return height;
		}
		set
		{
			height = value;
		}
	}

	[Browsable(false)]
	public float Left => X;

	[Browsable(false)]
	public float Top => Y;

	[Browsable(false)]
	public float Right => X + Width;

	[Browsable(false)]
	public float Bottom => Y + Height;

	[Browsable(false)]
	public bool IsEmpty
	{
		get
		{
			if (!(Width <= 0f))
			{
				return Height <= 0f;
			}
			return true;
		}
	}

	public RectangleF(float x, float y, float width, float height)
	{
		this.x = x;
		this.y = y;
		this.width = width;
		this.height = height;
	}

	public RectangleF(PointF location, SizeF size)
	{
		x = location.X;
		y = location.Y;
		width = size.Width;
		height = size.Height;
	}

	public static RectangleF FromLTRB(float left, float top, float right, float bottom)
	{
		return new RectangleF(left, top, right - left, bottom - top);
	}

	public override bool Equals(object obj)
	{
		if (obj is RectangleF)
		{
			return Equals((RectangleF)obj);
		}
		return false;
	}

	public bool Equals(RectangleF other)
	{
		return this == other;
	}

	public static bool operator ==(RectangleF left, RectangleF right)
	{
		if (left.X == right.X && left.Y == right.Y && left.Width == right.Width)
		{
			return left.Height == right.Height;
		}
		return false;
	}

	public static bool operator !=(RectangleF left, RectangleF right)
	{
		return !(left == right);
	}

	public bool Contains(float x, float y)
	{
		if (X <= x && x < X + Width && Y <= y)
		{
			return y < Y + Height;
		}
		return false;
	}

	public bool Contains(PointF pt)
	{
		return Contains(pt.X, pt.Y);
	}

	public bool Contains(RectangleF rect)
	{
		if (X <= rect.X && rect.X + rect.Width <= X + Width && Y <= rect.Y)
		{
			return rect.Y + rect.Height <= Y + Height;
		}
		return false;
	}

	public void Inflate(float x, float y)
	{
		X -= x;
		Y -= y;
		Width += 2f * x;
		Height += 2f * y;
	}

	public void Inflate(SizeF size)
	{
		Inflate(size.Width, size.Height);
	}

	public static RectangleF Inflate(RectangleF rect, float x, float y)
	{
		RectangleF result = rect;
		result.Inflate(x, y);
		return result;
	}

	public void Intersect(RectangleF rect)
	{
		RectangleF rectangleF = Intersect(rect, this);
		X = rectangleF.X;
		Y = rectangleF.Y;
		Width = rectangleF.Width;
		Height = rectangleF.Height;
	}

	public static RectangleF Intersect(RectangleF a, RectangleF b)
	{
		float num = Math.Max(a.X, b.X);
		float num2 = Math.Min(a.X + a.Width, b.X + b.Width);
		float num3 = Math.Max(a.Y, b.Y);
		float num4 = Math.Min(a.Y + a.Height, b.Y + b.Height);
		if (num2 >= num && num4 >= num3)
		{
			return new RectangleF(num, num3, num2 - num, num4 - num3);
		}
		return Empty;
	}

	public bool IntersectsWith(RectangleF rect)
	{
		if (rect.X < X + Width && X < rect.X + rect.Width && rect.Y < Y + Height)
		{
			return Y < rect.Y + rect.Height;
		}
		return false;
	}

	public static RectangleF Union(RectangleF a, RectangleF b)
	{
		float num = Math.Min(a.X, b.X);
		float num2 = Math.Max(a.X + a.Width, b.X + b.Width);
		float num3 = Math.Min(a.Y, b.Y);
		float num4 = Math.Max(a.Y + a.Height, b.Y + b.Height);
		return new RectangleF(num, num3, num2 - num, num4 - num3);
	}

	public void Offset(PointF pos)
	{
		Offset(pos.X, pos.Y);
	}

	public void Offset(float x, float y)
	{
		X += x;
		Y += y;
	}

	public static implicit operator RectangleF(Rectangle r)
	{
		return new RectangleF(r.X, r.Y, r.Width, r.Height);
	}

	public override string ToString()
	{
		return "{X=" + X + ",Y=" + Y + ",Width=" + Width + ",Height=" + Height + "}";
	}
}

public struct Rectangle : IEquatable<Rectangle>
{
	public static readonly Rectangle Empty;

	private int x;

	private int y;

	private int width;

	private int height;

	[Browsable(false)]
	public Point Location
	{
		get
		{
			return new Point(X, Y);
		}
		set
		{
			X = value.X;
			Y = value.Y;
		}
	}

	[Browsable(false)]
	public Size Size
	{
		get
		{
			return new Size(Width, Height);
		}
		set
		{
			Width = value.Width;
			Height = value.Height;
		}
	}

	public int X
	{
		get
		{
			return x;
		}
		set
		{
			x = value;
		}
	}

	public int Y
	{
		get
		{
			return y;
		}
		set
		{
			y = value;
		}
	}

	public int Width
	{
		get
		{
			return width;
		}
		set
		{
			width = value;
		}
	}

	public int Height
	{
		get
		{
			return height;
		}
		set
		{
			height = value;
		}
	}

	[Browsable(false)]
	public int Left => X;

	[Browsable(false)]
	public int Top => Y;

	[Browsable(false)]
	public int Right => X + Width;

	[Browsable(false)]
	public int Bottom => Y + Height;

	[Browsable(false)]
	public bool IsEmpty
	{
		get
		{
			if (height == 0 && width == 0 && x == 0)
			{
				return y == 0;
			}
			return false;
		}
	}

	public Rectangle(int x, int y, int width, int height)
	{
		this.x = x;
		this.y = y;
		this.width = width;
		this.height = height;
	}

	public Rectangle(Point location, Size size)
	{
		x = location.X;
		y = location.Y;
		width = size.Width;
		height = size.Height;
	}

	public static Rectangle FromLTRB(int left, int top, int right, int bottom)
	{
		return new Rectangle(left, top, right - left, bottom - top);
	}

	public override bool Equals(object obj)
	{
		if (obj is Rectangle)
		{
			return Equals((Rectangle)obj);
		}
		return false;
	}

	public bool Equals(Rectangle other)
	{
		return this == other;
	}

	public static bool operator ==(Rectangle left, Rectangle right)
	{
		if (left.X == right.X && left.Y == right.Y && left.Width == right.Width)
		{
			return left.Height == right.Height;
		}
		return false;
	}

	public static bool operator !=(Rectangle left, Rectangle right)
	{
		return !(left == right);
	}

	public static Rectangle Ceiling(RectangleF value)
	{
		return new Rectangle((int)Math.Ceiling(value.X), (int)Math.Ceiling(value.Y), (int)Math.Ceiling(value.Width), (int)Math.Ceiling(value.Height));
	}

	public static Rectangle Truncate(RectangleF value)
	{
		return new Rectangle((int)value.X, (int)value.Y, (int)value.Width, (int)value.Height);
	}

	public static Rectangle Round(RectangleF value)
	{
		return new Rectangle((int)Math.Round(value.X), (int)Math.Round(value.Y), (int)Math.Round(value.Width), (int)Math.Round(value.Height));
	}

	public bool Contains(int x, int y)
	{
		if (X <= x && x < X + Width && Y <= y)
		{
			return y < Y + Height;
		}
		return false;
	}

	public bool Contains(Point pt)
	{
		return Contains(pt.X, pt.Y);
	}

	public bool Contains(Rectangle rect)
	{
		if (X <= rect.X && rect.X + rect.Width <= X + Width && Y <= rect.Y)
		{
			return rect.Y + rect.Height <= Y + Height;
		}
		return false;
	}


	public void Inflate(int width, int height)
	{
		X -= width;
		Y -= height;
		Width += 2 * width;
		Height += 2 * height;
	}

	public void Inflate(Size size)
	{
		Inflate(size.Width, size.Height);
	}

	public static Rectangle Inflate(Rectangle rect, int x, int y)
	{
		Rectangle result = rect;
		result.Inflate(x, y);
		return result;
	}

	public void Intersect(Rectangle rect)
	{
		Rectangle rectangle = Intersect(rect, this);
		X = rectangle.X;
		Y = rectangle.Y;
		Width = rectangle.Width;
		Height = rectangle.Height;
	}

	public static Rectangle Intersect(Rectangle a, Rectangle b)
	{
		int num = Math.Max(a.X, b.X);
		int num2 = Math.Min(a.X + a.Width, b.X + b.Width);
		int num3 = Math.Max(a.Y, b.Y);
		int num4 = Math.Min(a.Y + a.Height, b.Y + b.Height);
		if (num2 >= num && num4 >= num3)
		{
			return new Rectangle(num, num3, num2 - num, num4 - num3);
		}
		return Empty;
	}

	public bool IntersectsWith(Rectangle rect)
	{
		if (rect.X < X + Width && X < rect.X + rect.Width && rect.Y < Y + Height)
		{
			return Y < rect.Y + rect.Height;
		}
		return false;
	}

	public static Rectangle Union(Rectangle a, Rectangle b)
	{
		int num = Math.Min(a.X, b.X);
		int num2 = Math.Max(a.X + a.Width, b.X + b.Width);
		int num3 = Math.Min(a.Y, b.Y);
		int num4 = Math.Max(a.Y + a.Height, b.Y + b.Height);
		return new Rectangle(num, num3, num2 - num, num4 - num3);
	}

	public void Offset(Point pos)
	{
		Offset(pos.X, pos.Y);
	}

	public void Offset(int x, int y)
	{
		X += x;
		Y += y;
	}

	public override string ToString()
	{
		return "{X=" + X + ",Y=" + Y + ",Width=" + Width + ",Height=" + Height + "}";
	}
}