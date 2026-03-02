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

namespace DroneCAN
{
    public partial class DroneCAN 
    {
        public partial class com_tmotor_esc_ParamCfg: IDroneCANSerialize 
        {
            public const int COM_TMOTOR_ESC_PARAMCFG_MAX_PACK_SIZE = 27;
            public const ulong COM_TMOTOR_ESC_PARAMCFG_DT_SIG = 0x948F5E0B33E0EDEE;
            public const int COM_TMOTOR_ESC_PARAMCFG_DT_ID = 1033;

            public uint8_t esc_index = new uint8_t();
            public uint32_t esc_uuid = new uint32_t();
            public uint16_t esc_id_set = new uint16_t();
            public uint16_t esc_ov_threshold = new uint16_t();
            public uint16_t esc_oc_threshold = new uint16_t();
            public uint16_t esc_ot_threshold = new uint16_t();
            public uint16_t esc_acc_threshold = new uint16_t();
            public uint16_t esc_dacc_threshold = new uint16_t();
            public int16_t esc_rotate_dir = new int16_t();
            public uint8_t esc_timing = new uint8_t();
            public uint8_t esc_signal_priority = new uint8_t();
            public uint16_t esc_led_mode = new uint16_t();
            public uint8_t esc_can_rate = new uint8_t();
            public uint16_t esc_fdb_rate = new uint16_t();
            public uint8_t esc_save_option = new uint8_t();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan = false)
            {
                encode_com_tmotor_esc_ParamCfg(this, chunk_cb, ctx, fdcan);
            }

            public void decode(CanardRxTransfer transfer, bool fdcan = false)
            {
                decode_com_tmotor_esc_ParamCfg(transfer, this, fdcan);
            }

            public static com_tmotor_esc_ParamCfg ByteArrayToDroneCANMsg(byte[] transfer, int startoffset, bool fdcan = false)
            {
                var ans = new com_tmotor_esc_ParamCfg();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()), fdcan);
                return ans;
            }
        }
    }
}