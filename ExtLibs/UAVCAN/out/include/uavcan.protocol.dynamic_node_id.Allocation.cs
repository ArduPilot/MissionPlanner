
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


public const int UAVCAN_PROTOCOL_DYNAMIC_NODE_ID_ALLOCATION_MAX_PACK_SIZE = 18;
public const ulong UAVCAN_PROTOCOL_DYNAMIC_NODE_ID_ALLOCATION_DT_SIG = 0xB2A812620A11D40;
public const int UAVCAN_PROTOCOL_DYNAMIC_NODE_ID_ALLOCATION_DT_ID = 1;



public const double UAVCAN_PROTOCOL_DYNAMIC_NODE_ID_ALLOCATION_MAX_REQUEST_PERIOD_MS = 1000; // saturated uint16
public const double UAVCAN_PROTOCOL_DYNAMIC_NODE_ID_ALLOCATION_MIN_REQUEST_PERIOD_MS = 600; // saturated uint16
public const double UAVCAN_PROTOCOL_DYNAMIC_NODE_ID_ALLOCATION_MAX_FOLLOWUP_DELAY_MS = 400; // saturated uint16
public const double UAVCAN_PROTOCOL_DYNAMIC_NODE_ID_ALLOCATION_MIN_FOLLOWUP_DELAY_MS = 0; // saturated uint16
public const double UAVCAN_PROTOCOL_DYNAMIC_NODE_ID_ALLOCATION_FOLLOWUP_TIMEOUT_MS = 500; // saturated uint16
public const double UAVCAN_PROTOCOL_DYNAMIC_NODE_ID_ALLOCATION_MAX_LENGTH_OF_UNIQUE_ID_IN_REQUEST = 6; // saturated uint8
public const double UAVCAN_PROTOCOL_DYNAMIC_NODE_ID_ALLOCATION_ANY_NODE_ID = 0; // saturated uint7

public class uavcan_protocol_dynamic_node_id_Allocation: IUAVCANSerialize {
    public uint8_t node_id = new uint8_t();
    public bool first_part_of_unique_id = new bool();
    public uint8_t unique_id_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=16)] public uint8_t[] unique_id = new uint8_t[16];

public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_uavcan_protocol_dynamic_node_id_Allocation(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_uavcan_protocol_dynamic_node_id_Allocation(transfer, this);
}

};

}
}