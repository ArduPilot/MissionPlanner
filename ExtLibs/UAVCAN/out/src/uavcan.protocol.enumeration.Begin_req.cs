
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

static uavcan_message_descriptor_s uavcan_protocol_enumeration_Begin_req_descriptor = {
    UAVCAN_PROTOCOL_ENUMERATION_BEGIN_REQ_DT_SIG,
    UAVCAN_PROTOCOL_ENUMERATION_BEGIN_REQ_DT_ID,
    CanardTransferTypeRequest,
    sizeof(uavcan_protocol_enumeration_Begin_req),
    UAVCAN_PROTOCOL_ENUMERATION_BEGIN_REQ_MAX_PACK_SIZE,
    encode_func,
    decode_func,
    &uavcan_protocol_enumeration_Begin_res_descriptor
};
*/

static void encode_uavcan_protocol_enumeration_Begin_req(uavcan_protocol_enumeration_Begin_req msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
    uint8_t[] buffer = new uint8_t[8];
    _encode_uavcan_protocol_enumeration_Begin_req(buffer, msg, chunk_cb, ctx, true);
}

static uint32_t decode_uavcan_protocol_enumeration_Begin_req(CanardRxTransfer transfer, uavcan_protocol_enumeration_Begin_req msg) {
    uint32_t bit_ofs = 0;
    _decode_uavcan_protocol_enumeration_Begin_req(transfer, ref bit_ofs, msg, true);
    return (bit_ofs+7)/8;
}

static void _encode_uavcan_protocol_enumeration_Begin_req(uint8_t[] buffer, uavcan_protocol_enumeration_Begin_req msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {

    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 16, msg.timeout_sec);
    chunk_cb(buffer, 16, ctx);
    if (!tao) {
        memset(buffer,0,8);
        canardEncodeScalar(buffer, 0, 7, msg.parameter_name_len);
        chunk_cb(buffer, 7, ctx);
    }
    for (int i=0; i < msg.parameter_name_len; i++) {
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 8, msg.parameter_name[i]);
            chunk_cb(buffer, 8, ctx);
    }
}

static void _decode_uavcan_protocol_enumeration_Begin_req(CanardRxTransfer transfer,ref uint32_t bit_ofs, uavcan_protocol_enumeration_Begin_req msg, bool tao) {

    canardDecodeScalar(transfer, bit_ofs, 16, false, ref msg.timeout_sec);
    bit_ofs += 16;

    if (!tao) {
        canardDecodeScalar(transfer, bit_ofs, 7, false, ref msg.parameter_name_len);
        bit_ofs += 7;
    } else {
        msg.parameter_name_len = (uint8_t)(((transfer.payload_len*8)-bit_ofs)/8);
    }

    for (int i=0; i < msg.parameter_name_len; i++) {
        canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.parameter_name[i]);
        bit_ofs += 8;
    }

}

}
}