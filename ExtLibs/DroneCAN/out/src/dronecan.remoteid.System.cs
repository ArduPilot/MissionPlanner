
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
        static void encode_dronecan_remoteid_System(dronecan_remoteid_System msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
            uint8_t[] buffer = new uint8_t[8];
            _encode_dronecan_remoteid_System(buffer, msg, chunk_cb, ctx, true);
        }

        static uint32_t decode_dronecan_remoteid_System(CanardRxTransfer transfer, dronecan_remoteid_System msg) {
            uint32_t bit_ofs = 0;
            _decode_dronecan_remoteid_System(transfer, ref bit_ofs, msg, true);
            return (bit_ofs+7)/8;
        }

        static void _encode_dronecan_remoteid_System(uint8_t[] buffer, dronecan_remoteid_System msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 5, msg.id_or_mac_len);
            chunk_cb(buffer, 5, ctx);
            for (int i=0; i < msg.id_or_mac_len; i++) {
                    memset(buffer,0,8);
                    canardEncodeScalar(buffer, 0, 8, msg.id_or_mac[i]);
                    chunk_cb(buffer, 8, ctx);
            }
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 8, msg.operator_location_type);
            chunk_cb(buffer, 8, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 8, msg.classification_type);
            chunk_cb(buffer, 8, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 32, msg.operator_latitude);
            chunk_cb(buffer, 32, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 32, msg.operator_longitude);
            chunk_cb(buffer, 32, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 16, msg.area_count);
            chunk_cb(buffer, 16, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 16, msg.area_radius);
            chunk_cb(buffer, 16, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 32, msg.area_ceiling);
            chunk_cb(buffer, 32, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 32, msg.area_floor);
            chunk_cb(buffer, 32, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 8, msg.category_eu);
            chunk_cb(buffer, 8, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 8, msg.class_eu);
            chunk_cb(buffer, 8, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 32, msg.operator_altitude_geo);
            chunk_cb(buffer, 32, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 32, msg.timestamp);
            chunk_cb(buffer, 32, ctx);
        }

        static void _decode_dronecan_remoteid_System(CanardRxTransfer transfer,ref uint32_t bit_ofs, dronecan_remoteid_System msg, bool tao) {

            canardDecodeScalar(transfer, bit_ofs, 5, false, ref msg.id_or_mac_len);
            bit_ofs += 5;
            msg.id_or_mac = new uint8_t[msg.id_or_mac_len];
            for (int i=0; i < msg.id_or_mac_len; i++) {
                canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.id_or_mac[i]);
                bit_ofs += 8;
            }

            canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.operator_location_type);
            bit_ofs += 8;

            canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.classification_type);
            bit_ofs += 8;

            canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.operator_latitude);
            bit_ofs += 32;

            canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.operator_longitude);
            bit_ofs += 32;

            canardDecodeScalar(transfer, bit_ofs, 16, false, ref msg.area_count);
            bit_ofs += 16;

            canardDecodeScalar(transfer, bit_ofs, 16, false, ref msg.area_radius);
            bit_ofs += 16;

            canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.area_ceiling);
            bit_ofs += 32;

            canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.area_floor);
            bit_ofs += 32;

            canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.category_eu);
            bit_ofs += 8;

            canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.class_eu);
            bit_ofs += 8;

            canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.operator_altitude_geo);
            bit_ofs += 32;

            canardDecodeScalar(transfer, bit_ofs, 32, false, ref msg.timestamp);
            bit_ofs += 32;

        }
    }
}