
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

static uavcan_message_descriptor_s uavcan_equipment_range_sensor_Measurement_descriptor = {
    UAVCAN_EQUIPMENT_RANGE_SENSOR_MEASUREMENT_DT_SIG,
    UAVCAN_EQUIPMENT_RANGE_SENSOR_MEASUREMENT_DT_ID,
    CanardTransferTypeBroadcast,
    sizeof(uavcan_equipment_range_sensor_Measurement),
    UAVCAN_EQUIPMENT_RANGE_SENSOR_MEASUREMENT_MAX_PACK_SIZE,
    encode_func,
    decode_func,
    null
};
*/

static void encode_uavcan_equipment_range_sensor_Measurement(uavcan_equipment_range_sensor_Measurement msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
    uint8_t[] buffer = new uint8_t[8];
    _encode_uavcan_equipment_range_sensor_Measurement(buffer, msg, chunk_cb, ctx, true);
}

static uint32_t decode_uavcan_equipment_range_sensor_Measurement(CanardRxTransfer transfer, uavcan_equipment_range_sensor_Measurement msg) {
    uint32_t bit_ofs = 0;
    _decode_uavcan_equipment_range_sensor_Measurement(transfer, ref bit_ofs, msg, true);
    return (bit_ofs+7)/8;
}

static void _encode_uavcan_equipment_range_sensor_Measurement(uint8_t[] buffer, uavcan_equipment_range_sensor_Measurement msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {

    _encode_uavcan_Timestamp(buffer, msg.timestamp, chunk_cb, ctx, false);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 8, msg.sensor_id);
    chunk_cb(buffer, 8, ctx);
    _encode_uavcan_CoarseOrientation(buffer, msg.beam_orientation_in_body_frame, chunk_cb, ctx, false);
    memset(buffer,0,8);
    {
        uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.field_of_view);
        canardEncodeScalar(buffer, 0, 16, float16_val);
    }
    chunk_cb(buffer, 16, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 5, msg.sensor_type);
    chunk_cb(buffer, 5, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 3, msg.reading_type);
    chunk_cb(buffer, 3, ctx);
    memset(buffer,0,8);
    {
        uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.range);
        canardEncodeScalar(buffer, 0, 16, float16_val);
    }
    chunk_cb(buffer, 16, ctx);
}

static void _decode_uavcan_equipment_range_sensor_Measurement(CanardRxTransfer transfer,ref uint32_t bit_ofs, uavcan_equipment_range_sensor_Measurement msg, bool tao) {

    _decode_uavcan_Timestamp(transfer, ref bit_ofs, msg.timestamp, false);

    canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.sensor_id);
    bit_ofs += 8;

    _decode_uavcan_CoarseOrientation(transfer, ref bit_ofs, msg.beam_orientation_in_body_frame, false);

    {
        uint16_t float16_val = 0;
        canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
        msg.field_of_view = canardConvertFloat16ToNativeFloat(float16_val);
    }
    bit_ofs += 16;

    canardDecodeScalar(transfer, bit_ofs, 5, false, ref msg.sensor_type);
    bit_ofs += 5;

    canardDecodeScalar(transfer, bit_ofs, 3, false, ref msg.reading_type);
    bit_ofs += 3;

    {
        uint16_t float16_val = 0;
        canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
        msg.range = canardConvertFloat16ToNativeFloat(float16_val);
    }
    bit_ofs += 16;

}

}
}