
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
        public partial class uavcan_equipment_air_data_IndicatedAirspeed: IDroneCANSerialize 
        {
            public const int UAVCAN_EQUIPMENT_AIR_DATA_INDICATEDAIRSPEED_MAX_PACK_SIZE = 4;
            public const ulong UAVCAN_EQUIPMENT_AIR_DATA_INDICATEDAIRSPEED_DT_SIG = 0xA1892D72AB8945F;
            public const int UAVCAN_EQUIPMENT_AIR_DATA_INDICATEDAIRSPEED_DT_ID = 1021;

            public Single indicated_airspeed = new Single();
            public Single indicated_airspeed_variance = new Single();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
            {
                encode_uavcan_equipment_air_data_IndicatedAirspeed(this, chunk_cb, ctx);
            }

            public void decode(CanardRxTransfer transfer)
            {
                decode_uavcan_equipment_air_data_IndicatedAirspeed(transfer, this);
            }

            public static uavcan_equipment_air_data_IndicatedAirspeed ByteArrayToDroneCANMsg(byte[] transfer, int startoffset)
            {
                var ans = new uavcan_equipment_air_data_IndicatedAirspeed();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()));
                return ans;
            }
        }
    }
}