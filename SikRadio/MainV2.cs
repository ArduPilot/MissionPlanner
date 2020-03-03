using System;
using MissionPlanner.Comms;

namespace MissionPlanner
{
    internal class Settings
    {
        internal static Settings Instance = new Settings();

        internal string UserAgent = "SikRadio";
    }
    internal class MainV2
    {
        public static portproxy comPort = new portproxy();

        public static string comPortName { get; internal set; }
        public static int comPortBaud { get; internal set; }
    }

    internal class portproxy : IDisposable
    {
        public SerialPort BaseStream = new SerialPort();

        public void Dispose()
        {
            BaseStream.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}