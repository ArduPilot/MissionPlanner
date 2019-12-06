

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




public const int UAVCAN_EQUIPMENT_ESC_STATUS_MAX_PACK_SIZE = 14;
public const ulong UAVCAN_EQUIPMENT_ESC_STATUS_DT_SIG = 0xA9AF28AEA2FBB254;

public const int UAVCAN_EQUIPMENT_ESC_STATUS_DT_ID = 1034;






public class uavcan_equipment_esc_Status: IUAVCANSerialize {



    public uint32_t error_count = new uint32_t();



    public Single voltage = new Single();



    public Single current = new Single();



    public Single temperature = new Single();



    public int32_t rpm = new int32_t();



    public uint8_t power_rating_pct = new uint8_t();



    public uint8_t esc_index = new uint8_t();




public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_uavcan_equipment_esc_Status(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_uavcan_equipment_esc_Status(transfer, this);
}

};

}
}