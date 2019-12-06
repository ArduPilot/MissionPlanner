

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




public const int UAVCAN_EQUIPMENT_AHRS_MAGNETICFIELDSTRENGTH_MAX_PACK_SIZE = 25;
public const ulong UAVCAN_EQUIPMENT_AHRS_MAGNETICFIELDSTRENGTH_DT_SIG = 0xE2A7D4A9460BC2F2;

public const int UAVCAN_EQUIPMENT_AHRS_MAGNETICFIELDSTRENGTH_DT_ID = 1001;






public class uavcan_equipment_ahrs_MagneticFieldStrength: IUAVCANSerialize {



    [MarshalAs(UnmanagedType.ByValArray,SizeConst=3)] public Single[] magnetic_field_ga = new Single[3];



    public uint8_t magnetic_field_covariance_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=9)] public Single[] magnetic_field_covariance = new Single[9];




public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_uavcan_equipment_ahrs_MagneticFieldStrength(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_uavcan_equipment_ahrs_MagneticFieldStrength(transfer, this);
}

};

}
}