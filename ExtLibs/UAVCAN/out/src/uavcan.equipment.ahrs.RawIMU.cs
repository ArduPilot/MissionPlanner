
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

static uavcan_message_descriptor_s uavcan_equipment_ahrs_RawIMU_descriptor = {
    UAVCAN_EQUIPMENT_AHRS_RAWIMU_DT_SIG,
    UAVCAN_EQUIPMENT_AHRS_RAWIMU_DT_ID,
    CanardTransferTypeBroadcast,
    sizeof(uavcan_equipment_ahrs_RawIMU),
    UAVCAN_EQUIPMENT_AHRS_RAWIMU_MAX_PACK_SIZE,
    encode_func,
    decode_func,
    null
};
*/

static void encode_uavcan_equipment_ahrs_RawIMU(uavcan_equipment_ahrs_RawIMU msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
    uint8_t[] buffer = new uint8_t[8];
    _encode_uavcan_equipment_ahrs_RawIMU(buffer, msg, chunk_cb, ctx, true);
}

static uint32_t decode_uavcan_equipment_ahrs_RawIMU(CanardRxTransfer transfer, uavcan_equipment_ahrs_RawIMU msg) {
    uint32_t bit_ofs = 0;
    _decode_uavcan_equipment_ahrs_RawIMU(transfer, ref bit_ofs, msg, true);
    return (bit_ofs+7)/8;
}

static void _encode_uavcan_equipment_ahrs_RawIMU(uint8_t[] buffer, uavcan_equipment_ahrs_RawIMU msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {

    _encode_uavcan_Timestamp(buffer, msg.timestamp, chunk_cb, ctx, false);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 32, msg.integration_interval);
    chunk_cb(buffer, 32, ctx);
    for (int i=0; i < 3; i++) {
            memset(buffer,0,8);
            {
                uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.rate_gyro_latest[i]);
                canardEncodeScalar(buffer, 0, 16, float16_val);
            }
            chunk_cb(buffer, 16, ctx);
    }
    for (int i=0; i < 3; i++) {
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 32, msg.rate_gyro_integral[i]);
            chunk_cb(buffer, 32, ctx);
    }
    for (int i=0; i < 3; i++) {
            memset(buffer,0,8);
            {
                uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.accelerometer_latest[i]);
                canardEncodeScalar(buffer, 0, 16, float16_val);
            }
            chunk_cb(buffer, 16, ctx);
    }
    for (int i=0; i < 3; i++) {
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 32, msg.accelerometer_integral[i]);
            chunk_cb(buffer, 32, ctx);
    }
    if (!tao) {
        memset(buffer,0,8);
        canardEncodeScalar(buffer, 0, 6, msg.covariance_len);
        chunk_cb(buffer, 6, ctx);
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

static void _decode_uavcan_equipment_ahrs_RawIMU(CanardRxTransfer transfer,ref uint32_t bit_ofs, uavcan_equipment_ahrs_RawIMU msg, bool tao) {

    _decode_uavcan_Timestamp(transfer, ref bit_ofs, msg.timestamp, false);

    canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.integration_interval);
    bit_ofs += 32;

/*['__doc__', '__init__', '__module__', '__repr__', '__str__', 'get_normalized_definition', 'name', 'type']*/
    for (int i=0; i < 3; i++) {
        {
            uint16_t float16_val = 0;
            canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
            msg.rate_gyro_latest[i] = canardConvertFloat16ToNativeFloat(float16_val);
        }
        bit_ofs += 16;
    }

/*['__doc__', '__init__', '__module__', '__repr__', '__str__', 'get_normalized_definition', 'name', 'type']*/
    for (int i=0; i < 3; i++) {
        canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.rate_gyro_integral[i]);
        bit_ofs += 32;
    }

/*['__doc__', '__init__', '__module__', '__repr__', '__str__', 'get_normalized_definition', 'name', 'type']*/
    for (int i=0; i < 3; i++) {
        {
            uint16_t float16_val = 0;
            canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
            msg.accelerometer_latest[i] = canardConvertFloat16ToNativeFloat(float16_val);
        }
        bit_ofs += 16;
    }

/*['__doc__', '__init__', '__module__', '__repr__', '__str__', 'get_normalized_definition', 'name', 'type']*/
    for (int i=0; i < 3; i++) {
        canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.accelerometer_integral[i]);
        bit_ofs += 32;
    }

    if (!tao) {
        canardDecodeScalar(transfer, bit_ofs, 6, false, ref msg.covariance_len);
        bit_ofs += 6;
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