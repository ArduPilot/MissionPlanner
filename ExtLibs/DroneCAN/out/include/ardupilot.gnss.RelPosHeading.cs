
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
//using uavcan.Timestamp.cs
        public partial class ardupilot_gnss_RelPosHeading: IDroneCANSerialize 
        {
            public const int ARDUPILOT_GNSS_RELPOSHEADING_MAX_PACK_SIZE = 20;
            public const ulong ARDUPILOT_GNSS_RELPOSHEADING_DT_SIG = 0xA1727AF295F94478;
            public const int ARDUPILOT_GNSS_RELPOSHEADING_DT_ID = 20006;

            public uavcan_Timestamp timestamp = new uavcan_Timestamp();
            public bool reported_heading_acc_available = new bool();
            public Single reported_heading_deg = new Single();
            public Single reported_heading_acc_deg = new Single();
            public Single relative_distance_m = new Single();
            public Single relative_down_pos_m = new Single();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
            {
                encode_ardupilot_gnss_RelPosHeading(this, chunk_cb, ctx);
            }

            public void decode(CanardRxTransfer transfer)
            {
                decode_ardupilot_gnss_RelPosHeading(transfer, this);
            }

            public static ardupilot_gnss_RelPosHeading ByteArrayToDroneCANMsg(byte[] transfer, int startoffset)
            {
                var ans = new ardupilot_gnss_RelPosHeading();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()));
                return ans;
            }
        }
    }
}