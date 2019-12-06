


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

static uavcan_message_descriptor_s uavcan_equipment_ice_FuelTankStatus_descriptor = {
    UAVCAN_EQUIPMENT_ICE_FUELTANKSTATUS_DT_SIG,
    UAVCAN_EQUIPMENT_ICE_FUELTANKSTATUS_DT_ID,

    CanardTransferTypeBroadcast,

    sizeof(uavcan_equipment_ice_FuelTankStatus),
    UAVCAN_EQUIPMENT_ICE_FUELTANKSTATUS_MAX_PACK_SIZE,
    encode_func,
    decode_func,

    null

};
*/


static void encode_uavcan_equipment_ice_FuelTankStatus(uavcan_equipment_ice_FuelTankStatus msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
    uint8_t[] buffer = new uint8_t[8];
    _encode_uavcan_equipment_ice_FuelTankStatus(buffer, msg, chunk_cb, ctx, true);
}

static uint32_t decode_uavcan_equipment_ice_FuelTankStatus(CanardRxTransfer transfer, uavcan_equipment_ice_FuelTankStatus msg) {
    uint32_t bit_ofs = 0;
    _decode_uavcan_equipment_ice_FuelTankStatus(transfer, ref bit_ofs, msg, true);
    return (bit_ofs+7)/8;
}

static void _encode_uavcan_equipment_ice_FuelTankStatus(uint8_t[] buffer, uavcan_equipment_ice_FuelTankStatus msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {






    chunk_cb(null, 9, ctx);





    memset(buffer,0,8);

    canardEncodeScalar(buffer, 0, 7, msg.available_fuel_volume_percent);

    chunk_cb(buffer, 7, ctx);





    memset(buffer,0,8);

    canardEncodeScalar(buffer, 0, 32, msg.available_fuel_volume_cm3);

    chunk_cb(buffer, 32, ctx);





    memset(buffer,0,8);

    canardEncodeScalar(buffer, 0, 32, msg.fuel_consumption_rate_cm3pm);

    chunk_cb(buffer, 32, ctx);





    memset(buffer,0,8);

    {
        uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.fuel_temperature);
        canardEncodeScalar(buffer, 0, 16, float16_val);
    }

    chunk_cb(buffer, 16, ctx);





    memset(buffer,0,8);

    canardEncodeScalar(buffer, 0, 8, msg.fuel_tank_id);

    chunk_cb(buffer, 8, ctx);





}

static void _decode_uavcan_equipment_ice_FuelTankStatus(CanardRxTransfer transfer,ref uint32_t bit_ofs, uavcan_equipment_ice_FuelTankStatus msg, bool tao) {






    bit_ofs += 9;








    canardDecodeScalar(transfer, bit_ofs, 7, false, ref msg.available_fuel_volume_percent);


    bit_ofs += 7;








    canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.available_fuel_volume_cm3);


    bit_ofs += 32;








    canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.fuel_consumption_rate_cm3pm);


    bit_ofs += 32;







    {
        uint16_t float16_val = 0;
        canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
        msg.fuel_temperature = canardConvertFloat16ToNativeFloat(float16_val);
    }

    bit_ofs += 16;








    canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.fuel_tank_id);


    bit_ofs += 8;





}

}
}