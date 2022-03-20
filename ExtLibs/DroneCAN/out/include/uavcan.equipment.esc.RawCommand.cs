
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
        public partial class uavcan_equipment_esc_RawCommand: IDroneCANSerialize 
        {
            public const int UAVCAN_EQUIPMENT_ESC_RAWCOMMAND_MAX_PACK_SIZE = 36;
            public const ulong UAVCAN_EQUIPMENT_ESC_RAWCOMMAND_DT_SIG = 0x217F5C87D7EC951D;
            public const int UAVCAN_EQUIPMENT_ESC_RAWCOMMAND_DT_ID = 1030;

            public uint8_t cmd_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=20)] public int16_t[] cmd = Enumerable.Range(1, 20).Select(i => new int16_t()).ToArray();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
            {
                encode_uavcan_equipment_esc_RawCommand(this, chunk_cb, ctx);
            }

            public void decode(CanardRxTransfer transfer)
            {
                decode_uavcan_equipment_esc_RawCommand(transfer, this);
            }

            public static uavcan_equipment_esc_RawCommand ByteArrayToDroneCANMsg(byte[] transfer, int startoffset)
            {
                var ans = new uavcan_equipment_esc_RawCommand();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()));
                return ans;
            }
        }
    }
}