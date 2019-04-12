

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




public const int UAVCAN_PROTOCOL_DYNAMIC_NODE_ID_SERVER_DISCOVERY_MAX_PACK_SIZE = 7;
public const ulong UAVCAN_PROTOCOL_DYNAMIC_NODE_ID_SERVER_DISCOVERY_DT_SIG = 0x821AE2F525F69F21;

public const int UAVCAN_PROTOCOL_DYNAMIC_NODE_ID_SERVER_DISCOVERY_DT_ID = 390;





public const double UAVCAN_PROTOCOL_DYNAMIC_NODE_ID_SERVER_DISCOVERY_BROADCASTING_PERIOD_MS = 1000; // saturated uint16




public class uavcan_protocol_dynamic_node_id_server_Discovery: IUAVCANSerialize {



    public uint8_t configured_cluster_size = new uint8_t();



    public uint8_t known_nodes_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=5)] public uint8_t[] known_nodes = new uint8_t[5];




public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_uavcan_protocol_dynamic_node_id_server_Discovery(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_uavcan_protocol_dynamic_node_id_server_Discovery(transfer, this);
}

};

}
}