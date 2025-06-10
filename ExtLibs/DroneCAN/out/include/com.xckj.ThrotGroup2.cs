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
        public partial class com_xckj_esc_ThrotGroup2 : IDroneCANSerialize
        {
            public const int COM_XCKJ_ESC_THROTGROUP2_MAX_PACK_SIZE = 7;
            public const ulong COM_XCKJ_ESC_THROTGROUP2_DT_SIG = 0xa28b683d1286d1fc;
            public const int COM_XCKJ_ESC_THROTGROUP2_DT_ID = 20151;

            public uint8_t command_len; [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] public uint16_t[] command = Enumerable.Range(1, 4).Select(i => new uint16_t()).ToArray();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan = false)
            {
                encode_com_xckj_esc_ThrotGroup2(this, chunk_cb, ctx, fdcan);
            }
            public void decode(CanardRxTransfer transfer, bool fdcan = false)
            {
                decode_com_xckj_esc_ThrotGroup2(transfer, this, fdcan);
            }
            public static com_xckj_esc_ThrotGroup2 ByteArrayToDroneCANMsg(byte[] transfer, int startoffset, bool fdcan = false)
            {
                var ans = new com_xckj_esc_ThrotGroup2();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()), fdcan);
                return ans;
            }
        }
    }

}