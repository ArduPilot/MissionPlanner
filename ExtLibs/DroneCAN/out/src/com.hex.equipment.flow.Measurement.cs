
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
        static void encode_com_hex_equipment_flow_Measurement(com_hex_equipment_flow_Measurement msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
            uint8_t[] buffer = new uint8_t[8];
            _encode_com_hex_equipment_flow_Measurement(buffer, msg, chunk_cb, ctx, true);
        }

        static uint32_t decode_com_hex_equipment_flow_Measurement(CanardRxTransfer transfer, com_hex_equipment_flow_Measurement msg) {
            uint32_t bit_ofs = 0;
            _decode_com_hex_equipment_flow_Measurement(transfer, ref bit_ofs, msg, true);
            return (bit_ofs+7)/8;
        }

        static void _encode_com_hex_equipment_flow_Measurement(uint8_t[] buffer, com_hex_equipment_flow_Measurement msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 32, msg.integration_interval);
            chunk_cb(buffer, 32, ctx);
            for (int i=0; i < 2; i++) {
                    memset(buffer,0,8);
                    canardEncodeScalar(buffer, 0, 32, msg.rate_gyro_integral[i]);
                    chunk_cb(buffer, 32, ctx);
            }
            for (int i=0; i < 2; i++) {
                    memset(buffer,0,8);
                    canardEncodeScalar(buffer, 0, 32, msg.flow_integral[i]);
                    chunk_cb(buffer, 32, ctx);
            }
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 8, msg.quality);
            chunk_cb(buffer, 8, ctx);
        }

        static void _decode_com_hex_equipment_flow_Measurement(CanardRxTransfer transfer,ref uint32_t bit_ofs, com_hex_equipment_flow_Measurement msg, bool tao) {

            canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.integration_interval);
            bit_ofs += 32;

            for (int i=0; i < 2; i++) {
                canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.rate_gyro_integral[i]);
                bit_ofs += 32;
            }

            for (int i=0; i < 2; i++) {
                canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.flow_integral[i]);
                bit_ofs += 32;
            }

            canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.quality);
            bit_ofs += 8;

        }
    }
}