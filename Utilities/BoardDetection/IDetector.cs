using System.Collections.Generic;
using MissionPlanner.ArduPilot;

namespace MissionPlanner.Utilities.BoardDetection
{
    public struct DetectResult
    {
        public static readonly DetectResult Failed = new DetectResult(null, null);

        public Boards? board;
        public string chbootloader;

        public DetectResult(Boards? board, string chbootloader)
        {
            this.board = board;
            this.chbootloader = chbootloader;
        }
    }

    public interface IDetector
    {
        DetectResult Detect(string port, IReadOnlyList<DeviceInfo> ports);
    }
}