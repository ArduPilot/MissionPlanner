
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



static void encode_uavcan_protocol_dynamic_node_id_server_Entry(uavcan_protocol_dynamic_node_id_server_Entry msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
    uint8_t[] buffer = new uint8_t[8];
    _encode_uavcan_protocol_dynamic_node_id_server_Entry(buffer, msg, chunk_cb, ctx, true);
}

static uint32_t decode_uavcan_protocol_dynamic_node_id_server_Entry(CanardRxTransfer transfer, uavcan_protocol_dynamic_node_id_server_Entry msg) {
    uint32_t bit_ofs = 0;
    _decode_uavcan_protocol_dynamic_node_id_server_Entry(transfer, ref bit_ofs, msg, true);
    return (bit_ofs+7)/8;
}

static void _encode_uavcan_protocol_dynamic_node_id_server_Entry(uint8_t[] buffer, uavcan_protocol_dynamic_node_id_server_Entry msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {

    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 32, msg.term);
    chunk_cb(buffer, 32, ctx);
    for (int i=0; i < 16; i++) {
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 8, msg.unique_id[i]);
            chunk_cb(buffer, 8, ctx);
    }
    chunk_cb(null, 1, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 7, msg.node_id);
    chunk_cb(buffer, 7, ctx);
}

static void _decode_uavcan_protocol_dynamic_node_id_server_Entry(CanardRxTransfer transfer,ref uint32_t bit_ofs, uavcan_protocol_dynamic_node_id_server_Entry msg, bool tao) {

    canardDecodeScalar(transfer, bit_ofs, 32, false, ref msg.term);
    bit_ofs += 32;

/*['__doc__', '__init__', '__module__', '__repr__', '__str__', 'get_normalized_definition', 'name', 'type']*/
    for (int i=0; i < 16; i++) {
        canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.unique_id[i]);
        bit_ofs += 8;
    }

    bit_ofs += 1;

    canardDecodeScalar(transfer, bit_ofs, 7, false, ref msg.node_id);
    bit_ofs += 7;

}

}
}