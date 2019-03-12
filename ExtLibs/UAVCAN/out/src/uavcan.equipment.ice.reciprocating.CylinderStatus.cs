
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



static void encode_uavcan_equipment_ice_reciprocating_CylinderStatus(uavcan_equipment_ice_reciprocating_CylinderStatus msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
    uint8_t[] buffer = new uint8_t[8];
    _encode_uavcan_equipment_ice_reciprocating_CylinderStatus(buffer, msg, chunk_cb, ctx, true);
}

static uint32_t decode_uavcan_equipment_ice_reciprocating_CylinderStatus(CanardRxTransfer transfer, uavcan_equipment_ice_reciprocating_CylinderStatus msg) {
    uint32_t bit_ofs = 0;
    _decode_uavcan_equipment_ice_reciprocating_CylinderStatus(transfer, ref bit_ofs, msg, true);
    return (bit_ofs+7)/8;
}

static void _encode_uavcan_equipment_ice_reciprocating_CylinderStatus(uint8_t[] buffer, uavcan_equipment_ice_reciprocating_CylinderStatus msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {

    memset(buffer,0,8);
    {
        uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.ignition_timing_deg);
        canardEncodeScalar(buffer, 0, 16, float16_val);
    }
    chunk_cb(buffer, 16, ctx);
    memset(buffer,0,8);
    {
        uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.injection_time_ms);
        canardEncodeScalar(buffer, 0, 16, float16_val);
    }
    chunk_cb(buffer, 16, ctx);
    memset(buffer,0,8);
    {
        uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.cylinder_head_temperature);
        canardEncodeScalar(buffer, 0, 16, float16_val);
    }
    chunk_cb(buffer, 16, ctx);
    memset(buffer,0,8);
    {
        uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.exhaust_gas_temperature);
        canardEncodeScalar(buffer, 0, 16, float16_val);
    }
    chunk_cb(buffer, 16, ctx);
    memset(buffer,0,8);
    {
        uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.lambda_coefficient);
        canardEncodeScalar(buffer, 0, 16, float16_val);
    }
    chunk_cb(buffer, 16, ctx);
}

static void _decode_uavcan_equipment_ice_reciprocating_CylinderStatus(CanardRxTransfer transfer,ref uint32_t bit_ofs, uavcan_equipment_ice_reciprocating_CylinderStatus msg, bool tao) {

    {
        uint16_t float16_val = 0;
        canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
        msg.ignition_timing_deg = canardConvertFloat16ToNativeFloat(float16_val);
    }
    bit_ofs += 16;

    {
        uint16_t float16_val = 0;
        canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
        msg.injection_time_ms = canardConvertFloat16ToNativeFloat(float16_val);
    }
    bit_ofs += 16;

    {
        uint16_t float16_val = 0;
        canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
        msg.cylinder_head_temperature = canardConvertFloat16ToNativeFloat(float16_val);
    }
    bit_ofs += 16;

    {
        uint16_t float16_val = 0;
        canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
        msg.exhaust_gas_temperature = canardConvertFloat16ToNativeFloat(float16_val);
    }
    bit_ofs += 16;

    {
        uint16_t float16_val = 0;
        canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
        msg.lambda_coefficient = canardConvertFloat16ToNativeFloat(float16_val);
    }
    bit_ofs += 16;

}

}
}