
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



static void encode_org_cubepilot_uwb_Node(org_cubepilot_uwb_Node msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
    uint8_t[] buffer = new uint8_t[8];
    _encode_org_cubepilot_uwb_Node(buffer, msg, chunk_cb, ctx, true);
}

static uint32_t decode_org_cubepilot_uwb_Node(CanardRxTransfer transfer, org_cubepilot_uwb_Node msg) {
    uint32_t bit_ofs = 0;
    _decode_org_cubepilot_uwb_Node(transfer, ref bit_ofs, msg, true);
    return (bit_ofs+7)/8;
}

static void _encode_org_cubepilot_uwb_Node(uint8_t[] buffer, org_cubepilot_uwb_Node msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {

    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 39, msg.node_id);
    chunk_cb(buffer, 39, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 1, msg.is_tag);
    chunk_cb(buffer, 1, ctx);
    for (int i=0; i < 3; i++) {
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 32, msg.pos[i]);
            chunk_cb(buffer, 32, ctx);
    }
}

static void _decode_org_cubepilot_uwb_Node(CanardRxTransfer transfer,ref uint32_t bit_ofs, org_cubepilot_uwb_Node msg, bool tao) {

    canardDecodeScalar(transfer, bit_ofs, 39, false, ref msg.node_id);
    bit_ofs += 39;

    canardDecodeScalar(transfer, bit_ofs, 1, false, ref msg.is_tag);
    bit_ofs += 1;

/*['__doc__', '__init__', '__module__', '__repr__', '__str__', 'get_normalized_definition', 'name', 'type']*/
    for (int i=0; i < 3; i++) {
        canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.pos[i]);
        bit_ofs += 32;
    }

}

}
}