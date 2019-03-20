
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

static uavcan_message_descriptor_s uavcan_equipment_esc_Status_descriptor = {
    UAVCAN_EQUIPMENT_ESC_STATUS_DT_SIG,
    UAVCAN_EQUIPMENT_ESC_STATUS_DT_ID,
    CanardTransferTypeBroadcast,
    sizeof(uavcan_equipment_esc_Status),
    UAVCAN_EQUIPMENT_ESC_STATUS_MAX_PACK_SIZE,
    encode_func,
    decode_func,
    null
};
*/

static void encode_uavcan_equipment_esc_Status(uavcan_equipment_esc_Status msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
    uint8_t[] buffer = new uint8_t[8];
    _encode_uavcan_equipment_esc_Status(buffer, msg, chunk_cb, ctx, true);
}

static uint32_t decode_uavcan_equipment_esc_Status(CanardRxTransfer transfer, uavcan_equipment_esc_Status msg) {
    uint32_t bit_ofs = 0;
    _decode_uavcan_equipment_esc_Status(transfer, ref bit_ofs, msg, true);
    return (bit_ofs+7)/8;
}

static void _encode_uavcan_equipment_esc_Status(uint8_t[] buffer, uavcan_equipment_esc_Status msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {

    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 32, msg.error_count);
    chunk_cb(buffer, 32, ctx);
    memset(buffer,0,8);
    {
        uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.voltage);
        canardEncodeScalar(buffer, 0, 16, float16_val);
    }
    chunk_cb(buffer, 16, ctx);
    memset(buffer,0,8);
    {
        uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.current);
        canardEncodeScalar(buffer, 0, 16, float16_val);
    }
    chunk_cb(buffer, 16, ctx);
    memset(buffer,0,8);
    {
        uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.temperature);
        canardEncodeScalar(buffer, 0, 16, float16_val);
    }
    chunk_cb(buffer, 16, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 18, msg.rpm);
    chunk_cb(buffer, 18, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 7, msg.power_rating_pct);
    chunk_cb(buffer, 7, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 5, msg.esc_index);
    chunk_cb(buffer, 5, ctx);
}

static void _decode_uavcan_equipment_esc_Status(CanardRxTransfer transfer,ref uint32_t bit_ofs, uavcan_equipment_esc_Status msg, bool tao) {

    canardDecodeScalar(transfer, bit_ofs, 32, false, ref msg.error_count);
    bit_ofs += 32;

    {
        uint16_t float16_val = 0;
        canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
        msg.voltage = canardConvertFloat16ToNativeFloat(float16_val);
    }
    bit_ofs += 16;

    {
        uint16_t float16_val = 0;
        canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
        msg.current = canardConvertFloat16ToNativeFloat(float16_val);
    }
    bit_ofs += 16;

    {
        uint16_t float16_val = 0;
        canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
        msg.temperature = canardConvertFloat16ToNativeFloat(float16_val);
    }
    bit_ofs += 16;

    canardDecodeScalar(transfer, bit_ofs, 18, true, ref msg.rpm);
    bit_ofs += 18;

    canardDecodeScalar(transfer, bit_ofs, 7, false, ref msg.power_rating_pct);
    bit_ofs += 7;

    canardDecodeScalar(transfer, bit_ofs, 5, false, ref msg.esc_index);
    bit_ofs += 5;

}

}
}