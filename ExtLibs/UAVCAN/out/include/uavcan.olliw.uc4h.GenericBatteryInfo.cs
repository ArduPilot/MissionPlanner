
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


public const int UAVCAN_OLLIW_UC4H_GENERICBATTERYINFO_MAX_PACK_SIZE = 36;
public const ulong UAVCAN_OLLIW_UC4H_GENERICBATTERYINFO_DT_SIG = 0x4711AD8CBD503D5;
public const int UAVCAN_OLLIW_UC4H_GENERICBATTERYINFO_DT_ID = 28310;



public const double UAVCAN_OLLIW_UC4H_GENERICBATTERYINFO_STATUS_FLAG_ERROR_OVERVOLTAGE = 1; // saturated uint8
public const double UAVCAN_OLLIW_UC4H_GENERICBATTERYINFO_STATUS_FLAG_ERROR_UNDERVOLTAGE = 2; // saturated uint8
public const double UAVCAN_OLLIW_UC4H_GENERICBATTERYINFO_STATUS_FLAG_ERROR_OVERCURRENT = 4; // saturated uint8
public const double UAVCAN_OLLIW_UC4H_GENERICBATTERYINFO_STATUS_FLAG_ERROR_UNDERCURRENT = 8; // saturated uint8

public class uavcan_olliw_uc4h_GenericBatteryInfo: IUAVCANSerialize {
    public uint16_t battery_id = new uint16_t();
    public Single voltage = new Single();
    public Single current = new Single();
    public Single charge_consumed_mAh = new Single();
    public Single energy_consumed_Wh = new Single();
    public uint8_t status_flags = new uint8_t();
    public uint8_t cell_voltages_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=12)] public Single[] cell_voltages = new Single[12];

public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_uavcan_olliw_uc4h_GenericBatteryInfo(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_uavcan_olliw_uc4h_GenericBatteryInfo(transfer, this);
}

};

}
}