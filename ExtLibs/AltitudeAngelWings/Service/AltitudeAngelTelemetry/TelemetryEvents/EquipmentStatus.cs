namespace AltitudeAngelWings.Service.AltitudeAngelTelemetry.TelemetryEvents
{
    /// <summary>
    /// The status of a piece of equipment, or operating environment
    /// </summary>
    public enum EquipmentStatus
    {
        /// <summary>
        /// Equipment or operating environment is within expected tolerances
        /// </summary>
        Ok = 0,

        /// <summary>
        /// Equipment is working but below expected performance or operating environment is outside of tolerance
        /// </summary>
        Degraded = 1,

        /// <summary>
        /// Equipment has failed.
        /// </summary>
        Failed = 2
    }
}
