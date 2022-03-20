
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
//using uavcan.equipment.indication.SingleLightCommand.cs
        public partial class uavcan_equipment_indication_LightsCommand: IDroneCANSerialize 
        {
            public const int UAVCAN_EQUIPMENT_INDICATION_LIGHTSCOMMAND_MAX_PACK_SIZE = 61;
            public const ulong UAVCAN_EQUIPMENT_INDICATION_LIGHTSCOMMAND_DT_SIG = 0x2031D93C8BDD1EC4;
            public const int UAVCAN_EQUIPMENT_INDICATION_LIGHTSCOMMAND_DT_ID = 1081;

            public uint8_t commands_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=20)] public uavcan_equipment_indication_SingleLightCommand[] commands = Enumerable.Range(1, 20).Select(i => new uavcan_equipment_indication_SingleLightCommand()).ToArray();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
            {
                encode_uavcan_equipment_indication_LightsCommand(this, chunk_cb, ctx);
            }

            public void decode(CanardRxTransfer transfer)
            {
                decode_uavcan_equipment_indication_LightsCommand(transfer, this);
            }

            public static uavcan_equipment_indication_LightsCommand ByteArrayToDroneCANMsg(byte[] transfer, int startoffset)
            {
                var ans = new uavcan_equipment_indication_LightsCommand();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()));
                return ans;
            }
        }
    }
}