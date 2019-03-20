
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



static void encode_uavcan_protocol_SoftwareVersion(uavcan_protocol_SoftwareVersion msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
    uint8_t[] buffer = new uint8_t[8];
    _encode_uavcan_protocol_SoftwareVersion(buffer, msg, chunk_cb, ctx, true);
}

static uint32_t decode_uavcan_protocol_SoftwareVersion(CanardRxTransfer transfer, uavcan_protocol_SoftwareVersion msg) {
    uint32_t bit_ofs = 0;
    _decode_uavcan_protocol_SoftwareVersion(transfer, ref bit_ofs, msg, true);
    return (bit_ofs+7)/8;
}

static void _encode_uavcan_protocol_SoftwareVersion(uint8_t[] buffer, uavcan_protocol_SoftwareVersion msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {

    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 8, msg.major);
    chunk_cb(buffer, 8, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 8, msg.minor);
    chunk_cb(buffer, 8, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 8, msg.optional_field_flags);
    chunk_cb(buffer, 8, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 32, msg.vcs_commit);
    chunk_cb(buffer, 32, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 64, msg.image_crc);
    chunk_cb(buffer, 64, ctx);
}

static void _decode_uavcan_protocol_SoftwareVersion(CanardRxTransfer transfer,ref uint32_t bit_ofs, uavcan_protocol_SoftwareVersion msg, bool tao) {

    canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.major);
    bit_ofs += 8;

    canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.minor);
    bit_ofs += 8;

    canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.optional_field_flags);
    bit_ofs += 8;

    canardDecodeScalar(transfer, bit_ofs, 32, false, ref msg.vcs_commit);
    bit_ofs += 32;

    canardDecodeScalar(transfer, bit_ofs, 64, false, ref msg.image_crc);
    bit_ofs += 64;

}

}
}