
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

static uavcan_message_descriptor_s uavcan_equipment_safety_ArmingStatus_descriptor = {
    UAVCAN_EQUIPMENT_SAFETY_ARMINGSTATUS_DT_SIG,
    UAVCAN_EQUIPMENT_SAFETY_ARMINGSTATUS_DT_ID,
    CanardTransferTypeBroadcast,
    sizeof(uavcan_equipment_safety_ArmingStatus),
    UAVCAN_EQUIPMENT_SAFETY_ARMINGSTATUS_MAX_PACK_SIZE,
    encode_func,
    decode_func,
    null
};
*/

static void encode_uavcan_equipment_safety_ArmingStatus(uavcan_equipment_safety_ArmingStatus msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
    uint8_t[] buffer = new uint8_t[8];
    _encode_uavcan_equipment_safety_ArmingStatus(buffer, msg, chunk_cb, ctx, true);
}

static uint32_t decode_uavcan_equipment_safety_ArmingStatus(CanardRxTransfer transfer, uavcan_equipment_safety_ArmingStatus msg) {
    uint32_t bit_ofs = 0;
    _decode_uavcan_equipment_safety_ArmingStatus(transfer, ref bit_ofs, msg, true);
    return (bit_ofs+7)/8;
}

static void _encode_uavcan_equipment_safety_ArmingStatus(uint8_t[] buffer, uavcan_equipment_safety_ArmingStatus msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {

    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 8, msg.status);
    chunk_cb(buffer, 8, ctx);
}

static void _decode_uavcan_equipment_safety_ArmingStatus(CanardRxTransfer transfer,ref uint32_t bit_ofs, uavcan_equipment_safety_ArmingStatus msg, bool tao) {

    canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.status);
    bit_ofs += 8;

}

}
}