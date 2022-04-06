using System;
using System.Runtime.InteropServices;

namespace AltitudeAngelWings.Service.AltitudeAngelTelemetry.TelemetryEvents
{
    /// <summary>
    /// Defines altitude.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Altitude
    {
        /// <summary>
        /// Create a new altitude 
        /// </summary>
        /// <param name="value">Height in metres</param>
        /// <param name="datum">Point of measurement</param>
        /// <param name="accuracy">Accuracy of value in meters</param>
        public Altitude(int value, AltitudeDatum datum, ushort accuracy)
        {
            this.Accuracy = accuracy;
            this.Datum = datum;
            this.Value = value;
        }

        /// <summary>
        /// Height in metres
        /// </summary>
        public int Value { get; }

        /// <summary>
        /// Altitude datum measurement
        /// </summary>
        public AltitudeDatum Datum { get; }

        /// <summary>
        /// Accuracy of <see cref="Value"/> in metres
        /// </summary>
        public ushort Accuracy { get; }
    }
}
