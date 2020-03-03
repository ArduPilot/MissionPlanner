
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

public const int COM_HEX_EQUIPMENT_GNSS_MOVINGBASEFIX_MAX_PACK_SIZE = 61;
public const ulong COM_HEX_EQUIPMENT_GNSS_MOVINGBASEFIX_DT_SIG = 0x22930B91F2563B98;
public const int COM_HEX_EQUIPMENT_GNSS_MOVINGBASEFIX_DT_ID = 20211;



public const double COM_HEX_EQUIPMENT_GNSS_MOVINGBASEFIX_CARRIER_SOLUTION_TYPE_NONE = 0; // saturated uint2
public const double COM_HEX_EQUIPMENT_GNSS_MOVINGBASEFIX_CARRIER_SOLUTION_TYPE_FLOAT = 1; // saturated uint2
public const double COM_HEX_EQUIPMENT_GNSS_MOVINGBASEFIX_CARRIER_SOLUTION_TYPE_FIXED = 2; // saturated uint2

public class com_hex_equipment_gnss_MovingBaseFix: IUAVCANSerialize {
    public uavcan_Timestamp timestamp = new uavcan_Timestamp();
    [MarshalAs(UnmanagedType.ByValArray,SizeConst=16)] public uint8_t[] base_in_use_hwid = new uint8_t[16];
    public uint8_t carrier_solution_type = new uint8_t();
    public uint8_t pos_rel_body_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=3)] public Single[] pos_rel_body = new Single[3];
    [MarshalAs(UnmanagedType.ByValArray,SizeConst=3)] public Single[] pos_rel_ecef = new Single[3];
    public uint8_t pos_rel_ned_covariance_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=6)] public Single[] pos_rel_ned_covariance = new Single[6];

public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_com_hex_equipment_gnss_MovingBaseFix(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_com_hex_equipment_gnss_MovingBaseFix(transfer, this);
}

};

}
}