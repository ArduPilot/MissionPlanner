using System;

namespace AltitudeAngelWings.Service.AltitudeAngelTelemetry.TelemetryEvents
{
    /// <summary>
    /// Telemetry event v1
    /// </summary>
    [Serializable]
    public class TelemetryEvent : ITelemetryEvent
    {
        public TelemetryEvent(Guid telemetryId, ITelemetryMessage message)
        {
            this.TelemetryId = telemetryId;
            this.Message = message;
        }

        /// <inheritdoc/>
        public int Version => 1;

        /// <summary>
        /// Timestamp event was received in the server
        /// </summary>
        public DateTime ReceivedTimestamp
        {
            get;
            set;
        }

        /// <summary>
        /// The id of one telemetry source which represents the data produced by an actor in this flight
        /// </summary>
        public Guid TelemetryId
        {
            get;
        }

        /// <summary>
        /// Sequence number incremented by 1 for each item sent
        /// </summary>
        public int SequenceNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Type of <see cref="ITelemetryMessage"/>
        /// </summary>
        public string MessageType => this.Message.MessageType;

        /// <summary>
        /// Telemetry message information
        /// </summary>
        public ITelemetryMessage Message { get; }
    }

    public class TelemetryEvent<T> : TelemetryEvent where T: ITelemetryMessage
    {
        public TelemetryEvent(Guid telemetryId, T message) :
            base(telemetryId, message)
        {
        }


        public new T Message => (T)base.Message;
    }
}

