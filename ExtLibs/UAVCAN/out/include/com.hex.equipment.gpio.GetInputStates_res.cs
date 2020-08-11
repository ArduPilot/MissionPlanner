
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

//using com.hex.equipment.gpio.InputStateArray.cs

public const int COM_HEX_EQUIPMENT_GPIO_GETINPUTSTATES_RES_MAX_PACK_SIZE = 161;
public const ulong COM_HEX_EQUIPMENT_GPIO_GETINPUTSTATES_RES_DT_SIG = 0x6147C4FB7586515E;
public const int COM_HEX_EQUIPMENT_GPIO_GETINPUTSTATES_RES_DT_ID = 225;



public class com_hex_equipment_gpio_GetInputStates_res: IUAVCANSerialize {
    public com_hex_equipment_gpio_InputStateArray input_state_array = new com_hex_equipment_gpio_InputStateArray();

public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_com_hex_equipment_gpio_GetInputStates_res(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_com_hex_equipment_gpio_GetInputStates_res(transfer, this);
}

};

}
}