using System.Collections.Generic;

namespace AltitudeAngelWings.ApiClient.Models.FlightV2.ServiceRequests
{
    /// <summary>
    /// Provides a response to a flight service request
    /// </summary>
    public interface IFlightServiceResponse
    {
        /// <summary>
        /// Name of the service providing the response
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Lists an conditions of use of the service. This may also include details of
        /// any restrictions or limitations as a result of the way the service is requested.
        /// </summary>
        List<object> Conditions { get; }

        /// <summary>
        /// Contains details of any errors in the service request.
        /// </summary>
        List<object> Errors { get; }
    }

    /// <summary>
    /// Provides a response to a flight service request with service specific properties
    /// </summary>
    public interface IFlightServiceResponse<T> : IFlightServiceResponse
    {
        /// <summary>
        /// Service response properties
        /// </summary>
        T Properties { get; }
    }
}
