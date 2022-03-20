
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
        static void encode_uavcan_olliw_uc4h_Distance(uavcan_olliw_uc4h_Distance msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
            uint8_t[] buffer = new uint8_t[8];
            _encode_uavcan_olliw_uc4h_Distance(buffer, msg, chunk_cb, ctx, true);
        }

        static uint32_t decode_uavcan_olliw_uc4h_Distance(CanardRxTransfer transfer, uavcan_olliw_uc4h_Distance msg) {
            uint32_t bit_ofs = 0;
            _decode_uavcan_olliw_uc4h_Distance(transfer, ref bit_ofs, msg, true);
            return (bit_ofs+7)/8;
        }

        static void _encode_uavcan_olliw_uc4h_Distance(uint8_t[] buffer, uavcan_olliw_uc4h_Distance msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 4, msg.fixed_axis_pitch);
            chunk_cb(buffer, 4, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 5, msg.fixed_axis_yaw);
            chunk_cb(buffer, 5, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 4, msg.sensor_sub_id);
            chunk_cb(buffer, 4, ctx);
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 3, msg.range_flag);
            chunk_cb(buffer, 3, ctx);
            memset(buffer,0,8);
            {
                uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.range);
                canardEncodeScalar(buffer, 0, 16, float16_val);
            }
            chunk_cb(buffer, 16, ctx);
            if (!tao) {
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 1, msg.sensor_property_len);
                chunk_cb(buffer, 1, ctx);
            }
            for (int i=0; i < msg.sensor_property_len; i++) {
                    _encode_uavcan_olliw_uc4h_DistanceSensorProperties(buffer, msg.sensor_property[i], chunk_cb, ctx, false);
            }
        }

        static void _decode_uavcan_olliw_uc4h_Distance(CanardRxTransfer transfer,ref uint32_t bit_ofs, uavcan_olliw_uc4h_Distance msg, bool tao) {

            canardDecodeScalar(transfer, bit_ofs, 4, true, ref msg.fixed_axis_pitch);
            bit_ofs += 4;

            canardDecodeScalar(transfer, bit_ofs, 5, true, ref msg.fixed_axis_yaw);
            bit_ofs += 5;

            canardDecodeScalar(transfer, bit_ofs, 4, false, ref msg.sensor_sub_id);
            bit_ofs += 4;

            canardDecodeScalar(transfer, bit_ofs, 3, false, ref msg.range_flag);
            bit_ofs += 3;

            {
                uint16_t float16_val = 0;
                canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
                msg.range = canardConvertFloat16ToNativeFloat(float16_val);
            }
            bit_ofs += 16;

            if (!tao) {
                canardDecodeScalar(transfer, bit_ofs, 1, false, ref msg.sensor_property_len);
                bit_ofs += 1;
            }


            if (tao) {
                msg.sensor_property_len = 0;
                var temp = new List<uavcan_olliw_uc4h_DistanceSensorProperties>();
                while (((transfer.payload_len*8)-bit_ofs) > 0) {
                    msg.sensor_property_len++;
                    temp.Add(new uavcan_olliw_uc4h_DistanceSensorProperties());
                    _decode_uavcan_olliw_uc4h_DistanceSensorProperties(transfer, ref bit_ofs, temp[msg.sensor_property_len - 1], false);
                }
                msg.sensor_property = temp.ToArray();
            } else {
                msg.sensor_property = new uavcan_olliw_uc4h_DistanceSensorProperties[msg.sensor_property_len];
                for (int i=0; i < msg.sensor_property_len; i++) {
                    _decode_uavcan_olliw_uc4h_DistanceSensorProperties(transfer, ref bit_ofs, msg.sensor_property[i], false);
                }
            }

        }
    }
}