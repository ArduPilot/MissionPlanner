using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MissionPlanner.Utilities.CoT;

namespace MissionPlanner.Controls.Tests
{
    [TestClass]
    public class CotOutputTests
    {
        static readonly DateTime Now = new DateTime(2026, 10, 27, 15, 21, 18, 600, DateTimeKind.Utc);

        // SITL's home at CMAC, 100 m up, flying east at 10 m/s
        static CotVehicle Copter(object connection = null, uint sysid = 1, byte compid = 1)
        {
            return new CotVehicle
            {
                Connection = connection ?? "link",
                SysId = sysid,
                CompId = compid,
                Type = MAVLink.MAV_TYPE.QUADROTOR,
                Autopilot = MAVLink.MAV_AUTOPILOT.ARDUPILOTMEGA,
                Lat = -35.363262,
                Lng = 149.165237,
                AltMslM = 684.07,
                GroundSpeedMs = 9.96,
                CourseDeg = 90,
                LastPacketUtc = Now.AddSeconds(-1)
            };
        }

        static CotIdentity Named(CotVehicle vehicle)
        {
            return new CotIdentity { Uid = "MP-UAS-" + vehicle.SysId, Callsign = "UAS-" + vehicle.SysId };
        }

        static @event One(CotVehicle vehicle, bool details = true, double stale = 120)
        {
            return new CotRound().Build(new[] { vehicle }, Named, "a-f-A-M-H-Q", details, Now, stale).Single();
        }

        [TestMethod]
        public void Hae_IsAboveTheEllipsoid_EGM96AtCmac()
        {
            var vehicle = Copter();
            var hae = CotVehicles.HeightAboveEllipsoid(vehicle);

            // EGM96 puts the geoid 19.4 m above the ellipsoid at CMAC
            Assert.AreEqual(684.07 + 19.4, hae, 0.06);
            Assert.AreEqual(hae.ToString("F2", CultureInfo.InvariantCulture), One(vehicle).point.hae);
        }

        [TestMethod]
        public void Read_GivesMetresAndMetresPerSecond_WhateverTheDisplayUnits()
        {
            var link = new MAVLinkInterface();
            var mav = link.MAVlist[1, 1];
            mav.apname = MAVLink.MAV_AUTOPILOT.ARDUPILOTMEGA;
            mav.cs.lat = -35.363262;
            mav.cs.lng = 149.165237;
            mav.cs.altasl = 684.07f;
            mav.cs.groundspeed = 10f;
            mav.cs.groundcourse = 90;

            var alt = CurrentState.multiplieralt;
            var speed = CurrentState.multiplierspeed;
            try
            {
                // feet and knots, as on 2026-10-27 when the output sent 2244.36 and 19.45
                CurrentState.multiplieralt = 3.2808399f;
                CurrentState.multiplierspeed = 1.94384449f;

                var vehicle = CotVehicles.Read(mav, link);

                Assert.AreEqual(684.07, vehicle.AltMslM, 0.01);
                Assert.AreEqual(10, vehicle.GroundSpeedMs, 0.01);
                Assert.AreEqual(90, vehicle.CourseDeg, 0.01);
                Assert.AreSame(link, vehicle.Connection);
            }
            finally
            {
                CurrentState.multiplieralt = alt;
                CurrentState.multiplierspeed = speed;
            }
        }

        [TestMethod]
        public void Read_EachVehicleFromItsOwnConnection()
        {
            var first = new MAVLinkInterface();
            var second = new MAVLinkInterface();
            first.MAVlist[1, 1].cs.lat = -35.36;
            second.MAVlist[2, 1].cs.lat = -35.40;

            Assert.AreEqual(-35.36, CotVehicles.Read(first.MAVlist[1, 1], first).Lat, 1e-9);
            Assert.AreEqual(-35.40, CotVehicles.Read(second.MAVlist[2, 1], second).Lat, 1e-9);
        }

        [TestMethod]
        public void Read_TakesTheGpsAccuracy()
        {
            var link = new MAVLinkInterface();
            var mav = link.MAVlist[1, 1];
            mav.cs.lat = -35.363262;
            mav.cs.lng = 149.165237;
            mav.lastvalidpacket = Now;
            typeof(CurrentState).GetProperty("gpsh_acc").SetValue(mav.cs, 1.54f);
            typeof(CurrentState).GetProperty("gpsv_acc").SetValue(mav.cs, 2.31f);

            var vehicle = CotVehicles.Read(mav, link);

            Assert.AreEqual(1.54, vehicle.HorizontalAccuracyM, 0.001);
            Assert.AreEqual("1.5", One(vehicle).point.ce);
            Assert.AreEqual("2.3", One(vehicle).point.le);
        }

        [TestMethod]
        public void Accuracy_NotReported_IsCoTsUnknown()
        {
            var point = One(Copter()).point;

            Assert.AreEqual("9999999.0", point.ce);
            Assert.AreEqual("9999999.0", point.le);
        }

        [TestMethod]
        public void Values_AreInMetresDegreesAndSeconds()
        {
            var cotevent = One(Copter());

            Assert.AreEqual("-35.3632620", cotevent.point.lat);
            Assert.AreEqual("149.1652370", cotevent.point.lon);
            Assert.AreEqual("90.00", cotevent.detail.track.course);
            Assert.AreEqual("9.96", cotevent.detail.track.speed);
            Assert.AreEqual("m-g", cotevent.how);
            Assert.AreEqual("2026-10-27T15:21:18.60Z", cotevent.time);
            Assert.AreEqual("2026-10-27T15:21:13.60Z", cotevent.start);
            Assert.AreEqual("2026-10-27T15:23:18.60Z", cotevent.stale);
        }

        [TestMethod]
        public void Times_IgnoreThePcsRegionalSettings()
        {
            var before = Thread.CurrentThread.CurrentCulture;
            try
            {
                foreach (var name in new[] { "fi-FI", "fa-IR", "ar-SA", "de-DE" })
                {
                    Thread.CurrentThread.CurrentCulture = new CultureInfo(name);
                    var cotevent = One(Copter());

                    Assert.AreEqual("2026-10-27T15:21:18.60Z", cotevent.time, name);
                    Assert.AreEqual("-35.3632620", cotevent.point.lat, name);
                    Assert.AreEqual("9.96", cotevent.detail.track.speed, name);
                }
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = before;
            }
        }

        [TestMethod]
        public void Stale_IsTwoMinutesOrThreeRounds()
        {
            Assert.AreEqual(120, CotVehicles.StaleSeconds(1));
            Assert.AreEqual(120, CotVehicles.StaleSeconds(10));
            Assert.AreEqual(120, CotVehicles.StaleSeconds(40));
            Assert.AreEqual(180, CotVehicles.StaleSeconds(60));
            Assert.AreEqual("2026-10-27T15:24:18.60Z", One(Copter(), stale: 180).stale);
        }

        [TestMethod]
        public void OnlyFlightControllersWithAPositionAndALink_AreReported()
        {
            var gimbal = Copter(compid: 154);
            gimbal.Autopilot = MAVLink.MAV_AUTOPILOT.INVALID;
            gimbal.Lat = gimbal.Lng = 0;

            var noFix = Copter(sysid: 2);
            noFix.Lat = noFix.Lng = 0;

            var silent = Copter(sysid: 3);
            silent.LastPacketUtc = Now.AddSeconds(-11);

            var playback = Copter(sysid: 4);
            playback.Playback = true;

            Assert.AreEqual(CotSkip.NotAVehicle, CotVehicles.Check(gimbal, Now));
            Assert.AreEqual(CotSkip.NoPosition, CotVehicles.Check(noFix, Now));
            Assert.AreEqual(CotSkip.Silent, CotVehicles.Check(silent, Now));
            Assert.AreEqual(CotSkip.Playback, CotVehicles.Check(playback, Now));
            Assert.AreEqual(CotSkip.None, CotVehicles.Check(Copter(), Now));

            var events = new CotRound().Build(new[] { Copter(), gimbal, noFix, silent, playback }, Named,
                "a-f-A-M-H-Q", true, Now, 120);

            Assert.AreEqual(1, events.Count, "only the copter: no 0, 0 twin from the gimbal");
            Assert.AreEqual("MP-UAS-1", events[0].uid);
        }

        [TestMethod]
        public void ATrackThatStops_IsSentAlreadyStale_InThatRoundAndTheNext()
        {
            var round = new CotRound();
            var copter = Copter();

            var first = round.Build(new[] { copter }, Named, "a-f-A-M-H-Q", true, Now, 120);
            Assert.AreEqual(1, first.Count);

            // the link goes quiet: an event stale at once, at the last position
            var later = Now.AddSeconds(20);
            var second = round.Build(new[] { copter }, Named, "a-f-A-M-H-Q", true, later, 120);
            Assert.AreEqual(1, second.Count);
            Assert.AreEqual("MP-UAS-1", second[0].uid);
            Assert.AreEqual(second[0].time, second[0].stale);
            Assert.AreEqual(first[0].point.lat, second[0].point.lat);

            // the same ending the next round, in case the first was lost, then nothing
            var third = round.Build(new[] { copter }, Named, "a-f-A-M-H-Q", true, later.AddSeconds(10), 120);
            Assert.AreEqual(1, third.Count);
            Assert.AreEqual("MP-UAS-1", third[0].uid);
            Assert.AreEqual(third[0].time, third[0].stale);
            Assert.AreEqual(first[0].point.lat, third[0].point.lat);
            Assert.AreEqual(0, round.Build(new[] { copter }, Named, "a-f-A-M-H-Q", true, later.AddSeconds(20), 120).Count);

            // it comes back when the link does
            copter.LastPacketUtc = later.AddSeconds(29);
            Assert.AreEqual(1, round.Build(new[] { copter }, Named, "a-f-A-M-H-Q", true, later.AddSeconds(30), 120).Count);
        }

        [TestMethod]
        public void ATrackHeardAgainByTheNextRound_IsNotEndedAgain()
        {
            var round = new CotRound();
            var copter = Copter();
            round.Build(new[] { copter }, Named, "a-f-A-M-H-Q", true, Now, 120);

            copter.LastPacketUtc = Now.AddSeconds(-30);
            Assert.AreEqual(1, round.Build(new[] { copter }, Named, "a-f-A-M-H-Q", true, Now.AddSeconds(10), 120).Count);

            // back before the next round: only the live event, so TAK shows it live
            copter.LastPacketUtc = Now.AddSeconds(19);
            var events = round.Build(new[] { copter }, Named, "a-f-A-M-H-Q", true, Now.AddSeconds(20), 120);
            Assert.AreEqual(1, events.Count);
            Assert.AreNotEqual(events[0].time, events[0].stale);
        }

        [TestMethod]
        public void OneOfTwoVehiclesGoingQuiet_IsEndedAheadOfTheOther_TwoRoundsRunning()
        {
            var round = new CotRound();
            var one = Copter();
            var two = Copter(sysid: 2);
            round.Build(new[] { one, two }, Named, "a-f-A-M-H-Q", true, Now, 120);

            // only the first vehicle is still heard
            one.LastPacketUtc = Now.AddSeconds(9);
            var events = round.Build(new[] { one, two }, Named, "a-f-A-M-H-Q", true, Now.AddSeconds(10), 120);
            CollectionAssert.AreEqual(new[] { "MP-UAS-2", "MP-UAS-1" }, events.Select(e => e.uid).ToArray());
            Assert.AreEqual(events[0].time, events[0].stale, "the ending first");
            Assert.AreNotEqual(events[1].time, events[1].stale);

            one.LastPacketUtc = Now.AddSeconds(19);
            events = round.Build(new[] { one, two }, Named, "a-f-A-M-H-Q", true, Now.AddSeconds(20), 120);
            CollectionAssert.AreEqual(new[] { "MP-UAS-2", "MP-UAS-1" }, events.Select(e => e.uid).ToArray(), "the ending again");
            Assert.AreEqual(events[0].time, events[0].stale);

            one.LastPacketUtc = Now.AddSeconds(29);
            events = round.Build(new[] { one, two }, Named, "a-f-A-M-H-Q", true, Now.AddSeconds(30), 120);
            CollectionAssert.AreEqual(new[] { "MP-UAS-1" }, events.Select(e => e.uid).ToArray());
        }

        [TestMethod]
        public void AVehicleThatLeavesTheList_IsExpiredToo()
        {
            var round = new CotRound();
            round.Build(new[] { Copter() }, Named, "a-f-A-M-H-Q", true, Now, 120);

            var events = round.Build(new CotVehicle[0], Named, "a-f-A-M-H-Q", true, Now.AddSeconds(10), 120);

            Assert.AreEqual(1, events.Count);
            Assert.AreEqual(events[0].time, events[0].stale);
        }

        [TestMethod]
        public void ANewUid_ExpiresTheOldTrack()
        {
            var round = new CotRound();
            round.Build(new[] { Copter() }, Named, "a-f-A-M-H-Q", true, Now, 120);

            var heard = Copter();
            heard.LastPacketUtc = Now.AddSeconds(9);
            var events = round.Build(new[] { heard }, v => new CotIdentity { Uid = "RENAMED" },
                "a-f-A-M-H-Q", true, Now.AddSeconds(10), 120);

            Assert.AreEqual(2, events.Count);
            Assert.AreEqual("MP-UAS-1", events[0].uid);
            Assert.AreEqual(events[0].time, events[0].stale);
            Assert.AreEqual("RENAMED", events[1].uid);
            Assert.AreNotEqual(events[1].time, events[1].stale);
        }

        [TestMethod]
        public void ExpireAll_MarksEveryReportedTrackStale()
        {
            var round = new CotRound();
            round.Build(new[] { Copter(), Copter(sysid: 2) }, Named, "a-f-A-M-H-Q", true, Now, 120);

            var events = round.ExpireAll(Now.AddSeconds(5));

            CollectionAssert.AreEquivalent(new[] { "MP-UAS-1", "MP-UAS-2" }, events.Select(e => e.uid).ToArray());
            Assert.IsTrue(events.All(e => e.stale == e.time));
            Assert.AreEqual(0, round.ExpireAll(Now.AddSeconds(6)).Count);
        }

        [TestMethod]
        public void ExpireAll_AlsoEndsATrackThatEndedLastRound()
        {
            var round = new CotRound();
            var quiet = Copter(sysid: 3);
            round.Build(new[] { Copter(), quiet }, Named, "a-f-A-M-H-Q", true, Now, 120);

            // the third vehicle goes quiet: its first ending goes in this round, the second is due next round
            quiet.LastPacketUtc = Now.AddSeconds(-20);
            round.Build(new[] { Copter(), quiet }, Named, "a-f-A-M-H-Q", true, Now.AddSeconds(5), 120);

            var events = round.ExpireAll(Now.AddSeconds(6));

            CollectionAssert.AreEquivalent(new[] { "MP-UAS-1", "MP-UAS-3" }, events.Select(e => e.uid).ToArray());
            Assert.IsTrue(events.All(e => e.stale == e.time));
            Assert.AreEqual(0, round.ExpireAll(Now.AddSeconds(7)).Count);
        }

        [TestMethod]
        public void TheSameSysidOnTwoConnections_IsTwoTracks()
        {
            var round = new CotRound();
            var events = round.Build(new[] { Copter("first"), Copter("second"), Copter("second", sysid: 2) }, Named,
                "a-f-A-M-H-Q", true, Now, 120);

            CollectionAssert.AreEqual(new[] { "MP-UAS-1", "MP-UAS-1-2", "MP-UAS-2" }, events.Select(e => e.uid).ToArray());
            CollectionAssert.AreEqual(new uint[] { 1 }, round.SharedSysIds);
        }

        [TestMethod]
        public void TwoConnectionsSwappingTheirUids_EndNeitherTrack()
        {
            var round = new CotRound();
            round.Build(new[] { Copter("first"), Copter("second") }, Named, "a-f-A-M-H-Q", true, Now, 120);

            // the connections come the other way round: each UID moves to the other vehicle and stays live
            var events = round.Build(new[] { Copter("second"), Copter("first") }, Named, "a-f-A-M-H-Q", true,
                Now.AddSeconds(5), 120);

            CollectionAssert.AreEquivalent(new[] { "MP-UAS-1", "MP-UAS-1-2" }, events.Select(e => e.uid).ToArray());
            Assert.IsTrue(events.All(e => e.stale != e.time), "no ending for a UID that is live");
        }

        [TestMethod]
        public void AutoType_FollowsTheKindOfVehicle()
        {
            Assert.AreEqual("a-f-A-M-F-Q", CotVehicles.AutoType(MAVLink.MAV_TYPE.FIXED_WING));
            Assert.AreEqual("a-f-A-M-F-Q", CotVehicles.AutoType(MAVLink.MAV_TYPE.VTOL_QUADROTOR));
            Assert.AreEqual("a-f-A-M-H-Q", CotVehicles.AutoType(MAVLink.MAV_TYPE.QUADROTOR));
            Assert.AreEqual("a-f-A-M-H-Q", CotVehicles.AutoType(MAVLink.MAV_TYPE.HELICOPTER));
            Assert.AreEqual("a-f-A-M-H-Q", CotVehicles.AutoType(MAVLink.MAV_TYPE.HEXAROTOR));
            Assert.AreEqual("a-f-A-M-L", CotVehicles.AutoType(MAVLink.MAV_TYPE.AIRSHIP));
            Assert.AreEqual("a-f-G-E-V", CotVehicles.AutoType(MAVLink.MAV_TYPE.GROUND_ROVER));
            Assert.AreEqual("a-f-S", CotVehicles.AutoType(MAVLink.MAV_TYPE.SURFACE_BOAT));
            Assert.AreEqual("a-f-U", CotVehicles.AutoType(MAVLink.MAV_TYPE.SUBMARINE));
            Assert.AreEqual("a-f-G-E-S", CotVehicles.AutoType(MAVLink.MAV_TYPE.ANTENNA_TRACKER));
            Assert.AreEqual("a-f-A-M-F-Q", CotVehicles.AutoType(MAVLink.MAV_TYPE.GENERIC));
        }

        [TestMethod]
        public void TheIdentitysType_OverridesTheRoundsType()
        {
            var cotevent = new CotRound().Build(new[] { Copter() },
                v => new CotIdentity { Uid = "MP-UAS-1", Type = "a-f-G-E-V" }, "a-f-A-M-F-Q", true, Now, 120).Single();

            Assert.AreEqual("a-f-G-E-V", cotevent.type);
            Assert.AreEqual("a-f-A-M-H-Q", One(Copter()).type, "no type of its own: the round's");
        }

        [TestMethod]
        public void Takv_SaysWhatSentIt()
        {
            var vehicle = Copter();
            vehicle.Firmware = "ArduCopter V4.7.1 (4776a3fb)";
            var cotevent = new CotRound().Build(new[] { vehicle }, v => new CotIdentity { Uid = "MP-UAS-1", Takv = true },
                "a-f-A-M-H-Q", true, Now, 120).Single();

            Assert.AreEqual("Mission Planner", cotevent.detail.takv.platform);
            Assert.AreEqual(CotRound.Version, cotevent.detail.takv.version);
            Assert.AreEqual("ArduCopter V4.7.1 (4776a3fb)", cotevent.detail.takv.device);
            StringAssert.Contains(CotXml.Serialize(cotevent, false), "<takv device=\"ArduCopter V4.7.1 (4776a3fb)\" platform=\"Mission Planner\"");
        }

        [TestMethod]
        public void Details_OnlyWithAdditionalDetails()
        {
            var vehicle = Copter();
            CotIdentity Full(CotVehicle v) => new CotIdentity
                { Uid = "MP-UAS-1", Takv = true, Callsign = "UAS-1", Endpoint = "192.168.1.20:4242:tcp", Vmf = "123" };

            var plain = new CotRound().Build(new[] { vehicle }, Full, "a-f-A-M-H-Q", false, Now, 120).Single();
            Assert.IsNull(plain.detail.contact);
            Assert.IsNull(plain.detail.takv);
            Assert.IsNull(plain.detail.uid);

            var detailed = new CotRound().Build(new[] { vehicle }, Full, "a-f-A-M-H-Q", true, Now, 120).Single();
            Assert.AreEqual("UAS-1", detailed.detail.contact.callsign);
            Assert.AreEqual("192.168.1.20:4242:tcp", detailed.detail.contact.endpoint);
            Assert.IsNotNull(detailed.detail.takv);
            Assert.AreEqual("123", detailed.detail.uid.vmf);
        }

        [TestMethod]
        public void Xml_IsTheEventWithItsDeclarationAndNoNamespaces()
        {
            var cotevent = One(Copter());

            var line = CotXml.Serialize(cotevent, false);
            var lines = line.Split(new[] { "\r\n" }, StringSplitOptions.None);
            Assert.AreEqual(2, lines.Length, "the declaration, then the event on one line");
            Assert.AreEqual(CotXml.Declaration, lines[0]);
            StringAssert.StartsWith(lines[1], "<event version=\"2.0\" uid=\"MP-UAS-1\" type=\"a-f-A-M-H-Q\"");
            StringAssert.Contains(lines[1], "<contact callsign=\"UAS-1\" />");
            Assert.IsFalse(line.Contains("xmlns"));

            var indented = CotXml.Serialize(cotevent, true);
            StringAssert.Contains(indented, "\r\n  uid=\"MP-UAS-1\"");
        }
    }
}
