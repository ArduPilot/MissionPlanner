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
        public partial class com_xckj_esc_AutoUpMsg1 : IDroneCANSerialize
        {
            public const int COM_XCKJ_ESC_AUTOUPMSG1_MAX_PACK_SIZE = 6;
            public const ulong COM_XCKJ_ESC_AUTOUPMSG1_DT_SIG = 0xb95f98a2bb955035;
            public const int COM_XCKJ_ESC_AUTOUPMSG1_DT_ID = 20130;

            public uint16_t rpm = new uint16_t();
            public uint16_t current = new uint16_t();
            public uint16_t runstatus = new uint16_t();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan = false)
            {
                encode_com_xckj_esc_AutoUpMsg1(this, chunk_cb, ctx, fdcan);
            }

            public void decode(CanardRxTransfer transfer, bool fdcan = false)
            {
                decode_com_xckj_esc_AutoUpMsg1(transfer, this, fdcan);
            }

            public static com_xckj_esc_AutoUpMsg1 ByteArrayToDroneCANMsg(byte[] transfer, int startoffset, bool fdcan = false)
            {
                var ans = new com_xckj_esc_AutoUpMsg1();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()), fdcan);
                return ans;
            }
        }
    }
}