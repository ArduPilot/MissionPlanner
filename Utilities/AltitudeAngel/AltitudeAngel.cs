using System;
using System.Threading.Tasks;
using AltitudeAngelWings;
using AltitudeAngelWings.ApiClient.Client;
using AltitudeAngelWings.Extra;
using AltitudeAngelWings.Service;
using AltitudeAngelWings.Service.FlightService;
using AltitudeAngelWings.Service.AltitudeAngelTelemetry;
using AltitudeAngelWings.Service.Messaging;
using MissionPlanner.GCSViews;

namespace MissionPlanner.Utilities.AltitudeAngel
{
    internal static class AltitudeAngel
    {
        internal static void Configure()
        {
            AltitudeAngelPlugin.Configure();
            ServiceLocator.Register<ISettings>(l => new AltitudeAngelWings.Service.Settings(
                key => Settings.Instance.ContainsKey(key) ? Settings.Instance[key] : null,
                key => Settings.Instance.Remove(key),
                (key, data) =>  Settings.Instance[key] = data));
            ServiceLocator.Register<IUiThreadInvoke>(l => new UiThreadInvoke(
                action => Task.Factory.FromAsync(MainV2.instance.BeginInvoke(action), result => MainV2.instance.EndInvoke(result))));
            ServiceLocator.Register<IMissionPlanner>(l => new MissionPlannerAdapter(
                l.Resolve<IUiThreadInvoke>(),
                new MapAdapter(FlightData.instance.gMapControl1,
                    () => l.Resolve<ISettings>().EnableDataMap,
                            new MapInfoDockPanel(
                                FlightData.instance.gMapControl1.Parent,
                                l.Resolve<IUiThreadInvoke>()),
                    l.Resolve<ISettings>()),
                new MapAdapter(FlightPlanner.instance.MainMap,
                    () => l.Resolve<ISettings>().EnablePlanMap,
                        new MapInfoDockPanel(
                            FlightPlanner.instance.MainMap.Parent,
                            l.Resolve<IUiThreadInvoke>()),
                    l.Resolve<ISettings>()),
                () => FlightPlanner.instance.GetFlightPlanLocations(),
                l.Resolve<ISettings>()));
            ServiceLocator.Register<IMissionPlannerState>(l => new MissionPlannerStateAdapter(
                () => MainV2.comPort.MAV.cs));
            ServiceLocator.Register<IAuthorizeCodeProvider>(l => new WpfAuthorizeDisplay(
                l.Resolve<IUiThreadInvoke>()));
            ServiceLocator.Register<IMessageDisplay>(l => new MessageDisplay(
                new [] {
                    FlightData.instance.gMapControl1.Parent,
                    FlightPlanner.instance.MainMap.Parent
                },
                FlightData.instance.gMapControl1.Parent.Controls["label4"],
                l.Resolve<IUiThreadInvoke>()));
        }

        internal static async Task Initialize()
        {
            var settings = ServiceLocator.GetService<ISettings>();

            var service = ServiceLocator.GetService<IAltitudeAngelService>();
            var telemetryService = ServiceLocator.GetService<ITelemetryService>();
            var flightService = ServiceLocator.GetService<IFlightService>();
            var messageDisplay = ServiceLocator.GetService<IMessageDisplay>();

            if (settings.CheckEnableAltitudeAngel)
            {
                await service.SignInAsync();
                return;
            }
            if (!Settings.Instance.GetBoolean("AA_check", false) && CustomMessageBox.Show(
                    "Do you wish to enable Altitude Angel airspace management data?\nFor more information visit [link;http://www.altitudeangel.com;www.altitudeangel.com]",
                    "Altitude Angel - Enable", CustomMessageBox.MessageBoxButtons.YesNo) == CustomMessageBox.DialogResult.Yes)
            {
                settings.CheckEnableAltitudeAngel = true;
                await service.SignInAsync();
                return;
            }

            Settings.Instance["AA_check"] = true.ToString();
        }

        internal static void Dispose()
        {
            ServiceLocator.Clear();
        }
    }
}