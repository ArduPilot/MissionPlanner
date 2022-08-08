
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
        public partial class dronecan_remoteid_System: IDroneCANSerialize 
        {
            public const int DRONECAN_REMOTEID_SYSTEM_MAX_PACK_SIZE = 53;
            public const ulong DRONECAN_REMOTEID_SYSTEM_DT_SIG = 0x9AC872F49BF32437;
            public const int DRONECAN_REMOTEID_SYSTEM_DT_ID = 20033;

            public const double DRONECAN_REMOTEID_SYSTEM_ODID_OPERATOR_LOCATION_TYPE_TAKEOFF = 0; // saturated uint8
            public const double DRONECAN_REMOTEID_SYSTEM_ODID_OPERATOR_LOCATION_TYPE_LIVE_GNSS = 1; // saturated uint8
            public const double DRONECAN_REMOTEID_SYSTEM_ODID_OPERATOR_LOCATION_TYPE_FIXED = 2; // saturated uint8
            public const double DRONECAN_REMOTEID_SYSTEM_ODID_CLASSIFICATION_TYPE_EU = 0; // saturated uint8
            public const double DRONECAN_REMOTEID_SYSTEM_ODID_CATEGORY_EU_UNDECLARED = 0; // saturated uint8
            public const double DRONECAN_REMOTEID_SYSTEM_ODID_CATEGORY_EU_OPEN = 1; // saturated uint8
            public const double DRONECAN_REMOTEID_SYSTEM_ODID_CATEGORY_EU_SPECIFIC = 2; // saturated uint8
            public const double DRONECAN_REMOTEID_SYSTEM_ODID_CATEGORY_EU_CERTIFIED = 3; // saturated uint8
            public const double DRONECAN_REMOTEID_SYSTEM_ODID_CLASS_EU_UNDECLARED = 0; // saturated uint8
            public const double DRONECAN_REMOTEID_SYSTEM_ODID_CLASS_EU_CLASS_0 = 1; // saturated uint8
            public const double DRONECAN_REMOTEID_SYSTEM_ODID_CLASS_EU_CLASS_1 = 2; // saturated uint8
            public const double DRONECAN_REMOTEID_SYSTEM_ODID_CLASS_EU_CLASS_2 = 3; // saturated uint8
            public const double DRONECAN_REMOTEID_SYSTEM_ODID_CLASS_EU_CLASS_3 = 4; // saturated uint8
            public const double DRONECAN_REMOTEID_SYSTEM_ODID_CLASS_EU_CLASS_4 = 5; // saturated uint8
            public const double DRONECAN_REMOTEID_SYSTEM_ODID_CLASS_EU_CLASS_5 = 6; // saturated uint8
            public const double DRONECAN_REMOTEID_SYSTEM_ODID_CLASS_EU_CLASS_6 = 7; // saturated uint8

            public uint8_t id_or_mac_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=20)] public uint8_t[] id_or_mac = Enumerable.Range(1, 20).Select(i => new uint8_t()).ToArray();
            public uint8_t operator_location_type = new uint8_t();
            public uint8_t classification_type = new uint8_t();
            public int32_t operator_latitude = new int32_t();
            public int32_t operator_longitude = new int32_t();
            public uint16_t area_count = new uint16_t();
            public uint16_t area_radius = new uint16_t();
            public Single area_ceiling = new Single();
            public Single area_floor = new Single();
            public uint8_t category_eu = new uint8_t();
            public uint8_t class_eu = new uint8_t();
            public Single operator_altitude_geo = new Single();
            public uint32_t timestamp = new uint32_t();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
            {
                encode_dronecan_remoteid_System(this, chunk_cb, ctx);
            }

            public void decode(CanardRxTransfer transfer)
            {
                decode_dronecan_remoteid_System(transfer, this);
            }

            public static dronecan_remoteid_System ByteArrayToDroneCANMsg(byte[] transfer, int startoffset)
            {
                var ans = new dronecan_remoteid_System();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()));
                return ans;
            }
        }
    }
}