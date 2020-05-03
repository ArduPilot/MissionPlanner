
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

static uavcan_message_descriptor_s uavcan_equipment_camera_gimbal_GEOPOICommand_descriptor = {
    UAVCAN_EQUIPMENT_CAMERA_GIMBAL_GEOPOICOMMAND_DT_SIG,
    UAVCAN_EQUIPMENT_CAMERA_GIMBAL_GEOPOICOMMAND_DT_ID,
    CanardTransferTypeBroadcast,
    sizeof(uavcan_equipment_camera_gimbal_GEOPOICommand),
    UAVCAN_EQUIPMENT_CAMERA_GIMBAL_GEOPOICOMMAND_MAX_PACK_SIZE,
    encode_func,
    decode_func,
    null
};
*/

static void encode_uavcan_equipment_camera_gimbal_GEOPOICommand(uavcan_equipment_camera_gimbal_GEOPOICommand msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
    uint8_t[] buffer = new uint8_t[8];
    _encode_uavcan_equipment_camera_gimbal_GEOPOICommand(buffer, msg, chunk_cb, ctx, true);
}

static uint32_t decode_uavcan_equipment_camera_gimbal_GEOPOICommand(CanardRxTransfer transfer, uavcan_equipment_camera_gimbal_GEOPOICommand msg) {
    uint32_t bit_ofs = 0;
    _decode_uavcan_equipment_camera_gimbal_GEOPOICommand(transfer, ref bit_ofs, msg, true);
    return (bit_ofs+7)/8;
}

static void _encode_uavcan_equipment_camera_gimbal_GEOPOICommand(uint8_t[] buffer, uavcan_equipment_camera_gimbal_GEOPOICommand msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {

    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 8, msg.gimbal_id);
    chunk_cb(buffer, 8, ctx);
    _encode_uavcan_equipment_camera_gimbal_Mode(buffer, msg.mode, chunk_cb, ctx, false);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 32, msg.longitude_deg_1e7);
    chunk_cb(buffer, 32, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 32, msg.latitude_deg_1e7);
    chunk_cb(buffer, 32, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 22, msg.height_cm);
    chunk_cb(buffer, 22, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 2, msg.height_reference);
    chunk_cb(buffer, 2, ctx);
}

static void _decode_uavcan_equipment_camera_gimbal_GEOPOICommand(CanardRxTransfer transfer,ref uint32_t bit_ofs, uavcan_equipment_camera_gimbal_GEOPOICommand msg, bool tao) {

    canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.gimbal_id);
    bit_ofs += 8;

    _decode_uavcan_equipment_camera_gimbal_Mode(transfer, ref bit_ofs, msg.mode, false);

    canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.longitude_deg_1e7);
    bit_ofs += 32;

    canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.latitude_deg_1e7);
    bit_ofs += 32;

    canardDecodeScalar(transfer, bit_ofs, 22, true, ref msg.height_cm);
    bit_ofs += 22;

    canardDecodeScalar(transfer, bit_ofs, 2, false, ref msg.height_reference);
    bit_ofs += 2;

}

}
}