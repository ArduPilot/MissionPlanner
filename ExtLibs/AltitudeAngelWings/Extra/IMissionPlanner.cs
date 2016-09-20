using AltitudeAngel.IsolatedPlugin.Common.Maps;

namespace AltitudeAngel.IsolatedPlugin.Common
{
    public interface IMissionPlanner
    {
        IMap FlightPlanningMap { get; }
        IMap FlightDataMap { get; }
        void SaveSetting(string key, string data);
        string LoadSetting(string key);
        void ClearSetting(string key);
    }
}
