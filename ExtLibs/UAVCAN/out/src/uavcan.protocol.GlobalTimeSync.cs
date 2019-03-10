


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

static uavcan_message_descriptor_s uavcan_protocol_GlobalTimeSync_descriptor = {
    UAVCAN_PROTOCOL_GLOBALTIMESYNC_DT_SIG,
    UAVCAN_PROTOCOL_GLOBALTIMESYNC_DT_ID,

    CanardTransferTypeBroadcast,

    sizeof(uavcan_protocol_GlobalTimeSync),
    UAVCAN_PROTOCOL_GLOBALTIMESYNC_MAX_PACK_SIZE,
    encode_func,
    decode_func,

    null

};
*/


static void encode_uavcan_protocol_GlobalTimeSync(uavcan_protocol_GlobalTimeSync msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
    uint8_t[] buffer = new uint8_t[8];
    _encode_uavcan_protocol_GlobalTimeSync(buffer, msg, chunk_cb, ctx, true);
}

static uint32_t decode_uavcan_protocol_GlobalTimeSync(CanardRxTransfer transfer, uavcan_protocol_GlobalTimeSync msg) {
    uint32_t bit_ofs = 0;
    _decode_uavcan_protocol_GlobalTimeSync(transfer, ref bit_ofs, msg, true);
    return (bit_ofs+7)/8;
}

static void _encode_uavcan_protocol_GlobalTimeSync(uint8_t[] buffer, uavcan_protocol_GlobalTimeSync msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {






    memset(buffer,0,8);

    canardEncodeScalar(buffer, 0, 56, msg.previous_transmission_timestamp_usec);

    chunk_cb(buffer, 56, ctx);





}

static void _decode_uavcan_protocol_GlobalTimeSync(CanardRxTransfer transfer,ref uint32_t bit_ofs, uavcan_protocol_GlobalTimeSync msg, bool tao) {








    canardDecodeScalar(transfer, bit_ofs, 56, false, ref msg.previous_transmission_timestamp_usec);


    bit_ofs += 56;





}

}
}