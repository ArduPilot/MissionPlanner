using System;
using System.Collections.Generic;
using System.Configuration;
using AltitudeAngelWings.Extra;
using DotNetOpenAuth.OAuth2;
using Newtonsoft.Json;

namespace AltitudeAngelWings.Service
{
    public class Settings : ISettings
    {
        private readonly IMissionPlanner _missionPlanner;

        public Settings(IMissionPlanner missionPlanner)
        {
            _missionPlanner = missionPlanner;
        }

        public bool CheckEnableAltitudeAngel
        {
            get => Get("AACheck2", false, bool.Parse);
            set => Set("AACheck2", value);
        }

        public string AuthenticationUrl => ConfigurationManager.AppSettings["AuthURL"];

        public string ApiUrl => ConfigurationManager.AppSettings["APIURL"];

        public string ClientId => ConfigurationManager.AppSettings["ClientId"];

        public string ClientSecret => ConfigurationManager.AppSettings["ClientSecret"];

        public IAuthorizationState AuthToken
        {
            get => Get("AAWings.Token", null, JsonConvert.DeserializeObject<AuthorizationState>);
            set => Set("AAWings.Token", value, JsonConvert.SerializeObject);
        }

        public bool GroundDataDisplay
        {
            get => Get("AA.Ground", true, bool.Parse);
            set => Set("AA.Ground", value);
        }

        public bool AirDataDisplay
        {
            get => Get("AA.Air", true, bool.Parse);
            set => Set("AA.Air", value);
        }

        public IList<string> MapFilters
        {
            get => Get("AAWings.Filters", new List<string>(), JsonConvert.DeserializeObject<List<string>>);
            set => Set("AAWings.Filters", value, JsonConvert.SerializeObject);
        }

        public bool FlightReportEnable
        {
            get => Get("AA.FlightReportEnable", true, bool.Parse);
            set => Set("AA.FlightReportEnable", value);
        }

        public string CurrentFlightReportId
        {
            get => Get("AA.CurrentFlightReportId", null, s => s);
            set => Set("AA.CurrentFlightReportId", value);
        }

        public string FlightReportName
        {
            get => Get("AA.FlightReportName", "MissionPlanner Flight", s => s);
            set => Set("AA.FlightReportName", value);
        }

        public bool FlightReportCommercial
        {
            get => Get("AA.FlightReportCommercial", false, bool.Parse);
            set => Set("AA.FlightReportCommercial", value);
        }

        public TimeSpan FlightReportTimeSpan
        {
            get => Get("AA.FlightReportTimeSpan", TimeSpan.FromMinutes(60), TimeSpan.Parse);
            set => Set("AA.FlightReportTimeSpan", value);
        }

        private T Get<T>(string settingName, T defaultValue, Func<string, T> getSetting)
        {
            return string.IsNullOrEmpty(_missionPlanner.LoadSetting(settingName))
                ? defaultValue
                : getSetting(_missionPlanner.LoadSetting(settingName));
        }

        private void Set<T>(string settingName, T value, Func<T, string> setSetting = null)
        {
            if (value == null)
            {
                _missionPlanner.ClearSetting(settingName);
                return;
            }
            if (setSetting == null)
            {
                setSetting = v => v.ToString();
            }
            _missionPlanner.SaveSetting(settingName, setSetting(value));
        }
    }
}