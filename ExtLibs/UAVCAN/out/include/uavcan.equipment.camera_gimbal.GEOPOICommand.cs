
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

public const int UAVCAN_EQUIPMENT_CAMERA_GIMBAL_GEOPOICOMMAND_MAX_PACK_SIZE = 13;
public const ulong UAVCAN_EQUIPMENT_CAMERA_GIMBAL_GEOPOICOMMAND_DT_SIG = 0x9371428A92F01FD6;
public const int UAVCAN_EQUIPMENT_CAMERA_GIMBAL_GEOPOICOMMAND_DT_ID = 1041;



public const double UAVCAN_EQUIPMENT_CAMERA_GIMBAL_GEOPOICOMMAND_HEIGHT_REFERENCE_ELLIPSOID = 0; // saturated uint2
public const double UAVCAN_EQUIPMENT_CAMERA_GIMBAL_GEOPOICOMMAND_HEIGHT_REFERENCE_MEAN_SEA_LEVEL = 1; // saturated uint2

public class uavcan_equipment_camera_gimbal_GEOPOICommand: IUAVCANSerialize {
    public uint8_t gimbal_id = new uint8_t();
    public uavcan_equipment_camera_gimbal_Mode mode = new uavcan_equipment_camera_gimbal_Mode();
    public int32_t longitude_deg_1e7 = new int32_t();
    public int32_t latitude_deg_1e7 = new int32_t();
    public int32_t height_cm = new int32_t();
    public uint8_t height_reference = new uint8_t();

public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_uavcan_equipment_camera_gimbal_GEOPOICommand(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_uavcan_equipment_camera_gimbal_GEOPOICommand(transfer, this);
}

};

}
}