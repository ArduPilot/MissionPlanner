using DroneCAN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MissionPlannerTests
{
    [TestClass()]
    public class DroneCANTests
    {
        [TestMethod()]
        public void DroneCAN1()
        {
            var ans = new DroneCAN.DroneCAN().LookForUpdate("com.cubepilot.test", 1.0, false);

            Console.WriteLine(ans);

            if (ans == String.Empty)
                return;
            Assert.Fail();
        }
        [TestMethod()]
        public void DroneCAN2()
        {
            var ans = new DroneCAN.DroneCAN().LookForUpdate("com.cubepilot.herepro", 19.137, false);
            Console.WriteLine(ans);
            if (ans != String.Empty)
                return;
            Assert.Fail();
        }
    }
}
