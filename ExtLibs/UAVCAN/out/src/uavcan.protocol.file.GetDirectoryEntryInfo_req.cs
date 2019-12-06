


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

static uavcan_message_descriptor_s uavcan_protocol_file_GetDirectoryEntryInfo_req_descriptor = {
    UAVCAN_PROTOCOL_FILE_GETDIRECTORYENTRYINFO_REQ_DT_SIG,
    UAVCAN_PROTOCOL_FILE_GETDIRECTORYENTRYINFO_REQ_DT_ID,

    CanardTransferTypeRequest,

    sizeof(uavcan_protocol_file_GetDirectoryEntryInfo_req),
    UAVCAN_PROTOCOL_FILE_GETDIRECTORYENTRYINFO_REQ_MAX_PACK_SIZE,
    encode_func,
    decode_func,

    &uavcan_protocol_file_GetDirectoryEntryInfo_res_descriptor

};
*/


static void encode_uavcan_protocol_file_GetDirectoryEntryInfo_req(uavcan_protocol_file_GetDirectoryEntryInfo_req msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
    uint8_t[] buffer = new uint8_t[8];
    _encode_uavcan_protocol_file_GetDirectoryEntryInfo_req(buffer, msg, chunk_cb, ctx, true);
}

static uint32_t decode_uavcan_protocol_file_GetDirectoryEntryInfo_req(CanardRxTransfer transfer, uavcan_protocol_file_GetDirectoryEntryInfo_req msg) {
    uint32_t bit_ofs = 0;
    _decode_uavcan_protocol_file_GetDirectoryEntryInfo_req(transfer, ref bit_ofs, msg, true);
    return (bit_ofs+7)/8;
}

static void _encode_uavcan_protocol_file_GetDirectoryEntryInfo_req(uint8_t[] buffer, uavcan_protocol_file_GetDirectoryEntryInfo_req msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {






    memset(buffer,0,8);

    canardEncodeScalar(buffer, 0, 32, msg.entry_index);

    chunk_cb(buffer, 32, ctx);





    _encode_uavcan_protocol_file_Path(buffer, msg.directory_path, chunk_cb, ctx, tao);





}

static void _decode_uavcan_protocol_file_GetDirectoryEntryInfo_req(CanardRxTransfer transfer,ref uint32_t bit_ofs, uavcan_protocol_file_GetDirectoryEntryInfo_req msg, bool tao) {








    canardDecodeScalar(transfer, bit_ofs, 32, false, ref msg.entry_index);


    bit_ofs += 32;






    _decode_uavcan_protocol_file_Path(transfer, ref bit_ofs, msg.directory_path, tao);





}

}
}