
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
using System.Linq;
using System.Runtime.InteropServices;

namespace UAVCAN
{
public partial class uavcan {



/*

static uavcan_message_descriptor_s org_ardupilot_sensor_output_IMUDeltas_descriptor = {
    ORG_ARDUPILOT_SENSOR_OUTPUT_IMUDELTAS_DT_SIG,
    ORG_ARDUPILOT_SENSOR_OUTPUT_IMUDELTAS_DT_ID,
    CanardTransferTypeBroadcast,
    sizeof(org_ardupilot_sensor_output_IMUDeltas),
    ORG_ARDUPILOT_SENSOR_OUTPUT_IMUDELTAS_MAX_PACK_SIZE,
    encode_func,
    decode_func,
    null
};
*/

static void encode_org_ardupilot_sensor_output_IMUDeltas(org_ardupilot_sensor_output_IMUDeltas msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
    uint8_t[] buffer = new uint8_t[8];
    _encode_org_ardupilot_sensor_output_IMUDeltas(buffer, msg, chunk_cb, ctx, true);
}

static uint32_t decode_org_ardupilot_sensor_output_IMUDeltas(CanardRxTransfer transfer, org_ardupilot_sensor_output_IMUDeltas msg) {
    uint32_t bit_ofs = 0;
    _decode_org_ardupilot_sensor_output_IMUDeltas(transfer, ref bit_ofs, msg, true);
    return (bit_ofs+7)/8;
}

static void _encode_org_ardupilot_sensor_output_IMUDeltas(uint8_t[] buffer, org_ardupilot_sensor_output_IMUDeltas msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {

    _encode_uavcan_Timestamp(buffer, msg.timestamp, chunk_cb, ctx, false);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 8, msg.sensor_index);
    chunk_cb(buffer, 8, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 32, msg.delta_angle_dt);
    chunk_cb(buffer, 32, ctx);
    for (int i=0; i < 3; i++) {
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 32, msg.delta_angle[i]);
            chunk_cb(buffer, 32, ctx);
    }
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 32, msg.delta_velocity_dt);
    chunk_cb(buffer, 32, ctx);
    for (int i=0; i < 3; i++) {
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 32, msg.delta_velocity[i]);
            chunk_cb(buffer, 32, ctx);
    }
}

static void _decode_org_ardupilot_sensor_output_IMUDeltas(CanardRxTransfer transfer,ref uint32_t bit_ofs, org_ardupilot_sensor_output_IMUDeltas msg, bool tao) {

    _decode_uavcan_Timestamp(transfer, ref bit_ofs, msg.timestamp, false);

    canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.sensor_index);
    bit_ofs += 8;

    canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.delta_angle_dt);
    bit_ofs += 32;

/*['__doc__', '__init__', '__module__', '__repr__', '__str__', 'get_normalized_definition', 'name', 'type']*/
    for (int i=0; i < 3; i++) {
        canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.delta_angle[i]);
        bit_ofs += 32;
    }

    canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.delta_velocity_dt);
    bit_ofs += 32;

/*['__doc__', '__init__', '__module__', '__repr__', '__str__', 'get_normalized_definition', 'name', 'type']*/
    for (int i=0; i < 3; i++) {
        canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.delta_velocity[i]);
        bit_ofs += 32;
    }

}

}
}