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
        public partial class com_tmotor_esc_FocCtrl: IDroneCANSerialize 
        {
            public const int COM_TMOTOR_ESC_FOCCTRL_MAX_PACK_SIZE = 8;
            public const ulong COM_TMOTOR_ESC_FOCCTRL_DT_SIG = 0x598143612FBC000B;
            public const int COM_TMOTOR_ESC_FOCCTRL_DT_ID = 1035;

            public uint8_t esc_index = new uint8_t();
            public uint8_t esc_mode = new uint8_t();
            public uint8_t esc_fdb_rate = new uint8_t();
            public uint8_t esc_cmd = new uint8_t();
            public int32_t esc_cmd_val = new int32_t();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan = false)
            {
                encode_com_tmotor_esc_FocCtrl(this, chunk_cb, ctx, fdcan);
            }

            public void decode(CanardRxTransfer transfer, bool fdcan = false)
            {
                decode_com_tmotor_esc_FocCtrl(transfer, this, fdcan);
            }

            public static com_tmotor_esc_FocCtrl ByteArrayToDroneCANMsg(byte[] transfer, int startoffset, bool fdcan = false)
            {
                var ans = new com_tmotor_esc_FocCtrl();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()), fdcan);
                return ans;
            }
        }
    }
}