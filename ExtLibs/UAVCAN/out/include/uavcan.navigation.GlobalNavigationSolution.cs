

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
using System.Runtime.InteropServices;

namespace UAVCAN
{
public partial class uavcan {



//using uavcan.Timestamp.cs


public const int UAVCAN_NAVIGATION_GLOBALNAVIGATIONSOLUTION_MAX_PACK_SIZE = 233;
public const ulong UAVCAN_NAVIGATION_GLOBALNAVIGATIONSOLUTION_DT_SIG = 0x463B10CCCBE51C3D;

public const int UAVCAN_NAVIGATION_GLOBALNAVIGATIONSOLUTION_DT_ID = 2000;






public class uavcan_navigation_GlobalNavigationSolution: IUAVCANSerialize {



    public uavcan_Timestamp timestamp = new uavcan_Timestamp();



    public Double longitude = new Double();



    public Double latitude = new Double();



    public Single height_ellipsoid = new Single();



    public Single height_msl = new Single();



    public Single height_agl = new Single();



    public Single height_baro = new Single();



    public Single qnh_hpa = new Single();



    [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)] public Single[] orientation_xyzw = new Single[4];



    public uint8_t pose_covariance_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=36)] public Single[] pose_covariance = new Single[36];



    [MarshalAs(UnmanagedType.ByValArray,SizeConst=3)] public Single[] linear_velocity_body = new Single[3];



    [MarshalAs(UnmanagedType.ByValArray,SizeConst=3)] public Single[] angular_velocity_body = new Single[3];



    [MarshalAs(UnmanagedType.ByValArray,SizeConst=3)] public Single[] linear_acceleration_body = new Single[3];



    public uint8_t velocity_covariance_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=36)] public Single[] velocity_covariance = new Single[36];




public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_uavcan_navigation_GlobalNavigationSolution(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_uavcan_navigation_GlobalNavigationSolution(transfer, this);
}

};

}
}