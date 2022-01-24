namespace AltitudeAngelWings.ApiClient.Models.Strategic
{
    public class StrategicDroneDetails
    {
        /// <summary>
        /// Gets or sets the color of the drone
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// Gets or sets the markings on the drone
        /// </summary>
        public string Markings { get; set; }

        /// <summary>
        /// Gets or sets the drone airframe
        /// </summary>
        public StrategicAirframeType AirFrame { get; set; }

        /// <summary>
        /// Gets or sets the weight of the drone
        /// </summary>
        public double Weight { get; set; }

        /// <summary>
        /// Gets or sets the max weight of the drone
        /// </summary>
        public double MaxWeight { get; set; }
    }
}
