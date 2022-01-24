using System;
using System.Runtime.InteropServices;

namespace AltitudeAngelWings.Service.AltitudeAngelTelemetry.TelemetryEvents
{
    /// <summary>
    /// Speed and direction
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Velocity
    {
        public Velocity(float x, float y, float z, float accuracy)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.Accuracy = accuracy;
        }

        /// <summary>
        /// Latitude in metres per second
        /// </summary>
        public float X { get;  }

        /// <summary>
        /// Longitude in etres per second
        /// </summary>
        public float Y { get; }

        /// <summary>
        /// Altitude in metres per second
        /// </summary>
        public float Z { get; }

        /// <summary>
        /// Metres per second
        /// </summary>
        public float Accuracy { get; }
    }
}
