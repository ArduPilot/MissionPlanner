


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

static uavcan_message_descriptor_s uavcan_equipment_air_data_RawAirData_descriptor = {
    UAVCAN_EQUIPMENT_AIR_DATA_RAWAIRDATA_DT_SIG,
    UAVCAN_EQUIPMENT_AIR_DATA_RAWAIRDATA_DT_ID,

    CanardTransferTypeBroadcast,

    sizeof(uavcan_equipment_air_data_RawAirData),
    UAVCAN_EQUIPMENT_AIR_DATA_RAWAIRDATA_MAX_PACK_SIZE,
    encode_func,
    decode_func,

    null

};
*/


static void encode_uavcan_equipment_air_data_RawAirData(uavcan_equipment_air_data_RawAirData msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
    uint8_t[] buffer = new uint8_t[8];
    _encode_uavcan_equipment_air_data_RawAirData(buffer, msg, chunk_cb, ctx, true);
}

static uint32_t decode_uavcan_equipment_air_data_RawAirData(CanardRxTransfer transfer, uavcan_equipment_air_data_RawAirData msg) {
    uint32_t bit_ofs = 0;
    _decode_uavcan_equipment_air_data_RawAirData(transfer, ref bit_ofs, msg, true);
    return (bit_ofs+7)/8;
}

static void _encode_uavcan_equipment_air_data_RawAirData(uint8_t[] buffer, uavcan_equipment_air_data_RawAirData msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {






    memset(buffer,0,8);

    canardEncodeScalar(buffer, 0, 8, msg.flags);

    chunk_cb(buffer, 8, ctx);





    memset(buffer,0,8);

    canardEncodeScalar(buffer, 0, 32, msg.static_pressure);

    chunk_cb(buffer, 32, ctx);





    memset(buffer,0,8);

    canardEncodeScalar(buffer, 0, 32, msg.differential_pressure);

    chunk_cb(buffer, 32, ctx);





    memset(buffer,0,8);

    {
        uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.static_pressure_sensor_temperature);
        canardEncodeScalar(buffer, 0, 16, float16_val);
    }

    chunk_cb(buffer, 16, ctx);





    memset(buffer,0,8);

    {
        uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.differential_pressure_sensor_temperature);
        canardEncodeScalar(buffer, 0, 16, float16_val);
    }

    chunk_cb(buffer, 16, ctx);





    memset(buffer,0,8);

    {
        uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.static_air_temperature);
        canardEncodeScalar(buffer, 0, 16, float16_val);
    }

    chunk_cb(buffer, 16, ctx);





    memset(buffer,0,8);

    {
        uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.pitot_temperature);
        canardEncodeScalar(buffer, 0, 16, float16_val);
    }

    chunk_cb(buffer, 16, ctx);







    if (!tao) {


        memset(buffer,0,8);
        canardEncodeScalar(buffer, 0, 5, msg.covariance_len);
        chunk_cb(buffer, 5, ctx);


    }

    for (int i=0; i < msg.covariance_len; i++) {



            memset(buffer,0,8);

            {
                uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.covariance[i]);
                canardEncodeScalar(buffer, 0, 16, float16_val);
            }

            chunk_cb(buffer, 16, ctx);


    }





}

static void _decode_uavcan_equipment_air_data_RawAirData(CanardRxTransfer transfer,ref uint32_t bit_ofs, uavcan_equipment_air_data_RawAirData msg, bool tao) {








    canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.flags);


    bit_ofs += 8;








    canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.static_pressure);


    bit_ofs += 32;








    canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.differential_pressure);


    bit_ofs += 32;







    {
        uint16_t float16_val = 0;
        canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
        msg.static_pressure_sensor_temperature = canardConvertFloat16ToNativeFloat(float16_val);
    }

    bit_ofs += 16;







    {
        uint16_t float16_val = 0;
        canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
        msg.differential_pressure_sensor_temperature = canardConvertFloat16ToNativeFloat(float16_val);
    }

    bit_ofs += 16;







    {
        uint16_t float16_val = 0;
        canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
        msg.static_air_temperature = canardConvertFloat16ToNativeFloat(float16_val);
    }

    bit_ofs += 16;







    {
        uint16_t float16_val = 0;
        canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
        msg.pitot_temperature = canardConvertFloat16ToNativeFloat(float16_val);
    }

    bit_ofs += 16;








    if (!tao) {


        canardDecodeScalar(transfer, bit_ofs, 5, false, ref msg.covariance_len);
        bit_ofs += 5;



    } else {

        msg.covariance_len = (uint8_t)(((transfer.payload_len*8)-bit_ofs)/16);


    }



    for (int i=0; i < msg.covariance_len; i++) {




        {
            uint16_t float16_val = 0;
            canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
            msg.covariance[i] = canardConvertFloat16ToNativeFloat(float16_val);
        }

        bit_ofs += 16;


    }






}

}
}