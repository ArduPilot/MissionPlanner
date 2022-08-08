
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
        public partial class dronecan_remoteid_SelfID: IDroneCANSerialize 
        {
            public const int DRONECAN_REMOTEID_SELFID_MAX_PACK_SIZE = 46;
            public const ulong DRONECAN_REMOTEID_SELFID_DT_SIG = 0x59BE81DC4C06A185;
            public const int DRONECAN_REMOTEID_SELFID_DT_ID = 20032;

            public const double DRONECAN_REMOTEID_SELFID_ODID_DESC_TYPE_TEXT = 0; // saturated uint8

            public uint8_t id_or_mac_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=20)] public uint8_t[] id_or_mac = Enumerable.Range(1, 20).Select(i => new uint8_t()).ToArray();
            public uint8_t description_type = new uint8_t();
            public uint8_t description_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=23)] public uint8_t[] description = Enumerable.Range(1, 23).Select(i => new uint8_t()).ToArray();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
            {
                encode_dronecan_remoteid_SelfID(this, chunk_cb, ctx);
            }

            public void decode(CanardRxTransfer transfer)
            {
                decode_dronecan_remoteid_SelfID(transfer, this);
            }

            public static dronecan_remoteid_SelfID ByteArrayToDroneCANMsg(byte[] transfer, int startoffset)
            {
                var ans = new dronecan_remoteid_SelfID();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()));
                return ans;
            }
        }
    }
}