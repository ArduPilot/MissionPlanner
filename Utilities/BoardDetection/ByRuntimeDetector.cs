using System;
using System.Collections.Generic;
using MissionPlanner.ArduPilot;

namespace MissionPlanner.Utilities.BoardDetection
{
    public class ByRuntimeDetector : IDetector
    {
        private readonly IDetector _default;
        private readonly IDetector _mono;

        public ByRuntimeDetector(IDetector @default, IDetector mono)
        {
            _default = @default;
            _mono = mono;
        }

        public DetectResult Detect(string port, IReadOnlyList<DeviceInfo> ports)
        {
            var t = Type.GetType("Mono.Runtime");
            var isMonoRuntime = (t != null);

            if (isMonoRuntime)
            {
                return _mono.Detect(port, ports);
            }

            return _default.Detect(port, ports);
        }
    }
}