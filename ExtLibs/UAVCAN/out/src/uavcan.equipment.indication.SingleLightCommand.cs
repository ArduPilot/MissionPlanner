
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



static void encode_uavcan_equipment_indication_SingleLightCommand(uavcan_equipment_indication_SingleLightCommand msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
    uint8_t[] buffer = new uint8_t[8];
    _encode_uavcan_equipment_indication_SingleLightCommand(buffer, msg, chunk_cb, ctx, true);
}

static uint32_t decode_uavcan_equipment_indication_SingleLightCommand(CanardRxTransfer transfer, uavcan_equipment_indication_SingleLightCommand msg) {
    uint32_t bit_ofs = 0;
    _decode_uavcan_equipment_indication_SingleLightCommand(transfer, ref bit_ofs, msg, true);
    return (bit_ofs+7)/8;
}

static void _encode_uavcan_equipment_indication_SingleLightCommand(uint8_t[] buffer, uavcan_equipment_indication_SingleLightCommand msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {

    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 8, msg.light_id);
    chunk_cb(buffer, 8, ctx);
    _encode_uavcan_equipment_indication_RGB565(buffer, msg.color, chunk_cb, ctx, tao);
}

static void _decode_uavcan_equipment_indication_SingleLightCommand(CanardRxTransfer transfer,ref uint32_t bit_ofs, uavcan_equipment_indication_SingleLightCommand msg, bool tao) {

    canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.light_id);
    bit_ofs += 8;

    _decode_uavcan_equipment_indication_RGB565(transfer, ref bit_ofs, msg.color, tao);

}

}
}