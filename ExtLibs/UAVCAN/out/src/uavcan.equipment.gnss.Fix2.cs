


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

static uavcan_message_descriptor_s uavcan_equipment_gnss_Fix2_descriptor = {
    UAVCAN_EQUIPMENT_GNSS_FIX2_DT_SIG,
    UAVCAN_EQUIPMENT_GNSS_FIX2_DT_ID,

    CanardTransferTypeBroadcast,

    sizeof(uavcan_equipment_gnss_Fix2),
    UAVCAN_EQUIPMENT_GNSS_FIX2_MAX_PACK_SIZE,
    encode_func,
    decode_func,

    null

};
*/


static void encode_uavcan_equipment_gnss_Fix2(uavcan_equipment_gnss_Fix2 msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
    uint8_t[] buffer = new uint8_t[8];
    _encode_uavcan_equipment_gnss_Fix2(buffer, msg, chunk_cb, ctx, true);
}

static uint32_t decode_uavcan_equipment_gnss_Fix2(CanardRxTransfer transfer, uavcan_equipment_gnss_Fix2 msg) {
    uint32_t bit_ofs = 0;
    _decode_uavcan_equipment_gnss_Fix2(transfer, ref bit_ofs, msg, true);
    return (bit_ofs+7)/8;
}

static void _encode_uavcan_equipment_gnss_Fix2(uint8_t[] buffer, uavcan_equipment_gnss_Fix2 msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {






    _encode_uavcan_Timestamp(buffer, msg.timestamp, chunk_cb, ctx, false);





    _encode_uavcan_Timestamp(buffer, msg.gnss_timestamp, chunk_cb, ctx, false);





    memset(buffer,0,8);

    canardEncodeScalar(buffer, 0, 3, msg.gnss_time_standard);

    chunk_cb(buffer, 3, ctx);





    chunk_cb(null, 13, ctx);





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

            canardEncodeScalar(buffer, 0, 32, msg.ned_velocity[i]);

            chunk_cb(buffer, 32, ctx);


    }





    memset(buffer,0,8);

    canardEncodeScalar(buffer, 0, 6, msg.sats_used);

    chunk_cb(buffer, 6, ctx);





    memset(buffer,0,8);

    canardEncodeScalar(buffer, 0, 2, msg.status);

    chunk_cb(buffer, 2, ctx);





    memset(buffer,0,8);

    canardEncodeScalar(buffer, 0, 4, msg.mode);

    chunk_cb(buffer, 4, ctx);





    memset(buffer,0,8);

    canardEncodeScalar(buffer, 0, 6, msg.sub_mode);

    chunk_cb(buffer, 6, ctx);







    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 6, msg.covariance_len);
    chunk_cb(buffer, 6, ctx);

    for (int i=0; i < msg.covariance_len; i++) {



            memset(buffer,0,8);

            {
                uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.covariance[i]);
                canardEncodeScalar(buffer, 0, 16, float16_val);
            }

            chunk_cb(buffer, 16, ctx);


    }





    memset(buffer,0,8);

    {
        uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.pdop);
        canardEncodeScalar(buffer, 0, 16, float16_val);
    }

    chunk_cb(buffer, 16, ctx);







    if (!tao) {


        memset(buffer,0,8);
        canardEncodeScalar(buffer, 0, 1, msg.ecef_position_velocity_len);
        chunk_cb(buffer, 1, ctx);


    }

    for (int i=0; i < msg.ecef_position_velocity_len; i++) {



            _encode_uavcan_equipment_gnss_ECEFPositionVelocity(buffer, msg.ecef_position_velocity[i], chunk_cb, ctx, false);


    }





}

static void _decode_uavcan_equipment_gnss_Fix2(CanardRxTransfer transfer,ref uint32_t bit_ofs, uavcan_equipment_gnss_Fix2 msg, bool tao) {






    _decode_uavcan_Timestamp(transfer, ref bit_ofs, msg.timestamp, false);






    _decode_uavcan_Timestamp(transfer, ref bit_ofs, msg.gnss_timestamp, false);








    canardDecodeScalar(transfer, bit_ofs, 3, false, ref msg.gnss_time_standard);


    bit_ofs += 3;






    bit_ofs += 13;








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




        canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.ned_velocity[i]);

        bit_ofs += 32;


    }









    canardDecodeScalar(transfer, bit_ofs, 6, false, ref msg.sats_used);


    bit_ofs += 6;








    canardDecodeScalar(transfer, bit_ofs, 2, false, ref msg.status);


    bit_ofs += 2;








    canardDecodeScalar(transfer, bit_ofs, 4, false, ref msg.mode);


    bit_ofs += 4;








    canardDecodeScalar(transfer, bit_ofs, 6, false, ref msg.sub_mode);


    bit_ofs += 6;








    canardDecodeScalar(transfer, bit_ofs, 6, false, ref msg.covariance_len);
    bit_ofs += 6;


    for (int i=0; i < msg.covariance_len; i++) {




        {
            uint16_t float16_val = 0;
            canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
            msg.covariance[i] = canardConvertFloat16ToNativeFloat(float16_val);
        }

        bit_ofs += 16;


    }








    {
        uint16_t float16_val = 0;
        canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
        msg.pdop = canardConvertFloat16ToNativeFloat(float16_val);
    }

    bit_ofs += 16;








    if (!tao) {


        canardDecodeScalar(transfer, bit_ofs, 1, false, ref msg.ecef_position_velocity_len);
        bit_ofs += 1;



    }




    if (tao) {

msg.ecef_position_velocity_len = 0;
        while (((transfer.payload_len*8)-bit_ofs) > 0) {

            _decode_uavcan_equipment_gnss_ECEFPositionVelocity(transfer, ref bit_ofs, msg.ecef_position_velocity[msg.ecef_position_velocity_len], false);
            msg.ecef_position_velocity_len++;

        }

    } else {


        for (int i=0; i < msg.ecef_position_velocity_len; i++) {



            _decode_uavcan_equipment_gnss_ECEFPositionVelocity(transfer, ref bit_ofs, msg.ecef_position_velocity[i], false);


        }


    }






}

}
}