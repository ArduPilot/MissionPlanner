using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using log4net;
using MissionPlanner.ArduPilot;
using MissionPlanner.Comms;
using px4uploader;

namespace MissionPlanner.Utilities.BoardDetection
{
    public class ComPortDetector : IDetector
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ComPortDetector));
        
        public DetectResult Detect(string port, IReadOnlyList<DeviceInfo> ports)
        {
            // check the device reported productname
            foreach (var item in ports)
            {
                log.InfoFormat("{0}: {1} - {2}", item.name, item.description, item.board);

                //if (port.ToLower() == item.name.ToLower())
                {
                    //USB\VID_0483&PID_DF11   -- stm32 bootloader
                    //USB\VID_1209&PID_5740   -- ardupilot chibios

                    // new style bootloader
                    if (item.hardwareid.StartsWith(@"USB\VID_0483&PID_5740") ||
                        Regex.IsMatch(item.hardwareid, "VID_2DAE") ||
                        Regex.IsMatch(item.hardwareid, "VID_3162") ||
                        item.hardwareid.StartsWith(@"USB\VID_1209&PID_5740"))
                    {
                        if (item.board == "fmuv2" || item.board.ToLower() == "fmuv2-bl")
                        {
                            log.Info("is a fmuv2");
                            return new DetectResult(Boards.px4v2, null);
                        }

                        if (item.board == "fmuv3" || item.board.ToLower() == "fmuv3-bl")
                        {
                            log.Info("is a fmuv3");
                            return new DetectResult(Boards.px4v3, null);
                        }

                        if (item.board == "fmuv4" || item.board.ToLower() == "fmuv4-bl")
                        {
                            log.Info("is a fmuv4");
                            return new DetectResult(Boards.px4v4, null);
                        }

                        if (item.board == "fmuv5" || item.board.ToLower() == "fmuv5-bl")
                        {
                            log.Info("is a fmuv5");
                        }

                        if (item.board == "revo-mini" || item.board.ToLower() == "revo-mini-bl")
                        {
                            log.Info("is a revo-mini");
                            //return boards.revomini;
                        }

                        if (item.board == "mini-pix" || item.board.ToLower() == "mini-pix-bl")
                        {
                            log.Info("is a mini-pix");
                            //return boards.minipix;
                        }

                        if (item.board == "mindpx-v2" || item.board.ToLower() == "mindpx-v2-bl")
                        {
                            log.Info("is a mindpx-v2");
                            //return boards.mindpxv2;
                        }

                        var chbootloader = item.board.Replace("-bl", "").Replace("-Bl", "").Replace("-BL", "");
                        return new DetectResult(Boards.chbootloader, chbootloader);
                    }
                    // old style bootloader
                    else if (item.board == "PX4 FMU v5.x")
                    {
                        //USB\VID_26AC&PID_0032\0
                        log.Info("is a PX4 FMU v5.x (fmuv5)");
                        return new DetectResult(Boards.fmuv5, null);
                    }
                    else if (item.board == "PX4 FMU v4.x")
                    {
                        log.Info("is a px4v4 pixracer");
                        return new DetectResult(Boards.px4v4, null);
                    }
                    else if (item.board == "PX4 FMU v1.x")
                    {
                        log.Info("is a px4v1");
                        return new DetectResult(Boards.px4, null);
                    }
                    else if (item.board == "PX4 FMU v2.x" || item.board == "PX4 FMU v3.x")
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
                }
            }

            return DetectResult.Failed;
        }
    }
}