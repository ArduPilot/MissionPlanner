

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


public const int UAVCAN_EQUIPMENT_AHRS_SOLUTION_MAX_PACK_SIZE = 84;
public const ulong UAVCAN_EQUIPMENT_AHRS_SOLUTION_DT_SIG = 0x72A63A3C6F41FA9B;

public const int UAVCAN_EQUIPMENT_AHRS_SOLUTION_DT_ID = 1000;






public class uavcan_equipment_ahrs_Solution: IUAVCANSerialize {



    public uavcan_Timestamp timestamp = new uavcan_Timestamp();



    [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)] public Single[] orientation_xyzw = new Single[4];





    public uint8_t orientation_covariance_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=9)] public Single[] orientation_covariance = new Single[9];



    [MarshalAs(UnmanagedType.ByValArray,SizeConst=3)] public Single[] angular_velocity = new Single[3];





    public uint8_t angular_velocity_covariance_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=9)] public Single[] angular_velocity_covariance = new Single[9];



    [MarshalAs(UnmanagedType.ByValArray,SizeConst=3)] public Single[] linear_acceleration = new Single[3];



    public uint8_t linear_acceleration_covariance_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=9)] public Single[] linear_acceleration_covariance = new Single[9];




public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_uavcan_equipment_ahrs_Solution(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_uavcan_equipment_ahrs_Solution(transfer, this);
}

};

}
}