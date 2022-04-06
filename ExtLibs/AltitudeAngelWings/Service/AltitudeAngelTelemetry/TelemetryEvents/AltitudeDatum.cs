namespace AltitudeAngelWings.Service.AltitudeAngelTelemetry.TelemetryEvents
{
    /// <summary>
    /// Scale of altitude
    /// </summary>
    public enum AltitudeDatum
    {
        /// <summary>
        /// Surface
        /// </summary>
        Sfc = 1,

        /// <summary>
        /// WGS84
        /// </summary>
        Wgs84 = 2,

        /// <summary>
        /// Mean sea level (per region)
        /// </summary>
        Msl = 3,

        /// <summary>
        /// At ground level
        /// </summary>
        Agl = 4
    }
}
