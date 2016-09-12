using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AltitudeAngel.IsolatedPlugin.Common;
using AltitudeAngel.IsolatedPlugin.Common.Maps;

namespace AltitudeAngelWings
{
    public class MissionPlanner: IMissionPlanner
    {
        public float LoopRateHz { get; set; }
        public IMap FlightPlanningMap { get; }
        public IMap FlightDataMap { get; }

        public void AddFlightMapMenuItem(string text, IPluginCommand pluginCommand)
        {
           
        }

        public void SaveSetting(string key, string data)
        {
            
        }

        public string LoadSetting(string key)
        {
            return null;
        }

        public void ClearSetting(string key)
        {
           
        }
    }
}
