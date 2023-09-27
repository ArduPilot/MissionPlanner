using System;
using System.Collections.Generic;
using log4net;
using MissionPlanner.ArduPilot;

namespace MissionPlanner.Utilities.BoardDetection
{
    public class BoardDetector
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(BoardDetector));

        public string chbootloader = "";

        private static readonly BoardDetector instance = new BoardDetector();

        protected BoardDetector()
        {
        }
        
        public static BoardDetector GetInstance()
        {
            return instance;
        }

        private readonly IDetector _detector = new ByRuntimeDetector(
            new CompositeDetector(
                new ComPortDetector(),
                new Win32USBDeviceDetector(),
                new FuncDetector(DiscoBebop2),
                new SerialPortDetector(),
                new FuncDetector(Px4B2560)
            ),
            new MonoRuntimeDetector()
        );

        /// <summary>
        /// Detect board version
        /// </summary>
        /// <param name="port"></param>
        /// <returns> boards enum value</returns>
        public Boards DetectBoard(string port, List<DeviceInfo> ports)
        {
            var result = _detector.Detect(port, ports);
            if (result.board != null)
            {
                if (result.chbootloader != null)
                {
                    chbootloader = result.chbootloader;
                }

                return result.board.Value;
            }

            throw new InvalidOperationException("unexpected state of detector");
        }

        private static DetectResult DiscoBebop2(string port, IReadOnlyList<DeviceInfo> ports)
        {
            var dialogResult = CustomMessageBox.Show("Is this a Linux board?", "Linux", CustomMessageBox.MessageBoxButtons.YesNo);
            if (dialogResult == CustomMessageBox.DialogResult.Yes)
            {
                dialogResult = CustomMessageBox.Show("Is this Bebop2?", "Bebop2", CustomMessageBox.MessageBoxButtons.YesNo);
                if (dialogResult == CustomMessageBox.DialogResult.Yes)
                {
                    return new DetectResult(Boards.bebop2, null);
                }

                dialogResult = CustomMessageBox.Show("Is this Disco?", "Disco", CustomMessageBox.MessageBoxButtons.YesNo);
                if (dialogResult == CustomMessageBox.DialogResult.Yes)
                {
                    return new DetectResult(Boards.disco, null);
                }
            }

            return DetectResult.None;
        }

        private static DetectResult Px4B2560(string port, IReadOnlyList<DeviceInfo> ports)
        {
            var dialogResult = CustomMessageBox.Show("Is this a APM 2+?", "APM 2+", CustomMessageBox.MessageBoxButtons.YesNo);
            if (dialogResult == CustomMessageBox.DialogResult.Yes)
            {
                return new DetectResult(Boards.b2560v2, null);
            }

            dialogResult = CustomMessageBox.Show("Is this a PX4/PIXHAWK?", "PX4/PIXHAWK", CustomMessageBox.MessageBoxButtons.YesNo);
            if (dialogResult == CustomMessageBox.DialogResult.Yes)
            {
                dialogResult = CustomMessageBox.Show("Is this a PIXHAWK?", "PIXHAWK", CustomMessageBox.MessageBoxButtons.YesNo);
                if (dialogResult == CustomMessageBox.DialogResult.Yes)
                {
                    return new DetectResult(Boards.px4v2, null);
                }

                return new DetectResult(Boards.px4, null);
            }

            return new DetectResult(Boards.b2560, null);
        }
    }
}