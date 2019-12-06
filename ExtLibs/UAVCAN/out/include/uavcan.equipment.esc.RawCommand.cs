

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




public const int UAVCAN_EQUIPMENT_ESC_RAWCOMMAND_MAX_PACK_SIZE = 36;
public const ulong UAVCAN_EQUIPMENT_ESC_RAWCOMMAND_DT_SIG = 0x217F5C87D7EC951D;

public const int UAVCAN_EQUIPMENT_ESC_RAWCOMMAND_DT_ID = 1030;






public class uavcan_equipment_esc_RawCommand: IUAVCANSerialize {



    public uint8_t cmd_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=20)] public int16_t[] cmd = new int16_t[20];




public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_uavcan_equipment_esc_RawCommand(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_uavcan_equipment_esc_RawCommand(transfer, this);
}

};

}
}