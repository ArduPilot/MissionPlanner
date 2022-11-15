using Microsoft.VisualStudio.TestTools.UnitTesting;
using MissionPlanner.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionPlanner.Utilities.Tests
{
    [TestClass()]
    public class DownloadTests
    {
        [TestMethod()]
        public void getFilefromNetTest()
        {
            if (Utilities.Download.getFilefromNet("https://www.google.com/", Path.GetTempFileName()))
                return;

            Assert.Fail();
        }

        [TestMethod()]
        public void CheckHTTPFileExists()
        {
            if (Utilities.Download.CheckHTTPFileExists("https://github.com/ArduPilot/MissionPlanner/releases/download/betarelease/MissionPlannerBeta.zip"))
                return;

            Assert.Fail();
        }

        [TestMethod()]
        public void GetFileSize()
        {
            if (Utilities.Download.GetFileSize("https://github.com/ArduPilot/MissionPlanner/releases/download/betarelease/MissionPlannerBeta.zip") > 0)
                return;

            Assert.Fail();
        }
    }
}