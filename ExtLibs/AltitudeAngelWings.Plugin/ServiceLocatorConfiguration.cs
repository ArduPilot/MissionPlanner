using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using AltitudeAngelWings.ApiClient.Client;
using AltitudeAngelWings.Extra;
using AltitudeAngelWings.Service;
using AltitudeAngelWings.Service.Messaging;
using GMap.NET.WindowsForms;
using MissionPlanner;
using MissionPlanner.GCSViews;
using MissionPlanner.Plugin;
using MissionPlanner.Utilities;
using Settings = AltitudeAngelWings.Service.Settings;

namespace AltitudeAngelWings.Plugin
{
    public class ServiceLocatorConfiguration : IServiceLocatorConfiguration
    {
        public void Configure()
        {
            ServiceLocator.Register<ISettings>(l => new Settings(
                key => l.Resolve<PluginHost>().config.ContainsKey(key) ? l.Resolve<PluginHost>().config[key] : null,
                key => l.Resolve<PluginHost>().config.Remove(key),
                (key, data) => l.Resolve<PluginHost>().config[key] = data));
            ServiceLocator.Register<IUiThreadInvoke>(l => new UiThreadInvoke(
                action => Task.Factory.FromAsync(l.Resolve<PluginHost>().MainForm.BeginInvoke(action), result => l.Resolve<PluginHost>().MainForm.EndInvoke(result))));
            ServiceLocator.Register<IMissionPlanner>(l => new MissionPlannerAdapter(
                l.Resolve<IUiThreadInvoke>(),
                new MapAdapter(l.Resolve<PluginHost>().FDGMapControl,
                    () => l.Resolve<ISettings>().EnableDataMap,
                    new MapInfoDockPanel(
                        l.Resolve<PluginHost>().FDGMapControl.Parent,
                        l.Resolve<IUiThreadInvoke>()),
                    l.Resolve<ISettings>()),
                new MapAdapter(l.Resolve<PluginHost>().FPGMapControl,
                    () => l.Resolve<ISettings>().EnablePlanMap,
                    new MapInfoDockPanel(
                        l.Resolve<PluginHost>().FPGMapControl.Parent,
                        l.Resolve<IUiThreadInvoke>()),
                    l.Resolve<ISettings>()),
                () => GetCurrentFlightPlan(l.Resolve<PluginHost>().MainForm.FlightPlanner),
                l.Resolve<ISettings>()));
            ServiceLocator.Register<IMissionPlannerState>(l => new MissionPlannerStateAdapter(
                () => l.Resolve<PluginHost>().comPort.MAV.cs));
            ServiceLocator.Register<IAuthorizeCodeProvider>(l => new WpfAuthorizeDisplay(
                l.Resolve<IUiThreadInvoke>(),
                l.Resolve<PluginHost>().MainForm));
            ServiceLocator.Register<IMessageDisplay>(l => new MessageDisplay(
                new [] {
                    l.Resolve<PluginHost>().FDGMapControl.Parent,
                    l.Resolve<PluginHost>().FPGMapControl.Parent
                },
                l.Resolve<PluginHost>().FDGMapControl.Parent.Controls["label4"],
                l.Resolve<IUiThreadInvoke>()));
        }

        private static IList<Locationwp> GetCurrentFlightPlan(FlightPlanner flightPlanner)
        {
            if (flightPlanner == null)
            {
                return new List<Locationwp>();
            }
            var getFlightPlanLocations = flightPlanner.GetType().GetMethod("GetFlightPlanLocations", BindingFlags.Instance | BindingFlags.NonPublic);
            return (IList<Locationwp>)getFlightPlanLocations.Invoke(flightPlanner, new object[] { });
        }
    }
}