

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



//using uavcan.equipment.ice.reciprocating.CylinderStatus.cs


public const int UAVCAN_EQUIPMENT_ICE_RECIPROCATING_STATUS_MAX_PACK_SIZE = 196;
public const ulong UAVCAN_EQUIPMENT_ICE_RECIPROCATING_STATUS_DT_SIG = 0xD38AA3EE75537EC6;

public const int UAVCAN_EQUIPMENT_ICE_RECIPROCATING_STATUS_DT_ID = 1120;





public const double UAVCAN_EQUIPMENT_ICE_RECIPROCATING_STATUS_STATE_STOPPED = 0; // saturated uint2

public const double UAVCAN_EQUIPMENT_ICE_RECIPROCATING_STATUS_STATE_STARTING = 1; // saturated uint2

public const double UAVCAN_EQUIPMENT_ICE_RECIPROCATING_STATUS_STATE_RUNNING = 2; // saturated uint2

public const double UAVCAN_EQUIPMENT_ICE_RECIPROCATING_STATUS_STATE_FAULT = 3; // saturated uint2

public const double UAVCAN_EQUIPMENT_ICE_RECIPROCATING_STATUS_FLAG_GENERAL_ERROR = 1; // saturated uint30

public const double UAVCAN_EQUIPMENT_ICE_RECIPROCATING_STATUS_FLAG_CRANKSHAFT_SENSOR_ERROR_SUPPORTED = 2; // saturated uint30

public const double UAVCAN_EQUIPMENT_ICE_RECIPROCATING_STATUS_FLAG_CRANKSHAFT_SENSOR_ERROR = 4; // saturated uint30

public const double UAVCAN_EQUIPMENT_ICE_RECIPROCATING_STATUS_FLAG_TEMPERATURE_SUPPORTED = 8; // saturated uint30

public const double UAVCAN_EQUIPMENT_ICE_RECIPROCATING_STATUS_FLAG_TEMPERATURE_BELOW_NOMINAL = 16; // saturated uint30

public const double UAVCAN_EQUIPMENT_ICE_RECIPROCATING_STATUS_FLAG_TEMPERATURE_ABOVE_NOMINAL = 32; // saturated uint30

public const double UAVCAN_EQUIPMENT_ICE_RECIPROCATING_STATUS_FLAG_TEMPERATURE_OVERHEATING = 64; // saturated uint30

public const double UAVCAN_EQUIPMENT_ICE_RECIPROCATING_STATUS_FLAG_TEMPERATURE_EGT_ABOVE_NOMINAL = 128; // saturated uint30

public const double UAVCAN_EQUIPMENT_ICE_RECIPROCATING_STATUS_FLAG_FUEL_PRESSURE_SUPPORTED = 256; // saturated uint30

public const double UAVCAN_EQUIPMENT_ICE_RECIPROCATING_STATUS_FLAG_FUEL_PRESSURE_BELOW_NOMINAL = 512; // saturated uint30

public const double UAVCAN_EQUIPMENT_ICE_RECIPROCATING_STATUS_FLAG_FUEL_PRESSURE_ABOVE_NOMINAL = 1024; // saturated uint30

public const double UAVCAN_EQUIPMENT_ICE_RECIPROCATING_STATUS_FLAG_DETONATION_SUPPORTED = 2048; // saturated uint30

public const double UAVCAN_EQUIPMENT_ICE_RECIPROCATING_STATUS_FLAG_DETONATION_OBSERVED = 4096; // saturated uint30

public const double UAVCAN_EQUIPMENT_ICE_RECIPROCATING_STATUS_FLAG_MISFIRE_SUPPORTED = 8192; // saturated uint30

public const double UAVCAN_EQUIPMENT_ICE_RECIPROCATING_STATUS_FLAG_MISFIRE_OBSERVED = 16384; // saturated uint30

public const double UAVCAN_EQUIPMENT_ICE_RECIPROCATING_STATUS_FLAG_OIL_PRESSURE_SUPPORTED = 32768; // saturated uint30

public const double UAVCAN_EQUIPMENT_ICE_RECIPROCATING_STATUS_FLAG_OIL_PRESSURE_BELOW_NOMINAL = 65536; // saturated uint30

public const double UAVCAN_EQUIPMENT_ICE_RECIPROCATING_STATUS_FLAG_OIL_PRESSURE_ABOVE_NOMINAL = 131072; // saturated uint30

public const double UAVCAN_EQUIPMENT_ICE_RECIPROCATING_STATUS_FLAG_DEBRIS_SUPPORTED = 262144; // saturated uint30

public const double UAVCAN_EQUIPMENT_ICE_RECIPROCATING_STATUS_FLAG_DEBRIS_DETECTED = 524288; // saturated uint30

public const double UAVCAN_EQUIPMENT_ICE_RECIPROCATING_STATUS_SPARK_PLUG_SINGLE = 0; // saturated uint3

public const double UAVCAN_EQUIPMENT_ICE_RECIPROCATING_STATUS_SPARK_PLUG_FIRST_ACTIVE = 1; // saturated uint3

public const double UAVCAN_EQUIPMENT_ICE_RECIPROCATING_STATUS_SPARK_PLUG_SECOND_ACTIVE = 2; // saturated uint3

public const double UAVCAN_EQUIPMENT_ICE_RECIPROCATING_STATUS_SPARK_PLUG_BOTH_ACTIVE = 3; // saturated uint3




public class uavcan_equipment_ice_reciprocating_Status: IUAVCANSerialize {



    public uint8_t state = new uint8_t();



    public uint32_t flags = new uint32_t();





    public uint8_t engine_load_percent = new uint8_t();



    public uint32_t engine_speed_rpm = new uint32_t();



    public Single spark_dwell_time_ms = new Single();



    public Single atmospheric_pressure_kpa = new Single();



    public Single intake_manifold_pressure_kpa = new Single();



    public Single intake_manifold_temperature = new Single();



    public Single coolant_temperature = new Single();



    public Single oil_pressure = new Single();



    public Single oil_temperature = new Single();



    public Single fuel_pressure = new Single();



    public Single fuel_consumption_rate_cm3pm = new Single();



    public Single estimated_consumed_fuel_volume_cm3 = new Single();



    public uint8_t throttle_position_percent = new uint8_t();



    public uint8_t ecu_index = new uint8_t();



    public uint8_t spark_plug_usage = new uint8_t();



    public uint8_t cylinder_status_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=16)] public uavcan_equipment_ice_reciprocating_CylinderStatus[] cylinder_status = new uavcan_equipment_ice_reciprocating_CylinderStatus[16];




public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_uavcan_equipment_ice_reciprocating_Status(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_uavcan_equipment_ice_reciprocating_Status(transfer, this);
}

};

}
}