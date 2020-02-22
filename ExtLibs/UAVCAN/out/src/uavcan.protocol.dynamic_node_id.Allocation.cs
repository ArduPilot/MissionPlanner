
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

static uavcan_message_descriptor_s uavcan_protocol_dynamic_node_id_Allocation_descriptor = {
    UAVCAN_PROTOCOL_DYNAMIC_NODE_ID_ALLOCATION_DT_SIG,
    UAVCAN_PROTOCOL_DYNAMIC_NODE_ID_ALLOCATION_DT_ID,
    CanardTransferTypeBroadcast,
    sizeof(uavcan_protocol_dynamic_node_id_Allocation),
    UAVCAN_PROTOCOL_DYNAMIC_NODE_ID_ALLOCATION_MAX_PACK_SIZE,
    encode_func,
    decode_func,
    null
};
*/

static void encode_uavcan_protocol_dynamic_node_id_Allocation(uavcan_protocol_dynamic_node_id_Allocation msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
    uint8_t[] buffer = new uint8_t[8];
    _encode_uavcan_protocol_dynamic_node_id_Allocation(buffer, msg, chunk_cb, ctx, true);
}

static uint32_t decode_uavcan_protocol_dynamic_node_id_Allocation(CanardRxTransfer transfer, uavcan_protocol_dynamic_node_id_Allocation msg) {
    uint32_t bit_ofs = 0;
    _decode_uavcan_protocol_dynamic_node_id_Allocation(transfer, ref bit_ofs, msg, true);
    return (bit_ofs+7)/8;
}

static void _encode_uavcan_protocol_dynamic_node_id_Allocation(uint8_t[] buffer, uavcan_protocol_dynamic_node_id_Allocation msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {

    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 7, msg.node_id);
    chunk_cb(buffer, 7, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 1, msg.first_part_of_unique_id);
    chunk_cb(buffer, 1, ctx);
    if (!tao) {
        memset(buffer,0,8);
        canardEncodeScalar(buffer, 0, 5, msg.unique_id_len);
        chunk_cb(buffer, 5, ctx);
    }
    for (int i=0; i < msg.unique_id_len; i++) {
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 8, msg.unique_id[i]);
            chunk_cb(buffer, 8, ctx);
    }
}

static void _decode_uavcan_protocol_dynamic_node_id_Allocation(CanardRxTransfer transfer,ref uint32_t bit_ofs, uavcan_protocol_dynamic_node_id_Allocation msg, bool tao) {

    canardDecodeScalar(transfer, bit_ofs, 7, false, ref msg.node_id);
    bit_ofs += 7;

    canardDecodeScalar(transfer, bit_ofs, 1, false, ref msg.first_part_of_unique_id);
    bit_ofs += 1;

    if (!tao) {
        canardDecodeScalar(transfer, bit_ofs, 5, false, ref msg.unique_id_len);
        bit_ofs += 5;
    } else {
        msg.unique_id_len = (uint8_t)(((transfer.payload_len*8)-bit_ofs)/8);
    }

    for (int i=0; i < msg.unique_id_len; i++) {
        canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.unique_id[i]);
        bit_ofs += 8;
    }

}

}
}