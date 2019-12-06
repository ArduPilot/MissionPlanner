using System;
using System.Collections.Generic;

namespace ClipperLib
{
    internal class ClipperBase
    {
        protected const double horizontal = -3.4E+38;
        protected const int Skip = -2;
        protected const int Unassigned = -1;
        protected const double tolerance = 1.0E-20;
        internal static bool near_zero(double val) { return (val > -tolerance) && (val < tolerance); }

#if use_int32
      internal const cInt loRange = 46340;
      internal const cInt hiRange = 46340;
#else
        internal const Int64 loRange = 0x3FFFFFFF;
        internal const Int64 hiRange = 0x3FFFFFFFFFFFFFFFL;
#endif

        internal LocalMinima m_MinimaList;
        internal LocalMinima m_CurrentLM;
        internal List<List<TEdge>> m_edges = new List<List<TEdge>>();
        internal bool m_UseFullRange;
        internal bool m_HasOpenPaths;

        //------------------------------------------------------------------------------

        public bool PreserveCollinear
        {
            get;
            set;
        }
        //------------------------------------------------------------------------------

        internal static bool IsHorizontal(TEdge e)
        {
            return e.Delta.Y == 0;
        }
        //------------------------------------------------------------------------------

        internal bool PointIsVertex(IntPoint pt, OutPt pp)
        {
            OutPt pp2 = pp;
            do
            {
                if (pp2.Pt == pt) return true;
                pp2 = pp2.Next;
            }
            while (pp2 != pp);
            return false;
        }
        //------------------------------------------------------------------------------

        internal bool PointOnLineSegment(IntPoint pt,
            IntPoint linePt1, IntPoint linePt2, bool UseFullInt64Range)
        {
            if (UseFullInt64Range)
                return ((pt.X == linePt1.X) && (pt.Y == linePt1.Y)) ||
                       ((pt.X == linePt2.X) && (pt.Y == linePt2.Y)) ||
                       (((pt.X > linePt1.X) == (pt.X < linePt2.X)) &&
                        ((pt.Y > linePt1.Y) == (pt.Y < linePt2.Y)) &&
                        ((Int128.Int128Mul((pt.X - linePt1.X), (linePt2.Y - linePt1.Y)) ==
                          Int128.Int128Mul((linePt2.X - linePt1.X), (pt.Y - linePt1.Y)))));
            else
                return ((pt.X == linePt1.X) && (pt.Y == linePt1.Y)) ||
                       ((pt.X == linePt2.X) && (pt.Y == linePt2.Y)) ||
                       (((pt.X > linePt1.X) == (pt.X < linePt2.X)) &&
                        ((pt.Y > linePt1.Y) == (pt.Y < linePt2.Y)) &&
                        ((pt.X - linePt1.X) * (linePt2.Y - linePt1.Y) ==
                         (linePt2.X - linePt1.X) * (pt.Y - linePt1.Y)));
        }
        //------------------------------------------------------------------------------

        internal bool PointOnPolygon(IntPoint pt, OutPt pp, bool UseFullInt64Range)
        {
            OutPt pp2 = pp;
            while (true)
            {
                if (PointOnLineSegment(pt, pp2.Pt, pp2.Next.Pt, UseFullInt64Range))
                    return true;
                pp2 = pp2.Next;
                if (pp2 == pp) break;
            }
            return false;
        }
        //------------------------------------------------------------------------------

        internal bool PointInPolygon(IntPoint pt, OutPt pp, bool UseFulllongRange)
        {
            OutPt pp2 = pp;
            bool result = false;
            if (UseFulllongRange)
            {
                do
                {
                    if ((((pp2.Pt.Y <= pt.Y) && (pt.Y < pp2.Prev.Pt.Y)) ||
                         ((pp2.Prev.Pt.Y <= pt.Y) && (pt.Y < pp2.Pt.Y))) &&
                        new Int128(pt.X - pp2.Pt.X) <
                        Int128.Int128Mul(pp2.Prev.Pt.X - pp2.Pt.X, pt.Y - pp2.Pt.Y) /
                        new Int128(pp2.Prev.Pt.Y - pp2.Pt.Y))
                        result = !result;
                    pp2 = pp2.Next;
                }
                while (pp2 != pp);
            }
            else
            {
                do
                {
                    if ((((pp2.Pt.Y <= pt.Y) && (pt.Y < pp2.Prev.Pt.Y)) ||
                         ((pp2.Prev.Pt.Y <= pt.Y) && (pt.Y < pp2.Pt.Y))) &&
                        (pt.X - pp2.Pt.X < (pp2.Prev.Pt.X - pp2.Pt.X) * (pt.Y - pp2.Pt.Y) /
                         (pp2.Prev.Pt.Y - pp2.Pt.Y))) result = !result;
                    pp2 = pp2.Next;
                }
                while (pp2 != pp);
            }
            return result;
        }
        //------------------------------------------------------------------------------

        internal static bool SlopesEqual(TEdge e1, TEdge e2, bool UseFullRange)
        {
            if (UseFullRange)
                return Int128.Int128Mul(e1.Delta.Y, e2.Delta.X) ==
                       Int128.Int128Mul(e1.Delta.X, e2.Delta.Y);
            else return (Int64)(e1.Delta.Y) * (e2.Delta.X) ==
                        (Int64)(e1.Delta.X) * (e2.Delta.Y);
        }
        //------------------------------------------------------------------------------

        protected static bool SlopesEqual(IntPoint pt1, IntPoint pt2,
            IntPoint pt3, bool UseFullRange)
        {
            if (UseFullRange)
                return Int128.Int128Mul(pt1.Y - pt2.Y, pt2.X - pt3.X) ==
                       Int128.Int128Mul(pt1.X - pt2.X, pt2.Y - pt3.Y);
            else return
                (Int64)(pt1.Y - pt2.Y) * (pt2.X - pt3.X) - (Int64)(pt1.X - pt2.X) * (pt2.Y - pt3.Y) == 0;
        }
        //------------------------------------------------------------------------------

        protected static bool SlopesEqual(IntPoint pt1, IntPoint pt2,
            IntPoint pt3, IntPoint pt4, bool UseFullRange)
        {
            if (UseFullRange)
                return Int128.Int128Mul(pt1.Y - pt2.Y, pt3.X - pt4.X) ==
                       Int128.Int128Mul(pt1.X - pt2.X, pt3.Y - pt4.Y);
            else return
                (Int64)(pt1.Y - pt2.Y) * (pt3.X - pt4.X) - (Int64)(pt1.X - pt2.X) * (pt3.Y - pt4.Y) == 0;
        }
        //------------------------------------------------------------------------------

        internal ClipperBase() //constructor (nb: no external instantiation)
        {
            m_MinimaList = null;
            m_CurrentLM = null;
            m_UseFullRange = false;
            m_HasOpenPaths = false;
        }
        //------------------------------------------------------------------------------

        public virtual void Clear()
        {
            DisposeLocalMinimaList();
            for (int i = 0; i < m_edges.Count; ++i)
            {
                for (int j = 0; j < m_edges[i].Count; ++j) m_edges[i][j] = null;
                m_edges[i].Clear();
            }
            m_edges.Clear();
            m_UseFullRange = false;
            m_HasOpenPaths = false;
        }
        //------------------------------------------------------------------------------

        private void DisposeLocalMinimaList()
        {
            while (m_MinimaList != null)
            {
                LocalMinima tmpLm = m_MinimaList.Next;
                m_MinimaList = null;
                m_MinimaList = tmpLm;
            }
            m_CurrentLM = null;
        }
        //------------------------------------------------------------------------------

        void RangeTest(IntPoint Pt, ref bool useFullRange)
        {
            if (useFullRange)
            {
                if (Pt.X > hiRange || Pt.Y > hiRange || -Pt.X > hiRange || -Pt.Y > hiRange)
                    throw new ClipperException("Coordinate outside allowed range");
            }
            else if (Pt.X > loRange || Pt.Y > loRange || -Pt.X > loRange || -Pt.Y > loRange)
            {
                useFullRange = true;
                RangeTest(Pt, ref useFullRange);
            }
        }
        //------------------------------------------------------------------------------

        private void InitEdge(TEdge e, TEdge eNext,
            TEdge ePrev, IntPoint pt)
        {
            e.Next = eNext;
            e.Prev = ePrev;
            e.Curr = pt;
            e.OutIdx = Unassigned;
        }
        //------------------------------------------------------------------------------

        private void InitEdge2(TEdge e, PolyType polyType)
        {
            if (e.Curr.Y >= e.Next.Curr.Y)
            {
                e.Bot = e.Curr;
                e.Top = e.Next.Curr;
            }
            else
            {
                e.Top = e.Curr;
                e.Bot = e.Next.Curr;
            }
            SetDx(e);
            e.PolyTyp = polyType;
        }
        //------------------------------------------------------------------------------

        public bool AddPath(List<IntPoint> pg, PolyType polyType, bool Closed)
        {
#if use_lines
        if (!Closed && polyType == PolyType.ptClip)
          throw new ClipperException("AddPath: Open paths must be subject.");
#else
            if (!Closed)
                throw new ClipperException("AddPath: Open paths have been disabled.");
#endif

            int highI = (int)pg.Count - 1;
            bool ClosedOrSemiClosed = (highI > 0) && (Closed || (pg[0] == pg[highI]));
            while (highI > 0 && (pg[highI] == pg[0])) --highI;
            while (highI > 0 && (pg[highI] == pg[highI - 1])) --highI;
            if ((Closed && highI < 2) || (!Closed && highI < 1)) return false;

            //create a new edge array ...
            List<TEdge> edges = new List<TEdge>(highI + 1);
            for (int i = 0; i <= highI; i++) edges.Add(new TEdge());

            //1. Basic initialization of Edges ...
            try
            {
                edges[1].Curr = pg[1];
                RangeTest(pg[0], ref m_UseFullRange);
                RangeTest(pg[highI], ref m_UseFullRange);
                InitEdge(edges[0], edges[1], edges[highI], pg[0]);
                InitEdge(edges[highI], edges[0], edges[highI - 1], pg[highI]);
                for (int i = highI - 1; i >= 1; --i)
                {
                    RangeTest(pg[i], ref m_UseFullRange);
                    InitEdge(edges[i], edges[i + 1], edges[i - 1], pg[i]);
                }
            }
            catch
            {
                return false; //almost certainly a vertex has exceeded range
            };

            TEdge eStart = edges[0];
            if (!ClosedOrSemiClosed) eStart.Prev.OutIdx = Skip;

            //2. Remove duplicate vertices, and collinear edges (when closed) ...
            TEdge E = eStart, eLoopStop = eStart;
            for (; ; )
            {
                if (E.Curr == E.Next.Curr)
                {
                    //nb if E.OutIdx == Skip, it would have been semiOpen
                    if (E == eStart) eStart = E.Next;
                    E = RemoveEdge(E);
                    eLoopStop = E;
                    continue;
                }
                if (E.Prev == E.Next)
                    break; //only two vertices
                else if ((ClosedOrSemiClosed ||
                          (E.Prev.OutIdx != Skip && E.OutIdx != Skip &&
                           E.Next.OutIdx != Skip)) &&
                         SlopesEqual(E.Prev.Curr, E.Curr, E.Next.Curr, m_UseFullRange))
                {
                    //All collinear edges are allowed for open paths but in closed paths
                    //inner vertices of adjacent collinear edges are removed. However if the
                    //PreserveCollinear property has been enabled, only overlapping collinear
                    //edges (ie spikes) are removed from closed paths.
                    if (Closed && (!PreserveCollinear ||
                                   !Pt2IsBetweenPt1AndPt3(E.Prev.Curr, E.Curr, E.Next.Curr)))
                    {
                        if (E == eStart) eStart = E.Next;
                        E = RemoveEdge(E);
                        E = E.Prev;
                        eLoopStop = E;
                        continue;
                    }
                }
                E = E.Next;
                if (E == eLoopStop) break;
            }

            if ((!Closed && (E == E.Next)) || (Closed && (E.Prev == E.Next)))
                return false;
            m_edges.Add(edges);

            if (!Closed)
                m_HasOpenPaths = true;

            //3. Do final Init and also find the 'highest' Edge. (nb: since I'm much
            //more familiar with positive downwards Y axes, 'highest' here will be
            //the Edge with the *smallest* Top.Y.)
            TEdge eHighest = eStart;
            E = eStart;
            do
            {
                InitEdge2(E, polyType);
                if (E.Top.Y < eHighest.Top.Y) eHighest = E;
                E = E.Next;
            }
            while (E != eStart);

            //4. build the local minima list ...
            if (AllHorizontal(E))
            {
                if (ClosedOrSemiClosed)
                    E.Prev.OutIdx = Skip;
                AscendToMax(ref E, false, false);
                return true;
            }

            //if eHighest is also the Skip then it's a natural break, otherwise
            //make sure eHighest is positioned so we're either at a top horizontal or
            //just starting to head down one edge of the polygon
            E = eStart.Prev; //EStart.Prev == Skip edge
            if (E.Prev == E.Next)
                eHighest = E.Next;
            else if (!ClosedOrSemiClosed && E.Top.Y == eHighest.Top.Y)
            {
                if ((IsHorizontal(E) || IsHorizontal(E.Next)) &&
                    E.Next.Bot.Y == eHighest.Top.Y)
                    eHighest = E.Next;
                else if (SharedVertWithPrevAtTop(E)) eHighest = E;
                else if (E.Top == E.Prev.Top) eHighest = E.Prev;
                else eHighest = E.Next;
            }
            else
            {
                E = eHighest;
                while (IsHorizontal(eHighest) ||
                       (eHighest.Top == eHighest.Next.Top) ||
                       (eHighest.Top == eHighest.Next.Bot)) //next is high horizontal
                {
                    eHighest = eHighest.Next;
                    if (eHighest == E)
                    {
                        while (IsHorizontal(eHighest) || !SharedVertWithPrevAtTop(eHighest))
                            eHighest = eHighest.Next;
                        break; //avoids potential endless loop
                    }
                }
            }
            E = eHighest;
            do
                E = AddBoundsToLML(E, Closed);
            while (E != eHighest);
            return true;
        }
        //------------------------------------------------------------------------------

        public bool AddPaths(List<List<IntPoint>> ppg, PolyType polyType, bool closed)
        {
            bool result = false;
            for (int i = 0; i < ppg.Count; ++i)
                if (AddPath(ppg[i], polyType, closed)) result = true;
            return result;
        }
        //------------------------------------------------------------------------------

#if use_deprecated
        public bool AddPolygon(List<IntPoint> pg, PolyType polyType)
        {
            return AddPath(pg, polyType, true);
        }
        //------------------------------------------------------------------------------

        public bool AddPolygons(List<List<IntPoint>> ppg, PolyType polyType)
        {
            bool result = false;
            for (int i = 0; i < ppg.Count; ++i)
                if (AddPath(ppg[i], polyType, true)) result = true;
            return result;
        }
        //------------------------------------------------------------------------------
#endif

        internal bool Pt2IsBetweenPt1AndPt3(IntPoint pt1, IntPoint pt2, IntPoint pt3)
        {
            if ((pt1 == pt3) || (pt1 == pt2) || (pt3 == pt2)) return false;
            else if (pt1.X != pt3.X) return (pt2.X > pt1.X) == (pt2.X < pt3.X);
            else return (pt2.Y > pt1.Y) == (pt2.Y < pt3.Y);
        }
        //------------------------------------------------------------------------------

        TEdge RemoveEdge(TEdge e)
        {
            //removes e from double_linked_list (but without removing from memory)
            e.Prev.Next = e.Next;
            e.Next.Prev = e.Prev;
            TEdge result = e.Next;
            e.Prev = null; //flag as removed (see ClipperBase.Clear)
            return result;
        }
        //------------------------------------------------------------------------------

        TEdge GetLastHorz(TEdge Edge)
        {
            TEdge result = Edge;
            while (result.OutIdx != Skip && result.Next != Edge && IsHorizontal(result.Next))
                result = result.Next;
            return result;
        }
        //------------------------------------------------------------------------------

        bool SharedVertWithPrevAtTop(TEdge Edge)
        {
            TEdge E = Edge;
            bool result = true;
            while (E.Prev != Edge)
            {
                if (E.Top == E.Prev.Top)
                {
                    if (E.Bot == E.Prev.Bot)
                    { E = E.Prev; continue; }
                    else result = true;
                }
                else result = false;
                break;
            }
            while (E != Edge)
            {
                result = !result;
                E = E.Next;
            }
            return result;
        }
        //------------------------------------------------------------------------------

        bool SharedVertWithNextIsBot(TEdge Edge)
        {
            bool result = true;
            TEdge E = Edge;
            while (E.Prev != Edge)
            {
                bool A = (E.Next.Bot == E.Bot);
                bool B = (E.Prev.Bot == E.Bot);
                if (A != B)
                {
                    result = A;
                    break;
                }
                A = (E.Next.Top == E.Top);
                B = (E.Prev.Top == E.Top);
                if (A != B)
                {
                    result = B;
                    break;
                }
                E = E.Prev;
            }
            while (E != Edge)
            {
                result = !result;
                E = E.Next;
            }
            return result;
        }
        //------------------------------------------------------------------------------

        bool MoreBelow(TEdge Edge)
        {
            //Edge is Skip heading down.
            TEdge E = Edge;
            if (IsHorizontal(E))
            {
                while (IsHorizontal(E.Next)) E = E.Next;
                return E.Next.Bot.Y > E.Bot.Y;
            }
            else if (IsHorizontal(E.Next))
            {
                while (IsHorizontal(E.Next)) E = E.Next;
                return E.Next.Bot.Y > E.Bot.Y;
            }
            else return (E.Bot == E.Next.Top);
        }
        //------------------------------------------------------------------------------

        bool JustBeforeLocMin(TEdge Edge)
        {
            //Edge is Skip and was heading down.
            TEdge E = Edge;
            if (IsHorizontal(E))
            {
                while (IsHorizontal(E.Next)) E = E.Next;
                return E.Next.Top.Y < E.Bot.Y;
            }
            else return SharedVertWithNextIsBot(E);
        }
        //------------------------------------------------------------------------------

        bool MoreAbove(TEdge Edge)
        {
            if (IsHorizontal(Edge))
            {
                Edge = GetLastHorz(Edge);
                return (Edge.Next.Top.Y < Edge.Top.Y);
            }
            else if (IsHorizontal(Edge.Next))
            {
                Edge = GetLastHorz(Edge.Next);
                return (Edge.Next.Top.Y < Edge.Top.Y);
            }
            else
                return (Edge.Next.Top.Y < Edge.Top.Y);
        }
        //------------------------------------------------------------------------------

        bool AllHorizontal(TEdge Edge)
        {
            if (!IsHorizontal(Edge)) return false;
            TEdge E = Edge.Next;
            while (E != Edge)
            {
                if (!IsHorizontal(E)) return false;
                else E = E.Next;
            }
            return true;
        }
        //------------------------------------------------------------------------------

        private void SetDx(TEdge e)
        {
            e.Delta.X = (e.Top.X - e.Bot.X);
            e.Delta.Y = (e.Top.Y - e.Bot.Y);
            if (e.Delta.Y == 0) e.Dx = horizontal;
            else e.Dx = (double)(e.Delta.X) / (e.Delta.Y);
        }
        //---------------------------------------------------------------------------

        void DoMinimaLML(TEdge E1, TEdge E2, bool IsClosed)
        {
            if (E1 == null)
            {
                if (E2 == null) return;
                LocalMinima NewLm = new LocalMinima();
                NewLm.Next = null;
                NewLm.Y = E2.Bot.Y;
                NewLm.LeftBound = null;
                E2.WindDelta = 0;
                NewLm.RightBound = E2;
                InsertLocalMinima(NewLm);
            }
            else
            {
                //E and E.Prev are now at a local minima ...
                LocalMinima NewLm = new LocalMinima();
                NewLm.Y = E1.Bot.Y;
                NewLm.Next = null;
                if (IsHorizontal(E2)) //Horz. edges never start a Left bound
                {
                    if (E2.Bot.X != E1.Bot.X) ReverseHorizontal(E2);
                    NewLm.LeftBound = E1;
                    NewLm.RightBound = E2;
                }
                else if (E2.Dx < E1.Dx)
                {
                    NewLm.LeftBound = E1;
                    NewLm.RightBound = E2;
                }
                else
                {
                    NewLm.LeftBound = E2;
                    NewLm.RightBound = E1;
                }
                NewLm.LeftBound.Side = EdgeSide.esLeft;
                NewLm.RightBound.Side = EdgeSide.esRight;
                //set the winding state of the first edge in each bound
                //(it'll be copied to subsequent edges in the bound) ...
                if (!IsClosed) NewLm.LeftBound.WindDelta = 0;
                else if (NewLm.LeftBound.Next == NewLm.RightBound) NewLm.LeftBound.WindDelta = -1;
                else NewLm.LeftBound.WindDelta = 1;
                NewLm.RightBound.WindDelta = -NewLm.LeftBound.WindDelta;
                InsertLocalMinima(NewLm);
            }
        }
        //----------------------------------------------------------------------

        TEdge DescendToMin(ref TEdge E)
        {
            //PRECONDITION: STARTING EDGE IS A VALID DESCENDING EDGE.
            //Starting at the top of one bound we progress to the bottom where there's
            //A local minima. We  go to the top of the Next bound. These two bounds
            //form the left and right (or right and left) bounds of the local minima.
            TEdge EHorz;
            E.NextInLML = null;
            if (IsHorizontal(E))
            {
                EHorz = E;
                while (IsHorizontal(EHorz.Next)) EHorz = EHorz.Next;
                if (EHorz.Bot != EHorz.Next.Top)
                    ReverseHorizontal(E);
            }
            for (; ; )
            {
                E = E.Next;
                if (E.OutIdx == Skip) break;
                else if (IsHorizontal(E))
                {
                    //nb: proceed through horizontals when approaching from their right,
                    //    but break on horizontal minima if approaching from their left.
                    //    This ensures 'local minima' are always on the left of horizontals.

                    //look ahead is required in case of multiple consec. horizontals
                    EHorz = GetLastHorz(E);
                    if (EHorz == E.Prev ||                    //horizontal line
                        (EHorz.Next.Top.Y < E.Top.Y &&      //bottom horizontal
                         EHorz.Next.Bot.X > E.Prev.Bot.X))  //approaching from the left
                        break;
                    if (E.Top.X != E.Prev.Bot.X) ReverseHorizontal(E);
                    if (EHorz.OutIdx == Skip) EHorz = EHorz.Prev;
                    while (E != EHorz)
                    {
                        E.NextInLML = E.Prev;
                        E = E.Next;
                        if (E.Top.X != E.Prev.Bot.X) ReverseHorizontal(E);
                    }
                }
                else if (E.Bot.Y == E.Prev.Bot.Y) break;
                E.NextInLML = E.Prev;
            }
            return E.Prev;
        }
        //----------------------------------------------------------------------

        void AscendToMax(ref TEdge E, bool Appending, bool IsClosed)
        {
            if (E.OutIdx == Skip)
            {
                E = E.Next;
                if (!MoreAbove(E.Prev)) return;
            }

            if (IsHorizontal(E) && Appending &&
                (E.Bot != E.Prev.Bot))
                ReverseHorizontal(E);
            //now process the ascending bound ....
            TEdge EStart = E;
            for (; ; )
            {
                if (E.Next.OutIdx == Skip ||
                    ((E.Next.Top.Y == E.Top.Y) && !IsHorizontal(E.Next))) break;
                E.NextInLML = E.Next;
                E = E.Next;
                if (IsHorizontal(E) && (E.Bot.X != E.Prev.Top.X))
                    ReverseHorizontal(E);
            }

            if (!Appending)
            {
                if (EStart.OutIdx == Skip) EStart = EStart.Next;
                if (EStart != E.Next)
                    DoMinimaLML(null, EStart, IsClosed);
            }
            E = E.Next;
        }
        //----------------------------------------------------------------------

        TEdge AddBoundsToLML(TEdge E, bool Closed)
        {
            //Starting at the top of one bound we progress to the bottom where there's
            //A local minima. We then go to the top of the Next bound. These two bounds
            //form the left and right (or right and left) bounds of the local minima.

            TEdge B;
            bool AppendMaxima;
            //do minima ...
            if (E.OutIdx == Skip)
            {
                if (MoreBelow(E))
                {
                    E = E.Next;
                    B = DescendToMin(ref E);
                }
                else
                    B = null;
            }
            else
                B = DescendToMin(ref E);

            if (E.OutIdx == Skip)    //nb: may be BEFORE, AT or just THRU LM
            {
                //do minima before Skip...
                DoMinimaLML(null, B, Closed);      //store what we've got so far (if anything)
                AppendMaxima = false;
                //finish off any minima ...
                if (E.Bot != E.Prev.Bot && MoreBelow(E))
                {
                    E = E.Next;
                    B = DescendToMin(ref E);
                    DoMinimaLML(B, E, Closed);
                    AppendMaxima = true;
                }
                else if (JustBeforeLocMin(E))
                    E = E.Next;
            }
            else
            {
                DoMinimaLML(B, E, Closed);
                AppendMaxima = true;
            }

            //now do maxima ...
            AscendToMax(ref E, AppendMaxima, Closed);

            if (E.OutIdx == Skip && (E.Top != E.Prev.Top)) //may be BEFORE, AT or just AFTER maxima
            {
                //finish off any maxima ...
                if (MoreAbove(E))
                {
                    E = E.Next;
                    AscendToMax(ref E, false, Closed);
                }
                else if (E.Top == E.Next.Top || (IsHorizontal(E.Next) && (E.Top == E.Next.Bot)))
                    E = E.Next; //ie just before Maxima
            }
            return E;
        }
        //------------------------------------------------------------------------------

        private void InsertLocalMinima(LocalMinima newLm)
        {
            if (m_MinimaList == null)
            {
                m_MinimaList = newLm;
            }
            else if (newLm.Y >= m_MinimaList.Y)
            {
                newLm.Next = m_MinimaList;
                m_MinimaList = newLm;
            }
            else
            {
                LocalMinima tmpLm = m_MinimaList;
                while (tmpLm.Next != null && (newLm.Y < tmpLm.Next.Y))
                    tmpLm = tmpLm.Next;
                newLm.Next = tmpLm.Next;
                tmpLm.Next = newLm;
            }
        }
        //------------------------------------------------------------------------------

        protected void PopLocalMinima()
        {
            if (m_CurrentLM == null) return;
            m_CurrentLM = m_CurrentLM.Next;
        }
        //------------------------------------------------------------------------------

        private void ReverseHorizontal(TEdge e)
        {
            //swap horizontal edges' top and bottom x's so they follow the natural
            //progression of the bounds - ie so their xbots will align with the
            //adjoining lower edge. [Helpful in the ProcessHorizontal() method.]
            Int64 tmp = e.Top.X;
            e.Top.X = e.Bot.X;
            e.Bot.X = tmp;
#if use_xyz
        tmp = e.Top.Z;
        e.Top.Z = e.Bot.Z;
        e.Bot.Z = tmp;
#endif
        }
        //------------------------------------------------------------------------------

        protected virtual void Reset()
        {
            m_CurrentLM = m_MinimaList;
            if (m_CurrentLM == null) return; //ie nothing to process

            //reset all edges ...
            LocalMinima lm = m_MinimaList;
            while (lm != null)
            {
                TEdge e = lm.LeftBound;
                if (e != null)
                {
                    e.Curr = e.Bot;
                    e.Side = EdgeSide.esLeft;
                    if (e.OutIdx != Skip)
                        e.OutIdx = Unassigned;
                }
                e = lm.RightBound;
                e.Curr = e.Bot;
                e.Side = EdgeSide.esRight;
                if (e.OutIdx != Skip)
                    e.OutIdx = Unassigned;

                lm = lm.Next;
            }
        }
        //------------------------------------------------------------------------------

        public IntRect GetBounds()
        {
            IntRect result = new IntRect();
            LocalMinima lm = m_MinimaList;
            if (lm == null) return result;
            result.left = lm.LeftBound.Bot.X;
            result.top = lm.LeftBound.Bot.Y;
            result.right = lm.LeftBound.Bot.X;
            result.bottom = lm.LeftBound.Bot.Y;
            while (lm != null)
            {
                if (lm.LeftBound.Bot.Y > result.bottom)
                    result.bottom = lm.LeftBound.Bot.Y;
                TEdge e = lm.LeftBound;
                for (; ; )
                {
                    TEdge bottomE = e;
                    while (e.NextInLML != null)
                    {
                        if (e.Bot.X < result.left) result.left = e.Bot.X;
                        if (e.Bot.X > result.right) result.right = e.Bot.X;
                        e = e.NextInLML;
                    }
                    if (e.Bot.X < result.left) result.left = e.Bot.X;
                    if (e.Bot.X > result.right) result.right = e.Bot.X;
                    if (e.Top.X < result.left) result.left = e.Top.X;
                    if (e.Top.X > result.right) result.right = e.Top.X;
                    if (e.Top.Y < result.top) result.top = e.Top.Y;

                    if (bottomE == lm.LeftBound) e = lm.RightBound;
                    else break;
                }
                lm = lm.Next;
            }
            return result;
        }

    }
}