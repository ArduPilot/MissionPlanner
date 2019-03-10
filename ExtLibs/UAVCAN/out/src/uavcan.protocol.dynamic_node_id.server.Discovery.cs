


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




/*

static uavcan_message_descriptor_s uavcan_protocol_dynamic_node_id_server_Discovery_descriptor = {
    UAVCAN_PROTOCOL_DYNAMIC_NODE_ID_SERVER_DISCOVERY_DT_SIG,
    UAVCAN_PROTOCOL_DYNAMIC_NODE_ID_SERVER_DISCOVERY_DT_ID,

    CanardTransferTypeBroadcast,

    sizeof(uavcan_protocol_dynamic_node_id_server_Discovery),
    UAVCAN_PROTOCOL_DYNAMIC_NODE_ID_SERVER_DISCOVERY_MAX_PACK_SIZE,
    encode_func,
    decode_func,

    null

};
*/


static void encode_uavcan_protocol_dynamic_node_id_server_Discovery(uavcan_protocol_dynamic_node_id_server_Discovery msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
    uint8_t[] buffer = new uint8_t[8];
    _encode_uavcan_protocol_dynamic_node_id_server_Discovery(buffer, msg, chunk_cb, ctx, true);
}

static uint32_t decode_uavcan_protocol_dynamic_node_id_server_Discovery(CanardRxTransfer transfer, uavcan_protocol_dynamic_node_id_server_Discovery msg) {
    uint32_t bit_ofs = 0;
    _decode_uavcan_protocol_dynamic_node_id_server_Discovery(transfer, ref bit_ofs, msg, true);
    return (bit_ofs+7)/8;
}

static void _encode_uavcan_protocol_dynamic_node_id_server_Discovery(uint8_t[] buffer, uavcan_protocol_dynamic_node_id_server_Discovery msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {






    memset(buffer,0,8);

    canardEncodeScalar(buffer, 0, 8, msg.configured_cluster_size);

    chunk_cb(buffer, 8, ctx);







    if (!tao) {


        memset(buffer,0,8);
        canardEncodeScalar(buffer, 0, 3, msg.known_nodes_len);
        chunk_cb(buffer, 3, ctx);


    }

    for (int i=0; i < msg.known_nodes_len; i++) {



            memset(buffer,0,8);

            canardEncodeScalar(buffer, 0, 8, msg.known_nodes[i]);

            chunk_cb(buffer, 8, ctx);


    }





}

static void _decode_uavcan_protocol_dynamic_node_id_server_Discovery(CanardRxTransfer transfer,ref uint32_t bit_ofs, uavcan_protocol_dynamic_node_id_server_Discovery msg, bool tao) {








    canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.configured_cluster_size);


    bit_ofs += 8;








    if (!tao) {


        canardDecodeScalar(transfer, bit_ofs, 3, false, ref msg.known_nodes_len);
        bit_ofs += 3;



    } else {

        msg.known_nodes_len = (uint8_t)(((transfer.payload_len*8)-bit_ofs)/8);


    }



    for (int i=0; i < msg.known_nodes_len; i++) {




        canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.known_nodes[i]);

        bit_ofs += 8;


    }






}

}
}