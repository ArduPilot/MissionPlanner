using Microsoft.VisualStudio.TestTools.UnitTesting;
using MissionPlanner.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionPlanner.Utilities.Tests
{
    [TestClass()]
    public class FirmwareTests
    {
        [TestMethod()]
        public void getFWListTest()
        {
            var list = new Firmware().getFWList();

            Console.WriteLine(list.ToJSON());

            if (list.Count > 0)
                return;
            Assert.Fail();
        }
    }
}