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
        public partial class com_hobbywing_esc_SetReportingFrequency_res: IDroneCANSerialize 
        {
            public const int COM_HOBBYWING_ESC_SETREPORTINGFREQUENCY_RES_MAX_PACK_SIZE = 4;
            public const ulong COM_HOBBYWING_ESC_SETREPORTINGFREQUENCY_RES_DT_SIG = 0x1FD0404420983DEB;
            public const int COM_HOBBYWING_ESC_SETREPORTINGFREQUENCY_RES_DT_ID = 214;

            public uint8_t option = new uint8_t();
            public uint16_t MSG_ID = new uint16_t();
            public uint8_t rate = new uint8_t();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan = false)
            {
                encode_com_hobbywing_esc_SetReportingFrequency_res(this, chunk_cb, ctx, fdcan);
            }

            public void decode(CanardRxTransfer transfer, bool fdcan = false)
            {
                decode_com_hobbywing_esc_SetReportingFrequency_res(transfer, this, fdcan);
            }

            public static com_hobbywing_esc_SetReportingFrequency_res ByteArrayToDroneCANMsg(byte[] transfer, int startoffset, bool fdcan = false)
            {
                var ans = new com_hobbywing_esc_SetReportingFrequency_res();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()), fdcan);
                return ans;
            }
        }
    }
}