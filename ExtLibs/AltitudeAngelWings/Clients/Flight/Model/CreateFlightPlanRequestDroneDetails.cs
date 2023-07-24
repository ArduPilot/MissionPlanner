using System.Collections.Generic;

namespace AltitudeAngelWings.Clients.Flight.Model
{
    public class CreateFlightPlanRequestDroneDetails
    {
        /// <summary>
        /// The primary color of the drone
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// Any distinctive markings on the drone
        /// </summary>
        public string Markings { get; set; }

        /// <summary>
        /// The airframe of the drone
        /// </summary>
        public AirFrameType AirFrame { get; set; }

        /// <summary>
        /// The maximum take-off weight of the drone in kg
        /// </summary>
        public double MaxWeight { get; set; }

        /// <summary>
        /// The manufacturer of the drone.
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// The model of the drone.
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// Registration Ids for the drone
        /// </summary>
        public IDictionary<string, string> RegistrationIds { get; set; }
    }
}