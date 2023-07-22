using AltitudeAngelWings.ApiClient.Models.FlightV2;
using AltitudeAngelWings.ApiClient.Models.Strategic;
using AltitudeAngelWings.ApiClient.Models;
using AltitudeAngelWings.Models;
using System;
using System.Threading.Tasks;

namespace AltitudeAngelWings.ApiClient.Client.FlightClient
{
    public interface IFlightClient : IDisposable
    {
        Task CompleteFlight(string flightId);
        Task<CreateStrategicPlanResponse> CreateFlightPlan(FlightPlan flightPlan, UserProfileInfo currentUser);
        Task<StartFlightResponse> StartFlight(string flightPlanId);
        Task CancelFlightPlan(string flightPlanId);
        Task AcceptInstruction(string instructionId);
        Task RejectInstruction(string instructionId);
    }
}
