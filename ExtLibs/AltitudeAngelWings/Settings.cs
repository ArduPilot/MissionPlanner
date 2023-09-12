using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AltitudeAngelWings.Clients;
using AltitudeAngelWings.Clients.Api.Model;
using AltitudeAngelWings.Clients.Auth.Model;
using AltitudeAngelWings.Service.AltitudeAngelTelemetry.Encryption;
using Newtonsoft.Json;

namespace AltitudeAngelWings
{
    public class Settings : ISettings
    {
        private readonly Func<string, string> _loadSetting;
        private readonly Action<string> _clearSetting;
        private readonly Action<string, string> _saveSetting;
        private readonly SemaphoreSlim _saveLock = new SemaphoreSlim(1);

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

        public bool UseFlightPlans
        {
            get => Get(nameof(UseFlightPlans), true, bool.Parse);
            set => Set(nameof(UseFlightPlans), value);
        }

        public bool UseFlights
        {
            get => Get(nameof(UseFlights), true, bool.Parse);
            set => Set(nameof(UseFlights), value);
        }

        public string AuthenticationUrl => $"https://auth.{UrlDomainSuffix}";

        public string ApiUrl => $"https://api.{UrlDomainSuffix}";

        public string ClientId => OverrideClientUrlSettings ? OverrideClientId : "zHTnuEq0RAWoLy5thcvTtMdwX7r6et2L3MAhxv8a0";

        public string ClientSecret => OverrideClientUrlSettings ? OverrideClientSecret : "1ylYlXV4GuWJHIUywFg+XxE6hxsd3P/Dq5+J1PCUGxulC05/GC4Xpg==";

        public string[] ClientScopes => new[]
        {
            Scopes.QueryMapData,
            Scopes.QueryMapAirData,
            Scopes.TalkTower,
            Scopes.QueryUserInfo,
            Scopes.ManageFlightReports,
            Scopes.RequestFlightApprovals,
            Scopes.StrategicCrs,
            Scopes.TacticalCrs,
            Scopes.SurveillanceApi
        };

        public string RedirectUri => $"https://auth.{UrlDomainSuffix}/authorization/poll_complete";

        public string FlightServiceUrl => $"https://flight.{UrlDomainSuffix}";

        public string SurveillanceUrl => $"https://surveillance-api.{UrlDomainSuffix}";

        public string CdnUrl => UrlDomainSuffix == DefaultDomainSuffix
            ? "https://dronesafetymap.com"
            : $"https://map.{UrlDomainSuffix}";

        private const string DefaultDomainSuffix = "altitudeangel.com";

        public string UrlDomainSuffix => OverrideClientUrlSettings ? OverrideUrlDomainSuffix : DefaultDomainSuffix;

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
        public string OutboundNotificationsUrl
        {
            get => Get(nameof(OutboundNotificationsUrl), null, s => s);
            set => Set(nameof(OutboundNotificationsUrl), value);
        }

        public FlightTelemetry SendFlightTelemetry
        {
            get => Get(nameof(SendFlightTelemetry), FlightTelemetry.Surveillance, s => (FlightTelemetry)Enum.Parse(typeof(FlightTelemetry), s));
            set => Set(nameof(SendFlightTelemetry), value, t => Enum.GetName(typeof(FlightTelemetry), t));
        }

        public TokenResponse TokenResponse
        {
            get => Get(nameof(TokenResponse), null, JsonConvert.DeserializeObject<TokenResponse>);
            set => Set(nameof(TokenResponse), value, JsonConvert.SerializeObject);
        }

        public HashType EncryptionHashType => HashType.Hmac256;

        public SymmetricEncryptionType EncryptionType => SymmetricEncryptionType.Aes128;

        public string EncryptionKeySecret => "e05b7b90-3866-4eea-9b2a-73b02f46423a";

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

        public bool UseExistingFlightPlanId
        {
            get => Get(nameof(UseExistingFlightPlanId), false, bool.Parse);
            set => Set(nameof(UseExistingFlightPlanId), value);
        }

        public Guid ExistingFlightPlanId
        {
            get => Get(nameof(ExistingFlightPlanId), Guid.Empty, Guid.Parse);
            set => Set(nameof(ExistingFlightPlanId), value);
        }

        public string CurrentFlightPlanId
        {
            get => Get(nameof(CurrentFlightPlanId), null, s => s);
            set => Set(nameof(CurrentFlightPlanId), value);
        }

        public string CurrentFlightId
        {
            get => Get(nameof(CurrentFlightId), null, s => s);
            set => Set(nameof(CurrentFlightId), value);
        }

        public string FlightPlanName
        {
            get => Get(nameof(FlightPlanName), "Mission Planner", s => s);
            set => Set(nameof(FlightPlanName), value);
        }

        public string FlightPlanDescription
        {
            get => Get(nameof(FlightPlanDescription), "Mission Planner flight plan", s => s);
            set => Set(nameof(FlightPlanDescription), value);
        }

        public TimeSpan FlightPlanTimeSpan
        {
            get => Get(nameof(FlightPlanTimeSpan), TimeSpan.FromMinutes(60), TimeSpan.Parse);
            set => Set(nameof(FlightPlanTimeSpan), value);
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

        public int AltitudeFilter
        {
            get => Get(nameof(AltitudeFilter), 300, int.Parse);
            set => Set(nameof(AltitudeFilter), value);
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
            get => Get(nameof(MapUpdateThrottle), 1, double.Parse);
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
            if (setSetting == null)
            {
                setSetting = v => v.ToString();
            }

            try
            {
                _saveLock.Wait();
                if (value == null)
                {
                    _clearSetting(settingName);
                    return;
                }
                _saveSetting(settingName, setSetting(value));
            }
            finally
            {
                _saveLock.Release();
            }
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