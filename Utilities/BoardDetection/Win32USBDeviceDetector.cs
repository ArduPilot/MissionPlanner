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

                // check vid and pid
                if (obj2.Properties["PNPDeviceID"].Value.ToString().Contains(@"USB\VID_2341&PID_0010"))
                {
                    // check port name as well
                    if (obj2.Properties["Name"].Value.ToString().ToUpper().Contains(port.ToUpper()))
                    {
                        log.Info("is a 2560-2");
                        return new DetectResult(Boards.b2560v2, null);
                    }
                }
                else if (obj2.Properties["PNPDeviceID"].Value.ToString().Contains(@"USB\VID_26AC&PID_0010"))
                {
                    // check port name as well
                    //if (obj2.Properties["Name"].Value.ToString().ToUpper().Contains(serialPort.PortName.ToUpper()))
                    {
                        log.Info("is a px4");
                        return new DetectResult(Boards.px4, null);
                    }
                }
                else if (obj2.Properties["PNPDeviceID"].Value.ToString().Contains(@"USB\VID_26AC&PID_0032"))
                {
                    // check port name as well
                    //if (obj2.Properties["Name"].Value.ToString().ToUpper().Contains(serialPort.PortName.ToUpper()))
                    {
                        log.Info("is a CUAVv5");
                        return new DetectResult(Boards.chbootloader, "CUAVv5");
                    }
                }
                // chibios or normal px4
                else if (obj2.Properties["PNPDeviceID"].Value.ToString().Contains(@"USB\VID_0483&PID_5740") ||
                         obj2.Properties["PNPDeviceID"].Value.ToString().Contains(@"USB\VID_26AC&PID_0011") ||
                         obj2.Properties["PNPDeviceID"].Value.ToString().Contains(@"USB\VID_1209&PID_5740"))
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
                else if (obj2.Properties["PNPDeviceID"].Value.ToString().Contains(@"USB\VID_26AC&PID_0021"))
                {
                    log.Info("is a px4v3 X2.1");
                    return new DetectResult(Boards.px4v3, null);
                }
                else if (obj2.Properties["PNPDeviceID"].Value.ToString().Contains(@"USB\VID_26AC&PID_0012"))
                {
                    log.Info("is a px4v4 pixracer");
                    return new DetectResult(Boards.px4v4, null);
                }
                else if (obj2.Properties["PNPDeviceID"].Value.ToString().Contains(@"USB\VID_26AC&PID_0013"))
                {
                    log.Info("is a px4v4pro pixhawk 3 pro");
                    return new DetectResult(Boards.px4v4pro, null);
                }
                else if (obj2.Properties["PNPDeviceID"].Value.ToString().Contains(@"USB\VID_26AC&PID_0001"))
                {
                    log.Info("is a px4v2 bootloader");
                    return new DetectResult(Boards.px4v2, null);
                }
                else if (obj2.Properties["PNPDeviceID"].Value.ToString().Contains(@"USB\VID_26AC&PID_0016"))
                {
                    return new DetectResult(Boards.px4rl, null);
                }
                //|| obj2.Properties["PNPDeviceID"].Value.ToString().Contains(@"USB\VID_26AC&PID_0012") || obj2.Properties["PNPDeviceID"].Value.ToString().Contains(@"USB\VID_26AC&PID_0013") || obj2.Properties["PNPDeviceID"].Value.ToString().Contains(@"USB\VID_26AC&PID_0014") || obj2.Properties["PNPDeviceID"].Value.ToString().Contains(@"USB\VID_26AC&PID_0015") || obj2.Properties["PNPDeviceID"].Value.ToString().Contains(@"USB\VID_26AC&PID_0016")

                else if (obj2.Properties["PNPDeviceID"].Value.ToString().Contains(@"USB\VID_27AC&PID_1140"))
                {
                    log.Info("is a vrbrain 4.0 bootloader");
                    return new DetectResult(Boards.vrbrainv40, null);
                }
                else if (obj2.Properties["PNPDeviceID"].Value.ToString().Contains(@"USB\VID_27AC&PID_1145"))
                {
                    log.Info("is a vrbrain 4.5 bootloader");
                    return new DetectResult(Boards.vrbrainv45, null);
                }
                else if (obj2.Properties["PNPDeviceID"].Value.ToString().Contains(@"USB\VID_27AC&PID_1150"))
                {
                    log.Info("is a vrbrain 5.0 bootloader");
                    return new DetectResult(Boards.vrbrainv50, null);
                }
                else if (obj2.Properties["PNPDeviceID"].Value.ToString().Contains(@"USB\VID_27AC&PID_1151"))
                {
                    log.Info("is a vrbrain 5.1 bootloader");
                    return new DetectResult(Boards.vrbrainv51, null);
                }
                else if (obj2.Properties["PNPDeviceID"].Value.ToString().Contains(@"USB\VID_27AC&PID_1152"))
                {
                    log.Info("is a vrbrain 5.2 bootloader");
                    return new DetectResult(Boards.vrbrainv52, null);
                }
                else if (obj2.Properties["PNPDeviceID"].Value.ToString().Contains(@"USB\VID_27AC&PID_1154"))
                {
                    log.Info("is a vrbrain 5.4 bootloader");
                    return new DetectResult(Boards.vrbrainv54, null);
                }
                else if (obj2.Properties["PNPDeviceID"].Value.ToString().Contains(@"USB\VID_27AC&PID_1910"))
                {
                    log.Info("is a vrbrain core 1.0 bootloader");
                    return new DetectResult(Boards.vrcorev10, null);
                }
                else if (obj2.Properties["PNPDeviceID"].Value.ToString().Contains(@"USB\VID_27AC&PID_1351"))
                {
                    log.Info("is a vrubrain 5.1 bootloader");
                    return new DetectResult(Boards.vrubrainv51, null);
                }
                else if (obj2.Properties["PNPDeviceID"].Value.ToString().Contains(@"USB\VID_27AC&PID_1352"))
                {
                    log.Info("is a vrubrain 5.2 bootloader");
                    return new DetectResult(Boards.vrubrainv52, null);
                }
                else if (obj2.Properties["PNPDeviceID"].Value.ToString().Contains(@"USB\VID_1FC9&PID_001C"))
                {
                    log.Info("is a NXP RDDRONE-FMUK66");
                    return new DetectResult(Boards.nxpfmuk66, null);
                }
            }

            return DetectResult.None;
        }
    }
}