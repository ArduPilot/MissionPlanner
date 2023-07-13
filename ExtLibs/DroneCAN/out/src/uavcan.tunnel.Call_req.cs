
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

        public partial class uavcan_tunnel_Call_req : IDroneCANSerialize
        {
            public static void encode_uavcan_tunnel_Call_req(uavcan_tunnel_Call_req msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan) {
                uint8_t[] buffer = new uint8_t[8];
                _encode_uavcan_tunnel_Call_req(buffer, msg, chunk_cb, ctx, !fdcan);
            }

            public static uint32_t decode_uavcan_tunnel_Call_req(CanardRxTransfer transfer, uavcan_tunnel_Call_req msg, bool fdcan) {
                uint32_t bit_ofs = 0;
                _decode_uavcan_tunnel_Call_req(transfer, ref bit_ofs, msg, !fdcan);
                return (bit_ofs+7)/8;
            }

            internal static void _encode_uavcan_tunnel_Call_req(uint8_t[] buffer, uavcan_tunnel_Call_req msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {
                uavcan_tunnel_Protocol._encode_uavcan_tunnel_Protocol(buffer, msg.protocol, chunk_cb, ctx, false);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 8, msg.channel_id);
                chunk_cb(buffer, 8, ctx);
                if (!tao) {
                    memset(buffer,0,8);
                    canardEncodeScalar(buffer, 0, 6, msg.buffer_len);
                    chunk_cb(buffer, 6, ctx);
                }
                for (int i=0; i < msg.buffer_len; i++) {
                        memset(buffer,0,8);
                        canardEncodeScalar(buffer, 0, 8, msg.buffer[i]);
                        chunk_cb(buffer, 8, ctx);
                }
            }

            internal static void _decode_uavcan_tunnel_Call_req(CanardRxTransfer transfer,ref uint32_t bit_ofs, uavcan_tunnel_Call_req msg, bool tao) {

                uavcan_tunnel_Protocol._decode_uavcan_tunnel_Protocol(transfer, ref bit_ofs, msg.protocol, false);

                canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.channel_id);
                bit_ofs += 8;

                if (!tao) {
                    canardDecodeScalar(transfer, bit_ofs, 6, false, ref msg.buffer_len);
                    bit_ofs += 6;
                } else {
                    msg.buffer_len = (uint8_t)(((transfer.payload_len*8)-bit_ofs)/8);
                }

                msg.buffer = new uint8_t[msg.buffer_len];
                for (int i=0; i < msg.buffer_len; i++) {
                    canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.buffer[i]);
                    bit_ofs += 8;
                }

            }
        }
    }
}