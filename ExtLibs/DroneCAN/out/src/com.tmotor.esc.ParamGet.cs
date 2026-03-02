
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

        public partial class com_tmotor_esc_ParamGet : IDroneCANSerialize
        {
            public static void encode_com_tmotor_esc_ParamGet(com_tmotor_esc_ParamGet msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan) {
                uint8_t[] buffer = new uint8_t[8];
                _encode_com_tmotor_esc_ParamGet(buffer, msg, chunk_cb, ctx, !fdcan);
            }

            public static uint32_t decode_com_tmotor_esc_ParamGet(CanardRxTransfer transfer, com_tmotor_esc_ParamGet msg, bool fdcan) {
                uint32_t bit_ofs = 0;
                _decode_com_tmotor_esc_ParamGet(transfer, ref bit_ofs, msg, !fdcan);
                return (bit_ofs+7)/8;
            }

            internal static void _encode_com_tmotor_esc_ParamGet(uint8_t[] buffer, com_tmotor_esc_ParamGet msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 8, msg.esc_index);
                chunk_cb(buffer, 8, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 32, msg.esc_uuid);
                chunk_cb(buffer, 32, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 16, msg.esc_id_req);
                chunk_cb(buffer, 16, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 16, msg.esc_ov_threshold);
                chunk_cb(buffer, 16, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 16, msg.esc_oc_threshold);
                chunk_cb(buffer, 16, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 16, msg.esc_ot_threshold);
                chunk_cb(buffer, 16, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 16, msg.esc_acc_threshold);
                chunk_cb(buffer, 16, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 16, msg.esc_dacc_threshold);
                chunk_cb(buffer, 16, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 16, msg.esc_rotate_dir);
                chunk_cb(buffer, 16, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 8, msg.esc_timing);
                chunk_cb(buffer, 8, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 16, msg.esc_startup_times);
                chunk_cb(buffer, 16, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 32, msg.esc_startup_duration);
                chunk_cb(buffer, 32, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 32, msg.esc_product_date);
                chunk_cb(buffer, 32, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 32, msg.esc_error_count);
                chunk_cb(buffer, 32, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 8, msg.esc_signal_priority);
                chunk_cb(buffer, 8, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 16, msg.esc_led_mode);
                chunk_cb(buffer, 16, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 8, msg.esc_can_rate);
                chunk_cb(buffer, 8, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 16, msg.esc_fdb_rate);
                chunk_cb(buffer, 16, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 8, msg.esc_save_option);
                chunk_cb(buffer, 8, ctx);
                if (!tao) {
                    memset(buffer,0,8);
                    canardEncodeScalar(buffer, 0, 6, msg.rsvd_len);
                    chunk_cb(buffer, 6, ctx);
                }
                for (int i=0; i < msg.rsvd_len; i++) {
                        memset(buffer,0,8);
                        canardEncodeScalar(buffer, 0, 8, msg.rsvd[i]);
                        chunk_cb(buffer, 8, ctx);
                }
            }

            internal static void _decode_com_tmotor_esc_ParamGet(CanardRxTransfer transfer,ref uint32_t bit_ofs, com_tmotor_esc_ParamGet msg, bool tao) {

                canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.esc_index);
                bit_ofs += 8;

                canardDecodeScalar(transfer, bit_ofs, 32, false, ref msg.esc_uuid);
                bit_ofs += 32;

                canardDecodeScalar(transfer, bit_ofs, 16, false, ref msg.esc_id_req);
                bit_ofs += 16;

                canardDecodeScalar(transfer, bit_ofs, 16, false, ref msg.esc_ov_threshold);
                bit_ofs += 16;

                canardDecodeScalar(transfer, bit_ofs, 16, false, ref msg.esc_oc_threshold);
                bit_ofs += 16;

                canardDecodeScalar(transfer, bit_ofs, 16, false, ref msg.esc_ot_threshold);
                bit_ofs += 16;

                canardDecodeScalar(transfer, bit_ofs, 16, false, ref msg.esc_acc_threshold);
                bit_ofs += 16;

                canardDecodeScalar(transfer, bit_ofs, 16, false, ref msg.esc_dacc_threshold);
                bit_ofs += 16;

                canardDecodeScalar(transfer, bit_ofs, 16, true, ref msg.esc_rotate_dir);
                bit_ofs += 16;

                canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.esc_timing);
                bit_ofs += 8;

                canardDecodeScalar(transfer, bit_ofs, 16, false, ref msg.esc_startup_times);
                bit_ofs += 16;

                canardDecodeScalar(transfer, bit_ofs, 32, false, ref msg.esc_startup_duration);
                bit_ofs += 32;

                canardDecodeScalar(transfer, bit_ofs, 32, false, ref msg.esc_product_date);
                bit_ofs += 32;

                canardDecodeScalar(transfer, bit_ofs, 32, false, ref msg.esc_error_count);
                bit_ofs += 32;

                canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.esc_signal_priority);
                bit_ofs += 8;

                canardDecodeScalar(transfer, bit_ofs, 16, false, ref msg.esc_led_mode);
                bit_ofs += 16;

                canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.esc_can_rate);
                bit_ofs += 8;

                canardDecodeScalar(transfer, bit_ofs, 16, false, ref msg.esc_fdb_rate);
                bit_ofs += 16;

                canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.esc_save_option);
                bit_ofs += 8;

                if (!tao) {
                    canardDecodeScalar(transfer, bit_ofs, 6, false, ref msg.rsvd_len);
                    bit_ofs += 6;
                } else {
                    msg.rsvd_len = (uint8_t)(((transfer.payload_len*8)-bit_ofs)/8);
                }

                msg.rsvd = new uint8_t[msg.rsvd_len];
                for (int i=0; i < msg.rsvd_len; i++) {
                    canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.rsvd[i]);
                    bit_ofs += 8;
                }

            }
        }
    }
}