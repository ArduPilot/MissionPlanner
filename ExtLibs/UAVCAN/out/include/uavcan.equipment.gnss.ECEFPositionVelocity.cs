

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




public const int UAVCAN_EQUIPMENT_GNSS_ECEFPOSITIONVELOCITY_MAX_PACK_SIZE = 99;
public const ulong UAVCAN_EQUIPMENT_GNSS_ECEFPOSITIONVELOCITY_DT_SIG = 0x24A5DA4ABEE3A248;






public class uavcan_equipment_gnss_ECEFPositionVelocity: IUAVCANSerialize {



    [MarshalAs(UnmanagedType.ByValArray,SizeConst=3)] public Single[] velocity_xyz = new Single[3];



    [MarshalAs(UnmanagedType.ByValArray,SizeConst=3)] public int64_t[] position_xyz_mm = new int64_t[3];





    public uint8_t covariance_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=36)] public Single[] covariance = new Single[36];




public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_uavcan_equipment_gnss_ECEFPositionVelocity(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_uavcan_equipment_gnss_ECEFPositionVelocity(transfer, this);
}

};

}
}