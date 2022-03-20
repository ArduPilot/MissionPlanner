
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
//using uavcan.olliw.uc4h.DistanceSensorProperties.cs
        public partial class uavcan_olliw_uc4h_Distance: IDroneCANSerialize 
        {
            public const int UAVCAN_OLLIW_UC4H_DISTANCE_MAX_PACK_SIZE = 13;
            public const ulong UAVCAN_OLLIW_UC4H_DISTANCE_DT_SIG = 0xE12C901C6174B583;
            public const int UAVCAN_OLLIW_UC4H_DISTANCE_DT_ID = 28350;

            public const double UAVCAN_OLLIW_UC4H_DISTANCE_ANGLE_MULTIPLIER = 4.7746482927568605; // saturated float32
            public const double UAVCAN_OLLIW_UC4H_DISTANCE_RANGE_INVALID = 0; // saturated uint3
            public const double UAVCAN_OLLIW_UC4H_DISTANCE_RANGE_VALID = 1; // saturated uint3
            public const double UAVCAN_OLLIW_UC4H_DISTANCE_RANGE_TOO_CLOSE = 2; // saturated uint3
            public const double UAVCAN_OLLIW_UC4H_DISTANCE_RANGE_TOO_FAR = 3; // saturated uint3

            public int8_t fixed_axis_pitch = new int8_t();
            public int8_t fixed_axis_yaw = new int8_t();
            public uint8_t sensor_sub_id = new uint8_t();
            public uint8_t range_flag = new uint8_t();
            public Single range = new Single();
            public uint8_t sensor_property_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=1)] public uavcan_olliw_uc4h_DistanceSensorProperties[] sensor_property = Enumerable.Range(1, 1).Select(i => new uavcan_olliw_uc4h_DistanceSensorProperties()).ToArray();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
            {
                encode_uavcan_olliw_uc4h_Distance(this, chunk_cb, ctx);
            }

            public void decode(CanardRxTransfer transfer)
            {
                decode_uavcan_olliw_uc4h_Distance(transfer, this);
            }

            public static uavcan_olliw_uc4h_Distance ByteArrayToDroneCANMsg(byte[] transfer, int startoffset)
            {
                var ans = new uavcan_olliw_uc4h_Distance();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()));
                return ans;
            }
        }
    }
}