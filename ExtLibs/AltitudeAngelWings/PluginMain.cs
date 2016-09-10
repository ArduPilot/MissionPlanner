using System;
using System.Reactive.Subjects;
using AltitudeAngel.IsolatedPlugin.Common;
using AltitudeAngelWings.Modules;
using AltitudeAngelWings.Service.FlightData;
using Autofac;

namespace AltitudeAngelWings
{
    public class PluginMain : IsolatedPluginMain
    {
        public static PluginMain Instance { get; private set; }
        public IObservable<long> PollMessages => _pollSubject;
        public UserInterfaceMain InterfaceMain { get; private set; }

        public PluginMain()
        {
            Instance = this;
        }

        public override bool Initialize()
        {
            //MissionPlannerInterfaces.MissionPlanner.LoopRateHz = 1;

            ConfigureContainer();
            ConfigureUI();

            return true;
        }

        public override bool Loaded()
        {
            InterfaceMain.WaitUntilUIReady();

            // Resolve these now to create them
            // Could normally use AutoFac's AutoActivate but since I can't control the thread
            // things create on some UI components get created on a background thread
            // and it explodes if we do that.
            _container.Resolve<FlightDataService>().Initialize();

            InterfaceMain.ShowMainWindow();

            return true;
        }

        public override bool Loop()
        {
            _pollSubject.OnNext(0);

            return true;
        }

        public override bool Exit()
        {
            if (InterfaceMain != null)
            {
                InterfaceMain.Dispose();
                InterfaceMain = null;
            }

            if (_lifetimeScope != null)
            {
                _lifetimeScope.Dispose();
                _lifetimeScope = null;
            }

            return true;
        }

        private void ConfigureContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<CoreModule>();

            _container = builder.Build();

            _lifetimeScope = _container.BeginLifetimeScope();
        }

        private void ConfigureUI()
        {
            InterfaceMain = _container.Resolve<UserInterfaceMain>();
            InterfaceMain.Run();

        }

        private readonly ISubject<long> _pollSubject = new Subject<long>();
        private IContainer _container;
        private ILifetimeScope _lifetimeScope;
    }
}
