using System;
using System.Collections.Generic;
using DotNetOpenAuth.OAuth2;

namespace AltitudeAngelWings.Service
{
    public interface ISettings
    {
        bool CheckEnableAltitudeAngel { get; set; }
        string AuthenticationUrl { get; }
        string ApiUrl { get; }
        string ClientId { get; }
        string ClientSecret { get; }
        IAuthorizationState AuthToken { get; set; }
        bool GroundDataDisplay { get; set; }
        bool AirDataDisplay { get; set; }
        IList<string> MapFilters { get; set; }
        bool FlightReportEnable { get; set; }
        string CurrentFlightReportId { get; set; }
        string FlightReportName { get; set; }
        bool FlightReportCommercial { get; set; }
        TimeSpan FlightReportTimeSpan { get; set; }
    }
}