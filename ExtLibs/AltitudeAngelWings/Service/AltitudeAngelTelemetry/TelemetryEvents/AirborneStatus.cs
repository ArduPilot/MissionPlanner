namespace AltitudeAngelWings.Service.AltitudeAngelTelemetry.TelemetryEvents
{
    /// <summary>
    /// The airborne state of a UAV
    /// </summary>
    public enum AirborneStatus : byte
    {
        /// <summary>
        /// Cannot be determined, or lacks the means to do so
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// In the air
        /// </summary>
        Airborne = 1,

        /// <summary>
        /// On the ground
        /// </summary>
        Grounded = 2
    }
}
