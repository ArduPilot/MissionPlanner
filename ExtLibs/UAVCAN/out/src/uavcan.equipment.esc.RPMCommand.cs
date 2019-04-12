


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

static uavcan_message_descriptor_s uavcan_equipment_esc_RPMCommand_descriptor = {
    UAVCAN_EQUIPMENT_ESC_RPMCOMMAND_DT_SIG,
    UAVCAN_EQUIPMENT_ESC_RPMCOMMAND_DT_ID,

    CanardTransferTypeBroadcast,

    sizeof(uavcan_equipment_esc_RPMCommand),
    UAVCAN_EQUIPMENT_ESC_RPMCOMMAND_MAX_PACK_SIZE,
    encode_func,
    decode_func,

    null

};
*/


static void encode_uavcan_equipment_esc_RPMCommand(uavcan_equipment_esc_RPMCommand msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
    uint8_t[] buffer = new uint8_t[8];
    _encode_uavcan_equipment_esc_RPMCommand(buffer, msg, chunk_cb, ctx, true);
}

static uint32_t decode_uavcan_equipment_esc_RPMCommand(CanardRxTransfer transfer, uavcan_equipment_esc_RPMCommand msg) {
    uint32_t bit_ofs = 0;
    _decode_uavcan_equipment_esc_RPMCommand(transfer, ref bit_ofs, msg, true);
    return (bit_ofs+7)/8;
}

static void _encode_uavcan_equipment_esc_RPMCommand(uint8_t[] buffer, uavcan_equipment_esc_RPMCommand msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {








    if (!tao) {


        memset(buffer,0,8);
        canardEncodeScalar(buffer, 0, 5, msg.rpm_len);
        chunk_cb(buffer, 5, ctx);


    }

    for (int i=0; i < msg.rpm_len; i++) {



            memset(buffer,0,8);

            canardEncodeScalar(buffer, 0, 18, msg.rpm[i]);

            chunk_cb(buffer, 18, ctx);


    }





}

static void _decode_uavcan_equipment_esc_RPMCommand(CanardRxTransfer transfer,ref uint32_t bit_ofs, uavcan_equipment_esc_RPMCommand msg, bool tao) {








    if (!tao) {


        canardDecodeScalar(transfer, bit_ofs, 5, false, ref msg.rpm_len);
        bit_ofs += 5;



    } else {

        msg.rpm_len = (uint8_t)(((transfer.payload_len*8)-bit_ofs)/18);


    }



    for (int i=0; i < msg.rpm_len; i++) {




        canardDecodeScalar(transfer, bit_ofs, 18, true, ref msg.rpm[i]);

        bit_ofs += 18;


    }






}

}
}