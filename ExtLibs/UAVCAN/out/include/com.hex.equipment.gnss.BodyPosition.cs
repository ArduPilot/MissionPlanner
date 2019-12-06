

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




public const int COM_HEX_EQUIPMENT_GNSS_BODYPOSITION_MAX_PACK_SIZE = 7;
public const ulong COM_HEX_EQUIPMENT_GNSS_BODYPOSITION_DT_SIG = 0x68DD4C23FEC97050;

public const int COM_HEX_EQUIPMENT_GNSS_BODYPOSITION_DT_ID = 20210;






public class com_hex_equipment_gnss_BodyPosition: IUAVCANSerialize {



    [MarshalAs(UnmanagedType.ByValArray,SizeConst=3)] public int32_t[] body_pos_mm = new int32_t[3];




public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_com_hex_equipment_gnss_BodyPosition(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_com_hex_equipment_gnss_BodyPosition(transfer, this);
}

};

}
}