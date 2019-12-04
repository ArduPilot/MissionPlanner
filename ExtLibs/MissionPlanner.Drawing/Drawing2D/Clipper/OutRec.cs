namespace ClipperLib
{
    internal class OutRec
    {
        public int Idx;
        public bool IsHole;
        public bool IsOpen;
        public OutRec FirstLeft; //see comments in clipper.pas
        public OutPt Pts;
        public OutPt BottomPt;
        public PolyNode PolyNode;
    };
}