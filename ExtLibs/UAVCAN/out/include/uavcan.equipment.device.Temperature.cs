

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




public const int UAVCAN_EQUIPMENT_DEVICE_TEMPERATURE_MAX_PACK_SIZE = 5;
public const ulong UAVCAN_EQUIPMENT_DEVICE_TEMPERATURE_DT_SIG = 0x70261C28A94144C6;

public const int UAVCAN_EQUIPMENT_DEVICE_TEMPERATURE_DT_ID = 1110;





public const double UAVCAN_EQUIPMENT_DEVICE_TEMPERATURE_ERROR_FLAG_OVERHEATING = 1; // saturated uint8

public const double UAVCAN_EQUIPMENT_DEVICE_TEMPERATURE_ERROR_FLAG_OVERCOOLING = 2; // saturated uint8




public class uavcan_equipment_device_Temperature: IUAVCANSerialize {



    public uint16_t device_id = new uint16_t();



    public Single temperature = new Single();



    public uint8_t error_flags = new uint8_t();




public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_uavcan_equipment_device_Temperature(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_uavcan_equipment_device_Temperature(transfer, this);
}

};

}
}