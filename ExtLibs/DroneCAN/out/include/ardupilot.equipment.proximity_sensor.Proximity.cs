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
        public partial class ardupilot_equipment_proximity_sensor_Proximity: IDroneCANSerialize 
        {
            public const int ARDUPILOT_EQUIPMENT_PROXIMITY_SENSOR_PROXIMITY_MAX_PACK_SIZE = 8;
            public const ulong ARDUPILOT_EQUIPMENT_PROXIMITY_SENSOR_PROXIMITY_DT_SIG = 0x99DD3985FB3222CE;
            public const int ARDUPILOT_EQUIPMENT_PROXIMITY_SENSOR_PROXIMITY_DT_ID = 21910;

            public const double ARDUPILOT_EQUIPMENT_PROXIMITY_SENSOR_PROXIMITY_READING_TYPE_NO_DATA = 0; // saturated uint3
            public const double ARDUPILOT_EQUIPMENT_PROXIMITY_SENSOR_PROXIMITY_READING_TYPE_NOT_CONNECTED = 1; // saturated uint3
            public const double ARDUPILOT_EQUIPMENT_PROXIMITY_SENSOR_PROXIMITY_READING_TYPE_GOOD = 2; // saturated uint3
            public const double ARDUPILOT_EQUIPMENT_PROXIMITY_SENSOR_PROXIMITY_FLAGS_NONE = 0; // saturated uint5

            public uint8_t sensor_id = new uint8_t();
            public uint8_t reading_type = new uint8_t();
            public uint8_t flags = new uint8_t();
            public Single yaw = new Single();
            public Single pitch = new Single();
            public Single distance = new Single();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan = false)
            {
                encode_ardupilot_equipment_proximity_sensor_Proximity(this, chunk_cb, ctx, fdcan);
            }

            public void decode(CanardRxTransfer transfer, bool fdcan = false)
            {
                decode_ardupilot_equipment_proximity_sensor_Proximity(transfer, this, fdcan);
            }

            public static ardupilot_equipment_proximity_sensor_Proximity ByteArrayToDroneCANMsg(byte[] transfer, int startoffset, bool fdcan = false)
            {
                var ans = new ardupilot_equipment_proximity_sensor_Proximity();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()), fdcan);
                return ans;
            }
        }
    }
}