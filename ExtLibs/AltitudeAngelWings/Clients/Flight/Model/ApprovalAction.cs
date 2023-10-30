using System.Collections.Generic;

namespace AltitudeAngelWings.Clients.Flight.Model
{
    public class ApprovalAction
    {
        /// <summary>
        /// The id of the action
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// A short description of the action to be displayed to the user. e.g. Button text
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// A list of required data fields which must be included when using this action.
        /// It is assumed that the client knows how to send a given data item.
        /// </summary>
        public List<string> RequiredData { get; set; } = new List<string>();
    }
}