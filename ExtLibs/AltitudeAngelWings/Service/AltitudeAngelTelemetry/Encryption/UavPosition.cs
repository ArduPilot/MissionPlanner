using System.Runtime.InteropServices;

namespace AltitudeAngelWings.Service.AltitudeAngelTelemetry.Encryption
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct UavPosition
    {
        /// <summary>
        ///     Timestamp of the GPS data (precision to 1 second).
        ///     This is in GPS time so does not account for leap seconds
        /// </summary>
        public int GpsTimestamp
        {
            get;
            set;
        }

        /// <summary>
        /// Optional: Number of milliseconds offset from the last GPS timestamp. This is intended to provide
        /// sub-second accurance. Default to 0 if not used
        /// </summary>
        public ushort GpsTimestampOffset
        {
            get;
            set;
        }

        /// <summary>
        /// GPS Latitude multiplied by 10E+7 as a 32 bit int
        /// </summary>
        public int Latitude
        {
            get;
            set;
        }

        /// <summary>
        /// GPS Longitude multiplied by 10E+7 as a 32 bit int
        /// </summary>
        public int Longitude
        {
            get;
            set;
        }

        /// <summary>
        /// Altitude in meters
        /// </summary>
        public int GpsAltitude
        {
            get;
            set;
        }

        /// <summary>
        /// Indicates of whether the drone is in the air.
        /// <see cref="UavAirborneStatus"/> for expected values
        /// </summary>
        public byte IsAirborne
        {
            get;
            set;
        }

        /// <summary>
        /// Latitude velocity in m/s multiplied by 10E+3 as a 32 bit int
        /// </summary>
        public int NorthVelocity
        {
            get;
            set;
        }

        /// <summary>
        /// Longitude velocity in m/s multiplied by 10E+3 as a 32 bit int
        /// </summary>
        public int EastVelocity
        {
            get;
            set;
        }

        /// <summary>
        /// Altitude velocity in m/s multiplied by 10E+3 as a 32 bit int
        /// </summary>
        public int UpVelocity
        {
            get;
            set;
        }

        /// <summary>
        /// Uncertainty of current altitude in meters
        /// </summary>
        public ushort AltitudeAccuracy
        {
            get;
            set;
        }

        /// <summary>
        /// Uncertainty of current position (lat and long) in meters
        /// </summary>
        public ushort PositionAccuracy
        {
            get;
            set;
        }

        /// <summary>
        /// Uncertainty of current velocity in m/s multiplied by 10E+3 as a 32 bit int
        /// </summary>
        public int VelocityAccuracy
        {
            get;
            set;
        }

        /// <summary>
        /// Number of satellites visible to the GPS receiver
        /// </summary>
        public byte SatellitesVisible
        {
            get;
            set;
        }

        /// <summary>
        /// % Battery charge
        /// </summary>
        public byte Fuel
        {
            get;
            set;
        }
    }
}
