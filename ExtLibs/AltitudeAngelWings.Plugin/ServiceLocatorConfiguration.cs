using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AltitudeAngelWings.Clients;
using AltitudeAngelWings.Clients.Api;
using AltitudeAngelWings.Clients.Auth;
using AltitudeAngelWings.Model;
using AltitudeAngelWings.Service.Messaging;
using MissionPlanner.GCSViews;
using MissionPlanner.Plugin;
using MissionPlanner.Utilities;

namespace AltitudeAngelWings.Plugin
{
    public class ServiceLocatorConfiguration : IServiceLocatorConfiguration
    {
        public void Configure()
        {
            ServiceLocator.Register<ISettings>(l => new Settings(
                key => l.Resolve<PluginHost>().config.ContainsKey(key) ? l.Resolve<PluginHost>().config[key] : null,
                key =>
                {
                    var host = l.Resolve<PluginHost>();
                    host.config.Remove(key);
                    host.config.Save();
                },
                (key, data) =>
                {
                    var host = l.Resolve<PluginHost>();
                    host.config[key] = data;
                    host.config.Save();
                }));
            ServiceLocator.Register<IUiThreadInvoke>(l => new UiThreadInvoke(
                action => Task.Factory.FromAsync(l.Resolve<PluginHost>().MainForm.BeginInvoke(action), result => l.Resolve<PluginHost>().MainForm.EndInvoke(result))));
            ServiceLocator.Register<IMissionPlanner>(l => new MissionPlannerAdapter(
                l.Resolve<IUiThreadInvoke>(),
                new MapAdapter(l.Resolve<PluginHost>().FDGMapControl,
                    () => l.Resolve<ISettings>().EnableDataMap,
                    new MapInfoDockPanel(
                        l.Resolve<PluginHost>().FDGMapControl.Parent,
                        l.Resolve<IUiThreadInvoke>(),
                         new Lazy<IApiClient>(() => l.Resolve<IApiClient>())),
                    l.Resolve<ISettings>(),
                    l.Resolve<IMessagesService>(),
                    false),
                new MapAdapter(l.Resolve<PluginHost>().FPGMapControl,
                    () => l.Resolve<ISettings>().EnablePlanMap,
                    new MapInfoDockPanel(
                        l.Resolve<PluginHost>().FPGMapControl.Parent,
                        l.Resolve<IUiThreadInvoke>(),
                        new Lazy<IApiClient>(() => l.Resolve<IApiClient>())),
                    l.Resolve<ISettings>(),
                    l.Resolve<IMessagesService>(),
                    true),
                l.Resolve<ISettings>(),
                l.Resolve<PluginHost>().MainForm.Text));
            ServiceLocator.Register<IMissionPlannerState>(l => new MissionPlannerStateAdapter(
                () => l.Resolve<PluginHost>().comPort.MAV.cs,
                () => GetCurrentWaypoints(l.Resolve<PluginHost>().MainForm.FlightPlanner),
                () => l.Resolve<PluginHost>().comPort.MAV.ToFlightCapability()));
            ServiceLocator.Register<IAuthorizeCodeProvider>(l => new ExternalWebBrowserAuthorizeCodeProvider(
                l.Resolve<ISettings>(),
                l.Resolve<IAuthClient>(),
                l.Resolve<IMessagesService>(),
                l.Resolve<PluginHost>(),
                l.Resolve<IUiThreadInvoke>()));
            ServiceLocator.Register<IMessageDisplay>(l => new MultipleMessageDisplay(
                new TextWriterMessageDisplay(Console.Out),
                new ControlOverlayMessageDisplay(
                    l.Resolve<PluginHost>().FDGMapControl.Parent,
                    l.Resolve<IUiThreadInvoke>(),
                    55),
                new ControlOverlayMessageDisplay(
                    l.Resolve<PluginHost>().FPGMapControl.Parent,
                    l.Resolve<IUiThreadInvoke>(),
                    10)));
        }

        private static IList<FlightPlanWaypoint> GetCurrentWaypoints(FlightPlanner flightPlanner)
        {
            if (flightPlanner == null)
            {
                return new List<FlightPlanWaypoint>();
            }
            var getFlightPlanLocations = flightPlanner.GetType().GetMethod("GetFlightPlanLocations", BindingFlags.Instance | BindingFlags.NonPublic);
            if (getFlightPlanLocations == null)
            {
                return new List<FlightPlanWaypoint>();
            }
            var locations = (IList<Locationwp>)getFlightPlanLocations.Invoke(flightPlanner, new object[] { });
            return locations.ToWaypoints().ToList();
        }
    }
}