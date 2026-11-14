using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MissionPlanner.Utilities;
using ZedGraph;

namespace MissionPlanner.Controls.Tests
{
    /// <summary>
    /// A zoom or pan of the Plan page's elevation graph stays through a redraw (an altitude
    /// changed in the grid or on the graph, a point moved on the map, terrain arriving): the
    /// same stretch of the path and the same altitudes stay in view, in km once the path has
    /// grown past 5 km and in feet once the altitude unit is feet, and Un-Zoom or Undo All Zoom
    /// goes back to the whole of the new graph. A graph of something else, or with none of its
    /// path left in the view, is shown whole.
    /// </summary>
    [TestClass]
    public class ElevationGraphZoomTests
    {
        // a made up position: round numbers, nowhere in particular
        private const double Lat0 = 10.0;
        private const double Lng0 = 20.0;
        private const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;

        private float altScale;
        private string altUnit;
        private float distScale;
        private string distUnit;

        [TestInitialize]
        public void MetresOnTheAxes()
        {
            altScale = CurrentState.multiplieralt;
            altUnit = CurrentState.AltUnit;
            distScale = CurrentState.multiplierdist;
            distUnit = CurrentState.DistanceUnit;
            CurrentState.multiplieralt = 1;
            CurrentState.AltUnit = "m";
            CurrentState.multiplierdist = 1;
            CurrentState.DistanceUnit = "m";
        }

        [TestCleanup]
        public void UnitsBack()
        {
            CurrentState.multiplieralt = altScale;
            CurrentState.AltUnit = altUnit;
            CurrentState.multiplierdist = distScale;
            CurrentState.DistanceUnit = distUnit;
        }

        private static double? Flat(double lat, double lng)
        {
            return 100;
        }

        /// <summary>
        /// Home at 100 m on flat ground at 100 m, and a waypoint at each of <paramref name="lats"/>
        /// degrees north of home, 50 m above home, the second at <paramref name="second"/> m.
        /// </summary>
        private static ElevationGraphProfile Plan(double second, params double[] lats)
        {
            var input = new ElevationGraphInput {Home = new PointLatLngAlt(Lat0, Lng0, 100, "H")};
            for (int i = 0; i < lats.Length; i++)
                input.Waypoints.Add(new ElevationGraphItem
                {
                    Row = i + 1, Label = (i + 1).ToString(), Lat = Lat0 + lats[i], Lng = Lng0,
                    Alt = i == 1 ? second : 50
                });
            return ElevationGraphProfile.Build(input, Flat);
        }

        /// <summary>A graph 800 x 300 showing a profile, and its ZedGraph control.</summary>
        private static PlanElevationGraph Shown(ElevationGraphProfile profile, out ZedGraphControl graph)
        {
            var view = new PlanElevationGraph {Size = new Size(800, 300)};
            view.ShowProfile(profile, false);
            graph = (ZedGraphControl) typeof(PlanElevationGraph).GetField("graph", Private).GetValue(view);
            var rect = graph.GraphPane.Chart.Rect;
            Assert.IsTrue(rect.Width > 100 && rect.Height > 50, "chart rect " + rect);
            return view;
        }

        /// <summary>The drawn curve with a label (not its empty stand-in in the legend).</summary>
        private static LineItem Curve(GraphPane pane, string label)
        {
            return pane.CurveList.OfType<LineItem>().Single(c => c.Label.Text == label && c.Points.Count > 0);
        }

        /// <summary>The mouse wheel turned away from the user over the middle of the chart: ZedGraph zooms in a step each notch.</summary>
        private static void Wheel(ZedGraphControl graph, int notches)
        {
            var rect = graph.GraphPane.Chart.Rect;
            var middle = Point.Round(new PointF(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2));
            var handler = typeof(ZedGraphControl).GetMethod("ZedGraphControl_MouseWheel", Private);
            for (int i = 0; i < notches; i++)
                handler.Invoke(graph, new object[] {graph, new MouseEventArgs(MouseButtons.None, 0, middle.X, middle.Y, 120)});
        }

        /// <summary>
        /// A box dragged over the graph (a zoom) or the graph dragged along (a pan), as ZedGraph
        /// leaves it: the view before it on the zoom stack, the new one fixed on both axes.
        /// </summary>
        private static void ViewTo(ZedGraphControl graph, ZoomState.StateType type, double xmin, double xmax,
            double ymin, double ymax)
        {
            var pane = graph.GraphPane;
            pane.ZoomStack.Push(pane, type);
            pane.XAxis.Scale.Min = xmin;
            pane.XAxis.Scale.Max = xmax;
            pane.YAxis.Scale.Min = ymin;
            pane.YAxis.Scale.Max = ymax;
            graph.AxisChange();
        }

        private static Tuple<double, double> Range(Axis axis)
        {
            return Tuple.Create(axis.Scale.Min, axis.Scale.Max);
        }

        /// <summary>The whole of a graph, in metres along the axis: the path with a little room either side, all altitudes.</summary>
        private static void AssertWhole(GraphPane pane, ElevationGraphProfile profile)
        {
            Assert.IsFalse(pane.IsZoomed, "nothing to un-zoom");
            var length = profile.TotalDistance > 0 ? profile.TotalDistance : 1;
            Assert.AreEqual(-length * 0.01, pane.XAxis.Scale.Min, 1e-9);
            Assert.AreEqual(length * 1.01, pane.XAxis.Scale.Max, 1e-9);
            Assert.IsTrue(pane.YAxis.Scale.MinAuto && pane.YAxis.Scale.MaxAuto, "altitudes on auto");
        }

        [TestMethod]
        public void Redraw_WithoutAZoom_ShowsTheWholeOfEachPath()
        {
            ZedGraphControl graph;
            var first = Plan(100, 0.01, 0.02, 0.03);
            using (var view = Shown(first, out graph))
            {
                AssertWhole(graph.GraphPane, first);
                var longer = Plan(100, 0.01, 0.02, 0.04);
                view.ShowProfile(longer, false);
                AssertWhole(graph.GraphPane, longer);
            }
        }

        [TestMethod]
        public void Redraw_AfterAnEditWhileZoomed_KeepsTheView_AndUndoAllZoomShowsTheWholeNewGraph()
        {
            ZedGraphControl graph;
            using (var view = Shown(Plan(100, 0.01, 0.02, 0.03), out graph))
            {
                var pane = graph.GraphPane;
                var whole = Range(pane.XAxis);
                Wheel(graph, 2);
                Assert.IsTrue(pane.IsZoomed);
                var x = Range(pane.XAxis);
                var y = Range(pane.YAxis);
                Assert.IsTrue(x.Item2 - x.Item1 < (whole.Item2 - whole.Item1) * 0.9, "zoomed in to " + x);

                // waypoint 2 lowered from 100 to 70 m above home: the same view, the point lower in it
                var edited = Plan(70, 0.01, 0.02, 0.03);
                view.ShowProfile(edited, false);
                Assert.AreEqual(x, Range(pane.XAxis));
                Assert.AreEqual(y, Range(pane.YAxis));
                Assert.AreEqual(170, Curve(pane, Strings.ElevationGraphWaypoints)[2].Y, 1e-9);
                Assert.AreEqual(1, pane.ZoomStack.Count);
                Assert.AreEqual(ZoomState.StateType.WheelZoom, pane.ZoomStack.Top.Type);

                // terrain arriving redraws the same plan: still the same view
                view.ShowProfile(edited, false);
                Assert.AreEqual(x, Range(pane.XAxis));
                Assert.AreEqual(y, Range(pane.YAxis));

                // Undo All Zoom: the whole of the new graph, all of its altitudes
                graph.ZoomOutAll(pane);
                Assert.IsFalse(pane.IsZoomed);
                Assert.AreEqual(whole, Range(pane.XAxis));
                Assert.IsTrue(pane.YAxis.Scale.MinAuto && pane.YAxis.Scale.MaxAuto, "altitudes on auto");
                Assert.IsTrue(pane.YAxis.Scale.Min <= 100 && pane.YAxis.Scale.Max >= 170, "altitudes " + Range(pane.YAxis));

                // and the next redraw shows the whole graph as well
                view.ShowProfile(edited, false);
                AssertWhole(pane, edited);
            }
        }

        [TestMethod]
        public void Redraw_PastFiveKilometres_KeepsTheSameStretchInKm()
        {
            ZedGraphControl graph;
            using (var view = Shown(Plan(100, 0.01, 0.02, 0.044), out graph))
            {
                var pane = graph.GraphPane;
                Assert.AreEqual(string.Format(Strings.ElevationGraphDistanceAxis, "m"), pane.XAxis.Title.Text);
                ViewTo(graph, ZoomState.StateType.Zoom, 1000, 2000, 120, 180);

                // the last waypoint moved out past 5 km: the axis is in km now
                var longer = Plan(100, 0.01, 0.02, 0.05);
                view.ShowProfile(longer, false);
                Assert.AreEqual(string.Format(Strings.ElevationGraphDistanceAxis, "km"), pane.XAxis.Title.Text);
                Assert.AreEqual(1, pane.XAxis.Scale.Min, 1e-9);
                Assert.AreEqual(2, pane.XAxis.Scale.Max, 1e-9);
                Assert.AreEqual(120, pane.YAxis.Scale.Min, 1e-9);
                Assert.AreEqual(180, pane.YAxis.Scale.Max, 1e-9);

                // Un-Zoom goes back to the whole path, in km
                graph.ZoomOut(pane);
                Assert.IsFalse(pane.IsZoomed);
                Assert.AreEqual(-longer.TotalDistance * 0.001 * 0.01, pane.XAxis.Scale.Min, 1e-9);
                Assert.AreEqual(longer.TotalDistance * 0.001 * 1.01, pane.XAxis.Scale.Max, 1e-9);

                // zoomed again, and the path back under 5 km: metres again
                ViewTo(graph, ZoomState.StateType.Zoom, 1, 2, 120, 180);
                view.ShowProfile(Plan(100, 0.01, 0.02, 0.044), false);
                Assert.AreEqual(string.Format(Strings.ElevationGraphDistanceAxis, "m"), pane.XAxis.Title.Text);
                Assert.AreEqual(1000, pane.XAxis.Scale.Min, 1e-6);
                Assert.AreEqual(2000, pane.XAxis.Scale.Max, 1e-6);
            }
        }

        [TestMethod]
        public void Redraw_InFeet_KeepsTheSameAltitudes()
        {
            ZedGraphControl graph;
            using (var view = Shown(Plan(100, 0.01, 0.02, 0.03), out graph))
            {
                var pane = graph.GraphPane;
                ViewTo(graph, ZoomState.StateType.Zoom, 1000, 2000, 120, 180);

                // the altitude unit changed on the Config screen; the Plan page redraws on its return
                CurrentState.multiplieralt = 3.2808399f;
                CurrentState.AltUnit = "ft";
                view.ShowProfile(Plan(100, 0.01, 0.02, 0.03), false);
                var feet = (double) CurrentState.multiplieralt;
                Assert.AreEqual(120 * feet, pane.YAxis.Scale.Min, 1e-6);
                Assert.AreEqual(180 * feet, pane.YAxis.Scale.Max, 1e-6);
                Assert.AreEqual(1000, pane.XAxis.Scale.Min, 1e-9);
                Assert.AreEqual(2000, pane.XAxis.Scale.Max, 1e-9);
                Assert.AreEqual(200 * feet, Curve(pane, Strings.ElevationGraphWaypoints)[2].Y, 1e-6);
            }
        }

        [TestMethod]
        public void Redraw_OfSomethingElse_OrWithNoneOfThePathInView_ShowsTheWholeGraph()
        {
            ZedGraphControl graph;
            using (var view = Shown(Plan(100, 0.01, 0.02, 0.03), out graph))
            {
                var pane = graph.GraphPane;

                // the rally points alone (no mission): their own line, not the path
                ViewTo(graph, ZoomState.StateType.Zoom, 1000, 2000, 120, 180);
                var input = new ElevationGraphInput {Home = new PointLatLngAlt(Lat0, Lng0, 100, "H")};
                input.RallyPoints.Add(new ElevationGraphItem {Row = 1, Label = "R1", Lat = Lat0 + 0.005, Lng = Lng0, Alt = 80});
                input.RallyPoints.Add(new ElevationGraphItem {Row = 2, Label = "R2", Lat = Lat0 + 0.015, Lng = Lng0, Alt = 80});
                var chain = ElevationGraphProfile.Build(input, Flat);
                Assert.IsTrue(chain.RallyChain);
                view.ShowProfile(chain, false);
                AssertWhole(pane, chain);

                // and from the rally points back to a path
                ViewTo(graph, ZoomState.StateType.Zoom, 200, 600, 120, 180);
                var plan = Plan(100, 0.01, 0.02, 0.03);
                view.ShowProfile(plan, false);
                AssertWhole(pane, plan);

                // panned to the far end of the path, then the path cut short of it
                ViewTo(graph, ZoomState.StateType.Pan, 2600, 3300, 120, 180);
                var shorter = Plan(100, 0.01, 0.02);
                view.ShowProfile(shorter, false);
                AssertWhole(pane, shorter);

                // part of the path still in the view: kept, and Un-Zoom still says Un-Pan
                ViewTo(graph, ZoomState.StateType.Pan, 2000, 3000, 120, 180);
                view.ShowProfile(plan, false);
                Assert.IsTrue(pane.IsZoomed);
                Assert.AreEqual(ZoomState.StateType.Pan, pane.ZoomStack.Top.Type);
                Assert.AreEqual(Tuple.Create(2000.0, 3000.0), Range(pane.XAxis));

                // no plan at all
                var empty = ElevationGraphProfile.Build(
                    new ElevationGraphInput {Home = new PointLatLngAlt(Lat0, Lng0, 100, "H")}, Flat);
                view.ShowProfile(empty, false);
                AssertWhole(pane, empty);
            }
        }
    }
}
