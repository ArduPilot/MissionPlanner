using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MissionPlanner.Joystick
{
    public struct JoyChannel
    {
        public int channel;
        [JsonConverter(typeof(StringEnumConverter))]
        public joystickaxis axis;
        public bool reverse;
        public int expo;
    }
}