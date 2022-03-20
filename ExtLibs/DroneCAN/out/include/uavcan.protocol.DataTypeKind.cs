
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
        public partial class uavcan_protocol_DataTypeKind: IDroneCANSerialize 
        {
            public const int UAVCAN_PROTOCOL_DATATYPEKIND_MAX_PACK_SIZE = 1;
            public const ulong UAVCAN_PROTOCOL_DATATYPEKIND_DT_SIG = 0x9420A73E008E5930;

            public const double UAVCAN_PROTOCOL_DATATYPEKIND_SERVICE = 0; // saturated uint8
            public const double UAVCAN_PROTOCOL_DATATYPEKIND_MESSAGE = 1; // saturated uint8

            public uint8_t value = new uint8_t();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
            {
                encode_uavcan_protocol_DataTypeKind(this, chunk_cb, ctx);
            }

            public void decode(CanardRxTransfer transfer)
            {
                decode_uavcan_protocol_DataTypeKind(transfer, this);
            }

            public static uavcan_protocol_DataTypeKind ByteArrayToDroneCANMsg(byte[] transfer, int startoffset)
            {
                var ans = new uavcan_protocol_DataTypeKind();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()));
                return ans;
            }
        }
    }
}