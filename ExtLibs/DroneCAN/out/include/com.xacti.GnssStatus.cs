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
        public partial class com_xacti_GnssStatus: IDroneCANSerialize 
        {
            public const int COM_XACTI_GNSSSTATUS_MAX_PACK_SIZE = 33;
            public const ulong COM_XACTI_GNSSSTATUS_DT_SIG = 0x3413AC5D3E1DCBE3;
            public const int COM_XACTI_GNSSSTATUS_DT_ID = 20305;

            public uint8_t gps_status = new uint8_t();
            public uint8_t order = new uint8_t();
            public uint8_t remain_buffer = new uint8_t();
            public uint16_t utc_year = new uint16_t();
            public uint8_t utc_month = new uint8_t();
            public uint8_t utc_day = new uint8_t();
            public uint8_t utc_hour = new uint8_t();
            public uint8_t utc_minute = new uint8_t();
            public Single utc_seconds = new Single();
            public Double latitude = new Double();
            public Double longitude = new Double();
            public Single altitude = new Single();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan = false)
            {
                encode_com_xacti_GnssStatus(this, chunk_cb, ctx, fdcan);
            }

            public void decode(CanardRxTransfer transfer, bool fdcan = false)
            {
                decode_com_xacti_GnssStatus(transfer, this, fdcan);
            }

            public static com_xacti_GnssStatus ByteArrayToDroneCANMsg(byte[] transfer, int startoffset, bool fdcan = false)
            {
                var ans = new com_xacti_GnssStatus();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()), fdcan);
                return ans;
            }
        }
    }
}