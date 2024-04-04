using System;
using System.Collections.Generic;
using System.Management;
using log4net;
using MissionPlanner.Comms;
using MissionPlanner.ArduPilot;
using px4uploader;

namespace MissionPlanner.Utilities.BoardDetection
{
    public class Win32USBDeviceDetector : IDetector
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Win32USBDeviceDetector));

        private static readonly Dictionary<string, DetectResult> ids2result = new Dictionary<string, DetectResult>
        {
            { @"USB\VID_26AC&PID_0010", new DetectResult(Boards.px4, null) },
            { @"USB\VID_26AC&PID_0032", new DetectResult(Boards.chbootloader, "CUAVv5") },
            { @"USB\VID_26AC&PID_0021", new DetectResult(Boards.px4v3, null) },
            { @"USB\VID_26AC&PID_0012", new DetectResult(Boards.px4v4, null) },
            { @"USB\VID_26AC&PID_0013", new DetectResult(Boards.px4v4pro, null) },
            { @"USB\VID_26AC&PID_0001", new DetectResult(Boards.px4v2, null) },
            { @"USB\VID_26AC&PID_0016", new DetectResult(Boards.px4rl, null) },
            { @"USB\VID_27AC&PID_1140", new DetectResult(Boards.vrbrainv40, null) },
            { @"USB\VID_27AC&PID_1145", new DetectResult(Boards.vrbrainv45, null) },
            { @"USB\VID_27AC&PID_1150", new DetectResult(Boards.vrbrainv50, null) },
            { @"USB\VID_27AC&PID_1151", new DetectResult(Boards.vrbrainv51, null) },
            { @"USB\VID_27AC&PID_1152", new DetectResult(Boards.vrbrainv52, null) },
            { @"USB\VID_27AC&PID_1154", new DetectResult(Boards.vrbrainv54, null) },
            { @"USB\VID_27AC&PID_1910", new DetectResult(Boards.vrcorev10, null) },
            { @"USB\VID_27AC&PID_1351", new DetectResult(Boards.vrubrainv51, null) },
            { @"USB\VID_27AC&PID_1352", new DetectResult(Boards.vrubrainv52, null) },
            { @"USB\VID_1FC9&PID_001C", new DetectResult(Boards.nxpfmuk66, null) }
        };

        public DetectResult Detect(string port, IReadOnlyList<DeviceInfo> ports)
        {
            var query = new ObjectQuery("SELECT * FROM Win32_SerialPort"); // Win32_USBControllerDevice
            var searcher = new ManagementObjectSearcher(query);
            foreach (ManagementObject obj2 in searcher.Get())
            {
                log.InfoFormat("-----------------------------------");
                log.InfoFormat("Win32_USBDevice instance");
                log.InfoFormat("-----------------------------------");

                foreach (var item in obj2.Properties)
                {
                    log.InfoFormat("{0}: {1}", item.Name, item.Value);
                }

                var deviceId = obj2.Properties["PNPDeviceID"].Value.ToString();
                var name = obj2.Properties["Name"].Value.ToString();
                
                // check vid and pid
                if (deviceId.Contains(@"USB\VID_2341&PID_0010"))
                {
                    // check port name as well
                    if (name.ToUpper().Contains(port.ToUpper()))
                    {
                        log.Info("is a 2560-2");
                        return new DetectResult(Boards.b2560v2, null);
                    }
                }
                // chibios or normal px4
                else if (deviceId.Contains(@"USB\VID_0483&PID_5740") ||
                         deviceId.Contains(@"USB\VID_26AC&PID_0011") ||
                         deviceId.Contains(@"USB\VID_1209&PID_5740"))
                {
                    CustomMessageBox.Show(Strings.PleaseUnplugTheBoardAnd);

                    DateTime DEADLINE = DateTime.Now.AddSeconds(30);

                    while (DateTime.Now < DEADLINE)
                    {
                        string[] allports = SerialPort.GetPortNames();

                        foreach (string port1 in allports)
                        {
                            log.Info(DateTime.Now.Millisecond + " Trying Port " + port1);
                            try
                            {
                                using (var up = new Uploader(port1, 115200))
                                {
                                    up.identify();
                                    Console.WriteLine("Found board type {0} brdrev {1} blrev {2} fwmax {3} chip {5:X} chipdes {6} on {4}", up.board_type,
                    up.board_rev, up.bl_rev, up.fw_maxsize, port1, up.chip, up.chip_desc);

                                    if (up.fw_maxsize == 2080768 && up.board_type == 9 && up.bl_rev >= 5)
                                    {
                                        log.Info("is a px4v3");
                                        return new DetectResult(Boards.px4v3, null);
                                    }
                                    else if (up.fw_maxsize == 2080768 && up.board_type == 50 && up.bl_rev >= 5)
                                    {
                                        log.Info("is a fmuv5");
                                        return new DetectResult(Boards.fmuv5, null);
                                    }
                                    else
                                    {
                                        log.Info("is a px4v2");
                                        return new DetectResult(Boards.px4v2, null);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                log.Error(ex);
                            }
                        }
                    }

                    log.Info("Failed to detect px4 board type");
                    return new DetectResult(Boards.none, null);
                }
                else
                {
                    if (ids2result.TryGetValue(deviceId, out var result))
                    {
                        return result;
                    }
                }
            }

            return DetectResult.Failed;
        }
    }
}