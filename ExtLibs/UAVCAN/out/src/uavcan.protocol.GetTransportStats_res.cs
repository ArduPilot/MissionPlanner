
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

static uavcan_message_descriptor_s uavcan_protocol_GetTransportStats_res_descriptor = {
    UAVCAN_PROTOCOL_GETTRANSPORTSTATS_RES_DT_SIG,
    UAVCAN_PROTOCOL_GETTRANSPORTSTATS_RES_DT_ID,
    CanardTransferTypeResponse,
    sizeof(uavcan_protocol_GetTransportStats_res),
    UAVCAN_PROTOCOL_GETTRANSPORTSTATS_RES_MAX_PACK_SIZE,
    encode_func,
    decode_func,
    null
};
*/

static void encode_uavcan_protocol_GetTransportStats_res(uavcan_protocol_GetTransportStats_res msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
    uint8_t[] buffer = new uint8_t[8];
    _encode_uavcan_protocol_GetTransportStats_res(buffer, msg, chunk_cb, ctx, true);
}

static uint32_t decode_uavcan_protocol_GetTransportStats_res(CanardRxTransfer transfer, uavcan_protocol_GetTransportStats_res msg) {
    uint32_t bit_ofs = 0;
    _decode_uavcan_protocol_GetTransportStats_res(transfer, ref bit_ofs, msg, true);
    return (bit_ofs+7)/8;
}

static void _encode_uavcan_protocol_GetTransportStats_res(uint8_t[] buffer, uavcan_protocol_GetTransportStats_res msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {

    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 48, msg.transfers_tx);
    chunk_cb(buffer, 48, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 48, msg.transfers_rx);
    chunk_cb(buffer, 48, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 48, msg.transfer_errors);
    chunk_cb(buffer, 48, ctx);
    if (!tao) {
        memset(buffer,0,8);
        canardEncodeScalar(buffer, 0, 2, msg.can_iface_stats_len);
        chunk_cb(buffer, 2, ctx);
    }
    for (int i=0; i < msg.can_iface_stats_len; i++) {
            _encode_uavcan_protocol_CANIfaceStats(buffer, msg.can_iface_stats[i], chunk_cb, ctx, false);
    }
}

static void _decode_uavcan_protocol_GetTransportStats_res(CanardRxTransfer transfer,ref uint32_t bit_ofs, uavcan_protocol_GetTransportStats_res msg, bool tao) {

    canardDecodeScalar(transfer, bit_ofs, 48, false, ref msg.transfers_tx);
    bit_ofs += 48;

    canardDecodeScalar(transfer, bit_ofs, 48, false, ref msg.transfers_rx);
    bit_ofs += 48;

    canardDecodeScalar(transfer, bit_ofs, 48, false, ref msg.transfer_errors);
    bit_ofs += 48;

    if (!tao) {
        canardDecodeScalar(transfer, bit_ofs, 2, false, ref msg.can_iface_stats_len);
        bit_ofs += 2;
    }


    if (tao) {
msg.can_iface_stats_len = 0;
        while (((transfer.payload_len*8)-bit_ofs) > 0) {
            _decode_uavcan_protocol_CANIfaceStats(transfer, ref bit_ofs, msg.can_iface_stats[msg.can_iface_stats_len], false);
            msg.can_iface_stats_len++;
        }
    } else {
        for (int i=0; i < msg.can_iface_stats_len; i++) {
            _decode_uavcan_protocol_CANIfaceStats(transfer, ref bit_ofs, msg.can_iface_stats[i], false);
        }
    }

}

}
}