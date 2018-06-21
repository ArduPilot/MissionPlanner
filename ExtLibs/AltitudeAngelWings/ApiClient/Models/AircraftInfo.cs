namespace AltitudeAngelWings.ApiClient.Models
{
    public class AircraftInfo
    {
        /// <summary>
        ///     The id of the aircraft in the AA system.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///     The take off weight of the aircraft in KG (optional).
        /// </summary>
        /// <remarks>
        ///     If omitted then the manufacturers default weight is assumed.
        /// </remarks>
        public double? TakeoffWeightKg { get; set; }

        /// <summary>
        ///     Does the aircraft have a camera equipped.
        /// </summary>
        public bool HasCamera { get; set; }
    }
}
