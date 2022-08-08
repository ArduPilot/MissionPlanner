
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
        static void encode_dronecan_remoteid_Location(dronecan_remoteid_Location msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
            uint8_t[] buffer = new uint8_t[8];
            _encode_dronecan_remoteid_Location(buffer, msg, chunk_cb, ctx, true);
        }

        static uint32_t decode_dronecan_remoteid_Location(CanardRxTransfer transfer, dronecan_remoteid_Location msg) {
            uint32_t bit_ofs = 0;
            _decode_dronecan_remoteid_Location(transfer, ref bit_ofs, msg, true);
            return (bit_ofs+7)/8;
        }

        static void _encode_dronecan_remoteid_Location(uint8_t[] buffer, dronecan_remoteid_Location msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 5, msg.id_or_mac_len);
            chunk_cb(buffer, 5, ctx);
            for (int i=0; i < msg.id_or_mac_len; i++) {
                    memset(buffer,0,8);
                    canardEncodeScalar(buffer, 0, 8, msg.id_or_mac[i]);
                    chunk_cb(buffer, 8, ctx);
            }
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 8, msg.status);
            chunk_cb(buffer, 8, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 16, msg.direction);
            chunk_cb(buffer, 16, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 16, msg.speed_horizontal);
            chunk_cb(buffer, 16, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 16, msg.speed_vertical);
            chunk_cb(buffer, 16, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 32, msg.latitude);
            chunk_cb(buffer, 32, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 32, msg.longitude);
            chunk_cb(buffer, 32, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 32, msg.altitude_barometric);
            chunk_cb(buffer, 32, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 32, msg.altitude_geodetic);
            chunk_cb(buffer, 32, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 8, msg.height_reference);
            chunk_cb(buffer, 8, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 32, msg.height);
            chunk_cb(buffer, 32, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 8, msg.horizontal_accuracy);
            chunk_cb(buffer, 8, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 8, msg.vertical_accuracy);
            chunk_cb(buffer, 8, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 8, msg.barometer_accuracy);
            chunk_cb(buffer, 8, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 8, msg.speed_accuracy);
            chunk_cb(buffer, 8, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 32, msg.timestamp);
            chunk_cb(buffer, 32, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 8, msg.timestamp_accuracy);
            chunk_cb(buffer, 8, ctx);
        }

        static void _decode_dronecan_remoteid_Location(CanardRxTransfer transfer,ref uint32_t bit_ofs, dronecan_remoteid_Location msg, bool tao) {

            canardDecodeScalar(transfer, bit_ofs, 5, false, ref msg.id_or_mac_len);
            bit_ofs += 5;
            msg.id_or_mac = new uint8_t[msg.id_or_mac_len];
            for (int i=0; i < msg.id_or_mac_len; i++) {
                canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.id_or_mac[i]);
                bit_ofs += 8;
            }

            canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.status);
            bit_ofs += 8;

            canardDecodeScalar(transfer, bit_ofs, 16, false, ref msg.direction);
            bit_ofs += 16;

            canardDecodeScalar(transfer, bit_ofs, 16, false, ref msg.speed_horizontal);
            bit_ofs += 16;

            canardDecodeScalar(transfer, bit_ofs, 16, true, ref msg.speed_vertical);
            bit_ofs += 16;

            canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.latitude);
            bit_ofs += 32;

            canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.longitude);
            bit_ofs += 32;

            canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.altitude_barometric);
            bit_ofs += 32;

            canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.altitude_geodetic);
            bit_ofs += 32;

            canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.height_reference);
            bit_ofs += 8;

            canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.height);
            bit_ofs += 32;

            canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.horizontal_accuracy);
            bit_ofs += 8;

            canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.vertical_accuracy);
            bit_ofs += 8;

            canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.barometer_accuracy);
            bit_ofs += 8;

            canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.speed_accuracy);
            bit_ofs += 8;

            canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.timestamp);
            bit_ofs += 32;

            canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.timestamp_accuracy);
            bit_ofs += 8;

        }
    }
}