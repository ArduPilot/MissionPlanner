
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


public const int UAVCAN_EQUIPMENT_POWER_BATTERYINFO_MAX_PACK_SIZE = 55;
public const ulong UAVCAN_EQUIPMENT_POWER_BATTERYINFO_DT_SIG = 0x249C26548A711966;
public const int UAVCAN_EQUIPMENT_POWER_BATTERYINFO_DT_ID = 1092;



public const double UAVCAN_EQUIPMENT_POWER_BATTERYINFO_STATUS_FLAG_IN_USE = 1; // saturated uint11
public const double UAVCAN_EQUIPMENT_POWER_BATTERYINFO_STATUS_FLAG_CHARGING = 2; // saturated uint11
public const double UAVCAN_EQUIPMENT_POWER_BATTERYINFO_STATUS_FLAG_CHARGED = 4; // saturated uint11
public const double UAVCAN_EQUIPMENT_POWER_BATTERYINFO_STATUS_FLAG_TEMP_HOT = 8; // saturated uint11
public const double UAVCAN_EQUIPMENT_POWER_BATTERYINFO_STATUS_FLAG_TEMP_COLD = 16; // saturated uint11
public const double UAVCAN_EQUIPMENT_POWER_BATTERYINFO_STATUS_FLAG_OVERLOAD = 32; // saturated uint11
public const double UAVCAN_EQUIPMENT_POWER_BATTERYINFO_STATUS_FLAG_BAD_BATTERY = 64; // saturated uint11
public const double UAVCAN_EQUIPMENT_POWER_BATTERYINFO_STATUS_FLAG_NEED_SERVICE = 128; // saturated uint11
public const double UAVCAN_EQUIPMENT_POWER_BATTERYINFO_STATUS_FLAG_BMS_ERROR = 256; // saturated uint11
public const double UAVCAN_EQUIPMENT_POWER_BATTERYINFO_STATUS_FLAG_RESERVED_A = 512; // saturated uint11
public const double UAVCAN_EQUIPMENT_POWER_BATTERYINFO_STATUS_FLAG_RESERVED_B = 1024; // saturated uint11
public const double UAVCAN_EQUIPMENT_POWER_BATTERYINFO_STATE_OF_HEALTH_UNKNOWN = 127; // saturated uint7

public class uavcan_equipment_power_BatteryInfo: IUAVCANSerialize {
    public Single temperature = new Single();
    public Single voltage = new Single();
    public Single current = new Single();
    public Single average_power_10sec = new Single();
    public Single remaining_capacity_wh = new Single();
    public Single full_charge_capacity_wh = new Single();
    public Single hours_to_full_charge = new Single();
    public uint16_t status_flags = new uint16_t();
    public uint8_t state_of_health_pct = new uint8_t();
    public uint8_t state_of_charge_pct = new uint8_t();
    public uint8_t state_of_charge_pct_stdev = new uint8_t();
    public uint8_t battery_id = new uint8_t();
    public uint32_t model_instance_id = new uint32_t();
    public uint8_t model_name_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=31)] public uint8_t[] model_name = new uint8_t[31];

public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_uavcan_equipment_power_BatteryInfo(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_uavcan_equipment_power_BatteryInfo(transfer, this);
}

};

}
}