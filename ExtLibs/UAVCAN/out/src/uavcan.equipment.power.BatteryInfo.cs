


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

static uavcan_message_descriptor_s uavcan_equipment_power_BatteryInfo_descriptor = {
    UAVCAN_EQUIPMENT_POWER_BATTERYINFO_DT_SIG,
    UAVCAN_EQUIPMENT_POWER_BATTERYINFO_DT_ID,

    CanardTransferTypeBroadcast,

    sizeof(uavcan_equipment_power_BatteryInfo),
    UAVCAN_EQUIPMENT_POWER_BATTERYINFO_MAX_PACK_SIZE,
    encode_func,
    decode_func,

    null

};
*/


static void encode_uavcan_equipment_power_BatteryInfo(uavcan_equipment_power_BatteryInfo msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
    uint8_t[] buffer = new uint8_t[8];
    _encode_uavcan_equipment_power_BatteryInfo(buffer, msg, chunk_cb, ctx, true);
}

static uint32_t decode_uavcan_equipment_power_BatteryInfo(CanardRxTransfer transfer, uavcan_equipment_power_BatteryInfo msg) {
    uint32_t bit_ofs = 0;
    _decode_uavcan_equipment_power_BatteryInfo(transfer, ref bit_ofs, msg, true);
    return (bit_ofs+7)/8;
}

static void _encode_uavcan_equipment_power_BatteryInfo(uint8_t[] buffer, uavcan_equipment_power_BatteryInfo msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {






    memset(buffer,0,8);

    {
        uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.temperature);
        canardEncodeScalar(buffer, 0, 16, float16_val);
    }

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
        uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.average_power_10sec);
        canardEncodeScalar(buffer, 0, 16, float16_val);
    }

    chunk_cb(buffer, 16, ctx);





    memset(buffer,0,8);

    {
        uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.remaining_capacity_wh);
        canardEncodeScalar(buffer, 0, 16, float16_val);
    }

    chunk_cb(buffer, 16, ctx);





    memset(buffer,0,8);

    {
        uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.full_charge_capacity_wh);
        canardEncodeScalar(buffer, 0, 16, float16_val);
    }

    chunk_cb(buffer, 16, ctx);





    memset(buffer,0,8);

    {
        uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.hours_to_full_charge);
        canardEncodeScalar(buffer, 0, 16, float16_val);
    }

    chunk_cb(buffer, 16, ctx);





    memset(buffer,0,8);

    canardEncodeScalar(buffer, 0, 11, msg.status_flags);

    chunk_cb(buffer, 11, ctx);





    memset(buffer,0,8);

    canardEncodeScalar(buffer, 0, 7, msg.state_of_health_pct);

    chunk_cb(buffer, 7, ctx);





    memset(buffer,0,8);

    canardEncodeScalar(buffer, 0, 7, msg.state_of_charge_pct);

    chunk_cb(buffer, 7, ctx);





    memset(buffer,0,8);

    canardEncodeScalar(buffer, 0, 7, msg.state_of_charge_pct_stdev);

    chunk_cb(buffer, 7, ctx);





    memset(buffer,0,8);

    canardEncodeScalar(buffer, 0, 8, msg.battery_id);

    chunk_cb(buffer, 8, ctx);





    memset(buffer,0,8);

    canardEncodeScalar(buffer, 0, 32, msg.model_instance_id);

    chunk_cb(buffer, 32, ctx);







    if (!tao) {


        memset(buffer,0,8);
        canardEncodeScalar(buffer, 0, 5, msg.model_name_len);
        chunk_cb(buffer, 5, ctx);


    }

    for (int i=0; i < msg.model_name_len; i++) {



            memset(buffer,0,8);

            canardEncodeScalar(buffer, 0, 8, msg.model_name[i]);

            chunk_cb(buffer, 8, ctx);


    }





}

static void _decode_uavcan_equipment_power_BatteryInfo(CanardRxTransfer transfer,ref uint32_t bit_ofs, uavcan_equipment_power_BatteryInfo msg, bool tao) {







    {
        uint16_t float16_val = 0;
        canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
        msg.temperature = canardConvertFloat16ToNativeFloat(float16_val);
    }

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
        msg.average_power_10sec = canardConvertFloat16ToNativeFloat(float16_val);
    }

    bit_ofs += 16;







    {
        uint16_t float16_val = 0;
        canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
        msg.remaining_capacity_wh = canardConvertFloat16ToNativeFloat(float16_val);
    }

    bit_ofs += 16;







    {
        uint16_t float16_val = 0;
        canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
        msg.full_charge_capacity_wh = canardConvertFloat16ToNativeFloat(float16_val);
    }

    bit_ofs += 16;







    {
        uint16_t float16_val = 0;
        canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
        msg.hours_to_full_charge = canardConvertFloat16ToNativeFloat(float16_val);
    }

    bit_ofs += 16;








    canardDecodeScalar(transfer, bit_ofs, 11, false, ref msg.status_flags);


    bit_ofs += 11;








    canardDecodeScalar(transfer, bit_ofs, 7, false, ref msg.state_of_health_pct);


    bit_ofs += 7;








    canardDecodeScalar(transfer, bit_ofs, 7, false, ref msg.state_of_charge_pct);


    bit_ofs += 7;








    canardDecodeScalar(transfer, bit_ofs, 7, false, ref msg.state_of_charge_pct_stdev);


    bit_ofs += 7;








    canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.battery_id);


    bit_ofs += 8;








    canardDecodeScalar(transfer, bit_ofs, 32, false, ref msg.model_instance_id);


    bit_ofs += 32;








    if (!tao) {


        canardDecodeScalar(transfer, bit_ofs, 5, false, ref msg.model_name_len);
        bit_ofs += 5;



    } else {

        msg.model_name_len = (uint8_t)(((transfer.payload_len*8)-bit_ofs)/8);


    }



    for (int i=0; i < msg.model_name_len; i++) {




        canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.model_name[i]);

        bit_ofs += 8;


    }






}

}
}