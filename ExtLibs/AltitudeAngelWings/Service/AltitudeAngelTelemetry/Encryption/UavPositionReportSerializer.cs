using AltitudeAngelWings.Service.AltitudeAngelTelemetry.TelemetryEvents;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace AltitudeAngelWings.Service.AltitudeAngelTelemetry.Encryption
{
    public static class UavPositionReportSerializer
    {
        public static bool TryReadReport(byte[] bytes, out UavPositionReport report)
        {
            report = null;
            using (var stream = new MemoryStream(bytes))
            {
                using (var reader = new BinaryReader(stream))
                {
                    if (!reader.TryReadStruct(out UavPosition position))
                    {
                        Debug.WriteLine("Could not read UavPosition", "Serialization Error");
                        return false;
                    }

                    if (reader.BytesRemaining() < 1)
                    {
                        Debug.WriteLine("Missing UavPosition.NumStatuses", "Serialization Error");
                        return false;
                    }
                    byte numStatuses = reader.ReadByte();

                    int expectedBytes = numStatuses * Marshal.SizeOf(typeof(AutpStatusMessage));
                    if (reader.BytesRemaining() < expectedBytes)
                    {
                        Debug.WriteLine($"Invalid status message bytes remaining. Expected {expectedBytes} bytes, got {reader.BytesRemaining()}", "Serialization Error");
                        return false;
                    }

                    var statuses = new List<AutpStatusMessage>();
                    for (byte i = 0; i < numStatuses; i++)
                    {
                        statuses.Add(reader.ReadStruct<AutpStatusMessage>());
                    }

                    report = position.ToPositionReport();
                    report.UavStatus = statuses.Select(s => s.ToUavStatus()).ToList();

                    return true;
                }
            }
        }

        public static byte[] WriteReport(UavPositionReport item)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream))
                {
                    try
                    {
                        var position = item.ToUavPosition();

                        writer.WriteStruct(position);
                        writer.Write((byte)item.UavStatus.Count);
                        foreach (UavStatus status in item.UavStatus)
                        {
                            writer.WriteStruct(status.ToAutpStatusMessage());
                        }
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
