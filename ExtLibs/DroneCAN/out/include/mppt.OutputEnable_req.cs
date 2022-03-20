
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
        public partial class mppt_OutputEnable_req: IDroneCANSerialize 
        {
            public const int MPPT_OUTPUTENABLE_REQ_MAX_PACK_SIZE = 1;
            public const ulong MPPT_OUTPUTENABLE_REQ_DT_SIG = 0xEA251F2A6DD1D8A5;
            public const int MPPT_OUTPUTENABLE_REQ_DT_ID = 240;

            public bool enable = new bool();
            public bool disable = new bool();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
            {
                encode_mppt_OutputEnable_req(this, chunk_cb, ctx);
            }

            public void decode(CanardRxTransfer transfer)
            {
                decode_mppt_OutputEnable_req(transfer, this);
            }

            public static mppt_OutputEnable_req ByteArrayToDroneCANMsg(byte[] transfer, int startoffset)
            {
                var ans = new mppt_OutputEnable_req();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()));
                return ans;
            }
        }
    }
}