using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Windows.Forms;
using log4net;
using MissionPlanner.Utilities;
using ZedGraph;
using Label = System.Windows.Forms.Label;

namespace MissionPlanner.Controls
{
    /// <summary>
    /// The Plan screen's elevation graph: the terrain under the mission, the planned path and its
    /// waypoints, the rally points in purple, the vehicle's fence ceiling and floor, where the
    /// path crosses its fence circle, and a red ring around each point outside the fence. It lies
    /// over the bottom of the map; drag its title bar to resize it, or use the arrow to make it tall.
    /// </summary>
    public class PlanElevationGraph : UserControl
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>Rally points are purple, to tell them from the waypoints.</summary>
        public static readonly Color RallyColor = Color.FromArgb(160, 80, 255);

        private static readonly Color TerrainColor = Color.FromArgb(160, 120, 80);
        private static readonly Color FenceColor = Color.Red;

        /// <summary>More waypoints than this and only every few get a number.</summary>
        private const int MaxLabels = 40;

        private readonly Panel header = new Panel();
        private readonly Label title = new Label();
        private readonly Label summary = new Label();
        private readonly Label fence = new Label();
        private readonly Label outsideFence = new Label();
        private readonly Label status = new Label();
        private readonly MyButton expandButton = new MyButton();
        private readonly MyButton closeButton = new MyButton();
        private readonly ToolTip toolTip = new ToolTip();
        private readonly ZedGraphControl graph = new ZedGraphControl();

        /// <summary>Bold for the title and the outside-the-fence warning; made here, so disposed here.</summary>
        private readonly Font boldFont;

        /// <summary>The least height the graph is worth drawing at.</summary>
        public const int MinimumHeight = 90;

        /// <summary>The bar that follows the mouse while the title bar is dragged, as thick as a splitter.</summary>
        private const int DragBarThickness = 3;

        /// <summary>More points outside the fence than this and the title bar names only the first few.</summary>
        private const int MaxOutsideNamed = 12;

        private ElevationGraphProfile profile;
        private bool expanded;

        // the title bar drag: as with the splitter above the waypoint list, a bar follows the mouse
        // and the graph changes height once, when the button is released
        private bool dragging;
        private int dragStartY;
        private int dragStartHeight;
        private int dragHeight;
        private bool dragBarShown;
        private Rectangle dragBar;

        /// <summary>display units per metre along the path</summary>
        private double distScale = 1;

        private string distUnit = "m";

        /// <summary>The arrow button or a double click on the title bar changed <see cref="Expanded"/>.</summary>
        public event EventHandler ExpandedChanged;

        public event EventHandler CloseClicked;

        /// <summary>The title bar was dragged and let go; the argument is the height it was dragged to.</summary>
        public event EventHandler<int> HeightDragged;

        /// <summary>The mouse is over the graph: the point of the path under it, or null when it left.</summary>
        public event EventHandler<PointLatLngAlt> HoverChanged;

        /// <summary>The tallest the title bar can be dragged to; the owner sets it from the room the map has.</summary>
        public int MaximumHeight { get; set; } = int.MaxValue;

        public PlanElevationGraph()
        {
            BorderStyle = BorderStyle.FixedSingle;

            graph.Dock = DockStyle.Fill;
            graph.IsShowPointValues = true;
            graph.IsShowCopyMessage = false;
            graph.IsAntiAlias = true;
            graph.PointValueEvent += Graph_PointValueEvent;
            graph.MouseMove += Graph_MouseMove;
            graph.MouseLeave += (sender, e) => HoverChanged?.Invoke(this, null);

            var pane = graph.GraphPane;
            pane.Title.IsVisible = false;
            pane.IsFontsScaled = false;
            pane.Margin.All = 4;
            pane.Legend.Position = LegendPos.Top;
            pane.Legend.IsHStack = true;
            pane.Legend.Border.IsVisible = false;
            pane.Legend.FontSpec.Size = 8;
            foreach (var axis in new Axis[] {pane.XAxis, pane.YAxis})
            {
                axis.Title.FontSpec.Size = 8;
                axis.Title.FontSpec.IsBold = false;
                axis.Scale.FontSpec.Size = 8;
                axis.MajorGrid.IsVisible = true;
                axis.MajorTic.IsOpposite = false;
                axis.MinorTic.IsOpposite = false;
            }

            title.AutoSize = true;
            title.Location = new Point(4, 4);
            boldFont = new Font(Font, FontStyle.Bold);
            title.Font = boldFont;
            title.Text = Strings.ElevationGraphTitle;

            summary.AutoSize = true;
            fence.AutoSize = true;
            outsideFence.AutoSize = true;
            outsideFence.Font = boldFont;
            outsideFence.ForeColor = FenceColor;
            status.AutoSize = true;
            // the theme leaves a "custom" label's colour alone; these set their own
            fence.Tag = "custom";
            outsideFence.Tag = "custom";
            status.Tag = "custom";

            expandButton.Size = new Size(26, 18);
            expandButton.Click += (sender, e) => Expanded = !Expanded;
            closeButton.Size = new Size(26, 18);
            closeButton.Text = "\u00d7";
            closeButton.Click += (sender, e) => CloseClicked?.Invoke(this, EventArgs.Empty);
            toolTip.SetToolTip(closeButton, Strings.ElevationGraphHideTip);
            UpdateExpandButton();

            header.Dock = DockStyle.Top;
            header.Height = 22;
            header.Controls.Add(closeButton);
            header.Controls.Add(expandButton);
            header.Controls.Add(status);
            header.Controls.Add(outsideFence);
            header.Controls.Add(fence);
            header.Controls.Add(summary);
            header.Controls.Add(title);
            header.Resize += (sender, e) => LayoutHeader();

            // the title bar resizes the graph: drag it, or double click it to make the graph tall
            foreach (var bar in new Control[] {header, title, summary, fence, outsideFence, status})
            {
                bar.Cursor = Cursors.SizeNS;
                bar.MouseDown += Header_MouseDown;
                bar.MouseMove += Header_MouseMove;
                bar.MouseUp += Header_MouseUp;
                bar.MouseCaptureChanged += Header_MouseCaptureChanged;
                bar.DoubleClick += (sender, e) => Expanded = !Expanded;
                toolTip.SetToolTip(bar, Strings.ElevationGraphResizeTip);
            }

            // docked controls lay out last added first: the title bar on top, the graph below it
            Controls.Add(graph);
            Controls.Add(header);
        }

        /// <summary>Tall (most of the map) instead of the height the user dragged it to.</summary>
        public bool Expanded
        {
            get { return expanded; }
            set
            {
                if (expanded == value)
                    return;
                expanded = value;
                UpdateExpandButton();
                ExpandedChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>The graph on show is missing terrain somewhere along the path.</summary>
        public bool TerrainMissing
        {
            get { return profile != null && profile.TerrainMissing; }
        }

        /// <summary>
        /// Draw a profile. <paramref name="terrainLoading"/> says missing terrain may still arrive
        /// (SRTM tiles download in the background), rather than there being none.
        /// </summary>
        public void ShowProfile(ElevationGraphProfile newProfile, bool terrainLoading)
        {
            profile = newProfile;
            ChooseDistanceUnit(profile.TotalDistance);

            var pane = graph.GraphPane;
            pane.CurveList.Clear();
            pane.GraphObjList.Clear();

            // the theme fills the chart; pick line and text colours that show on it
            var dark = pane.Chart.Fill.Color.GetBrightness() < 0.5f;
            var pathColor = dark ? Color.Gold : Color.DarkOrange;
            var textColor = dark ? Color.White : Color.Black;

            // a little room at both ends, so a climb at the start or a landing at the end is not
            // hidden under the axis
            var xmax = profile.TotalDistance > 0 ? profile.TotalDistance * distScale : 1;
            var axisMin = -xmax * 0.01;
            var axisMax = xmax * 1.01;

            var terrain = new PointPairList();
            var path = new PointPairList();
            var below = new PointPairList();
            var anyBelow = false;
            foreach (var sample in profile.Samples)
            {
                var x = sample.Dist * distScale;
                var under = sample.Airborne && sample.Alt < sample.Terrain;
                anyBelow |= under;
                terrain.Add(new PointPair(x, ToAxis(sample.Terrain)) {Tag = sample});
                path.Add(new PointPair(x, ToAxis(sample.Alt)) {Tag = sample});
                below.Add(new PointPair(x, under ? ToAxis(sample.Alt) : PointPair.Missing) {Tag = sample});
            }

            // added bottom layer first: this copy of ZedGraph draws the curve list in order
            var terrainCurve = pane.AddCurve(Strings.ElevationGraphTerrain, terrain, TerrainColor, SymbolType.None);
            terrainCurve.Line.Width = 1.5f;
            terrainCurve.Line.Fill = new Fill(Color.FromArgb(dark ? 110 : 80, TerrainColor));

            AddFenceLine(pane, profile.FenceCeiling, profile.FenceCeilingAlt, Strings.ElevationGraphFenceCeiling,
                false, axisMin, axisMax);
            AddFenceLine(pane, profile.FenceFloor, profile.FenceFloorAlt, Strings.ElevationGraphFenceFloor,
                true, axisMin, axisMax);
            AddFenceCircleCrossings(pane, axisMin, axisMax);

            // a line through the rally points is not flown, so it has no path to draw
            LineItem pathCurve = null;
            if (!profile.RallyChain)
            {
                pathCurve = pane.AddCurve(Strings.ElevationGraphPath, path, pathColor, SymbolType.None);
                pathCurve.Line.Width = 2;
            }

            LineItem belowCurve = null;
            if (anyBelow)
            {
                belowCurve = pane.AddCurve(Strings.ElevationGraphBelowTerrain, below, Color.Red, SymbolType.None);
                belowCurve.Line.Width = 3;
            }

            if (profile.Waypoints.Count > 0)
            {
                var waypoints = MarkerPoints(profile.Waypoints);
                var waypointCurve = pane.AddCurve(Strings.ElevationGraphWaypoints, waypoints, pathColor,
                    SymbolType.Circle);
                waypointCurve.Line.IsVisible = false;
                waypointCurve.Symbol.Size = 6;
                waypointCurve.Symbol.Fill = new Fill(pathColor);
                AddLabels(pane, profile.Waypoints, textColor);
            }

            if (profile.RallyPoints.Count > 0)
            {
                var rallyCurve = pane.AddCurve(Strings.ElevationGraphRallyPoints, MarkerPoints(profile.RallyPoints),
                    RallyColor, SymbolType.Diamond);
                rallyCurve.Line.IsVisible = false;
                rallyCurve.Symbol.Size = 10;
                rallyCurve.Symbol.Fill = new Fill(RallyColor);
                AddLabels(pane, profile.RallyPoints, RallyColor);
            }

            // a red ring around each point outside the fence, drawn over the points themselves
            if (profile.OutsideFence.Count > 0)
            {
                var outsideCurve = pane.AddCurve(Strings.ElevationGraphOutsideFenceLegend,
                    MarkerPoints(profile.OutsideFence), FenceColor, SymbolType.Circle);
                outsideCurve.Line.IsVisible = false;
                outsideCurve.Symbol.Size = 16;
                outsideCurve.Symbol.Border.Width = 2.5f;
                outsideCurve.Symbol.Fill = new Fill(Color.Transparent);
            }

            var legendAt = 0;
            LegendStandIn(pane, terrainCurve, legendAt++);
            if (pathCurve != null)
                LegendStandIn(pane, pathCurve, legendAt++);
            if (belowCurve != null)
                LegendStandIn(pane, belowCurve, legendAt);
            pane.Legend.Fill.IsVisible = false;

            pane.XAxis.Title.Text = string.Format(
                profile.RallyChain ? Strings.ElevationGraphRallyDistanceAxis : Strings.ElevationGraphDistanceAxis,
                distUnit);
            pane.YAxis.Title.Text = string.Format(Strings.ElevationGraphAltitudeAxis, AltUnit);
            pane.XAxis.Scale.MinAuto = false;
            pane.XAxis.Scale.MaxAuto = false;
            pane.XAxis.Scale.Min = axisMin;
            pane.XAxis.Scale.Max = axisMax;
            pane.YAxis.Scale.MinAuto = true;
            pane.YAxis.Scale.MaxAuto = true;

            ShowStatus(anyBelow, terrainLoading);

            try
            {
                graph.AxisChange();
            }
            catch (ArgumentException ex)
            {
                // the graph keeps its last scale; nothing on screen depends on this one
                log.Warn("elevation graph rescale failed", ex);
            }

            graph.Invalidate();
        }

        private void ShowStatus(bool belowTerrain, bool terrainLoading)
        {
            var waypoints = profile.Waypoints.Exists(a => a.Item != null);
            var anything = waypoints || profile.RallyPoints.Count > 0;

            // rally points alone make no path to measure: their own clearance is what counts
            if (profile.RallyChain)
                summary.Text = double.IsNaN(profile.LowestClearance)
                    ? string.Format(Strings.ElevationGraphRallySummaryNoClearance, profile.RallyPoints.Count)
                    : string.Format(Strings.ElevationGraphRallySummary, profile.RallyPoints.Count,
                        FormatAltitude(profile.LowestClearance));
            else if (!waypoints)
                summary.Text = Strings.ElevationGraphNoWaypoints;
            else if (double.IsNaN(profile.LowestClearance))
                summary.Text = string.Format(Strings.ElevationGraphSummaryNoClearance,
                    FormatDistance(profile.TotalDistance));
            else
                summary.Text = string.Format(Strings.ElevationGraphSummary, FormatDistance(profile.TotalDistance),
                    FormatAltitude(profile.LowestClearance));

            // what the vehicle says of its fence, so the fence lines can be checked against it
            switch (profile.FenceState)
            {
                case ElevationGraphFenceState.Enabled:
                    ShowFence(Strings.ElevationGraphFenceEnabled, true);
                    break;
                case ElevationGraphFenceState.EnablesAfterTakeoff:
                    ShowFence(Strings.ElevationGraphFenceAfterTakeoff, true);
                    break;
                case ElevationGraphFenceState.EnablesWhenArmed:
                    ShowFence(Strings.ElevationGraphFenceWhenArmed, true);
                    break;
                case ElevationGraphFenceState.ReportOnly:
                    ShowFence(Strings.ElevationGraphFenceReportOnly, false);
                    break;
                case ElevationGraphFenceState.Disabled:
                    ShowFence(Strings.ElevationGraphFenceDisabled, false);
                    break;
                default:
                    ShowFence("", false);
                    break;
            }

            // the points outside the fence by name, so they can be found even when crowded
            var named = profile.OutsideFence.ConvertAll(a => a.Label);
            if (named.Count > MaxOutsideNamed)
            {
                named.RemoveRange(MaxOutsideNamed, named.Count - MaxOutsideNamed);
                named.Add("...");
            }
            outsideFence.Text = named.Count > 0
                ? string.Format(Strings.ElevationGraphOutsideFence, string.Join(", ", named))
                : "";

            status.ForeColor = Color.DarkOrange;
            if (belowTerrain)
            {
                status.Text = Strings.ElevationGraphBelowTerrainWarning;
                status.ForeColor = Color.Red;
            }
            else if (!profile.HomeSet && anything)
            {
                status.Text = Strings.ElevationGraphNoHome;
            }
            else if (profile.TerrainMissing)
            {
                status.Text = terrainLoading ? Strings.ElevationGraphTerrainLoading : Strings.ElevationGraphTerrainMissing;
            }
            else
            {
                status.Text = "";
            }

            LayoutHeader();
        }

        private void ShowFence(string text, bool on)
        {
            fence.Text = text;
            fence.ForeColor = on ? FenceColor : Color.Gray;
        }

        private void LayoutHeader()
        {
            closeButton.Location = new Point(header.ClientSize.Width - closeButton.Width - 2, 2);
            expandButton.Location = new Point(closeButton.Left - expandButton.Width - 2, 2);
            summary.Location = new Point(title.Right + 10, title.Top);
            fence.Location = new Point(summary.Right + 10, title.Top);
            outsideFence.Location = new Point(fence.Right + (fence.Text == "" ? 0 : 10), title.Top);
            status.Location = new Point(outsideFence.Right + (outsideFence.Text == "" ? 0 : 10), title.Top);
        }

        private void UpdateExpandButton()
        {
            // the same arrows the waypoint list's own size button uses
            expandButton.Text = expanded ? "\u02c5" : "\u02c4";
            toolTip.SetToolTip(expandButton,
                expanded ? Strings.ElevationGraphShrinkTip : Strings.ElevationGraphExpandTip);
        }

        private PointPairList MarkerPoints(List<ElevationGraphProfile.Marker> markers)
        {
            var list = new PointPairList();
            foreach (var marker in markers)
                list.Add(new PointPair(marker.Dist * distScale, ToAxis(marker.Alt)) {Tag = marker});
            return list;
        }

        /// <summary>
        /// Write each point's label above it, in <paramref name="color"/>, or white on red for a
        /// point outside the fence. A point outside the fence is always labelled, however many there are.
        /// </summary>
        private void AddLabels(GraphPane pane, List<ElevationGraphProfile.Marker> markers, Color color)
        {
            var every = Math.Max(1, (int) Math.Ceiling(markers.Count / (double) MaxLabels));
            for (int i = 0; i < markers.Count; i++)
            {
                var marker = markers[i];
                var outside = marker.Breach != ElevationGraphFenceBreach.None;
                if (double.IsNaN(marker.Alt) || (i % every != 0 && !outside))
                    continue;

                var text = new TextObj(marker.Label, marker.Dist * distScale, ToAxis(marker.Alt),
                    CoordType.AxisXYScale, AlignH.Center, AlignV.Bottom);
                text.FontSpec.Size = 8;
                text.FontSpec.Border.IsVisible = false;
                if (outside)
                {
                    // white on red, so it reads over the red ring around the point
                    text.FontSpec.IsBold = true;
                    text.FontSpec.FontColor = Color.White;
                    text.FontSpec.Fill = new Fill(FenceColor);
                }
                else
                {
                    text.FontSpec.FontColor = color;
                    text.FontSpec.Fill.IsVisible = false;
                }

                text.IsClippedToChartRect = true;
                pane.GraphObjList.Add(text);
            }
        }

        /// <summary>
        /// This copy of ZedGraph writes the Min, Max and Mean of every line it draws into the line's
        /// label, which means nothing here. So the drawn line stays out of the legend, and an empty
        /// line in the same colours stands in for it there, at <paramref name="position"/>.
        /// </summary>
        private static void LegendStandIn(GraphPane pane, LineItem drawn, int position)
        {
            drawn.Label.IsVisible = false;
            var standIn = new LineItem(drawn.Label.Text, new PointPairList(), drawn.Color, SymbolType.None);
            standIn.Line.Width = drawn.Line.Width;
            standIn.Line.Fill = drawn.Line.Fill;
            pane.CurveList.Insert(position, standIn);
        }

        /// <summary>
        /// A fence limit as a level line across the graph, its name written just above it: dashed
        /// red when the vehicle will hold to it, dotted grey when the fence is off or does not use
        /// it (the title bar says which fence state the vehicle reports).
        /// </summary>
        private void AddFenceLine(GraphPane pane, ElevationGraphFenceLimit limit, double alt, string format,
            bool floor, double xmin, double xmax)
        {
            if (limit == null || double.IsNaN(alt))
                return;

            var enforced = profile.Enforces(limit);
            var label = string.Format(format, FormatAltitude(limit.Alt));
            if (!limit.Used)
                label = string.Format(Strings.ElevationGraphFenceNote, label, Strings.ElevationGraphFenceNotInType);
            else if (floor && enforced)
                // ArduPilot arms the floor once the vehicle has climbed above it, so a climb out
                // from below is no breach
                label = string.Format(Strings.ElevationGraphFenceNote, label, Strings.ElevationGraphFenceFloorArms);

            var y = ToAxis(alt);
            var color = enforced ? FenceColor : Color.Gray;
            var line = new PointPairList
            {
                new PointPair(xmin, y) {Tag = label},
                new PointPair(xmax, y) {Tag = label}
            };
            var curve = pane.AddCurve(label, line, color, SymbolType.None);
            curve.Label.IsVisible = false;
            curve.Line.Width = 1.5f;
            curve.Line.Style = enforced ? DashStyle.Dash : DashStyle.Dot;

            var text = new TextObj(label, 0.012, y, CoordType.XChartFractionYScale, AlignH.Left, AlignV.Bottom);
            text.FontSpec.Size = 8;
            text.FontSpec.FontColor = color;
            text.FontSpec.Border.IsVisible = false;
            text.FontSpec.Fill.IsVisible = false;
            text.IsClippedToChartRect = true;
            pane.GraphObjList.Add(text);
        }

        /// <summary>
        /// Where the path crosses the fence circle, a dashed red line from top to bottom with the
        /// radius written beside it. Only while the vehicle will hold to the circle: a circle the
        /// fence does not use, or a fence that is off, draws nothing.
        /// </summary>
        private void AddFenceCircleCrossings(GraphPane pane, double xmin, double xmax)
        {
            if (!profile.Enforces(profile.FenceCircle))
                return;

            var label = string.Format(Strings.ElevationGraphFenceRadius, FormatDistance(profile.FenceCircle.Radius));
            var width = xmax - xmin;
            var lastLabel = double.NegativeInfinity;
            foreach (var dist in profile.FenceCircleCrossings)
            {
                var x = dist * distScale;
                var line = new LineObj(FenceColor, x, 0, x, 1);
                line.Location.CoordinateFrame = CoordType.XScaleYChartFraction;
                line.Line.Style = DashStyle.Dash;
                line.Line.Width = 1.5f;
                line.IsClippedToChartRect = true;
                line.ZOrder = ZOrder.E_BehindCurves;
                pane.GraphObjList.Add(line);

                // one label for crossings close together; on the right of the graph it goes on
                // the line's left, so it is not cut off
                if (x - lastLabel < width * 0.15)
                    continue;
                lastLabel = x;
                var right = width > 0 && (x - xmin) / width > 0.75;
                var text = new TextObj(label, x, 0.02, CoordType.XScaleYChartFraction,
                    right ? AlignH.Right : AlignH.Left, AlignV.Top);
                text.FontSpec.Size = 8;
                text.FontSpec.FontColor = FenceColor;
                text.FontSpec.Border.IsVisible = false;
                text.FontSpec.Fill.IsVisible = false;
                text.IsClippedToChartRect = true;
                pane.GraphObjList.Add(text);
            }
        }

        private string Graph_PointValueEvent(ZedGraphControl sender, GraphPane pane, CurveItem curve, int iPt)
        {
            var tag = curve[iPt].Tag;

            var marker = tag as ElevationGraphProfile.Marker;
            if (marker != null)
            {
                if (double.IsNaN(marker.Alt))
                    return "";
                var tip = MarkerTip(marker);
                return marker.Breach == ElevationGraphFenceBreach.None
                    ? tip
                    : tip + "\n" + string.Format(Strings.ElevationGraphOutsideFence, BreachText(marker.Breach));
            }

            var sample = tag as ElevationGraphProfile.Sample;
            if (sample != null)
            {
                var dist = FormatDistance(sample.Dist);
                if (double.IsNaN(sample.Alt))
                    return double.IsNaN(sample.Terrain)
                        ? ""
                        : string.Format(Strings.ElevationGraphSampleTipTerrainOnly, dist,
                            FormatAltitude(sample.Terrain));
                if (double.IsNaN(sample.Terrain))
                    return string.Format(Strings.ElevationGraphSampleTipNoTerrain, dist, FormatAltitude(sample.Alt));
                return string.Format(Strings.ElevationGraphSampleTip, dist, FormatAltitude(sample.Alt),
                    FormatAltitude(sample.Terrain), FormatAltitude(sample.Alt - sample.Terrain));
            }

            // a fence line carries its own label
            return tag as string ?? "";
        }

        private string MarkerTip(ElevationGraphProfile.Marker marker)
        {
            if (marker.Item == null)
                return string.Format(Strings.ElevationGraphHomeTip, FormatAltitude(marker.Alt));

            var clearance = marker.Alt - marker.Terrain;
            var rally = profile.RallyPoints.Contains(marker);
            if (rally && profile.RallyChain)
            {
                // on its own line, a rally point is not off any path
                return double.IsNaN(clearance)
                    ? string.Format(Strings.ElevationGraphRallyChainTipNoTerrain, marker.Item.Row,
                        FormatAltitude(marker.Alt))
                    : string.Format(Strings.ElevationGraphRallyChainTip, marker.Item.Row,
                        FormatAltitude(marker.Alt), FormatAltitude(clearance));
            }

            if (rally)
            {
                return double.IsNaN(clearance)
                    ? string.Format(Strings.ElevationGraphRallyTipNoTerrain, marker.Item.Row,
                        FormatAltitude(marker.Alt), FormatDistance(marker.OffPath))
                    : string.Format(Strings.ElevationGraphRallyTip, marker.Item.Row, FormatAltitude(marker.Alt),
                        FormatAltitude(clearance), FormatDistance(marker.OffPath));
            }

            return double.IsNaN(clearance)
                ? string.Format(Strings.ElevationGraphWaypointTipNoTerrain, marker.Item.Row,
                    FormatAltitude(marker.Alt))
                : string.Format(Strings.ElevationGraphWaypointTip, marker.Item.Row, FormatAltitude(marker.Alt),
                    FormatAltitude(clearance));
        }

        /// <summary>How a point is outside the fence, as a list: "above the ceiling, outside the circle".</summary>
        private static string BreachText(ElevationGraphFenceBreach breach)
        {
            var reasons = new List<string>();
            if ((breach & ElevationGraphFenceBreach.AboveCeiling) != 0)
                reasons.Add(Strings.ElevationGraphBreachCeiling);
            if ((breach & ElevationGraphFenceBreach.BelowFloor) != 0)
                reasons.Add(Strings.ElevationGraphBreachFloor);
            if ((breach & ElevationGraphFenceBreach.OutsideCircle) != 0)
                reasons.Add(Strings.ElevationGraphBreachCircle);
            if ((breach & ElevationGraphFenceBreach.OutsideInclusion) != 0)
                reasons.Add(Strings.ElevationGraphBreachInclusion);
            if ((breach & ElevationGraphFenceBreach.InsideExclusion) != 0)
                reasons.Add(Strings.ElevationGraphBreachExclusion);
            return string.Join(", ", reasons);
        }

        private void Graph_MouseMove(object sender, MouseEventArgs e)
        {
            if (profile == null || profile.Samples.Count == 0 || HoverChanged == null)
                return;

            var pane = graph.GraphPane;
            if (!pane.Chart.Rect.Contains(e.Location))
            {
                HoverChanged(this, null);
                return;
            }

            double x, y;
            pane.ReverseTransform(e.Location, out x, out y);
            HoverChanged(this, profile.PositionAt(x / distScale));
        }

        // Dragging the title bar works like the splitter above the waypoint list: a bar follows the
        // mouse while the button is down, and the graph takes its new height once, on release,
        // instead of being laid out and redrawn at every pixel of the drag.
        private void Header_MouseDown(object sender, MouseEventArgs e)
        {
            // the second press of a double click toggles Expanded instead
            if (e.Button != MouseButtons.Left || e.Clicks != 1)
                return;
            // coordinates of the control under the mouse, which holds the mouse and stays put
            // until the button is released
            dragging = true;
            dragStartY = e.Y;
            dragStartHeight = Height;
            dragHeight = Height;
        }

        private void Header_MouseMove(object sender, MouseEventArgs e)
        {
            if (!dragging)
                return;

            var height = ClampHeight(dragStartHeight + dragStartY - e.Y);
            if (height == dragHeight && dragBarShown)
                return;

            HideDragBar();
            dragHeight = height;
            ShowDragBar();
        }

        private void Header_MouseUp(object sender, MouseEventArgs e)
        {
            if (dragging)
                EndDrag(true);
        }

        private void Header_MouseCaptureChanged(object sender, EventArgs e)
        {
            // the mouse was taken away mid-drag (another window, a dialog): nothing changes
            if (dragging && !((Control) sender).Capture)
                EndDrag(false);
        }

        private void EndDrag(bool apply)
        {
            dragging = false;
            HideDragBar();
            if (apply && dragHeight != dragStartHeight)
                HeightDragged?.Invoke(this, dragHeight);
        }

        private int ClampHeight(int height)
        {
            return Math.Max(MinimumHeight, Math.Min(height, MaximumHeight));
        }

        /// <summary>
        /// The bar is drawn straight on the screen, as a splitter's is, and drawing it a second
        /// time at the same place takes it away again. The graph keeps its bottom edge, so the bar
        /// sits where the top edge will be.
        /// </summary>
        private void ShowDragBar()
        {
            if (dragBarShown || Parent == null)
                return;
            dragBar = Parent.RectangleToScreen(
                new Rectangle(Left, Bottom - dragHeight - 1, Width, DragBarThickness));
            ControlPaint.FillReversibleRectangle(dragBar, BackColor);
            dragBarShown = true;
        }

        private void HideDragBar()
        {
            if (!dragBarShown)
                return;
            ControlPaint.FillReversibleRectangle(dragBar, BackColor);
            dragBarShown = false;
        }

        /// <summary>Metres along the path as km (or miles) once the path is long, else the distance unit.</summary>
        private void ChooseDistanceUnit(double totalMetres)
        {
            distScale = CurrentState.multiplierdist;
            distUnit = string.IsNullOrEmpty(CurrentState.DistanceUnit) ? "m" : CurrentState.DistanceUnit;

            if (distUnit == "m" && totalMetres > 5000)
            {
                distScale = 0.001;
                distUnit = "km";
            }
            else if (distUnit == "ft" && totalMetres * distScale > 15000)
            {
                distScale = 1 / 1609.344;
                distUnit = "mi";
            }
        }

        private static string AltUnit
        {
            get { return string.IsNullOrEmpty(CurrentState.AltUnit) ? "m" : CurrentState.AltUnit; }
        }

        private static double ToAxis(double metres)
        {
            return double.IsNaN(metres) ? PointPair.Missing : metres * CurrentState.multiplieralt;
        }

        private string FormatDistance(double metres)
        {
            return (metres * distScale).ToString(distScale < 0.01 ? "0.00" : "0") + " " + distUnit;
        }

        private static string FormatAltitude(double metres)
        {
            return (metres * CurrentState.multiplieralt).ToString("0") + " " + AltUnit;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                toolTip.Dispose();
            base.Dispose(disposing);
            // once the labels that use it are gone
            if (disposing)
                boldFont.Dispose();
        }
    }
}
