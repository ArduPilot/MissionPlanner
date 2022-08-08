
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
        public partial class dronecan_remoteid_ArmStatus: IDroneCANSerialize 
        {
            public const int DRONECAN_REMOTEID_ARMSTATUS_MAX_PACK_SIZE = 52;
            public const ulong DRONECAN_REMOTEID_ARMSTATUS_DT_SIG = 0xFEDF72CCF06F3BDD;
            public const int DRONECAN_REMOTEID_ARMSTATUS_DT_ID = 20035;

            public const double DRONECAN_REMOTEID_ARMSTATUS_ODID_ARM_STATUS_GOOD_TO_ARM = 0; // saturated uint8
            public const double DRONECAN_REMOTEID_ARMSTATUS_ODID_ARM_STATUS_FAIL_GENERIC = 1; // saturated uint8

            public uint8_t status = new uint8_t();
            public uint8_t error_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=50)] public uint8_t[] error = Enumerable.Range(1, 50).Select(i => new uint8_t()).ToArray();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
            {
                encode_dronecan_remoteid_ArmStatus(this, chunk_cb, ctx);
            }

            public void decode(CanardRxTransfer transfer)
            {
                decode_dronecan_remoteid_ArmStatus(transfer, this);
            }

            public static dronecan_remoteid_ArmStatus ByteArrayToDroneCANMsg(byte[] transfer, int startoffset)
            {
                var ans = new dronecan_remoteid_ArmStatus();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()));
                return ans;
            }
        }
    }
}