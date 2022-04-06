using System.Threading.Tasks;
using AltitudeAngelWings.Models;

namespace AltitudeAngelWings.Extra
{
    public interface IMissionPlanner
    {
        IMap FlightPlanningMap { get; }
        IMap FlightDataMap { get; }
        Task<FlightPlan> GetFlightPlan();
        Task CommandDroneToReturnToBase();
        Task CommandDroneToLoiter(
            float latitude,
            float longitude,
            float altitude);
        Task CommandDroneToLand(
            float latitude,
            float longitude);
        Task CommandDroneAllClear();
        Task NotifyConflict(string message);
        Task NotifyConflictResolved(string message);
        Task Disarm();
        Task ShowMessageBox(string message, string caption = "Message");
        Task<bool> ShowYesNoMessageBox(string message, string caption = "Message");
    }
}