using System;

namespace ClipperLib
{
    internal class LocalMinima
    {
        public Int64 Y;
        public TEdge LeftBound;
        public TEdge RightBound;
        public LocalMinima Next;
    };
}