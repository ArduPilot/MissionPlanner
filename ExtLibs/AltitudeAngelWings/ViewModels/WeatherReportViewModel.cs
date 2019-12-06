using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AltitudeAngelWings.ApiClient.Models;
using AltitudeAngelWings.Service;

namespace AltitudeAngelWings.ViewModels
{
    public class WeatherReportViewModel : INotifyPropertyChanged
    {
        public string Temperature { get; private set; }
        public string WindSpeed { get; private set; }
        public string WindDirection { get; private set; }


        public WeatherReportViewModel(WeatherInfo weatherReport)
        {
            if (weatherReport == null)
            {
                return;
            }


            Temperature = $"{(int)weatherReport.TemperatureC}C";
            WindSpeed = $"{(int)weatherReport.WindSpeedKph} km/h";
            WindDirection = weatherReport.WindDirection;
        }


        #region INotifyPropertyChanged/Changing

        public event PropertyChangedEventHandler PropertyChanged;
        public int TempC { get; set; }
        public int TempF { get; set; }
        public int WindMph { get; set; }
        public int WindKph { get; set; }
        public string IconUrl { get; set; }

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
