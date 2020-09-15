using System;
using System.Collections.Generic;

namespace ClipperLib
{
    internal class Clipper : ClipperBase
    {
        //InitOptions that can be passed to the constructor ...
        public const int ioReverseSolution = 1;
        public const int ioStrictlySimple = 2;
        public const int ioPreserveCollinear = 4;

        private List<OutRec> m_PolyOuts;
        private ClipType m_ClipType;
        private Scanbeam m_Scanbeam;
        private TEdge m_ActiveEdges;
        private TEdge m_SortedEdges;
        private IntersectNode m_IntersectNodes;
        private bool m_ExecuteLocked;
        private PolyFillType m_ClipFillType;
        private PolyFillType m_SubjFillType;
        private List<Join> m_Joins;
        private List<Join> m_GhostJoins;
        private bool m_UsingPolyTree;
#if use_xyz
      public delegate void TZFillCallback(IntPoint vert1, IntPoint vert2, ref IntPoint intersectPt);
      public TZFillCallback ZFillFunction { get; set; }
#endif
        public Clipper(int InitOptions = 0) : base() //constructor
        {
            m_Scanbeam = null;
            m_ActiveEdges = null;
            m_SortedEdges = null;
            m_IntersectNodes = null;
            m_ExecuteLocked = false;
            m_UsingPolyTree = false;
            m_PolyOuts = new List<OutRec>();
            m_Joins = new List<Join>();
            m_GhostJoins = new List<Join>();
            ReverseSolution = (ioReverseSolution & InitOptions) != 0;
            StrictlySimple = (ioStrictlySimple & InitOptions) != 0;
            PreserveCollinear = (ioPreserveCollinear & InitOptions) != 0;
#if use_xyz
          ZFillFunction = null;
#endif
        }
        //------------------------------------------------------------------------------

        public override void Clear()
        {
            if (m_edges.Count == 0) return; //avoids problems with ClipperBase destructor
            DisposeAllPolyPts();
            base.Clear();
        }
        //------------------------------------------------------------------------------

        void DisposeScanbeamList()
        {
            while (m_Scanbeam != null)
            {
                Scanbeam sb2 = m_Scanbeam.Next;
                m_Scanbeam = null;
                m_Scanbeam = sb2;
            }
        }
        //------------------------------------------------------------------------------

        protected override void Reset()
        {
            base.Reset();
            m_Scanbeam = null;
            m_ActiveEdges = null;
            m_SortedEdges = null;
            DisposeAllPolyPts();
            LocalMinima lm = m_MinimaList;
            while (lm != null)
            {
                InsertScanbeam(lm.Y);
                lm = lm.Next;
            }
        }
        //------------------------------------------------------------------------------

        public bool ReverseSolution { get; set; }
        //------------------------------------------------------------------------------

        public bool StrictlySimple { get; set; }
        //------------------------------------------------------------------------------

        private void InsertScanbeam(Int64 Y)
        {
            if (m_Scanbeam == null)
            {
                m_Scanbeam = new Scanbeam();
                m_Scanbeam.Next = null;
                m_Scanbeam.Y = Y;
            }
            else if (Y > m_Scanbeam.Y)
            {
                Scanbeam newSb = new Scanbeam();
                newSb.Y = Y;
                newSb.Next = m_Scanbeam;
                m_Scanbeam = newSb;
            }
            else
            {
                Scanbeam sb2 = m_Scanbeam;
                while (sb2.Next != null && (Y <= sb2.Next.Y)) sb2 = sb2.Next;
                if (Y == sb2.Y) return; //ie ignores duplicates
                Scanbeam newSb = new Scanbeam();
                newSb.Y = Y;
                newSb.Next = sb2.Next;
                sb2.Next = newSb;
            }
        }
        //------------------------------------------------------------------------------

        public bool Execute(ClipType clipType, List<List<IntPoint>> solution,
            PolyFillType subjFillType, PolyFillType clipFillType)
        {
            if (m_ExecuteLocked) return false;
            if (m_HasOpenPaths)
                throw
                    new ClipperException("Error: PolyTree struct is need for open path clipping.");

            m_ExecuteLocked = true;
            solution.Clear();
            m_SubjFillType = subjFillType;
            m_ClipFillType = clipFillType;
            m_ClipType = clipType;
            m_UsingPolyTree = false;
            bool succeeded = ExecuteInternal();
            //build the return polygons ...
            if (succeeded) BuildResult(solution);
            m_ExecuteLocked = false;
            return succeeded;
        }
        //------------------------------------------------------------------------------

        public bool Execute(ClipType clipType, PolyTree polytree,
            PolyFillType subjFillType, PolyFillType clipFillType)
        {
            if (m_ExecuteLocked) return false;
            m_ExecuteLocked = true;
            m_SubjFillType = subjFillType;
            m_ClipFillType = clipFillType;
            m_ClipType = clipType;
            m_UsingPolyTree = true;
            bool succeeded = ExecuteInternal();
            //build the return polygons ...
            if (succeeded) BuildResult2(polytree);
            m_ExecuteLocked = false;
            return succeeded;
        }
        //------------------------------------------------------------------------------

        public bool Execute(ClipType clipType, List<List<IntPoint>> solution)
        {
            return Execute(clipType, solution,
                PolyFillType.pftEvenOdd, PolyFillType.pftEvenOdd);
        }
        //------------------------------------------------------------------------------

        public bool Execute(ClipType clipType, PolyTree polytree)
        {
            return Execute(clipType, polytree,
                PolyFillType.pftEvenOdd, PolyFillType.pftEvenOdd);
        }
        //------------------------------------------------------------------------------

        internal void FixHoleLinkage(OutRec outRec)
        {
            //skip if an outermost polygon or
            //already already points to the correct FirstLeft ...
            if (outRec.FirstLeft == null ||
                (outRec.IsHole != outRec.FirstLeft.IsHole &&
                 outRec.FirstLeft.Pts != null)) return;

            OutRec orfl = outRec.FirstLeft;
            while (orfl != null && ((orfl.IsHole == outRec.IsHole) || orfl.Pts == null))
                orfl = orfl.FirstLeft;
            outRec.FirstLeft = orfl;
        }
        //------------------------------------------------------------------------------

        private bool ExecuteInternal()
        {
            try
            {
                Reset();
                if (m_CurrentLM == null) return false;

                Int64 botY = PopScanbeam();
                do
                {
                    InsertLocalMinimaIntoAEL(botY);
                    m_GhostJoins.Clear();
                    ProcessHorizontals(false);
                    if (m_Scanbeam == null) break;
                    Int64 topY = PopScanbeam();
                    if (!ProcessIntersections(botY, topY)) return false;
                    ProcessEdgesAtTopOfScanbeam(topY);
                    botY = topY;
                } while (m_Scanbeam != null || m_CurrentLM != null);

                //fix orientations ...
                for (int i = 0; i < m_PolyOuts.Count; i++)
                {
                    OutRec outRec = m_PolyOuts[i];
                    if (outRec.Pts == null || outRec.IsOpen) continue;
                    if ((outRec.IsHole ^ ReverseSolution) == (Area(outRec) > 0))
                        ReversePolyPtLinks(outRec.Pts);
                }

                JoinCommonEdges();

                for (int i = 0; i < m_PolyOuts.Count; i++)
                {
                    OutRec outRec = m_PolyOuts[i];
                    if (outRec.Pts != null && !outRec.IsOpen)
                        FixupOutPolygon(outRec);
                }

                if (StrictlySimple) DoSimplePolygons();
                return true;
            }
            //catch { return false; }
            finally
            {
                m_Joins.Clear();
                m_GhostJoins.Clear();
            }
        }
        //------------------------------------------------------------------------------

        private Int64 PopScanbeam()
        {
            Int64 Y = m_Scanbeam.Y;
            Scanbeam sb2 = m_Scanbeam;
            m_Scanbeam = m_Scanbeam.Next;
            sb2 = null;
            return Y;
        }
        //------------------------------------------------------------------------------

        private void DisposeAllPolyPts()
        {
            for (int i = 0; i < m_PolyOuts.Count; ++i) DisposeOutRec(i);
            m_PolyOuts.Clear();
        }
        //------------------------------------------------------------------------------

        void DisposeOutRec(int index)
        {
            OutRec outRec = m_PolyOuts[index];
            if (outRec.Pts != null) DisposeOutPts(outRec.Pts);
            outRec = null;
            m_PolyOuts[index] = null;
        }
        //------------------------------------------------------------------------------

        private void DisposeOutPts(OutPt pp)
        {
            if (pp == null) return;
            OutPt tmpPp = null;
            pp.Prev.Next = null;
            while (pp != null)
            {
                tmpPp = pp;
                pp = pp.Next;
                tmpPp = null;
            }
        }
        //------------------------------------------------------------------------------

        private void AddJoin(OutPt Op1, OutPt Op2, IntPoint OffPt)
        {
            Join j = new Join();
            j.OutPt1 = Op1;
            j.OutPt2 = Op2;
            j.OffPt = OffPt;
            m_Joins.Add(j);
        }
        //------------------------------------------------------------------------------

        private void AddGhostJoin(OutPt Op, IntPoint OffPt)
        {
            Join j = new Join();
            j.OutPt1 = Op;
            j.OffPt = OffPt;
            m_GhostJoins.Add(j);
        }
        //------------------------------------------------------------------------------

#if use_xyz
      internal void SetZ(ref IntPoint pt, TEdge e)
      {
        pt.Z = 0;
        if (ZFillFunction != null)
        {
          //put the 'preferred' point as first parameter ...
          if (e.OutIdx < 0)
            ZFillFunction(e.Bot, e.Top, ref pt); //outside a path so presume entering
          else
            ZFillFunction(e.Top, e.Bot, ref pt); //inside a path so presume exiting
        }
      }
      //------------------------------------------------------------------------------
#endif

        private void InsertLocalMinimaIntoAEL(Int64 botY)
        {
            while (m_CurrentLM != null && (m_CurrentLM.Y == botY))
            {
                TEdge lb = m_CurrentLM.LeftBound;
                TEdge rb = m_CurrentLM.RightBound;
                PopLocalMinima();

                OutPt Op1 = null;
                if (lb == null)
                {
                    InsertEdgeIntoAEL(rb, null);
                    SetWindingCount(rb);
                    if (IsContributing(rb))
                        Op1 = AddOutPt(rb, rb.Bot);
                }
                else
                {
                    InsertEdgeIntoAEL(lb, null);
                    InsertEdgeIntoAEL(rb, lb);
                    SetWindingCount(lb);
                    rb.WindCnt = lb.WindCnt;
                    rb.WindCnt2 = lb.WindCnt2;
                    if (IsContributing(lb))
                        Op1 = AddLocalMinPoly(lb, rb, lb.Bot);

                    InsertScanbeam(lb.Top.Y);
                }

                if (IsHorizontal(rb))
                    AddEdgeToSEL(rb);
                else
                    InsertScanbeam(rb.Top.Y);

                if (lb == null) continue;

                //if output polygons share an Edge with a horizontal rb, they'll need joining later ...
                if (Op1 != null && IsHorizontal(rb) &&
                    m_GhostJoins.Count > 0 && rb.WindDelta != 0)
                {
                    for (int i = 0; i < m_GhostJoins.Count; i++)
                    {
                        //if the horizontal Rb and a 'ghost' horizontal overlap, then convert
                        //the 'ghost' join to a real join ready for later ...
                        Join j = m_GhostJoins[i];
                        if (HorzSegmentsOverlap(j.OutPt1.Pt, j.OffPt, rb.Bot, rb.Top))
                            AddJoin(j.OutPt1, Op1, j.OffPt);
                    }
                }

                if (lb.OutIdx >= 0 && lb.PrevInAEL != null &&
                    lb.PrevInAEL.Curr.X == lb.Bot.X &&
                    lb.PrevInAEL.OutIdx >= 0 &&
                    SlopesEqual(lb.PrevInAEL, lb, m_UseFullRange) &&
                    lb.WindDelta != 0 && lb.PrevInAEL.WindDelta != 0)
                {
                    OutPt Op2 = AddOutPt(lb.PrevInAEL, lb.Bot);
                    AddJoin(Op1, Op2, lb.Top);
                }

                if (lb.NextInAEL != rb)
                {
                    if (rb.OutIdx >= 0 && rb.PrevInAEL.OutIdx >= 0 &&
                        SlopesEqual(rb.PrevInAEL, rb, m_UseFullRange) &&
                        rb.WindDelta != 0 && rb.PrevInAEL.WindDelta != 0)
                    {
                        OutPt Op2 = AddOutPt(rb.PrevInAEL, rb.Bot);
                        AddJoin(Op1, Op2, rb.Top);
                    }

                    TEdge e = lb.NextInAEL;
                    if (e != null)
                        while (e != rb)
                        {
                            //nb: For calculating winding counts etc, IntersectEdges() assumes
                            //that param1 will be to the right of param2 ABOVE the intersection ...
                            IntersectEdges(rb, e, lb.Curr); //order important here
                            e = e.NextInAEL;
                        }
                }
            }
        }
        //------------------------------------------------------------------------------

        private void InsertEdgeIntoAEL(TEdge edge, TEdge startEdge)
        {
            if (m_ActiveEdges == null)
            {
                edge.PrevInAEL = null;
                edge.NextInAEL = null;
                m_ActiveEdges = edge;
            }
            else if (startEdge == null && E2InsertsBeforeE1(m_ActiveEdges, edge))
            {
                edge.PrevInAEL = null;
                edge.NextInAEL = m_ActiveEdges;
                m_ActiveEdges.PrevInAEL = edge;
                m_ActiveEdges = edge;
            }
            else
            {
                if (startEdge == null) startEdge = m_ActiveEdges;
                while (startEdge.NextInAEL != null &&
                       !E2InsertsBeforeE1(startEdge.NextInAEL, edge))
                    startEdge = startEdge.NextInAEL;
                edge.NextInAEL = startEdge.NextInAEL;
                if (startEdge.NextInAEL != null) startEdge.NextInAEL.PrevInAEL = edge;
                edge.PrevInAEL = startEdge;
                startEdge.NextInAEL = edge;
            }
        }
        //----------------------------------------------------------------------

        private bool E2InsertsBeforeE1(TEdge e1, TEdge e2)
        {
            if (e2.Curr.X == e1.Curr.X)
            {
                if (e2.Top.Y > e1.Top.Y)
                    return e2.Top.X < TopX(e1, e2.Top.Y);
                else return e1.Top.X > TopX(e2, e1.Top.Y);
            }
            else return e2.Curr.X < e1.Curr.X;
        }
        //------------------------------------------------------------------------------

        private bool IsEvenOddFillType(TEdge edge)
        {
            if (edge.PolyTyp == PolyType.ptSubject)
                return m_SubjFillType == PolyFillType.pftEvenOdd;
            else
                return m_ClipFillType == PolyFillType.pftEvenOdd;
        }
        //------------------------------------------------------------------------------

        private bool IsEvenOddAltFillType(TEdge edge)
        {
            if (edge.PolyTyp == PolyType.ptSubject)
                return m_ClipFillType == PolyFillType.pftEvenOdd;
            else
                return m_SubjFillType == PolyFillType.pftEvenOdd;
        }
        //------------------------------------------------------------------------------

        private bool IsContributing(TEdge edge)
        {
            PolyFillType pft, pft2;
            if (edge.PolyTyp == PolyType.ptSubject)
            {
                pft = m_SubjFillType;
                pft2 = m_ClipFillType;
            }
            else
            {
                pft = m_ClipFillType;
                pft2 = m_SubjFillType;
            }

            switch (pft)
            {
                case PolyFillType.pftEvenOdd:
                    //return false if a subj line has been flagged as inside a subj polygon
                    if (edge.WindDelta == 0 && edge.WindCnt != 1) return false;
                    break;
                case PolyFillType.pftNonZero:
                    if (Math.Abs(edge.WindCnt) != 1) return false;
                    break;
                case PolyFillType.pftPositive:
                    if (edge.WindCnt != 1) return false;
                    break;
                default: //PolyFillType.pftNegative
                    if (edge.WindCnt != -1) return false;
                    break;
            }

            switch (m_ClipType)
            {
                case ClipType.ctIntersection:
                    switch (pft2)
                    {
                        case PolyFillType.pftEvenOdd:
                        case PolyFillType.pftNonZero:
                            return (edge.WindCnt2 != 0);
                        case PolyFillType.pftPositive:
                            return (edge.WindCnt2 > 0);
                        default:
                            return (edge.WindCnt2 < 0);
                    }
                case ClipType.ctUnion:
                    switch (pft2)
                    {
                        case PolyFillType.pftEvenOdd:
                        case PolyFillType.pftNonZero:
                            return (edge.WindCnt2 == 0);
                        case PolyFillType.pftPositive:
                            return (edge.WindCnt2 <= 0);
                        default:
                            return (edge.WindCnt2 >= 0);
                    }
                case ClipType.ctDifference:
                    if (edge.PolyTyp == PolyType.ptSubject)
                        switch (pft2)
                        {
                            case PolyFillType.pftEvenOdd:
                            case PolyFillType.pftNonZero:
                                return (edge.WindCnt2 == 0);
                            case PolyFillType.pftPositive:
                                return (edge.WindCnt2 <= 0);
                            default:
                                return (edge.WindCnt2 >= 0);
                        }
                    else
                        switch (pft2)
                        {
                            case PolyFillType.pftEvenOdd:
                            case PolyFillType.pftNonZero:
                                return (edge.WindCnt2 != 0);
                            case PolyFillType.pftPositive:
                                return (edge.WindCnt2 > 0);
                            default:
                                return (edge.WindCnt2 < 0);
                        }
                case ClipType.ctXor:
                    if (edge.WindDelta == 0) //XOr always contributing unless open
                        switch (pft2)
                        {
                            case PolyFillType.pftEvenOdd:
                            case PolyFillType.pftNonZero:
                                return (edge.WindCnt2 == 0);
                            case PolyFillType.pftPositive:
                                return (edge.WindCnt2 <= 0);
                            default:
                                return (edge.WindCnt2 >= 0);
                        }
                    else
                        return true;
            }

            return true;
        }
        //------------------------------------------------------------------------------

        private void SetWindingCount(TEdge edge)
        {
            TEdge e = edge.PrevInAEL;
            //find the edge of the same polytype that immediately preceeds 'edge' in AEL
            while (e != null && ((e.PolyTyp != edge.PolyTyp) || (e.WindDelta == 0))) e = e.PrevInAEL;
            if (e == null)
            {
                edge.WindCnt = (edge.WindDelta == 0 ? 1 : edge.WindDelta);
                edge.WindCnt2 = 0;
                e = m_ActiveEdges; //ie get ready to calc WindCnt2
            }
            else if (edge.WindDelta == 0 && m_ClipType != ClipType.ctUnion)
            {
                edge.WindCnt = 1;
                edge.WindCnt2 = e.WindCnt2;
                e = e.NextInAEL; //ie get ready to calc WindCnt2
            }
            else if (IsEvenOddFillType(edge))
            {
                //EvenOdd filling ...
                if (edge.WindDelta == 0)
                {
                    //are we inside a subj polygon ...
                    bool Inside = true;
                    TEdge e2 = e.PrevInAEL;
                    while (e2 != null)
                    {
                        if (e2.PolyTyp == e.PolyTyp && e2.WindDelta != 0)
                            Inside = !Inside;
                        e2 = e2.PrevInAEL;
                    }

                    edge.WindCnt = (Inside ? 0 : 1);
                }
                else
                {
                    edge.WindCnt = edge.WindDelta;
                }

                edge.WindCnt2 = e.WindCnt2;
                e = e.NextInAEL; //ie get ready to calc WindCnt2
            }
            else
            {
                //nonZero, Positive or Negative filling ...
                if (e.WindCnt * e.WindDelta < 0)
                {
                    //prev edge is 'decreasing' WindCount (WC) toward zero
                    //so we're outside the previous polygon ...
                    if (Math.Abs(e.WindCnt) > 1)
                    {
                        //outside prev poly but still inside another.
                        //when reversing direction of prev poly use the same WC 
                        if (e.WindDelta * edge.WindDelta < 0) edge.WindCnt = e.WindCnt;
                        //otherwise continue to 'decrease' WC ...
                        else edge.WindCnt = e.WindCnt + edge.WindDelta;
                    }
                    else
                        //now outside all polys of same polytype so set own WC ...
                        edge.WindCnt = (edge.WindDelta == 0 ? 1 : edge.WindDelta);
                }
                else
                {
                    //prev edge is 'increasing' WindCount (WC) away from zero
                    //so we're inside the previous polygon ...
                    if (edge.WindDelta == 0)
                        edge.WindCnt = (e.WindCnt < 0 ? e.WindCnt - 1 : e.WindCnt + 1);
                    //if wind direction is reversing prev then use same WC
                    else if (e.WindDelta * edge.WindDelta < 0)
                        edge.WindCnt = e.WindCnt;
                    //otherwise add to WC ...
                    else edge.WindCnt = e.WindCnt + edge.WindDelta;
                }

                edge.WindCnt2 = e.WindCnt2;
                e = e.NextInAEL; //ie get ready to calc WindCnt2
            }

            //update WindCnt2 ...
            if (IsEvenOddAltFillType(edge))
            {
                //EvenOdd filling ...
                while (e != edge)
                {
                    if (e.WindDelta != 0)
                        edge.WindCnt2 = (edge.WindCnt2 == 0 ? 1 : 0);
                    e = e.NextInAEL;
                }
            }
            else
            {
                //nonZero, Positive or Negative filling ...
                while (e != edge)
                {
                    edge.WindCnt2 += e.WindDelta;
                    e = e.NextInAEL;
                }
            }
        }
        //------------------------------------------------------------------------------

        private void AddEdgeToSEL(TEdge edge)
        {
            //SEL pointers in PEdge are reused to build a list of horizontal edges.
            //However, we don't need to worry about order with horizontal edge processing.
            if (m_SortedEdges == null)
            {
                m_SortedEdges = edge;
                edge.PrevInSEL = null;
                edge.NextInSEL = null;
            }
            else
            {
                edge.NextInSEL = m_SortedEdges;
                edge.PrevInSEL = null;
                m_SortedEdges.PrevInSEL = edge;
                m_SortedEdges = edge;
            }
        }
        //------------------------------------------------------------------------------

        private void CopyAELToSEL()
        {
            TEdge e = m_ActiveEdges;
            m_SortedEdges = e;
            while (e != null)
            {
                e.PrevInSEL = e.PrevInAEL;
                e.NextInSEL = e.NextInAEL;
                e = e.NextInAEL;
            }
        }
        //------------------------------------------------------------------------------

        private void SwapPositionsInAEL(TEdge edge1, TEdge edge2)
        {
            //check that one or other edge hasn't already been removed from AEL ...
            if (edge1.NextInAEL == edge1.PrevInAEL ||
                edge2.NextInAEL == edge2.PrevInAEL) return;

            if (edge1.NextInAEL == edge2)
            {
                TEdge next = edge2.NextInAEL;
                if (next != null)
                    next.PrevInAEL = edge1;
                TEdge prev = edge1.PrevInAEL;
                if (prev != null)
                    prev.NextInAEL = edge2;
                edge2.PrevInAEL = prev;
                edge2.NextInAEL = edge1;
                edge1.PrevInAEL = edge2;
                edge1.NextInAEL = next;
            }
            else if (edge2.NextInAEL == edge1)
            {
                TEdge next = edge1.NextInAEL;
                if (next != null)
                    next.PrevInAEL = edge2;
                TEdge prev = edge2.PrevInAEL;
                if (prev != null)
                    prev.NextInAEL = edge1;
                edge1.PrevInAEL = prev;
                edge1.NextInAEL = edge2;
                edge2.PrevInAEL = edge1;
                edge2.NextInAEL = next;
            }
            else
            {
                TEdge next = edge1.NextInAEL;
                TEdge prev = edge1.PrevInAEL;
                edge1.NextInAEL = edge2.NextInAEL;
                if (edge1.NextInAEL != null)
                    edge1.NextInAEL.PrevInAEL = edge1;
                edge1.PrevInAEL = edge2.PrevInAEL;
                if (edge1.PrevInAEL != null)
                    edge1.PrevInAEL.NextInAEL = edge1;
                edge2.NextInAEL = next;
                if (edge2.NextInAEL != null)
                    edge2.NextInAEL.PrevInAEL = edge2;
                edge2.PrevInAEL = prev;
                if (edge2.PrevInAEL != null)
                    edge2.PrevInAEL.NextInAEL = edge2;
            }

            if (edge1.PrevInAEL == null)
                m_ActiveEdges = edge1;
            else if (edge2.PrevInAEL == null)
                m_ActiveEdges = edge2;
        }
        //------------------------------------------------------------------------------

        private void SwapPositionsInSEL(TEdge edge1, TEdge edge2)
        {
            if (edge1.NextInSEL == null && edge1.PrevInSEL == null)
                return;
            if (edge2.NextInSEL == null && edge2.PrevInSEL == null)
                return;

            if (edge1.NextInSEL == edge2)
            {
                TEdge next = edge2.NextInSEL;
                if (next != null)
                    next.PrevInSEL = edge1;
                TEdge prev = edge1.PrevInSEL;
                if (prev != null)
                    prev.NextInSEL = edge2;
                edge2.PrevInSEL = prev;
                edge2.NextInSEL = edge1;
                edge1.PrevInSEL = edge2;
                edge1.NextInSEL = next;
            }
            else if (edge2.NextInSEL == edge1)
            {
                TEdge next = edge1.NextInSEL;
                if (next != null)
                    next.PrevInSEL = edge2;
                TEdge prev = edge2.PrevInSEL;
                if (prev != null)
                    prev.NextInSEL = edge1;
                edge1.PrevInSEL = prev;
                edge1.NextInSEL = edge2;
                edge2.PrevInSEL = edge1;
                edge2.NextInSEL = next;
            }
            else
            {
                TEdge next = edge1.NextInSEL;
                TEdge prev = edge1.PrevInSEL;
                edge1.NextInSEL = edge2.NextInSEL;
                if (edge1.NextInSEL != null)
                    edge1.NextInSEL.PrevInSEL = edge1;
                edge1.PrevInSEL = edge2.PrevInSEL;
                if (edge1.PrevInSEL != null)
                    edge1.PrevInSEL.NextInSEL = edge1;
                edge2.NextInSEL = next;
                if (edge2.NextInSEL != null)
                    edge2.NextInSEL.PrevInSEL = edge2;
                edge2.PrevInSEL = prev;
                if (edge2.PrevInSEL != null)
                    edge2.PrevInSEL.NextInSEL = edge2;
            }

            if (edge1.PrevInSEL == null)
                m_SortedEdges = edge1;
            else if (edge2.PrevInSEL == null)
                m_SortedEdges = edge2;
        }
        //------------------------------------------------------------------------------


        private void AddLocalMaxPoly(TEdge e1, TEdge e2, IntPoint pt)
        {
            AddOutPt(e1, pt);
            if (e1.OutIdx == e2.OutIdx)
            {
                e1.OutIdx = Unassigned;
                e2.OutIdx = Unassigned;
            }
            else if (e1.OutIdx < e2.OutIdx)
                AppendPolygon(e1, e2);
            else
                AppendPolygon(e2, e1);
        }
        //------------------------------------------------------------------------------

        private OutPt AddLocalMinPoly(TEdge e1, TEdge e2, IntPoint pt)
        {
            OutPt result;
            TEdge e, prevE;
            if (IsHorizontal(e2) || (e1.Dx > e2.Dx))
            {
                result = AddOutPt(e1, pt);
                e2.OutIdx = e1.OutIdx;
                e1.Side = EdgeSide.esLeft;
                e2.Side = EdgeSide.esRight;
                e = e1;
                if (e.PrevInAEL == e2)
                    prevE = e2.PrevInAEL;
                else
                    prevE = e.PrevInAEL;
            }
            else
            {
                result = AddOutPt(e2, pt);
                e1.OutIdx = e2.OutIdx;
                e1.Side = EdgeSide.esRight;
                e2.Side = EdgeSide.esLeft;
                e = e2;
                if (e.PrevInAEL == e1)
                    prevE = e1.PrevInAEL;
                else
                    prevE = e.PrevInAEL;
            }

            if (prevE != null && prevE.OutIdx >= 0 &&
                (TopX(prevE, pt.Y) == TopX(e, pt.Y)) &&
                SlopesEqual(e, prevE, m_UseFullRange) &&
                (e.WindDelta != 0) && (prevE.WindDelta != 0))
            {
                OutPt outPt = AddOutPt(prevE, pt);
                AddJoin(result, outPt, e.Top);
            }

            return result;
        }
        //------------------------------------------------------------------------------

        private OutRec CreateOutRec()
        {
            OutRec result = new OutRec();
            result.Idx = Unassigned;
            result.IsHole = false;
            result.IsOpen = false;
            result.FirstLeft = null;
            result.Pts = null;
            result.BottomPt = null;
            result.PolyNode = null;
            m_PolyOuts.Add(result);
            result.Idx = m_PolyOuts.Count - 1;
            return result;
        }
        //------------------------------------------------------------------------------

        private OutPt AddOutPt(TEdge e, IntPoint pt)
        {
            bool ToFront = (e.Side == EdgeSide.esLeft);
            if (e.OutIdx < 0)
            {
                OutRec outRec = CreateOutRec();
                outRec.IsOpen = (e.WindDelta == 0);
                OutPt newOp = new OutPt();
                outRec.Pts = newOp;
                newOp.Idx = outRec.Idx;
                newOp.Pt = pt;
                newOp.Next = newOp;
                newOp.Prev = newOp;
                if (!outRec.IsOpen)
                    SetHoleState(e, outRec);
#if use_xyz
          if (pt == e.Bot)
            newOp.Pt = e.Bot;
          else if (pt == e.Top)
            newOp.Pt = e.Top;
          else
            SetZ(ref newOp.Pt, e);
#endif
                e.OutIdx = outRec.Idx; //nb: do this after SetZ !
                return newOp;
            }
            else
            {
                OutRec outRec = m_PolyOuts[e.OutIdx];
                //OutRec.Pts is the 'Left-most' point & OutRec.Pts.Prev is the 'Right-most'
                OutPt op = outRec.Pts;
                if (ToFront && pt == op.Pt) return op;
                else if (!ToFront && pt == op.Prev.Pt) return op.Prev;

                OutPt newOp = new OutPt();
                newOp.Idx = outRec.Idx;
                newOp.Pt = pt;
                newOp.Next = op;
                newOp.Prev = op.Prev;
                newOp.Prev.Next = newOp;
                op.Prev = newOp;
                if (ToFront) outRec.Pts = newOp;
#if use_xyz
          if (pt == e.Bot)
            newOp.Pt = e.Bot;
          else if (pt == e.Top)
            newOp.Pt = e.Top;
          else
            SetZ(ref newOp.Pt, e);
#endif
                return newOp;
            }
        }
        //------------------------------------------------------------------------------

        internal void SwapPoints(ref IntPoint pt1, ref IntPoint pt2)
        {
            IntPoint tmp = new IntPoint(pt1);
            pt1 = pt2;
            pt2 = tmp;
        }
        //------------------------------------------------------------------------------

        private bool HorzSegmentsOverlap(
            IntPoint Pt1a, IntPoint Pt1b, IntPoint Pt2a, IntPoint Pt2b)
        {
            //precondition: both segments are horizontal
            if ((Pt1a.X > Pt2a.X) == (Pt1a.X < Pt2b.X)) return true;
            else if ((Pt1b.X > Pt2a.X) == (Pt1b.X < Pt2b.X)) return true;
            else if ((Pt2a.X > Pt1a.X) == (Pt2a.X < Pt1b.X)) return true;
            else if ((Pt2b.X > Pt1a.X) == (Pt2b.X < Pt1b.X)) return true;
            else if ((Pt1a.X == Pt2a.X) && (Pt1b.X == Pt2b.X)) return true;
            else if ((Pt1a.X == Pt2b.X) && (Pt1b.X == Pt2a.X)) return true;
            else return false;
        }
        //------------------------------------------------------------------------------

        private OutPt InsertPolyPtBetween(OutPt p1, OutPt p2, IntPoint pt)
        {
            OutPt result = new OutPt();
            result.Pt = pt;
            if (p2 == p1.Next)
            {
                p1.Next = result;
                p2.Prev = result;
                result.Next = p2;
                result.Prev = p1;
            }
            else
            {
                p2.Next = result;
                p1.Prev = result;
                result.Next = p1;
                result.Prev = p2;
            }

            return result;
        }
        //------------------------------------------------------------------------------

        private void SetHoleState(TEdge e, OutRec outRec)
        {
            bool isHole = false;
            TEdge e2 = e.PrevInAEL;
            while (e2 != null)
            {
                if (e2.OutIdx >= 0)
                {
                    isHole = !isHole;
                    if (outRec.FirstLeft == null)
                        outRec.FirstLeft = m_PolyOuts[e2.OutIdx];
                }

                e2 = e2.PrevInAEL;
            }

            if (isHole) outRec.IsHole = true;
        }
        //------------------------------------------------------------------------------

        private double GetDx(IntPoint pt1, IntPoint pt2)
        {
            if (pt1.Y == pt2.Y) return horizontal;
            else return (double) (pt2.X - pt1.X) / (pt2.Y - pt1.Y);
        }
        //---------------------------------------------------------------------------

        private bool FirstIsBottomPt(OutPt btmPt1, OutPt btmPt2)
        {
            OutPt p = btmPt1.Prev;
            while ((p.Pt == btmPt1.Pt) && (p != btmPt1)) p = p.Prev;
            double dx1p = Math.Abs(GetDx(btmPt1.Pt, p.Pt));
            p = btmPt1.Next;
            while ((p.Pt == btmPt1.Pt) && (p != btmPt1)) p = p.Next;
            double dx1n = Math.Abs(GetDx(btmPt1.Pt, p.Pt));

            p = btmPt2.Prev;
            while ((p.Pt == btmPt2.Pt) && (p != btmPt2)) p = p.Prev;
            double dx2p = Math.Abs(GetDx(btmPt2.Pt, p.Pt));
            p = btmPt2.Next;
            while ((p.Pt == btmPt2.Pt) && (p != btmPt2)) p = p.Next;
            double dx2n = Math.Abs(GetDx(btmPt2.Pt, p.Pt));
            return (dx1p >= dx2p && dx1p >= dx2n) || (dx1n >= dx2p && dx1n >= dx2n);
        }
        //------------------------------------------------------------------------------

        private OutPt GetBottomPt(OutPt pp)
        {
            OutPt dups = null;
            OutPt p = pp.Next;
            while (p != pp)
            {
                if (p.Pt.Y > pp.Pt.Y)
                {
                    pp = p;
                    dups = null;
                }
                else if (p.Pt.Y == pp.Pt.Y && p.Pt.X <= pp.Pt.X)
                {
                    if (p.Pt.X < pp.Pt.X)
                    {
                        dups = null;
                        pp = p;
                    }
                    else
                    {
                        if (p.Next != pp && p.Prev != pp) dups = p;
                    }
                }

                p = p.Next;
            }

            if (dups != null)
            {
                //there appears to be at least 2 vertices at bottomPt so ...
                while (dups != p)
                {
                    if (!FirstIsBottomPt(p, dups)) pp = dups;
                    dups = dups.Next;
                    while (dups.Pt != pp.Pt) dups = dups.Next;
                }
            }

            return pp;
        }
        //------------------------------------------------------------------------------

        private OutRec GetLowermostRec(OutRec outRec1, OutRec outRec2)
        {
            //work out which polygon fragment has the correct hole state ...
            if (outRec1.BottomPt == null)
                outRec1.BottomPt = GetBottomPt(outRec1.Pts);
            if (outRec2.BottomPt == null)
                outRec2.BottomPt = GetBottomPt(outRec2.Pts);
            OutPt bPt1 = outRec1.BottomPt;
            OutPt bPt2 = outRec2.BottomPt;
            if (bPt1.Pt.Y > bPt2.Pt.Y) return outRec1;
            else if (bPt1.Pt.Y < bPt2.Pt.Y) return outRec2;
            else if (bPt1.Pt.X < bPt2.Pt.X) return outRec1;
            else if (bPt1.Pt.X > bPt2.Pt.X) return outRec2;
            else if (bPt1.Next == bPt1) return outRec2;
            else if (bPt2.Next == bPt2) return outRec1;
            else if (FirstIsBottomPt(bPt1, bPt2)) return outRec1;
            else return outRec2;
        }
        //------------------------------------------------------------------------------

        bool Param1RightOfParam2(OutRec outRec1, OutRec outRec2)
        {
            do
            {
                outRec1 = outRec1.FirstLeft;
                if (outRec1 == outRec2) return true;
            } while (outRec1 != null);

            return false;
        }
        //------------------------------------------------------------------------------

        private OutRec GetOutRec(int idx)
        {
            OutRec outrec = m_PolyOuts[idx];
            while (outrec != m_PolyOuts[outrec.Idx])
                outrec = m_PolyOuts[outrec.Idx];
            return outrec;
        }
        //------------------------------------------------------------------------------

        private void AppendPolygon(TEdge e1, TEdge e2)
        {
            //get the start and ends of both output polygons ...
            OutRec outRec1 = m_PolyOuts[e1.OutIdx];
            OutRec outRec2 = m_PolyOuts[e2.OutIdx];

            OutRec holeStateRec;
            if (Param1RightOfParam2(outRec1, outRec2))
                holeStateRec = outRec2;
            else if (Param1RightOfParam2(outRec2, outRec1))
                holeStateRec = outRec1;
            else
                holeStateRec = GetLowermostRec(outRec1, outRec2);

            OutPt p1_lft = outRec1.Pts;
            OutPt p1_rt = p1_lft.Prev;
            OutPt p2_lft = outRec2.Pts;
            OutPt p2_rt = p2_lft.Prev;

            EdgeSide side;
            //join e2 poly onto e1 poly and delete pointers to e2 ...
            if (e1.Side == EdgeSide.esLeft)
            {
                if (e2.Side == EdgeSide.esLeft)
                {
                    //z y x a b c
                    ReversePolyPtLinks(p2_lft);
                    p2_lft.Next = p1_lft;
                    p1_lft.Prev = p2_lft;
                    p1_rt.Next = p2_rt;
                    p2_rt.Prev = p1_rt;
                    outRec1.Pts = p2_rt;
                }
                else
                {
                    //x y z a b c
                    p2_rt.Next = p1_lft;
                    p1_lft.Prev = p2_rt;
                    p2_lft.Prev = p1_rt;
                    p1_rt.Next = p2_lft;
                    outRec1.Pts = p2_lft;
                }

                side = EdgeSide.esLeft;
            }
            else
            {
                if (e2.Side == EdgeSide.esRight)
                {
                    //a b c z y x
                    ReversePolyPtLinks(p2_lft);
                    p1_rt.Next = p2_rt;
                    p2_rt.Prev = p1_rt;
                    p2_lft.Next = p1_lft;
                    p1_lft.Prev = p2_lft;
                }
                else
                {
                    //a b c x y z
                    p1_rt.Next = p2_lft;
                    p2_lft.Prev = p1_rt;
                    p1_lft.Prev = p2_rt;
                    p2_rt.Next = p1_lft;
                }

                side = EdgeSide.esRight;
            }

            outRec1.BottomPt = null;
            if (holeStateRec == outRec2)
            {
                if (outRec2.FirstLeft != outRec1)
                    outRec1.FirstLeft = outRec2.FirstLeft;
                outRec1.IsHole = outRec2.IsHole;
            }

            outRec2.Pts = null;
            outRec2.BottomPt = null;

            outRec2.FirstLeft = outRec1;

            int OKIdx = e1.OutIdx;
            int ObsoleteIdx = e2.OutIdx;

            e1.OutIdx = Unassigned; //nb: safe because we only get here via AddLocalMaxPoly
            e2.OutIdx = Unassigned;

            TEdge e = m_ActiveEdges;
            while (e != null)
            {
                if (e.OutIdx == ObsoleteIdx)
                {
                    e.OutIdx = OKIdx;
                    e.Side = side;
                    break;
                }

                e = e.NextInAEL;
            }

            outRec2.Idx = outRec1.Idx;
        }
        //------------------------------------------------------------------------------

        private void ReversePolyPtLinks(OutPt pp)
        {
            if (pp == null) return;
            OutPt pp1;
            OutPt pp2;
            pp1 = pp;
            do
            {
                pp2 = pp1.Next;
                pp1.Next = pp1.Prev;
                pp1.Prev = pp2;
                pp1 = pp2;
            } while (pp1 != pp);
        }
        //------------------------------------------------------------------------------

        private static void SwapSides(TEdge edge1, TEdge edge2)
        {
            EdgeSide side = edge1.Side;
            edge1.Side = edge2.Side;
            edge2.Side = side;
        }
        //------------------------------------------------------------------------------

        private static void SwapPolyIndexes(TEdge edge1, TEdge edge2)
        {
            int outIdx = edge1.OutIdx;
            edge1.OutIdx = edge2.OutIdx;
            edge2.OutIdx = outIdx;
        }
        //------------------------------------------------------------------------------

        private void IntersectEdges(TEdge e1, TEdge e2, IntPoint pt, bool protect = false)
        {
            //e1 will be to the left of e2 BELOW the intersection. Therefore e1 is before
            //e2 in AEL except when e1 is being inserted at the intersection point ...

            bool e1stops = !protect && e1.NextInLML == null &&
                           e1.Top.X == pt.X && e1.Top.Y == pt.Y;
            bool e2stops = !protect && e2.NextInLML == null &&
                           e2.Top.X == pt.X && e2.Top.Y == pt.Y;
            bool e1Contributing = (e1.OutIdx >= 0);
            bool e2Contributing = (e2.OutIdx >= 0);

#if use_lines
          //if either edge is on an OPEN path ...
          if (e1.WindDelta == 0 || e2.WindDelta == 0)
          {
            //ignore subject-subject open path intersections UNLESS they
            //are both open paths, AND they are both 'contributing maximas' ...
            if (e1.WindDelta == 0 && e2.WindDelta == 0)
            {
              if ((e1stops || e2stops) && e1Contributing && e2Contributing)
                AddLocalMaxPoly(e1, e2, pt);
            }
            //if intersecting a subj line with a subj poly ...
            else if (e1.PolyTyp == e2.PolyTyp && 
              e1.WindDelta != e2.WindDelta && m_ClipType == ClipType.ctUnion)
            {
              if (e1.WindDelta == 0)
              {
                if (e2Contributing)
                {
                  AddOutPt(e1, pt);
                  if (e1Contributing) e1.OutIdx = Unassigned;
                }
              }
              else
              {
                if (e1Contributing)
                {
                  AddOutPt(e2, pt);
                  if (e2Contributing) e2.OutIdx = Unassigned;
                }
              }
            }
            else if (e1.PolyTyp != e2.PolyTyp)
            {
              if ((e1.WindDelta == 0) && Math.Abs(e2.WindCnt) == 1 && 
                (m_ClipType != ClipType.ctUnion || e2.WindCnt2 == 0))
              {
                AddOutPt(e1, pt);
                if (e1Contributing) e1.OutIdx = Unassigned;
              }
              else if ((e2.WindDelta == 0) && (Math.Abs(e1.WindCnt) == 1) && 
                (m_ClipType != ClipType.ctUnion || e1.WindCnt2 == 0))
              {
                AddOutPt(e2, pt);
                if (e2Contributing) e2.OutIdx = Unassigned;
              }
            }

            if (e1stops)
              if (e1.OutIdx < 0) DeleteFromAEL(e1);
              else throw new ClipperException("Error intersecting polylines");
            if (e2stops) 
              if (e2.OutIdx < 0) DeleteFromAEL(e2);
              else throw new ClipperException("Error intersecting polylines");
            return;
          }
#endif

            //update winding counts...
            //assumes that e1 will be to the Right of e2 ABOVE the intersection
            if (e1.PolyTyp == e2.PolyTyp)
            {
                if (IsEvenOddFillType(e1))
                {
                    int oldE1WindCnt = e1.WindCnt;
                    e1.WindCnt = e2.WindCnt;
                    e2.WindCnt = oldE1WindCnt;
                }
                else
                {
                    if (e1.WindCnt + e2.WindDelta == 0) e1.WindCnt = -e1.WindCnt;
                    else e1.WindCnt += e2.WindDelta;
                    if (e2.WindCnt - e1.WindDelta == 0) e2.WindCnt = -e2.WindCnt;
                    else e2.WindCnt -= e1.WindDelta;
                }
            }
            else
            {
                if (!IsEvenOddFillType(e2)) e1.WindCnt2 += e2.WindDelta;
                else e1.WindCnt2 = (e1.WindCnt2 == 0) ? 1 : 0;
                if (!IsEvenOddFillType(e1)) e2.WindCnt2 -= e1.WindDelta;
                else e2.WindCnt2 = (e2.WindCnt2 == 0) ? 1 : 0;
            }

            PolyFillType e1FillType, e2FillType, e1FillType2, e2FillType2;
            if (e1.PolyTyp == PolyType.ptSubject)
            {
                e1FillType = m_SubjFillType;
                e1FillType2 = m_ClipFillType;
            }
            else
            {
                e1FillType = m_ClipFillType;
                e1FillType2 = m_SubjFillType;
            }

            if (e2.PolyTyp == PolyType.ptSubject)
            {
                e2FillType = m_SubjFillType;
                e2FillType2 = m_ClipFillType;
            }
            else
            {
                e2FillType = m_ClipFillType;
                e2FillType2 = m_SubjFillType;
            }

            int e1Wc, e2Wc;
            switch (e1FillType)
            {
                case PolyFillType.pftPositive:
                    e1Wc = e1.WindCnt;
                    break;
                case PolyFillType.pftNegative:
                    e1Wc = -e1.WindCnt;
                    break;
                default:
                    e1Wc = Math.Abs(e1.WindCnt);
                    break;
            }

            switch (e2FillType)
            {
                case PolyFillType.pftPositive:
                    e2Wc = e2.WindCnt;
                    break;
                case PolyFillType.pftNegative:
                    e2Wc = -e2.WindCnt;
                    break;
                default:
                    e2Wc = Math.Abs(e2.WindCnt);
                    break;
            }

            if (e1Contributing && e2Contributing)
            {
                if (e1stops || e2stops ||
                    (e1Wc != 0 && e1Wc != 1) || (e2Wc != 0 && e2Wc != 1) ||
                    (e1.PolyTyp != e2.PolyTyp && m_ClipType != ClipType.ctXor))
                    AddLocalMaxPoly(e1, e2, pt);
                else
                {
                    AddOutPt(e1, pt);
                    AddOutPt(e2, pt);
                    SwapSides(e1, e2);
                    SwapPolyIndexes(e1, e2);
                }
            }
            else if (e1Contributing)
            {
                if (e2Wc == 0 || e2Wc == 1)
                {
                    AddOutPt(e1, pt);
                    SwapSides(e1, e2);
                    SwapPolyIndexes(e1, e2);
                }
            }
            else if (e2Contributing)
            {
                if (e1Wc == 0 || e1Wc == 1)
                {
                    AddOutPt(e2, pt);
                    SwapSides(e1, e2);
                    SwapPolyIndexes(e1, e2);
                }
            }
            else if ((e1Wc == 0 || e1Wc == 1) &&
                     (e2Wc == 0 || e2Wc == 1) && !e1stops && !e2stops)
            {
                //neither edge is currently contributing ...
                Int64 e1Wc2, e2Wc2;
                switch (e1FillType2)
                {
                    case PolyFillType.pftPositive:
                        e1Wc2 = e1.WindCnt2;
                        break;
                    case PolyFillType.pftNegative:
                        e1Wc2 = -e1.WindCnt2;
                        break;
                    default:
                        e1Wc2 = Math.Abs(e1.WindCnt2);
                        break;
                }

                switch (e2FillType2)
                {
                    case PolyFillType.pftPositive:
                        e2Wc2 = e2.WindCnt2;
                        break;
                    case PolyFillType.pftNegative:
                        e2Wc2 = -e2.WindCnt2;
                        break;
                    default:
                        e2Wc2 = Math.Abs(e2.WindCnt2);
                        break;
                }

                if (e1.PolyTyp != e2.PolyTyp)
                    AddLocalMinPoly(e1, e2, pt);
                else if (e1Wc == 1 && e2Wc == 1)
                    switch (m_ClipType)
                    {
                        case ClipType.ctIntersection:
                            if (e1Wc2 > 0 && e2Wc2 > 0)
                                AddLocalMinPoly(e1, e2, pt);
                            break;
                        case ClipType.ctUnion:
                            if (e1Wc2 <= 0 && e2Wc2 <= 0)
                                AddLocalMinPoly(e1, e2, pt);
                            break;
                        case ClipType.ctDifference:
                            if (((e1.PolyTyp == PolyType.ptClip) && (e1Wc2 > 0) && (e2Wc2 > 0)) ||
                                ((e1.PolyTyp == PolyType.ptSubject) && (e1Wc2 <= 0) && (e2Wc2 <= 0)))
                                AddLocalMinPoly(e1, e2, pt);
                            break;
                        case ClipType.ctXor:
                            AddLocalMinPoly(e1, e2, pt);
                            break;
                    }
                else
                    SwapSides(e1, e2);
            }

            if ((e1stops != e2stops) &&
                ((e1stops && (e1.OutIdx >= 0)) || (e2stops && (e2.OutIdx >= 0))))
            {
                SwapSides(e1, e2);
                SwapPolyIndexes(e1, e2);
            }

            //finally, delete any non-contributing maxima edges  ...
            if (e1stops) DeleteFromAEL(e1);
            if (e2stops) DeleteFromAEL(e2);
        }
        //------------------------------------------------------------------------------

        private void DeleteFromAEL(TEdge e)
        {
            TEdge AelPrev = e.PrevInAEL;
            TEdge AelNext = e.NextInAEL;
            if (AelPrev == null && AelNext == null && (e != m_ActiveEdges))
                return; //already deleted
            if (AelPrev != null)
                AelPrev.NextInAEL = AelNext;
            else m_ActiveEdges = AelNext;
            if (AelNext != null)
                AelNext.PrevInAEL = AelPrev;
            e.NextInAEL = null;
            e.PrevInAEL = null;
        }
        //------------------------------------------------------------------------------

        private void DeleteFromSEL(TEdge e)
        {
            TEdge SelPrev = e.PrevInSEL;
            TEdge SelNext = e.NextInSEL;
            if (SelPrev == null && SelNext == null && (e != m_SortedEdges))
                return; //already deleted
            if (SelPrev != null)
                SelPrev.NextInSEL = SelNext;
            else m_SortedEdges = SelNext;
            if (SelNext != null)
                SelNext.PrevInSEL = SelPrev;
            e.NextInSEL = null;
            e.PrevInSEL = null;
        }
        //------------------------------------------------------------------------------

        private void UpdateEdgeIntoAEL(ref TEdge e)
        {
            if (e.NextInLML == null)
                throw new ClipperException("UpdateEdgeIntoAEL: invalid call");
            TEdge AelPrev = e.PrevInAEL;
            TEdge AelNext = e.NextInAEL;
            e.NextInLML.OutIdx = e.OutIdx;
            if (AelPrev != null)
                AelPrev.NextInAEL = e.NextInLML;
            else m_ActiveEdges = e.NextInLML;
            if (AelNext != null)
                AelNext.PrevInAEL = e.NextInLML;
            e.NextInLML.Side = e.Side;
            e.NextInLML.WindDelta = e.WindDelta;
            e.NextInLML.WindCnt = e.WindCnt;
            e.NextInLML.WindCnt2 = e.WindCnt2;
            e = e.NextInLML;
            e.Curr = e.Bot;
            e.PrevInAEL = AelPrev;
            e.NextInAEL = AelNext;
            if (!IsHorizontal(e)) InsertScanbeam(e.Top.Y);
        }
        //------------------------------------------------------------------------------

        private void ProcessHorizontals(bool isTopOfScanbeam)
        {
            TEdge horzEdge = m_SortedEdges;
            while (horzEdge != null)
            {
                DeleteFromSEL(horzEdge);
                ProcessHorizontal(horzEdge, isTopOfScanbeam);
                horzEdge = m_SortedEdges;
            }
        }
        //------------------------------------------------------------------------------

        void GetHorzDirection(TEdge HorzEdge, out Direction Dir, out Int64 Left, out Int64 Right)
        {
            if (HorzEdge.Bot.X < HorzEdge.Top.X)
            {
                Left = HorzEdge.Bot.X;
                Right = HorzEdge.Top.X;
                Dir = Direction.dLeftToRight;
            }
            else
            {
                Left = HorzEdge.Top.X;
                Right = HorzEdge.Bot.X;
                Dir = Direction.dRightToLeft;
            }
        }
        //------------------------------------------------------------------------

        void PrepareHorzJoins(TEdge horzEdge, bool isTopOfScanbeam)
        {
            //get the last Op for this horizontal edge
            //the point may be anywhere along the horizontal ...
            OutPt outPt = m_PolyOuts[horzEdge.OutIdx].Pts;
            if (horzEdge.Side != EdgeSide.esLeft) outPt = outPt.Prev;

            //First, match up overlapping horizontal edges (eg when one polygon's
            //intermediate horz edge overlaps an intermediate horz edge of another, or
            //when one polygon sits on top of another) ...
            for (int i = 0; i < m_GhostJoins.Count; ++i)
            {
                Join j = m_GhostJoins[i];
                if (HorzSegmentsOverlap(j.OutPt1.Pt, j.OffPt, horzEdge.Bot, horzEdge.Top))
                    AddJoin(j.OutPt1, outPt, j.OffPt);
            }

            //Also, since horizontal edges at the top of one SB are often removed from
            //the AEL before we process the horizontal edges at the bottom of the next,
            //we need to create 'ghost' Join records of 'contrubuting' horizontals that
            //we can compare with horizontals at the bottom of the next SB.
            if (isTopOfScanbeam)
                if (outPt.Pt == horzEdge.Top)
                    AddGhostJoin(outPt, horzEdge.Bot);
                else
                    AddGhostJoin(outPt, horzEdge.Top);
        }
        //------------------------------------------------------------------------------

        private void ProcessHorizontal(TEdge horzEdge, bool isTopOfScanbeam)
        {
            Direction dir;
            Int64 horzLeft, horzRight;

            GetHorzDirection(horzEdge, out dir, out horzLeft, out horzRight);

            TEdge eLastHorz = horzEdge, eMaxPair = null;
            while (eLastHorz.NextInLML != null && IsHorizontal(eLastHorz.NextInLML))
                eLastHorz = eLastHorz.NextInLML;
            if (eLastHorz.NextInLML == null)
                eMaxPair = GetMaximaPair(eLastHorz);

            for (;;)
            {
                bool IsLastHorz = (horzEdge == eLastHorz);
                TEdge e = GetNextInAEL(horzEdge, dir);
                while (e != null)
                {
                    //Break if we've got to the end of an intermediate horizontal edge ...
                    //nb: Smaller Dx's are to the right of larger Dx's ABOVE the horizontal.
                    if (e.Curr.X == horzEdge.Top.X && horzEdge.NextInLML != null &&
                        e.Dx < horzEdge.NextInLML.Dx) break;

                    TEdge eNext = GetNextInAEL(e, dir); //saves eNext for later

                    if ((dir == Direction.dLeftToRight && e.Curr.X <= horzRight) ||
                        (dir == Direction.dRightToLeft && e.Curr.X >= horzLeft))
                    {
                        //so far we're still in range of the horizontal Edge  but make sure
                        //we're at the last of consec. horizontals when matching with eMaxPair
                        if (e == eMaxPair && IsLastHorz)
                        {
                            if (horzEdge.OutIdx >= 0 && horzEdge.WindDelta != 0)
                                PrepareHorzJoins(horzEdge, isTopOfScanbeam);
                            if (dir == Direction.dLeftToRight)
                                IntersectEdges(horzEdge, e, e.Top);
                            else
                                IntersectEdges(e, horzEdge, e.Top);
                            if (eMaxPair.OutIdx >= 0)
                                throw
                                    new ClipperException("ProcessHorizontal error");
                            return;
                        }
                        else if (dir == Direction.dLeftToRight)
                        {
                            IntPoint Pt = new IntPoint(e.Curr.X, horzEdge.Curr.Y);
                            IntersectEdges(horzEdge, e, Pt, true);
                        }
                        else
                        {
                            IntPoint Pt = new IntPoint(e.Curr.X, horzEdge.Curr.Y);
                            IntersectEdges(e, horzEdge, Pt, true);
                        }

                        SwapPositionsInAEL(horzEdge, e);
                    }
                    else if ((dir == Direction.dLeftToRight && e.Curr.X >= horzRight) ||
                             (dir == Direction.dRightToLeft && e.Curr.X <= horzLeft)) break;

                    e = eNext;
                } //end while

                if (horzEdge.OutIdx >= 0 && horzEdge.WindDelta != 0)
                    PrepareHorzJoins(horzEdge, isTopOfScanbeam);

                if (horzEdge.NextInLML != null && IsHorizontal(horzEdge.NextInLML))
                {
                    UpdateEdgeIntoAEL(ref horzEdge);
                    if (horzEdge.OutIdx >= 0) AddOutPt(horzEdge, horzEdge.Bot);
                    GetHorzDirection(horzEdge, out dir, out horzLeft, out horzRight);
                }
                else
                    break;
            } //end for (;;)

            if (horzEdge.NextInLML != null)
            {
                if (horzEdge.OutIdx >= 0)
                {
                    OutPt op1 = AddOutPt(horzEdge, horzEdge.Top);
                    UpdateEdgeIntoAEL(ref horzEdge);
                    if (horzEdge.WindDelta == 0) return;
                    //nb: HorzEdge is no longer horizontal here
                    TEdge ePrev = horzEdge.PrevInAEL;
                    TEdge eNext = horzEdge.NextInAEL;
                    if (ePrev != null && ePrev.Curr.X == horzEdge.Bot.X &&
                        ePrev.Curr.Y == horzEdge.Bot.Y && ePrev.WindDelta != 0 &&
                        (ePrev.OutIdx >= 0 && ePrev.Curr.Y > ePrev.Top.Y &&
                         SlopesEqual(horzEdge, ePrev, m_UseFullRange)))
                    {
                        OutPt op2 = AddOutPt(ePrev, horzEdge.Bot);
                        AddJoin(op1, op2, horzEdge.Top);
                    }
                    else if (eNext != null && eNext.Curr.X == horzEdge.Bot.X &&
                             eNext.Curr.Y == horzEdge.Bot.Y && eNext.WindDelta != 0 &&
                             eNext.OutIdx >= 0 && eNext.Curr.Y > eNext.Top.Y &&
                             SlopesEqual(horzEdge, eNext, m_UseFullRange))
                    {
                        OutPt op2 = AddOutPt(eNext, horzEdge.Bot);
                        AddJoin(op1, op2, horzEdge.Top);
                    }
                }
                else
                    UpdateEdgeIntoAEL(ref horzEdge);
            }
            else if (eMaxPair != null)
            {
                if (eMaxPair.OutIdx >= 0)
                {
                    if (dir == Direction.dLeftToRight)
                        IntersectEdges(horzEdge, eMaxPair, horzEdge.Top);
                    else
                        IntersectEdges(eMaxPair, horzEdge, horzEdge.Top);
                    if (eMaxPair.OutIdx >= 0)
                        throw
                            new ClipperException("ProcessHorizontal error");
                }
                else
                {
                    DeleteFromAEL(horzEdge);
                    DeleteFromAEL(eMaxPair);
                }
            }
            else
            {
                if (horzEdge.OutIdx >= 0) AddOutPt(horzEdge, horzEdge.Top);
                DeleteFromAEL(horzEdge);
            }
        }
        //------------------------------------------------------------------------------

        private TEdge GetNextInAEL(TEdge e, Direction Direction)
        {
            return Direction == Direction.dLeftToRight ? e.NextInAEL : e.PrevInAEL;
        }
        //------------------------------------------------------------------------------

        private bool IsMinima(TEdge e)
        {
            return e != null && (e.Prev.NextInLML != e) && (e.Next.NextInLML != e);
        }
        //------------------------------------------------------------------------------

        private bool IsMaxima(TEdge e, double Y)
        {
            return (e != null && e.Top.Y == Y && e.NextInLML == null);
        }
        //------------------------------------------------------------------------------

        private bool IsIntermediate(TEdge e, double Y)
        {
            return (e.Top.Y == Y && e.NextInLML != null);
        }
        //------------------------------------------------------------------------------

        private TEdge GetMaximaPair(TEdge e)
        {
            TEdge result = null;
            if ((e.Next.Top == e.Top) && e.Next.NextInLML == null)
                result = e.Next;
            else if ((e.Prev.Top == e.Top) && e.Prev.NextInLML == null)
                result = e.Prev;
            if (result != null && (result.OutIdx == Skip ||
                                   (result.NextInAEL == result.PrevInAEL && !IsHorizontal(result))))
                return null;
            return result;
        }
        //------------------------------------------------------------------------------

        private bool ProcessIntersections(Int64 botY, Int64 topY)
        {
            if (m_ActiveEdges == null) return true;
            try
            {
                BuildIntersectList(botY, topY);
                if (m_IntersectNodes == null) return true;
                if (m_IntersectNodes.Next == null || FixupIntersectionOrder())
                    ProcessIntersectList();
                else
                    return false;
            }
            catch
            {
                m_SortedEdges = null;
                DisposeIntersectNodes();
                throw new ClipperException("ProcessIntersections error");
            }

            m_SortedEdges = null;
            return true;
        }
        //------------------------------------------------------------------------------

        private void BuildIntersectList(Int64 botY, Int64 topY)
        {
            if (m_ActiveEdges == null) return;

            //prepare for sorting ...
            TEdge e = m_ActiveEdges;
            m_SortedEdges = e;
            while (e != null)
            {
                e.PrevInSEL = e.PrevInAEL;
                e.NextInSEL = e.NextInAEL;
                e.Curr.X = TopX(e, topY);
                e = e.NextInAEL;
            }

            //bubblesort ...
            bool isModified = true;
            while (isModified && m_SortedEdges != null)
            {
                isModified = false;
                e = m_SortedEdges;
                while (e.NextInSEL != null)
                {
                    TEdge eNext = e.NextInSEL;
                    IntPoint pt;
                    if (e.Curr.X > eNext.Curr.X)
                    {
                        if (!IntersectPoint(e, eNext, out pt) && e.Curr.X > eNext.Curr.X + 1)
                            throw new ClipperException("Intersection error");
                        if (pt.Y > botY)
                        {
                            pt.Y = botY;
                            if (Math.Abs(e.Dx) > Math.Abs(eNext.Dx))
                                pt.X = TopX(eNext, botY);
                            else
                                pt.X = TopX(e, botY);
                        }

                        InsertIntersectNode(e, eNext, pt);
                        SwapPositionsInSEL(e, eNext);
                        isModified = true;
                    }
                    else
                        e = eNext;
                }

                if (e.PrevInSEL != null) e.PrevInSEL.NextInSEL = null;
                else break;
            }

            m_SortedEdges = null;
        }
        //------------------------------------------------------------------------------

        private bool EdgesAdjacent(IntersectNode inode)
        {
            return (inode.Edge1.NextInSEL == inode.Edge2) ||
                   (inode.Edge1.PrevInSEL == inode.Edge2);
        }
        //------------------------------------------------------------------------------

        private bool FixupIntersectionOrder()
        {
            //pre-condition: intersections are sorted bottom-most (then left-most) first.
            //Now it's crucial that intersections are made only between adjacent edges,
            //so to ensure this the order of intersections may need adjusting ...
            IntersectNode inode = m_IntersectNodes;
            CopyAELToSEL();
            while (inode != null)
            {
                if (!EdgesAdjacent(inode))
                {
                    IntersectNode nextNode = inode.Next;
                    while (nextNode != null && !EdgesAdjacent(nextNode))
                        nextNode = nextNode.Next;
                    if (nextNode == null)
                        return false;
                    SwapIntersectNodes(inode, nextNode);
                }

                SwapPositionsInSEL(inode.Edge1, inode.Edge2);
                inode = inode.Next;
            }

            return true;
        }
        //------------------------------------------------------------------------------

        private void ProcessIntersectList()
        {
            while (m_IntersectNodes != null)
            {
                IntersectNode iNode = m_IntersectNodes.Next;
                {
                    IntersectEdges(m_IntersectNodes.Edge1,
                        m_IntersectNodes.Edge2, m_IntersectNodes.Pt, true);
                    SwapPositionsInAEL(m_IntersectNodes.Edge1, m_IntersectNodes.Edge2);
                }
                m_IntersectNodes = null;
                m_IntersectNodes = iNode;
            }
        }
        //------------------------------------------------------------------------------

        internal static Int64 Round(double value)
        {
            return value < 0 ? (Int64) (value - 0.5) : (Int64) (value + 0.5);
        }
        //------------------------------------------------------------------------------

        private static Int64 TopX(TEdge edge, Int64 currentY)
        {
            if (currentY == edge.Top.Y)
                return edge.Top.X;
            return edge.Bot.X + Round(edge.Dx * (currentY - edge.Bot.Y));
        }
        //------------------------------------------------------------------------------

        private void InsertIntersectNode(TEdge e1, TEdge e2, IntPoint pt)
        {
            IntersectNode newNode = new IntersectNode();
            newNode.Edge1 = e1;
            newNode.Edge2 = e2;
            newNode.Pt = pt;
            newNode.Next = null;
            if (m_IntersectNodes == null) m_IntersectNodes = newNode;
            else if (newNode.Pt.Y > m_IntersectNodes.Pt.Y)
            {
                newNode.Next = m_IntersectNodes;
                m_IntersectNodes = newNode;
            }
            else
            {
                IntersectNode iNode = m_IntersectNodes;
                while (iNode.Next != null && newNode.Pt.Y < iNode.Next.Pt.Y)
                    iNode = iNode.Next;
                newNode.Next = iNode.Next;
                iNode.Next = newNode;
            }
        }
        //------------------------------------------------------------------------------

        private void SwapIntersectNodes(IntersectNode int1, IntersectNode int2)
        {
            TEdge e1 = int1.Edge1;
            TEdge e2 = int1.Edge2;
            IntPoint p = new IntPoint(int1.Pt);
            int1.Edge1 = int2.Edge1;
            int1.Edge2 = int2.Edge2;
            int1.Pt = int2.Pt;
            int2.Edge1 = e1;
            int2.Edge2 = e2;
            int2.Pt = p;
        }
        //------------------------------------------------------------------------------

        private bool IntersectPoint(TEdge edge1, TEdge edge2, out IntPoint ip)
        {
            ip = new IntPoint();
            double b1, b2;
            if (SlopesEqual(edge1, edge2, m_UseFullRange))
            {
                if (edge2.Bot.Y > edge1.Bot.Y)
                    ip.Y = edge2.Bot.Y;
                else
                    ip.Y = edge1.Bot.Y;
                return false;
            }
            else if (edge1.Delta.X == 0)
            {
                ip.X = edge1.Bot.X;
                if (IsHorizontal(edge2))
                {
                    ip.Y = edge2.Bot.Y;
                }
                else
                {
                    b2 = edge2.Bot.Y - (edge2.Bot.X / edge2.Dx);
                    ip.Y = Round(ip.X / edge2.Dx + b2);
                }
            }
            else if (edge2.Delta.X == 0)
            {
                ip.X = edge2.Bot.X;
                if (IsHorizontal(edge1))
                {
                    ip.Y = edge1.Bot.Y;
                }
                else
                {
                    b1 = edge1.Bot.Y - (edge1.Bot.X / edge1.Dx);
                    ip.Y = Round(ip.X / edge1.Dx + b1);
                }
            }
            else
            {
                b1 = edge1.Bot.X - edge1.Bot.Y * edge1.Dx;
                b2 = edge2.Bot.X - edge2.Bot.Y * edge2.Dx;
                double q = (b2 - b1) / (edge1.Dx - edge2.Dx);
                ip.Y = Round(q);
                if (Math.Abs(edge1.Dx) < Math.Abs(edge2.Dx))
                    ip.X = Round(edge1.Dx * q + b1);
                else
                    ip.X = Round(edge2.Dx * q + b2);
            }

            if (ip.Y < edge1.Top.Y || ip.Y < edge2.Top.Y)
            {
                if (edge1.Top.Y > edge2.Top.Y)
                {
                    ip.Y = edge1.Top.Y;
                    ip.X = TopX(edge2, edge1.Top.Y);
                    return ip.X < edge1.Top.X;
                }
                else
                {
                    ip.Y = edge2.Top.Y;
                    ip.X = TopX(edge1, edge2.Top.Y);
                    return ip.X > edge2.Top.X;
                }
            }
            else
                return true;
        }
        //------------------------------------------------------------------------------

        private void DisposeIntersectNodes()
        {
            while (m_IntersectNodes != null)
            {
                IntersectNode iNode = m_IntersectNodes.Next;
                m_IntersectNodes = null;
                m_IntersectNodes = iNode;
            }
        }
        //------------------------------------------------------------------------------

        private void ProcessEdgesAtTopOfScanbeam(Int64 topY)
        {
            TEdge e = m_ActiveEdges;
            while (e != null)
            {
                //1. process maxima, treating them as if they're 'bent' horizontal edges,
                //   but exclude maxima with horizontal edges. nb: e can't be a horizontal.
                bool IsMaximaEdge = IsMaxima(e, topY);

                if (IsMaximaEdge)
                {
                    TEdge eMaxPair = GetMaximaPair(e);
                    IsMaximaEdge = (eMaxPair == null || !IsHorizontal(eMaxPair));
                }

                if (IsMaximaEdge)
                {
                    TEdge ePrev = e.PrevInAEL;
                    DoMaxima(e);
                    if (ePrev == null) e = m_ActiveEdges;
                    else e = ePrev.NextInAEL;
                }
                else
                {
                    //2. promote horizontal edges, otherwise update Curr.X and Curr.Y ...
                    if (IsIntermediate(e, topY) && IsHorizontal(e.NextInLML))
                    {
                        UpdateEdgeIntoAEL(ref e);
                        if (e.OutIdx >= 0)
                            AddOutPt(e, e.Bot);
                        AddEdgeToSEL(e);
                    }
                    else
                    {
                        e.Curr.X = TopX(e, topY);
                        e.Curr.Y = topY;
                    }

                    if (StrictlySimple)
                    {
                        TEdge ePrev = e.PrevInAEL;
                        if ((e.OutIdx >= 0) && (e.WindDelta != 0) && ePrev != null &&
                            (ePrev.OutIdx >= 0) && (ePrev.Curr.X == e.Curr.X) &&
                            (ePrev.WindDelta != 0))
                        {
                            OutPt op = AddOutPt(ePrev, e.Curr);
                            OutPt op2 = AddOutPt(e, e.Curr);
                            AddJoin(op, op2, e.Curr); //StrictlySimple (type-3) join
                        }
                    }

                    e = e.NextInAEL;
                }
            }

            //3. Process horizontals at the Top of the scanbeam ...
            ProcessHorizontals(true);

            //4. Promote intermediate vertices ...
            e = m_ActiveEdges;
            while (e != null)
            {
                if (IsIntermediate(e, topY))
                {
                    OutPt op = null;
                    if (e.OutIdx >= 0)
                        op = AddOutPt(e, e.Top);
                    UpdateEdgeIntoAEL(ref e);

                    //if output polygons share an edge, they'll need joining later ...
                    TEdge ePrev = e.PrevInAEL;
                    TEdge eNext = e.NextInAEL;
                    if (ePrev != null && ePrev.Curr.X == e.Bot.X &&
                        ePrev.Curr.Y == e.Bot.Y && op != null &&
                        ePrev.OutIdx >= 0 && ePrev.Curr.Y > ePrev.Top.Y &&
                        SlopesEqual(e, ePrev, m_UseFullRange) &&
                        (e.WindDelta != 0) && (ePrev.WindDelta != 0))
                    {
                        OutPt op2 = AddOutPt(ePrev, e.Bot);
                        AddJoin(op, op2, e.Top);
                    }
                    else if (eNext != null && eNext.Curr.X == e.Bot.X &&
                             eNext.Curr.Y == e.Bot.Y && op != null &&
                             eNext.OutIdx >= 0 && eNext.Curr.Y > eNext.Top.Y &&
                             SlopesEqual(e, eNext, m_UseFullRange) &&
                             (e.WindDelta != 0) && (eNext.WindDelta != 0))
                    {
                        OutPt op2 = AddOutPt(eNext, e.Bot);
                        AddJoin(op, op2, e.Top);
                    }
                }

                e = e.NextInAEL;
            }
        }
        //------------------------------------------------------------------------------

        private void DoMaxima(TEdge e)
        {
            TEdge eMaxPair = GetMaximaPair(e);
            if (eMaxPair == null)
            {
                if (e.OutIdx >= 0)
                    AddOutPt(e, e.Top);
                DeleteFromAEL(e);
                return;
            }

            TEdge eNext = e.NextInAEL;
            while (eNext != null && eNext != eMaxPair)
            {
                IntersectEdges(e, eNext, e.Top, true);
                SwapPositionsInAEL(e, eNext);
                eNext = e.NextInAEL;
            }

            if (e.OutIdx == Unassigned && eMaxPair.OutIdx == Unassigned)
            {
                DeleteFromAEL(e);
                DeleteFromAEL(eMaxPair);
            }
            else if (e.OutIdx >= 0 && eMaxPair.OutIdx >= 0)
            {
                IntersectEdges(e, eMaxPair, e.Top);
            }
#if use_lines
        else if (e.WindDelta == 0)
        {
          if (e.OutIdx >= 0) 
          {
            AddOutPt(e, e.Top);
            e.OutIdx = Unassigned;
          }
          DeleteFromAEL(e);

          if (eMaxPair.OutIdx >= 0)
          {
            AddOutPt(eMaxPair, e.Top);
            eMaxPair.OutIdx = Unassigned;
          }
          DeleteFromAEL(eMaxPair);
        }
#endif
            else throw new ClipperException("DoMaxima error");
        }
        //------------------------------------------------------------------------------

        public static void ReversePaths(List<List<IntPoint>> polys)
        {
            polys.ForEach(delegate(List<IntPoint> poly) { poly.Reverse(); });
        }
        //------------------------------------------------------------------------------

        public static bool Orientation(List<IntPoint> poly)
        {
            return Area(poly) >= 0;
        }
        //------------------------------------------------------------------------------

        private int PointCount(OutPt pts)
        {
            if (pts == null) return 0;
            int result = 0;
            OutPt p = pts;
            do
            {
                result++;
                p = p.Next;
            } while (p != pts);

            return result;
        }
        //------------------------------------------------------------------------------

        private void BuildResult(List<List<IntPoint>> polyg)
        {
            polyg.Clear();
            polyg.Capacity = m_PolyOuts.Count;
            for (int i = 0; i < m_PolyOuts.Count; i++)
            {
                OutRec outRec = m_PolyOuts[i];
                if (outRec.Pts == null) continue;
                OutPt p = outRec.Pts;
                int cnt = PointCount(p);
                if (cnt < 2) continue;
                List<IntPoint> pg = new List<IntPoint>(cnt);
                for (int j = 0; j < cnt; j++)
                {
                    pg.Add(p.Pt);
                    p = p.Prev;
                }

                polyg.Add(pg);
            }
        }
        //------------------------------------------------------------------------------

        private void BuildResult2(PolyTree polytree)
        {
            polytree.Clear();

            //add each output polygon/contour to polytree ...
            polytree.m_AllPolys.Capacity = m_PolyOuts.Count;
            for (int i = 0; i < m_PolyOuts.Count; i++)
            {
                OutRec outRec = m_PolyOuts[i];
                int cnt = PointCount(outRec.Pts);
                if ((outRec.IsOpen && cnt < 2) ||
                    (!outRec.IsOpen && cnt < 3)) continue;
                FixHoleLinkage(outRec);
                PolyNode pn = new PolyNode();
                polytree.m_AllPolys.Add(pn);
                outRec.PolyNode = pn;
                pn.m_polygon.Capacity = cnt;
                OutPt op = outRec.Pts.Prev;
                for (int j = 0; j < cnt; j++)
                {
                    pn.m_polygon.Add(op.Pt);
                    op = op.Prev;
                }
            }

            //fixup PolyNode links etc ...
            polytree.m_Childs.Capacity = m_PolyOuts.Count;
            for (int i = 0; i < m_PolyOuts.Count; i++)
            {
                OutRec outRec = m_PolyOuts[i];
                if (outRec.PolyNode == null) continue;
                else if (outRec.IsOpen)
                {
                    outRec.PolyNode.IsOpen = true;
                    polytree.AddChild(outRec.PolyNode);
                }
                else if (outRec.FirstLeft != null)
                    outRec.FirstLeft.PolyNode.AddChild(outRec.PolyNode);
                else
                    polytree.AddChild(outRec.PolyNode);
            }
        }
        //------------------------------------------------------------------------------

        private void FixupOutPolygon(OutRec outRec)
        {
            //FixupOutPolygon() - removes duplicate points and simplifies consecutive
            //parallel edges by removing the middle vertex.
            OutPt lastOK = null;
            outRec.BottomPt = null;
            OutPt pp = outRec.Pts;
            for (;;)
            {
                if (pp.Prev == pp || pp.Prev == pp.Next)
                {
                    DisposeOutPts(pp);
                    outRec.Pts = null;
                    return;
                }

                //test for duplicate points and collinear edges ...
                if ((pp.Pt == pp.Next.Pt) || (pp.Pt == pp.Prev.Pt) ||
                    (SlopesEqual(pp.Prev.Pt, pp.Pt, pp.Next.Pt, m_UseFullRange) &&
                     (!PreserveCollinear || !Pt2IsBetweenPt1AndPt3(pp.Prev.Pt, pp.Pt, pp.Next.Pt))))
                {
                    lastOK = null;
                    OutPt tmp = pp;
                    pp.Prev.Next = pp.Next;
                    pp.Next.Prev = pp.Prev;
                    pp = pp.Prev;
                    tmp = null;
                }
                else if (pp == lastOK) break;
                else
                {
                    if (lastOK == null) lastOK = pp;
                    pp = pp.Next;
                }
            }

            outRec.Pts = pp;
        }
        //------------------------------------------------------------------------------

        OutPt DupOutPt(OutPt outPt, bool InsertAfter)
        {
            OutPt result = new OutPt();
            result.Pt = outPt.Pt;
            result.Idx = outPt.Idx;
            if (InsertAfter)
            {
                result.Next = outPt.Next;
                result.Prev = outPt;
                outPt.Next.Prev = result;
                outPt.Next = result;
            }
            else
            {
                result.Prev = outPt.Prev;
                result.Next = outPt;
                outPt.Prev.Next = result;
                outPt.Prev = result;
            }

            return result;
        }
        //------------------------------------------------------------------------------

        bool GetOverlap(Int64 a1, Int64 a2, Int64 b1, Int64 b2, out Int64 Left, out Int64 Right)
        {
            if (a1 < a2)
            {
                if (b1 < b2)
                {
                    Left = Math.Max(a1, b1);
                    Right = Math.Min(a2, b2);
                }
                else
                {
                    Left = Math.Max(a1, b2);
                    Right = Math.Min(a2, b1);
                }
            }
            else
            {
                if (b1 < b2)
                {
                    Left = Math.Max(a2, b1);
                    Right = Math.Min(a1, b2);
                }
                else
                {
                    Left = Math.Max(a2, b2);
                    Right = Math.Min(a1, b1);
                }
            }

            return Left < Right;
        }
        //------------------------------------------------------------------------------

        bool JoinHorz(OutPt op1, OutPt op1b, OutPt op2, OutPt op2b,
            IntPoint Pt, bool DiscardLeft)
        {
            Direction Dir1 = (op1.Pt.X > op1b.Pt.X ? Direction.dRightToLeft : Direction.dLeftToRight);
            Direction Dir2 = (op2.Pt.X > op2b.Pt.X ? Direction.dRightToLeft : Direction.dLeftToRight);
            if (Dir1 == Dir2) return false;

            //When DiscardLeft, we want Op1b to be on the Left of Op1, otherwise we
            //want Op1b to be on the Right. (And likewise with Op2 and Op2b.)
            //So, to facilitate this while inserting Op1b and Op2b ...
            //when DiscardLeft, make sure we're AT or RIGHT of Pt before adding Op1b,
            //otherwise make sure we're AT or LEFT of Pt. (Likewise with Op2b.)
            if (Dir1 == Direction.dLeftToRight)
            {
                while (op1.Next.Pt.X <= Pt.X &&
                       op1.Next.Pt.X >= op1.Pt.X && op1.Next.Pt.Y == Pt.Y)
                    op1 = op1.Next;
                if (DiscardLeft && (op1.Pt.X != Pt.X)) op1 = op1.Next;
                op1b = DupOutPt(op1, !DiscardLeft);
                if (op1b.Pt != Pt)
                {
                    op1 = op1b;
                    op1.Pt = Pt;
                    op1b = DupOutPt(op1, !DiscardLeft);
                }
            }
            else
            {
                while (op1.Next.Pt.X >= Pt.X &&
                       op1.Next.Pt.X <= op1.Pt.X && op1.Next.Pt.Y == Pt.Y)
                    op1 = op1.Next;
                if (!DiscardLeft && (op1.Pt.X != Pt.X)) op1 = op1.Next;
                op1b = DupOutPt(op1, DiscardLeft);
                if (op1b.Pt != Pt)
                {
                    op1 = op1b;
                    op1.Pt = Pt;
                    op1b = DupOutPt(op1, DiscardLeft);
                }
            }

            if (Dir2 == Direction.dLeftToRight)
            {
                while (op2.Next.Pt.X <= Pt.X &&
                       op2.Next.Pt.X >= op2.Pt.X && op2.Next.Pt.Y == Pt.Y)
                    op2 = op2.Next;
                if (DiscardLeft && (op2.Pt.X != Pt.X)) op2 = op2.Next;
                op2b = DupOutPt(op2, !DiscardLeft);
                if (op2b.Pt != Pt)
                {
                    op2 = op2b;
                    op2.Pt = Pt;
                    op2b = DupOutPt(op2, !DiscardLeft);
                }

                ;
            }
            else
            {
                while (op2.Next.Pt.X >= Pt.X &&
                       op2.Next.Pt.X <= op2.Pt.X && op2.Next.Pt.Y == Pt.Y)
                    op2 = op2.Next;
                if (!DiscardLeft && (op2.Pt.X != Pt.X)) op2 = op2.Next;
                op2b = DupOutPt(op2, DiscardLeft);
                if (op2b.Pt != Pt)
                {
                    op2 = op2b;
                    op2.Pt = Pt;
                    op2b = DupOutPt(op2, DiscardLeft);
                }

                ;
            }

            ;

            if ((Dir1 == Direction.dLeftToRight) == DiscardLeft)
            {
                op1.Prev = op2;
                op2.Next = op1;
                op1b.Next = op2b;
                op2b.Prev = op1b;
            }
            else
            {
                op1.Next = op2;
                op2.Prev = op1;
                op1b.Prev = op2b;
                op2b.Next = op1b;
            }

            return true;
        }
        //------------------------------------------------------------------------------

        private bool JoinPoints(Join j, out OutPt p1, out OutPt p2)
        {
            OutRec outRec1 = GetOutRec(j.OutPt1.Idx);
            OutRec outRec2 = GetOutRec(j.OutPt2.Idx);
            OutPt op1 = j.OutPt1, op1b;
            OutPt op2 = j.OutPt2, op2b;
            p1 = null;
            p2 = null;

            //There are 3 kinds of joins for output polygons ...
            //1. Horizontal joins where Join.OutPt1 & Join.OutPt2 are a vertices anywhere
            //along (horizontal) collinear edges (& Join.OffPt is on the same horizontal).
            //2. Non-horizontal joins where Join.OutPt1 & Join.OutPt2 are at the same
            //location at the Bottom of the overlapping segment (& Join.OffPt is above).
            //3. StrictSimple joins where edges touch but are not collinear and where
            //Join.OutPt1, Join.OutPt2 & Join.OffPt all share the same point.
            bool isHorizontal = (j.OutPt1.Pt.Y == j.OffPt.Y);

            if (isHorizontal && (j.OffPt == j.OutPt1.Pt) && (j.OffPt == j.OutPt2.Pt))
            {
                //Strictly Simple join ...
                op1b = j.OutPt1.Next;
                while (op1b != op1 && (op1b.Pt == j.OffPt))
                    op1b = op1b.Next;
                bool reverse1 = (op1b.Pt.Y > j.OffPt.Y);
                op2b = j.OutPt2.Next;
                while (op2b != op2 && (op2b.Pt == j.OffPt))
                    op2b = op2b.Next;
                bool reverse2 = (op2b.Pt.Y > j.OffPt.Y);
                if (reverse1 == reverse2) return false;
                if (reverse1)
                {
                    op1b = DupOutPt(op1, false);
                    op2b = DupOutPt(op2, true);
                    op1.Prev = op2;
                    op2.Next = op1;
                    op1b.Next = op2b;
                    op2b.Prev = op1b;
                    p1 = op1;
                    p2 = op1b;
                    return true;
                }
                else
                {
                    op1b = DupOutPt(op1, true);
                    op2b = DupOutPt(op2, false);
                    op1.Next = op2;
                    op2.Prev = op1;
                    op1b.Prev = op2b;
                    op2b.Next = op1b;
                    p1 = op1;
                    p2 = op1b;
                    return true;
                }
            }
            else if (isHorizontal)
            {
                //treat horizontal joins differently to non-horizontal joins since with
                //them we're not yet sure where the overlapping is. OutPt1.Pt & OutPt2.Pt
                //may be anywhere along the horizontal edge.
                op1b = op1;
                while (op1.Prev.Pt.Y == op1.Pt.Y && op1.Prev != op1b && op1.Prev != op2)
                    op1 = op1.Prev;
                while (op1b.Next.Pt.Y == op1b.Pt.Y && op1b.Next != op1 && op1b.Next != op2)
                    op1b = op1b.Next;
                if (op1b.Next == op1 || op1b.Next == op2) return false; //a flat 'polygon'

                op2b = op2;
                while (op2.Prev.Pt.Y == op2.Pt.Y && op2.Prev != op2b && op2.Prev != op1b)
                    op2 = op2.Prev;
                while (op2b.Next.Pt.Y == op2b.Pt.Y && op2b.Next != op2 && op2b.Next != op1)
                    op2b = op2b.Next;
                if (op2b.Next == op2 || op2b.Next == op1) return false; //a flat 'polygon'

                Int64 Left, Right;
                //Op1 -. Op1b & Op2 -. Op2b are the extremites of the horizontal edges
                if (!GetOverlap(op1.Pt.X, op1b.Pt.X, op2.Pt.X, op2b.Pt.X, out Left, out Right))
                    return false;

                //DiscardLeftSide: when overlapping edges are joined, a spike will created
                //which needs to be cleaned up. However, we don't want Op1 or Op2 caught up
                //on the discard Side as either may still be needed for other joins ...
                IntPoint Pt;
                bool DiscardLeftSide;
                if (op1.Pt.X >= Left && op1.Pt.X <= Right)
                {
                    Pt = op1.Pt;
                    DiscardLeftSide = (op1.Pt.X > op1b.Pt.X);
                }
                else if (op2.Pt.X >= Left && op2.Pt.X <= Right)
                {
                    Pt = op2.Pt;
                    DiscardLeftSide = (op2.Pt.X > op2b.Pt.X);
                }
                else if (op1b.Pt.X >= Left && op1b.Pt.X <= Right)
                {
                    Pt = op1b.Pt;
                    DiscardLeftSide = op1b.Pt.X > op1.Pt.X;
                }
                else
                {
                    Pt = op2b.Pt;
                    DiscardLeftSide = (op2b.Pt.X > op2.Pt.X);
                }

                p1 = op1;
                p2 = op2;
                return JoinHorz(op1, op1b, op2, op2b, Pt, DiscardLeftSide);
            }
            else
            {
                //nb: For non-horizontal joins ...
                //    1. Jr.OutPt1.Pt.Y == Jr.OutPt2.Pt.Y
                //    2. Jr.OutPt1.Pt > Jr.OffPt.Y

                //make sure the polygons are correctly oriented ...
                op1b = op1.Next;
                while ((op1b.Pt == op1.Pt) && (op1b != op1)) op1b = op1b.Next;
                bool Reverse1 = ((op1b.Pt.Y > op1.Pt.Y) ||
                                 !SlopesEqual(op1.Pt, op1b.Pt, j.OffPt, m_UseFullRange));
                if (Reverse1)
                {
                    op1b = op1.Prev;
                    while ((op1b.Pt == op1.Pt) && (op1b != op1)) op1b = op1b.Prev;
                    if ((op1b.Pt.Y > op1.Pt.Y) ||
                        !SlopesEqual(op1.Pt, op1b.Pt, j.OffPt, m_UseFullRange)) return false;
                }

                ;
                op2b = op2.Next;
                while ((op2b.Pt == op2.Pt) && (op2b != op2)) op2b = op2b.Next;
                bool Reverse2 = ((op2b.Pt.Y > op2.Pt.Y) ||
                                 !SlopesEqual(op2.Pt, op2b.Pt, j.OffPt, m_UseFullRange));
                if (Reverse2)
                {
                    op2b = op2.Prev;
                    while ((op2b.Pt == op2.Pt) && (op2b != op2)) op2b = op2b.Prev;
                    if ((op2b.Pt.Y > op2.Pt.Y) ||
                        !SlopesEqual(op2.Pt, op2b.Pt, j.OffPt, m_UseFullRange)) return false;
                }

                if ((op1b == op1) || (op2b == op2) || (op1b == op2b) ||
                    ((outRec1 == outRec2) && (Reverse1 == Reverse2))) return false;

                if (Reverse1)
                {
                    op1b = DupOutPt(op1, false);
                    op2b = DupOutPt(op2, true);
                    op1.Prev = op2;
                    op2.Next = op1;
                    op1b.Next = op2b;
                    op2b.Prev = op1b;
                    p1 = op1;
                    p2 = op1b;
                    return true;
                }
                else
                {
                    op1b = DupOutPt(op1, true);
                    op2b = DupOutPt(op2, false);
                    op1.Next = op2;
                    op2.Prev = op1;
                    op1b.Prev = op2b;
                    op2b.Next = op1b;
                    p1 = op1;
                    p2 = op1b;
                    return true;
                }
            }
        }
        //----------------------------------------------------------------------


        private bool Poly2ContainsPoly1(OutPt outPt1, OutPt outPt2, bool UseFullInt64Range)
        {
            OutPt pt = outPt1;
            //Because the polygons may be touching, we need to find a vertex that
            //isn't touching the other polygon ...
            if (PointOnPolygon(pt.Pt, outPt2, UseFullInt64Range))
            {
                pt = pt.Next;
                while (pt != outPt1 && PointOnPolygon(pt.Pt, outPt2, UseFullInt64Range))
                    pt = pt.Next;
                if (pt == outPt1) return true;
            }

            return PointInPolygon(pt.Pt, outPt2, UseFullInt64Range);
        }
        //----------------------------------------------------------------------

        private void FixupFirstLefts1(OutRec OldOutRec, OutRec NewOutRec)
        {
            for (int i = 0; i < m_PolyOuts.Count; i++)
            {
                OutRec outRec = m_PolyOuts[i];
                if (outRec.Pts != null && outRec.FirstLeft == OldOutRec)
                {
                    if (Poly2ContainsPoly1(outRec.Pts, NewOutRec.Pts, m_UseFullRange))
                        outRec.FirstLeft = NewOutRec;
                }
            }
        }
        //----------------------------------------------------------------------

        private void FixupFirstLefts2(OutRec OldOutRec, OutRec NewOutRec)
        {
            foreach (OutRec outRec in m_PolyOuts)
                if (outRec.FirstLeft == OldOutRec)
                    outRec.FirstLeft = NewOutRec;
        }
        //----------------------------------------------------------------------

        private void JoinCommonEdges()
        {
            for (int i = 0; i < m_Joins.Count; i++)
            {
                Join j = m_Joins[i];

                OutRec outRec1 = GetOutRec(j.OutPt1.Idx);
                OutRec outRec2 = GetOutRec(j.OutPt2.Idx);

                if (outRec1.Pts == null || outRec2.Pts == null) continue;

                //get the polygon fragment with the correct hole state (FirstLeft)
                //before calling JoinPoints() ...
                OutRec holeStateRec;
                if (outRec1 == outRec2) holeStateRec = outRec1;
                else if (Param1RightOfParam2(outRec1, outRec2)) holeStateRec = outRec2;
                else if (Param1RightOfParam2(outRec2, outRec1)) holeStateRec = outRec1;
                else holeStateRec = GetLowermostRec(outRec1, outRec2);

                OutPt p1, p2;
                if (!JoinPoints(j, out p1, out p2)) continue;

                if (outRec1 == outRec2)
                {
                    //instead of joining two polygons, we've just created a new one by
                    //splitting one polygon into two.
                    outRec1.Pts = p1;
                    outRec1.BottomPt = null;
                    outRec2 = CreateOutRec();
                    outRec2.Pts = p2;

                    //update all OutRec2.Pts Idx's ...
                    UpdateOutPtIdxs(outRec2);

                    if (Poly2ContainsPoly1(outRec2.Pts, outRec1.Pts, m_UseFullRange))
                    {
                        //outRec2 is contained by outRec1 ...
                        outRec2.IsHole = !outRec1.IsHole;
                        outRec2.FirstLeft = outRec1;

                        //fixup FirstLeft pointers that may need reassigning to OutRec1
                        if (m_UsingPolyTree) FixupFirstLefts2(outRec2, outRec1);

                        if ((outRec2.IsHole ^ ReverseSolution) == (Area(outRec2) > 0))
                            ReversePolyPtLinks(outRec2.Pts);
                    }
                    else if (Poly2ContainsPoly1(outRec1.Pts, outRec2.Pts, m_UseFullRange))
                    {
                        //outRec1 is contained by outRec2 ...
                        outRec2.IsHole = outRec1.IsHole;
                        outRec1.IsHole = !outRec2.IsHole;
                        outRec2.FirstLeft = outRec1.FirstLeft;
                        outRec1.FirstLeft = outRec2;

                        //fixup FirstLeft pointers that may need reassigning to OutRec1
                        if (m_UsingPolyTree) FixupFirstLefts2(outRec1, outRec2);

                        if ((outRec1.IsHole ^ ReverseSolution) == (Area(outRec1) > 0))
                            ReversePolyPtLinks(outRec1.Pts);
                    }
                    else
                    {
                        //the 2 polygons are completely separate ...
                        outRec2.IsHole = outRec1.IsHole;
                        outRec2.FirstLeft = outRec1.FirstLeft;

                        //fixup FirstLeft pointers that may need reassigning to OutRec2
                        if (m_UsingPolyTree) FixupFirstLefts1(outRec1, outRec2);
                    }
                }
                else
                {
                    //joined 2 polygons together ...

                    outRec2.Pts = null;
                    outRec2.BottomPt = null;
                    outRec2.Idx = outRec1.Idx;

                    outRec1.IsHole = holeStateRec.IsHole;
                    if (holeStateRec == outRec2)
                        outRec1.FirstLeft = outRec2.FirstLeft;
                    outRec2.FirstLeft = outRec1;

                    //fixup FirstLeft pointers that may need reassigning to OutRec1
                    if (m_UsingPolyTree) FixupFirstLefts2(outRec2, outRec1);
                }
            }
        }
        //------------------------------------------------------------------------------

        private void UpdateOutPtIdxs(OutRec outrec)
        {
            OutPt op = outrec.Pts;
            do
            {
                op.Idx = outrec.Idx;
                op = op.Prev;
            } while (op != outrec.Pts);
        }
        //------------------------------------------------------------------------------

        private void DoSimplePolygons()
        {
            int i = 0;
            while (i < m_PolyOuts.Count)
            {
                OutRec outrec = m_PolyOuts[i++];
                OutPt op = outrec.Pts;
                if (op == null) continue;
                do //for each Pt in Polygon until duplicate found do ...
                {
                    OutPt op2 = op.Next;
                    while (op2 != outrec.Pts)
                    {
                        if ((op.Pt == op2.Pt) && op2.Next != op && op2.Prev != op)
                        {
                            //split the polygon into two ...
                            OutPt op3 = op.Prev;
                            OutPt op4 = op2.Prev;
                            op.Prev = op4;
                            op4.Next = op;
                            op2.Prev = op3;
                            op3.Next = op2;

                            outrec.Pts = op;
                            OutRec outrec2 = CreateOutRec();
                            outrec2.Pts = op2;
                            UpdateOutPtIdxs(outrec2);
                            if (Poly2ContainsPoly1(outrec2.Pts, outrec.Pts, m_UseFullRange))
                            {
                                //OutRec2 is contained by OutRec1 ...
                                outrec2.IsHole = !outrec.IsHole;
                                outrec2.FirstLeft = outrec;
                            }
                            else if (Poly2ContainsPoly1(outrec.Pts, outrec2.Pts, m_UseFullRange))
                            {
                                //OutRec1 is contained by OutRec2 ...
                                outrec2.IsHole = outrec.IsHole;
                                outrec.IsHole = !outrec2.IsHole;
                                outrec2.FirstLeft = outrec.FirstLeft;
                                outrec.FirstLeft = outrec2;
                            }
                            else
                            {
                                //the 2 polygons are separate ...
                                outrec2.IsHole = outrec.IsHole;
                                outrec2.FirstLeft = outrec.FirstLeft;
                            }

                            op2 = op; //ie get ready for the next iteration
                        }

                        op2 = op2.Next;
                    }

                    op = op.Next;
                } while (op != outrec.Pts);
            }
        }
        //------------------------------------------------------------------------------

        public static double Area(List<IntPoint> poly)
        {
            int highI = poly.Count - 1;
            if (highI < 2) return 0;
            double area = ((double) poly[highI].X + poly[0].X) * ((double) poly[0].Y - poly[highI].Y);
            for (int i = 1; i <= highI; ++i)
                area += ((double) poly[i - 1].X + poly[i].X) * ((double) poly[i].Y - poly[i - 1].Y);
            return area / 2;
        }
        //------------------------------------------------------------------------------

        double Area(OutRec outRec)
        {
            OutPt op = outRec.Pts;
            if (op == null) return 0;
            double a = 0;
            do
            {
                a = a + (double) (op.Pt.X + op.Prev.Pt.X) * (double) (op.Prev.Pt.Y - op.Pt.Y);
                op = op.Next;
            } while (op != outRec.Pts);

            return a / 2;
        }

        //------------------------------------------------------------------------------
        // OffsetPolygon functions ...
        //------------------------------------------------------------------------------

        internal static DoublePoint GetUnitNormal(IntPoint pt1, IntPoint pt2)
        {
            double dx = (pt2.X - pt1.X);
            double dy = (pt2.Y - pt1.Y);
            if ((dx == 0) && (dy == 0)) return new DoublePoint();

            double f = 1 * 1.0 / Math.Sqrt(dx * dx + dy * dy);
            dx *= f;
            dy *= f;

            return new DoublePoint(dy, -dx);
        }
        //------------------------------------------------------------------------------

        private class PolyOffsetBuilder
        {
            private List<List<IntPoint>> m_p;
            private List<IntPoint> currentPoly;
            private List<DoublePoint> normals = new List<DoublePoint>();
            private double m_delta, m_sinA, m_sin, m_cos;
            private double m_miterLim, m_Steps360;
            private int m_i, m_j, m_k;
            private const int m_buffLength = 128;

            void OffsetPoint(JoinType jointype)
            {
                m_sinA = (normals[m_k].X * normals[m_j].Y - normals[m_j].X * normals[m_k].Y);
                if (m_sinA > 1.0) m_sinA = 1.0;
                else if (m_sinA < -1.0) m_sinA = -1.0;

                if (m_sinA * m_delta < 0)
                {
                    AddPoint(new IntPoint(Round(m_p[m_i][m_j].X + normals[m_k].X * m_delta),
                        Round(m_p[m_i][m_j].Y + normals[m_k].Y * m_delta)));
                    AddPoint(m_p[m_i][m_j]);
                    AddPoint(new IntPoint(Round(m_p[m_i][m_j].X + normals[m_j].X * m_delta),
                        Round(m_p[m_i][m_j].Y + normals[m_j].Y * m_delta)));
                }
                else
                    switch (jointype)
                    {
                        case JoinType.jtMiter:
                        {
                            double r = 1 + (normals[m_j].X * normals[m_k].X +
                                            normals[m_j].Y * normals[m_k].Y);
                            if (r >= m_miterLim) DoMiter(r);
                            else DoSquare();
                            break;
                        }
                        case JoinType.jtSquare:
                            DoSquare();
                            break;
                        case JoinType.jtRound:
                            DoRound();
                            break;
                    }

                m_k = m_j;
            }
            //------------------------------------------------------------------------------

            public PolyOffsetBuilder(List<List<IntPoint>> pts, out List<List<IntPoint>> solution, double delta,
                JoinType jointype, EndType endtype, double limit = 0)
            {
                //precondition: solution != pts
                solution = new List<List<IntPoint>>();
                if (ClipperBase.near_zero(delta))
                {
                    solution = pts;
                    return;
                }

                m_p = pts;
                if (endtype != EndType.etClosed && delta < 0) delta = -delta;
                m_delta = delta;

                if (jointype == JoinType.jtMiter)
                {
                    //m_miterVal: see offset_triginometry.svg in the documentation folder ...
                    if (limit > 2) m_miterLim = 2 / (limit * limit);
                    else m_miterLim = 0.5;
                    if (endtype == EndType.etRound) limit = 0.25;
                }

                if (jointype == JoinType.jtRound || endtype == EndType.etRound)
                {
                    if (limit <= 0) limit = 0.25;
                    else if (limit > Math.Abs(delta) * 0.25) limit = Math.Abs(delta) * 0.25;
                    //m_roundVal: see offset_triginometry2.svg in the documentation folder ...
                    m_Steps360 = Math.PI / Math.Acos(1 - limit / Math.Abs(delta));
                    m_sin = Math.Sin(2 * Math.PI / m_Steps360);
                    m_cos = Math.Cos(2 * Math.PI / m_Steps360);
                    m_Steps360 /= Math.PI * 2;
                    if (delta < 0) m_sin = -m_sin;
                }

                double deltaSq = delta * delta;
                solution.Capacity = pts.Count;
                for (m_i = 0; m_i < pts.Count; m_i++)
                {
                    int len = pts[m_i].Count;
                    if (len == 0 || (len < 3 && delta <= 0)) continue;

                    if (len == 1)
                    {
                        if (jointype == JoinType.jtRound)
                        {
                            double X = 1.0, Y = 0.0;
                            for (Int64 j = 1; j <= Round(m_Steps360 * 2 * Math.PI); j++)
                            {
                                AddPoint(new IntPoint(
                                    Round(m_p[m_i][0].X + X * delta),
                                    Round(m_p[m_i][0].Y + Y * delta)));
                                double X2 = X;
                                X = X * m_cos - m_sin * Y;
                                Y = X2 * m_sin + Y * m_cos;
                            }
                        }
                        else
                        {
                            double X = -1.0, Y = -1.0;
                            for (int j = 0; j < 4; ++j)
                            {
                                AddPoint(new IntPoint(Round(m_p[m_i][0].X + X * delta),
                                    Round(m_p[m_i][0].Y + Y * delta)));
                                if (X < 0) X = 1;
                                else if (Y < 0) Y = 1;
                                else X = -1;
                            }
                        }

                        continue;
                    }

                    //build normals ...
                    normals.Clear();
                    normals.Capacity = len;
                    for (int j = 0; j < len - 1; ++j)
                        normals.Add(GetUnitNormal(pts[m_i][j], pts[m_i][j + 1]));
                    if (endtype == EndType.etClosed)
                        normals.Add(GetUnitNormal(pts[m_i][len - 1], pts[m_i][0]));
                    else
                        normals.Add(new DoublePoint(normals[len - 2]));

                    currentPoly = new List<IntPoint>();
                    if (endtype == EndType.etClosed)
                    {
                        m_k = len - 1;
                        for (m_j = 0; m_j < len; ++m_j)
                            OffsetPoint(jointype);
                        solution.Add(currentPoly);
                    }
                    else
                    {
                        m_k = 0;
                        for (m_j = 1; m_j < len - 1; ++m_j)
                            OffsetPoint(jointype);

                        IntPoint pt1;
                        if (endtype == EndType.etButt)
                        {
                            m_j = len - 1;
                            pt1 = new IntPoint((Int64) Round(pts[m_i][m_j].X + normals[m_j].X *
                                    delta),
                                (Int64) Round(pts[m_i][m_j].Y + normals[m_j].Y * delta));
                            AddPoint(pt1);
                            pt1 = new IntPoint((Int64) Round(pts[m_i][m_j].X - normals[m_j].X *
                                    delta),
                                (Int64) Round(pts[m_i][m_j].Y - normals[m_j].Y * delta));
                            AddPoint(pt1);
                        }
                        else
                        {
                            m_j = len - 1;
                            m_k = len - 2;
                            m_sinA = 0;
                            normals[m_j].X = -normals[m_j].X;
                            normals[m_j].Y = -normals[m_j].Y;
                            if (endtype == EndType.etSquare)
                                DoSquare();
                            else
                                DoRound();
                        }

                        //re-build Normals ...
                        for (int j = len - 1; j > 0; j--)
                        {
                            normals[j].X = -normals[j - 1].X;
                            normals[j].Y = -normals[j - 1].Y;
                        }

                        normals[0].X = -normals[1].X;
                        normals[0].Y = -normals[1].Y;

                        m_k = len - 1;
                        for (m_j = m_k - 1; m_j > 0; --m_j)
                            OffsetPoint(jointype);

                        if (endtype == EndType.etButt)
                        {
                            pt1 = new IntPoint((Int64) Round(pts[m_i][0].X - normals[0].X * delta),
                                (Int64) Round(pts[m_i][0].Y - normals[0].Y * delta));
                            AddPoint(pt1);
                            pt1 = new IntPoint((Int64) Round(pts[m_i][0].X + normals[0].X * delta),
                                (Int64) Round(pts[m_i][0].Y + normals[0].Y * delta));
                            AddPoint(pt1);
                        }
                        else
                        {
                            m_k = 1;
                            m_sinA = 0;
                            if (endtype == EndType.etSquare)
                                DoSquare();
                            else
                                DoRound();
                        }

                        solution.Add(currentPoly);
                    }
                }

                //finally, clean up untidy corners ...
                Clipper clpr = new Clipper();
                clpr.AddPaths(solution, PolyType.ptSubject, true);
                if (delta > 0)
                {
                    clpr.Execute(ClipType.ctUnion, solution, PolyFillType.pftPositive, PolyFillType.pftPositive);
                }
                else
                {
                    IntRect r = clpr.GetBounds();
                    List<IntPoint> outer = new List<IntPoint>(4);

                    outer.Add(new IntPoint(r.left - 10, r.bottom + 10));
                    outer.Add(new IntPoint(r.right + 10, r.bottom + 10));
                    outer.Add(new IntPoint(r.right + 10, r.top - 10));
                    outer.Add(new IntPoint(r.left - 10, r.top - 10));

                    clpr.AddPath(outer, PolyType.ptSubject, true);
                    clpr.ReverseSolution = true;
                    clpr.Execute(ClipType.ctUnion, solution, PolyFillType.pftNegative, PolyFillType.pftNegative);
                    if (solution.Count > 0) solution.RemoveAt(0);
                }
            }
            //------------------------------------------------------------------------------

            internal void AddPoint(IntPoint pt)
            {
                if (currentPoly.Count == currentPoly.Capacity)
                    currentPoly.Capacity += m_buffLength;
                currentPoly.Add(pt);
            }
            //------------------------------------------------------------------------------

            internal void DoSquare()
            {
                double dx = Math.Tan(Math.Atan2(m_sinA,
                    normals[m_k].X * normals[m_j].X + normals[m_k].Y * normals[m_j].Y) / 4);
                AddPoint(new IntPoint(
                    Round(m_p[m_i][m_j].X + m_delta * (normals[m_k].X - normals[m_k].Y * dx)),
                    Round(m_p[m_i][m_j].Y + m_delta * (normals[m_k].Y + normals[m_k].X * dx))));
                AddPoint(new IntPoint(
                    Round(m_p[m_i][m_j].X + m_delta * (normals[m_j].X + normals[m_j].Y * dx)),
                    Round(m_p[m_i][m_j].Y + m_delta * (normals[m_j].Y - normals[m_j].X * dx))));
            }
            //------------------------------------------------------------------------------

            internal void DoMiter(double r)
            {
                double q = m_delta / r;
                AddPoint(new IntPoint(Round(m_p[m_i][m_j].X + (normals[m_k].X + normals[m_j].X) * q),
                    Round(m_p[m_i][m_j].Y + (normals[m_k].Y + normals[m_j].Y) * q)));
            }
            //------------------------------------------------------------------------------

            internal void DoRound()
            {
                double a = Math.Atan2(m_sinA,
                    normals[m_k].X * normals[m_j].X + normals[m_k].Y * normals[m_j].Y);
                int steps = (int) Round(m_Steps360 * Math.Abs(a));

                double X = normals[m_k].X, Y = normals[m_k].Y, X2;
                for (int i = 0; i < steps; ++i)
                {
                    AddPoint(new IntPoint(
                        Round(m_p[m_i][m_j].X + X * m_delta),
                        Round(m_p[m_i][m_j].Y + Y * m_delta)));
                    X2 = X;
                    X = X * m_cos - m_sin * Y;
                    Y = X2 * m_sin + Y * m_cos;
                }

                AddPoint(new IntPoint(
                    Round(m_p[m_i][m_j].X + normals[m_j].X * m_delta),
                    Round(m_p[m_i][m_j].Y + normals[m_j].Y * m_delta)));
            }

            //------------------------------------------------------------------------------
        } //end PolyOffsetBuilder
        //------------------------------------------------------------------------------

        internal static bool UpdateBotPt(IntPoint pt, ref IntPoint botPt)
        {
            if (pt.Y > botPt.Y || (pt.Y == botPt.Y && pt.X < botPt.X))
            {
                botPt = pt;
                return true;
            }
            else return false;
        }
        //------------------------------------------------------------------------------

        internal static bool StripDupsAndGetBotPt(List<IntPoint> in_path,
            List<IntPoint> out_path, bool closed, out IntPoint botPt)
        {
            botPt = new IntPoint(0, 0);
            int len = in_path.Count;
            if (closed)
                while (len > 0 && (in_path[0] == in_path[len - 1]))
                    len--;
            if (len == 0) return false;
            out_path.Capacity = len;
            int j = 0;
            out_path.Add(in_path[0]);
            botPt = in_path[0];
            for (int i = 1; i < len; ++i)
                if (in_path[i] != out_path[j])
                {
                    out_path.Add(in_path[i]);
                    j++;
                    if (out_path[j].Y > botPt.Y ||
                        ((out_path[j].Y == botPt.Y) && out_path[j].X < botPt.X))
                        botPt = out_path[j];
                }

            j++;
            if (j < 2 || (closed && (j == 2))) j = 0;
            while (out_path.Count > j) out_path.RemoveAt(j);
            return j > 0;
        }
        //------------------------------------------------------------------------------

        public static List<List<IntPoint>> OffsetPaths(List<List<IntPoint>> polys, double delta,
            JoinType jointype, EndType endtype, double MiterLimit)
        {
            List<List<IntPoint>> out_polys = new List<List<IntPoint>>(polys.Count);
            IntPoint botPt = new IntPoint();
            IntPoint pt;
            int botIdx = -1;
            for (int i = 0; i < polys.Count; ++i)
            {
                out_polys.Add(new List<IntPoint>());
                if (StripDupsAndGetBotPt(polys[i], out_polys[i], endtype == EndType.etClosed, out pt))
                    if (botIdx < 0 || pt.Y > botPt.Y || (pt.Y == botPt.Y && pt.X < botPt.X))
                    {
                        botPt = pt;
                        botIdx = i;
                    }
            }

            if (endtype == EndType.etClosed && botIdx >= 0 && !Orientation(out_polys[botIdx]))
                ReversePaths(out_polys);

            List<List<IntPoint>> result;
            new PolyOffsetBuilder(out_polys, out result, delta, jointype, endtype, MiterLimit);
            return result;
        }
        //------------------------------------------------------------------------------

#if use_deprecated
        public static List<List<IntPoint>> OffsetPolygons(List<List<IntPoint>> poly, double delta,
            JoinType jointype, double MiterLimit, bool AutoFix)
        {
            return OffsetPaths(poly, delta, jointype, EndType.etClosed, MiterLimit);
        }
        //------------------------------------------------------------------------------

        public static List<List<IntPoint>> OffsetPolygons(List<List<IntPoint>> poly, double delta,
            JoinType jointype, double MiterLimit)
        {
            return OffsetPaths(poly, delta, jointype, EndType.etClosed, MiterLimit);
        }
        //------------------------------------------------------------------------------

        public static List<List<IntPoint>> OffsetPolygons(List<List<IntPoint>> polys, double delta, JoinType jointype)
        {
            return OffsetPaths(polys, delta, jointype, EndType.etClosed, 0);
        }
        //------------------------------------------------------------------------------

        public static List<List<IntPoint>> OffsetPolygons(List<List<IntPoint>> polys, double delta)
        {
            return OffsetPolygons(polys, delta, JoinType.jtSquare, 0, true);
        }
        //------------------------------------------------------------------------------

        public static void ReversePolygons(List<List<IntPoint>> polys)
        {
            polys.ForEach(delegate (List<IntPoint> poly) { poly.Reverse(); });
        }
        //------------------------------------------------------------------------------

        public static void PolyTreeToPolygons(PolyTree polytree, List<List<IntPoint>> polys)
        {
            polys.Clear();
            polys.Capacity = polytree.Total;
            AddPolyNodeToPaths(polytree, NodeType.ntAny, polys);
        }
        //------------------------------------------------------------------------------
#endif

        //------------------------------------------------------------------------------
        // SimplifyPolygon functions ...
        // Convert self-intersecting polygons into simple polygons
        //------------------------------------------------------------------------------

        public static List<List<IntPoint>> SimplifyPolygon(List<IntPoint> poly,
            PolyFillType fillType = PolyFillType.pftEvenOdd)
        {
            List<List<IntPoint>> result = new List<List<IntPoint>>();
            Clipper c = new Clipper();
            c.StrictlySimple = true;
            c.AddPath(poly, PolyType.ptSubject, true);
            c.Execute(ClipType.ctUnion, result, fillType, fillType);
            return result;
        }
        //------------------------------------------------------------------------------

        public static List<List<IntPoint>> SimplifyPolygons(List<List<IntPoint>> polys,
            PolyFillType fillType = PolyFillType.pftEvenOdd)
        {
            List<List<IntPoint>> result = new List<List<IntPoint>>();
            Clipper c = new Clipper();
            c.StrictlySimple = true;
            c.AddPaths(polys, PolyType.ptSubject, true);
            c.Execute(ClipType.ctUnion, result, fillType, fillType);
            return result;
        }
        //------------------------------------------------------------------------------

        private static double DistanceSqrd(IntPoint pt1, IntPoint pt2)
        {
            double dx = ((double) pt1.X - pt2.X);
            double dy = ((double) pt1.Y - pt2.Y);
            return (dx * dx + dy * dy);
        }
        //------------------------------------------------------------------------------

        private static DoublePoint ClosestPointOnLine(IntPoint pt, IntPoint linePt1, IntPoint linePt2)
        {
            double dx = ((double) linePt2.X - linePt1.X);
            double dy = ((double) linePt2.Y - linePt1.Y);
            if (dx == 0 && dy == 0)
                return new DoublePoint(linePt1.X, linePt1.Y);
            double q = ((pt.X - linePt1.X) * dx + (pt.Y - linePt1.Y) * dy) / (dx * dx + dy * dy);
            return new DoublePoint(
                (1 - q) * linePt1.X + q * linePt2.X,
                (1 - q) * linePt1.Y + q * linePt2.Y);
        }
        //------------------------------------------------------------------------------

        private static bool SlopesNearCollinear(IntPoint pt1,
            IntPoint pt2, IntPoint pt3, double distSqrd)
        {
            if (DistanceSqrd(pt1, pt2) > DistanceSqrd(pt1, pt3)) return false;
            DoublePoint cpol = ClosestPointOnLine(pt2, pt1, pt3);
            double dx = pt2.X - cpol.X;
            double dy = pt2.Y - cpol.Y;
            return (dx * dx + dy * dy) < distSqrd;
        }
        //------------------------------------------------------------------------------

        private static bool PointsAreClose(IntPoint pt1, IntPoint pt2, double distSqrd)
        {
            double dx = (double) pt1.X - pt2.X;
            double dy = (double) pt1.Y - pt2.Y;
            return ((dx * dx) + (dy * dy) <= distSqrd);
        }
        //------------------------------------------------------------------------------

        public static List<IntPoint> CleanPolygon(List<IntPoint> path,
            double distance = 1.415)
        {
            //distance = proximity in units/pixels below which vertices
            //will be stripped. Default ~= sqrt(2) so when adjacent
            //vertices have both x & y coords within 1 unit, then
            //the second vertex will be stripped.
            double distSqrd = (distance * distance);
            int highI = path.Count - 1;
            List<IntPoint> result = new List<IntPoint>(highI + 1);
            while (highI > 0 && PointsAreClose(path[highI], path[0], distSqrd)) highI--;
            if (highI < 2) return result;
            IntPoint pt = path[highI];
            int i = 0;
            for (;;)
            {
                while (i < highI && PointsAreClose(pt, path[i], distSqrd)) i += 2;
                int i2 = i;
                while (i < highI && (PointsAreClose(path[i], path[i + 1], distSqrd) ||
                                     SlopesNearCollinear(pt, path[i], path[i + 1], distSqrd))) i++;
                if (i >= highI) break;
                else if (i != i2) continue;
                pt = path[i++];
                result.Add(pt);
            }

            if (i <= highI) result.Add(path[i]);
            i = result.Count;
            if (i > 2 && SlopesNearCollinear(result[i - 2], result[i - 1], result[0], distSqrd))
                result.RemoveAt(i - 1);
            if (result.Count < 3) result.Clear();
            return result;
        }
        //------------------------------------------------------------------------------

        public static List<List<IntPoint>> CleanPolygons(List<List<IntPoint>> polys,
            double distance = 1.415)
        {
            List<List<IntPoint>> result = new List<List<IntPoint>>(polys.Count);
            for (int i = 0; i < polys.Count; i++)
                result.Add(CleanPolygon(polys[i], distance));
            return result;
        }
        //------------------------------------------------------------------------------

        internal enum NodeType
        {
            ntAny,
            ntOpen,
            ntClosed
        };

        public static List<List<IntPoint>> PolyTreeToPaths(PolyTree polytree)
        {
            List<List<IntPoint>> result = new List<List<IntPoint>>();
            result.Capacity = polytree.Total;
            AddPolyNodeToPaths(polytree, NodeType.ntAny, result);
            return result;
        }
        //------------------------------------------------------------------------------

        internal static void AddPolyNodeToPaths(PolyNode polynode, NodeType nt, List<List<IntPoint>> paths)
        {
            bool match = true;
            switch (nt)
            {
                case NodeType.ntOpen: return;
                case NodeType.ntClosed:
                    match = !polynode.IsOpen;
                    break;
                default: break;
            }

            if (polynode.Contour.Count > 0 && match)
                paths.Add(polynode.Contour);
            foreach (PolyNode pn in polynode.Childs)
                AddPolyNodeToPaths(pn, nt, paths);
        }
        //------------------------------------------------------------------------------

        public static List<List<IntPoint>> OpenPathsFromPolyTree(PolyTree polytree)
        {
            List<List<IntPoint>> result = new List<List<IntPoint>>();
            result.Capacity = polytree.ChildCount;
            for (int i = 0; i < polytree.ChildCount; i++)
                if (polytree.Childs[i].IsOpen)
                    result.Add(polytree.Childs[i].Contour);
            return result;
        }
        //------------------------------------------------------------------------------

        public static List<List<IntPoint>> ClosedPathsFromPolyTree(PolyTree polytree)
        {
            List<List<IntPoint>> result = new List<List<IntPoint>>();
            result.Capacity = polytree.Total;
            AddPolyNodeToPaths(polytree, NodeType.ntClosed, result);
            return result;
        }

        //------------------------------------------------------------------------------
    }
}