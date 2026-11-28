using Microsoft.VisualStudio.TestTools.UnitTesting;
using MissionPlanner.Controls;
using MissionPlanner.Controls.BackstageView;
using MissionPlanner.GCSViews.ConfigurationView;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace MissionPlanner.GCSViews.Tests
{
    [TestClass]
    public class CompassBackgroundTrackingTests
    {
        [TestMethod]
        public void StoppingTrackingUnsubscribesBothPacketsWithoutRemovingOtherSubscribers()
        {
            RunOnSta(() =>
            {
                using (var port = new MAVLinkInterface())
                using (var control = new ConfigHWCompass2())
                {
                    var type = typeof(ConfigHWCompass2);
                    var flags = BindingFlags.Instance | BindingFlags.NonPublic;
                    var subscriptions = (System.Collections.ICollection)typeof(MAVLinkInterface)
                        .GetField("Subscriptions", flags).GetValue(port);
                    Func<MAVLink.MAVLinkMessage, bool> callback = packet => true;
                    int other = port.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.HEARTBEAT, callback, 1, 1);

                    for (int i = 0; i < 2; i++)
                    {
                        type.GetField("_calibrationPort", flags).SetValue(control, port);
                        type.GetField("packetsub1", flags).SetValue(control,
                            port.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.MAG_CAL_PROGRESS, callback, 1, 1));
                        type.GetField("packetsub2", flags).SetValue(control,
                            port.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.MAG_CAL_REPORT, callback, 1, 1));
                        Assert.AreEqual(3, subscriptions.Count);

                        type.GetMethod("StopCalibrationTracking", flags).Invoke(control, null);
                        Assert.AreEqual(1, subscriptions.Count);
                    }

                    port.UnSubscribeToPacketType(other);
                }
            });
        }

        [TestMethod]
        public void LeavingSetupRetainsTrackingAndDisposesSetupOnceFinished()
        {
            RunOnSta(() =>
            {
                using (var parent = new UserControl())
                using (var switcher = new MainSwitcher(parent))
                {
                    var setup = new InitialSetup();
                    var flags = BindingFlags.Instance | BindingFlags.NonPublic;
                    var backstage = (BackstageView)typeof(InitialSetup)
                        .GetField("backstageView", flags).GetValue(setup);
                    var selected = new TrackingPage();
                    var selectedPage = new BackstageViewPage(typeof(TrackingPage), "Selected") { Page = selected };
                    var compass = new ConfigHWCompass2();
                    var compassPage = new BackstageViewPage(typeof(ConfigHWCompass2), "Compass") { Page = compass };
                    backstage.Pages.Add(selectedPage);
                    backstage.Pages.Add(compassPage);
                    typeof(BackstageView).GetField("_activePage", flags).SetValue(backstage, selectedPage);

                    var type = typeof(ConfigHWCompass2);
                    var timer = (System.Windows.Forms.Timer)type.GetField("timer1", flags).GetValue(compass);
                    timer.Start();
                    var setupScreen = new MainSwitcher.Screen("setup", setup);
                    switcher.AddScreen(setupScreen);
                    switcher.AddScreen(new MainSwitcher.Screen("other", new TrackingPage()));
                    switcher.current = setupScreen;

                    switcher.ShowScreen("other");
                    Assert.IsFalse(setup.IsDisposed);
                    Assert.IsTrue(timer.Enabled);
                    Assert.AreEqual(1, selected.Deactivations);

                    var progress = new MAVLink.mavlink_mag_cal_progress_t
                    {
                        compass_id = 0, cal_mask = 1, completion_pct = 42
                    };
                    var frame = new MAVLink.MavlinkParse().GenerateMAVLinkPacket20(
                        MAVLink.MAVLINK_MSG_ID.MAG_CAL_PROGRESS, progress);
                    type.GetMethod("ReceviedPacket", flags).Invoke(compass,
                        new object[] { new MAVLink.MAVLinkMessage(frame) });
                    type.GetMethod("timer1_Tick", flags).Invoke(compass, new object[] { null, EventArgs.Empty });
                    var label = (TextBox)type.GetField("lbl_obmagresult", flags).GetValue(compass);
                    StringAssert.Contains(label.Text, "Mag 1 (id: 0) 42%");

                    timer.Stop();
                    type.GetField("_rebootPromptPending", flags).SetValue(compass, true);
                    Assert.IsTrue(setup.KeepAlive);
                    switcher.ShowScreen("setup");
                    Assert.AreSame(setup, setupScreen.Control);
                    Assert.AreEqual(1, selected.Activations);

                    type.GetField("_rebootPromptPending", flags).SetValue(compass, false);
                    switcher.ShowScreen("other");
                    Assert.IsTrue(setup.IsDisposed);
                }
            });
        }

        public class TrackingPage : MyUserControl, IActivate, IDeactivate
        {
            public int Activations { get; private set; }
            public int Deactivations { get; private set; }

            public void Activate() => Activations++;
            public void Deactivate() => Deactivations++;
        }

        [TestMethod]
        public void HiddenPageKeepsLatestProgressAndMarksStaleUpdates()
        {
            RunOnSta(() =>
            {
                using (var control = new ConfigHWCompass2())
                {
                    var type = typeof(ConfigHWCompass2);
                    var flags = BindingFlags.Instance | BindingFlags.NonPublic;
                    var receive = type.GetMethod("ReceviedPacket", flags);
                    var tick = type.GetMethod("timer1_Tick", flags);
                    var render = type.GetMethod("RenderCalibrationState", flags);
                    var timer = (System.Windows.Forms.Timer)type.GetField("timer1", flags).GetValue(control);
                    var label = (TextBox)type.GetField("lbl_obmagresult", flags).GetValue(control);
                    var parser = new MAVLink.MavlinkParse();
                    Action<byte> progress = pct =>
                    {
                        var value = new MAVLink.mavlink_mag_cal_progress_t
                        {
                            compass_id = 0, cal_mask = 3, completion_pct = pct
                        };
                        var frame = parser.GenerateMAVLinkPacket20(MAVLink.MAVLINK_MSG_ID.MAG_CAL_PROGRESS, value);
                        receive.Invoke(control, new object[] { new MAVLink.MAVLinkMessage(frame) });
                    };

                    timer.Start();
                    control.Deactivate();
                    Assert.IsTrue(timer.Enabled);
                    for (int i = 0; i < 100; i++)
                        progress((byte)i);
                    var packets = (System.Collections.ICollection)type.GetField("_calPackets", flags).GetValue(control);
                    Assert.AreEqual(1, packets.Count);
                    tick.Invoke(control, new object[] { null, EventArgs.Empty });
                    StringAssert.Contains(label.Text, "Mag 1 (id: 0) 99%");

                    type.GetField("_calibrationPort", flags).SetValue(control, MainV2.comPort);
                    type.GetField("_lastCalPacketTime", flags).SetValue(control,
                        Stopwatch.GetTimestamp() - 11 * Stopwatch.Frequency);
                    render.Invoke(control, null);
                    StringAssert.Contains(label.Text, "No recent calibration updates.");

                    progress(42);
                    render.Invoke(control, null);
                    Assert.IsFalse(label.Text.Contains("No recent calibration updates."));
                    type.GetField("_calibrationPort", flags).SetValue(control, null);
                }
            });
        }

        [TestMethod]
        public void CompletionWhileHiddenDefersPromptAndStopsTracking()
        {
            RunOnSta(() =>
            {
                using (var control = new ConfigHWCompass2())
                {
                    var type = typeof(ConfigHWCompass2);
                    var flags = BindingFlags.Instance | BindingFlags.NonPublic;
                    var receive = type.GetMethod("ReceviedPacket", flags);
                    var tick = type.GetMethod("timer1_Tick", flags);
                    var timer = (System.Windows.Forms.Timer)type.GetField("timer1", flags).GetValue(control);
                    var report = new MAVLink.mavlink_mag_cal_report_t
                    {
                        compass_id = 0, cal_mask = 1, ofs_x = 1,
                        cal_status = (byte)MAVLink.MAG_CAL_STATUS.MAG_CAL_SUCCESS
                    };
                    var frame = new MAVLink.MavlinkParse().GenerateMAVLinkPacket20(
                        MAVLink.MAVLINK_MSG_ID.MAG_CAL_REPORT, report);
                    timer.Start();
                    control.Deactivate();
                    receive.Invoke(control, new object[] { new MAVLink.MAVLinkMessage(frame) });
                    for (int i = 0; i < 3; i++)
                        tick.Invoke(control, new object[] { null, EventArgs.Empty });
                    Assert.IsFalse(timer.Enabled);
                    Assert.IsTrue((bool)type.GetField("_rebootPromptPending", flags).GetValue(control));
                }
            });
        }

        [TestMethod]
        public void EmptyNotStartedReportDoesNotReplacePendingSuccess()
        {
            RunOnSta(() =>
            {
                using (var control = new ConfigHWCompass2())
                {
                    var type = typeof(ConfigHWCompass2);
                    var flags = BindingFlags.Instance | BindingFlags.NonPublic;
                    var receive = type.GetMethod("ReceviedPacket", flags);
                    var tick = type.GetMethod("timer1_Tick", flags);
                    var timer = (System.Windows.Forms.Timer)type.GetField("timer1", flags).GetValue(control);
                    var label = (TextBox)type.GetField("lbl_obmagresult", flags).GetValue(control);
                    var parser = new MAVLink.MavlinkParse();
                    var success = new MAVLink.mavlink_mag_cal_report_t
                    {
                        compass_id = 0, cal_mask = 7, ofs_x = 5, ofs_y = 13, ofs_z = -18,
                        fitness = 0.037f, cal_status = (byte)MAVLink.MAG_CAL_STATUS.MAG_CAL_SUCCESS
                    };
                    var placeholder = new MAVLink.mavlink_mag_cal_report_t
                    {
                        compass_id = 0, cal_mask = 7,
                        cal_status = (byte)MAVLink.MAG_CAL_STATUS.MAG_CAL_NOT_STARTED
                    };

                    timer.Start();
                    receive.Invoke(control, new object[] { new MAVLink.MAVLinkMessage(
                        parser.GenerateMAVLinkPacket20(MAVLink.MAVLINK_MSG_ID.MAG_CAL_REPORT, success)) });
                    receive.Invoke(control, new object[] { new MAVLink.MAVLinkMessage(
                        parser.GenerateMAVLinkPacket20(MAVLink.MAVLINK_MSG_ID.MAG_CAL_REPORT, placeholder)) });
                    tick.Invoke(control, new object[] { null, EventArgs.Empty });

                    StringAssert.Contains(label.Text, "Mag 1 (id: 0): Success");
                    StringAssert.Contains(label.Text, "x:5.0 y:13.0 z:-18.0");
                }
            });
        }

        [TestMethod]
        public void CancelledCalibrationClearsProgressAndPreservesSavedNotice()
        {
            RunOnSta(() =>
            {
                foreach (var type in new[] { typeof(ConfigHWCompass), typeof(ConfigHWCompass2) })
                {
                    using (var control = (IDisposable)Activator.CreateInstance(type))
                    {
                        var flags = BindingFlags.Instance | BindingFlags.NonPublic;
                        var label = (TextBox)type.GetField("lbl_obmagresult", flags).GetValue(control);
                        label.Text = "Mag 1 (id: 0) 14%";
                        for (int i = 1; i <= 3; i++)
                        {
                            var bar = type.GetField("horizontalProgressBar" + i, flags).GetValue(control);
                            bar.GetType().GetProperty("Value").SetValue(bar, 14);
                        }
                        if (type == typeof(ConfigHWCompass2))
                            type.GetField("_calChangesRequireReboot", flags).SetValue(control, true);
                        else
                            ((System.Collections.Generic.HashSet<byte>)type.GetField("_autosavedCompasses", flags)
                                .GetValue(control)).Add(0);

                        type.GetMethod("ResetCancelledCalibration", flags).Invoke(control, null);

                        for (int i = 1; i <= 3; i++)
                        {
                            var bar = type.GetField("horizontalProgressBar" + i, flags).GetValue(control);
                            Assert.AreEqual(0, (int)bar.GetType().GetProperty("Value").GetValue(bar));
                        }
                        StringAssert.Contains(label.Text, "Calibration cancelled.");
                        StringAssert.Contains(label.Text, "require a reboot");
                        Assert.IsFalse(label.Text.Contains("14%"));
                    }
                }
            });
        }

        private static void RunOnSta(Action action)
        {
            Exception error = null;
            var thread = new Thread(() =>
            {
                try { action(); }
                catch (Exception ex) { error = ex; }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            if (error != null)
                System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(error).Throw();
        }
    }
}
