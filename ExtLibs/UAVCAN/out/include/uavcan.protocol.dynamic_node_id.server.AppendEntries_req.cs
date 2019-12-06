

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



//using uavcan.protocol.dynamic_node_id.server.Entry.cs


public const int UAVCAN_PROTOCOL_DYNAMIC_NODE_ID_SERVER_APPENDENTRIES_REQ_MAX_PACK_SIZE = 32;
public const ulong UAVCAN_PROTOCOL_DYNAMIC_NODE_ID_SERVER_APPENDENTRIES_REQ_DT_SIG = 0x8032C7097B48A3CC;

public const int UAVCAN_PROTOCOL_DYNAMIC_NODE_ID_SERVER_APPENDENTRIES_REQ_DT_ID = 30;





public const double UAVCAN_PROTOCOL_DYNAMIC_NODE_ID_SERVER_APPENDENTRIES_REQ_DEFAULT_MIN_ELECTION_TIMEOUT_MS = 2000; // saturated uint16

public const double UAVCAN_PROTOCOL_DYNAMIC_NODE_ID_SERVER_APPENDENTRIES_REQ_DEFAULT_MAX_ELECTION_TIMEOUT_MS = 4000; // saturated uint16




public class uavcan_protocol_dynamic_node_id_server_AppendEntries_req: IUAVCANSerialize {



    public uint32_t term = new uint32_t();



    public uint32_t prev_log_term = new uint32_t();



    public uint8_t prev_log_index = new uint8_t();



    public uint8_t leader_commit = new uint8_t();



    public uint8_t entries_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=1)] public uavcan_protocol_dynamic_node_id_server_Entry[] entries = new uavcan_protocol_dynamic_node_id_server_Entry[1];




public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_uavcan_protocol_dynamic_node_id_server_AppendEntries_req(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_uavcan_protocol_dynamic_node_id_server_AppendEntries_req(transfer, this);
}

};

}
}