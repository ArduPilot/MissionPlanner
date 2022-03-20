
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
        public partial class dronecan_sensors_hygrometer_Hygrometer: IDroneCANSerialize 
        {
            public const int DRONECAN_SENSORS_HYGROMETER_HYGROMETER_MAX_PACK_SIZE = 5;
            public const ulong DRONECAN_SENSORS_HYGROMETER_HYGROMETER_DT_SIG = 0xCEB308892BF163E8;
            public const int DRONECAN_SENSORS_HYGROMETER_HYGROMETER_DT_ID = 1032;

            public Single temperature = new Single();
            public Single humidity = new Single();
            public uint8_t id = new uint8_t();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
            {
                encode_dronecan_sensors_hygrometer_Hygrometer(this, chunk_cb, ctx);
            }

            public void decode(CanardRxTransfer transfer)
            {
                decode_dronecan_sensors_hygrometer_Hygrometer(transfer, this);
            }

            public static dronecan_sensors_hygrometer_Hygrometer ByteArrayToDroneCANMsg(byte[] transfer, int startoffset)
            {
                var ans = new dronecan_sensors_hygrometer_Hygrometer();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()));
                return ans;
            }
        }
    }
}