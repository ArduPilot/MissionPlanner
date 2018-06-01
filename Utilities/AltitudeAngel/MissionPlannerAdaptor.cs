using System;
using AltitudeAngel.IsolatedPlugin.Common;
using AltitudeAngel.IsolatedPlugin.Common.Maps;
using AltitudeAngelWings.Models;
using MissionPlanner.GCSViews;
using FlightData = MissionPlanner.GCSViews.FlightData;

namespace MissionPlanner.Utilities.AltitudeAngel
{
    public class MissionPlannerAdaptor : IMissionPlanner
    {
        public IMap FlightPlanningMap { get; set; }
        public IMap FlightDataMap { get; set; }

        public MissionPlannerAdaptor()
        {
            FlightDataMap = new MapAdapter(FlightData.instance.gMapControl1);
            FlightPlanningMap = new MapAdapter(FlightPlanner.instance.MainMap);
        }

        public void SaveSetting(string key, string data)
        {
            Settings.Instance[key] = data;
        }

        public string LoadSetting(string key)
        {
            if (Settings.Instance.ContainsKey(key))
                return Settings.Instance[key];

            return null;
        }

        public void ClearSetting(string key)
        {
            Settings.Instance.Remove(key);
        }
    }
}