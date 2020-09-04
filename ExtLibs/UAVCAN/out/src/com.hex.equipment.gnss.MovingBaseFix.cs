
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

static uavcan_message_descriptor_s com_hex_equipment_gnss_MovingBaseFix_descriptor = {
    COM_HEX_EQUIPMENT_GNSS_MOVINGBASEFIX_DT_SIG,
    COM_HEX_EQUIPMENT_GNSS_MOVINGBASEFIX_DT_ID,
    CanardTransferTypeBroadcast,
    sizeof(com_hex_equipment_gnss_MovingBaseFix),
    COM_HEX_EQUIPMENT_GNSS_MOVINGBASEFIX_MAX_PACK_SIZE,
    encode_func,
    decode_func,
    null
};
*/

static void encode_com_hex_equipment_gnss_MovingBaseFix(com_hex_equipment_gnss_MovingBaseFix msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
    uint8_t[] buffer = new uint8_t[8];
    _encode_com_hex_equipment_gnss_MovingBaseFix(buffer, msg, chunk_cb, ctx, true);
}

static uint32_t decode_com_hex_equipment_gnss_MovingBaseFix(CanardRxTransfer transfer, com_hex_equipment_gnss_MovingBaseFix msg) {
    uint32_t bit_ofs = 0;
    _decode_com_hex_equipment_gnss_MovingBaseFix(transfer, ref bit_ofs, msg, true);
    return (bit_ofs+7)/8;
}

static void _encode_com_hex_equipment_gnss_MovingBaseFix(uint8_t[] buffer, com_hex_equipment_gnss_MovingBaseFix msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {

    _encode_uavcan_Timestamp(buffer, msg.timestamp, chunk_cb, ctx, false);
    for (int i=0; i < 16; i++) {
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 8, msg.base_in_use_hwid[i]);
            chunk_cb(buffer, 8, ctx);
    }
    chunk_cb(null, 6, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 2, msg.carrier_solution_type);
    chunk_cb(buffer, 2, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 2, msg.pos_rel_body_len);
    chunk_cb(buffer, 2, ctx);
    for (int i=0; i < msg.pos_rel_body_len; i++) {
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 32, msg.pos_rel_body[i]);
            chunk_cb(buffer, 32, ctx);
    }
    for (int i=0; i < 3; i++) {
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 32, msg.pos_rel_ecef[i]);
            chunk_cb(buffer, 32, ctx);
    }
    if (!tao) {
        memset(buffer,0,8);
        canardEncodeScalar(buffer, 0, 3, msg.pos_rel_ecef_covariance_len);
        chunk_cb(buffer, 3, ctx);
    }
    for (int i=0; i < msg.pos_rel_ecef_covariance_len; i++) {
            memset(buffer,0,8);
            {
                uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.pos_rel_ecef_covariance[i]);
                canardEncodeScalar(buffer, 0, 16, float16_val);
            }
            chunk_cb(buffer, 16, ctx);
    }
}

static void _decode_com_hex_equipment_gnss_MovingBaseFix(CanardRxTransfer transfer,ref uint32_t bit_ofs, com_hex_equipment_gnss_MovingBaseFix msg, bool tao) {

    _decode_uavcan_Timestamp(transfer, ref bit_ofs, msg.timestamp, false);

/*['__doc__', '__init__', '__module__', '__repr__', '__str__', 'get_normalized_definition', 'name', 'type']*/
    for (int i=0; i < 16; i++) {
        canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.base_in_use_hwid[i]);
        bit_ofs += 8;
    }

    bit_ofs += 6;

    canardDecodeScalar(transfer, bit_ofs, 2, false, ref msg.carrier_solution_type);
    bit_ofs += 2;

    canardDecodeScalar(transfer, bit_ofs, 2, false, ref msg.pos_rel_body_len);
    bit_ofs += 2;
    msg.pos_rel_body = new Single[msg.pos_rel_body_len];
    for (int i=0; i < msg.pos_rel_body_len; i++) {
        canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.pos_rel_body[i]);
        bit_ofs += 32;
    }

/*['__doc__', '__init__', '__module__', '__repr__', '__str__', 'get_normalized_definition', 'name', 'type']*/
    for (int i=0; i < 3; i++) {
        canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.pos_rel_ecef[i]);
        bit_ofs += 32;
    }

    if (!tao) {
        canardDecodeScalar(transfer, bit_ofs, 3, false, ref msg.pos_rel_ecef_covariance_len);
        bit_ofs += 3;
    } else {
        msg.pos_rel_ecef_covariance_len = (uint8_t)(((transfer.payload_len*8)-bit_ofs)/16);
    }

    msg.pos_rel_ecef_covariance = new Single[msg.pos_rel_ecef_covariance_len];
    for (int i=0; i < msg.pos_rel_ecef_covariance_len; i++) {
        {
            uint16_t float16_val = 0;
            canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
            msg.pos_rel_ecef_covariance[i] = canardConvertFloat16ToNativeFloat(float16_val);
        }
        bit_ofs += 16;
    }

}

}
}