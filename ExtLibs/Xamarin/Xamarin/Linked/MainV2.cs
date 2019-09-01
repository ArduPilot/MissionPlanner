using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MissionPlanner.Utilities
{
    internal class MainV2
    {
        internal static MAVLinkInterface comPort;
        internal static MainV2 instance;
        /// <summary>
        /// other planes in the area from adsb
        /// </summary>
        public object adsblock = new object();

        public ConcurrentDictionary<string, adsb.PointLatLngAltHdg> adsbPlanes = new ConcurrentDictionary<string, adsb.PointLatLngAltHdg>();


        static MainV2()
        {
            instance = new MainV2();
        }

        public MainV2()
        {
            instance = this;
            comPort = new MAVLinkInterface();
            comPort.BaseStream = new Comms.UdpSerial();
        }

        public static List<MAVLinkInterface> Comports { get; set; } = new List<MAVLinkInterface>();
        public static bool ShowAirports { get; set; }

        internal void Invoke(Action methodInvoker)
        {
            throw new NotImplementedException();
        }
    }
}