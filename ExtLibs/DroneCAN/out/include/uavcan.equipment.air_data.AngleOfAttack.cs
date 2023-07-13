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
        public partial class uavcan_equipment_air_data_AngleOfAttack: IDroneCANSerialize 
        {
            public const int UAVCAN_EQUIPMENT_AIR_DATA_ANGLEOFATTACK_MAX_PACK_SIZE = 5;
            public const ulong UAVCAN_EQUIPMENT_AIR_DATA_ANGLEOFATTACK_DT_SIG = 0xD5513C3F7AFAC74E;
            public const int UAVCAN_EQUIPMENT_AIR_DATA_ANGLEOFATTACK_DT_ID = 1025;

            public const double UAVCAN_EQUIPMENT_AIR_DATA_ANGLEOFATTACK_SENSOR_ID_LEFT = 254; // saturated uint8
            public const double UAVCAN_EQUIPMENT_AIR_DATA_ANGLEOFATTACK_SENSOR_ID_RIGHT = 255; // saturated uint8

            public uint8_t sensor_id = new uint8_t();
            public Single aoa = new Single();
            public Single aoa_variance = new Single();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan = false)
            {
                encode_uavcan_equipment_air_data_AngleOfAttack(this, chunk_cb, ctx, fdcan);
            }

            public void decode(CanardRxTransfer transfer, bool fdcan = false)
            {
                decode_uavcan_equipment_air_data_AngleOfAttack(transfer, this, fdcan);
            }

            public static uavcan_equipment_air_data_AngleOfAttack ByteArrayToDroneCANMsg(byte[] transfer, int startoffset, bool fdcan = false)
            {
                var ans = new uavcan_equipment_air_data_AngleOfAttack();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()), fdcan);
                return ans;
            }
        }
    }
}