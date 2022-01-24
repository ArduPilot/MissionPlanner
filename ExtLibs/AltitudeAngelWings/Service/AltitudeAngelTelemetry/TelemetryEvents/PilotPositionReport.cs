using System;
namespace AltitudeAngelWings.Service.AltitudeAngelTelemetry.TelemetryEvents
{
    /// <summary>
    /// Pilot position report
    /// </summary>
    [Serializable]
    public class PilotPositionReport : ITelemetryMessage
    {
        public string MessageType => MessageTypes.PilotPositionReport;

        public int Version => 1;

        public Position Pos
        {
            get;
            set;
        }
    }
}
