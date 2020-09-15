
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


public const int UAVCAN_OLLIW_STORM32_COMMAND_MAX_PACK_SIZE = 130;
public const ulong UAVCAN_OLLIW_STORM32_COMMAND_DT_SIG = 0x49874D32ADB9DA75;
public const int UAVCAN_OLLIW_STORM32_COMMAND_DT_ID = 28302;



public class uavcan_olliw_storm32_Command: IUAVCANSerialize {
    public uint8_t gimbal_id = new uint8_t();
    public uint8_t payload_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=128)] public uint8_t[] payload = Enumerable.Range(1, 128).Select(i => new uint8_t()).ToArray();

public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_uavcan_olliw_storm32_Command(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_uavcan_olliw_storm32_Command(transfer, this);
}

};

}
}