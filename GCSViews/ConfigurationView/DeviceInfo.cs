using MissionPlanner.Utilities;
using Newtonsoft.Json;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public class DeviceInfo
    {
        [JsonIgnore]
        public int _index;
        private Device.DeviceStructure _devid;

        public DeviceInfo(int index, string ParamName, uint id)
        {
            _index = index;
            this.ParamName = ParamName;
            _devid = new Device.DeviceStructure(ParamName, id);
        }

        public string ParamName { get; }

        public int DevID => (int)_devid.devid;

        public string BusType => _devid.bus_type.ToString().Replace("BUS_TYPE_", "");
        public int Bus => (int)_devid.bus;
        public int Address => (int)_devid.address;

        public string DevType
        {
            get
            {
                if (_devid.bus_type == Device.BusType.BUS_TYPE_UAVCAN)
                    return "SENSOR_ID#" + (_devid.devtype).ToString();

                if(ParamName.Contains("COMP"))
                    return _devid.devtypecompass.ToString().Replace("DEVTYPE_", "");

                if (ParamName.Contains("BARO"))
                    return _devid.devtypebaro.ToString().Replace("DEVTYPE_", "");

                if (ParamName.Contains("ASP"))
                    return _devid.devtypeairspd.ToString().Replace("DEVTYPE_", "");

                return _devid.devtypeimu.ToString().Replace("DEVTYPE_", "");
            }
        }

        public override string ToString()
        {
            return this.ToJSON(Formatting.None);
        }
    }
}