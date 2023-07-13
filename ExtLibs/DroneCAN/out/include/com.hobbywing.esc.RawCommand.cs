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
        public partial class com_hobbywing_esc_RawCommand: IDroneCANSerialize 
        {
            public const int COM_HOBBYWING_ESC_RAWCOMMAND_MAX_PACK_SIZE = 15;
            public const ulong COM_HOBBYWING_ESC_RAWCOMMAND_DT_SIG = 0xBDF086C79F6640AD;
            public const int COM_HOBBYWING_ESC_RAWCOMMAND_DT_ID = 20100;

            public uint8_t command_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=8)] public int16_t[] command = Enumerable.Range(1, 8).Select(i => new int16_t()).ToArray();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan = false)
            {
                encode_com_hobbywing_esc_RawCommand(this, chunk_cb, ctx, fdcan);
            }

            public void decode(CanardRxTransfer transfer, bool fdcan = false)
            {
                decode_com_hobbywing_esc_RawCommand(transfer, this, fdcan);
            }

            public static com_hobbywing_esc_RawCommand ByteArrayToDroneCANMsg(byte[] transfer, int startoffset, bool fdcan = false)
            {
                var ans = new com_hobbywing_esc_RawCommand();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()), fdcan);
                return ans;
            }
        }
    }
}