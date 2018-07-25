using System.Diagnostics;

namespace AltitudeAngelWings.Service.Logging
{
    public interface ILogger
    {
        TraceSource Default { get; set; }
        TraceSource Client { get; set; }
    }
}
