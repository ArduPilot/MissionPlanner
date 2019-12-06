


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

static uavcan_message_descriptor_s uavcan_equipment_esc_RawCommand_descriptor = {
    UAVCAN_EQUIPMENT_ESC_RAWCOMMAND_DT_SIG,
    UAVCAN_EQUIPMENT_ESC_RAWCOMMAND_DT_ID,

    CanardTransferTypeBroadcast,

    sizeof(uavcan_equipment_esc_RawCommand),
    UAVCAN_EQUIPMENT_ESC_RAWCOMMAND_MAX_PACK_SIZE,
    encode_func,
    decode_func,

    null

};
*/


static void encode_uavcan_equipment_esc_RawCommand(uavcan_equipment_esc_RawCommand msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
    uint8_t[] buffer = new uint8_t[8];
    _encode_uavcan_equipment_esc_RawCommand(buffer, msg, chunk_cb, ctx, true);
}

static uint32_t decode_uavcan_equipment_esc_RawCommand(CanardRxTransfer transfer, uavcan_equipment_esc_RawCommand msg) {
    uint32_t bit_ofs = 0;
    _decode_uavcan_equipment_esc_RawCommand(transfer, ref bit_ofs, msg, true);
    return (bit_ofs+7)/8;
}

static void _encode_uavcan_equipment_esc_RawCommand(uint8_t[] buffer, uavcan_equipment_esc_RawCommand msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {








    if (!tao) {


        memset(buffer,0,8);
        canardEncodeScalar(buffer, 0, 5, msg.cmd_len);
        chunk_cb(buffer, 5, ctx);


    }

    for (int i=0; i < msg.cmd_len; i++) {



            memset(buffer,0,8);

            canardEncodeScalar(buffer, 0, 14, msg.cmd[i]);

            chunk_cb(buffer, 14, ctx);


    }





}

static void _decode_uavcan_equipment_esc_RawCommand(CanardRxTransfer transfer,ref uint32_t bit_ofs, uavcan_equipment_esc_RawCommand msg, bool tao) {








    if (!tao) {


        canardDecodeScalar(transfer, bit_ofs, 5, false, ref msg.cmd_len);
        bit_ofs += 5;



    } else {

        msg.cmd_len = (uint8_t)(((transfer.payload_len*8)-bit_ofs)/14);


    }



    for (int i=0; i < msg.cmd_len; i++) {




        canardDecodeScalar(transfer, bit_ofs, 14, true, ref msg.cmd[i]);

        bit_ofs += 14;


    }






}

}
}