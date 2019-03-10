

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




public const int UAVCAN_PROTOCOL_DYNAMIC_NODE_ID_SERVER_REQUESTVOTE_RES_MAX_PACK_SIZE = 5;
public const ulong UAVCAN_PROTOCOL_DYNAMIC_NODE_ID_SERVER_REQUESTVOTE_RES_DT_SIG = 0xCDDE07BB89A56356;

public const int UAVCAN_PROTOCOL_DYNAMIC_NODE_ID_SERVER_REQUESTVOTE_RES_DT_ID = 31;






public class uavcan_protocol_dynamic_node_id_server_RequestVote_res: IUAVCANSerialize {



    public uint32_t term = new uint32_t();



    public bool vote_granted = new bool();




public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_uavcan_protocol_dynamic_node_id_server_RequestVote_res(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_uavcan_protocol_dynamic_node_id_server_RequestVote_res(transfer, this);
}

};

}
}