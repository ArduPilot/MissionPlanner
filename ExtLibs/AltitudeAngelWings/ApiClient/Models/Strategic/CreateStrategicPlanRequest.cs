using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace AltitudeAngelWings.ApiClient.Models.Strategic
{
    class CreateStrategicPlanRequest
    {
        public string Summary { get; set; }

        public string Description { get; set; }

        public List<CreateStrategicPlanPartRequest> Parts { get; set; }

        public JObject DataFields { get; set; }

        /// <summary>
        /// The Id of the operator to which the flight belongs
        /// Presumably unique per OrgId
        /// </summary>
        public string ExternalOperatorId { get; set; }

        /// <summary>
        /// The regulations related to the request
        /// </summary>
        public IList<string> Regulations { get; set; }

        /// <summary>
        /// A collection of identifiers relating to the flightplan
        /// </summary>
        public IDictionary<string, string> Identifiers { get; set; }

        /// <summary>
        /// The point of contact for the flightplan
        /// </summary>
        public StrategicContactDetails PointOfContact { get; set; }

        /// <summary>
        /// True if the flight plan is a draft
        /// </summary>
        public bool IsDraft { get; set; }

        /// <summary>
        /// Gets or sets the drone details
        /// </summary>
        public StrategicDroneDetails DroneDetails { get; set; }

        /// <summary>
        /// Gets or sets the conflict resolution scope
        /// </summary>
        public StrategicConflictResolutionScope ConflictResolutionScope { get; set; }
    }
}
