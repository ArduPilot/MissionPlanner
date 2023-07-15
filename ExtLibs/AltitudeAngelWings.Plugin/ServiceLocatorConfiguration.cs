using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using AltitudeAngelWings.ApiClient.Client;
using AltitudeAngelWings.Extra;
using AltitudeAngelWings.Service;
using AltitudeAngelWings.Service.Messaging;
using MissionPlanner.GCSViews;
using MissionPlanner.Plugin;
using MissionPlanner.Utilities;
using Polly;
using Settings = AltitudeAngelWings.Service.Settings;

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
                         new Lazy<IAltitudeAngelClient>(l.Resolve<IAltitudeAngelClient>)),
                    l.Resolve<ISettings>(),
                    l.Resolve<IMessagesService>(),
                    false),
                new MapAdapter(l.Resolve<PluginHost>().FPGMapControl,
                    () => l.Resolve<ISettings>().EnablePlanMap,
                    new MapInfoDockPanel(
                        l.Resolve<PluginHost>().FPGMapControl.Parent,
                        l.Resolve<IUiThreadInvoke>(),
                        new Lazy<IAltitudeAngelClient>(l.Resolve<IAltitudeAngelClient>)),
                    l.Resolve<ISettings>(),
                    l.Resolve<IMessagesService>(),
                    true),
                () => GetCurrentFlightPlan(l.Resolve<PluginHost>().MainForm.FlightPlanner),
                l.Resolve<ISettings>(),
                l.Resolve<PluginHost>().MainForm.Text));
            ServiceLocator.Register<IMissionPlannerState>(l => new MissionPlannerStateAdapter(
                () => l.Resolve<PluginHost>().comPort.MAV.cs));
            ServiceLocator.Register<IAuthorizeCodeProvider>(l => new ExternalWebBrowserAuthorizeCodeProvider(
                l.Resolve<ISettings>(),
                l.Resolve<IAsyncPolicy>(),
                l.Resolve<IMissionPlanner>().VersionHeader));
            ServiceLocator.Register<IMessageDisplay>(l => new MessageDisplay(
                new [] {
                    l.Resolve<PluginHost>().FDGMapControl.Parent,
                    l.Resolve<PluginHost>().FPGMapControl.Parent
                },
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