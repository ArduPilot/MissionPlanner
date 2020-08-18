
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

//using com.hex.equipment.gpio.InputState.cs

public const int COM_HEX_EQUIPMENT_GPIO_INPUTSTATEARRAY_MAX_PACK_SIZE = 161;
public const ulong COM_HEX_EQUIPMENT_GPIO_INPUTSTATEARRAY_DT_SIG = 0xE4C758178F4D1A45;
public const int COM_HEX_EQUIPMENT_GPIO_INPUTSTATEARRAY_DT_ID = 42455;



public class com_hex_equipment_gpio_InputStateArray: IUAVCANSerialize {
    public uint8_t input_states_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=32)] public com_hex_equipment_gpio_InputState[] input_states = Enumerable.Range(1, 32).Select(i => new com_hex_equipment_gpio_InputState()).ToArray();

public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_com_hex_equipment_gpio_InputStateArray(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_com_hex_equipment_gpio_InputStateArray(transfer, this);
}

};

}
}