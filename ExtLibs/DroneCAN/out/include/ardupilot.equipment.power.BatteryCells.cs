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
        public partial class ardupilot_equipment_power_BatteryCells: IDroneCANSerialize 
        {
            public const int ARDUPILOT_EQUIPMENT_POWER_BATTERYCELLS_MAX_PACK_SIZE = 51;
            public const ulong ARDUPILOT_EQUIPMENT_POWER_BATTERYCELLS_DT_SIG = 0x5C8B1ABD15890EA4;
            public const int ARDUPILOT_EQUIPMENT_POWER_BATTERYCELLS_DT_ID = 20012;

            public uint8_t voltages_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=24)] public Single[] voltages = Enumerable.Range(1, 24).Select(i => new Single()).ToArray();
            public uint16_t index = new uint16_t();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan = false)
            {
                encode_ardupilot_equipment_power_BatteryCells(this, chunk_cb, ctx, fdcan);
            }

            public void decode(CanardRxTransfer transfer, bool fdcan = false)
            {
                decode_ardupilot_equipment_power_BatteryCells(transfer, this, fdcan);
            }

            public static ardupilot_equipment_power_BatteryCells ByteArrayToDroneCANMsg(byte[] transfer, int startoffset, bool fdcan = false)
            {
                var ans = new ardupilot_equipment_power_BatteryCells();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()), fdcan);
                return ans;
            }
        }
    }
}