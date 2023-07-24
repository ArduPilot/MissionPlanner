using System.Linq;

namespace AltitudeAngelWings.Clients.Surveillance.Model
{
    public class Target
    {
        /// <summary>
        /// Construct a new <see cref="Target"/> with defined properties
        /// </summary>
        public Target(string name, SurveillanceIdentifier[] ids, string type, int confidence, object additionalInfo)
        {
            Name = name;
            Ids = ids;
            Id = ids?.FirstOrDefault()?.Id ?? string.Empty;
            Type = type;
            Confidence = confidence;
            AdditionalInfo = additionalInfo;
        }


        /// <summary>
        /// Human readable name of the flying object
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Unique Id that represents the flying object (highest preference in Ids)
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// URN encoding of the flying object
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// percentage confidence of classification of flying object
        /// </summary>
        public int Confidence { get; }

        /// <summary>
        /// Additional info regarding the flying object.
        /// </summary>
        public object AdditionalInfo { get; }


        /// <summary>
        /// Ids that represent the flying object in order of preference
        /// </summary>
        public SurveillanceIdentifier[] Ids { get; }
    }
}