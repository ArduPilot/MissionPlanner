


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

static uavcan_message_descriptor_s uavcan_equipment_gnss_Fix_descriptor = {
    UAVCAN_EQUIPMENT_GNSS_FIX_DT_SIG,
    UAVCAN_EQUIPMENT_GNSS_FIX_DT_ID,

    CanardTransferTypeBroadcast,

    sizeof(uavcan_equipment_gnss_Fix),
    UAVCAN_EQUIPMENT_GNSS_FIX_MAX_PACK_SIZE,
    encode_func,
    decode_func,

    null

};
*/


static void encode_uavcan_equipment_gnss_Fix(uavcan_equipment_gnss_Fix msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
    uint8_t[] buffer = new uint8_t[8];
    _encode_uavcan_equipment_gnss_Fix(buffer, msg, chunk_cb, ctx, true);
}

static uint32_t decode_uavcan_equipment_gnss_Fix(CanardRxTransfer transfer, uavcan_equipment_gnss_Fix msg) {
    uint32_t bit_ofs = 0;
    _decode_uavcan_equipment_gnss_Fix(transfer, ref bit_ofs, msg, true);
    return (bit_ofs+7)/8;
}

static void _encode_uavcan_equipment_gnss_Fix(uint8_t[] buffer, uavcan_equipment_gnss_Fix msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {






    _encode_uavcan_Timestamp(buffer, msg.timestamp, chunk_cb, ctx, false);





    _encode_uavcan_Timestamp(buffer, msg.gnss_timestamp, chunk_cb, ctx, false);





    memset(buffer,0,8);

    canardEncodeScalar(buffer, 0, 3, msg.gnss_time_standard);

    chunk_cb(buffer, 3, ctx);





    chunk_cb(null, 5, ctx);





    memset(buffer,0,8);

    canardEncodeScalar(buffer, 0, 8, msg.num_leap_seconds);

    chunk_cb(buffer, 8, ctx);





    memset(buffer,0,8);

    canardEncodeScalar(buffer, 0, 37, msg.longitude_deg_1e8);

    chunk_cb(buffer, 37, ctx);





    memset(buffer,0,8);

    canardEncodeScalar(buffer, 0, 37, msg.latitude_deg_1e8);

    chunk_cb(buffer, 37, ctx);





    memset(buffer,0,8);

    canardEncodeScalar(buffer, 0, 27, msg.height_ellipsoid_mm);

    chunk_cb(buffer, 27, ctx);





    memset(buffer,0,8);

    canardEncodeScalar(buffer, 0, 27, msg.height_msl_mm);

    chunk_cb(buffer, 27, ctx);






    for (int i=0; i < 3; i++) {



            memset(buffer,0,8);

            {
                uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.ned_velocity[i]);
                canardEncodeScalar(buffer, 0, 16, float16_val);
            }

            chunk_cb(buffer, 16, ctx);


    }





    memset(buffer,0,8);

    canardEncodeScalar(buffer, 0, 6, msg.sats_used);

    chunk_cb(buffer, 6, ctx);





    memset(buffer,0,8);

    canardEncodeScalar(buffer, 0, 2, msg.status);

    chunk_cb(buffer, 2, ctx);





    memset(buffer,0,8);

    {
        uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.pdop);
        canardEncodeScalar(buffer, 0, 16, float16_val);
    }

    chunk_cb(buffer, 16, ctx);





    chunk_cb(null, 4, ctx);







    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 4, msg.position_covariance_len);
    chunk_cb(buffer, 4, ctx);

    for (int i=0; i < msg.position_covariance_len; i++) {



            memset(buffer,0,8);

            {
                uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.position_covariance[i]);
                canardEncodeScalar(buffer, 0, 16, float16_val);
            }

            chunk_cb(buffer, 16, ctx);


    }







    if (!tao) {


        memset(buffer,0,8);
        canardEncodeScalar(buffer, 0, 4, msg.velocity_covariance_len);
        chunk_cb(buffer, 4, ctx);


    }

    for (int i=0; i < msg.velocity_covariance_len; i++) {



            memset(buffer,0,8);

            {
                uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.velocity_covariance[i]);
                canardEncodeScalar(buffer, 0, 16, float16_val);
            }

            chunk_cb(buffer, 16, ctx);


    }





}

static void _decode_uavcan_equipment_gnss_Fix(CanardRxTransfer transfer,ref uint32_t bit_ofs, uavcan_equipment_gnss_Fix msg, bool tao) {






    _decode_uavcan_Timestamp(transfer, ref bit_ofs, msg.timestamp, false);






    _decode_uavcan_Timestamp(transfer, ref bit_ofs, msg.gnss_timestamp, false);








    canardDecodeScalar(transfer, bit_ofs, 3, false, ref msg.gnss_time_standard);


    bit_ofs += 3;






    bit_ofs += 5;








    canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.num_leap_seconds);


    bit_ofs += 8;








    canardDecodeScalar(transfer, bit_ofs, 37, true, ref msg.longitude_deg_1e8);


    bit_ofs += 37;








    canardDecodeScalar(transfer, bit_ofs, 37, true, ref msg.latitude_deg_1e8);


    bit_ofs += 37;








    canardDecodeScalar(transfer, bit_ofs, 27, true, ref msg.height_ellipsoid_mm);


    bit_ofs += 27;








    canardDecodeScalar(transfer, bit_ofs, 27, true, ref msg.height_msl_mm);


    bit_ofs += 27;







/*['__doc__', '__init__', '__module__', '__repr__', '__str__', 'get_normalized_definition', 'name', 'type']*/
    for (int i=0; i < 3; i++) {




        {
            uint16_t float16_val = 0;
            canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
            msg.ned_velocity[i] = canardConvertFloat16ToNativeFloat(float16_val);
        }

        bit_ofs += 16;


    }









    canardDecodeScalar(transfer, bit_ofs, 6, false, ref msg.sats_used);


    bit_ofs += 6;








    canardDecodeScalar(transfer, bit_ofs, 2, false, ref msg.status);


    bit_ofs += 2;







    {
        uint16_t float16_val = 0;
        canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
        msg.pdop = canardConvertFloat16ToNativeFloat(float16_val);
    }

    bit_ofs += 16;






    bit_ofs += 4;








    canardDecodeScalar(transfer, bit_ofs, 4, false, ref msg.position_covariance_len);
    bit_ofs += 4;


    for (int i=0; i < msg.position_covariance_len; i++) {




        {
            uint16_t float16_val = 0;
            canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
            msg.position_covariance[i] = canardConvertFloat16ToNativeFloat(float16_val);
        }

        bit_ofs += 16;


    }









    if (!tao) {


        canardDecodeScalar(transfer, bit_ofs, 4, false, ref msg.velocity_covariance_len);
        bit_ofs += 4;



    } else {

        msg.velocity_covariance_len = (uint8_t)(((transfer.payload_len*8)-bit_ofs)/16);


    }



    for (int i=0; i < msg.velocity_covariance_len; i++) {




        {
            uint16_t float16_val = 0;
            canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
            msg.velocity_covariance[i] = canardConvertFloat16ToNativeFloat(float16_val);
        }

        bit_ofs += 16;


    }






}

}
}