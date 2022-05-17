using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using AltitudeAngelWings.ApiClient.Client;
using AltitudeAngelWings.Service.AltitudeAngelTelemetry.Encryption;
using Newtonsoft.Json;

namespace AltitudeAngelWings.Service
{
    public class Settings : ISettings
    {
        private readonly Func<string, string> _loadSetting;
        private readonly Action<string> _clearSetting;
        private readonly Action<string, string> _saveSetting;

        public Settings(Func<string, string> loadSetting, Action<string> clearSetting, Action<string, string> saveSetting)
        {
            _loadSetting = loadSetting;
            _clearSetting = clearSetting;
            _saveSetting = saveSetting;
        }

        public bool CheckEnableAltitudeAngel
        {
            get => Get(nameof(CheckEnableAltitudeAngel), true, bool.Parse);
            set => Set(nameof(CheckEnableAltitudeAngel), value);
        }

        public bool SurveillanceMode
        {
            get
            {
                var value = ConfigurationManager.AppSettings["SurveillanceMode"];
                bool.TryParse(value, out var result);
                return result;
            }
        }

        public string AuthenticationUrl => $"https://auth.{UrlDomainSuffix}";

        public string ApiUrl => $"https://api.{UrlDomainSuffix}";

        public string ClientId => OverrideClientUrlSettings ? OverrideClientId : ConfigurationManager.AppSettings["ClientId"];

        public string ClientSecret => OverrideClientUrlSettings ? OverrideClientSecret : ConfigurationManager.AppSettings["ClientSecret"];

        public string[] ClientScopes => new[]
        {
            "query_mapdata",
            "query_mapairdata",
            "talk_tower",
            "query_userinfo",
            "manage_flightreports",
            "strategic_crs",
            "tactical_crs"
        };

        public string RedirectUri => "https://aawings.com/";

        public string FlightServiceUrl => $"https://flight.{UrlDomainSuffix}";

        public string UrlDomainSuffix => OverrideClientUrlSettings ? OverrideUrlDomainSuffix : ConfigurationManager.AppSettings["UrlDomainSuffix"];

        public bool OverrideClientUrlSettings
        {
            get => Get(nameof(OverrideClientUrlSettings), false, bool.Parse);
            set => Set(nameof(OverrideClientUrlSettings), value);
        }

        public string OverrideClientId
        {
            get => Get(nameof(OverrideClientId), null, s => s);
            set => Set(nameof(OverrideClientId), value);
        }

        public string OverrideClientSecret
        {
            get => Get(nameof(OverrideClientSecret), null, s => s);
            set => Set(nameof(OverrideClientSecret), value);
        }

        public string OverrideUrlDomainSuffix
        {
            get => Get(nameof(OverrideUrlDomainSuffix), "altitudeangel.com", s => s);
            set => Set(nameof(OverrideUrlDomainSuffix), value);
        }
        public string OutboundNotifsEndpointUrl
        {
            get => Get(nameof(OutboundNotifsEndpointUrl), null, s => s);
            set => Set(nameof(OutboundNotifsEndpointUrl), value);
        }

        /// <inheritdoc />
        public bool DisableTelemetrySending => Convert.ToBoolean(ConfigurationManager.AppSettings["DisableTelemetrySending"]);

        public TokenResponse TokenResponse
        {
            get => Get(nameof(TokenResponse), null, JsonConvert.DeserializeObject<TokenResponse>);
            set => Set(nameof(TokenResponse), value, JsonConvert.SerializeObject);
        }

        public HashType EncryptionHashType => HashType.Hmac256; //TODO add to config file to hide from mission planner

        public SymmetricEncryptionType EncryptionType => SymmetricEncryptionType.Aes128; //TODO add to config file to hide from mission planner

        public string EncryptionKeySecret => ConfigurationManager.AppSettings["AA.Telemetry.EncryptionKeySecret"];

        public TimeSpan MinimumPollInterval
        {
            get => Get(nameof(MinimumPollInterval), TimeSpan.FromMilliseconds(500), s => TimeSpan.FromMilliseconds(double.Parse(s)));
            set => Set(nameof(MinimumPollInterval), value);
        }

        public IList<FilterInfoDisplay> MapFilters
        {
            get => Get(nameof(MapFilters), new List<FilterInfoDisplay>(), JsonConvert.DeserializeObject<List<FilterInfoDisplay>>);
            set => Set(nameof(MapFilters), value, JsonConvert.SerializeObject);
        }

        public bool FlightReportEnable
        {
            get => Get(nameof(FlightReportEnable), true, bool.Parse);
            set => Set(nameof(FlightReportEnable), value);
        }

        public bool UseExistingFlightPlanId
        {
            get => Get(nameof(UseExistingFlightPlanId), false, bool.Parse);
            set => Set(nameof(UseExistingFlightPlanId), value);
        }

        public bool UseFlightPlanLocalScope
        {
            get => Get(nameof(UseFlightPlanLocalScope), false, bool.Parse);
            set => Set(nameof(UseFlightPlanLocalScope), value);
        }

        public Guid ExistingFlightPlanId
        {
            get => Get(nameof(ExistingFlightPlanId), Guid.Empty, Guid.Parse);
            set => Set(nameof(ExistingFlightPlanId), value);
        }

        public string CurrentFlightReportId
        {
            get => Get(nameof(CurrentFlightReportId), null, s => s);
            set => Set(nameof(CurrentFlightReportId), value);
        }

        public string CurrentFlightId
        {
            get => Get(nameof(CurrentFlightId), null, s => s);
            set => Set(nameof(CurrentFlightId), value);
        }

        public string FlightReportName
        {
            get => Get(nameof(FlightReportName), "Mission Planner", s => s);
            set => Set(nameof(FlightReportName), value);
        }

        public string FlightReportDescription
        {
            get => Get(nameof(FlightReportDescription), "Mission Planner flight report", s => s);
            set => Set(nameof(FlightReportDescription), value);
        }

        public bool FlightReportCommercial
        {
            get => Get(nameof(FlightReportCommercial), false, bool.Parse);
            set => Set(nameof(FlightReportCommercial), value);
        }

        public TimeSpan FlightReportTimeSpan
        {
            get => Get(nameof(FlightReportTimeSpan), TimeSpan.FromMinutes(60), TimeSpan.Parse);
            set => Set(nameof(FlightReportTimeSpan), value);
        }

        public string CurrentTelemetryId
        {
            get => Get(nameof(CurrentTelemetryId), null, s => s);
            set => Set(nameof(CurrentTelemetryId), value);
        }

        public string EncryptionKey
        {
            get => Get(nameof(EncryptionKey), null, s => s);
            set => Set(nameof(EncryptionKey), value);
        }

        public string TelemetryHostName
        {
            get => Get(nameof(TelemetryHostName), null, s => s);
            set => Set(nameof(TelemetryHostName), value);
        }

        public int TelemetryPortNumber
        {
            get => Get(nameof(TelemetryPortNumber), 0, int.Parse);
            set => Set(nameof(TelemetryPortNumber), value);
        }

        public int TransmissionRateInMilliseconds
        {
            get => Get(nameof(TransmissionRateInMilliseconds), 500, int.Parse);
            set => Set(nameof(TransmissionRateInMilliseconds), value);
        }
        
        public float MapOpacityAdjust
        {
            get => Get(nameof(MapOpacityAdjust), 1f, float.Parse);
            set => Set(nameof(MapOpacityAdjust), value);
        }

        public bool EnableDataMap
        {
            get => Get(nameof(EnableDataMap), true, bool.Parse);
            set => Set(nameof(EnableDataMap), value);
        }

        public bool EnablePlanMap
        {
            get => Get(nameof(EnablePlanMap), true, bool.Parse);
            set => Set(nameof(EnablePlanMap), value);
        }

        public double MapUpdateThrottle
        {
            get => Get(nameof(MapUpdateThrottle), 0.25, double.Parse);
            set => Set(nameof(MapUpdateThrottle), value);
        }

        public double MapUpdateRefresh
        {
            get => Get(nameof(MapUpdateRefresh), 10, double.Parse);
            set => Set(nameof(MapUpdateRefresh), value);
        }

        public string FlightPhoneNumber
        {
            get => Get(nameof(FlightPhoneNumber), "+447999999999", s => s);
            set => Set(nameof(FlightPhoneNumber), value);
        }

        public bool FlightAllowSms
        {
            get => Get(nameof(FlightAllowSms), false, bool.Parse);
            set => Set(nameof(FlightAllowSms), value);
        }

        public bool FlightIdentifierIcao
        {
            get => Get(nameof(FlightIdentifierIcao), false, bool.Parse);
            set => Set(nameof(FlightIdentifierIcao), value);
        }

        public string FlightIdentifierIcaoAddress
        {
            get => Get(nameof(FlightIdentifierIcaoAddress), "AC82EC", s => s);
            set => Set(nameof(FlightIdentifierIcaoAddress), value);
        }

        public bool FlightIdentifierSerial
        {
            get => Get(nameof(FlightIdentifierSerial), false, bool.Parse);
            set => Set(nameof(FlightIdentifierSerial), value);
        }

        public string FlightIdentifierSerialNumber
        {
            get => Get(nameof(FlightIdentifierSerialNumber), "0123456789", s => s);
            set => Set(nameof(FlightIdentifierSerialNumber), value);
        }

        private T Get<T>(string settingName, T defaultValue, Func<string, T> getSetting)
        {
            settingName = CheckAndPrefixSettingName(settingName);
            return string.IsNullOrEmpty(_loadSetting(settingName))
                ? defaultValue
                : getSetting(_loadSetting(settingName));
        }

        private void Set<T>(string settingName, T value, Func<T, string> setSetting = null)
        {
            settingName = CheckAndPrefixSettingName(settingName);
            if (value == null)
            {
                _clearSetting(settingName);
                return;
            }
            if (setSetting == null)
            {
                setSetting = v => v.ToString();
            }
            _saveSetting(settingName, setSetting(value));
        }

        private static string CheckAndPrefixSettingName(string settingName)
        {
            if (string.IsNullOrEmpty(settingName))
            {
                throw new ArgumentException($"The argument {nameof(settingName)} cannot be null or empty.");
            }

            char[] invalidChars = { '/', ' ', '-', ':', ';', '.' };
            if (settingName.Any(c => invalidChars.Contains(c)))
            {
                throw new ArgumentException(
                    $"The argument {nameof(settingName)} '{settingName}' contains an invalid character.");
            }

            return $"AA_{settingName}";
        }
    }
}