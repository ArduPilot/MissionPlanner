
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



static void encode_uavcan_equipment_indication_RGB565(uavcan_equipment_indication_RGB565 msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
    uint8_t[] buffer = new uint8_t[8];
    _encode_uavcan_equipment_indication_RGB565(buffer, msg, chunk_cb, ctx, true);
}

static uint32_t decode_uavcan_equipment_indication_RGB565(CanardRxTransfer transfer, uavcan_equipment_indication_RGB565 msg) {
    uint32_t bit_ofs = 0;
    _decode_uavcan_equipment_indication_RGB565(transfer, ref bit_ofs, msg, true);
    return (bit_ofs+7)/8;
}

static void _encode_uavcan_equipment_indication_RGB565(uint8_t[] buffer, uavcan_equipment_indication_RGB565 msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {

    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 5, msg.red);
    chunk_cb(buffer, 5, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 6, msg.green);
    chunk_cb(buffer, 6, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 5, msg.blue);
    chunk_cb(buffer, 5, ctx);
}

static void _decode_uavcan_equipment_indication_RGB565(CanardRxTransfer transfer,ref uint32_t bit_ofs, uavcan_equipment_indication_RGB565 msg, bool tao) {

    canardDecodeScalar(transfer, bit_ofs, 5, false, ref msg.red);
    bit_ofs += 5;

    canardDecodeScalar(transfer, bit_ofs, 6, false, ref msg.green);
    bit_ofs += 6;

    canardDecodeScalar(transfer, bit_ofs, 5, false, ref msg.blue);
    bit_ofs += 5;

}

}
}