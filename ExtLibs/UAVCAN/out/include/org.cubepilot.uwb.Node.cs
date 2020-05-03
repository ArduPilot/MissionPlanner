
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


public const int ORG_CUBEPILOT_UWB_NODE_MAX_PACK_SIZE = 17;
public const ulong ORG_CUBEPILOT_UWB_NODE_DT_SIG = 0xCF728D1C28AB0C3B;



public class org_cubepilot_uwb_Node: IUAVCANSerialize {
    public uint64_t node_id = new uint64_t();
    public bool is_tag = new bool();
    [MarshalAs(UnmanagedType.ByValArray,SizeConst=3)] public Single[] pos = new Single[3];

public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_org_cubepilot_uwb_Node(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_org_cubepilot_uwb_Node(transfer, this);
}

};

}
}