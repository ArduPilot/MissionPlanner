namespace ClipperLib
{
    internal class TEdge
    {
        public IntPoint Bot;
        public IntPoint Curr;
        public IntPoint Top;
        public IntPoint Delta;
        public double Dx;
        public PolyType PolyTyp;
        public EdgeSide Side;
        public int WindDelta; //1 or -1 depending on winding direction
        public int WindCnt;
        public int WindCnt2; //winding count of the opposite polytype
        public int OutIdx;
        public TEdge Next;
        public TEdge Prev;
        public TEdge NextInLML;
        public TEdge NextInAEL;
        public TEdge PrevInAEL;
        public TEdge NextInSEL;
        public TEdge PrevInSEL;
    };
}