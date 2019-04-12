

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




public const int UAVCAN_EQUIPMENT_AIR_DATA_RAWAIRDATA_MAX_PACK_SIZE = 50;
public const ulong UAVCAN_EQUIPMENT_AIR_DATA_RAWAIRDATA_DT_SIG = 0xC77DF38BA122F5DA;

public const int UAVCAN_EQUIPMENT_AIR_DATA_RAWAIRDATA_DT_ID = 1027;





public const double UAVCAN_EQUIPMENT_AIR_DATA_RAWAIRDATA_FLAG_HEATER_AVAILABLE = 1; // saturated uint8

public const double UAVCAN_EQUIPMENT_AIR_DATA_RAWAIRDATA_FLAG_HEATER_WORKING = 2; // saturated uint8

public const double UAVCAN_EQUIPMENT_AIR_DATA_RAWAIRDATA_FLAG_HEATER_OVERCURRENT = 4; // saturated uint8

public const double UAVCAN_EQUIPMENT_AIR_DATA_RAWAIRDATA_FLAG_HEATER_OPENCIRCUIT = 8; // saturated uint8




public class uavcan_equipment_air_data_RawAirData: IUAVCANSerialize {



    public uint8_t flags = new uint8_t();



    public Single static_pressure = new Single();



    public Single differential_pressure = new Single();



    public Single static_pressure_sensor_temperature = new Single();



    public Single differential_pressure_sensor_temperature = new Single();



    public Single static_air_temperature = new Single();



    public Single pitot_temperature = new Single();



    public uint8_t covariance_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=16)] public Single[] covariance = new Single[16];




public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_uavcan_equipment_air_data_RawAirData(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_uavcan_equipment_air_data_RawAirData(transfer, this);
}

};

}
}