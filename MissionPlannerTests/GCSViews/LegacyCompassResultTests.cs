using Microsoft.VisualStudio.TestTools.UnitTesting;
using MissionPlanner.GCSViews.ConfigurationView;
using System.Globalization;
using System.Threading;

namespace MissionPlanner.GCSViews.Tests
{
    [TestClass]
    public class LegacyCompassResultTests
    {
        [TestMethod]
        public void SuccessfulResultsPersistAndAreReplacedPerCompass()
        {
            System.Exception error = null;
            var thread = new Thread(() =>
            {
                try
                {
                    Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                    var type = typeof(ConfigHWCompass);
                    var flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
                    using (var control = (System.IDisposable)System.Activator.CreateInstance(type))
                    {
                        var receive = type.GetMethod("ReceviedPacket", flags);
                        var tick = type.GetMethod("timer1_Tick", flags);
                        var label = type.GetField("lbl_obmagresult", flags).GetValue(control);
                        var text = label.GetType().GetProperty("Text");
                        var parser = new MAVLink.MavlinkParse();

                        System.Action<byte, MAVLink.MAG_CAL_STATUS, float> report = (id, status, x) =>
                        {
                            var value = new MAVLink.mavlink_mag_cal_report_t
                            {
                                compass_id = id, cal_mask = 3, cal_status = (byte)status,
                                ofs_x = x, ofs_y = 2, ofs_z = 3, fitness = 4
                            };
                            var frame = parser.GenerateMAVLinkPacket20(MAVLink.MAVLINK_MSG_ID.MAG_CAL_REPORT, value);
                            receive.Invoke(control, new object[] { new MAVLink.MAVLinkMessage(frame) });
                        };
                        System.Action render = () => tick.Invoke(control, new object[] { null, System.EventArgs.Empty });
                        System.Func<string> displayed = () => (string)text.GetValue(label);

                        report(0, MAVLink.MAG_CAL_STATUS.MAG_CAL_SUCCESS, 1);
                        report(1, MAVLink.MAG_CAL_STATUS.MAG_CAL_SUCCESS, 5);
                        render();
                        render();
                        StringAssert.Contains(displayed(), "Mag 1 (id: 0) x:1.0 y:2.0 z:3.0 fit:4.0");
                        StringAssert.Contains(displayed(), "Mag 2 (id: 1) x:5.0 y:2.0 z:3.0 fit:4.0");

                        report(0, MAVLink.MAG_CAL_STATUS.MAG_CAL_FAILED_OFFSETS, 9);
                        render();
                        render();
                        Assert.IsFalse(displayed().Contains("Mag 1 (id: 0) x:1.0"));
                        StringAssert.Contains(displayed(), "Mag 2 (id: 1) x:5.0");

                        report(0, MAVLink.MAG_CAL_STATUS.MAG_CAL_SUCCESS, 7);
                        render();
                        render();
                        StringAssert.Contains(displayed(), "Mag 1 (id: 0) x:7.0");
                        StringAssert.Contains(displayed(), "Mag 2 (id: 1) x:5.0");
                    }
                }
                catch (System.Exception ex)
                {
                    error = ex;
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            if (error != null)
                System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(error).Throw();
        }
    }
}
