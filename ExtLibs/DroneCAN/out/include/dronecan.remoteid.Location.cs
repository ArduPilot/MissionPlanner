
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
        public partial class dronecan_remoteid_Location: IDroneCANSerialize 
        {
            public const int DRONECAN_REMOTEID_LOCATION_MAX_PACK_SIZE = 58;
            public const ulong DRONECAN_REMOTEID_LOCATION_DT_SIG = 0xEAA3A2C5BCB14CAA;
            public const int DRONECAN_REMOTEID_LOCATION_DT_ID = 20031;

            public const double DRONECAN_REMOTEID_LOCATION_ODID_STATUS_UNDECLARED = 0; // saturated uint8
            public const double DRONECAN_REMOTEID_LOCATION_ODID_STATUS_GROUND = 1; // saturated uint8
            public const double DRONECAN_REMOTEID_LOCATION_ODID_STATUS_AIRBORNE = 2; // saturated uint8
            public const double DRONECAN_REMOTEID_LOCATION_ODID_STATUS_EMERGENCY = 3; // saturated uint8
            public const double DRONECAN_REMOTEID_LOCATION_ODID_HEIGHT_REF_OVER_TAKEOFF = 0; // saturated uint8
            public const double DRONECAN_REMOTEID_LOCATION_ODID_HEIGHT_REF_OVER_GROUND = 1; // saturated uint8
            public const double DRONECAN_REMOTEID_LOCATION_ODID_HOR_ACC_UNKNOWN = 0; // saturated uint8
            public const double DRONECAN_REMOTEID_LOCATION_ODID_HOR_ACC_10NM = 1; // saturated uint8
            public const double DRONECAN_REMOTEID_LOCATION_ODID_HOR_ACC_4NM = 2; // saturated uint8
            public const double DRONECAN_REMOTEID_LOCATION_ODID_HOR_ACC_2NM = 3; // saturated uint8
            public const double DRONECAN_REMOTEID_LOCATION_ODID_HOR_ACC_1NM = 4; // saturated uint8
            public const double DRONECAN_REMOTEID_LOCATION_ODID_HOR_ACC_0_5NM = 5; // saturated uint8
            public const double DRONECAN_REMOTEID_LOCATION_ODID_HOR_ACC_0_3NM = 6; // saturated uint8
            public const double DRONECAN_REMOTEID_LOCATION_ODID_HOR_ACC_0_1NM = 7; // saturated uint8
            public const double DRONECAN_REMOTEID_LOCATION_ODID_HOR_ACC_0_05NM = 8; // saturated uint8
            public const double DRONECAN_REMOTEID_LOCATION_ODID_HOR_ACC_30_METER = 9; // saturated uint8
            public const double DRONECAN_REMOTEID_LOCATION_ODID_HOR_ACC_10_METER = 10; // saturated uint8
            public const double DRONECAN_REMOTEID_LOCATION_ODID_HOR_ACC_3_METER = 11; // saturated uint8
            public const double DRONECAN_REMOTEID_LOCATION_ODID_HOR_ACC_1_METER = 12; // saturated uint8
            public const double DRONECAN_REMOTEID_LOCATION_ODID_VER_ACC_UNKNOWN = 0; // saturated uint8
            public const double DRONECAN_REMOTEID_LOCATION_ODID_VER_ACC_150_METER = 1; // saturated uint8
            public const double DRONECAN_REMOTEID_LOCATION_ODID_VER_ACC_45_METER = 2; // saturated uint8
            public const double DRONECAN_REMOTEID_LOCATION_ODID_VER_ACC_25_METER = 3; // saturated uint8
            public const double DRONECAN_REMOTEID_LOCATION_ODID_VER_ACC_10_METER = 4; // saturated uint8
            public const double DRONECAN_REMOTEID_LOCATION_ODID_VER_ACC_3_METER = 5; // saturated uint8
            public const double DRONECAN_REMOTEID_LOCATION_ODID_VER_ACC_1_METER = 6; // saturated uint8
            public const double DRONECAN_REMOTEID_LOCATION_ODID_SPEED_ACC_UNKNOWN = 0; // saturated uint8
            public const double DRONECAN_REMOTEID_LOCATION_ODID_SPEED_ACC_10_METERS_PER_SECOND = 1; // saturated uint8
            public const double DRONECAN_REMOTEID_LOCATION_ODID_SPEED_ACC_3_METERS_PER_SECOND = 2; // saturated uint8
            public const double DRONECAN_REMOTEID_LOCATION_ODID_SPEED_ACC_1_METERS_PER_SECOND = 3; // saturated uint8
            public const double DRONECAN_REMOTEID_LOCATION_ODID_SPEED_ACC_0_3_METERS_PER_SECOND = 4; // saturated uint8
            public const double DRONECAN_REMOTEID_LOCATION_ODID_TIME_ACC_0_1_SECOND = 1; // saturated uint8
            public const double DRONECAN_REMOTEID_LOCATION_ODID_TIME_ACC_0_2_SECOND = 2; // saturated uint8
            public const double DRONECAN_REMOTEID_LOCATION_ODID_TIME_ACC_0_3_SECOND = 3; // saturated uint8
            public const double DRONECAN_REMOTEID_LOCATION_ODID_TIME_ACC_0_4_SECOND = 4; // saturated uint8
            public const double DRONECAN_REMOTEID_LOCATION_ODID_TIME_ACC_0_5_SECOND = 5; // saturated uint8
            public const double DRONECAN_REMOTEID_LOCATION_ODID_TIME_ACC_0_6_SECOND = 6; // saturated uint8
            public const double DRONECAN_REMOTEID_LOCATION_ODID_TIME_ACC_0_7_SECOND = 7; // saturated uint8
            public const double DRONECAN_REMOTEID_LOCATION_ODID_TIME_ACC_0_8_SECOND = 8; // saturated uint8
            public const double DRONECAN_REMOTEID_LOCATION_ODID_TIME_ACC_0_9_SECOND = 9; // saturated uint8
            public const double DRONECAN_REMOTEID_LOCATION_ODID_TIME_ACC_1_0_SECOND = 10; // saturated uint8
            public const double DRONECAN_REMOTEID_LOCATION_ODID_TIME_ACC_1_1_SECOND = 11; // saturated uint8
            public const double DRONECAN_REMOTEID_LOCATION_ODID_TIME_ACC_1_2_SECOND = 12; // saturated uint8
            public const double DRONECAN_REMOTEID_LOCATION_ODID_TIME_ACC_1_3_SECOND = 13; // saturated uint8
            public const double DRONECAN_REMOTEID_LOCATION_ODID_TIME_ACC_1_4_SECOND = 14; // saturated uint8
            public const double DRONECAN_REMOTEID_LOCATION_ODID_TIME_ACC_1_5_SECOND = 15; // saturated uint8

            public uint8_t id_or_mac_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=20)] public uint8_t[] id_or_mac = Enumerable.Range(1, 20).Select(i => new uint8_t()).ToArray();
            public uint8_t status = new uint8_t();
            public uint16_t direction = new uint16_t();
            public uint16_t speed_horizontal = new uint16_t();
            public int16_t speed_vertical = new int16_t();
            public int32_t latitude = new int32_t();
            public int32_t longitude = new int32_t();
            public Single altitude_barometric = new Single();
            public Single altitude_geodetic = new Single();
            public uint8_t height_reference = new uint8_t();
            public Single height = new Single();
            public uint8_t horizontal_accuracy = new uint8_t();
            public uint8_t vertical_accuracy = new uint8_t();
            public uint8_t barometer_accuracy = new uint8_t();
            public uint8_t speed_accuracy = new uint8_t();
            public Single timestamp = new Single();
            public uint8_t timestamp_accuracy = new uint8_t();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
            {
                encode_dronecan_remoteid_Location(this, chunk_cb, ctx);
            }

            public void decode(CanardRxTransfer transfer)
            {
                decode_dronecan_remoteid_Location(transfer, this);
            }

            public static dronecan_remoteid_Location ByteArrayToDroneCANMsg(byte[] transfer, int startoffset)
            {
                var ans = new dronecan_remoteid_Location();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()));
                return ans;
            }
        }
    }
}