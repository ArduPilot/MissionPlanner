using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using AltitudeAngel.IsolatedPlugin.Common;
using AltitudeAngelWings.Service;
using AltitudeAngelWings.Service.FlightData;
using AltitudeAngelWings.Service.FlightData.Providers;
using AltitudeAngelWings.Service.Logging;
using AltitudeAngelWings.Service.Messaging;
using AltitudeAngelWings.Views;
using Autofac;
using Module = Autofac.Module;

namespace AltitudeAngelWings.Modules
{
    public class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(ThisAssembly);

            builder.RegisterType<MissionPlannerFlightDataProvider>()
                   .As<IFlightDataProvider>()
                   .SingleInstance();

            builder.RegisterType<FlightDataService>()
                   .AsSelf()
                   .WithParameter(new TypedParameter(typeof (IObservable<long>), PluginMain.Instance.PollMessages))
                   .SingleInstance();

            builder.RegisterType<AltitudeAngelService>()
                   .AsSelf()
                   .SingleInstance();

            builder.RegisterType<Logger>()
                   .AsSelf()
                   .As<ILogger>()
                   .SingleInstance();

            builder
                .RegisterType<MessagesService>()
                .As<IMessagesService>()
                .SingleInstance();

            HookupViewBuildup(builder);
        }

        private void HookupViewBuildup(ContainerBuilder builder)
        {
            IEnumerable<Type> viewTypes = Assembly.GetExecutingAssembly()
                                                  .GetTypes()
                                                  .Where(
                                                      t => t.GetInterface(typeof (IView<>).FullName) != null &&
                                                          (t.IsSubclassOf(typeof (Page)) || t.IsSubclassOf(typeof (Window))));

            foreach (Type viewType in viewTypes)
            {
                builder.RegisterType(viewType)
                       .AsSelf()
                       .AsImplementedInterfaces()
                       .OnActivating(i => { BuildupView(i.Context, (DependencyObject) i.Instance); });
            }
        }

        private void BuildupView(IComponentContext context, DependencyObject instance)
        {
            try
            {
                if (instance == null)
                    return;

                foreach (object c in LogicalTreeHelper.GetChildren(instance))
                {
                    var child = c as DependencyObject;
                    if (child == null)
                        continue;

                    BuildupView(context, child);
                }

                Type viewInterfaceType = instance.GetType().GetInterface(typeof (IView<>).FullName);
                if (viewInterfaceType == null)
                    return;

                PropertyInfo viewModelProperty = viewInterfaceType.GetProperty("ViewModel");
                Type viewModelType = viewInterfaceType.GetGenericArguments()[0];


                object viewModel = context.Resolve(viewModelType);
                viewModelProperty.SetValue(instance, viewModel, null);

                viewInterfaceType.GetMethod("ViewInitialized").Invoke(instance, null);
            }
            catch (Exception e)
            {
            }
        }
    }
}
