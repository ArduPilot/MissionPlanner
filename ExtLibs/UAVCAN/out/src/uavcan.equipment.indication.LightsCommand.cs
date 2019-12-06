


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

static uavcan_message_descriptor_s uavcan_equipment_indication_LightsCommand_descriptor = {
    UAVCAN_EQUIPMENT_INDICATION_LIGHTSCOMMAND_DT_SIG,
    UAVCAN_EQUIPMENT_INDICATION_LIGHTSCOMMAND_DT_ID,

    CanardTransferTypeBroadcast,

    sizeof(uavcan_equipment_indication_LightsCommand),
    UAVCAN_EQUIPMENT_INDICATION_LIGHTSCOMMAND_MAX_PACK_SIZE,
    encode_func,
    decode_func,

    null

};
*/


static void encode_uavcan_equipment_indication_LightsCommand(uavcan_equipment_indication_LightsCommand msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
    uint8_t[] buffer = new uint8_t[8];
    _encode_uavcan_equipment_indication_LightsCommand(buffer, msg, chunk_cb, ctx, true);
}

static uint32_t decode_uavcan_equipment_indication_LightsCommand(CanardRxTransfer transfer, uavcan_equipment_indication_LightsCommand msg) {
    uint32_t bit_ofs = 0;
    _decode_uavcan_equipment_indication_LightsCommand(transfer, ref bit_ofs, msg, true);
    return (bit_ofs+7)/8;
}

static void _encode_uavcan_equipment_indication_LightsCommand(uint8_t[] buffer, uavcan_equipment_indication_LightsCommand msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {








    if (!tao) {


        memset(buffer,0,8);
        canardEncodeScalar(buffer, 0, 5, msg.commands_len);
        chunk_cb(buffer, 5, ctx);


    }

    for (int i=0; i < msg.commands_len; i++) {



            _encode_uavcan_equipment_indication_SingleLightCommand(buffer, msg.commands[i], chunk_cb, ctx, false);


    }





}

static void _decode_uavcan_equipment_indication_LightsCommand(CanardRxTransfer transfer,ref uint32_t bit_ofs, uavcan_equipment_indication_LightsCommand msg, bool tao) {








    if (!tao) {


        canardDecodeScalar(transfer, bit_ofs, 5, false, ref msg.commands_len);
        bit_ofs += 5;



    }




    if (tao) {

msg.commands_len = 0;
        while (((transfer.payload_len*8)-bit_ofs) > 0) {

            _decode_uavcan_equipment_indication_SingleLightCommand(transfer, ref bit_ofs, msg.commands[msg.commands_len], false);
            msg.commands_len++;

        }

    } else {


        for (int i=0; i < msg.commands_len; i++) {



            _decode_uavcan_equipment_indication_SingleLightCommand(transfer, ref bit_ofs, msg.commands[i], false);


        }


    }






}

}
}