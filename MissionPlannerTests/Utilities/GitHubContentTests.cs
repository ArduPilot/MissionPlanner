using Microsoft.VisualStudio.TestTools.UnitTesting;
using MissionPlanner.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MissionPlanner.Utilities.Tests
{
    [TestClass()]
    public class GitHubContentTests
    {
        [TestMethod()]
        public void GetDirContentTest()
        {
            var paramfiles = GitHubContent.GetDirContent("ArduPilot", "ardupilot", "/Tools/Frame_params/", ".param");

            Console.WriteLine(paramfiles);

            if (paramfiles.Count > 0)
                return;
            Assert.Fail();
        }
        [TestMethod()]
        public void GetFileContentTest()
        {
            var paramfiles = GitHubContent.GetDirContent("ArduPilot", "ardupilot", "/Tools/Frame_params/", ".param");
            var data = GitHubContent.GetFileContent("ArduPilot", "ardupilot", (paramfiles[0].path));

            Console.WriteLine(data);

            if (data.Length > 0)
                return;
            Assert.Fail();
        }
    }
}