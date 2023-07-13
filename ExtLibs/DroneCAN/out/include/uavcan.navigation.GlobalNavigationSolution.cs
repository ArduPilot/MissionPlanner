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
//using uavcan.Timestamp.cs
        public partial class uavcan_navigation_GlobalNavigationSolution: IDroneCANSerialize 
        {
            public const int UAVCAN_NAVIGATION_GLOBALNAVIGATIONSOLUTION_MAX_PACK_SIZE = 233;
            public const ulong UAVCAN_NAVIGATION_GLOBALNAVIGATIONSOLUTION_DT_SIG = 0x463B10CCCBE51C3D;
            public const int UAVCAN_NAVIGATION_GLOBALNAVIGATIONSOLUTION_DT_ID = 2000;

            public uavcan_Timestamp timestamp = new uavcan_Timestamp();
            public Double longitude = new Double();
            public Double latitude = new Double();
            public Single height_ellipsoid = new Single();
            public Single height_msl = new Single();
            public Single height_agl = new Single();
            public Single height_baro = new Single();
            public Single qnh_hpa = new Single();
            [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)] public Single[] orientation_xyzw = new Single[4];
            public uint8_t pose_covariance_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=36)] public Single[] pose_covariance = Enumerable.Range(1, 36).Select(i => new Single()).ToArray();
            [MarshalAs(UnmanagedType.ByValArray,SizeConst=3)] public Single[] linear_velocity_body = new Single[3];
            [MarshalAs(UnmanagedType.ByValArray,SizeConst=3)] public Single[] angular_velocity_body = new Single[3];
            [MarshalAs(UnmanagedType.ByValArray,SizeConst=3)] public Single[] linear_acceleration_body = new Single[3];
            public uint8_t velocity_covariance_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=36)] public Single[] velocity_covariance = Enumerable.Range(1, 36).Select(i => new Single()).ToArray();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan = false)
            {
                encode_uavcan_navigation_GlobalNavigationSolution(this, chunk_cb, ctx, fdcan);
            }

            public void decode(CanardRxTransfer transfer, bool fdcan = false)
            {
                decode_uavcan_navigation_GlobalNavigationSolution(transfer, this, fdcan);
            }

            public static uavcan_navigation_GlobalNavigationSolution ByteArrayToDroneCANMsg(byte[] transfer, int startoffset, bool fdcan = false)
            {
                var ans = new uavcan_navigation_GlobalNavigationSolution();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()), fdcan);
                return ans;
            }
        }
    }
}