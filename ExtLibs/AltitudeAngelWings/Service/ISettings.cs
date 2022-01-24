using System;
using System.Collections.Generic;
using AltitudeAngelWings.ApiClient.Client;
using AltitudeAngelWings.Service.AltitudeAngelTelemetry.Encryption;

namespace AltitudeAngelWings.Service
{
    public interface ISettings
    {
        bool CheckEnableAltitudeAngel { get; set; }
        string AuthenticationUrl { get; }
        string ApiUrl { get; }
        string FlightServiceUrl { get; }
        string ClientId { get; }
        string ClientSecret { get; }
        bool SurveillanceMode { get; }
        TokenResponse TokenResponse { get; set; }
        IList<FilterInfoDisplay> MapFilters { get; set; }
        bool FlightReportEnable { get; set; }
        bool UseExistingFlightPlanId { get; set; }
        bool UseFlightPlanLocalScope { get; set; }
        Guid ExistingFlightPlanId { get; set; }
        string CurrentFlightReportId { get; set; }
        string CurrentFlightId { get; set; }
        string FlightReportName { get; set; }
        string FlightReportDescription { get; set; }
        bool FlightReportCommercial { get; set; }
        TimeSpan FlightReportTimeSpan { get; set; }
        HashType EncryptionHashType { get; }
        string EncryptionKeySecret { get; }
        SymmetricEncryptionType EncryptionType { get; }
        string CurrentTelemetryId { get; set; }
        string EncryptionKey { get; set; }
        string TelemetryHostName { get; set; }
        int TelemetryPortNumber { get; set; }
        int TransmissionRateInMilliseconds { get; set; }
        TimeSpan MinimumPollInterval { get; set; }
        string OutboundNotifsEndpointUrl { get; set; }
        bool DisableTelemetrySending { get; }
        string UrlDomainSuffix { get; }
        bool OverrideClientUrlSettings { get; set; }
        string OverrideClientId { get; set; }
        string OverrideClientSecret { get; set; }
        string OverrideUrlDomainSuffix { get; set; }
        string[] ClientScopes { get; }
        string RedirectUri { get; }
        float MapOpacityAdjust { get; set; }
        bool EnableDataMap { get; set; }
        bool EnablePlanMap { get; set; }
        double MapUpdateThrottle { get; set; }
        double MapUpdateRefresh { get; set; }
        string FlightPhoneNumber { get; set; }
        bool FlightAllowSms { get; set; }
        bool FlightIdentifierIcao { get; set; }
        string FlightIdentifierIcaoAddress { get; set; }
        bool FlightIdentifierSerial { get; set; }
        string FlightIdentifierSerialNumber { get; set; }
    }

}