
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
//using uavcan.protocol.DataTypeKind.cs
        public partial class uavcan_protocol_GetDataTypeInfo_req: IDroneCANSerialize 
        {
            public const int UAVCAN_PROTOCOL_GETDATATYPEINFO_REQ_MAX_PACK_SIZE = 84;
            public const ulong UAVCAN_PROTOCOL_GETDATATYPEINFO_REQ_DT_SIG = 0x1B283338A7BED2D8;
            public const int UAVCAN_PROTOCOL_GETDATATYPEINFO_REQ_DT_ID = 2;

            public uint16_t id = new uint16_t();
            public uavcan_protocol_DataTypeKind kind = new uavcan_protocol_DataTypeKind();
            public uint8_t name_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=80)] public uint8_t[] name = Enumerable.Range(1, 80).Select(i => new uint8_t()).ToArray();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
            {
                encode_uavcan_protocol_GetDataTypeInfo_req(this, chunk_cb, ctx);
            }

            public void decode(CanardRxTransfer transfer)
            {
                decode_uavcan_protocol_GetDataTypeInfo_req(transfer, this);
            }

            public static uavcan_protocol_GetDataTypeInfo_req ByteArrayToDroneCANMsg(byte[] transfer, int startoffset)
            {
                var ans = new uavcan_protocol_GetDataTypeInfo_req();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()));
                return ans;
            }
        }
    }
}