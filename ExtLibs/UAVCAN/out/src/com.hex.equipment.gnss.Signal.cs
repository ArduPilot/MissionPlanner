
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
using System.Linq;
using System.Runtime.InteropServices;

namespace UAVCAN
{
public partial class uavcan {



static void encode_com_hex_equipment_gnss_Signal(com_hex_equipment_gnss_Signal msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
    uint8_t[] buffer = new uint8_t[8];
    _encode_com_hex_equipment_gnss_Signal(buffer, msg, chunk_cb, ctx, true);
}

static uint32_t decode_com_hex_equipment_gnss_Signal(CanardRxTransfer transfer, com_hex_equipment_gnss_Signal msg) {
    uint32_t bit_ofs = 0;
    _decode_com_hex_equipment_gnss_Signal(transfer, ref bit_ofs, msg, true);
    return (bit_ofs+7)/8;
}

static void _encode_com_hex_equipment_gnss_Signal(uint8_t[] buffer, com_hex_equipment_gnss_Signal msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {

    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 8, msg.sv_id);
    chunk_cb(buffer, 8, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 4, msg.gnss_id);
    chunk_cb(buffer, 4, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 4, msg.sig_id);
    chunk_cb(buffer, 4, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 4, msg.freq_id);
    chunk_cb(buffer, 4, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 4, msg.quality_ind);
    chunk_cb(buffer, 4, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 16, msg.pr_res);
    chunk_cb(buffer, 16, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 8, msg.cno);
    chunk_cb(buffer, 8, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 4, msg.corr_source);
    chunk_cb(buffer, 4, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 4, msg.iono_model);
    chunk_cb(buffer, 4, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 2, msg.health);
    chunk_cb(buffer, 2, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 1, msg.pr_smoothed);
    chunk_cb(buffer, 1, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 1, msg.pr_used);
    chunk_cb(buffer, 1, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 1, msg.cr_used);
    chunk_cb(buffer, 1, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 1, msg.do_used);
    chunk_cb(buffer, 1, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 1, msg.pr_corr_used);
    chunk_cb(buffer, 1, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 1, msg.cr_corr_used);
    chunk_cb(buffer, 1, ctx);
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 1, msg.do_corr_used);
    chunk_cb(buffer, 1, ctx);
}

static void _decode_com_hex_equipment_gnss_Signal(CanardRxTransfer transfer,ref uint32_t bit_ofs, com_hex_equipment_gnss_Signal msg, bool tao) {

    canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.sv_id);
    bit_ofs += 8;

    canardDecodeScalar(transfer, bit_ofs, 4, false, ref msg.gnss_id);
    bit_ofs += 4;

    canardDecodeScalar(transfer, bit_ofs, 4, false, ref msg.sig_id);
    bit_ofs += 4;

    canardDecodeScalar(transfer, bit_ofs, 4, false, ref msg.freq_id);
    bit_ofs += 4;

    canardDecodeScalar(transfer, bit_ofs, 4, false, ref msg.quality_ind);
    bit_ofs += 4;

    canardDecodeScalar(transfer, bit_ofs, 16, true, ref msg.pr_res);
    bit_ofs += 16;

    canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.cno);
    bit_ofs += 8;

    canardDecodeScalar(transfer, bit_ofs, 4, false, ref msg.corr_source);
    bit_ofs += 4;

    canardDecodeScalar(transfer, bit_ofs, 4, false, ref msg.iono_model);
    bit_ofs += 4;

    canardDecodeScalar(transfer, bit_ofs, 2, false, ref msg.health);
    bit_ofs += 2;

    canardDecodeScalar(transfer, bit_ofs, 1, false, ref msg.pr_smoothed);
    bit_ofs += 1;

    canardDecodeScalar(transfer, bit_ofs, 1, false, ref msg.pr_used);
    bit_ofs += 1;

    canardDecodeScalar(transfer, bit_ofs, 1, false, ref msg.cr_used);
    bit_ofs += 1;

    canardDecodeScalar(transfer, bit_ofs, 1, false, ref msg.do_used);
    bit_ofs += 1;

    canardDecodeScalar(transfer, bit_ofs, 1, false, ref msg.pr_corr_used);
    bit_ofs += 1;

    canardDecodeScalar(transfer, bit_ofs, 1, false, ref msg.cr_corr_used);
    bit_ofs += 1;

    canardDecodeScalar(transfer, bit_ofs, 1, false, ref msg.do_corr_used);
    bit_ofs += 1;

}

}
}