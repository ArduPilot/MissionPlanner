
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

//using uavcan.equipment.actuator.Command.cs

public const int UAVCAN_EQUIPMENT_ACTUATOR_ARRAYCOMMAND_MAX_PACK_SIZE = 61;
public const ulong UAVCAN_EQUIPMENT_ACTUATOR_ARRAYCOMMAND_DT_SIG = 0xD8A7486238EC3AF3;
public const int UAVCAN_EQUIPMENT_ACTUATOR_ARRAYCOMMAND_DT_ID = 1010;



public class uavcan_equipment_actuator_ArrayCommand: IUAVCANSerialize {
    public uint8_t commands_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=15)] public uavcan_equipment_actuator_Command[] commands = new uavcan_equipment_actuator_Command[15];

public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_uavcan_equipment_actuator_ArrayCommand(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_uavcan_equipment_actuator_ArrayCommand(transfer, this);
}

};

}
}