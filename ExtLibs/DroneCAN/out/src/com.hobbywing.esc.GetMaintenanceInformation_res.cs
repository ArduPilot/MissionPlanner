
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

        public partial class com_hobbywing_esc_GetMaintenanceInformation_res : IDroneCANSerialize
        {
            public static void encode_com_hobbywing_esc_GetMaintenanceInformation_res(com_hobbywing_esc_GetMaintenanceInformation_res msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan) {
                uint8_t[] buffer = new uint8_t[8];
                _encode_com_hobbywing_esc_GetMaintenanceInformation_res(buffer, msg, chunk_cb, ctx, !fdcan);
            }

            public static uint32_t decode_com_hobbywing_esc_GetMaintenanceInformation_res(CanardRxTransfer transfer, com_hobbywing_esc_GetMaintenanceInformation_res msg, bool fdcan) {
                uint32_t bit_ofs = 0;
                _decode_com_hobbywing_esc_GetMaintenanceInformation_res(transfer, ref bit_ofs, msg, !fdcan);
                return (bit_ofs+7)/8;
            }

            internal static void _encode_com_hobbywing_esc_GetMaintenanceInformation_res(uint8_t[] buffer, com_hobbywing_esc_GetMaintenanceInformation_res msg, dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 32, msg.total_rotation_time_min);
                chunk_cb(buffer, 32, ctx);
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 24, msg.time_since_maintainence_min);
                chunk_cb(buffer, 24, ctx);
            }

            internal static void _decode_com_hobbywing_esc_GetMaintenanceInformation_res(CanardRxTransfer transfer,ref uint32_t bit_ofs, com_hobbywing_esc_GetMaintenanceInformation_res msg, bool tao) {

                canardDecodeScalar(transfer, bit_ofs, 32, false, ref msg.total_rotation_time_min);
                bit_ofs += 32;

                canardDecodeScalar(transfer, bit_ofs, 24, false, ref msg.time_since_maintainence_min);
                bit_ofs += 24;

            }
        }
    }
}