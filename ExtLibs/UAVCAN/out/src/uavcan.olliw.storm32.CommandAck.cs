
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
using System.Linq;
using System.Runtime.InteropServices;

namespace UAVCAN
{
public partial class uavcan {



/*

static uavcan_message_descriptor_s uavcan_olliw_storm32_CommandAck_descriptor = {
    UAVCAN_OLLIW_STORM32_COMMANDACK_DT_SIG,
    UAVCAN_OLLIW_STORM32_COMMANDACK_DT_ID,
    CanardTransferTypeBroadcast,
    sizeof(uavcan_olliw_storm32_CommandAck),
    UAVCAN_OLLIW_STORM32_COMMANDACK_MAX_PACK_SIZE,
    encode_func,
    decode_func,
    null
};
*/

static void encode_uavcan_olliw_storm32_CommandAck(uavcan_olliw_storm32_CommandAck msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
    uint8_t[] buffer = new uint8_t[8];
    _encode_uavcan_olliw_storm32_CommandAck(buffer, msg, chunk_cb, ctx, true);
}

static uint32_t decode_uavcan_olliw_storm32_CommandAck(CanardRxTransfer transfer, uavcan_olliw_storm32_CommandAck msg) {
    uint32_t bit_ofs = 0;
    _decode_uavcan_olliw_storm32_CommandAck(transfer, ref bit_ofs, msg, true);
    return (bit_ofs+7)/8;
}

static void _encode_uavcan_olliw_storm32_CommandAck(uint8_t[] buffer, uavcan_olliw_storm32_CommandAck msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {

    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 8, msg.gimbal_id);
    chunk_cb(buffer, 8, ctx);
    if (!tao) {
        memset(buffer,0,8);
        canardEncodeScalar(buffer, 0, 8, msg.payload_len);
        chunk_cb(buffer, 8, ctx);
    }
    for (int i=0; i < msg.payload_len; i++) {
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 8, msg.payload[i]);
            chunk_cb(buffer, 8, ctx);
    }
}

static void _decode_uavcan_olliw_storm32_CommandAck(CanardRxTransfer transfer,ref uint32_t bit_ofs, uavcan_olliw_storm32_CommandAck msg, bool tao) {

    canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.gimbal_id);
    bit_ofs += 8;

    if (!tao) {
        canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.payload_len);
        bit_ofs += 8;
    } else {
        msg.payload_len = (uint8_t)(((transfer.payload_len*8)-bit_ofs)/8);
    }

    msg.payload = new uint8_t[msg.payload_len];
    for (int i=0; i < msg.payload_len; i++) {
        canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.payload[i]);
        bit_ofs += 8;
    }

}

}
}