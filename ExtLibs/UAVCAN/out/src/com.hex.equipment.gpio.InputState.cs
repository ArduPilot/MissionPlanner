
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



static void encode_com_hex_equipment_gpio_InputState(com_hex_equipment_gpio_InputState msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
    uint8_t[] buffer = new uint8_t[8];
    _encode_com_hex_equipment_gpio_InputState(buffer, msg, chunk_cb, ctx, true);
}

static uint32_t decode_com_hex_equipment_gpio_InputState(CanardRxTransfer transfer, com_hex_equipment_gpio_InputState msg) {
    uint32_t bit_ofs = 0;
    _decode_com_hex_equipment_gpio_InputState(transfer, ref bit_ofs, msg, true);
    return (bit_ofs+7)/8;
}

static void _encode_com_hex_equipment_gpio_InputState(uint8_t[] buffer, com_hex_equipment_gpio_InputState msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {

    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 7, msg.input_idx);
    chunk_cb(buffer, 7, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 1, msg.binary_input);
    chunk_cb(buffer, 1, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 32, msg.input_value);
    chunk_cb(buffer, 32, ctx);
}

static void _decode_com_hex_equipment_gpio_InputState(CanardRxTransfer transfer,ref uint32_t bit_ofs, com_hex_equipment_gpio_InputState msg, bool tao) {

    canardDecodeScalar(transfer, bit_ofs, 7, false, ref msg.input_idx);
    bit_ofs += 7;

    canardDecodeScalar(transfer, bit_ofs, 1, false, ref msg.binary_input);
    bit_ofs += 1;

    canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.input_value);
    bit_ofs += 32;

}

}
}