using System;
using System.Collections.Generic;
using MissionPlanner.ArduPilot;

namespace MissionPlanner.Utilities.BoardDetection
{
    public class MonoRuntimeDetector : IDetector
    {
        public DetectResult Detect(string port, IReadOnlyList<DeviceInfo> ports)
        {
            var dialogResult = CustomMessageBox.Show("Is this a APM 2+?", "APM 2+", CustomMessageBox.MessageBoxButtons.YesNo);
            if (dialogResult == CustomMessageBox.DialogResult.Yes)
            {
                return new DetectResult(Boards.b2560v2, null);
            }

            dialogResult = CustomMessageBox.Show("Is this a CUBE/PX4/PIXHAWK/PIXRACER?", "PX4/PIXHAWK", CustomMessageBox.MessageBoxButtons.YesNo);
            if (dialogResult == CustomMessageBox.DialogResult.Yes)
            {
                dialogResult = CustomMessageBox.Show("Is this a PIXRACER?", "PIXRACER", CustomMessageBox.MessageBoxButtons.YesNo);
                if (dialogResult == CustomMessageBox.DialogResult.Yes)
                {
                    return new DetectResult(Boards.px4v4, null);
                }

                dialogResult = CustomMessageBox.Show("Is this a CUBE?", "CUBE", CustomMessageBox.MessageBoxButtons.YesNo);
                if (dialogResult == CustomMessageBox.DialogResult.Yes)
                {
                    return new DetectResult(Boards.px4v3, null);
                }

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