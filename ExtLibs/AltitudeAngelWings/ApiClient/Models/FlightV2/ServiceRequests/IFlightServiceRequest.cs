namespace AltitudeAngelWings.ApiClient.Models.FlightV2.ServiceRequests
{
    /// <summary>
    /// Requests an in-flight service
    /// </summary>
    public interface IFlightServiceRequest
    {
        /// <summary>
        /// Name of the service being requested
        /// </summary>
        string Name { get; }
    }

    /// <summary>
    /// Requests an in-flight service with some service specific properties
    /// </summary>
    public interface IFlightServiceRequest<T> : IFlightServiceRequest
    {
        /// <summary>
        /// Service specific properties
        /// </summary>
        T Properties { get; }
    }
}
