


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

static uavcan_message_descriptor_s uavcan_equipment_power_PrimaryPowerSupplyStatus_descriptor = {
    UAVCAN_EQUIPMENT_POWER_PRIMARYPOWERSUPPLYSTATUS_DT_SIG,
    UAVCAN_EQUIPMENT_POWER_PRIMARYPOWERSUPPLYSTATUS_DT_ID,

    CanardTransferTypeBroadcast,

    sizeof(uavcan_equipment_power_PrimaryPowerSupplyStatus),
    UAVCAN_EQUIPMENT_POWER_PRIMARYPOWERSUPPLYSTATUS_MAX_PACK_SIZE,
    encode_func,
    decode_func,

    null

};
*/


static void encode_uavcan_equipment_power_PrimaryPowerSupplyStatus(uavcan_equipment_power_PrimaryPowerSupplyStatus msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
    uint8_t[] buffer = new uint8_t[8];
    _encode_uavcan_equipment_power_PrimaryPowerSupplyStatus(buffer, msg, chunk_cb, ctx, true);
}

static uint32_t decode_uavcan_equipment_power_PrimaryPowerSupplyStatus(CanardRxTransfer transfer, uavcan_equipment_power_PrimaryPowerSupplyStatus msg) {
    uint32_t bit_ofs = 0;
    _decode_uavcan_equipment_power_PrimaryPowerSupplyStatus(transfer, ref bit_ofs, msg, true);
    return (bit_ofs+7)/8;
}

static void _encode_uavcan_equipment_power_PrimaryPowerSupplyStatus(uint8_t[] buffer, uavcan_equipment_power_PrimaryPowerSupplyStatus msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {






    memset(buffer,0,8);

    {
        uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.hours_to_empty_at_10sec_avg_power);
        canardEncodeScalar(buffer, 0, 16, float16_val);
    }

    chunk_cb(buffer, 16, ctx);





    memset(buffer,0,8);

    {
        uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.hours_to_empty_at_10sec_avg_power_variance);
        canardEncodeScalar(buffer, 0, 16, float16_val);
    }

    chunk_cb(buffer, 16, ctx);





    memset(buffer,0,8);

    canardEncodeScalar(buffer, 0, 1, msg.external_power_available);

    chunk_cb(buffer, 1, ctx);





    memset(buffer,0,8);

    canardEncodeScalar(buffer, 0, 7, msg.remaining_energy_pct);

    chunk_cb(buffer, 7, ctx);





    memset(buffer,0,8);

    canardEncodeScalar(buffer, 0, 7, msg.remaining_energy_pct_stdev);

    chunk_cb(buffer, 7, ctx);





}

static void _decode_uavcan_equipment_power_PrimaryPowerSupplyStatus(CanardRxTransfer transfer,ref uint32_t bit_ofs, uavcan_equipment_power_PrimaryPowerSupplyStatus msg, bool tao) {







    {
        uint16_t float16_val = 0;
        canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
        msg.hours_to_empty_at_10sec_avg_power = canardConvertFloat16ToNativeFloat(float16_val);
    }

    bit_ofs += 16;







    {
        uint16_t float16_val = 0;
        canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
        msg.hours_to_empty_at_10sec_avg_power_variance = canardConvertFloat16ToNativeFloat(float16_val);
    }

    bit_ofs += 16;








    canardDecodeScalar(transfer, bit_ofs, 1, false, ref msg.external_power_available);


    bit_ofs += 1;








    canardDecodeScalar(transfer, bit_ofs, 7, false, ref msg.remaining_energy_pct);


    bit_ofs += 7;








    canardDecodeScalar(transfer, bit_ofs, 7, false, ref msg.remaining_energy_pct_stdev);


    bit_ofs += 7;





}

}
}