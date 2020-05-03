
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


public const int UAVCAN_EQUIPMENT_POWER_PRIMARYPOWERSUPPLYSTATUS_MAX_PACK_SIZE = 6;
public const ulong UAVCAN_EQUIPMENT_POWER_PRIMARYPOWERSUPPLYSTATUS_DT_SIG = 0xBBA05074AD757480;
public const int UAVCAN_EQUIPMENT_POWER_PRIMARYPOWERSUPPLYSTATUS_DT_ID = 1090;



public class uavcan_equipment_power_PrimaryPowerSupplyStatus: IUAVCANSerialize {
    public Single hours_to_empty_at_10sec_avg_power = new Single();
    public Single hours_to_empty_at_10sec_avg_power_variance = new Single();
    public bool external_power_available = new bool();
    public uint8_t remaining_energy_pct = new uint8_t();
    public uint8_t remaining_energy_pct_stdev = new uint8_t();

public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_uavcan_equipment_power_PrimaryPowerSupplyStatus(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_uavcan_equipment_power_PrimaryPowerSupplyStatus(transfer, this);
}

};

}
}