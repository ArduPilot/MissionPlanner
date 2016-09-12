using AltitudeAngel.IsolatedPlugin.Common.Maps;

namespace AltitudeAngel.IsolatedPlugin.Common
{
    public interface IMissionPlanner
    {
        float LoopRateHz { get; set; }
        IMap FlightPlanningMap { get; }
        IMap FlightDataMap { get; }
        void AddFlightMapMenuItem(string text, IPluginCommand pluginCommand);
        void SaveSetting(string key, string data);
        string LoadSetting(string key);
        void ClearSetting(string key);
    }
}
