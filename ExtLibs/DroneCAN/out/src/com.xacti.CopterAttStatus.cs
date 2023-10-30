
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

        public partial class com_xacti_CopterAttStatus : IDroneCANSerialize
        {
            public static void encode_com_xacti_CopterAttStatus(com_xacti_CopterAttStatus msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan) {
                uint8_t[] buffer = new uint8_t[8];
                _encode_com_xacti_CopterAttStatus(buffer, msg, chunk_cb, ctx, !fdcan);
            }

            public static uint32_t decode_com_xacti_CopterAttStatus(CanardRxTransfer transfer, com_xacti_CopterAttStatus msg, bool fdcan) {
                uint32_t bit_ofs = 0;
                _decode_com_xacti_CopterAttStatus(transfer, ref bit_ofs, msg, !fdcan);
                return (bit_ofs+7)/8;
            }

            internal static void _encode_com_xacti_CopterAttStatus(uint8_t[] buffer, com_xacti_CopterAttStatus msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {
                for (int i=0; i < 4; i++) {
                        memset(buffer,0,8);
                        canardEncodeScalar(buffer, 0, 16, msg.quaternion_wxyz_e4[i]);
                        chunk_cb(buffer, 16, ctx);
                }
                if (!tao) {
                    memset(buffer,0,8);
                    canardEncodeScalar(buffer, 0, 2, msg.reserved_len);
                    chunk_cb(buffer, 2, ctx);
                }
                for (int i=0; i < msg.reserved_len; i++) {
                        memset(buffer,0,8);
                        {
                            uint16_t float16_val = canardConvertNativeFloatToFloat16(msg.reserved[i]);
                            canardEncodeScalar(buffer, 0, 16, float16_val);
                        }
                        chunk_cb(buffer, 16, ctx);
                }
            }

            internal static void _decode_com_xacti_CopterAttStatus(CanardRxTransfer transfer,ref uint32_t bit_ofs, com_xacti_CopterAttStatus msg, bool tao) {

                for (int i=0; i < 4; i++) {
                    canardDecodeScalar(transfer, bit_ofs, 16, true, ref msg.quaternion_wxyz_e4[i]);
                    bit_ofs += 16;
                }

                if (!tao) {
                    canardDecodeScalar(transfer, bit_ofs, 2, false, ref msg.reserved_len);
                    bit_ofs += 2;
                } else {
                    msg.reserved_len = (uint8_t)(((transfer.payload_len*8)-bit_ofs)/16);
                }

                msg.reserved = new Single[msg.reserved_len];
                for (int i=0; i < msg.reserved_len; i++) {
                    {
                        uint16_t float16_val = 0;
                        canardDecodeScalar(transfer, bit_ofs, 16, true, ref float16_val);
                        msg.reserved[i] = canardConvertFloat16ToNativeFloat(float16_val);
                    }
                    bit_ofs += 16;
                }

            }
        }
    }
}