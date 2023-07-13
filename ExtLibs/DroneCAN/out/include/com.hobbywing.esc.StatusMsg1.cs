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
        public partial class com_hobbywing_esc_StatusMsg1: IDroneCANSerialize 
        {
            public const int COM_HOBBYWING_ESC_STATUSMSG1_MAX_PACK_SIZE = 6;
            public const ulong COM_HOBBYWING_ESC_STATUSMSG1_DT_SIG = 0x813B3E2C4AD670E;
            public const int COM_HOBBYWING_ESC_STATUSMSG1_DT_ID = 20050;

            public uint16_t rpm = new uint16_t();
            public uint16_t pwm = new uint16_t();
            public uint16_t status = new uint16_t();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan = false)
            {
                encode_com_hobbywing_esc_StatusMsg1(this, chunk_cb, ctx, fdcan);
            }

            public void decode(CanardRxTransfer transfer, bool fdcan = false)
            {
                decode_com_hobbywing_esc_StatusMsg1(transfer, this, fdcan);
            }

            public static com_hobbywing_esc_StatusMsg1 ByteArrayToDroneCANMsg(byte[] transfer, int startoffset, bool fdcan = false)
            {
                var ans = new com_hobbywing_esc_StatusMsg1();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()), fdcan);
                return ans;
            }
        }
    }
}