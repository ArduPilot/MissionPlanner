using log4net;
using MissionPlanner.ArduPilot;
using MissionPlanner.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MissionPlanner.Joystick
{
    /// <summary>
    /// How a joystick button drives an RCx_OPTION auxiliary function (stored in JoyButton.p2)
    /// </summary>
    public enum auxfunctiontrigger
    {
        /// <summary>send switch HIGH when the button is pressed</summary>
        HighOnPress = 0,
        /// <summary>send switch HIGH on press and switch LOW on release (momentary)</summary>
        HighOnPressLowOnRelease = 1,
        /// <summary>alternate between switch HIGH and switch LOW on each press</summary>
        ToggleHighLow = 2,
        /// <summary>send switch MIDDLE when the button is pressed</summary>
        MiddleOnPress = 3,
        /// <summary>send switch LOW when the button is pressed</summary>
        LowOnPress = 4,
        /// <summary>cycle LOW -> MIDDLE -> HIGH -> LOW on each press (for 3 position options)</summary>
        CycleLowMiddleHigh = 5,
    }

    /// <summary>
    /// Helpers for the RCx_OPTION auxiliary function list, sourced from the per-vehicle
    /// parameter metadata so the list always matches the firmware and vehicle type.
    /// </summary>
    public static class AuxFunction
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Human readable descriptions of the trigger behaviours, in enum order
        /// </summary>
        public static List<KeyValuePair<int, string>> GetTriggerList()
        {
            return new List<KeyValuePair<int, string>>
            {
                new KeyValuePair<int, string>((int) auxfunctiontrigger.HighOnPress, "High on press"),
                new KeyValuePair<int, string>((int) auxfunctiontrigger.HighOnPressLowOnRelease, "High on press, Low on release"),
                new KeyValuePair<int, string>((int) auxfunctiontrigger.ToggleHighLow, "Toggle High / Low each press"),
                new KeyValuePair<int, string>((int) auxfunctiontrigger.MiddleOnPress, "Middle on press"),
                new KeyValuePair<int, string>((int) auxfunctiontrigger.LowOnPress, "Low on press"),
                new KeyValuePair<int, string>((int) auxfunctiontrigger.CycleLowMiddleHigh, "Cycle Low / Middle / High each press"),
            };
        }

        /// <summary>
        /// Returns the RCx_OPTION values (number, description) valid for the given vehicle firmware.
        /// "0:Do Nothing" is excluded. Returns an empty list if no metadata is available.
        /// </summary>
        public static List<KeyValuePair<int, string>> GetList(Firmwares firmware)
        {
            var vehicle = firmware.ToString();

            // every ArduPilot vehicle exposes the same list on each RCn_OPTION; try a few in case
            // a metadata file is missing a channel
            foreach (var param in new[] {"RC7_OPTION", "RC6_OPTION", "RC8_OPTION", "RC5_OPTION"})
            {
                List<KeyValuePair<int, string>> list;
                try
                {
                    list = ParameterMetaDataRepository.GetParameterOptionsInt(param, vehicle);
                }
                catch (System.Exception ex)
                {
                    log.Error("Failed to read " + param + " metadata for " + vehicle, ex);
                    continue;
                }

                if (list != null && list.Count > 0)
                {
                    return list.Where(a => a.Key != 0)
                        .GroupBy(a => a.Key).Select(g => g.First())
                        .OrderBy(a => a.Key)
                        .ToList();
                }
            }

            return new List<KeyValuePair<int, string>>();
        }

        /// <summary>
        /// Description for an aux function number for the given vehicle, or null if unknown
        /// </summary>
        public static string GetName(Firmwares firmware, int function)
        {
            foreach (var item in GetList(firmware))
            {
                if (item.Key == function)
                    return item.Value;
            }

            return null;
        }
    }
}
