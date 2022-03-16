using System;

namespace ClipperLib
{
    internal struct IntRect
    {
        public Int64 left;
        public Int64 top;
        public Int64 right;
        public Int64 bottom;

        public IntRect(Int64 l, Int64 t, Int64 r, Int64 b)
        {
            this.left = l;
            this.top = t;
            this.right = r;
            this.bottom = b;
        }

        public IntRect(IntRect ir)
        {
            this.left = ir.left;
            this.top = ir.top;
            this.right = ir.right;
            this.bottom = ir.bottom;
        }
    }
}