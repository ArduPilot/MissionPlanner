

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




public const int UAVCAN_COARSEORIENTATION_MAX_PACK_SIZE = 2;
public const ulong UAVCAN_COARSEORIENTATION_DT_SIG = 0x271BA10B0DAC9E52;





public const double UAVCAN_COARSEORIENTATION_ANGLE_MULTIPLIER = 4.77464829276; // saturated float32




public class uavcan_CoarseOrientation: IUAVCANSerialize {



    [MarshalAs(UnmanagedType.ByValArray,SizeConst=3)] public int8_t[] fixed_axis_roll_pitch_yaw = new int8_t[3];



    public bool orientation_defined = new bool();




public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_uavcan_CoarseOrientation(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_uavcan_CoarseOrientation(transfer, this);
}

};

}
}