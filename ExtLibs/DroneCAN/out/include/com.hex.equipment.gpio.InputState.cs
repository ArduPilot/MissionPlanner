
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

namespace DroneCAN
{
    public partial class DroneCAN 
    {
        public partial class com_hex_equipment_gpio_InputState: IDroneCANSerialize 
        {
            public const int COM_HEX_EQUIPMENT_GPIO_INPUTSTATE_MAX_PACK_SIZE = 5;
            public const ulong COM_HEX_EQUIPMENT_GPIO_INPUTSTATE_DT_SIG = 0x3A6638F63439999E;

            public uint8_t input_idx = new uint8_t();
            public bool binary_input = new bool();
            public Single input_value = new Single();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
            {
                encode_com_hex_equipment_gpio_InputState(this, chunk_cb, ctx);
            }

            public void decode(CanardRxTransfer transfer)
            {
                decode_com_hex_equipment_gpio_InputState(transfer, this);
            }

            public static com_hex_equipment_gpio_InputState ByteArrayToDroneCANMsg(byte[] transfer, int startoffset)
            {
                var ans = new com_hex_equipment_gpio_InputState();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()));
                return ans;
            }
        }
    }
}