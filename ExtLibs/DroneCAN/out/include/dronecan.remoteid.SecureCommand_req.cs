
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
        public partial class dronecan_remoteid_SecureCommand_req: IDroneCANSerialize 
        {
            public const int DRONECAN_REMOTEID_SECURECOMMAND_REQ_MAX_PACK_SIZE = 230;
            public const ulong DRONECAN_REMOTEID_SECURECOMMAND_REQ_DT_SIG = 0x126A47C9C17A8BD7;
            public const int DRONECAN_REMOTEID_SECURECOMMAND_REQ_DT_ID = 64;

            public const double DRONECAN_REMOTEID_SECURECOMMAND_REQ_SECURE_COMMAND_GET_SESSION_KEY = 0; // saturated uint32
            public const double DRONECAN_REMOTEID_SECURECOMMAND_REQ_SECURE_COMMAND_GET_REMOTEID_SESSION_KEY = 1; // saturated uint32
            public const double DRONECAN_REMOTEID_SECURECOMMAND_REQ_SECURE_COMMAND_REMOVE_PUBLIC_KEYS = 2; // saturated uint32
            public const double DRONECAN_REMOTEID_SECURECOMMAND_REQ_SECURE_COMMAND_GET_PUBLIC_KEYS = 3; // saturated uint32
            public const double DRONECAN_REMOTEID_SECURECOMMAND_REQ_SECURE_COMMAND_SET_PUBLIC_KEYS = 4; // saturated uint32
            public const double DRONECAN_REMOTEID_SECURECOMMAND_REQ_SECURE_COMMAND_GET_REMOTEID_CONFIG = 5; // saturated uint32
            public const double DRONECAN_REMOTEID_SECURECOMMAND_REQ_SECURE_COMMAND_SET_REMOTEID_CONFIG = 6; // saturated uint32

            public uint32_t sequence = new uint32_t();
            public uint32_t operation = new uint32_t();
            public uint8_t sig_length = new uint8_t();
            public uint8_t data_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=220)] public uint8_t[] data = Enumerable.Range(1, 220).Select(i => new uint8_t()).ToArray();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
            {
                encode_dronecan_remoteid_SecureCommand_req(this, chunk_cb, ctx);
            }

            public void decode(CanardRxTransfer transfer)
            {
                decode_dronecan_remoteid_SecureCommand_req(transfer, this);
            }

            public static dronecan_remoteid_SecureCommand_req ByteArrayToDroneCANMsg(byte[] transfer, int startoffset)
            {
                var ans = new dronecan_remoteid_SecureCommand_req();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()));
                return ans;
            }
        }
    }
}