
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

static uavcan_message_descriptor_s org_cubepilot_uwb_Observation_descriptor = {
    ORG_CUBEPILOT_UWB_OBSERVATION_DT_SIG,
    ORG_CUBEPILOT_UWB_OBSERVATION_DT_ID,
    CanardTransferTypeBroadcast,
    sizeof(org_cubepilot_uwb_Observation),
    ORG_CUBEPILOT_UWB_OBSERVATION_MAX_PACK_SIZE,
    encode_func,
    decode_func,
    null
};
*/

static void encode_org_cubepilot_uwb_Observation(org_cubepilot_uwb_Observation msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
    uint8_t[] buffer = new uint8_t[8];
    _encode_org_cubepilot_uwb_Observation(buffer, msg, chunk_cb, ctx, true);
}

static uint32_t decode_org_cubepilot_uwb_Observation(CanardRxTransfer transfer, org_cubepilot_uwb_Observation msg) {
    uint32_t bit_ofs = 0;
    _decode_org_cubepilot_uwb_Observation(transfer, ref bit_ofs, msg, true);
    return (bit_ofs+7)/8;
}

static void _encode_org_cubepilot_uwb_Observation(uint8_t[] buffer, org_cubepilot_uwb_Observation msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {

    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 56, msg.timestamp_us);
    chunk_cb(buffer, 56, ctx);
    _encode_org_cubepilot_uwb_Node(buffer, msg.tx_node, chunk_cb, ctx, false);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 40, msg.tx_timestamp);
    chunk_cb(buffer, 40, ctx);
    if (!tao) {
        memset(buffer,0,8);
        canardEncodeScalar(buffer, 0, 4, msg.rx_timestamps_len);
        chunk_cb(buffer, 4, ctx);
    }
    for (int i=0; i < msg.rx_timestamps_len; i++) {
            _encode_org_cubepilot_uwb_ReceiveTimestamp(buffer, msg.rx_timestamps[i], chunk_cb, ctx, false);
    }
}

static void _decode_org_cubepilot_uwb_Observation(CanardRxTransfer transfer,ref uint32_t bit_ofs, org_cubepilot_uwb_Observation msg, bool tao) {

    canardDecodeScalar(transfer, bit_ofs, 56, false, ref msg.timestamp_us);
    bit_ofs += 56;

    _decode_org_cubepilot_uwb_Node(transfer, ref bit_ofs, msg.tx_node, false);

    canardDecodeScalar(transfer, bit_ofs, 40, false, ref msg.tx_timestamp);
    bit_ofs += 40;

    if (!tao) {
        canardDecodeScalar(transfer, bit_ofs, 4, false, ref msg.rx_timestamps_len);
        bit_ofs += 4;
    }


    if (tao) {
msg.rx_timestamps_len = 0;
        while (((transfer.payload_len*8)-bit_ofs) > 0) {
            _decode_org_cubepilot_uwb_ReceiveTimestamp(transfer, ref bit_ofs, msg.rx_timestamps[msg.rx_timestamps_len], false);
            msg.rx_timestamps_len++;
        }
    } else {
        for (int i=0; i < msg.rx_timestamps_len; i++) {
            _decode_org_cubepilot_uwb_ReceiveTimestamp(transfer, ref bit_ofs, msg.rx_timestamps[i], false);
        }
    }

}

}
}