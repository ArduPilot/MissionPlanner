

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




public const int UAVCAN_EQUIPMENT_ICE_RECIPROCATING_CYLINDERSTATUS_MAX_PACK_SIZE = 10;
public const ulong UAVCAN_EQUIPMENT_ICE_RECIPROCATING_CYLINDERSTATUS_DT_SIG = 0xD68AC83A89D5B36B;






public class uavcan_equipment_ice_reciprocating_CylinderStatus: IUAVCANSerialize {



    public Single ignition_timing_deg = new Single();



    public Single injection_time_ms = new Single();



    public Single cylinder_head_temperature = new Single();



    public Single exhaust_gas_temperature = new Single();



    public Single lambda_coefficient = new Single();




public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_uavcan_equipment_ice_reciprocating_CylinderStatus(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_uavcan_equipment_ice_reciprocating_CylinderStatus(transfer, this);
}

};

}
}