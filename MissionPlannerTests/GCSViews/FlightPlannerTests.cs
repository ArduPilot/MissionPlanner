using Microsoft.VisualStudio.TestTools.UnitTesting;
using MissionPlanner.GCSViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissionPlanner.Utilities;

namespace MissionPlanner.GCSViews.Tests
{
    [TestClass()]
    public class FlightPlannerTests
    {
        [TestMethod()]
        public void MakeRequestTest()
        {
            FlightPlanner flightPlanner = new FlightPlanner();

            var xmldoc = flightPlanner.MakeRequest("https://mesonet.agron.iastate.edu/cgi-bin/wms/iowa/rainfall.cgi?VERSION=1.1.1&REQUEST=GetCapabilities&SERVICE=WMS&");

            Console.WriteLine(xmldoc.ToJSON());

            Assert.IsNotNull(xmldoc);
        }
    }
}