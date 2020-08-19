
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

static uavcan_message_descriptor_s com_hex_equipment_gpio_GetInputStates_res_descriptor = {
    COM_HEX_EQUIPMENT_GPIO_GETINPUTSTATES_RES_DT_SIG,
    COM_HEX_EQUIPMENT_GPIO_GETINPUTSTATES_RES_DT_ID,
    CanardTransferTypeResponse,
    sizeof(com_hex_equipment_gpio_GetInputStates_res),
    COM_HEX_EQUIPMENT_GPIO_GETINPUTSTATES_RES_MAX_PACK_SIZE,
    encode_func,
    decode_func,
    null
};
*/

static void encode_com_hex_equipment_gpio_GetInputStates_res(com_hex_equipment_gpio_GetInputStates_res msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
    uint8_t[] buffer = new uint8_t[8];
    _encode_com_hex_equipment_gpio_GetInputStates_res(buffer, msg, chunk_cb, ctx, true);
}

static uint32_t decode_com_hex_equipment_gpio_GetInputStates_res(CanardRxTransfer transfer, com_hex_equipment_gpio_GetInputStates_res msg) {
    uint32_t bit_ofs = 0;
    _decode_com_hex_equipment_gpio_GetInputStates_res(transfer, ref bit_ofs, msg, true);
    return (bit_ofs+7)/8;
}

static void _encode_com_hex_equipment_gpio_GetInputStates_res(uint8_t[] buffer, com_hex_equipment_gpio_GetInputStates_res msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {

    _encode_com_hex_equipment_gpio_InputStateArray(buffer, msg.input_state_array, chunk_cb, ctx, tao);
}

static void _decode_com_hex_equipment_gpio_GetInputStates_res(CanardRxTransfer transfer,ref uint32_t bit_ofs, com_hex_equipment_gpio_GetInputStates_res msg, bool tao) {

    _decode_com_hex_equipment_gpio_InputStateArray(transfer, ref bit_ofs, msg.input_state_array, tao);

}

}
}