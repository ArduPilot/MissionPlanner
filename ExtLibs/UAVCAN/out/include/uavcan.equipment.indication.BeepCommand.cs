
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


public const int UAVCAN_EQUIPMENT_INDICATION_BEEPCOMMAND_MAX_PACK_SIZE = 4;
public const ulong UAVCAN_EQUIPMENT_INDICATION_BEEPCOMMAND_DT_SIG = 0xBE9EA9FEC2B15D52;
public const int UAVCAN_EQUIPMENT_INDICATION_BEEPCOMMAND_DT_ID = 1080;



public class uavcan_equipment_indication_BeepCommand: IUAVCANSerialize {
    public Single frequency = new Single();
    public Single duration = new Single();

public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_uavcan_equipment_indication_BeepCommand(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_uavcan_equipment_indication_BeepCommand(transfer, this);
}

};

}
}