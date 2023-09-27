using System;
using System.Collections.Generic;
using MissionPlanner.ArduPilot;

namespace MissionPlanner.Utilities.BoardDetection
{
    public class FuncDetector : IDetector
    {
        private readonly Func<string, IReadOnlyList<DeviceInfo>, DetectResult> _func;

        public FuncDetector(Func<string, IReadOnlyList<DeviceInfo>, DetectResult> func)
        {
            _func = func;
        }

        public DetectResult Detect(string port, IReadOnlyList<DeviceInfo> ports)
        {
            return _func.Invoke(port, ports);
        }
    }
}