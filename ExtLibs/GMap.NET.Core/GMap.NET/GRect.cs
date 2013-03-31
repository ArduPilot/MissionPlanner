
namespace GMap.NET
{
   using System;
   using System.Globalization;

   /// <summary>
   /// the rect
   /// </summary>
   public struct GRect
   {
      public static readonly GRect Empty = new GRect();

      private int x;
      private int y;
      private int width;
      private int height;

      public GRect(int x, int y, int width, int height)
      {
         this.x = x;
         this.y = y;
         this.width = width;
         this.height = height;
      }

      public GRect(GPoint location, GSize size)
      {
         this.x = location.X;
         this.y = location.Y;
         this.width = size.Width;
         this.height = size.Height;
      }

      public static GRect FromLTRB(int left, int top, int right, int bottom)
      {
         return new GRect(left,
                              top,
                              right - left,
                              bottom - top);
      }

      public GPoint Location
      {
         get
         {
            return new GPoint(X, Y);
         }
         set
         {
            X = value.X;
            Y = value.Y;
         }
      }

      public GSize Size
      {
         get
         {
            return new GSize(Width, Height);
         }
         set
         {
            this.Width = value.Width;
            this.Height = value.Height;
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

      public int Left
      {
         get
         {
            return X;
         }
      }

      public int Top
      {
         get
         {
            return Y;
         }
      }

      public int Right
      {
         get
         {
            return X + Width;
         }
      }

      public int Bottom
      {
         get
         {
            return Y + Height;
         }
      }

      public bool IsEmpty
      {
         get
         {
            return height == 0 && width == 0 && x == 0 && y == 0;
         }
      }

      public override bool Equals(object obj)
      {
         if(!(obj is GRect))
            return false;

         GRect comp = (GRect) obj;

         return (comp.X == this.X) &&
            (comp.Y == this.Y) &&
            (comp.Width == this.Width) &&
            (comp.Height == this.Height);
      }

      public static bool operator==(GRect left, GRect right)
      {
         return (left.X == right.X 
                    && left.Y == right.Y 
                    && left.Width == right.Width
                    && left.Height == right.Height);
      }

      public static bool operator!=(GRect left, GRect right)
      {
         return !(left == right);
      }

      public bool Contains(int x, int y)
      {
         return this.X <= x && 
            x < this.X + this.Width &&
            this.Y <= y && 
            y < this.Y + this.Height;
      }

      public bool Contains(GPoint pt)
      {
         return Contains(pt.X, pt.Y);
      }

      public bool Contains(GRect rect)
      {
         return (this.X <= rect.X) &&
            ((rect.X + rect.Width) <= (this.X + this.Width)) && 
            (this.Y <= rect.Y) &&
            ((rect.Y + rect.Height) <= (this.Y + this.Height));
      }

      public override int GetHashCode()
      {
         if(this.IsEmpty)
         {
            return 0;
         }
         return (((this.X ^ ((this.Y << 13) | (this.Y >> 0x13))) ^ ((this.Width << 0x1a) | (this.Width >> 6))) ^ ((this.Height << 7) | (this.Height >> 0x19)));
      }

      public void Inflate(int width, int height)
      {
         this.X -= width;
         this.Y -= height;
         this.Width += 2*width;
         this.Height += 2*height;
      }

      public void Inflate(GSize size)
      {

         Inflate(size.Width, size.Height);
      }

      public static GRect Inflate(GRect rect, int x, int y)
      {
         GRect r = rect;
         r.Inflate(x, y);
         return r;
      }

      public void Intersect(GRect rect)
      {
         GRect result = GRect.Intersect(rect, this);

         this.X = result.X;
         this.Y = result.Y;
         this.Width = result.Width;
         this.Height = result.Height;
      }

      public static GRect Intersect(GRect a, GRect b)
      {
         int x1 = Math.Max(a.X, b.X);
         int x2 = Math.Min(a.X + a.Width, b.X + b.Width);
         int y1 = Math.Max(a.Y, b.Y);
         int y2 = Math.Min(a.Y + a.Height, b.Y + b.Height);

         if(x2 >= x1
                && y2 >= y1)
         {

            return new GRect(x1, y1, x2 - x1, y2 - y1);
         }
         return GRect.Empty;
      }

      public bool IntersectsWith(GRect rect)
      {
         return (rect.X < this.X + this.Width) &&
            (this.X < (rect.X + rect.Width)) && 
            (rect.Y < this.Y + this.Height) &&
            (this.Y < rect.Y + rect.Height);
      }

      public static GRect Union(GRect a, GRect b)
      {
         int x1 = Math.Min(a.X, b.X);
         int x2 = Math.Max(a.X + a.Width, b.X + b.Width);
         int y1 = Math.Min(a.Y, b.Y);
         int y2 = Math.Max(a.Y + a.Height, b.Y + b.Height);

         return new GRect(x1, y1, x2 - x1, y2 - y1);
      }

      public void Offset(GPoint pos)
      {
         Offset(pos.X, pos.Y);
      }

      public void Offset(int x, int y)
      {
         this.X += x;
         this.Y += y;
      }

      public override string ToString()
      {
         return "{X=" + X.ToString(CultureInfo.CurrentCulture) + ",Y=" + Y.ToString(CultureInfo.CurrentCulture) + 
            ",Width=" + Width.ToString(CultureInfo.CurrentCulture) +
            ",Height=" + Height.ToString(CultureInfo.CurrentCulture) + "}";
      }
   }
}