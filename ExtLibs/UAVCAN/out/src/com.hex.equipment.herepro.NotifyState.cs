
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

static uavcan_message_descriptor_s com_hex_equipment_herepro_NotifyState_descriptor = {
    COM_HEX_EQUIPMENT_HEREPRO_NOTIFYSTATE_DT_SIG,
    COM_HEX_EQUIPMENT_HEREPRO_NOTIFYSTATE_DT_ID,
    CanardTransferTypeBroadcast,
    sizeof(com_hex_equipment_herepro_NotifyState),
    COM_HEX_EQUIPMENT_HEREPRO_NOTIFYSTATE_MAX_PACK_SIZE,
    encode_func,
    decode_func,
    null
};
*/

static void encode_com_hex_equipment_herepro_NotifyState(com_hex_equipment_herepro_NotifyState msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
    uint8_t[] buffer = new uint8_t[8];
    _encode_com_hex_equipment_herepro_NotifyState(buffer, msg, chunk_cb, ctx, true);
}

static uint32_t decode_com_hex_equipment_herepro_NotifyState(CanardRxTransfer transfer, com_hex_equipment_herepro_NotifyState msg) {
    uint32_t bit_ofs = 0;
    _decode_com_hex_equipment_herepro_NotifyState(transfer, ref bit_ofs, msg, true);
    return (bit_ofs+7)/8;
}

static void _encode_com_hex_equipment_herepro_NotifyState(uint8_t[] buffer, com_hex_equipment_herepro_NotifyState msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {

    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 64, msg.vehicle_state);
    chunk_cb(buffer, 64, ctx);
    memset(buffer,0,8);
    {
        uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.yaw_earth);
        canardEncodeScalar(buffer, 0, 16, float16_val);
    }
    chunk_cb(buffer, 16, ctx);
}

static void _decode_com_hex_equipment_herepro_NotifyState(CanardRxTransfer transfer,ref uint32_t bit_ofs, com_hex_equipment_herepro_NotifyState msg, bool tao) {

    canardDecodeScalar(transfer, bit_ofs, 64, false, ref msg.vehicle_state);
    bit_ofs += 64;

    {
        uint16_t float16_val = 0;
        canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
        msg.yaw_earth = canardConvertFloat16ToNativeFloat(float16_val);
    }
    bit_ofs += 16;

}

}
}