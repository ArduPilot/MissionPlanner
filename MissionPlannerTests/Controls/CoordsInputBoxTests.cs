using Microsoft.VisualStudio.TestTools.UnitTesting;
using MissionPlanner.GCSViews;
using System.Reflection;

namespace MissionPlanner.Controls.Tests
{
    /// <summary>
    /// The coordinate text shared by the Plan page home box, the POI labels and the SITL home.
    /// 0, 0 is a position like any other in Format and TryParse; only the Plan page home box
    /// reads it as "home not set". Every position here is made up: round numbers in UTM zone 34.
    /// </summary>
    [TestClass]
    public class CoordsInputBoxTests
    {
        private const string GEO = "GEO";
        private const string UTM = "UTM";
        private const string MGRS = "MGRS";

        // a made up point north of the equator (zone 34, band P) and its mirror south of it (band L)
        private const double Lat = 10.5;
        private const double Lng = 20.25;

        [TestMethod]
        public void Format_Origin_IsAPositionLikeAnyOther()
        {
            Assert.AreEqual("0.0000000;0.0000000", CoordsInputBox.Format(GEO, 0, 0));
            StringAssert.StartsWith(CoordsInputBox.Format(UTM, 0, 0), "31N ");
            Assert.AreNotEqual("", CoordsInputBox.Format(MGRS, 0, 0));
        }

        [TestMethod]
        public void TryParse_Origin_IsAcceptedInEveryFrame()
        {
            foreach (var system in new[] {GEO, UTM, MGRS})
            {
                var text = CoordsInputBox.Format(system, 0, 0);
                Assert.IsTrue(CoordsInputBox.TryParse(system, text, out var lat, out var lng, out _),
                    system + " " + text);
                Assert.AreEqual(0, lat, 1e-5, system);
                Assert.AreEqual(0, lng, 1e-5, system);
            }
        }

        [TestMethod]
        public void UtmAndMgrs_RoundTripToTheMetre()
        {
            foreach (var system in new[] {UTM, MGRS})
            {
                foreach (var south in new[] {false, true})
                {
                    var wanted = south ? -Lat : Lat;
                    var text = CoordsInputBox.Format(system, wanted, Lng);
                    Assert.IsTrue(CoordsInputBox.TryParse(system, text, out var lat, out var lng, out _),
                        system + " " + text);
                    Assert.AreEqual(wanted, lat, 2e-5, system + " " + text);
                    Assert.AreEqual(Lng, lng, 2e-5, system + " " + text);
                }
            }
        }

        [TestMethod]
        public void PlanHomeBox_ShowsNothingForAnUnsetHome()
        {
            var planner = new FlightPlanner();
            planner.coords1.System = UTM;
            Assert.AreEqual(UTM, planner.coords1.System);

            var format = typeof(FlightPlanner).GetMethod("FormatHomeCoord",
                BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.AreEqual("", format.Invoke(planner, new object[] {0.0, 0.0}),
                "an unset home is 0, 0 and must show an empty box, not a grid reference");
            StringAssert.StartsWith((string) format.Invoke(planner, new object[] {Lat, Lng}), "34P ");
        }

        [TestMethod]
        public void TryParse_Utm_RefusesABandThatMissesTheLatitude()
        {
            // a point south of the equator is in band L here; typed with S "for south" it would land
            // far north, because the letter is the latitude band and S is a northern one
            var south = CoordsInputBox.Format(UTM, -Lat, Lng);
            StringAssert.StartsWith(south, "34L ");
            Assert.IsTrue(CoordsInputBox.TryParse(UTM, south, out var lat, out var lng, out _), south);
            Assert.AreEqual(-Lat, lat, 2e-5);
            Assert.AreEqual(Lng, lng, 2e-5);
            Assert.IsFalse(CoordsInputBox.TryParse(UTM, "34S" + south.Substring(3), out _, out _, out _));

            // a point just inside a band edge, rounded to whole metres, still reads back
            var text = CoordsInputBox.Format(UTM, 56.0001, Lng);
            StringAssert.StartsWith(text, "34V ");
            Assert.IsTrue(CoordsInputBox.TryParse(UTM, text, out lat, out _, out _), text);
            Assert.AreEqual(56.0001, lat, 1e-4);
        }

        [TestMethod]
        public void TryParse_Geo_RefusesADecimalComma()
        {
            // "10,5;7" would be lat 10, long 5, alt 7; "1,5" would be read as 15
            Assert.IsFalse(CoordsInputBox.TryParse(GEO, "10,5;7", out _, out _, out _));
            Assert.IsFalse(CoordsInputBox.TryParse(GEO, "1,5;7", out _, out _, out _));

            Assert.IsTrue(CoordsInputBox.TryParse(GEO, "10.5,7", out var lat, out var lng, out var alt));
            Assert.AreEqual(10.5, lat, 1e-9);
            Assert.AreEqual(7, lng, 1e-9);
            Assert.IsNull(alt);
            Assert.IsTrue(CoordsInputBox.TryParse(GEO, "10.5;7;100", out lat, out lng, out alt));
            Assert.AreEqual(100, alt.Value, 1e-9);
        }

        [TestMethod]
        public void TryParse_Geo_RefusesACommaBetweenWholeNumbersAmongSpaces()
        {
            // "10,5 7" would be read as lat 10, long 5, alt 7; with a decimal comma it means lat 10.5,
            // long 7
            foreach (var text in new[] {"10,5 7", "10 20,5", "10,5, 7", "-10,5 20", "10,5 20,4", "10,5\t7"})
                Assert.IsFalse(CoordsInputBox.TryParse(GEO, text, out _, out _, out _), text);

            // what reads only one way parses as it did
            Assert.IsTrue(CoordsInputBox.TryParse(GEO, "10.5,20.25", out var lat, out var lng, out var alt));
            Assert.AreEqual(10.5, lat, 1e-9);
            Assert.AreEqual(20.25, lng, 1e-9);
            Assert.IsNull(alt);
            Assert.IsTrue(CoordsInputBox.TryParse(GEO, "10.5, 20.25", out lat, out lng, out alt));
            Assert.AreEqual(20.25, lng, 1e-9);
            Assert.IsTrue(CoordsInputBox.TryParse(GEO, "10.5,20.25,100", out lat, out lng, out alt));
            Assert.AreEqual(100, alt.Value, 1e-9);
            Assert.IsTrue(CoordsInputBox.TryParse(GEO, "10.5 20.25 7", out lat, out lng, out alt));
            Assert.AreEqual(7, alt.Value, 1e-9);

            // no space: the comma can only separate
            Assert.IsTrue(CoordsInputBox.TryParse(GEO, "10,5", out lat, out lng, out alt));
            Assert.AreEqual(10, lat, 1e-9);
            Assert.AreEqual(5, lng, 1e-9);

            // a number with a decimal point holds no decimal comma
            Assert.IsTrue(CoordsInputBox.TryParse(GEO, "10.5,20.25 100", out lat, out lng, out alt));
            Assert.AreEqual(20.25, lng, 1e-9);
            Assert.AreEqual(100, alt.Value, 1e-9);

            // UTM is not GEO: easting and northing are whole metres, a comma between them separates
            var utm = CoordsInputBox.Format(UTM, Lat, Lng);
            var parts = utm.Split(' ');
            Assert.AreEqual(3, parts.Length, utm);
            Assert.IsTrue(CoordsInputBox.TryParse(UTM, parts[0] + " " + parts[1] + "," + parts[2], out lat, out lng, out alt));
            Assert.IsTrue(CoordsInputBox.TryParse(UTM, utm, out var lat2, out var lng2, out alt));
            Assert.AreEqual(lat2, lat, 1e-9);
            Assert.AreEqual(lng2, lng, 1e-9);
        }

        [TestMethod]
        public void TryParseAltitude_TakesADecimalCommaButNotAThousandsSeparator()
        {
            var culture = System.Threading.Thread.CurrentThread.CurrentCulture;
            try
            {
                foreach (var name in new[] {"en-US", "de-DE"})
                {
                    System.Threading.Thread.CurrentThread.CurrentCulture =
                        System.Globalization.CultureInfo.GetCultureInfo(name);

                    Assert.IsTrue(CoordsInputBox.TryParseAltitude("12.5", out var alt), name);
                    Assert.AreEqual(12.5, alt, 1e-9, name);
                    Assert.IsTrue(CoordsInputBox.TryParseAltitude("12,5", out alt), name);
                    Assert.AreEqual(12.5, alt, 1e-9, name);
                    Assert.IsTrue(CoordsInputBox.TryParseAltitude(" -3,25 ", out alt), name);
                    Assert.AreEqual(-3.25, alt, 1e-9, name);
                    Assert.IsTrue(CoordsInputBox.TryParseAltitude("1.500", out alt), name);
                    Assert.AreEqual(1.5, alt, 1e-9, name);

                    // a thousands separator to many users: refused, not read as 1.5
                    Assert.IsFalse(CoordsInputBox.TryParseAltitude("1,500", out alt), name);
                    Assert.IsFalse(CoordsInputBox.TryParseAltitude("-1,500", out alt), name);
                    Assert.IsFalse(CoordsInputBox.TryParseAltitude("1.500,5", out alt), name);
                    Assert.IsFalse(CoordsInputBox.TryParseAltitude("abc", out alt), name);
                    Assert.IsFalse(CoordsInputBox.TryParseAltitude("", out alt), name);
                }
            }
            finally
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = culture;
            }
        }
    }
}
