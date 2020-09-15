
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


public const int UAVCAN_OLLIW_STORM32_STATUS_MAX_PACK_SIZE = 36;
public const ulong UAVCAN_OLLIW_STORM32_STATUS_DT_SIG = 0xFD38D6AA0F5099A5;
public const int UAVCAN_OLLIW_STORM32_STATUS_DT_ID = 28301;



public class uavcan_olliw_storm32_Status: IUAVCANSerialize {
    public uint8_t gimbal_id = new uint8_t();
    public uint8_t mode = new uint8_t();
    public uint32_t status = new uint32_t();
    public uint8_t orientation_type = new uint8_t();
    [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)] public Single[] orientation = new Single[4];
    public uint8_t angular_velocity_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=3)] public Single[] angular_velocity = Enumerable.Range(1, 3).Select(i => new Single()).ToArray();

public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_uavcan_olliw_storm32_Status(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_uavcan_olliw_storm32_Status(transfer, this);
}

};

}
}