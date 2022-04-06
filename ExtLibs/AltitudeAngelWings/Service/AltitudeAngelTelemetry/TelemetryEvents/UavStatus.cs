using System;
using System.Runtime.InteropServices;

namespace AltitudeAngelWings.Service.AltitudeAngelTelemetry.TelemetryEvents
{
    /// <summary>
    /// An indication of a part of the UAV or its operating environment
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct UavStatus
    {
        public UavStatus(EquipmentId equipmentId, int index, EquipmentStatus status, int code)
        {
            if (code > (1 << 6))
            {
                throw new ArgumentException($"Error code must be in the range 0-{1<<6}", nameof(code));
            }

            this.EquipmentId = equipmentId;
            this.Index = index;
            this.Status = status;
            this.Code = code;
        }

        /// <summary>
        /// Equipment id or operating environment
        /// </summary>
        public EquipmentId EquipmentId { get; }

        /// <summary>
        /// Used to indicate the number of the item with an issue (e.g. Engine 4).
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// Equipment status
        /// </summary>
        public EquipmentStatus Status { get; }

        /// <summary>
        /// A custom error code for this specific device
        /// </summary>
        public int Code { get; }
    }
}
