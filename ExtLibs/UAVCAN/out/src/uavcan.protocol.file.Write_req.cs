


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

static uavcan_message_descriptor_s uavcan_protocol_file_Write_req_descriptor = {
    UAVCAN_PROTOCOL_FILE_WRITE_REQ_DT_SIG,
    UAVCAN_PROTOCOL_FILE_WRITE_REQ_DT_ID,

    CanardTransferTypeRequest,

    sizeof(uavcan_protocol_file_Write_req),
    UAVCAN_PROTOCOL_FILE_WRITE_REQ_MAX_PACK_SIZE,
    encode_func,
    decode_func,

    &uavcan_protocol_file_Write_res_descriptor

};
*/


static void encode_uavcan_protocol_file_Write_req(uavcan_protocol_file_Write_req msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
    uint8_t[] buffer = new uint8_t[8];
    _encode_uavcan_protocol_file_Write_req(buffer, msg, chunk_cb, ctx, true);
}

static uint32_t decode_uavcan_protocol_file_Write_req(CanardRxTransfer transfer, uavcan_protocol_file_Write_req msg) {
    uint32_t bit_ofs = 0;
    _decode_uavcan_protocol_file_Write_req(transfer, ref bit_ofs, msg, true);
    return (bit_ofs+7)/8;
}

static void _encode_uavcan_protocol_file_Write_req(uint8_t[] buffer, uavcan_protocol_file_Write_req msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {






    memset(buffer,0,8);

    canardEncodeScalar(buffer, 0, 40, msg.offset);

    chunk_cb(buffer, 40, ctx);





    _encode_uavcan_protocol_file_Path(buffer, msg.path, chunk_cb, ctx, false);







    if (!tao) {


        memset(buffer,0,8);
        canardEncodeScalar(buffer, 0, 8, msg.data_len);
        chunk_cb(buffer, 8, ctx);


    }

    for (int i=0; i < msg.data_len; i++) {



            memset(buffer,0,8);

            canardEncodeScalar(buffer, 0, 8, msg.data[i]);

            chunk_cb(buffer, 8, ctx);


    }





}

static void _decode_uavcan_protocol_file_Write_req(CanardRxTransfer transfer,ref uint32_t bit_ofs, uavcan_protocol_file_Write_req msg, bool tao) {








    canardDecodeScalar(transfer, bit_ofs, 40, false, ref msg.offset);


    bit_ofs += 40;






    _decode_uavcan_protocol_file_Path(transfer, ref bit_ofs, msg.path, false);








    if (!tao) {


        canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.data_len);
        bit_ofs += 8;



    } else {

        msg.data_len = (uint8_t)(((transfer.payload_len*8)-bit_ofs)/8);


    }



    for (int i=0; i < msg.data_len; i++) {




        canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.data[i]);

        bit_ofs += 8;


    }






}

}
}