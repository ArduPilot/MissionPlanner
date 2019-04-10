
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
//using uavcan.CoarseOrientation.cs

public const int UAVCAN_EQUIPMENT_RANGE_SENSOR_MEASUREMENT_MAX_PACK_SIZE = 15;
public const ulong UAVCAN_EQUIPMENT_RANGE_SENSOR_MEASUREMENT_DT_SIG = 0x68FFFE70FC771952;
public const int UAVCAN_EQUIPMENT_RANGE_SENSOR_MEASUREMENT_DT_ID = 1050;



public const double UAVCAN_EQUIPMENT_RANGE_SENSOR_MEASUREMENT_SENSOR_TYPE_UNDEFINED = 0; // saturated uint5
public const double UAVCAN_EQUIPMENT_RANGE_SENSOR_MEASUREMENT_SENSOR_TYPE_SONAR = 1; // saturated uint5
public const double UAVCAN_EQUIPMENT_RANGE_SENSOR_MEASUREMENT_SENSOR_TYPE_LIDAR = 2; // saturated uint5
public const double UAVCAN_EQUIPMENT_RANGE_SENSOR_MEASUREMENT_SENSOR_TYPE_RADAR = 3; // saturated uint5
public const double UAVCAN_EQUIPMENT_RANGE_SENSOR_MEASUREMENT_READING_TYPE_UNDEFINED = 0; // saturated uint3
public const double UAVCAN_EQUIPMENT_RANGE_SENSOR_MEASUREMENT_READING_TYPE_VALID_RANGE = 1; // saturated uint3
public const double UAVCAN_EQUIPMENT_RANGE_SENSOR_MEASUREMENT_READING_TYPE_TOO_CLOSE = 2; // saturated uint3
public const double UAVCAN_EQUIPMENT_RANGE_SENSOR_MEASUREMENT_READING_TYPE_TOO_FAR = 3; // saturated uint3

public class uavcan_equipment_range_sensor_Measurement: IUAVCANSerialize {
    public uavcan_Timestamp timestamp = new uavcan_Timestamp();
    public uint8_t sensor_id = new uint8_t();
    public uavcan_CoarseOrientation beam_orientation_in_body_frame = new uavcan_CoarseOrientation();
    public Single field_of_view = new Single();
    public uint8_t sensor_type = new uint8_t();
    public uint8_t reading_type = new uint8_t();
    public Single range = new Single();

public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_uavcan_equipment_range_sensor_Measurement(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_uavcan_equipment_range_sensor_Measurement(transfer, this);
}

};

}
}