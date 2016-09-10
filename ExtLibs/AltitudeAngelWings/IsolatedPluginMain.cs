using System;
using System.Runtime.Remoting.Lifetime;

namespace AltitudeAngel.IsolatedPlugin.Common
{
    public class IsolatedPluginMain : MarshalByRefObject, IDisposable
    {
        private MissionPlannerInterfaces _missionPlannerInterfaces;
        private ClientSponsor _clientSponsor;

        public MissionPlannerInterfaces MissionPlannerInterfaces
        {
            get
            {
                return _missionPlannerInterfaces;
            }
            set
            {
                if (_missionPlannerInterfaces != null)
                {
                    throw new InvalidOperationException("Property has already been set once and is immutable.");
                }

                _missionPlannerInterfaces = value;

                SetLeases();
            }
        }

        private void SetLeases()
        {
            _clientSponsor?.Close();
            _clientSponsor = new ClientSponsor(TimeSpan.MaxValue);
            _clientSponsor.Register((MarshalByRefObject) _missionPlannerInterfaces.CurrentState);
            //_clientSponsor.Register((MarshalByRefObject) _missionPlannerInterfaces.FlightComms);
            _clientSponsor.Register((MarshalByRefObject) _missionPlannerInterfaces.MissionPlanner);

            var lease = (ILease)_clientSponsor.InitializeLifetimeService();
            _clientSponsor.Renewal(lease);
        }


        public virtual bool Initialize()
        {
            return true;
        }

        public virtual bool Loaded()
        {
            return true;
        }

        public virtual bool Exit()
        {
            return true;
        }

        public virtual bool Loop()
        {
            return true;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                _clientSponsor?.Close();
                _clientSponsor = null;
            }
        }
    }
}
