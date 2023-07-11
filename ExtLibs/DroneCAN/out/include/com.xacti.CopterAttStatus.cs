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
        public partial class com_xacti_CopterAttStatus: IDroneCANSerialize 
        {
            public const int COM_XACTI_COPTERATTSTATUS_MAX_PACK_SIZE = 13;
            public const ulong COM_XACTI_COPTERATTSTATUS_DT_SIG = 0x6C1F30F1893763B1;
            public const int COM_XACTI_COPTERATTSTATUS_DT_ID = 20407;

            [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)] public int16_t[] quaternion_wxyz_e4 = new int16_t[4];
            public uint8_t reserved_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=2)] public Single[] reserved = Enumerable.Range(1, 2).Select(i => new Single()).ToArray();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan = false)
            {
                encode_com_xacti_CopterAttStatus(this, chunk_cb, ctx, fdcan);
            }

            public void decode(CanardRxTransfer transfer, bool fdcan = false)
            {
                decode_com_xacti_CopterAttStatus(transfer, this, fdcan);
            }

            public static com_xacti_CopterAttStatus ByteArrayToDroneCANMsg(byte[] transfer, int startoffset, bool fdcan = false)
            {
                var ans = new com_xacti_CopterAttStatus();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()), fdcan);
                return ans;
            }
        }
    }
}