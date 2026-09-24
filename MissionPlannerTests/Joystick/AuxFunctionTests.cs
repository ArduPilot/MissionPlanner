using Microsoft.VisualStudio.TestTools.UnitTesting;
using MissionPlanner.ArduPilot;
using MissionPlanner.Joystick;
using System;
using System.Linq;

namespace MissionPlanner.Joystick.Tests
{
    [TestClass()]
    public class AuxFunctionTests
    {
        [TestMethod()]
        public void CopterListIsVehicleSpecific()
        {
            var copter = AuxFunction.GetList(Firmwares.ArduCopter2);

            Console.WriteLine("ArduCopter: " + string.Join(", ", copter.Select(a => a.Key + ":" + a.Value)));

            Assert.IsTrue(copter.Count > 20, "expected a populated RCx_OPTION list for copter");
            Assert.IsFalse(copter.Any(a => a.Key == 0), "0:Do Nothing must be excluded");
            Assert.AreEqual(copter.Count, copter.Select(a => a.Key).Distinct().Count(), "duplicate option numbers");
            Assert.IsTrue(copter.Any(a => a.Key == 32), "copter should list 32:Motor Interlock");
            Assert.IsFalse(copter.Any(a => a.Key == 64), "copter should not list 64:Reverse Throttle (plane only)");
        }

        [TestMethod()]
        public void PlaneListIsVehicleSpecific()
        {
            var plane = AuxFunction.GetList(Firmwares.ArduPlane);

            Console.WriteLine("ArduPlane: " + string.Join(", ", plane.Select(a => a.Key + ":" + a.Value)));

            Assert.IsTrue(plane.Count > 20, "expected a populated RCx_OPTION list for plane");
            Assert.IsFalse(plane.Any(a => a.Key == 0), "0:Do Nothing must be excluded");
            Assert.AreEqual(plane.Count, plane.Select(a => a.Key).Distinct().Count(), "duplicate option numbers");
            Assert.IsTrue(plane.Any(a => a.Key == 64), "plane should list 64:Reverse Throttle");
            Assert.IsFalse(plane.Any(a => a.Key == 32), "plane should not list 32:Motor Interlock (copter only)");
        }

        [TestMethod()]
        public void CopterAndPlaneListsDiffer()
        {
            var copter = AuxFunction.GetList(Firmwares.ArduCopter2).Select(a => a.Key).ToList();
            var plane = AuxFunction.GetList(Firmwares.ArduPlane).Select(a => a.Key).ToList();

            Assert.IsTrue(copter.Except(plane).Any(), "copter should have options plane does not");
            Assert.IsTrue(plane.Except(copter).Any(), "plane should have options copter does not");
            // shared options should be present in both (e.g. 9:Camera Trigger, 28:Relay On/Off)
            Assert.IsTrue(copter.Contains(9) && plane.Contains(9));
            Assert.IsTrue(copter.Contains(28) && plane.Contains(28));
        }

        [TestMethod()]
        public void GetNameResolvesKnownAndUnknown()
        {
            Assert.IsNotNull(AuxFunction.GetName(Firmwares.ArduPlane, 9));
            Assert.IsNull(AuxFunction.GetName(Firmwares.ArduPlane, 65000));
        }

        [TestMethod()]
        public void TriggerListCoversEnum()
        {
            var triggers = AuxFunction.GetTriggerList();
var enumValues = Enum.GetValues(typeof(auxfunctiontrigger)).Cast<auxfunctiontrigger>().Select(a => (int) a).OrderBy(a => a).ToList();

            CollectionAssert.AreEqual(enumValues, triggers.Select(a => a.Key).OrderBy(a => a).ToList());
        }
    }
}
