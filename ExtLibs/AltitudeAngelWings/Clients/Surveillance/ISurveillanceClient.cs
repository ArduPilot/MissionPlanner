using System;
using System.Threading;
using System.Threading.Tasks;
using AltitudeAngelWings.Clients.Surveillance.Model;

namespace AltitudeAngelWings.Clients.Surveillance
{
    public interface ISurveillanceClient : IDisposable
    {
        Task SendReport(SurveillanceReport surveillanceReport, CancellationToken cancellationToken = default);
    }
}