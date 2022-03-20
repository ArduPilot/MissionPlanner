
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
        public partial class ardupilot_gnss_MovingBaselineData: IDroneCANSerialize 
        {
            public const int ARDUPILOT_GNSS_MOVINGBASELINEDATA_MAX_PACK_SIZE = 302;
            public const ulong ARDUPILOT_GNSS_MOVINGBASELINEDATA_DT_SIG = 0x9F323748C32133A;
            public const int ARDUPILOT_GNSS_MOVINGBASELINEDATA_DT_ID = 20005;

            public uint16_t data_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=300)] public uint8_t[] data = Enumerable.Range(1, 300).Select(i => new uint8_t()).ToArray();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
            {
                encode_ardupilot_gnss_MovingBaselineData(this, chunk_cb, ctx);
            }

            public void decode(CanardRxTransfer transfer)
            {
                decode_ardupilot_gnss_MovingBaselineData(transfer, this);
            }

            public static ardupilot_gnss_MovingBaselineData ByteArrayToDroneCANMsg(byte[] transfer, int startoffset)
            {
                var ans = new ardupilot_gnss_MovingBaselineData();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()));
                return ans;
            }
        }
    }
}