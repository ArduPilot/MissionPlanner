

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



//using uavcan.equipment.camera_gimbal.Mode.cs


public const int UAVCAN_EQUIPMENT_CAMERA_GIMBAL_ANGULARCOMMAND_MAX_PACK_SIZE = 10;
public const ulong UAVCAN_EQUIPMENT_CAMERA_GIMBAL_ANGULARCOMMAND_DT_SIG = 0x4AF6E57B2B2BE29C;

public const int UAVCAN_EQUIPMENT_CAMERA_GIMBAL_ANGULARCOMMAND_DT_ID = 1040;






public class uavcan_equipment_camera_gimbal_AngularCommand: IUAVCANSerialize {



    public uint8_t gimbal_id = new uint8_t();



    public uavcan_equipment_camera_gimbal_Mode mode = new uavcan_equipment_camera_gimbal_Mode();



    [MarshalAs(UnmanagedType.ByValArray,SizeConst=4)] public Single[] quaternion_xyzw = new Single[4];




public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_uavcan_equipment_camera_gimbal_AngularCommand(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_uavcan_equipment_camera_gimbal_AngularCommand(transfer, this);
}

};

}
}