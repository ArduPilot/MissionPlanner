
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
        public partial class mppt_OutputEnable_res: IDroneCANSerialize 
        {
            public const int MPPT_OUTPUTENABLE_RES_MAX_PACK_SIZE = 1;
            public const ulong MPPT_OUTPUTENABLE_RES_DT_SIG = 0xEA251F2A6DD1D8A5;
            public const int MPPT_OUTPUTENABLE_RES_DT_ID = 240;

            public bool enabled = new bool();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
            {
                encode_mppt_OutputEnable_res(this, chunk_cb, ctx);
            }

            public void decode(CanardRxTransfer transfer)
            {
                decode_mppt_OutputEnable_res(transfer, this);
            }

            public static mppt_OutputEnable_res ByteArrayToDroneCANMsg(byte[] transfer, int startoffset)
            {
                var ans = new mppt_OutputEnable_res();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()));
                return ans;
            }
        }
    }
}