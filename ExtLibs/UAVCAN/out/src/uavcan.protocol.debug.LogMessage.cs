


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

static uavcan_message_descriptor_s uavcan_protocol_debug_LogMessage_descriptor = {
    UAVCAN_PROTOCOL_DEBUG_LOGMESSAGE_DT_SIG,
    UAVCAN_PROTOCOL_DEBUG_LOGMESSAGE_DT_ID,

    CanardTransferTypeBroadcast,

    sizeof(uavcan_protocol_debug_LogMessage),
    UAVCAN_PROTOCOL_DEBUG_LOGMESSAGE_MAX_PACK_SIZE,
    encode_func,
    decode_func,

    null

};
*/


static void encode_uavcan_protocol_debug_LogMessage(uavcan_protocol_debug_LogMessage msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
    uint8_t[] buffer = new uint8_t[8];
    _encode_uavcan_protocol_debug_LogMessage(buffer, msg, chunk_cb, ctx, true);
}

static uint32_t decode_uavcan_protocol_debug_LogMessage(CanardRxTransfer transfer, uavcan_protocol_debug_LogMessage msg) {
    uint32_t bit_ofs = 0;
    _decode_uavcan_protocol_debug_LogMessage(transfer, ref bit_ofs, msg, true);
    return (bit_ofs+7)/8;
}

static void _encode_uavcan_protocol_debug_LogMessage(uint8_t[] buffer, uavcan_protocol_debug_LogMessage msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {






    _encode_uavcan_protocol_debug_LogLevel(buffer, msg.level, chunk_cb, ctx, false);







    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 5, msg.source_len);
    chunk_cb(buffer, 5, ctx);

    for (int i=0; i < msg.source_len; i++) {



            memset(buffer,0,8);

            canardEncodeScalar(buffer, 0, 8, msg.source[i]);

            chunk_cb(buffer, 8, ctx);


    }







    if (!tao) {


        memset(buffer,0,8);
        canardEncodeScalar(buffer, 0, 7, msg.text_len);
        chunk_cb(buffer, 7, ctx);


    }

    for (int i=0; i < msg.text_len; i++) {



            memset(buffer,0,8);

            canardEncodeScalar(buffer, 0, 8, msg.text[i]);

            chunk_cb(buffer, 8, ctx);


    }





}

static void _decode_uavcan_protocol_debug_LogMessage(CanardRxTransfer transfer,ref uint32_t bit_ofs, uavcan_protocol_debug_LogMessage msg, bool tao) {






    _decode_uavcan_protocol_debug_LogLevel(transfer, ref bit_ofs, msg.level, false);








    canardDecodeScalar(transfer, bit_ofs, 5, false, ref msg.source_len);
    bit_ofs += 5;


    for (int i=0; i < msg.source_len; i++) {




        canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.source[i]);

        bit_ofs += 8;


    }









    if (!tao) {


        canardDecodeScalar(transfer, bit_ofs, 7, false, ref msg.text_len);
        bit_ofs += 7;



    } else {

        msg.text_len = (uint8_t)(((transfer.payload_len*8)-bit_ofs)/8);


    }



    for (int i=0; i < msg.text_len; i++) {




        canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.text[i]);

        bit_ofs += 8;


    }






}

}
}