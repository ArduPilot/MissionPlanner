
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


public const int UAVCAN_OLLIW_STORM32_CONTROL_MAX_PACK_SIZE = 32;
public const ulong UAVCAN_OLLIW_STORM32_CONTROL_DT_SIG = 0xBF15FB6305CE5599;
public const int UAVCAN_OLLIW_STORM32_CONTROL_DT_ID = 28300;



public class uavcan_olliw_storm32_Control: IUAVCANSerialize {
    public uint8_t gimbal_id = new uint8_t();
    public uint8_t mode = new uint8_t();
    public uint8_t control_mode = new uint8_t();
    [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)] public Single[] orientation = new Single[4];
    public uint8_t angular_velocity_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=3)] public Single[] angular_velocity = new Single[3];

public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_uavcan_olliw_storm32_Control(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_uavcan_olliw_storm32_Control(transfer, this);
}

};

}
}