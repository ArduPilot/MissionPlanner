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
        public partial class uavcan_protocol_param_ExecuteOpcode_req: IDroneCANSerialize 
        {
            public const int UAVCAN_PROTOCOL_PARAM_EXECUTEOPCODE_REQ_MAX_PACK_SIZE = 7;
            public const ulong UAVCAN_PROTOCOL_PARAM_EXECUTEOPCODE_REQ_DT_SIG = 0x3B131AC5EB69D2CD;
            public const int UAVCAN_PROTOCOL_PARAM_EXECUTEOPCODE_REQ_DT_ID = 10;

            public const double UAVCAN_PROTOCOL_PARAM_EXECUTEOPCODE_REQ_OPCODE_SAVE = 0; // saturated uint8
            public const double UAVCAN_PROTOCOL_PARAM_EXECUTEOPCODE_REQ_OPCODE_ERASE = 1; // saturated uint8

            public uint8_t opcode = new uint8_t();
            public int64_t argument = new int64_t();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan = false)
            {
                encode_uavcan_protocol_param_ExecuteOpcode_req(this, chunk_cb, ctx, fdcan);
            }

            public void decode(CanardRxTransfer transfer, bool fdcan = false)
            {
                decode_uavcan_protocol_param_ExecuteOpcode_req(transfer, this, fdcan);
            }

            public static uavcan_protocol_param_ExecuteOpcode_req ByteArrayToDroneCANMsg(byte[] transfer, int startoffset, bool fdcan = false)
            {
                var ans = new uavcan_protocol_param_ExecuteOpcode_req();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()), fdcan);
                return ans;
            }
        }
    }
}