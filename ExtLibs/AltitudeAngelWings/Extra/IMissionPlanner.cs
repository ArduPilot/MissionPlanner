using AltitudeAngelWings.Models;

namespace AltitudeAngelWings.Extra
{
    public interface IMissionPlanner
    {
        IMap FlightPlanningMap { get; }
        IMap FlightDataMap { get; }
        void SaveSetting(string key, string data);
        string LoadSetting(string key);
        void ClearSetting(string key);
        FlightPlan GetFlightPlan();
    }
}
