
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

        public partial class com_xacti_GimbalAttitudeStatus : IDroneCANSerialize
        {
            public static void encode_com_xacti_GimbalAttitudeStatus(com_xacti_GimbalAttitudeStatus msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan) {
                uint8_t[] buffer = new uint8_t[8];
                _encode_com_xacti_GimbalAttitudeStatus(buffer, msg, chunk_cb, ctx, !fdcan);
            }

            public static uint32_t decode_com_xacti_GimbalAttitudeStatus(CanardRxTransfer transfer, com_xacti_GimbalAttitudeStatus msg, bool fdcan) {
                uint32_t bit_ofs = 0;
                _decode_com_xacti_GimbalAttitudeStatus(transfer, ref bit_ofs, msg, !fdcan);
                return (bit_ofs+7)/8;
            }

            internal static void _encode_com_xacti_GimbalAttitudeStatus(uint8_t[] buffer, com_xacti_GimbalAttitudeStatus msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 16, msg.gimbal_roll);
                chunk_cb(buffer, 16, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 16, msg.gimbal_pitch);
                chunk_cb(buffer, 16, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 16, msg.gimbal_yaw);
                chunk_cb(buffer, 16, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 16, msg.magneticencoder_roll);
                chunk_cb(buffer, 16, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 16, msg.magneticencoder_pitch);
                chunk_cb(buffer, 16, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 16, msg.magneticencoder_yaw);
                chunk_cb(buffer, 16, ctx);
            }

            internal static void _decode_com_xacti_GimbalAttitudeStatus(CanardRxTransfer transfer,ref uint32_t bit_ofs, com_xacti_GimbalAttitudeStatus msg, bool tao) {

                canardDecodeScalar(transfer, bit_ofs, 16, true, ref msg.gimbal_roll);
                bit_ofs += 16;

                canardDecodeScalar(transfer, bit_ofs, 16, true, ref msg.gimbal_pitch);
                bit_ofs += 16;

                canardDecodeScalar(transfer, bit_ofs, 16, true, ref msg.gimbal_yaw);
                bit_ofs += 16;

                canardDecodeScalar(transfer, bit_ofs, 16, true, ref msg.magneticencoder_roll);
                bit_ofs += 16;

                canardDecodeScalar(transfer, bit_ofs, 16, true, ref msg.magneticencoder_pitch);
                bit_ofs += 16;

                canardDecodeScalar(transfer, bit_ofs, 16, true, ref msg.magneticencoder_yaw);
                bit_ofs += 16;

            }
        }
    }
}