
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

static uavcan_message_descriptor_s com_hex_equipment_gpio_InputStateArray_descriptor = {
    COM_HEX_EQUIPMENT_GPIO_INPUTSTATEARRAY_DT_SIG,
    COM_HEX_EQUIPMENT_GPIO_INPUTSTATEARRAY_DT_ID,
    CanardTransferTypeBroadcast,
    sizeof(com_hex_equipment_gpio_InputStateArray),
    COM_HEX_EQUIPMENT_GPIO_INPUTSTATEARRAY_MAX_PACK_SIZE,
    encode_func,
    decode_func,
    null
};
*/

static void encode_com_hex_equipment_gpio_InputStateArray(com_hex_equipment_gpio_InputStateArray msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
    uint8_t[] buffer = new uint8_t[8];
    _encode_com_hex_equipment_gpio_InputStateArray(buffer, msg, chunk_cb, ctx, true);
}

static uint32_t decode_com_hex_equipment_gpio_InputStateArray(CanardRxTransfer transfer, com_hex_equipment_gpio_InputStateArray msg) {
    uint32_t bit_ofs = 0;
    _decode_com_hex_equipment_gpio_InputStateArray(transfer, ref bit_ofs, msg, true);
    return (bit_ofs+7)/8;
}

static void _encode_com_hex_equipment_gpio_InputStateArray(uint8_t[] buffer, com_hex_equipment_gpio_InputStateArray msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {

    if (!tao) {
        memset(buffer,0,8);
        canardEncodeScalar(buffer, 0, 6, msg.input_states_len);
        chunk_cb(buffer, 6, ctx);
    }
    for (int i=0; i < msg.input_states_len; i++) {
            _encode_com_hex_equipment_gpio_InputState(buffer, msg.input_states[i], chunk_cb, ctx, false);
    }
}

static void _decode_com_hex_equipment_gpio_InputStateArray(CanardRxTransfer transfer,ref uint32_t bit_ofs, com_hex_equipment_gpio_InputStateArray msg, bool tao) {

    if (!tao) {
        canardDecodeScalar(transfer, bit_ofs, 6, false, ref msg.input_states_len);
        bit_ofs += 6;
    }


    if (tao) {
msg.input_states_len = 0;
        while (((transfer.payload_len*8)-bit_ofs) > 0) {
            _decode_com_hex_equipment_gpio_InputState(transfer, ref bit_ofs, msg.input_states[msg.input_states_len], false);
            msg.input_states_len++;
        }
    } else {
        msg.input_states = new com_hex_equipment_gpio_InputState[msg.input_states_len];
        for (int i=0; i < msg.input_states_len; i++) {
            _decode_com_hex_equipment_gpio_InputState(transfer, ref bit_ofs, msg.input_states[i], false);
        }
    }

}

}
}