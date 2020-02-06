
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


public const int UAVCAN_EQUIPMENT_AIR_DATA_ANGLEOFATTACK_MAX_PACK_SIZE = 5;
public const ulong UAVCAN_EQUIPMENT_AIR_DATA_ANGLEOFATTACK_DT_SIG = 0xD5513C3F7AFAC74E;
public const int UAVCAN_EQUIPMENT_AIR_DATA_ANGLEOFATTACK_DT_ID = 1025;



public const double UAVCAN_EQUIPMENT_AIR_DATA_ANGLEOFATTACK_SENSOR_ID_LEFT = 254; // saturated uint8
public const double UAVCAN_EQUIPMENT_AIR_DATA_ANGLEOFATTACK_SENSOR_ID_RIGHT = 255; // saturated uint8

public class uavcan_equipment_air_data_AngleOfAttack: IUAVCANSerialize {
    public uint8_t sensor_id = new uint8_t();
    public Single aoa = new Single();
    public Single aoa_variance = new Single();

public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_uavcan_equipment_air_data_AngleOfAttack(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_uavcan_equipment_air_data_AngleOfAttack(transfer, this);
}

};

}
}