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
        public partial class com_himark_servo_ServoCmd: IDroneCANSerialize 
        {
            public const int COM_HIMARK_SERVO_SERVOCMD_MAX_PACK_SIZE = 22;
            public const ulong COM_HIMARK_SERVO_SERVOCMD_DT_SIG = 0x5D09E48551CE9194;
            public const int COM_HIMARK_SERVO_SERVOCMD_DT_ID = 2018;

            public uint8_t cmd_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=17)] public uint16_t[] cmd = Enumerable.Range(1, 17).Select(i => new uint16_t()).ToArray();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan = false)
            {
                encode_com_himark_servo_ServoCmd(this, chunk_cb, ctx, fdcan);
            }

            public void decode(CanardRxTransfer transfer, bool fdcan = false)
            {
                decode_com_himark_servo_ServoCmd(transfer, this, fdcan);
            }

            public static com_himark_servo_ServoCmd ByteArrayToDroneCANMsg(byte[] transfer, int startoffset, bool fdcan = false)
            {
                var ans = new com_himark_servo_ServoCmd();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()), fdcan);
                return ans;
            }
        }
    }
}