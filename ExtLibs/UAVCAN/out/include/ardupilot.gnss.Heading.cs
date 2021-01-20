
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


        public const int ARDUPILOT_GNSS_HEADING_MAX_PACK_SIZE = 5;
        public const ulong ARDUPILOT_GNSS_HEADING_DT_SIG = 0x315CAE39ECED3412;
        public const int ARDUPILOT_GNSS_HEADING_DT_ID = 20002;



        public partial class ardupilot_gnss_Heading: IUAVCANSerialize {
            public bool heading_valid = new bool();
            public bool heading_accuracy_valid = new bool();
            public Single heading_rad = new Single();
            public Single heading_accuracy_rad = new Single();

            public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
            {
                encode_ardupilot_gnss_Heading(this, chunk_cb, ctx);
            }

            public void decode(CanardRxTransfer transfer)
            {
                decode_ardupilot_gnss_Heading(transfer, this);
            }
        }
    }
}