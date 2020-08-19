
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
using System.Linq;
using System.Runtime.InteropServices;

namespace UAVCAN
{
public partial class uavcan {

//using uavcan.Timestamp.cs

public const int ORG_ARDUPILOT_SENSOR_OUTPUT_IMUDELTAS_MAX_PACK_SIZE = 40;
public const ulong ORG_ARDUPILOT_SENSOR_OUTPUT_IMUDELTAS_DT_SIG = 0x63C61262BD84919D;
public const int ORG_ARDUPILOT_SENSOR_OUTPUT_IMUDELTAS_DT_ID = 20900;



public class org_ardupilot_sensor_output_IMUDeltas: IUAVCANSerialize {
    public uavcan_Timestamp timestamp = new uavcan_Timestamp();
    public uint8_t sensor_index = new uint8_t();
    public Single delta_angle_dt = new Single();
    [MarshalAs(UnmanagedType.ByValArray,SizeConst=3)] public Single[] delta_angle = new Single[3];
    public Single delta_velocity_dt = new Single();
    [MarshalAs(UnmanagedType.ByValArray,SizeConst=3)] public Single[] delta_velocity = new Single[3];

public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_org_ardupilot_sensor_output_IMUDeltas(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_org_ardupilot_sensor_output_IMUDeltas(transfer, this);
}

};

}
}