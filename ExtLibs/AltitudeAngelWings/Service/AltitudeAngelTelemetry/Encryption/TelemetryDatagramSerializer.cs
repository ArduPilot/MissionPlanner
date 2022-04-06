using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;

namespace AltitudeAngelWings.Service.AltitudeAngelTelemetry.Encryption
{
    internal static class TelemetryDatagramSerializer
    {
        public static bool TryReadDatagram(byte[] receivedBytes, out UdpTelemetryDatagram datagram)
        {
            datagram = null;
            using (var stream = new MemoryStream(receivedBytes))
            {
                using (var reader = new BinaryReader(stream))
                {
                    if (!reader.TryReadStruct(out DatagramHeader header))
                    {
                        Debug.WriteLine("Could not read DatagramHeader", "Serialization Error");
                        return false;
                    }

                    if (header.Version != 1)
                    {
                        Debug.WriteLine($"Unknown datagram version {header.Version}", "Serialization Error");
                        return false;
                    }

                    if (reader.BytesRemaining() != header.PayloadLength)
                    {
                        Debug.WriteLine($"Invalid datagram payload size. Expected {header.PayloadLength} bytes, got {reader.BytesRemaining()}", "Serialization Error");
                        return false;
                    }

                    datagram = new UdpTelemetryDatagram
                    {
                        Header = header,
                        Payload = reader.ReadBytes(header.PayloadLength)
                    };
                    return true;
                }
            }
        }

        public static byte[] WriteDatagram(UdpTelemetryDatagram datagram)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream))
                {
                    try
                    {
                        writer.WriteStruct(datagram.Header);
                        writer.Write(datagram.Payload);
                    }
                    catch (Exception e)
                    {
                        throw new SerializationException(e.ToString());
                    }
                }

                return stream.ToArray();
            }
        }
    }
}
