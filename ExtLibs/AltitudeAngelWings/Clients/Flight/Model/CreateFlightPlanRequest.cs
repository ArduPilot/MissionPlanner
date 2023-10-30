using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace AltitudeAngelWings.Clients.Flight.Model
{
    public class CreateFlightPlanRequest
    {
        /// <summary>
        ///     The summary/title of the flight
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        ///     A short description of the flight
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Contact details for the pilot
        /// </summary>
        public ContactDetails PointOfContact { get; set; }

        /// <summary>
        ///     The serial number of the drone that will be flying to this flight plan, if applicable
        /// </summary>
        public string DroneSerialNumber { get; set; }

        /// <summary>
        ///     The 24-bit ICAO Address the drone is flying under, in hexadecimal format, if applicable
        /// </summary>
        public string IcaoAddress { get; set; }

        /// <summary>
        ///     Details on the drone carrying out the flight plan
        /// </summary>
        public CreateFlightPlanRequestDroneDetails DroneDetails { get; set; }

        /// <summary>
        ///     The flight's operation mode
        /// </summary>
        public FlightOperationMode FlightOperationMode { get; set; }

        /// <summary>
        ///     The flight characteristics of the drone, if applicable
        /// </summary>
        public FlightCapability FlightCapability { get; set; }

        /// <summary>
        ///     Each part that the flight is segmented into
        /// </summary>
        public List<CreateFlightPartRequest> Parts { get; set; }

        /// <summary>
        ///     Extra data as required by aviation authorities in the flight area
        /// </summary>
        public JObject DataFields { get; set; }

        /// <summary>
        ///     The reason for the flight i.e. Commercial or Recreational
        /// </summary>
        public FlightReason FlightReason { get; set; }

        /// <summary>
        ///     The owners of the Flight Plan. Used if more complex ownership than "the user initiating the request owns the plan"
        ///     is required.
        /// </summary>
        public FlightPlanOwners Owners { set; get; }

        /// <summary>
        ///     Contact details for the Organization behind the flight
        /// </summary>
        public FlightPlanRequestOrganization Organization { get; set; }

        /// <summary>
        ///     If true (default), and providing the client is authorized to do so, approval will be requested for the provided flight plan
        ///     If false, no approvals will be requested and the flight plan is for information only. The request will generate a "NoService" response.
        /// </summary>
        public bool RequestApproval { get; set; } = true;

        /// <summary>
        /// Set to false to suppress normal emails from flight - safety critical emails will still be sent.
        /// </summary>
        public bool SendEmails { get; set; } = true;
    }
}
