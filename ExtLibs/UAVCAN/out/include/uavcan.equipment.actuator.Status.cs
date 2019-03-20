
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


public const int UAVCAN_EQUIPMENT_ACTUATOR_STATUS_MAX_PACK_SIZE = 8;
public const ulong UAVCAN_EQUIPMENT_ACTUATOR_STATUS_DT_SIG = 0x5E9BBA44FAF1EA04;
public const int UAVCAN_EQUIPMENT_ACTUATOR_STATUS_DT_ID = 1011;



public const double UAVCAN_EQUIPMENT_ACTUATOR_STATUS_POWER_RATING_PCT_UNKNOWN = 127; // saturated uint7

public class uavcan_equipment_actuator_Status: IUAVCANSerialize {
    public uint8_t actuator_id = new uint8_t();
    public Single position = new Single();
    public Single force = new Single();
    public Single speed = new Single();
    public uint8_t power_rating_pct = new uint8_t();

public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_uavcan_equipment_actuator_Status(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_uavcan_equipment_actuator_Status(transfer, this);
}

};

}
}