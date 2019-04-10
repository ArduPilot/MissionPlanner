
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



static void encode_uavcan_CoarseOrientation(uavcan_CoarseOrientation msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
    uint8_t[] buffer = new uint8_t[8];
    _encode_uavcan_CoarseOrientation(buffer, msg, chunk_cb, ctx, true);
}

static uint32_t decode_uavcan_CoarseOrientation(CanardRxTransfer transfer, uavcan_CoarseOrientation msg) {
    uint32_t bit_ofs = 0;
    _decode_uavcan_CoarseOrientation(transfer, ref bit_ofs, msg, true);
    return (bit_ofs+7)/8;
}

static void _encode_uavcan_CoarseOrientation(uint8_t[] buffer, uavcan_CoarseOrientation msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {

    for (int i=0; i < 3; i++) {
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 5, msg.fixed_axis_roll_pitch_yaw[i]);
            chunk_cb(buffer, 5, ctx);
    }
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 1, msg.orientation_defined);
    chunk_cb(buffer, 1, ctx);
}

static void _decode_uavcan_CoarseOrientation(CanardRxTransfer transfer,ref uint32_t bit_ofs, uavcan_CoarseOrientation msg, bool tao) {

/*['__doc__', '__init__', '__module__', '__repr__', '__str__', 'get_normalized_definition', 'name', 'type']*/
    for (int i=0; i < 3; i++) {
        canardDecodeScalar(transfer, bit_ofs, 5, true, ref msg.fixed_axis_roll_pitch_yaw[i]);
        bit_ofs += 5;
    }

    canardDecodeScalar(transfer, bit_ofs, 1, false, ref msg.orientation_defined);
    bit_ofs += 1;

}

}
}