
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

static uavcan_message_descriptor_s ardupilot_equipment_trafficmonitor_TrafficReport_descriptor = {
    ARDUPILOT_EQUIPMENT_TRAFFICMONITOR_TRAFFICREPORT_DT_SIG,
    ARDUPILOT_EQUIPMENT_TRAFFICMONITOR_TRAFFICREPORT_DT_ID,
    CanardTransferTypeBroadcast,
    sizeof(ardupilot_equipment_trafficmonitor_TrafficReport),
    ARDUPILOT_EQUIPMENT_TRAFFICMONITOR_TRAFFICREPORT_MAX_PACK_SIZE,
    encode_func,
    decode_func,
    null
};
*/

static void encode_ardupilot_equipment_trafficmonitor_TrafficReport(ardupilot_equipment_trafficmonitor_TrafficReport msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
    uint8_t[] buffer = new uint8_t[8];
    _encode_ardupilot_equipment_trafficmonitor_TrafficReport(buffer, msg, chunk_cb, ctx, true);
}

static uint32_t decode_ardupilot_equipment_trafficmonitor_TrafficReport(CanardRxTransfer transfer, ardupilot_equipment_trafficmonitor_TrafficReport msg) {
    uint32_t bit_ofs = 0;
    _decode_ardupilot_equipment_trafficmonitor_TrafficReport(transfer, ref bit_ofs, msg, true);
    return (bit_ofs+7)/8;
}

static void _encode_ardupilot_equipment_trafficmonitor_TrafficReport(uint8_t[] buffer, ardupilot_equipment_trafficmonitor_TrafficReport msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {

    _encode_uavcan_Timestamp(buffer, msg.timestamp, chunk_cb, ctx, false);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 32, msg.icao_address);
    chunk_cb(buffer, 32, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 16, msg.tslc);
    chunk_cb(buffer, 16, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 32, msg.latitude_deg_1e7);
    chunk_cb(buffer, 32, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 32, msg.longitude_deg_1e7);
    chunk_cb(buffer, 32, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 32, msg.alt_m);
    chunk_cb(buffer, 32, ctx);
    memset(buffer,0,8);
    {
        uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.heading);
        canardEncodeScalar(buffer, 0, 16, float16_val);
    }
    chunk_cb(buffer, 16, ctx);
    for (int i=0; i < 3; i++) {
            memset(buffer,0,8);
            {
                uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.velocity[i]);
                canardEncodeScalar(buffer, 0, 16, float16_val);
            }
            chunk_cb(buffer, 16, ctx);
    }
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 16, msg.squawk);
    chunk_cb(buffer, 16, ctx);
    for (int i=0; i < 9; i++) {
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 8, msg.callsign[i]);
            chunk_cb(buffer, 8, ctx);
    }
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 3, msg.source);
    chunk_cb(buffer, 3, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 5, msg.traffic_type);
    chunk_cb(buffer, 5, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 7, msg.alt_type);
    chunk_cb(buffer, 7, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 1, msg.lat_lon_valid);
    chunk_cb(buffer, 1, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 1, msg.heading_valid);
    chunk_cb(buffer, 1, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 1, msg.velocity_valid);
    chunk_cb(buffer, 1, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 1, msg.callsign_valid);
    chunk_cb(buffer, 1, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 1, msg.ident_valid);
    chunk_cb(buffer, 1, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 1, msg.simulated_report);
    chunk_cb(buffer, 1, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 1, msg.vertical_velocity_valid);
    chunk_cb(buffer, 1, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 1, msg.baro_valid);
    chunk_cb(buffer, 1, ctx);
}

static void _decode_ardupilot_equipment_trafficmonitor_TrafficReport(CanardRxTransfer transfer,ref uint32_t bit_ofs, ardupilot_equipment_trafficmonitor_TrafficReport msg, bool tao) {

    _decode_uavcan_Timestamp(transfer, ref bit_ofs, msg.timestamp, false);

    canardDecodeScalar(transfer, bit_ofs, 32, false, ref msg.icao_address);
    bit_ofs += 32;

    canardDecodeScalar(transfer, bit_ofs, 16, false, ref msg.tslc);
    bit_ofs += 16;

    canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.latitude_deg_1e7);
    bit_ofs += 32;

    canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.longitude_deg_1e7);
    bit_ofs += 32;

    canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.alt_m);
    bit_ofs += 32;

    {
        uint16_t float16_val = 0;
        canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
        msg.heading = canardConvertFloat16ToNativeFloat(float16_val);
    }
    bit_ofs += 16;

/*['__doc__', '__init__', '__module__', '__repr__', '__str__', 'get_normalized_definition', 'name', 'type']*/
    for (int i=0; i < 3; i++) {
        {
            uint16_t float16_val = 0;
            canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
            msg.velocity[i] = canardConvertFloat16ToNativeFloat(float16_val);
        }
        bit_ofs += 16;
    }

    canardDecodeScalar(transfer, bit_ofs, 16, false, ref msg.squawk);
    bit_ofs += 16;

/*['__doc__', '__init__', '__module__', '__repr__', '__str__', 'get_normalized_definition', 'name', 'type']*/
    for (int i=0; i < 9; i++) {
        canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.callsign[i]);
        bit_ofs += 8;
    }

    canardDecodeScalar(transfer, bit_ofs, 3, false, ref msg.source);
    bit_ofs += 3;

    canardDecodeScalar(transfer, bit_ofs, 5, false, ref msg.traffic_type);
    bit_ofs += 5;

    canardDecodeScalar(transfer, bit_ofs, 7, false, ref msg.alt_type);
    bit_ofs += 7;

    canardDecodeScalar(transfer, bit_ofs, 1, false, ref msg.lat_lon_valid);
    bit_ofs += 1;

    canardDecodeScalar(transfer, bit_ofs, 1, false, ref msg.heading_valid);
    bit_ofs += 1;

    canardDecodeScalar(transfer, bit_ofs, 1, false, ref msg.velocity_valid);
    bit_ofs += 1;

    canardDecodeScalar(transfer, bit_ofs, 1, false, ref msg.callsign_valid);
    bit_ofs += 1;

    canardDecodeScalar(transfer, bit_ofs, 1, false, ref msg.ident_valid);
    bit_ofs += 1;

    canardDecodeScalar(transfer, bit_ofs, 1, false, ref msg.simulated_report);
    bit_ofs += 1;

    canardDecodeScalar(transfer, bit_ofs, 1, false, ref msg.vertical_velocity_valid);
    bit_ofs += 1;

    canardDecodeScalar(transfer, bit_ofs, 1, false, ref msg.baro_valid);
    bit_ofs += 1;

}

}
}