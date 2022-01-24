namespace AltitudeAngelWings.Service.AltitudeAngelTelemetry.TelemetryEvents
{
    /// <summary>
    /// References a piece of equipment or part of the UAV Environment
    /// </summary>
    public enum EquipmentId
    {
        /// <summary>
        /// Related to engines
        /// </summary>
        Powertrain = 1,

        /// <summary>
        /// e.g. GPS
        /// </summary>
        PositioningSystem = 2,

        /// <summary>
        /// Communications equipment
        /// </summary>
        ComsLink = 3,

        /// <summary>
        /// Power equipment, e.g. battery
        /// </summary>
        Power = 4,

        /// <summary>
        /// UAV structure
        /// </summary>
        Airframe = 5,

        /// <summary>
        /// The operating environment of the UAV. For example when reporting that an aircraft is outside it's performance envelope (wind conditions, etc)
        /// </summary>
        Weather = 6,

        /// <summary>
        /// Uav lights, indicators, etc.
        /// </summary>
        Lights = 7
    }
}
