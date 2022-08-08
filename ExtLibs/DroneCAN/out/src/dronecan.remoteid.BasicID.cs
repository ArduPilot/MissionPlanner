
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
using System.Collections.Generic;

namespace DroneCAN
{
    public partial class DroneCAN {
        static void encode_dronecan_remoteid_BasicID(dronecan_remoteid_BasicID msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
            uint8_t[] buffer = new uint8_t[8];
            _encode_dronecan_remoteid_BasicID(buffer, msg, chunk_cb, ctx, true);
        }

        static uint32_t decode_dronecan_remoteid_BasicID(CanardRxTransfer transfer, dronecan_remoteid_BasicID msg) {
            uint32_t bit_ofs = 0;
            _decode_dronecan_remoteid_BasicID(transfer, ref bit_ofs, msg, true);
            return (bit_ofs+7)/8;
        }

        static void _encode_dronecan_remoteid_BasicID(uint8_t[] buffer, dronecan_remoteid_BasicID msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 5, msg.id_or_mac_len);
            chunk_cb(buffer, 5, ctx);
            for (int i=0; i < msg.id_or_mac_len; i++) {
                    memset(buffer,0,8);
                    canardEncodeScalar(buffer, 0, 8, msg.id_or_mac[i]);
                    chunk_cb(buffer, 8, ctx);
            }
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 8, msg.id_type);
            chunk_cb(buffer, 8, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 8, msg.ua_type);
            chunk_cb(buffer, 8, ctx);
            if (!tao) {
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 5, msg.uas_id_len);
                chunk_cb(buffer, 5, ctx);
            }
            for (int i=0; i < msg.uas_id_len; i++) {
                    memset(buffer,0,8);
                    canardEncodeScalar(buffer, 0, 8, msg.uas_id[i]);
                    chunk_cb(buffer, 8, ctx);
            }
        }

        static void _decode_dronecan_remoteid_BasicID(CanardRxTransfer transfer,ref uint32_t bit_ofs, dronecan_remoteid_BasicID msg, bool tao) {

            canardDecodeScalar(transfer, bit_ofs, 5, false, ref msg.id_or_mac_len);
            bit_ofs += 5;
            msg.id_or_mac = new uint8_t[msg.id_or_mac_len];
            for (int i=0; i < msg.id_or_mac_len; i++) {
                canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.id_or_mac[i]);
                bit_ofs += 8;
            }

            canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.id_type);
            bit_ofs += 8;

            canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.ua_type);
            bit_ofs += 8;

            if (!tao) {
                canardDecodeScalar(transfer, bit_ofs, 5, false, ref msg.uas_id_len);
                bit_ofs += 5;
            } else {
                msg.uas_id_len = (uint8_t)(((transfer.payload_len*8)-bit_ofs)/8);
            }

            msg.uas_id = new uint8_t[msg.uas_id_len];
            for (int i=0; i < msg.uas_id_len; i++) {
                canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.uas_id[i]);
                bit_ofs += 8;
            }

        }
    }
}