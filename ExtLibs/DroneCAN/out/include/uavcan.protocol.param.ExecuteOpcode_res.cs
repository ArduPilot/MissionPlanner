
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
        public partial class uavcan_protocol_param_ExecuteOpcode_res: IDroneCANSerialize 
        {
            public const int UAVCAN_PROTOCOL_PARAM_EXECUTEOPCODE_RES_MAX_PACK_SIZE = 7;
            public const ulong UAVCAN_PROTOCOL_PARAM_EXECUTEOPCODE_RES_DT_SIG = 0x3B131AC5EB69D2CD;
            public const int UAVCAN_PROTOCOL_PARAM_EXECUTEOPCODE_RES_DT_ID = 10;

            public int64_t argument = new int64_t();
            public bool ok = new bool();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
            {
                encode_uavcan_protocol_param_ExecuteOpcode_res(this, chunk_cb, ctx);
            }

            public void decode(CanardRxTransfer transfer)
            {
                decode_uavcan_protocol_param_ExecuteOpcode_res(transfer, this);
            }

            public static uavcan_protocol_param_ExecuteOpcode_res ByteArrayToDroneCANMsg(byte[] transfer, int startoffset)
            {
                var ans = new uavcan_protocol_param_ExecuteOpcode_res();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()));
                return ans;
            }
        }
    }
}