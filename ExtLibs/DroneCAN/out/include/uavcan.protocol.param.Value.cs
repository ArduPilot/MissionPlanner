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
        public partial class uavcan_protocol_param_Value: IDroneCANSerialize 
        {
            public const int UAVCAN_PROTOCOL_PARAM_VALUE_MAX_PACK_SIZE = 130;
            public const ulong UAVCAN_PROTOCOL_PARAM_VALUE_DT_SIG = 0x29F14BF484727267;

            public enum uavcan_protocol_param_Value_type_t {
                UAVCAN_PROTOCOL_PARAM_VALUE_TYPE_EMPTY,
                UAVCAN_PROTOCOL_PARAM_VALUE_TYPE_INTEGER_VALUE,
                UAVCAN_PROTOCOL_PARAM_VALUE_TYPE_REAL_VALUE,
                UAVCAN_PROTOCOL_PARAM_VALUE_TYPE_BOOLEAN_VALUE,
                UAVCAN_PROTOCOL_PARAM_VALUE_TYPE_STRING_VALUE,
            }

            public uavcan_protocol_param_Value_type_t uavcan_protocol_param_Value_type;
            //[StructLayout(LayoutKind.Explicit, Pack = 1)] 
            public class unions {
                //[FieldOffset(0)]
                public uavcan_protocol_param_Empty empty = new uavcan_protocol_param_Empty();
                //[FieldOffset(0)]
                public int64_t integer_value = new int64_t();
                //[FieldOffset(0)]
                public Single real_value = new Single();
                //[FieldOffset(0)]
                public uint8_t boolean_value = new uint8_t();
                //[FieldOffset(0)]
                public uint8_t string_value_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=128)] public uint8_t[] string_value = Enumerable.Range(1, 128).Select(i => new uint8_t()).ToArray();
            }
            public unions union = new unions();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan = false)
            {
                encode_uavcan_protocol_param_Value(this, chunk_cb, ctx, fdcan);
            }

            public void decode(CanardRxTransfer transfer, bool fdcan = false)
            {
                decode_uavcan_protocol_param_Value(transfer, this, fdcan);
            }

            public static uavcan_protocol_param_Value ByteArrayToDroneCANMsg(byte[] transfer, int startoffset, bool fdcan = false)
            {
                var ans = new uavcan_protocol_param_Value();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()), fdcan);
                return ans;
            }
        }
    }
}