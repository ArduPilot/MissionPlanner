using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using AltitudeAngelWings.ApiClient.Models;
using AltitudeAngelWings.Commands;
using AltitudeAngelWings.Service;

namespace AltitudeAngelWings.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ICommand SignInCommand
        {
            get { return _signInCommand; }
            set
            {
                if (value != _signInCommand)
                {
                    _signInCommand = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand DisconnectCommand
        {
            get { return _disconnectCommand; }
            set
            {
                if (value != _disconnectCommand)
                {
                    _disconnectCommand = value;
                    OnPropertyChanged();
                }
            }
        }

        public WeatherReportViewModel WeatherReport
        {
            get { return _weatherReport; }
            set
            {
                if (value != _weatherReport)
                {
                    _weatherReport = value;
                    OnPropertyChanged();
                }
            }
        }


        public ObservableProperty<SignInStates> SignInState { get; private set; }
        public ObservableProperty<PermitStates> PermitStatus { get; }
        public ObservableProperty<bool> TelemetryPulse { get; }

        public string SignInStatus
        {
            get { return _signInStatus; }
            set
            {
                if (value != _signInStatus)
                {
                    _signInStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        public MainViewModel()
        {
            CreateSampleData();
        }

        public MainViewModel(AltitudeAngelService altitudeAngelService)
        {
            _altitudeAngelService = altitudeAngelService;
            SignInCommand = new DelegateCommandAsync<object>(ExecuteConnect, true);
            DisconnectCommand = new DelegateCommandAsync<object>(ExecuteDisconnect, true);
            SignInState = new ObservableProperty<SignInStates>(SignInStates.NotSignedIn);
            PermitStatus = new ObservableProperty<PermitStates>(PermitStates.NoPermit);
            TelemetryPulse = new ObservableProperty<bool>();


            _altitudeAngelService.IsSignedIn
                                 .Subscribe(UpdateSignInState);

            _altitudeAngelService.WeatherReport
                                 .ObserveOnDispatcher()
                                 .Subscribe(WeatherChanged);

            _altitudeAngelService.SentTelemetry
                                 .Subscribe(i => NewTelemetry());
        }

        private void NewTelemetry()
        {
            TelemetryPulse.Value = true;
        }


        private void WeatherChanged(WeatherInfo weatherReport)
        {
            WeatherReport = weatherReport == null 
                ? null 
                : new WeatherReportViewModel(weatherReport);
        }


        private void UpdateSignInState(bool signedIn)
        {
            SignInState.Value = signedIn ? SignInStates.SignedIn : SignInStates.NotSignedIn;

            string userName = signedIn
                ? $"{_altitudeAngelService.CurrentUser.FirstName} {_altitudeAngelService.CurrentUser.LastName}"
                : null;

            SignInStatus = signedIn ? $"({userName}) CONNECTED" : "NOT CONNECTED";
        }

        private async Task ExecuteConnect(object notUsed)
        {
            try
            {
                await _altitudeAngelService.SignInAsync();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private Task ExecuteDisconnect(object arg)
        {
            return _altitudeAngelService.DisconnectAsync();
        }

        private void CreateSampleData()
        {
            SignInState = new ObservableProperty<SignInStates>(SignInStates.SignedIn);
            SignInStatus = "NOT CONNECTED";

            WeatherReport = new WeatherReportViewModel(new WeatherInfo
            {
                TemperatureC = 32,
                At = new DateTime(2016, 01, 01, 12, 45, 23),
                CloudCoverPercent = 12,
                WindDirection = "ENE",
                Summary = "Sunny with light cloud"
            });
        }

        private readonly AltitudeAngelService _altitudeAngelService;


        private ICommand _disconnectCommand;
        private ICommand _signInCommand;
        private string _signInStatus;
        private WeatherReportViewModel _weatherReport;

        public enum PermitStates
        {
            NoPermit,
            PermitGranted
        }

        public enum SignInStates
        {
            NotSignedIn,
            SignedIn
        }

        #region INotifyPropertyChanged/Changing

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
