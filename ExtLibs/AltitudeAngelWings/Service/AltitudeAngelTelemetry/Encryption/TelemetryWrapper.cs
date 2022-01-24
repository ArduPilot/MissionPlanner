using System;

namespace AltitudeAngelWings.Service.AltitudeAngelTelemetry.Encryption
{
    public class TelemetryWrapper
    {
        public TelemetryWrapper(short messageType, byte[] message)
        {
            if (message.Length > short.MaxValue)
            {
                throw new ArgumentException(string.Format(message.Length.ToString()),
                    nameof(message));
            }

            uint crc = Crc32.CalculateHash(message);
            this.Header = new TelemetryHeader(messageType, (short)message.Length, crc);
            this.Message = message;
        }

        public TelemetryWrapper(TelemetryHeader header, byte[] message)
        {
            this.Header = header;
            this.Message = message;

        }
        public TelemetryHeader Header
        {
            get;
        }

        public byte[] Message
        {
            get;
        }
    }
}
