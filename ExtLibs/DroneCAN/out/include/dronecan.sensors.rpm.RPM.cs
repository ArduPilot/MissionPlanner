using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;
using uint64_t = System.UInt64;

using int8_t = System.SByte;
using int16_t = System.Int16;
using int32_t = System.Int32;
using int64_t = System.Int64;

using float32 = System.Single;

using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace DroneCAN
{
    public partial class DroneCAN 
    {
        public partial class dronecan_sensors_rpm_RPM: IDroneCANSerialize 
        {
            public const int DRONECAN_SENSORS_RPM_RPM_MAX_PACK_SIZE = 7;
            public const ulong DRONECAN_SENSORS_RPM_RPM_DT_SIG = 0x140707C09274F6E7;
            public const int DRONECAN_SENSORS_RPM_RPM_DT_ID = 1045;

            public const double DRONECAN_SENSORS_RPM_RPM_FLAGS_UNHEALTHY = 1; // saturated uint16

            public uint8_t sensor_id = new uint8_t();
            public uint16_t flags = new uint16_t();
            public Single rpm = new Single();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan = false)
            {
                encode_dronecan_sensors_rpm_RPM(this, chunk_cb, ctx, fdcan);
            }

            public void decode(CanardRxTransfer transfer, bool fdcan = false)
            {
                decode_dronecan_sensors_rpm_RPM(transfer, this, fdcan);
            }

            public static dronecan_sensors_rpm_RPM ByteArrayToDroneCANMsg(byte[] transfer, int startoffset, bool fdcan = false)
            {
                var ans = new dronecan_sensors_rpm_RPM();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()), fdcan);
                return ans;
            }
        }
    }
}