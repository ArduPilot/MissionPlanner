using System.Collections.Generic;

namespace ClipperLib
{
    internal class PolyNode
    {
        internal PolyNode m_Parent;
        internal List<IntPoint> m_polygon = new List<IntPoint>();
        internal int m_Index;
        internal List<PolyNode> m_Childs = new List<PolyNode>();

        private bool IsHoleNode()
        {
            bool result = true;
            PolyNode node = m_Parent;
            while (node != null)
            {
                result = !result;
                node = node.m_Parent;
            }
            return result;
        }

        public int ChildCount
        {
            get { return m_Childs.Count; }
        }

        public List<IntPoint> Contour
        {
            get { return m_polygon; }
        }

        internal void AddChild(PolyNode Child)
        {
            int cnt = m_Childs.Count;
            m_Childs.Add(Child);
            Child.m_Parent = this;
            Child.m_Index = cnt;
        }

        public PolyNode GetNext()
        {
            if (m_Childs.Count > 0)
                return m_Childs[0];
            else
                return GetNextSiblingUp();
        }

        internal PolyNode GetNextSiblingUp()
        {
            if (m_Parent == null)
                return null;
            else if (m_Index == m_Parent.m_Childs.Count - 1)
                return m_Parent.GetNextSiblingUp();
            else
                return m_Parent.m_Childs[m_Index + 1];
        }

        public List<PolyNode> Childs
        {
            get { return m_Childs; }
        }

        public PolyNode Parent
        {
            get { return m_Parent; }
        }

        public bool IsHole
        {
            get { return IsHoleNode(); }
        }

        public bool IsOpen { get; set; }
    }
}