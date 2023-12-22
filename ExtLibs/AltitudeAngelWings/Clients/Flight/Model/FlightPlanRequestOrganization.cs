namespace AltitudeAngelWings.Clients.Flight.Model
{
    public class FlightPlanRequestOrganization
    {
        /// <summary>
        /// The name of the organization
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The primary phone number of the organization
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// The primary email address of the organization
        /// </summary>
        public string Email { get; set; }
    }
}