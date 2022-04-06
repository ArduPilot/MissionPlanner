using System;
using System.Collections.Generic;

namespace AltitudeAngelWings.Service.AltitudeAngelTelemetry.TelemetryEvents
{
    [Serializable]
    public class UavPositionReport : ITelemetryMessage
    {
        /// <inheritdoc />
        public int Version => 1;

        /// <inheritdoc />
        public string MessageType => MessageTypes.UavPositionReport;


        /// <summary>
        /// The current position of the UAV
        /// </summary>
        public Position Pos { get; set; }

        /// <summary>
        /// Speed and direction of Uav
        /// </summary>
        public Velocity Velocity { get; set; }

        /// <summary>
        /// Altitude of Uav
        /// </summary>
        public Altitude Alt { get; set; }

        /// <summary>
        /// Percentage fuel/battery remaining
        /// </summary>
        public byte Fuel { get; set; }

        /// <summary>
        /// Number of Gps Satellites visible
        /// </summary>
        public byte SatellitesVisible { get; set; }

        /// <summary>
        /// The GPS Clock Time (UTC)
        /// </summary>
        public DateTime GpsTimestamp { get; set; }

        /// <summary>
        /// Is the UAV in the air or on the ground?
        /// </summary>
        public AirborneStatus IsAirborne { get; set; }

        /// <summary>
        /// A collection of status messages
        /// </summary>
        public ICollection<UavStatus> UavStatus { get; set; } = new List<UavStatus>();

    }
}
