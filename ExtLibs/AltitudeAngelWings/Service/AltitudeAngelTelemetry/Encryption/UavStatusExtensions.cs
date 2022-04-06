using AltitudeAngelWings.Service.AltitudeAngelTelemetry.TelemetryEvents;
using System;

namespace AltitudeAngelWings.Service.AltitudeAngelTelemetry.Encryption
{
    static class UavStatusExtensions
    {
        private const byte StatusMask = 0b00000011;
        private const byte CodeMask = 0b11111100;

        /// <summary>
        /// Convert a <see cref="AutpStatusMessage"/> to a <see cref="UavStatus"/>
        /// </summary>
        /// <returns>A UavStatus object</returns>
        public static UavStatus ToUavStatus(this AutpStatusMessage autpStatus)
        {
            if (!Enum.IsDefined(typeof(EquipmentId), (int)autpStatus.EquipmentId))
            {
                throw new Exception();
            }

            byte status = (byte)(autpStatus.Status & UavStatusExtensions.StatusMask);
            byte code = (byte)((autpStatus.Status & UavStatusExtensions.CodeMask) >> 2);

            return new UavStatus((EquipmentId)autpStatus.EquipmentId, autpStatus.Index, (EquipmentStatus)status, code);
        }

        /// <summary>
        /// Convert a <see cref="UavStatus"/> to a <see cref="AutpStatusMessage"/>
        /// </summary>
        /// <returns>An AutpStatusMessage</returns>
        public static AutpStatusMessage ToAutpStatusMessage(this UavStatus uavStatus)
        {
            byte status = (byte)((byte)uavStatus.Status & UavStatusExtensions.StatusMask);
            byte code = (byte)(uavStatus.Code << 2 & UavStatusExtensions.CodeMask);

            return new AutpStatusMessage
            {
                EquipmentId = (byte)uavStatus.EquipmentId,
                Index = (byte)uavStatus.Index,
                Status = (byte)(code | status)
            };
        }
    }
}
