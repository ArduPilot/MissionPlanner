
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

static uavcan_message_descriptor_s com_hex_equipment_gnss_BodyPosition_descriptor = {
    COM_HEX_EQUIPMENT_GNSS_BODYPOSITION_DT_SIG,
    COM_HEX_EQUIPMENT_GNSS_BODYPOSITION_DT_ID,
    CanardTransferTypeBroadcast,
    sizeof(com_hex_equipment_gnss_BodyPosition),
    COM_HEX_EQUIPMENT_GNSS_BODYPOSITION_MAX_PACK_SIZE,
    encode_func,
    decode_func,
    null
};
*/

static void encode_com_hex_equipment_gnss_BodyPosition(com_hex_equipment_gnss_BodyPosition msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
    uint8_t[] buffer = new uint8_t[8];
    _encode_com_hex_equipment_gnss_BodyPosition(buffer, msg, chunk_cb, ctx, true);
}

static uint32_t decode_com_hex_equipment_gnss_BodyPosition(CanardRxTransfer transfer, com_hex_equipment_gnss_BodyPosition msg) {
    uint32_t bit_ofs = 0;
    _decode_com_hex_equipment_gnss_BodyPosition(transfer, ref bit_ofs, msg, true);
    return (bit_ofs+7)/8;
}

static void _encode_com_hex_equipment_gnss_BodyPosition(uint8_t[] buffer, com_hex_equipment_gnss_BodyPosition msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {

    for (int i=0; i < 3; i++) {
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 18, msg.body_pos_mm[i]);
            chunk_cb(buffer, 18, ctx);
    }
}

static void _decode_com_hex_equipment_gnss_BodyPosition(CanardRxTransfer transfer,ref uint32_t bit_ofs, com_hex_equipment_gnss_BodyPosition msg, bool tao) {

/*['__doc__', '__init__', '__module__', '__repr__', '__str__', 'get_normalized_definition', 'name', 'type']*/
    for (int i=0; i < 3; i++) {
        canardDecodeScalar(transfer, bit_ofs, 18, true, ref msg.body_pos_mm[i]);
        bit_ofs += 18;
    }

}

}
}