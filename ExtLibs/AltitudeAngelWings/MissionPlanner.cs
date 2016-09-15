using System;
using System.Collections.Generic;
using System.IO;
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

        public MissionPlanner()
        {
            FlightDataMap = new FMap();
            FlightPlanningMap = new FMap();
        }

        public void AddFlightMapMenuItem(string text, IPluginCommand pluginCommand)
        {
           
        }

        public void SaveSetting(string key, string data)
        {
            try
            {
                File.WriteAllText(key + ".txt", data);
            }
            catch
            {
                
            }
        }

        public string LoadSetting(string key)
        {
            try
            {
                return File.ReadAllText(key + ".txt");
            }
            catch
            {
                return null;
            }
        }

        public void ClearSetting(string key)
        {
           
        }
    }
}
