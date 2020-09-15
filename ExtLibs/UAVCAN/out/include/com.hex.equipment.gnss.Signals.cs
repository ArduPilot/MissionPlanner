
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

namespace UAVCAN
{
public partial class uavcan {

//using com.hex.equipment.gnss.Signal.cs
//using uavcan.Timestamp.cs

public const int COM_HEX_EQUIPMENT_GNSS_SIGNALS_MAX_PACK_SIZE = 987;
public const ulong COM_HEX_EQUIPMENT_GNSS_SIGNALS_DT_SIG = 0xE448A43008E96FA0;
public const int COM_HEX_EQUIPMENT_GNSS_SIGNALS_DT_ID = 20212;



public class com_hex_equipment_gnss_Signals: IUAVCANSerialize {
    public uavcan_Timestamp timestamp = new uavcan_Timestamp();
    public uint32_t iTOW = new uint32_t();
    public uint8_t signals_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=120)] public com_hex_equipment_gnss_Signal[] signals = Enumerable.Range(1, 120).Select(i => new com_hex_equipment_gnss_Signal()).ToArray();

public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_com_hex_equipment_gnss_Signals(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_com_hex_equipment_gnss_Signals(transfer, this);
}

};

}
}