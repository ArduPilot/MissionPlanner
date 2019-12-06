using System;
using System.Threading.Tasks;
using AltitudeAngelWings;
using AltitudeAngelWings.Extra;
using AltitudeAngelWings.Service;
using MissionPlanner.GCSViews;

namespace MissionPlanner.Utilities.AltitudeAngel
{
    internal static class AltitudeAngel
    {
        internal static void Configure()
        {
            AltitudeAngelPlugin.Configure();
            ServiceLocator.Register<IMissionPlanner>(l => new MissionPlannerAdaptor(
                new MapAdapter(FlightData.instance.gMapControl1),
                new MapAdapter(FlightPlanner.instance.MainMap),
                () => FlightPlanner.instance.GetFlightPlanLocations()));
            ServiceLocator.Register<IMissionPlannerState>(l => new MissionPlannerStateAdapter(
                () => MainV2.comPort.MAV.cs));
        }

        internal static async Task Initialize()
        {
            var settings = ServiceLocator.GetService<ISettings>();
            var service = ServiceLocator.GetService<IAltitudeAngelService>();
            if (settings.CheckEnableAltitudeAngel)
            {
                await service.SignInIfAuthenticated();
                return;
            }
            if (CustomMessageBox.Show(
                    "Do you wish to enable Altitude Angel airspace management data?\nFor more information visit [link;http://www.altitudeangel.com;www.altitudeangel.com]",
                    "Altitude Angel - Enable", CustomMessageBox.MessageBoxButtons.YesNo) == CustomMessageBox.DialogResult.Yes)
            {
                await service.SignInAsync();
            }
            settings.CheckEnableAltitudeAngel = true;
        }

        internal static void Dispose()
        {
            ServiceLocator.Clear();
        }
    }
}