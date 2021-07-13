
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

namespace UAVCAN
{
    public partial class uavcan {


        public const int ARDUPILOT_GNSS_STATUS_MAX_PACK_SIZE = 7;
        public const ulong ARDUPILOT_GNSS_STATUS_DT_SIG = 0xBA3CB4ABBB007F69;
        public const int ARDUPILOT_GNSS_STATUS_DT_ID = 20003;



        public const double ARDUPILOT_GNSS_STATUS_STATUS_LOGGING = 1; // saturated uint23
        public const double ARDUPILOT_GNSS_STATUS_STATUS_ARMABLE = 2; // saturated uint23

        public partial class ardupilot_gnss_Status: IUAVCANSerialize {
            public uint32_t error_codes = new uint32_t();
            public bool healthy = new bool();
            public uint32_t status = new uint32_t();

            public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
            {
                encode_ardupilot_gnss_Status(this, chunk_cb, ctx);
            }

            public void decode(CanardRxTransfer transfer)
            {
                decode_ardupilot_gnss_Status(transfer, this);
            }
        }
    }
}