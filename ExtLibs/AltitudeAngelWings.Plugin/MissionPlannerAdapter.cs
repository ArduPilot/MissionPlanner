using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows.Forms;
using MissionPlanner;
using Resources = AltitudeAngelWings.Plugin.Properties.Resources;

namespace AltitudeAngelWings.Plugin
{
    public class MissionPlannerAdapter : IMissionPlanner
    {
        private readonly IUiThreadInvoke _uiThreadInvoke;
        private readonly ISettings _settings;
        public IMap FlightPlanningMap { get; }
        public IMap FlightDataMap { get; }
        public ProductInfoHeaderValue VersionHeader { get; }

        public MissionPlannerAdapter(IUiThreadInvoke uiThreadInvoke, IMap flightDataMap, IMap flightPlanningMap, ISettings settings, string titleVersionString)
        {
            FlightDataMap = flightDataMap;
            FlightPlanningMap = flightPlanningMap;
            _uiThreadInvoke = uiThreadInvoke;
            _settings = settings;
            var lastPart = titleVersionString.LastIndexOf(' ');
            if (lastPart > 0 && lastPart < titleVersionString.Length - 1)
            {
                titleVersionString = titleVersionString.Substring(lastPart + 1);
            }

            try
            {
                VersionHeader = new ProductInfoHeaderValue("MissionPlanner", titleVersionString);
            }
            catch (FormatException)
            {
                VersionHeader = new ProductInfoHeaderValue("MissionPlanner", "unknown");
            }
        }

        public Task CommandDroneToReturnToBase()
            => SetMode("RTL");

        public Task CommandDroneToLand(
            float latitude,
            float longitude)
            => SetMode("Land");

        public Task CommandDroneToLoiter(
            float latitude,
            float longitude,
            float altitude)
            => SetMode("Loiter");

        public Task CommandDroneAllClear()
            => SetMode("Auto");

        private static Task SetMode(string mode)
        {
            MainV2.comPort.setMode(MainV2.comPort.MAV.sysid, MainV2.comPort.MAV.compid, mode);
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task NotifyConflict(string message)
            => ShowMessageBox(message, Resources.MissionPlannerAdapterNotifyMessageTitle);

        /// <inheritdoc />
        public Task NotifyConflictResolved(string message)
            => ShowMessageBox(message, Resources.MissionPlannerAdapterNotifyMessageTitle);

        /// <inheritdoc />
        public Task Disarm()
            => MainV2.comPort.doARMAsync(MainV2.comPort.MAV.sysid, MainV2.comPort.MAV.compid, false);

        public Task ShowMessageBox(string message, string caption = null)
        {
            if (string.IsNullOrEmpty(caption))
            {
                caption = Resources.MissionPlannerAdapterMessageBoxDefaultCaption;
            }
            return _uiThreadInvoke.Invoke(() => CustomMessageBox.Show(message, caption));
        }

        public Task<bool> ShowYesNoMessageBox(string message, string caption = null)
        {
            if (string.IsNullOrEmpty(caption))
            {
                caption = Resources.MissionPlannerAdapterMessageBoxDefaultCaption;
            }
            return _uiThreadInvoke.Invoke(() =>
                CustomMessageBox.Show(message, caption, MessageBoxButtons.YesNo) == (int)DialogResult.Yes);
        }
    }
}