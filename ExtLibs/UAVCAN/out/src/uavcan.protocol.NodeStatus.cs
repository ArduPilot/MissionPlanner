
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

static uavcan_message_descriptor_s uavcan_protocol_NodeStatus_descriptor = {
    UAVCAN_PROTOCOL_NODESTATUS_DT_SIG,
    UAVCAN_PROTOCOL_NODESTATUS_DT_ID,
    CanardTransferTypeBroadcast,
    sizeof(uavcan_protocol_NodeStatus),
    UAVCAN_PROTOCOL_NODESTATUS_MAX_PACK_SIZE,
    encode_func,
    decode_func,
    null
};
*/

static void encode_uavcan_protocol_NodeStatus(uavcan_protocol_NodeStatus msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
    uint8_t[] buffer = new uint8_t[8];
    _encode_uavcan_protocol_NodeStatus(buffer, msg, chunk_cb, ctx, true);
}

static uint32_t decode_uavcan_protocol_NodeStatus(CanardRxTransfer transfer, uavcan_protocol_NodeStatus msg) {
    uint32_t bit_ofs = 0;
    _decode_uavcan_protocol_NodeStatus(transfer, ref bit_ofs, msg, true);
    return (bit_ofs+7)/8;
}

static void _encode_uavcan_protocol_NodeStatus(uint8_t[] buffer, uavcan_protocol_NodeStatus msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {

    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 32, msg.uptime_sec);
    chunk_cb(buffer, 32, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 2, msg.health);
    chunk_cb(buffer, 2, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 3, msg.mode);
    chunk_cb(buffer, 3, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 3, msg.sub_mode);
    chunk_cb(buffer, 3, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 16, msg.vendor_specific_status_code);
    chunk_cb(buffer, 16, ctx);
}

static void _decode_uavcan_protocol_NodeStatus(CanardRxTransfer transfer,ref uint32_t bit_ofs, uavcan_protocol_NodeStatus msg, bool tao) {

    canardDecodeScalar(transfer, bit_ofs, 32, false, ref msg.uptime_sec);
    bit_ofs += 32;

    canardDecodeScalar(transfer, bit_ofs, 2, false, ref msg.health);
    bit_ofs += 2;

    canardDecodeScalar(transfer, bit_ofs, 3, false, ref msg.mode);
    bit_ofs += 3;

    canardDecodeScalar(transfer, bit_ofs, 3, false, ref msg.sub_mode);
    bit_ofs += 3;

    canardDecodeScalar(transfer, bit_ofs, 16, false, ref msg.vendor_specific_status_code);
    bit_ofs += 16;

}

}
}