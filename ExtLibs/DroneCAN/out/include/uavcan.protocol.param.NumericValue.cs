
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
//using uavcan.protocol.param.Empty.cs
        public partial class uavcan_protocol_param_NumericValue: IDroneCANSerialize 
        {
            public const int UAVCAN_PROTOCOL_PARAM_NUMERICVALUE_MAX_PACK_SIZE = 9;
            public const ulong UAVCAN_PROTOCOL_PARAM_NUMERICVALUE_DT_SIG = 0xDA6D6FEA22E3587;

            public enum uavcan_protocol_param_NumericValue_type_t {
                UAVCAN_PROTOCOL_PARAM_NUMERICVALUE_TYPE_EMPTY,
                UAVCAN_PROTOCOL_PARAM_NUMERICVALUE_TYPE_INTEGER_VALUE,
                UAVCAN_PROTOCOL_PARAM_NUMERICVALUE_TYPE_REAL_VALUE,
            }

            public uavcan_protocol_param_NumericValue_type_t uavcan_protocol_param_NumericValue_type;
            //[StructLayout(LayoutKind.Explicit, Pack = 1)] 
            public class unions {
                //[FieldOffset(0)]
                public uavcan_protocol_param_Empty empty = new uavcan_protocol_param_Empty();
                //[FieldOffset(0)]
                public int64_t integer_value = new int64_t();
                //[FieldOffset(0)]
                public Single real_value = new Single();
            }
            public unions union = new unions();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
            {
                encode_uavcan_protocol_param_NumericValue(this, chunk_cb, ctx);
            }

            public void decode(CanardRxTransfer transfer)
            {
                decode_uavcan_protocol_param_NumericValue(transfer, this);
            }

            public static uavcan_protocol_param_NumericValue ByteArrayToDroneCANMsg(byte[] transfer, int startoffset)
            {
                var ans = new uavcan_protocol_param_NumericValue();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()));
                return ans;
            }
        }
    }
}