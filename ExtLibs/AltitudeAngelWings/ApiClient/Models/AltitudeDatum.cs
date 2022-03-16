namespace AltitudeAngelWings.ApiClient.Models
{
    public enum AltitudeDatum
    {
        /// <summary>
        ///     Standard pressure setting ~1013.25HPa).
        /// </summary>
        Sps = 1,
        /// <summary>
        ///     Surface.
        /// </summary>
        Sfc = 2,
        /// <summary>
        ///     WGS84.
        /// </summary>
        Wgs84 = 3,
        /// <summary>
        ///     Mean sea level (per region).
        /// </summary>
        Msl = 4,
        /// <summary>
        ///     At ground level.
        /// </summary>
        Agl = 5
    }
}
