
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
        public partial class dronecan_remoteid_OperatorID: IDroneCANSerialize 
        {
            public const int DRONECAN_REMOTEID_OPERATORID_MAX_PACK_SIZE = 43;
            public const ulong DRONECAN_REMOTEID_OPERATORID_DT_SIG = 0x581E7FC7F03AF935;
            public const int DRONECAN_REMOTEID_OPERATORID_DT_ID = 20034;

            public const double DRONECAN_REMOTEID_OPERATORID_ODID_OPERATOR_ID_TYPE_CAA = 0; // saturated uint8

            public uint8_t id_or_mac_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=20)] public uint8_t[] id_or_mac = Enumerable.Range(1, 20).Select(i => new uint8_t()).ToArray();
            public uint8_t operator_id_type = new uint8_t();
            public uint8_t operator_id_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=20)] public uint8_t[] operator_id = Enumerable.Range(1, 20).Select(i => new uint8_t()).ToArray();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
            {
                encode_dronecan_remoteid_OperatorID(this, chunk_cb, ctx);
            }

            public void decode(CanardRxTransfer transfer)
            {
                decode_dronecan_remoteid_OperatorID(transfer, this);
            }

            public static dronecan_remoteid_OperatorID ByteArrayToDroneCANMsg(byte[] transfer, int startoffset)
            {
                var ans = new dronecan_remoteid_OperatorID();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()));
                return ans;
            }
        }
    }
}