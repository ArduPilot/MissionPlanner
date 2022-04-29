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
        private static bool _configured;

        internal static void Configure()
        {
            if (_configured)
                return;
            _configured = true;
            AltitudeAngelPlugin.Configure();
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
            ServiceLocator.Register<ISettings>(l => new AltitudeAngelWings.Service.Settings(
                key => Settings.Instance.ContainsKey(key) ? Settings.Instance[key] : null,
                key => Settings.Instance.Remove(key),
                (key, data) => Settings.Instance[key] = data));

            var settings = ServiceLocator.GetService<ISettings>();

            if (settings.CheckEnableAltitudeAngel)
            {
                AltitudeAngel.Configure();
                var service = ServiceLocator.GetService<IAltitudeAngelService>();
                await service.SignInAsync();
                return;
            }
            if (!Settings.Instance.GetBoolean("AACheck2", false) && CustomMessageBox.Show(
                    "Do you wish to enable Altitude Angel airspace management data?\nFor more information visit [link;http://www.altitudeangel.com;www.altitudeangel.com]",
                    "Altitude Angel - Enable", CustomMessageBox.MessageBoxButtons.YesNo) == CustomMessageBox.DialogResult.Yes)
            {
                AltitudeAngel.Configure();
                settings.CheckEnableAltitudeAngel = true;
                var service = ServiceLocator.GetService<IAltitudeAngelService>();
                await service.SignInAsync();
                return;
            }

            Settings.Instance["AACheck2"] = true.ToString();
        }

        internal static void Dispose()
        {
            ServiceLocator.Clear();
        }
    }
}