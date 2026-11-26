using System;
using System.Threading;
using System.Threading.Tasks;

namespace MissionPlanner.Utilities
{
    /// <summary>
    /// Short waits for loops that poll a stream for data.
    /// </summary>
    public static class PollDelay
    {
        private static readonly bool mono = Type.GetType("Mono.Runtime") != null;

        // the default Windows timer tick, so Task.Delay(1) really waits this long there
        private const int WindowsTickMs = 15;

        /// <summary>
        /// On Windows Task.Delay(1) waits a full timer tick, about 15ms, and the polling loops have
        /// always run at that rate. On Mono it is a real 1ms, so the loops run 15 times as often,
        /// and Mono's timer thread busy waits once less than a millisecond is left, keeping a core
        /// busy. On Mono sleep a pool thread for a Windows tick instead.
        /// </summary>
        public static Task Wait(int ms = 1)
        {
            // only short waits are raised, Timeout.Infinite and longer delays pass through
            if (mono && ms >= 0 && ms < WindowsTickMs)
                return Task.Run(() => Thread.Sleep(WindowsTickMs));

            return Task.Delay(ms);
        }
    }
}
