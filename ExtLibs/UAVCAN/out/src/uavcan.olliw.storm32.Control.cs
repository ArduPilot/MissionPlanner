
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

static uavcan_message_descriptor_s uavcan_olliw_storm32_Control_descriptor = {
    UAVCAN_OLLIW_STORM32_CONTROL_DT_SIG,
    UAVCAN_OLLIW_STORM32_CONTROL_DT_ID,
    CanardTransferTypeBroadcast,
    sizeof(uavcan_olliw_storm32_Control),
    UAVCAN_OLLIW_STORM32_CONTROL_MAX_PACK_SIZE,
    encode_func,
    decode_func,
    null
};
*/

static void encode_uavcan_olliw_storm32_Control(uavcan_olliw_storm32_Control msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
    uint8_t[] buffer = new uint8_t[8];
    _encode_uavcan_olliw_storm32_Control(buffer, msg, chunk_cb, ctx, true);
}

static uint32_t decode_uavcan_olliw_storm32_Control(CanardRxTransfer transfer, uavcan_olliw_storm32_Control msg) {
    uint32_t bit_ofs = 0;
    _decode_uavcan_olliw_storm32_Control(transfer, ref bit_ofs, msg, true);
    return (bit_ofs+7)/8;
}

static void _encode_uavcan_olliw_storm32_Control(uint8_t[] buffer, uavcan_olliw_storm32_Control msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {

    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 8, msg.gimbal_id);
    chunk_cb(buffer, 8, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 8, msg.mode);
    chunk_cb(buffer, 8, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 8, msg.control_mode);
    chunk_cb(buffer, 8, ctx);
    for (int i=0; i < 4; i++) {
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 32, msg.orientation[i]);
            chunk_cb(buffer, 32, ctx);
    }
    if (!tao) {
        memset(buffer,0,8);
        canardEncodeScalar(buffer, 0, 2, msg.angular_velocity_len);
        chunk_cb(buffer, 2, ctx);
    }
    for (int i=0; i < msg.angular_velocity_len; i++) {
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 32, msg.angular_velocity[i]);
            chunk_cb(buffer, 32, ctx);
    }
}

static void _decode_uavcan_olliw_storm32_Control(CanardRxTransfer transfer,ref uint32_t bit_ofs, uavcan_olliw_storm32_Control msg, bool tao) {

    canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.gimbal_id);
    bit_ofs += 8;

    canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.mode);
    bit_ofs += 8;

    canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.control_mode);
    bit_ofs += 8;

/*['__doc__', '__init__', '__module__', '__repr__', '__str__', 'get_normalized_definition', 'name', 'type']*/
    for (int i=0; i < 4; i++) {
        canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.orientation[i]);
        bit_ofs += 32;
    }

    if (!tao) {
        canardDecodeScalar(transfer, bit_ofs, 2, false, ref msg.angular_velocity_len);
        bit_ofs += 2;
    } else {
        msg.angular_velocity_len = (uint8_t)(((transfer.payload_len*8)-bit_ofs)/32);
    }

    msg.angular_velocity = new Single[msg.angular_velocity_len];
    for (int i=0; i < msg.angular_velocity_len; i++) {
        canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.angular_velocity[i]);
        bit_ofs += 32;
    }

}

}
}