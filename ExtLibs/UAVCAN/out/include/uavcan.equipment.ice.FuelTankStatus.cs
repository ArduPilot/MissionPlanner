

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




public const int UAVCAN_EQUIPMENT_ICE_FUELTANKSTATUS_MAX_PACK_SIZE = 13;
public const ulong UAVCAN_EQUIPMENT_ICE_FUELTANKSTATUS_DT_SIG = 0x286B4A387BA84BC4;

public const int UAVCAN_EQUIPMENT_ICE_FUELTANKSTATUS_DT_ID = 1129;






public class uavcan_equipment_ice_FuelTankStatus: IUAVCANSerialize {





    public uint8_t available_fuel_volume_percent = new uint8_t();



    public Single available_fuel_volume_cm3 = new Single();



    public Single fuel_consumption_rate_cm3pm = new Single();



    public Single fuel_temperature = new Single();



    public uint8_t fuel_tank_id = new uint8_t();




public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_uavcan_equipment_ice_FuelTankStatus(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_uavcan_equipment_ice_FuelTankStatus(transfer, this);
}

};

}
}