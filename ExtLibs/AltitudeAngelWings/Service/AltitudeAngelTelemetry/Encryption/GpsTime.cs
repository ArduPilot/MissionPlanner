using System;

namespace AltitudeAngelWings.Service.AltitudeAngelTelemetry.Encryption
{
    public static class GpsTime
    {
        public static DateTime Epoch = new DateTime(1980, 1, 6);

        /// <summary>
        /// Returns a GPS timestamp representation of a date time. It does *NOT* convert a UTC based date to a GPS one.
        /// i.e. it does not account for leap seconds
        /// The input DateTime is assumed to already have taken into account the difference between UTC and TAI.
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static int ToGpsTimestamp(DateTime dt)
        {
            int gpsTimestamp = (int)dt.Subtract(GpsTime.Epoch).TotalSeconds;
            return gpsTimestamp;
        }

        /// <summary>
        /// Converts a GPS timestamp to a DateTime object. It does *NOT* convert to a UTC based date.
        /// i.e. it does not account for leap seconds
        /// The returned DateTime will have the same clock error as the original int representation.
        /// </summary>
        /// <param name="gpsTimestamp"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(int gpsTimestamp)
        {
            DateTime dt = GpsTime.Epoch.Add(TimeSpan.FromSeconds(gpsTimestamp));
            return dt;
        }
    }
}
