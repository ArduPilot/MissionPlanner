
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

static uavcan_message_descriptor_s uavcan_olliw_uc4h_GenericBatteryInfo_descriptor = {
    UAVCAN_OLLIW_UC4H_GENERICBATTERYINFO_DT_SIG,
    UAVCAN_OLLIW_UC4H_GENERICBATTERYINFO_DT_ID,
    CanardTransferTypeBroadcast,
    sizeof(uavcan_olliw_uc4h_GenericBatteryInfo),
    UAVCAN_OLLIW_UC4H_GENERICBATTERYINFO_MAX_PACK_SIZE,
    encode_func,
    decode_func,
    null
};
*/

static void encode_uavcan_olliw_uc4h_GenericBatteryInfo(uavcan_olliw_uc4h_GenericBatteryInfo msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
    uint8_t[] buffer = new uint8_t[8];
    _encode_uavcan_olliw_uc4h_GenericBatteryInfo(buffer, msg, chunk_cb, ctx, true);
}

static uint32_t decode_uavcan_olliw_uc4h_GenericBatteryInfo(CanardRxTransfer transfer, uavcan_olliw_uc4h_GenericBatteryInfo msg) {
    uint32_t bit_ofs = 0;
    _decode_uavcan_olliw_uc4h_GenericBatteryInfo(transfer, ref bit_ofs, msg, true);
    return (bit_ofs+7)/8;
}

static void _encode_uavcan_olliw_uc4h_GenericBatteryInfo(uint8_t[] buffer, uavcan_olliw_uc4h_GenericBatteryInfo msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {

    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 16, msg.battery_id);
    chunk_cb(buffer, 16, ctx);
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
        uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.charge_consumed_mAh);
        canardEncodeScalar(buffer, 0, 16, float16_val);
    }
    chunk_cb(buffer, 16, ctx);
    memset(buffer,0,8);
    {
        uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.energy_consumed_Wh);
        canardEncodeScalar(buffer, 0, 16, float16_val);
    }
    chunk_cb(buffer, 16, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 8, msg.status_flags);
    chunk_cb(buffer, 8, ctx);
    if (!tao) {
        memset(buffer,0,8);
        canardEncodeScalar(buffer, 0, 4, msg.cell_voltages_len);
        chunk_cb(buffer, 4, ctx);
    }
    for (int i=0; i < msg.cell_voltages_len; i++) {
            memset(buffer,0,8);
            {
                uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.cell_voltages[i]);
                canardEncodeScalar(buffer, 0, 16, float16_val);
            }
            chunk_cb(buffer, 16, ctx);
    }
}

static void _decode_uavcan_olliw_uc4h_GenericBatteryInfo(CanardRxTransfer transfer,ref uint32_t bit_ofs, uavcan_olliw_uc4h_GenericBatteryInfo msg, bool tao) {

    canardDecodeScalar(transfer, bit_ofs, 16, false, ref msg.battery_id);
    bit_ofs += 16;

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
        msg.charge_consumed_mAh = canardConvertFloat16ToNativeFloat(float16_val);
    }
    bit_ofs += 16;

    {
        uint16_t float16_val = 0;
        canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
        msg.energy_consumed_Wh = canardConvertFloat16ToNativeFloat(float16_val);
    }
    bit_ofs += 16;

    canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.status_flags);
    bit_ofs += 8;

    if (!tao) {
        canardDecodeScalar(transfer, bit_ofs, 4, false, ref msg.cell_voltages_len);
        bit_ofs += 4;
    } else {
        msg.cell_voltages_len = (uint8_t)(((transfer.payload_len*8)-bit_ofs)/16);
    }

    for (int i=0; i < msg.cell_voltages_len; i++) {
        {
            uint16_t float16_val = 0;
            canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
            msg.cell_voltages[i] = canardConvertFloat16ToNativeFloat(float16_val);
        }
        bit_ofs += 16;
    }

}

}
}