
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
//using uavcan.equipment.gnss.ECEFPositionVelocity.cs

public const int UAVCAN_EQUIPMENT_GNSS_FIX2_MAX_PACK_SIZE = 222;
public const ulong UAVCAN_EQUIPMENT_GNSS_FIX2_DT_SIG = 0xCA41E7000F37435F;
public const int UAVCAN_EQUIPMENT_GNSS_FIX2_DT_ID = 1063;



public const double UAVCAN_EQUIPMENT_GNSS_FIX2_GNSS_TIME_STANDARD_NONE = 0; // saturated uint3
public const double UAVCAN_EQUIPMENT_GNSS_FIX2_GNSS_TIME_STANDARD_TAI = 1; // saturated uint3
public const double UAVCAN_EQUIPMENT_GNSS_FIX2_GNSS_TIME_STANDARD_UTC = 2; // saturated uint3
public const double UAVCAN_EQUIPMENT_GNSS_FIX2_GNSS_TIME_STANDARD_GPS = 3; // saturated uint3
public const double UAVCAN_EQUIPMENT_GNSS_FIX2_NUM_LEAP_SECONDS_UNKNOWN = 0; // saturated uint8
public const double UAVCAN_EQUIPMENT_GNSS_FIX2_STATUS_NO_FIX = 0; // saturated uint2
public const double UAVCAN_EQUIPMENT_GNSS_FIX2_STATUS_TIME_ONLY = 1; // saturated uint2
public const double UAVCAN_EQUIPMENT_GNSS_FIX2_STATUS_2D_FIX = 2; // saturated uint2
public const double UAVCAN_EQUIPMENT_GNSS_FIX2_STATUS_3D_FIX = 3; // saturated uint2
public const double UAVCAN_EQUIPMENT_GNSS_FIX2_MODE_SINGLE = 0; // saturated uint4
public const double UAVCAN_EQUIPMENT_GNSS_FIX2_MODE_DGPS = 1; // saturated uint4
public const double UAVCAN_EQUIPMENT_GNSS_FIX2_MODE_RTK = 2; // saturated uint4
public const double UAVCAN_EQUIPMENT_GNSS_FIX2_MODE_PPP = 3; // saturated uint4
public const double UAVCAN_EQUIPMENT_GNSS_FIX2_SUB_MODE_DGPS_OTHER = 0; // saturated uint6
public const double UAVCAN_EQUIPMENT_GNSS_FIX2_SUB_MODE_DGPS_SBAS = 1; // saturated uint6
public const double UAVCAN_EQUIPMENT_GNSS_FIX2_SUB_MODE_RTK_FLOAT = 0; // saturated uint6
public const double UAVCAN_EQUIPMENT_GNSS_FIX2_SUB_MODE_RTK_FIXED = 1; // saturated uint6

public class uavcan_equipment_gnss_Fix2: IUAVCANSerialize {
    public uavcan_Timestamp timestamp = new uavcan_Timestamp();
    public uavcan_Timestamp gnss_timestamp = new uavcan_Timestamp();
    public uint8_t gnss_time_standard = new uint8_t();
    public uint8_t num_leap_seconds = new uint8_t();
    public int64_t longitude_deg_1e8 = new int64_t();
    public int64_t latitude_deg_1e8 = new int64_t();
    public int32_t height_ellipsoid_mm = new int32_t();
    public int32_t height_msl_mm = new int32_t();
    [MarshalAs(UnmanagedType.ByValArray,SizeConst=3)] public Single[] ned_velocity = new Single[3];
    public uint8_t sats_used = new uint8_t();
    public uint8_t status = new uint8_t();
    public uint8_t mode = new uint8_t();
    public uint8_t sub_mode = new uint8_t();
    public uint8_t covariance_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=36)] public Single[] covariance = new Single[36];
    public Single pdop = new Single();
    public uint8_t ecef_position_velocity_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=1)] public uavcan_equipment_gnss_ECEFPositionVelocity[] ecef_position_velocity = new uavcan_equipment_gnss_ECEFPositionVelocity[1];

public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_uavcan_equipment_gnss_Fix2(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_uavcan_equipment_gnss_Fix2(transfer, this);
}

};

}
}