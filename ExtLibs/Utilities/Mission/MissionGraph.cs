using System.Collections.Generic;
using static MissionPlanner.Utilities.Mission.CommandUtils;

namespace MissionPlanner.Utilities.Mission
{
    /// <summary>
    /// A positional waypoint in the mission graph, created for each mission
    /// item that satisfies <see cref="CommandUtils.IsNode"/>.
    /// </summary>
    public sealed class MissionNode
    {
        /// <summary>0-based index into the original mission item list.</summary>
        public int MissionIndex { get; }
        public Locationwp Command { get; }
        public List<MissionEdge> IncomingEdges { get; } = new List<MissionEdge>();
        public List<MissionEdge> OutgoingEdges { get; } = new List<MissionEdge>();

        public MissionNode(int missionIndex, Locationwp command)
        {
            MissionIndex = missionIndex;
            Command = command;
        }
    }

    /// <summary>
    /// A directed edge between two mission nodes: either a sequential
    /// transition or a jump (DO_JUMP / DO_JUMP_TAG).
    /// </summary>
    public sealed class MissionEdge
    {
        public MissionNode FromNode { get; }
        public MissionNode ToNode { get; }
        public bool IsJump { get; }
        /// <summary>
        /// For jump edges: positive = finite repeats, negative = infinite.
        /// </summary>
        public int? JumpRepeat { get; }

        public MissionEdge(MissionNode fromNode, MissionNode toNode, bool isJump, int? jumpRepeat = null)
        {
            FromNode = fromNode;
            ToNode = toNode;
            IsJump = isJump;
            JumpRepeat = jumpRepeat;
        }

        public override string ToString()
        {
            return $"{(IsJump ? "Jump" : "Seq")}: {FromNode} -> {ToNode}";
        }
    }

    /// <summary>
    /// A non-navigated label in the mission sequence (DO_LAND_START, JUMP_TAG, etc.)
    /// that points to the next graph node at or after its position.
    /// </summary>
    public sealed class MissionBookmark
    {
        /// <summary>0-based index into the original mission item list.</summary>
        public int MissionIndex { get; }
        public Locationwp Command { get; }
        /// <summary>The first graph node at or after this bookmark's mission index.</summary>
        public MissionNode Target { get; }

        public MissionBookmark(int missionIndex, Locationwp command, MissionNode target)
        {
            MissionIndex = missionIndex;
            Command = command;
            Target = target;
        }
    }

    /// <summary>
    /// Directed graph of mission waypoints with sequential and jump edges.
    /// Constructed via <see cref="Create"/>.
    /// </summary>
    public sealed class MissionGraph
    {
        public IReadOnlyList<MissionNode> Nodes { get; }
        public IReadOnlyList<MissionEdge> Edges { get; }
        public IReadOnlyList<MissionBookmark> Bookmarks { get; }
        public readonly PointLatLngAlt Home;

        private readonly IReadOnlyList<MissionNode> firstAtOrAfter;
        private readonly IReadOnlyList<MissionNode> lastAtOrBefore;

        private MissionGraph(List<MissionNode> nodes,
                            List<MissionEdge> edges,
                            List<MissionBookmark> bookmarks,
                            PointLatLngAlt home,
                            List<MissionNode> firstAtOrAfter,
                            List<MissionNode> lastAtOrBefore)
        {
            Nodes = nodes;
            Edges = edges;
            Bookmarks = bookmarks;
            Home = home;
            this.firstAtOrAfter = firstAtOrAfter;
            this.lastAtOrBefore = lastAtOrBefore;
        }

        /// <summary>
        /// Builds the graph. Mission items are 0-indexed (home is not in the list).
        /// </summary>
        public static MissionGraph Create(PointLatLngAlt home, List<Locationwp> missionitems)
        {
            var nodes = new List<MissionNode>();
            var edges = new List<MissionEdge>();
            var bookmarks = new List<MissionBookmark>();

            // Map from mission index (0-based) to node indices
            var missionToNode = new Dictionary<int, MissionNode>();
            var jumpTags = new Dictionary<int, int>(); // key=tag, value=mission index
            // Track nodes that cannot sequentially move to the next node (terminal nodes, or nodes preceding infinite jumps)
            var nodesWithoutSequentialFallthrough = new HashSet<MissionNode>();
            for (int i = 0; i < missionitems.Count; i++)
            {
                var cmd = missionitems[i];
                if (IsNode(cmd))
                {
                    var node = new MissionNode(i, cmd);
                    nodes.Add(node);
                    if (IsTerminal(cmd.id))
                    {
                        nodesWithoutSequentialFallthrough.Add(node);
                    }
                    missionToNode[i] = node;
                }
                if (cmd.id == (ushort)MAVLink.MAV_CMD.JUMP_TAG)
                {
                    jumpTags[(int)cmd.p1] = i;
                }
                if (IsJumpCommand(cmd.id) && GetJumpCount(cmd) < 0 && nodes.Count > 0)
                {
                    nodesWithoutSequentialFallthrough.Add(nodes[nodes.Count - 1]);
                }
            }

            // Sequential edges over nodes
            for (int i = 0; i < nodes.Count - 1; i++)
            {
                var node1 = nodes[i];
                var node2 = nodes[i + 1];
                if (nodesWithoutSequentialFallthrough.Contains(node1))
                {
                    continue;
                }
                if (IsLand(node1.Command.id) && !IsTakeoff(node2.Command.id))
                {
                    continue;
                }
                var edge = new MissionEdge(node1, nodes[i + 1], false);
                edges.Add(edge);
                node1.OutgoingEdges.Add(edge);
                node2.IncomingEdges.Add(edge);
            }

            // Build the firstAtOrAfter and lastAtOrBefore lists, and bookmarks
            var firstAtOrAfter = new List<MissionNode>(new MissionNode[missionitems.Count]);
            MissionNode next = null;
            for (int i = missionitems.Count - 1; i >= 0; i--)
            {
                if (missionToNode.TryGetValue(i, out var node))
                {
                    next = node;
                }
                firstAtOrAfter[i] = next;
                if (IsBookmark(missionitems[i].id))
                {
                    bookmarks.Add(new MissionBookmark(i, missionitems[i], next));
                }
            }

            var lastAtOrBefore = new List<MissionNode>(new MissionNode[missionitems.Count]);
            MissionNode last = null;
            for (int i = 0; i < missionitems.Count; i++)
            {
                if (missionToNode.TryGetValue(i, out var node))
                {
                    last = node;
                }
                lastAtOrBefore[i] = last;
            }

            // Jump edges
            for (int i = 0; i < missionitems.Count; i++)
            {
                var item = missionitems[i];
                if (!IsJumpCommand(item.id) || GetJumpCount(item) == 0 || !TryGetJumpTarget(item, jumpTags, out int jumpTargetMissionIndex))
                {
                    continue; // not a jump
                }

                if (jumpTargetMissionIndex < 0 || jumpTargetMissionIndex >= missionitems.Count)
                {
                    continue; // invalid target
                }

                var srcNode = lastAtOrBefore[i];
                if (srcNode == null)
                {
                    continue; // source is not a node
                }

                var destNode = firstAtOrAfter[jumpTargetMissionIndex];
                if (destNode == null)
                {
                    continue; // target is not a node
                }

                if (IsLand(srcNode.Command.id) && !IsTakeoff(destNode.Command.id))
                {
                    continue; // Landing without a subsequent takeoff, is considered terminal; do not count this as a valid edge.
                }

                int jumpRepeat = GetJumpCount(item);
                if (jumpRepeat == 0)
                {
                    continue; // no jump
                }

                var edge = new MissionEdge(srcNode, destNode, true, jumpRepeat);
                edges.Add(edge);
                srcNode.OutgoingEdges.Add(edge);
                destNode.IncomingEdges.Add(edge);
            }

            return new MissionGraph(nodes, edges, bookmarks, home, firstAtOrAfter, lastAtOrBefore);
        }

        /// <summary>
        /// O(1) lookup from a 0-based mission item index to the nearest node
        /// at or after that position, or null if no nodes follow.
        /// </summary>
        public MissionNode FirstNodeAtOrAfter(int missionIndex)
        {
            if (missionIndex >= firstAtOrAfter.Count)
            {
                return null;
            }
            if (missionIndex < 0)
            {
                missionIndex = 0;
            }
            return firstAtOrAfter[missionIndex];
        }

        /// <summary>
        /// O(1) lookup from a 0-based mission item index to the nearest node
        /// at or before that position, or null if no nodes precede it.
        /// </summary>
        public MissionNode LastNodeAtOrBefore(int missionIndex)
        {
            if (missionIndex < 0)
            {
                return null;
            }
            if (missionIndex >= lastAtOrBefore.Count)
            {
                missionIndex = lastAtOrBefore.Count - 1;
            }
            return lastAtOrBefore[missionIndex];
        }
    }
}
