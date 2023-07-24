using AltitudeAngelWings.ApiClient.Models.FlightV2;
using AltitudeAngelWings.ApiClient.Models.Strategic;
using AltitudeAngelWings.ApiClient.Models;
using AltitudeAngelWings.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AltitudeAngelWings.ApiClient.Client.FlightClient
{
    public interface IFlightClient : IDisposable
    {
        Task CompleteFlight(string flightId, CancellationToken cancellationToken = default);
        Task<CreateStrategicPlanResponse> CreateFlightPlan(FlightPlan flightPlan, UserProfileInfo currentUser, CancellationToken cancellationToken = default);
        Task<StartFlightResponse> StartFlight(string flightPlanId, CancellationToken cancellationToken = default);
        Task CompleteFlightPlan(string flightPlanId, CancellationToken cancellationToken = default);
        Task CancelFlightPlan(string flightPlanId, CancellationToken cancellationToken = default);
        Task AcceptInstruction(string instructionId, CancellationToken cancellationToken = default);
        Task RejectInstruction(string instructionId, CancellationToken cancellationToken = default);
    }
}
