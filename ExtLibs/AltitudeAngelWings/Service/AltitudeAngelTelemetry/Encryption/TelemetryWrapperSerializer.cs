using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;

namespace AltitudeAngelWings.Service.AltitudeAngelTelemetry.Encryption
{
    internal static class TelemetryWrapperSerializer
    {
        public static bool TryReadWrapper(byte[] bytes, out TelemetryWrapper wrapper)
        {
            wrapper = null;
            using (var stream = new MemoryStream(bytes))
            {
                using (var reader = new BinaryReader(stream))
                {
                    if (!reader.TryReadStruct(out TelemetryHeader header))
                    {
                        Debug.WriteLine("Could not read telemetry header", "Serialization Error");
                        return false;
                    }

                    if (header.MessageSize != reader.BytesRemaining())
                    {
                        Debug.WriteLine($"Invalid telemetry message size {reader.BytesRemaining()} expected {header.MessageSize}", "Serialization Error");
                        return false;
                    }

                    byte[] content = reader.ReadBytes(header.MessageSize);
                    wrapper = new TelemetryWrapper(header, content);

                    return true;
                }
            }
        }

        public static byte[] WriteWrapper(TelemetryWrapper item)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream))
                {
                    try
                    {
                        writer.WriteStruct(item.Header);
                        writer.Write(item.Message);
                    }
                    catch (Exception e) when (!(e is SerializationException))
                    {
                        throw new SerializationException(e.ToString());
                    }
                }

                return stream.ToArray();
            }
        }
    }
}
