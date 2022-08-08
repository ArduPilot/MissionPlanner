
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
        public partial class dronecan_remoteid_BasicID: IDroneCANSerialize 
        {
            public const int DRONECAN_REMOTEID_BASICID_MAX_PACK_SIZE = 44;
            public const ulong DRONECAN_REMOTEID_BASICID_DT_SIG = 0x5B1C624A8E4FC533;
            public const int DRONECAN_REMOTEID_BASICID_DT_ID = 20030;

            public const double DRONECAN_REMOTEID_BASICID_ODID_ID_TYPE_NONE = 0; // saturated uint8
            public const double DRONECAN_REMOTEID_BASICID_ODID_ID_TYPE_SERIAL_NUMBER = 1; // saturated uint8
            public const double DRONECAN_REMOTEID_BASICID_ODID_ID_TYPE_CAA_REGISTRATION_ID = 2; // saturated uint8
            public const double DRONECAN_REMOTEID_BASICID_ODID_ID_TYPE_UTM_ASSIGNED_UUID = 3; // saturated uint8
            public const double DRONECAN_REMOTEID_BASICID_ODID_ID_TYPE_SPECIFIC_SESSION_ID = 4; // saturated uint8
            public const double DRONECAN_REMOTEID_BASICID_ODID_UA_TYPE_NONE = 0; // saturated uint8
            public const double DRONECAN_REMOTEID_BASICID_ODID_UA_TYPE_AEROPLANE = 1; // saturated uint8
            public const double DRONECAN_REMOTEID_BASICID_ODID_UA_TYPE_HELICOPTER_OR_MULTIROTOR = 2; // saturated uint8
            public const double DRONECAN_REMOTEID_BASICID_ODID_UA_TYPE_GYROPLANE = 3; // saturated uint8
            public const double DRONECAN_REMOTEID_BASICID_ODID_UA_TYPE_HYBRID_LIFT = 4; // saturated uint8
            public const double DRONECAN_REMOTEID_BASICID_ODID_UA_TYPE_ORNITHOPTER = 5; // saturated uint8
            public const double DRONECAN_REMOTEID_BASICID_ODID_UA_TYPE_GLIDER = 6; // saturated uint8
            public const double DRONECAN_REMOTEID_BASICID_ODID_UA_TYPE_KITE = 7; // saturated uint8
            public const double DRONECAN_REMOTEID_BASICID_ODID_UA_TYPE_FREE_BALLOON = 8; // saturated uint8
            public const double DRONECAN_REMOTEID_BASICID_ODID_UA_TYPE_CAPTIVE_BALLOON = 9; // saturated uint8
            public const double DRONECAN_REMOTEID_BASICID_ODID_UA_TYPE_AIRSHIP = 10; // saturated uint8
            public const double DRONECAN_REMOTEID_BASICID_ODID_UA_TYPE_FREE_FALL_PARACHUTE = 11; // saturated uint8
            public const double DRONECAN_REMOTEID_BASICID_ODID_UA_TYPE_ROCKET = 12; // saturated uint8
            public const double DRONECAN_REMOTEID_BASICID_ODID_UA_TYPE_TETHERED_POWERED_AIRCRAFT = 13; // saturated uint8
            public const double DRONECAN_REMOTEID_BASICID_ODID_UA_TYPE_GROUND_OBSTACLE = 14; // saturated uint8
            public const double DRONECAN_REMOTEID_BASICID_ODID_UA_TYPE_OTHER = 15; // saturated uint8

            public uint8_t id_or_mac_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=20)] public uint8_t[] id_or_mac = Enumerable.Range(1, 20).Select(i => new uint8_t()).ToArray();
            public uint8_t id_type = new uint8_t();
            public uint8_t ua_type = new uint8_t();
            public uint8_t uas_id_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=20)] public uint8_t[] uas_id = Enumerable.Range(1, 20).Select(i => new uint8_t()).ToArray();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
            {
                encode_dronecan_remoteid_BasicID(this, chunk_cb, ctx);
            }

            public void decode(CanardRxTransfer transfer)
            {
                decode_dronecan_remoteid_BasicID(transfer, this);
            }

            public static dronecan_remoteid_BasicID ByteArrayToDroneCANMsg(byte[] transfer, int startoffset)
            {
                var ans = new dronecan_remoteid_BasicID();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()));
                return ans;
            }
        }
    }
}