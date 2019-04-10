
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

public const int UAVCAN_EQUIPMENT_GNSS_FIX_MAX_PACK_SIZE = 79;
public const ulong UAVCAN_EQUIPMENT_GNSS_FIX_DT_SIG = 0x54C1572B9E07F297;
public const int UAVCAN_EQUIPMENT_GNSS_FIX_DT_ID = 1060;



public const double UAVCAN_EQUIPMENT_GNSS_FIX_GNSS_TIME_STANDARD_NONE = 0; // saturated uint3
public const double UAVCAN_EQUIPMENT_GNSS_FIX_GNSS_TIME_STANDARD_TAI = 1; // saturated uint3
public const double UAVCAN_EQUIPMENT_GNSS_FIX_GNSS_TIME_STANDARD_UTC = 2; // saturated uint3
public const double UAVCAN_EQUIPMENT_GNSS_FIX_GNSS_TIME_STANDARD_GPS = 3; // saturated uint3
public const double UAVCAN_EQUIPMENT_GNSS_FIX_NUM_LEAP_SECONDS_UNKNOWN = 0; // saturated uint8
public const double UAVCAN_EQUIPMENT_GNSS_FIX_STATUS_NO_FIX = 0; // saturated uint2
public const double UAVCAN_EQUIPMENT_GNSS_FIX_STATUS_TIME_ONLY = 1; // saturated uint2
public const double UAVCAN_EQUIPMENT_GNSS_FIX_STATUS_2D_FIX = 2; // saturated uint2
public const double UAVCAN_EQUIPMENT_GNSS_FIX_STATUS_3D_FIX = 3; // saturated uint2

public class uavcan_equipment_gnss_Fix: IUAVCANSerialize {
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
    public Single pdop = new Single();
    public uint8_t position_covariance_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=9)] public Single[] position_covariance = new Single[9];
    public uint8_t velocity_covariance_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=9)] public Single[] velocity_covariance = new Single[9];

public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_uavcan_equipment_gnss_Fix(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_uavcan_equipment_gnss_Fix(transfer, this);
}

};

}
}