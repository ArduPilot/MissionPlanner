


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

static uavcan_message_descriptor_s uavcan_navigation_GlobalNavigationSolution_descriptor = {
    UAVCAN_NAVIGATION_GLOBALNAVIGATIONSOLUTION_DT_SIG,
    UAVCAN_NAVIGATION_GLOBALNAVIGATIONSOLUTION_DT_ID,

    CanardTransferTypeBroadcast,

    sizeof(uavcan_navigation_GlobalNavigationSolution),
    UAVCAN_NAVIGATION_GLOBALNAVIGATIONSOLUTION_MAX_PACK_SIZE,
    encode_func,
    decode_func,

    null

};
*/


static void encode_uavcan_navigation_GlobalNavigationSolution(uavcan_navigation_GlobalNavigationSolution msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
    uint8_t[] buffer = new uint8_t[8];
    _encode_uavcan_navigation_GlobalNavigationSolution(buffer, msg, chunk_cb, ctx, true);
}

static uint32_t decode_uavcan_navigation_GlobalNavigationSolution(CanardRxTransfer transfer, uavcan_navigation_GlobalNavigationSolution msg) {
    uint32_t bit_ofs = 0;
    _decode_uavcan_navigation_GlobalNavigationSolution(transfer, ref bit_ofs, msg, true);
    return (bit_ofs+7)/8;
}

static void _encode_uavcan_navigation_GlobalNavigationSolution(uint8_t[] buffer, uavcan_navigation_GlobalNavigationSolution msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {






    _encode_uavcan_Timestamp(buffer, msg.timestamp, chunk_cb, ctx, false);





    memset(buffer,0,8);

    canardEncodeScalar(buffer, 0, 64, msg.longitude);

    chunk_cb(buffer, 64, ctx);





    memset(buffer,0,8);

    canardEncodeScalar(buffer, 0, 64, msg.latitude);

    chunk_cb(buffer, 64, ctx);





    memset(buffer,0,8);

    canardEncodeScalar(buffer, 0, 32, msg.height_ellipsoid);

    chunk_cb(buffer, 32, ctx);





    memset(buffer,0,8);

    canardEncodeScalar(buffer, 0, 32, msg.height_msl);

    chunk_cb(buffer, 32, ctx);





    memset(buffer,0,8);

    canardEncodeScalar(buffer, 0, 32, msg.height_agl);

    chunk_cb(buffer, 32, ctx);





    memset(buffer,0,8);

    canardEncodeScalar(buffer, 0, 32, msg.height_baro);

    chunk_cb(buffer, 32, ctx);





    memset(buffer,0,8);

    {
        uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.qnh_hpa);
        canardEncodeScalar(buffer, 0, 16, float16_val);
    }

    chunk_cb(buffer, 16, ctx);






    for (int i=0; i < 4; i++) {



            memset(buffer,0,8);

            canardEncodeScalar(buffer, 0, 32, msg.orientation_xyzw[i]);

            chunk_cb(buffer, 32, ctx);


    }







    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 6, msg.pose_covariance_len);
    chunk_cb(buffer, 6, ctx);

    for (int i=0; i < msg.pose_covariance_len; i++) {



            memset(buffer,0,8);

            {
                uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.pose_covariance[i]);
                canardEncodeScalar(buffer, 0, 16, float16_val);
            }

            chunk_cb(buffer, 16, ctx);


    }






    for (int i=0; i < 3; i++) {



            memset(buffer,0,8);

            canardEncodeScalar(buffer, 0, 32, msg.linear_velocity_body[i]);

            chunk_cb(buffer, 32, ctx);


    }






    for (int i=0; i < 3; i++) {



            memset(buffer,0,8);

            canardEncodeScalar(buffer, 0, 32, msg.angular_velocity_body[i]);

            chunk_cb(buffer, 32, ctx);


    }






    for (int i=0; i < 3; i++) {



            memset(buffer,0,8);

            {
                uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.linear_acceleration_body[i]);
                canardEncodeScalar(buffer, 0, 16, float16_val);
            }

            chunk_cb(buffer, 16, ctx);


    }







    if (!tao) {


        memset(buffer,0,8);
        canardEncodeScalar(buffer, 0, 6, msg.velocity_covariance_len);
        chunk_cb(buffer, 6, ctx);


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

static void _decode_uavcan_navigation_GlobalNavigationSolution(CanardRxTransfer transfer,ref uint32_t bit_ofs, uavcan_navigation_GlobalNavigationSolution msg, bool tao) {






    _decode_uavcan_Timestamp(transfer, ref bit_ofs, msg.timestamp, false);








    canardDecodeScalar(transfer, bit_ofs, 64, true, ref msg.longitude);


    bit_ofs += 64;








    canardDecodeScalar(transfer, bit_ofs, 64, true, ref msg.latitude);


    bit_ofs += 64;








    canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.height_ellipsoid);


    bit_ofs += 32;








    canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.height_msl);


    bit_ofs += 32;








    canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.height_agl);


    bit_ofs += 32;








    canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.height_baro);


    bit_ofs += 32;







    {
        uint16_t float16_val = 0;
        canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
        msg.qnh_hpa = canardConvertFloat16ToNativeFloat(float16_val);
    }

    bit_ofs += 16;







/*['__doc__', '__init__', '__module__', '__repr__', '__str__', 'get_normalized_definition', 'name', 'type']*/
    for (int i=0; i < 4; i++) {




        canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.orientation_xyzw[i]);

        bit_ofs += 32;


    }









    canardDecodeScalar(transfer, bit_ofs, 6, false, ref msg.pose_covariance_len);
    bit_ofs += 6;


    for (int i=0; i < msg.pose_covariance_len; i++) {




        {
            uint16_t float16_val = 0;
            canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
            msg.pose_covariance[i] = canardConvertFloat16ToNativeFloat(float16_val);
        }

        bit_ofs += 16;


    }








/*['__doc__', '__init__', '__module__', '__repr__', '__str__', 'get_normalized_definition', 'name', 'type']*/
    for (int i=0; i < 3; i++) {




        canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.linear_velocity_body[i]);

        bit_ofs += 32;


    }








/*['__doc__', '__init__', '__module__', '__repr__', '__str__', 'get_normalized_definition', 'name', 'type']*/
    for (int i=0; i < 3; i++) {




        canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.angular_velocity_body[i]);

        bit_ofs += 32;


    }








/*['__doc__', '__init__', '__module__', '__repr__', '__str__', 'get_normalized_definition', 'name', 'type']*/
    for (int i=0; i < 3; i++) {




        {
            uint16_t float16_val = 0;
            canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
            msg.linear_acceleration_body[i] = canardConvertFloat16ToNativeFloat(float16_val);
        }

        bit_ofs += 16;


    }









    if (!tao) {


        canardDecodeScalar(transfer, bit_ofs, 6, false, ref msg.velocity_covariance_len);
        bit_ofs += 6;



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