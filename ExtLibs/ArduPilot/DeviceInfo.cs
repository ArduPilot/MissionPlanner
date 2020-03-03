namespace MissionPlanner.ArduPilot
{
    public struct DeviceInfo
    {
        /// <summary>
        /// Com Port Name
        /// </summary>
        public string name;
        /// <summary>
        /// Windows device description
        /// </summary>
        public string description;
        /// <summary>
        /// usb reported device description
        /// </summary>
        public string board;
        /// <summary>
        /// device vid pid in windows format
        /// </summary>
        public string hardwareid;
    }
}