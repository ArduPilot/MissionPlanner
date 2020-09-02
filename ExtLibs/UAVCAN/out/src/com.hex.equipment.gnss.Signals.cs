
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

static uavcan_message_descriptor_s com_hex_equipment_gnss_Signals_descriptor = {
    COM_HEX_EQUIPMENT_GNSS_SIGNALS_DT_SIG,
    COM_HEX_EQUIPMENT_GNSS_SIGNALS_DT_ID,
    CanardTransferTypeBroadcast,
    sizeof(com_hex_equipment_gnss_Signals),
    COM_HEX_EQUIPMENT_GNSS_SIGNALS_MAX_PACK_SIZE,
    encode_func,
    decode_func,
    null
};
*/

static void encode_com_hex_equipment_gnss_Signals(com_hex_equipment_gnss_Signals msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
    uint8_t[] buffer = new uint8_t[8];
    _encode_com_hex_equipment_gnss_Signals(buffer, msg, chunk_cb, ctx, true);
}

static uint32_t decode_com_hex_equipment_gnss_Signals(CanardRxTransfer transfer, com_hex_equipment_gnss_Signals msg) {
    uint32_t bit_ofs = 0;
    _decode_com_hex_equipment_gnss_Signals(transfer, ref bit_ofs, msg, true);
    return (bit_ofs+7)/8;
}

static void _encode_com_hex_equipment_gnss_Signals(uint8_t[] buffer, com_hex_equipment_gnss_Signals msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {

    _encode_uavcan_Timestamp(buffer, msg.timestamp, chunk_cb, ctx, false);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 32, msg.iTOW);
    chunk_cb(buffer, 32, ctx);
    if (!tao) {
        memset(buffer,0,8);
        canardEncodeScalar(buffer, 0, 7, msg.signals_len);
        chunk_cb(buffer, 7, ctx);
    }
    for (int i=0; i < msg.signals_len; i++) {
            _encode_com_hex_equipment_gnss_Signal(buffer, msg.signals[i], chunk_cb, ctx, false);
    }
}

static void _decode_com_hex_equipment_gnss_Signals(CanardRxTransfer transfer,ref uint32_t bit_ofs, com_hex_equipment_gnss_Signals msg, bool tao) {

    _decode_uavcan_Timestamp(transfer, ref bit_ofs, msg.timestamp, false);

    canardDecodeScalar(transfer, bit_ofs, 32, false, ref msg.iTOW);
    bit_ofs += 32;

    if (!tao) {
        canardDecodeScalar(transfer, bit_ofs, 7, false, ref msg.signals_len);
        bit_ofs += 7;
    }


    if (tao) {
msg.signals_len = 0;
        while (((transfer.payload_len*8)-bit_ofs) > 0) {
            _decode_com_hex_equipment_gnss_Signal(transfer, ref bit_ofs, msg.signals[msg.signals_len], false);
            msg.signals_len++;
        }
    } else {
        msg.signals = new com_hex_equipment_gnss_Signal[msg.signals_len];
        for (int i=0; i < msg.signals_len; i++) {
            _decode_com_hex_equipment_gnss_Signal(transfer, ref bit_ofs, msg.signals[i], false);
        }
    }

}

}
}