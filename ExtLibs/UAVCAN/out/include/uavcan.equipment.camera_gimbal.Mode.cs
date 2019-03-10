

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




public const int UAVCAN_EQUIPMENT_CAMERA_GIMBAL_MODE_MAX_PACK_SIZE = 1;
public const ulong UAVCAN_EQUIPMENT_CAMERA_GIMBAL_MODE_DT_SIG = 0x9108C7785AEB69C4;





public const double UAVCAN_EQUIPMENT_CAMERA_GIMBAL_MODE_COMMAND_MODE_ANGULAR_VELOCITY = 0; // saturated uint8

public const double UAVCAN_EQUIPMENT_CAMERA_GIMBAL_MODE_COMMAND_MODE_ORIENTATION_FIXED_FRAME = 1; // saturated uint8

public const double UAVCAN_EQUIPMENT_CAMERA_GIMBAL_MODE_COMMAND_MODE_ORIENTATION_BODY_FRAME = 2; // saturated uint8

public const double UAVCAN_EQUIPMENT_CAMERA_GIMBAL_MODE_COMMAND_MODE_GEO_POI = 3; // saturated uint8




public class uavcan_equipment_camera_gimbal_Mode: IUAVCANSerialize {



    public uint8_t command_mode = new uint8_t();




public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_uavcan_equipment_camera_gimbal_Mode(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_uavcan_equipment_camera_gimbal_Mode(transfer, this);
}

};

}
}