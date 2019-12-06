


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




static void encode_org_cubepilot_uwb_ReceiveTimestamp(org_cubepilot_uwb_ReceiveTimestamp msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
    uint8_t[] buffer = new uint8_t[8];
    _encode_org_cubepilot_uwb_ReceiveTimestamp(buffer, msg, chunk_cb, ctx, true);
}

static uint32_t decode_org_cubepilot_uwb_ReceiveTimestamp(CanardRxTransfer transfer, org_cubepilot_uwb_ReceiveTimestamp msg) {
    uint32_t bit_ofs = 0;
    _decode_org_cubepilot_uwb_ReceiveTimestamp(transfer, ref bit_ofs, msg, true);
    return (bit_ofs+7)/8;
}

static void _encode_org_cubepilot_uwb_ReceiveTimestamp(uint8_t[] buffer, org_cubepilot_uwb_ReceiveTimestamp msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {






    _encode_org_cubepilot_uwb_Node(buffer, msg.rx_node, chunk_cb, ctx, false);





    memset(buffer,0,8);

    canardEncodeScalar(buffer, 0, 40, msg.rx_timestamp);

    chunk_cb(buffer, 40, ctx);





}

static void _decode_org_cubepilot_uwb_ReceiveTimestamp(CanardRxTransfer transfer,ref uint32_t bit_ofs, org_cubepilot_uwb_ReceiveTimestamp msg, bool tao) {






    _decode_org_cubepilot_uwb_Node(transfer, ref bit_ofs, msg.rx_node, false);








    canardDecodeScalar(transfer, bit_ofs, 40, false, ref msg.rx_timestamp);


    bit_ofs += 40;





}

}
}