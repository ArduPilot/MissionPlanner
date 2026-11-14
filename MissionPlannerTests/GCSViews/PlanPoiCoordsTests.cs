using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MissionPlanner.GCSViews;

namespace MissionPlanner.GCSViews.Tests
{
    /// <summary>
    /// The Plan page POI menu offers Coords, as the Data page does under Add POI, so a POI can be
    /// added by typed coordinates (#3781, d318bcd22). Both pages open CoordsInputBox.AddPOI.
    /// </summary>
    [TestClass]
    public class PlanPoiCoordsTests
    {
        [TestMethod]
        public void PoiMenuListsCoordsAfterAdd()
        {
            var fp = new FlightPlanner();

            var items = fp.pOIToolStripMenuItem.DropDownItems.Cast<ToolStripItem>().ToArray();

            CollectionAssert.AreEqual(new ToolStripItem[]
            {
                fp.poiaddToolStripMenuItem, fp.poiatcoordsToolStripMenuItem, fp.poideleteToolStripMenuItem,
                fp.poieditToolStripMenuItem
            }, items);
            Assert.AreEqual("Coords", fp.poiatcoordsToolStripMenuItem.Text);
        }

        [TestMethod]
        public void CoordsItemIsWiredToItsHandler()
        {
            var fp = new FlightPlanner();

            var key = typeof(ToolStripItem).GetField("EventClick", BindingFlags.Static | BindingFlags.NonPublic)
                .GetValue(null);
            var events = (EventHandlerList) typeof(Component)
                .GetProperty("Events", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(fp.poiatcoordsToolStripMenuItem);
            var handler = events[key];

            Assert.IsNotNull(handler, "Coords has no Click handler");
            Assert.AreEqual(nameof(FlightPlanner.poiatcoordsToolStripMenuItem_Click),
                handler.GetInvocationList().Single().Method.Name);
        }
    }
}
