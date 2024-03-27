using System;
using System.Collections.Generic;
using log4net;
using MissionPlanner.ArduPilot;

namespace MissionPlanner.Utilities.BoardDetection
{
    public class CompositeDetector : IDetector
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Win32USBDeviceDetector));
        private readonly IReadOnlyList<IDetector> _detectors;

        public CompositeDetector(params IDetector[] detectors)
        {
            _detectors = detectors;
        }

        public DetectResult Detect(string port, IReadOnlyList<DeviceInfo> ports)
        {
            foreach (var detector in _detectors)
            {
                try
                {
                    var result = detector.Detect(port, ports);
                    if (result.board != null)
                    {
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }

            return DetectResult.Failed;
        }
    }
}