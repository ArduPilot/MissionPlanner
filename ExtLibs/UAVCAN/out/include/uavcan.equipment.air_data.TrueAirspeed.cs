

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
using System.Runtime.InteropServices;

namespace UAVCAN
{
public partial class uavcan {




public const int UAVCAN_EQUIPMENT_AIR_DATA_TRUEAIRSPEED_MAX_PACK_SIZE = 4;
public const ulong UAVCAN_EQUIPMENT_AIR_DATA_TRUEAIRSPEED_DT_SIG = 0x306F69E0A591AFAA;

public const int UAVCAN_EQUIPMENT_AIR_DATA_TRUEAIRSPEED_DT_ID = 1020;






public class uavcan_equipment_air_data_TrueAirspeed: IUAVCANSerialize {



    public Single true_airspeed = new Single();



    public Single true_airspeed_variance = new Single();




public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_uavcan_equipment_air_data_TrueAirspeed(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_uavcan_equipment_air_data_TrueAirspeed(transfer, this);
}

};

}
}