using System.Diagnostics;

namespace AltitudeAngelWings.Service.Logging
{
    public class Logger : ILogger
    {
        public TraceSource Default { get; set; }
        public TraceSource Client { get; set; }

        public Logger()
        {
            Default = new TraceSource("AAWings.Default");
            Client = new TraceSource("AAWings.Client");
        }
    }
}
