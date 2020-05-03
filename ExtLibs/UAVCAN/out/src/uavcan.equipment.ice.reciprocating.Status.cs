
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

static uavcan_message_descriptor_s uavcan_equipment_ice_reciprocating_Status_descriptor = {
    UAVCAN_EQUIPMENT_ICE_RECIPROCATING_STATUS_DT_SIG,
    UAVCAN_EQUIPMENT_ICE_RECIPROCATING_STATUS_DT_ID,
    CanardTransferTypeBroadcast,
    sizeof(uavcan_equipment_ice_reciprocating_Status),
    UAVCAN_EQUIPMENT_ICE_RECIPROCATING_STATUS_MAX_PACK_SIZE,
    encode_func,
    decode_func,
    null
};
*/

static void encode_uavcan_equipment_ice_reciprocating_Status(uavcan_equipment_ice_reciprocating_Status msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
    uint8_t[] buffer = new uint8_t[8];
    _encode_uavcan_equipment_ice_reciprocating_Status(buffer, msg, chunk_cb, ctx, true);
}

static uint32_t decode_uavcan_equipment_ice_reciprocating_Status(CanardRxTransfer transfer, uavcan_equipment_ice_reciprocating_Status msg) {
    uint32_t bit_ofs = 0;
    _decode_uavcan_equipment_ice_reciprocating_Status(transfer, ref bit_ofs, msg, true);
    return (bit_ofs+7)/8;
}

static void _encode_uavcan_equipment_ice_reciprocating_Status(uint8_t[] buffer, uavcan_equipment_ice_reciprocating_Status msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {

    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 2, msg.state);
    chunk_cb(buffer, 2, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 30, msg.flags);
    chunk_cb(buffer, 30, ctx);
    chunk_cb(null, 16, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 7, msg.engine_load_percent);
    chunk_cb(buffer, 7, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 17, msg.engine_speed_rpm);
    chunk_cb(buffer, 17, ctx);
    memset(buffer,0,8);
    {
        uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.spark_dwell_time_ms);
        canardEncodeScalar(buffer, 0, 16, float16_val);
    }
    chunk_cb(buffer, 16, ctx);
    memset(buffer,0,8);
    {
        uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.atmospheric_pressure_kpa);
        canardEncodeScalar(buffer, 0, 16, float16_val);
    }
    chunk_cb(buffer, 16, ctx);
    memset(buffer,0,8);
    {
        uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.intake_manifold_pressure_kpa);
        canardEncodeScalar(buffer, 0, 16, float16_val);
    }
    chunk_cb(buffer, 16, ctx);
    memset(buffer,0,8);
    {
        uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.intake_manifold_temperature);
        canardEncodeScalar(buffer, 0, 16, float16_val);
    }
    chunk_cb(buffer, 16, ctx);
    memset(buffer,0,8);
    {
        uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.coolant_temperature);
        canardEncodeScalar(buffer, 0, 16, float16_val);
    }
    chunk_cb(buffer, 16, ctx);
    memset(buffer,0,8);
    {
        uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.oil_pressure);
        canardEncodeScalar(buffer, 0, 16, float16_val);
    }
    chunk_cb(buffer, 16, ctx);
    memset(buffer,0,8);
    {
        uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.oil_temperature);
        canardEncodeScalar(buffer, 0, 16, float16_val);
    }
    chunk_cb(buffer, 16, ctx);
    memset(buffer,0,8);
    {
        uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.fuel_pressure);
        canardEncodeScalar(buffer, 0, 16, float16_val);
    }
    chunk_cb(buffer, 16, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 32, msg.fuel_consumption_rate_cm3pm);
    chunk_cb(buffer, 32, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 32, msg.estimated_consumed_fuel_volume_cm3);
    chunk_cb(buffer, 32, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 7, msg.throttle_position_percent);
    chunk_cb(buffer, 7, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 6, msg.ecu_index);
    chunk_cb(buffer, 6, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 3, msg.spark_plug_usage);
    chunk_cb(buffer, 3, ctx);
    if (!tao) {
        memset(buffer,0,8);
        canardEncodeScalar(buffer, 0, 5, msg.cylinder_status_len);
        chunk_cb(buffer, 5, ctx);
    }
    for (int i=0; i < msg.cylinder_status_len; i++) {
            _encode_uavcan_equipment_ice_reciprocating_CylinderStatus(buffer, msg.cylinder_status[i], chunk_cb, ctx, false);
    }
}

static void _decode_uavcan_equipment_ice_reciprocating_Status(CanardRxTransfer transfer,ref uint32_t bit_ofs, uavcan_equipment_ice_reciprocating_Status msg, bool tao) {

    canardDecodeScalar(transfer, bit_ofs, 2, false, ref msg.state);
    bit_ofs += 2;

    canardDecodeScalar(transfer, bit_ofs, 30, false, ref msg.flags);
    bit_ofs += 30;

    bit_ofs += 16;

    canardDecodeScalar(transfer, bit_ofs, 7, false, ref msg.engine_load_percent);
    bit_ofs += 7;

    canardDecodeScalar(transfer, bit_ofs, 17, false, ref msg.engine_speed_rpm);
    bit_ofs += 17;

    {
        uint16_t float16_val = 0;
        canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
        msg.spark_dwell_time_ms = canardConvertFloat16ToNativeFloat(float16_val);
    }
    bit_ofs += 16;

    {
        uint16_t float16_val = 0;
        canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
        msg.atmospheric_pressure_kpa = canardConvertFloat16ToNativeFloat(float16_val);
    }
    bit_ofs += 16;

    {
        uint16_t float16_val = 0;
        canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
        msg.intake_manifold_pressure_kpa = canardConvertFloat16ToNativeFloat(float16_val);
    }
    bit_ofs += 16;

    {
        uint16_t float16_val = 0;
        canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
        msg.intake_manifold_temperature = canardConvertFloat16ToNativeFloat(float16_val);
    }
    bit_ofs += 16;

    {
        uint16_t float16_val = 0;
        canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
        msg.coolant_temperature = canardConvertFloat16ToNativeFloat(float16_val);
    }
    bit_ofs += 16;

    {
        uint16_t float16_val = 0;
        canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
        msg.oil_pressure = canardConvertFloat16ToNativeFloat(float16_val);
    }
    bit_ofs += 16;

    {
        uint16_t float16_val = 0;
        canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
        msg.oil_temperature = canardConvertFloat16ToNativeFloat(float16_val);
    }
    bit_ofs += 16;

    {
        uint16_t float16_val = 0;
        canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
        msg.fuel_pressure = canardConvertFloat16ToNativeFloat(float16_val);
    }
    bit_ofs += 16;

    canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.fuel_consumption_rate_cm3pm);
    bit_ofs += 32;

    canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.estimated_consumed_fuel_volume_cm3);
    bit_ofs += 32;

    canardDecodeScalar(transfer, bit_ofs, 7, false, ref msg.throttle_position_percent);
    bit_ofs += 7;

    canardDecodeScalar(transfer, bit_ofs, 6, false, ref msg.ecu_index);
    bit_ofs += 6;

    canardDecodeScalar(transfer, bit_ofs, 3, false, ref msg.spark_plug_usage);
    bit_ofs += 3;

    if (!tao) {
        canardDecodeScalar(transfer, bit_ofs, 5, false, ref msg.cylinder_status_len);
        bit_ofs += 5;
    }


    if (tao) {
msg.cylinder_status_len = 0;
        while (((transfer.payload_len*8)-bit_ofs) > 0) {
            _decode_uavcan_equipment_ice_reciprocating_CylinderStatus(transfer, ref bit_ofs, msg.cylinder_status[msg.cylinder_status_len], false);
            msg.cylinder_status_len++;
        }
    } else {
        for (int i=0; i < msg.cylinder_status_len; i++) {
            _decode_uavcan_equipment_ice_reciprocating_CylinderStatus(transfer, ref bit_ofs, msg.cylinder_status[i], false);
        }
    }

}

}
}